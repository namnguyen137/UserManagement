using UserManagement.CQRS.Interfaces;
using UserManagement.Repositories;

namespace UserManagement.CQRS.Commands;

public record DeleteUserCommand(Guid Id, bool Hard) : ICommand<bool>;

public class DeleteUserHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(IUserRepository repo, ILogger<DeleteUserHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(DeleteUserCommand command, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(command.Id);
        if (existing == null) return false;

        if (command.Hard)
        {
            var result = await _repo.DeleteAsync(command.Id);
            if (result) _logger.LogWarning("Hard-deleted user {UserId}", command.Id);
            return result;
        }
        else
        {
            await _repo.SoftDeleteAsync(command.Id);
            _logger.LogInformation("Soft-deleted user {UserId}", command.Id);
            return true;
        }
    }
}