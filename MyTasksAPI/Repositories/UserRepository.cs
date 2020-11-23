using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyTasksAPI.Database;
using MyTasksAPI.Models;
using MyTasksAPI.Repositories.Contracts;

namespace MyTasksAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _context;

        public UserRepository(UserManager<ApplicationUser> context)
        {
            _context = context;
        }

        public async Task CreateAsync(ApplicationUser user, string senha)
        {
            var result = await _context.CreateAsync(user, senha);
            if(!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var erro in result.Errors)
                {
                    sb.AppendLine(erro.Description);
                }
                
                throw new Exception($"Usuário não cadastrado! {sb.ToString()}");
            }
        }

        public async Task<ApplicationUser> FindAsync(string email, string senha)
        {
            var user = await _context.FindByEmailAsync(email);
            if(await _context.CheckPasswordAsync(user, senha))
            {
                return user;
            }
            else
            {
                throw new Exception("Usuário não encontrado!");
            }
        }
    }
}