using System.ComponentModel.DataAnnotations;

namespace NewsletterWorker.Configuration;

public static class OptionsConfigurations
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("Email"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CronExpressionOptions>()
            .Bind(configuration.GetSection("CronSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}

public class EmailOptions 
{
    [Required]
    [EmailAddress]
    public required string SmtpEmail { get; init; }

    [Required]
    public required string SmtpHost { get; init; }

    [Required]
    public int SmtpPort { get; init; }

    [Required]
    public required string SmtpUserName { get; init; }

    [Required]
    [MinLength(6, ErrorMessage = "SmtpPassword must be at least 6 characters.")]
    public required string SmtpPassword { get; init; }

    [Range(1, 65535, ErrorMessage = "SmtpPort must be between 1 and 65535.")]
    public bool SmtpEnableSsl { get; init; }

    [Required]
    public required string DisplayName { get; init; }
}

public class CronExpressionOptions
{
    [Required]
    public required string CronExpression { get; set; }

    [Required]
    public required string CronDescription { get; set; }
}