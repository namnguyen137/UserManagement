using UserManagement.Dtos;

namespace UserManagement.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> SignInAsync(SignInRequest req);
    }
}
