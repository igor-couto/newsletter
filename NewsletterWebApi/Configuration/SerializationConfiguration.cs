using System.Text.Json.Serialization; 
using Microsoft.AspNetCore.Http.Json;

namespace NewsletterWebApi.Configuration;

public static class SerializationConfiguration
{
    public static IServiceCollection AddSerializationConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }
}