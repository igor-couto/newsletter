using Microsoft.Extensions.Options;

namespace NewsletterWebApi.Configuration;

public static class ApiKeyConfiguration
{
    public static TBuilder RequireApiKey<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddEndpointFilterFactory((endpointContext, next) =>
        {
            return async invocationContext =>
            {
                var httpContext = invocationContext.HttpContext;

                var apiKey = httpContext.RequestServices.GetRequiredService<IOptions<AuthenticationOptions>>().Value.ApiKey;

                var logger = httpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ApiKeyConfiguration");

                if (!httpContext.Request.Headers.TryGetValue("X-Api-Key", out var providedKey) ||
                    providedKey != apiKey)
                {
                    logger.LogWarning("Unauthorized request attempt");
                    return Results.Unauthorized();
                }

                return await next(invocationContext);
            };
        });

        return builder;
    }
}