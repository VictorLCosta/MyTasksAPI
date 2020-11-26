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

        public UserController(IUserRepository repository, SignInManager<ApplicationUser> manager)
        {
            _repository = repository;
            _manager = manager;
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
    }
}