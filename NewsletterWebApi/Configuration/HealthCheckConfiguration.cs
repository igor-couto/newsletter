using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NewsletterSDK.Configuration;

namespace NewsletterWebApi.Configuration;

public static class HealthCheckConfiguration
{
    private static readonly string[] databaseTags = [ "db", "sql", "postgres" ];

    public static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        using var serviceProvider = services.BuildServiceProvider();
        var connectionStringsOptions = serviceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;
        
        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("HealthCheckConfiguration");

        connectionStringsOptions.LogConnectionStringDetails(logger);
    
        services
            .AddHealthChecks()
            .AddCheck("Web API", () => HealthCheckResult.Healthy("The web api application is up and running"))
            .AddNpgSql(
                connectionString: connectionStringsOptions.DefaultConnection,
                name: "PostgreSQL Database",
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(5),
                tags: databaseTags);

        return services;    
    }

    public static void UseHealthCheckConfiguration(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Could use app.MapHealthChecks here but it won't be on swagger 
        app.MapGet("/api/health", async (HealthCheckService healthCheckService, ILogger<HealthCheckService > logger) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = GetDescription(e.Key, e.Value, logger)
                })
            };

            return Results.Ok(response);
        })
        .WithTags("Application Settings")
        .WithName("HealthStatus")
        .WithSummary("Retrieves the application's health status.")
        .WithDescription("Returns the overall health state of the application and its registered dependencies. It has a 1 minute cache response")
        .Produces(StatusCodes.Status200OK)
        .WithOpenApi();
    }

    private static string GetDescription(string name, HealthReportEntry healthReportEntry, ILogger logger)
    {
        if(healthReportEntry.Status is HealthStatus.Healthy)
            return healthReportEntry.Description?? "The system is operating normally and is fully functional.";

        if(healthReportEntry.Status is HealthStatus.Degraded)
            return healthReportEntry.Description?? "The system is functioning but may be experiencing performance issues or partial outages.";

        if(healthReportEntry.Status is HealthStatus.Unhealthy)
        {
            logger.LogError("Unhealthy {Name} system detected. Error: {Description}", name, healthReportEntry.Description);
            return "The system is not functioning correctly and requires immediate attention.";
        }

        return "Unknown health status.";
    }
}