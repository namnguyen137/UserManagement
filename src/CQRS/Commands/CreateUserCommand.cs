using UserManagement.CQRS.Interfaces;
using UserManagement.Dtos;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Services;
using UserManagement.Utils;

namespace UserManagement.CQRS.Commands;

public record CreateUserCommand(UserCreateRequest Request) : ICommand<UserResponse>;

public class CreateUserHandler : ICommandHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(IUserRepository repo, IPasswordHasher hasher, ILogger<CreateUserHandler> logger)
    {
        _repo = repo;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<UserResponse> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        var req = command.Request;
        var existing = await _repo.GetByEmailAsync(req.Email);
        if (existing != null) throw new DuplicateEmailException();

        var user = new User
        {
            Name = req.Name.Trim(),
            Title = string.IsNullOrWhiteSpace(req.Title) ? null : req.Title!.Trim(),
            Email = req.Email.Trim(),
            PasswordHash = _hasher.HashPassword(req.Password)
        };

        await _repo.AddAsync(user);
        _logger.LogInformation("Created user {Email}", user.Email);
        return UserResponse.FromModel(user);
    }
}

