using UserManagement.Dtos;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public UserService(IUserRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<UserResponse> CreateAsync(UserCreateRequest req)
        {
            var existing = await _repo.GetByEmailAsync(req.Email);
            if (existing != null) throw new DuplicateEmailException();

            var user = new User
            {
                Name = req.Name.Trim(),
                Title = string.IsNullOrWhiteSpace(req.Title) ? null : req.Title!.Trim(),
                Email = req.Email.Trim(),
                PasswordHash = _hasher.HashPassword(req.Password)
            };
            await _repo.AddAsync(user);
            return UserResponse.FromModel(user);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            await _repo.SoftDeleteAsync(id);
            return true;
        }

        public async Task<PagedResult<UserResponse>> SearchAsync(string? search, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
            var (items, total) = await _repo.SearchAsync(search, page, pageSize);
            return new PagedResult<UserResponse>
            {
                Items = items.Select(UserResponse.FromModel).ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<UserResponse?> UpdateAsync(Guid id, UserUpdateRequest req)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // if changing email, ensure unique
            if (!string.Equals(existing.Email, req.Email, StringComparison.OrdinalIgnoreCase))
            {
                var byEmail = await _repo.GetByEmailAsync(req.Email);
                if (byEmail != null && byEmail.Id != id) throw new DuplicateEmailException();
            }

            existing.Name = req.Name.Trim();
            existing.Title = string.IsNullOrWhiteSpace(req.Title) ? null : req.Title!.Trim();
            existing.Email = req.Email.Trim();

            await _repo.UpdateAsync(existing);
            return UserResponse.FromModel(existing);
        }
    }

    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException() : base("Email already exists") { }
    }
}
