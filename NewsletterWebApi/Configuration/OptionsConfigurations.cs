using System.ComponentModel.DataAnnotations;

namespace NewsletterWebApi.Configuration;

public static class OptionsConfiguration
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuthenticationOptions>()
            .Bind(configuration.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<VersionOptions>()
            .Bind(configuration.GetSection("Version"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ConfigurationOptions>()
            .Bind(configuration.GetSection("Configuration"));

        return services;
    }
}

public class AuthenticationOptions
{
    [Required(ErrorMessage = "The API Key could not be found.")]
    public required string ApiKey { get; init; }
}

public class VersionOptions
{
    [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Major { get; init; } = 1;

    [Range(0, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Minor { get; init; } = 0;

    public void LogVersion(ILogger logger)
    {
        logger.LogInformation("Web API Version: v{Major}.{Minor}", Major, Minor);
    }
}

public class ConfigurationOptions
{
    public required bool EnableCache { get; set; } = true;
    public required bool EnableRateLimit { get; set; } = true;
}