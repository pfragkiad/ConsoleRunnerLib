using Microsoft.Extensions.DependencyInjection;

namespace ConsoleRunnerLib;

public static class DependencyInjection
{
    public static IServiceCollection AddConsoleRunnerLib(this IServiceCollection services)
    {
        services
            .AddScoped<ConsoleLogger>()
            .AddTransient<ConsoleRunner>();

        return services;
    }

}

