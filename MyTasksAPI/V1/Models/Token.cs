using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTasksAPI.V1.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Used { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}