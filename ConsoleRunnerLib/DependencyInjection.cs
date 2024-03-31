using Microsoft.Extensions.DependencyInjection;

namespace ConsoleRunnerLib;

public static class DependencyInjection
{
    public static IServiceCollection AddConsoleRunnerLib(this IServiceCollection services)
    {
        services
            .AddScoped<IConsoleLogger,ConsoleLogger>()
            .AddTransient<IConsoleRunner, ConsoleRunner>();

        return services;
    }

    public static IConsoleLogger GetConsoleLogger(this IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredService<IConsoleLogger> ();

    public static IConsoleRunner GetConsoleRunner(this IServiceProvider serviceProvider) =>
    serviceProvider.GetRequiredService<IConsoleRunner>();


}

