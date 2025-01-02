using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace NewsletterWebApi.Configuration;

public static class CompressionConfiguration
{
    public static IServiceCollection AddCompression(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.EnableForHttps = true;
        });

        return services;
    }

    public static void UseCompression(this IApplicationBuilder app) 
    {
        ArgumentNullException.ThrowIfNull(app);
        app.UseResponseCompression();
    }
}