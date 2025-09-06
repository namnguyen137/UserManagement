namespace UserManagement.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
