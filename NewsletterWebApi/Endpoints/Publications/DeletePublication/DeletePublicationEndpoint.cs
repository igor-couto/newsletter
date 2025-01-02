using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;

namespace NewsletterWebApi.Endpoints;

public class DeletePublicationEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/publications/{id}", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<DeletePublicationEndpoint> logger,
            [FromServices] IPublicationService publicationService,
            [FromRoute] Guid id) =>
        {
            logger.LogInformation("Received request to delete publication with ID {PublicationId}.", id);

            try
            {
                await publicationService.DeletePublication(id, cancellationToken);
                logger.LogInformation("Successfully deleted publication with ID {PublicationId}.", id);
                return Results.NoContent();
            }
            catch(KeyNotFoundException keyNotFoundException)
            {
                logger.LogWarning(keyNotFoundException, 
                    "Invalid request: could not delete publication with ID {PublicationId}. Error: {ErrorMessage}",
                    id, keyNotFoundException.Message);

                return Results.NotFound(keyNotFoundException.Message);
            }
            catch (ArgumentException argumentException)
            {
                logger.LogWarning(argumentException, 
                    "Invalid request: could not delete publication with ID {PublicationId}. Error: {ErrorMessage}",
                    id, argumentException.Message);

                return Results.BadRequest(argumentException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, 
                    "Unhandled exception occurred while deleting publication with ID {PublicationId}.",
                    id);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while deleting the publication."
                );
            }
        })
        .WithTags("Publications")
        .WithName("DeletePublication")
        .WithSummary("Deletes a pending publication.")
        .WithDescription("Removes the pending publication associated with the specified ID from the system.")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host")
        .RequireApiKey();
    }
}
