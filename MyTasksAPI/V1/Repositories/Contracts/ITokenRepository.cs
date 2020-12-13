using System.Threading.Tasks;
using MyTasksAPI.V1.Models;

namespace MyTasksAPI.V1.Repositories.Contracts
{
    public interface ITokenRepository
    {
        Task Create(Token token);
        Task<Token> GetToken(string refreshToken);
        Task Update(Token token);
    }
}