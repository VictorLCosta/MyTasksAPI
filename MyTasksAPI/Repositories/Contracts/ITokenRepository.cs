using System.Threading.Tasks;
using MyTasksAPI.Models;

namespace MyTasksAPI.Repositories.Contracts
{
    public interface ITokenRepository
    {
        Task Create(Token token);
        Task GetToken(string refreshToken);
        Task Update(Token token);
    }
}