using System;

namespace MyTasksAPI.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateHour { get; set; }
        public string Local { get; set; }
        public string Description { get; set; }
        public string Tipo { get; set; }
        public bool Done { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}