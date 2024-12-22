using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NewsletterSDK.Configuration;

public static class OptionsConfiguration
{
    private static readonly string[] connectionStringRequiredFields = ["Server", "Database", "Port", "User Id", "Password"];

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        using var tempServiceProvider = services.BuildServiceProvider();

        var logger = tempServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("OptionsConfiguration");

        var connectionStringsOptions = configuration.GetSection("ConnectionStrings"); 
        
        if(string.IsNullOrEmpty(connectionStringsOptions.GetValue<string>("DefaultConnection")))
        {
            throw new OptionsValidationException(nameof(ConnectionStringsOptions), typeof(ConnectionStringsOptions), ["The database connection string is missing"]);
        }

        services.AddOptions<ConnectionStringsOptions>()
            .Bind(connectionStringsOptions)
            .Validate(
                connectionStringsOptions  => 
                {
                    var builder = new DbConnectionStringBuilder { ConnectionString = connectionStringsOptions.DefaultConnection };

                    foreach (var key in connectionStringRequiredFields)
                    {
                        if (!builder.ContainsKey(key))
                        {
                            logger.LogCritical("Configuration Error: The connection string is missing the '{key}' parameter.", key);
                            return false;
                        }

                        var value = builder[key]?.ToString();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            logger.LogCritical("Configuration Error: The '{key}' parameter in the connection string cannot be empty.", key);
                            return false;
                        }
                    }
                    return true;
                },
                "Configuration Error: The database connection string is in an invalid format. Please ensure that the 'ConnectionString__DefaultConnection' setting is well defined."
            )
            .ValidateOnStart();

        return services;    
    }
}

public class ConnectionStringsOptions
{
    public required string DefaultConnection { get; init; }

    public void LogConnectionStringDetails(ILogger logger)
    {
        var builder = new DbConnectionStringBuilder { ConnectionString = DefaultConnection };

        var server = builder["Server"].ToString();
        var database = builder["Database"].ToString();
        var port = builder["Port"].ToString();

        logger.LogInformation("Database Connection Details: Server {server} | Database {database} | Port {port}", server, database, port);
    }
}