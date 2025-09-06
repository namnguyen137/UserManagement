using UserManagement.Models;

namespace UserManagement.Repositories
{
    public interface IUserRepository
    {
        Task<(IEnumerable<User> Items, int Total)> SearchAsync(string? search, int page, int pageSize);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SoftDeleteAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
