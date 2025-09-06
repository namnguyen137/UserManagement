using System.Collections.Concurrent;
using UserManagement.Models;

namespace UserManagement.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new();
        private readonly ConcurrentDictionary<string, Guid> _emailIndex = new(StringComparer.OrdinalIgnoreCase);

        public Task AddAsync(User user)
        {
            if (!_users.TryAdd(user.Id, user))
                throw new InvalidOperationException("Failed to add user");

            // ensure unique email among non-deleted users
            if (!_emailIndex.TryAdd(user.Email, user.Id))
            {
                _users.TryRemove(user.Id, out _);
                throw new InvalidOperationException("Duplicate email");
            }
            return Task.CompletedTask;
        }
        public Task<bool> DeleteAsync(Guid id)
        {
            if (_users.TryRemove(id, out var removed))
            {
                _emailIndex.TryRemove(removed.Email, out _);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            if (_emailIndex.TryGetValue(email, out var id) && _users.TryGetValue(id, out var u))
            {
                if (!u.IsDeleted) return Task.FromResult<User?>(u);
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            if (_users.TryGetValue(id, out var u) && !u.IsDeleted) return Task.FromResult<User?>(u);
            return Task.FromResult<User?>(null);
        }

        public Task<(IEnumerable<User> Items, int Total)> SearchAsync(string? search, int page, int pageSize)
        {
            var q = _users.Values.Where(u => !u.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(u => u.Name.Contains(s, StringComparison.OrdinalIgnoreCase)
                               || u.Email.Contains(s, StringComparison.OrdinalIgnoreCase));
            }
            var total = q.Count();
            var items = q
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(((IEnumerable<User>)items, total));
        }

        public Task SoftDeleteAsync(Guid id)
        {
            if (_users.TryGetValue(id, out var u) && !u.IsDeleted)
            {
                u.IsDeleted = true;
                u.DeletedAt = DateTimeOffset.UtcNow;
                // free up email for re-use
                _emailIndex.TryRemove(u.Email, out _);
            }
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            // handle email uniqueness
            if (_emailIndex.TryGetValue(user.Email, out var currentId) && currentId != user.Id)
            {
                throw new InvalidOperationException("Duplicate email");
            }

            _users.AddOrUpdate(user.Id, user, (id, old) =>
            {
                // if email changed, update index
                if (!string.Equals(old.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    _emailIndex.TryRemove(old.Email, out _);
                    _emailIndex[user.Email] = user.Id;
                }
                return user;
            });

            return Task.CompletedTask;
        }
    }
}
