using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace NewsletterWebApi.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddEndpointsApiExplorer();

        using var serviceProvider = services.BuildServiceProvider();
        var version = serviceProvider.GetRequiredService<IOptions<VersionOptions>>().Value;

        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc($"v{version.Major}.{version.Minor}",
                new OpenApiInfo
                {
                    Title = $"Newsletter API v{version.Major}.{version.Minor}",
                    Description = "A simple newsletter application.",
                    Version = $"v{version.Major}.{version.Minor}",
                    License = new OpenApiLicense { Name = "GNU Affero General Public License", Url = new Uri("https://github.com/igor-couto/newsletter/blob/main/LICENCE") },
                    Contact = new OpenApiContact
                    {
                        Name = "Igor Couto",
                        Email = "igor.fcouto@gmail.com",
                        Url = new Uri("https://github.com/igor-couto/newsletter")
                    }
                });

            swaggerGenOptions.AddSecurityDefinition("API Key", new OpenApiSecurityScheme() 
            { 
                Description = "The API Key header is needed to access the endpoints. Example: X-Api-Key: AIzaSyDaGmWKa4JsXZ-HjGw7ISLn_3namBGewQe. When using swagger UI insert only the value of the API Key in the field bellow.",
                In = ParameterLocation.Header, 
                Name = "X-Api-Key",
                Type = SecuritySchemeType.ApiKey, 
                Scheme = "ApiKeyScheme"
            });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement 
            { 
                { 
                    new OpenApiSecurityScheme 
                    { 
                        Reference = new OpenApiReference 
                        { 
                            Type = ReferenceType.SecurityScheme, 
                            Id = "API Key"
                        } 
                    },
                    Array.Empty<string>()
                } 
            });

            swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "NewsletterWebApi.xml"));
        });

        return services;
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var version = app.ApplicationServices.GetRequiredService<IOptions<VersionOptions>>().Value;
        var title = $"Newsletter API v{version.Major}.{version.Minor}";

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "newsletter-api/api-docs/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = title;
            options.SwaggerEndpoint($"/newsletter-api/api-docs/v{version.Major}.{version.Minor}/swagger.json", title);
            options.RoutePrefix = "api/swagger";
            options.DefaultModelsExpandDepth(-1);
            options.DocExpansion(DocExpansion.None);
            options.DisplayRequestDuration();
        });
    }
}
