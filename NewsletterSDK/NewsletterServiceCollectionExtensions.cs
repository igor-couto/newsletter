using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsletterSDK.Configuration;

namespace NewsletterSDK;

public static class NewsletterServiceCollectionExtensions
{
    public static IServiceCollection AddNewletterSDKServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration == null)
        {
            var assemblyLocation = typeof(NewsletterServiceCollectionExtensions).Assembly.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(assemblyDirectory)
                .AddJsonFile("appsettings.sdk.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
            
            configuration = configBuilder.Build();
        }

        services
            .AddOptions(configuration)
            .AddDatabase(configuration)
            .AddDependencyInjection();

        return services;
    }
}