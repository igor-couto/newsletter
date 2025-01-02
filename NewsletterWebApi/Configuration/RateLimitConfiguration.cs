using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace NewsletterWebApi.Configuration;

public static class RateLimitConfiguration
{
    public static IServiceCollection AddRateLimit(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("RateLimitConfiguration");

        var EnableRateLimit = serviceProvider.GetRequiredService<IOptions<ConfigurationOptions>>().Value.EnableRateLimit;

        if (EnableRateLimit)
            logger.LogInformation("Rate limiting is enabled for the Web API.");
        else
            logger.LogInformation("Rate limiting is disabled for the Web API.");

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = (int) HttpStatusCode.TooManyRequests;

            options.AddPolicy("Three requests per second per host", httpContext => {

                if(EnableRateLimit is false)
                    return RateLimitPartition.GetNoLimiter("Three requests per second per host");

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Request.Headers.Host.ToString(),
                    factory:
                        partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 3,
                            Window = TimeSpan.FromSeconds(1)
                        }
                    );
            });

            options.AddPolicy("One request every 5 seconds per host", httpContext => {

                if(EnableRateLimit is false)
                    return RateLimitPartition.GetNoLimiter("One request every 5 seconds per host");

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Request.Headers.Host.ToString(),
                    factory:
                        partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 1,
                            Window = TimeSpan.FromSeconds(3)
                        }
                    );
            });
        });

        return services;
    }

    public static void UseRateLimit(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseRateLimiter();
    }
}