using UserManagement.CQRS.Interfaces;
using UserManagement.Dtos;
using UserManagement.Repositories;
using UserManagement.Services;

namespace UserManagement.CQRS.Commands;

public record UpdateUserCommand(Guid Id, UserUpdateRequest Request) : ICommand<UserResponse?>;

public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, UserResponse?>
{
    private readonly IUserRepository _repo;
    private readonly ILogger<UpdateUserCommand> _logger;

    public UpdateUserHandler(IUserRepository repo, ILogger<UpdateUserCommand> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<UserResponse?> HandleAsync(UpdateUserCommand command, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(command.Id);
        if (existing == null) return null;

        if (!string.Equals(existing.Email, command.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var byEmail = await _repo.GetByEmailAsync(command.Request.Email);
            if (byEmail != null && byEmail.Id != command.Id) throw new DuplicateEmailException();
        }

        existing.Name = command.Request.Name.Trim();
        existing.Title = string.IsNullOrWhiteSpace(command.Request.Title) ? null : command.Request.Title!.Trim();
        existing.Email = command.Request.Email.Trim();

        await _repo.UpdateAsync(existing);
        _logger.LogInformation("Updated user {UserId}", existing.Id);
        return UserResponse.FromModel(existing);
    }
}
