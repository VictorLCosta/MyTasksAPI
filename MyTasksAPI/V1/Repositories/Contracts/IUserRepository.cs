using MyTasksAPI.V1.Models; 
using System.Threading.Tasks;
using System.Text;

namespace MyTasksAPI.V1.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindAsync(string email, string password);
        Task<ApplicationUser> FindAsync(string id);
    }
}