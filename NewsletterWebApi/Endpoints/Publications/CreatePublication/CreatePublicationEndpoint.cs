using System.Security;
using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Configuration;
using NewsletterSDK.Services;
using Microsoft.Extensions.Options;

namespace NewsletterWebApi.Endpoints;

public class CreatePublicationEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/publications", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<CreatePublicationEndpoint> logger,
            [FromServices] IPublicationService publicationService,
            [FromBody] CreatePublicationRequest createPublicationRequest,
            [FromServices] IOptions<ConnectionStringsOptions> connectionStringsOptions) =>
        {
            logger.LogInformation(
                "Received request to create publication with Title: {Title}, SendingDate: {SendingDate}",
                createPublicationRequest.Title,
                createPublicationRequest.SendingDate
            );

            try
            {
                var publication = await publicationService.Publish(
                    createPublicationRequest.Content,
                    createPublicationRequest.Title,
                    createPublicationRequest.SendingDate,
                    cancellationToken);

                logger.LogInformation(
                    "Publication '{Title}' scheduled successfully for {SendingDate}.",
                    createPublicationRequest.Title,
                    createPublicationRequest.SendingDate
                );

                return Results.Created($"/api/publications/id={publication.Id}", publication);
            }
            catch (ArgumentException argumentException)
            {
                logger.LogWarning(
                    argumentException,
                    "Invalid input provided when creating publication. Title: {Title}, SendingDate: {SendingDate}",
                    createPublicationRequest.Title,
                    createPublicationRequest.SendingDate
                );
                return Results.BadRequest(argumentException.Message);
            }
            catch (SecurityException securityException)
            {
                logger.LogWarning(
                    securityException,
                    "Invalid input provided when creating publication. Title: {Title}, SendingDate: {SendingDate}",
                    createPublicationRequest.Title,
                    createPublicationRequest.SendingDate
                );
                return Results.UnprocessableEntity(securityException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "An unexpected error occurred while creating publication. Title: {Title}, SendingDate: {SendingDate}",
                    createPublicationRequest.Title,
                    createPublicationRequest.SendingDate
                );

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while creating the publication." 
                );
            }
        })
        .WithTags("Publications")
        .WithName("CreatePublication")
        .WithSummary("Creates a new publication that can be sent now or scheduled.")
        .WithDescription("Schedules a new publication with the provided title, content, and sending date.")
        .WithOpenApi()
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("One request every 5 seconds per host")
        .RequireApiKey();
    }
}
