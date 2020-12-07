using System;

namespace MyTasksAPI.Models.DTO
{
    public class DTOToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
    }
}