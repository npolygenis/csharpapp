using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CSharpApp.Core.Settings;
using CSharpApp.Core.Interfaces;
using CSharpApp.Infrastructure.Security;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
    public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}