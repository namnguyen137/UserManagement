using FluentAssertions;
using UserManagement.Utils;

namespace UserManagement.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_And_Verify_Works()
        {
            var hasher = new PasswordHasher();
            var hash = hasher.HashPassword("S3cret!!");
            hash.Should().NotBeNullOrEmpty();
            hasher.VerifyPassword("S3cret!!", hash).Should().BeTrue();
            hasher.VerifyPassword("wrong", hash).Should().BeFalse();
        }
    }
}
