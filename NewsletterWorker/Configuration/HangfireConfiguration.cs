
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.Hangfire;
using NewsletterSDK.Configuration;

namespace NewsletterWorker.Configuration;

public static class HangfireConfiguration
{
    public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
     
        using var serviceProvider = services.BuildServiceProvider();
        var connectionString = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;

        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("HangfireConfiguration");

        services.Configure<HangfireOptions>(options =>
        {
            options.MinimumAvailableServers = 1;
            options.MaximumJobsFailed = 1;
        });

        connectionString.LogConnectionStringDetails(logger);

        services.AddHangfire(config => config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString.DefaultConnection)));
        services.AddHangfireServer();

        services.AddHealthChecks()
            .AddCheck<HangfireHealthCheck>("hangfire");

        return services;    
    }

    public static WebApplication UseHangfireConfiguration(this WebApplication app)
    {
        app.UseHangfireDashboard(pathMatch: "/hangfire");
        app.MapHealthChecks("/health");

        return app;
    }
}