using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTasksAPI.Models;
using MyTasksAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTasksAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        private readonly UserManager<ApplicationUser> _user;

        public TaskController(ITaskRepository repo, UserManager<ApplicationUser> user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<IActionResult> Synchronizate([FromBody]List<Tasks> tasks)
        {
            return Ok(await _repo.Synchronization(tasks));
        }

        public async Task<IActionResult> Restore(DateTime date)
        {
            var user = await _user.GetUserAsync(HttpContext.User);
            return Ok(_repo.Restoration(user, date));
        }
    }
}