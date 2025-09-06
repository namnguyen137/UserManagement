using FluentAssertions;
using UserManagement.Dtos;
using UserManagement.Repositories;
using UserManagement.Services;
using UserManagement.Utils;

namespace UserManagement.Tests
{
    public class PagingLogicTests
    {
        [Fact]
        public async Task Should_Return_Correct_Page()
        {
            var repo = new InMemoryUserRepository();
            var hasher = new PasswordHasher();
            var svc = new UserService(repo, hasher);

            for (int i = 1; i <= 12; i++)
            {
                await svc.CreateAsync(new UserCreateRequest { Name = $"User {i}", Email = $"u{i}@test.com", Password = "Passw0rd!" });
            }

            var page2 = await svc.SearchAsync(null, page: 2, pageSize: 5);
            page2.Items.Should().HaveCount(5);
            page2.Page.Should().Be(2);
            page2.PageSize.Should().Be(5);
            page2.Total.Should().Be(12);
        }
    }
}
