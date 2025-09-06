using UserManagement.CQRS.Interfaces;
using UserManagement.Dtos;
using UserManagement.Repositories;

namespace UserManagement.CQRS.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserResponse?>;

public class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUserRepository _repo;

    public GetUserByIdHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserResponse?> HandleAsync(GetUserByIdQuery query, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(query.Id);
        return u == null ? null : UserResponse.FromModel(u);
    }
}