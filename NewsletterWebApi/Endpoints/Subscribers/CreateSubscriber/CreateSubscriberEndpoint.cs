using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;

namespace NewsletterWebApi.Endpoints;

public class CreateSubscriberEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/subscribers", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<CreateSubscriberEndpoint> logger,
            [FromServices] ISubscriptionService subscriptionService,
            [FromBody] CreateSubscriberRequest createSubscriberRequest) =>
        {
            logger.LogInformation("Received request to create subscriber with Email: {Email}, Name: {Name}", 
                createSubscriberRequest.Email, createSubscriberRequest.Name);

            try
            {
                await subscriptionService.Subscribe(
                    createSubscriberRequest.Email, 
                    createSubscriberRequest.Name, 
                    cancellationToken);

                logger.LogInformation("Successfully created subscriber with Email: {Email}", createSubscriberRequest.Email);
                return Results.Created($"/api/subscribers/{createSubscriberRequest.Email}", new { createSubscriberRequest.Email });
            }
            catch (ArgumentException argumentException)
            {
                logger.LogWarning(argumentException, 
                    "Invalid argument(s) when creating subscriber with Email: {Email}. Error: {ErrorMessage}", 
                    createSubscriberRequest.Email, argumentException.Message);

                return Results.BadRequest(argumentException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, 
                    "Unhandled exception occurred while creating subscriber with Email: {Email}", 
                    createSubscriberRequest.Email);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError, 
                    title: "An unexpected error occurred while creating the subscriber."
                );
            }
        })
        .WithTags("Subscribers")
        .WithName("CreateSubscriber")
        .WithSummary("Registers a new subscriber to the newsletter's mailing list.")
        .WithDescription("This endpoint registers a new subscriber in the newsletter using the provided email and name.")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host");
    }
}
