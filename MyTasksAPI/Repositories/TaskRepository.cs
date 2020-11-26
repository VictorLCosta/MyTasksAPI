using MyTasksAPI.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTasksAPI.Models;
using MyTasksAPI.Repositories.Contracts;
using System.Linq;

namespace MyTasksAPI.Repositories
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
            var newTasks = tasks.Where(t => t.IdTaskApi == 0);

            if(newTasks.Count() > 0)
            {
                foreach(Tasks task in tasks)
                {
                    await _context.Tasks.AddAsync(task);
                }
            }

            var updatedTasks = tasks.Where(t => t.IdTaskApi != 0);

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