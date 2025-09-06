using System;

namespace UserManagement.Dtos
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        public static UserResponse FromModel(UserManagement.Models.User u) => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Title = u.Title,
            Email = u.Email,
            CreatedAt = u.CreatedAt
        };
    }
}
