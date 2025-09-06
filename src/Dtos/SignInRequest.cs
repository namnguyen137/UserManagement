using System.ComponentModel.DataAnnotations;

namespace UserManagement.Dtos
{
    public class SignInRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
