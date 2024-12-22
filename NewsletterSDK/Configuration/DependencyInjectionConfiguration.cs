using Microsoft.Extensions.DependencyInjection;
using NewsletterSDK.Repositories;
using NewsletterSDK.Services;

namespace NewsletterSDK.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<PublicationRepository>();
        services.AddScoped<SubscriptionsRepository>();
        services.AddScoped<DeliveriesRepository>();

        services.AddScoped<IPublicationService, PublicationService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        return services;
    }
}