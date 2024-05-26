using AtonTest.Core;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Options;
using AtonTest.Infrastructure.Database;
using pomotracker.Core.Options;

namespace pomotracker.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReadonlyUserRepository, ReadonlyUserRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        return services;
    }
    
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration _configuration)
    {
        services.Configure<AtonTestServiceOptions>(_configuration.GetSection(nameof(AtonTestServiceOptions)));
        services.Configure<DatabaseOptions>(_configuration.GetSection(nameof(DatabaseOptions)));
        return services;
    }
}