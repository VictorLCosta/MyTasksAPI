using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyTasksAPI.Database;
using MyTasksAPI.V1.Models;
using MyTasksAPI.V1.Repositories.Contracts;

namespace MyTasksAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MyTasksContext _context;

        public TokenRepository(MyTasksContext context)
        {
            _context = context;
        }

        public async Task Create(Token token)
        {
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<Token> GetToken(string refreshToken)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.Used == false);
        }

        public async Task Update(Token token)
        {
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}