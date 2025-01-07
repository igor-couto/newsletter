using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using FluentMigrator.Runner;
using NewsletterSDK;
using NewsletterSDK.Configuration;

namespace NewsletterMigrations.Services;

public class MigrationService
{
    public static void MigrateUp(string connectionString = null)
    {
        using var host = CreateHost(connectionString);
        using var scope = host.Services.CreateScope();

        var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrator.MigrateUp();
    }

    public static void MigrateDown(string connectionString = null)
    {
        using var host = CreateHost(connectionString);
        using var scope = host.Services.CreateScope();

        var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrator.MigrateDown(-1);
    }

    private static IHost CreateHost(string connectionString = null)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => 
            {
                services.AddNewletterSDKServices();

                using var serviceProvider = services.BuildServiceProvider();

                var logger = serviceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Migration");

                logger.LogInformation("Initializing migration process.");

                var connectionStringOptions = new ConnectionStringsOptions();
                if(connectionString is null)
                {
                    connectionStringOptions = serviceProvider
                        .GetRequiredService<IOptions<ConnectionStringsOptions>>()
                        .Value;
                }
                else
                {
                    connectionStringOptions.DefaultConnection = connectionString;
                }

                connectionStringOptions.LogConnectionStringDetails(logger);

                services.AddFluentMigratorCore()
                    .ConfigureRunner(rb =>
                    {
                        rb.AddPostgres()
                            .WithGlobalConnectionString(connectionStringOptions.DefaultConnection)
                            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()
                            .For.All();
                    })
                    .AddLogging(lb => lb.AddFluentMigratorConsole());
            })
            .Build();
    }
}