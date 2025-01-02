using Microsoft.Extensions.Options;

namespace NewsletterWebApi.Configuration;

public static class CacheConfiguration
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("CacheConfiguration");

        var enableCache = serviceProvider.GetRequiredService<IOptions<ConfigurationOptions>>().Value.EnableCache;

        if (enableCache)
            logger.LogInformation("Output caching is enabled for the Web API.");
        else
            logger.LogInformation("Output caching is disabled for the Web API.");

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder  => {
                if (enableCache) 
                    builder.SetVaryByHost(true);
                else 
                    builder.NoCache();
            });

            options.AddPolicy("Expire in 3 minutes", builder => {
                if (enableCache) 
                    builder.Expire(TimeSpan.FromMinutes(3));
                else 
                    builder.NoCache();
            });

            options.AddPolicy("Expire in 30 minutes", builder => {
                if (enableCache) 
                    builder.Expire(TimeSpan.FromMinutes(30));
                else 
                    builder.NoCache();
            });
        });

        return services;
    }

    public static void UseCache(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseOutputCache();
    }
}