using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyTasksAPI.Models;
using MyTasksAPI.Models.DTO;
using MyTasksAPI.Repositories.Contracts;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;

namespace MyTasksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IConfiguration _conf;
        private readonly IUserRepository _repository;
        private readonly SignInManager<ApplicationUser> _manager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IConfiguration conf, IUserRepository repository, SignInManager<ApplicationUser> manager, UserManager<ApplicationUser> userManager)
        {
            _conf = conf;
            _repository = repository;
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
                    await _manager.SignInAsync(user, false);

                    return Ok(GenerateToken(user));
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

        public string GenerateToken(ApplicationUser user)
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
                Expires = DateTime.UtcNow.AddHours(10),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Issuer = null,
                Audience = null,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}