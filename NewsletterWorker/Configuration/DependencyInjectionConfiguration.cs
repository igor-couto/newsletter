using NewsletterSDK;
using NewsletterWorker.Services;

namespace NewsletterWorker.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddNewletterSDKServices();
        
        services.AddScoped<NewsletterEmailService>();
        services.AddSingleton<EmailSender>();

        return services;
    }
}