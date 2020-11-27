using MyTasksAPI.Models; 
using System.Threading.Tasks;
using System.Text;

namespace MyTasksAPI.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindAsync(string email, string password);
    }
}