using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTasksAPI.V1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        [ForeignKey("UsuarioId")]
        public ICollection<Tasks> Tasks { get; set; }

        [ForeignKey("UsuarioId")]
        public ICollection<Token> Tokens { get; set; }
    }
}