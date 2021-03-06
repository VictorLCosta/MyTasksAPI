using System.ComponentModel.DataAnnotations;

namespace MyTasksAPI.V1.Models.DTO
{
    public class DTOUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Insira um e-mail válido")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string PasswordConfirm { get; set; }

    }
}