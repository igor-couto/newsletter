using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;

namespace NewsletterWebApi.Configuration;

public static class ApiVersioningConfiguration
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        using var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("ApiVersioningConfiguration");

        var version = serviceProvider.GetRequiredService<IOptions<VersionOptions>>().Value;

        version.LogVersion(logger);

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(version.Major, version.Minor);
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });

        services.AddEndpointsApiExplorer();

        return services;
    }

    public static void UseVersioning(this IEndpointRouteBuilder app) 
    {
        ArgumentNullException.ThrowIfNull(app);
        
        app.MapGet("/api/version", (HttpContext httpContext, [FromServices] IOptions<VersionOptions> version) =>
        {
            var apiVersionFeature = httpContext.Features.Get<IApiVersioningFeature>();
            var apiVersion = apiVersionFeature?.RequestedApiVersion ?? new ApiVersion(version.Value.Major, version.Value.Minor);

            return Results.Ok(new { version = apiVersion.ToString() });
        })
            .WithTags("Application Settings")
            .WithName("GetApiVersion")
            .WithSummary("Retrieves the current API version.")
            .WithDescription("Returns the current version of this Web API in the format v{Major}.{Minor}-{Status}")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .CacheOutput("Expire in 30 minutes");
    }
}