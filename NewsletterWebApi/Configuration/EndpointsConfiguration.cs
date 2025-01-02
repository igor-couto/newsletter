using NewsletterWebApi.Endpoints;

namespace NewsletterWebApi.Configuration;

public static class EndpointsConfiguration
{
    public static void UseEndpoints(this IEndpointRouteBuilder app) 
    {
        ArgumentNullException.ThrowIfNull(app);

        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList()
            .ForEach(endpointInstance => endpointInstance.MapEndpoints(app));
    }
}