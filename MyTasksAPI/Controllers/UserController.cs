using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTasksAPI.Models;
using MyTasksAPI.Models.DTO;
using MyTasksAPI.Repositories.Contracts;

namespace MyTasksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly SignInManager<ApplicationUser> _manager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserRepository repository, SignInManager<ApplicationUser> manager, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _manager = manager;
            _userManager = userManager;
        }

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

                    return Ok();
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
    }
}