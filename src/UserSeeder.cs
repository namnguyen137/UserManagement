using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Utils;

namespace UserManagement;

public static class UserSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var users = new[]
        {
                new User { Name = "AAA", Title = "IT", Email = "AAA@test.com", PasswordHash = hasher.HashPassword("Passw0rd!") },
                new User { Name = "BBB", Title = "BA", Email = "BBB@test.com", PasswordHash = hasher.HashPassword("Passw0rd!") },
                new User { Name = "CCC", Title = "PM", Email = "CCC@test.com", PasswordHash = hasher.HashPassword("Passw0rd!") }
            };

        foreach (var u in users)
        {
            await repo.AddAsync(u);
        }
    }
}
