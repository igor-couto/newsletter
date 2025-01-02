namespace NewsletterWebApi.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhostOrigins",
                builder => builder
                    .WithOrigins("http://localhost", "https://localhost", "http://localhost:5000", "http://localhost:50010")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        return services;
    }

    public static void UseCorsConfiguration(this WebApplication app) 
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseCors("AllowLocalhostOrigins");
    }
}