using UserManagement.CQRS.Interfaces;
using UserManagement.Dtos;
using UserManagement.Repositories;

namespace UserManagement.CQRS.Queries;

public record SearchUsersQuery(string? Search, int Page, int PageSize) : IQuery<PagedResult<UserResponse>>;

public class SearchUsersHandler : IQueryHandler<SearchUsersQuery, PagedResult<UserResponse>>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<SearchUsersHandler> _logger;

    public SearchUsersHandler(IUserRepository repo, ILogger<SearchUsersHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedResult<UserResponse>> HandleAsync(SearchUsersQuery query, CancellationToken ct = default)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);
        var (items, total) = await _repo.SearchAsync(query.Search, page, pageSize);

        _logger.LogDebug("Search users: '{Search}', page {Page}, size {Size}, total {Total}",
            query.Search, page, pageSize, total);

        return new PagedResult<UserResponse>
        {
            Items = items.Select(UserResponse.FromModel).ToList(),
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}