using System.ComponentModel.DataAnnotations;

namespace UserManagement.Dtos
{
    public class UserUpdateRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Title { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;
    }
}
