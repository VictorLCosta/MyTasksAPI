using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTasksAPI.V1.Models;

namespace MyTasksAPI.Database
{
    public class MyTasksContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Token> Tokens { get; set; }

        public MyTasksContext(DbContextOptions<MyTasksContext> options) 
            : base(options) 
        {
            
        }

        /*protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(t => t.Tokens);
        }*/
    }
}