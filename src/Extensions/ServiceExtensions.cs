using UserManagement.CQRS.Commands;
using UserManagement.CQRS.Interfaces;
using UserManagement.CQRS.Queries;
using UserManagement.Dtos;
using UserManagement.Repositories;
using UserManagement.Services;
using UserManagement.Utils;

namespace UserManagement.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();



        // CQRS handlers
        services.AddScoped<ICommandHandler<CreateUserCommand, UserResponse>, CreateUserHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UserResponse?>, UpdateUserHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserHandler>();
        services.AddScoped<IQueryHandler<SearchUsersQuery, PagedResult<UserResponse>>, SearchUsersHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserResponse?>, GetUserByIdHandler>();

        return services;
    }
}
