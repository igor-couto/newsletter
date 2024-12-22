using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using FluentMigrator.Runner;
using NewsletterSDK;
using NewsletterSDK.Configuration;
using Microsoft.Extensions.Logging;

var direction = args.Length > 0 ? args[0].ToLower() : "up";

var app = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {

        services.AddNewletterSDKServices();

        using var tempServiceProvider = services.BuildServiceProvider();
        var connectionString = tempServiceProvider.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value;

        var logger = tempServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ApiKeyConfiguration");

        logger.LogInformation("Initializing migration {direction} process.", direction);
        connectionString.LogConnectionStringDetails(logger);

        services.AddFluentMigratorCore()
            .ConfigureRunner(
                runnerBuilder =>
                {
                    runnerBuilder
                        .AddPostgres()
                        .WithGlobalConnectionString(connectionString.DefaultConnection)
                        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()
                        .For.All();
                })
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }).Build();

using var scope = app.Services.CreateScope();
var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();

if (direction == "down")
    migrator.MigrateDown(-1);

if (direction == "up")
    migrator.MigrateUp();