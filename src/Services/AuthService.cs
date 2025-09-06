using System.Text;
using UserManagement.Dtos;
using UserManagement.Repositories;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public AuthService(IUserRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<AuthResponse?> SignInAsync(SignInRequest req)
        {
            var user = await _repo.GetByEmailAsync(req.Email);
            if (user == null) return null;
            if (!_hasher.VerifyPassword(req.Password, user.PasswordHash)) return null;

            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes($"uid={user.Id};ts={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"));
            var token = $"FAKE.{payload}.VALID_TOKEN";
            return new AuthResponse { Token = token };
        }
    }
}
