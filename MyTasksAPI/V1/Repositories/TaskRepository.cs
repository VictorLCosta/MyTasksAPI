using MyTasksAPI.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyTasksAPI.V1.Repositories.Contracts;
using MyTasksAPI.V1.Models;

namespace MyTasksAPI.V1.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly MyTasksContext _context;

        public TaskRepository(MyTasksContext context)
        {
            _context = context;
        }

        public IEnumerable<Tasks> Restoration(ApplicationUser user, DateTime lastSyncDate)
        {
            var query = _context.Tasks.Where(t => t.User.Id == user.Id).AsQueryable();

            if(lastSyncDate != null)
            {
               query.Where(t => t.Created >= lastSyncDate || t.Updated >= lastSyncDate);
            }
            
            return query.AsEnumerable();
        }

        public async Task<List<Tasks>> Synchronization(List<Tasks> tasks)
        {
            var newTasks = tasks.Where(t => t.IdTaskApi == 0).ToList();
            var updatedTasks = tasks.Where(t => t.IdTaskApi != 0).ToList();

            if(newTasks.Count() > 0)
            {
                foreach(Tasks task in tasks)
                {
                    await _context.Tasks.AddAsync(task);
                }
            }

            if(updatedTasks.Count() > 0)
            {
                foreach(Tasks task in tasks)
                {
                    _context.Tasks.Update(task);
                }
            }

            await _context.SaveChangesAsync();
            return newTasks.ToList();
        }
    }
}