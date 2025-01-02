using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;
using NewsletterSDK.Models;

namespace NewsletterWebApi.Endpoints;

public class ReadAllUnsubscriptionsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/unsubscriptions", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<ReadAllUnsubscriptionsEndpoint> logger,
            [FromServices] ISubscriptionService subscriptionService) =>
        {
            logger.LogInformation("Received request to retrieve all unsubscribed users.");

            try
            {
                var unsubscriptions = await subscriptionService.GetAllUnsubscriptions(cancellationToken);

                logger.LogInformation("Successfully retrieved {Count} unsubscriptions.", unsubscriptions?.Count() ?? 0);
                return Results.Ok(unsubscriptions);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while retrieving unsubscriptions.");
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while retrieving the unsubscriptions."
                );
            }
        })
        .WithTags("Unsubscriptions")
        .WithName("GetAllUnsubscriptions")
        .WithSummary("Retrieves all unsubscriptions.")
        .WithDescription("Returns a list of all unsubscriptions with emails and reasons.")
        .WithOpenApi()
        .Produces<IEnumerable<Unsubscription>?>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host")
        .CacheOutput("Expire in 3 minutes")
        .RequireApiKey();
    }
}