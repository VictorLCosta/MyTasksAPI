using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyTasksAPI.V1.Models;
using MyTasksAPI.V1.Models.DTO;
using MyTasksAPI.V1.Repositories.Contracts;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;

namespace MyTasksAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private IConfiguration _conf;

        private readonly IUserRepository _repository;
        private readonly SignInManager<ApplicationUser> _manager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRepository _tokenRepo;

        public UserController(IConfiguration conf, IUserRepository repository, SignInManager<ApplicationUser> manager, UserManager<ApplicationUser> userManager, ITokenRepository tokenRepo)
        {
            _conf = conf;
            _repository = repository;
            _tokenRepo = tokenRepo;
            _manager = manager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]DTOUser _user)
        {
            if(_user == null)
            {
                return BadRequest();
            }

            ModelState.Remove("Name");
            ModelState.Remove("PasswordConfirm");

            if(ModelState.IsValid)
            {
                ApplicationUser user = await _repository.FindAsync(_user.Email, _user.Password);
                if(user != null)
                {
                    var token = GenerateToken(user);

                    var tokenModel = new Token
                    {
                        RefreshToken = token.RefreshToken,
                        ExpirationDate = token.ExpirationDate,
                        ExpirationRefreshToken = token.ExpirationRefreshToken,
                        User = user,
                        Created = DateTime.Now,
                        Used = false
                    };

                    await _tokenRepo.Create(tokenModel);
                    return Ok(token);
                }
                else
                {
                    return NotFound("Usuário não localizado!");
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        [HttpPost("Renovate")]
        public async Task<IActionResult> Renovate([FromBody]DTOToken _token)
        {
            var refreshToken = await _tokenRepo.GetToken(_token.RefreshToken);

            if(refreshToken == null)
            {
                return NotFound();
            }

            refreshToken.Updated = DateTime.Now;
            refreshToken.Used = true;
            await _tokenRepo.Update(refreshToken);

            //Gerando token
            var user = await _repository.FindAsync(refreshToken.UserId);

            return BuildNewToken(user);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody]DTOUser _user)
        {
            if(_user == null)
            {
                return BadRequest();
            }

            if(ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.FullName = _user.Name;
                user.Email = _user.Email;

                
                var result = await _userManager.CreateAsync(user, _user.Password);
                
                if(!result.Succeeded)
                {
                    List<string> sb = new List<string>();
                    foreach(IdentityError error in result.Errors)
                    {
                        sb.Add(error.Description);
                    }
                    return UnprocessableEntity(sb);
                }

                return Created($"api/[controller]/{user.Id}", user);
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        private DTOToken GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetValue<string>("Key")));

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Issuer = null,
                Audience = null,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            string tokenString = tokenHandler.WriteToken(token);
            var expirationToken = DateTime.UtcNow.AddDays(15);

            return new DTOToken { Token = tokenString, ExpirationDate = tokenDescriptor.Expires.Value, ExpirationRefreshToken = expirationToken };
        }

        private IActionResult BuildNewToken(ApplicationUser user)
        {
            var token = GenerateToken(user);

            //Salvar o Token no Banco
            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationDate = token.ExpirationDate,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                User = user,
                Created = DateTime.Now,
                Used = false
            };

            _tokenRepo.Create(tokenModel);
            return Ok(token);
        }
    }
}