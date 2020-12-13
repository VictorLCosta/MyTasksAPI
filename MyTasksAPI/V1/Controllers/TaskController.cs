using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTasksAPI.V1.Models;
using MyTasksAPI.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTasksAPI.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _repo;
        private readonly UserManager<ApplicationUser> _user;

        public TaskController(ITaskRepository repo, UserManager<ApplicationUser> user)
        {
            _repo = repo;
            _user = user;
        }

        [Authorize]
        [HttpPost("synchronizate")]
        public async Task<IActionResult> Synchronizate([FromBody]List<Tasks> tasks)
        {
            return Ok(await _repo.Synchronization(tasks));
        }

        [Authorize]
        [HttpGet("restore")]
        public async Task<IActionResult> Restore(DateTime date)
        {
            var user = await _user.GetUserAsync(HttpContext.User);
            return Ok(_repo.Restoration(user, date));
        }
    }
}