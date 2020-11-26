using MyTasksAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTasksAPI.Repositories.Contracts
{
    public interface ITaskRepository
    {
        Task<List<Tasks>> Synchronization(List<Tasks> tasks);
        IEnumerable<Tasks> Restoration(ApplicationUser user, DateTime lastSyncDate);
    }
}