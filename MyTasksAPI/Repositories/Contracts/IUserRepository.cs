using MyTasksAPI.Models; 
using System.Threading.Tasks;

namespace MyTasksAPI.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task CreateAsync(ApplicationUser user, string senha);
        Task<ApplicationUser> FindAsync(string email, string senha);
    }
}