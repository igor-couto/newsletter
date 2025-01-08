namespace NewsletterWorker.Configuration;

public static class LoggingConfiguration
{
    public static void AddLogging(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
    }
}