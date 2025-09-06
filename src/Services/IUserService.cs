using UserManagement.Dtos;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> SearchAsync(string? search, int page, int pageSize);
        Task<UserResponse> CreateAsync(UserCreateRequest req);
        Task<UserResponse?> UpdateAsync(Guid id, UserUpdateRequest req);
        Task<bool> DeleteAsync(Guid id);
    }
}
