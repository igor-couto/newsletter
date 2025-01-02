namespace NewsletterWebApi.Configuration;

public static class KestrelConfiguration
{
    public static WebApplicationBuilder AddKestrelConfiguration(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.WebHost.UseKestrel(option => option.AddServerHeader = false);
        return builder;
    }
}