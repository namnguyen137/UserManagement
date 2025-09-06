using FluentAssertions;
using UserManagement.Dtos;
using UserManagement.Repositories;
using UserManagement.Services;
using UserManagement.Utils;

namespace UserManagement.Tests
{
    public class DuplicateEmailTests
    {
        [Fact]
        public async Task Duplicate_Email_Should_Throw()
        {
            var repo = new InMemoryUserRepository();
            var hasher = new PasswordHasher();
            var svc = new UserService(repo, hasher);

            await svc.CreateAsync(new UserCreateRequest { Name = "A", Email = "a@test.com", Password = "Passw0rd!" });
            var act = async () => await svc.CreateAsync(new UserCreateRequest { Name = "B", Email = "a@test.com", Password = "Passw0rd!" });

            await act.Should().ThrowAsync<DuplicateEmailException>();
        }
    }
}
