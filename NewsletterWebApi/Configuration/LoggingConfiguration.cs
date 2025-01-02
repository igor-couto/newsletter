namespace NewsletterWebApi.Configuration;

public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        return builder;
    }
}