using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTasksAPI.V1.Models
{
    public class Tasks
    {
        [Key]
        public int IdTaskApi { get; set; }

        public int IdTaskApp { get; set; }
        public string Title { get; set; }
        public DateTime DateHour { get; set; }
        public string Local { get; set; }
        public string Description { get; set; }
        public string Tipo { get; set; }
        public bool Done { get; set; }
        public bool Removed { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        
        public virtual ApplicationUser User { get; set; }
    }
}