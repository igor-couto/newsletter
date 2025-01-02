using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;

namespace NewsletterWebApi.Endpoints;

public class ReadAllSubscribersEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/subscribers", async(
            CancellationToken cancellationToken,
            [FromServices] ILogger<ReadAllSubscribersEndpoint> logger,
            [FromServices] ISubscriptionService subscriptionService,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25) =>
        {
            logger.LogInformation("Received request to retrieve subscribers. Page: {Page}, Limit: {Limit}", page, limit);

            try
            {
                var searchResult = await subscriptionService.GetSubscribers(page, limit);

                var response = new SubscribersPaginatedReponse
                {
                    Subscribers = searchResult.subscribers,
                    Page = page,
                    Limit = limit,
                    Total = searchResult.total
                };

                logger.LogInformation("Successfully retrieved {Count} subscribers for page {Page} with limit {Limit}. Total: {Total}",
                    response.Subscribers.Count(), page, limit, response.Total);

                return Results.Ok(response);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "An error occurred while retrieving subscribers. Page: {Page}, Limit: {Limit}",
                    page, limit);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while retrieving the subscribers."
                );
            }
        })
        .WithTags("Subscribers")
        .WithName("GetAllSubscribers")
        .WithSummary("Retrieves paginated subscribers.")
        .WithDescription("Returns a paginated list of subscribers based on the specified page and limit.")
        .WithOpenApi()
        .Produces<SubscribersPaginatedReponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host")
        .CacheOutput("Expire in 3 minutes")
        .RequireApiKey();
    }
}
