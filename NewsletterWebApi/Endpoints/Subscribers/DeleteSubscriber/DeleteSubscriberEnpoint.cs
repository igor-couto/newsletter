using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;

namespace NewsletterWebApi.Endpoints;

public class DeleteSubscriberEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/subscribers", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<DeleteSubscriberEndpoint> logger,
            [FromServices] ISubscriptionService subscriptionService,
            [FromBody] UnsubcribeRequest unsubcribeRequest) =>
        {
            logger.LogInformation("Received request to unsubscribe email: {Email} with reason: {Reason}",
                unsubcribeRequest.Email, unsubcribeRequest.Reason);

            try
            {
                await subscriptionService.Unsubscribe(unsubcribeRequest.Email, unsubcribeRequest.Reason, cancellationToken);

                logger.LogInformation("Successfully unsubscribed email: {Email}", unsubcribeRequest.Email);
                return Results.Ok();
            }
            catch (ArgumentException argumentException)
            {
                logger.LogWarning(argumentException,
                    "Invalid request to unsubscribe. Email: {Email}, Reason: {Reason}, Error: {ErrorMessage}",
                    unsubcribeRequest.Email, unsubcribeRequest.Reason, argumentException.Message);

                return Results.BadRequest(argumentException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "An unexpected error occurred while unsubscribing email: {Email}",
                    unsubcribeRequest.Email);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while unsubscribing."
                );
            }
        })
        .WithTags("Subscribers")
        .WithName("DeleteSubscriber")
        .WithSummary("Deletes (unsubscribes) a subscriber.")
        .WithDescription("Removes the subscriber's email from the active subscriber list, optionally recording a reason for the unsubscription.")
        .WithOpenApi()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host")
        .RequireApiKey();
    }
}
