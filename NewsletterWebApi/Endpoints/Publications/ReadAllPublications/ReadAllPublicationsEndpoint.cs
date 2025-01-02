using Microsoft.AspNetCore.Mvc;
using NewsletterSDK.Services;
using NewsletterSDK.Models;

namespace NewsletterWebApi.Endpoints;

public class ReadAllPublicationsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/publications", async (
            CancellationToken cancellationToken,
            [FromServices] ILogger<ReadAllPublicationsEndpoint> logger,
            [FromServices] IPublicationService publicationService,
            [FromQuery] PublicationStatus? status,
            [FromQuery] string? title,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25
            ) =>
        {
            logger.LogInformation(
                "Received request to retrieve publications. Status: {Status}, Title: {Title}, Page: {Page}, Limit: {Limit}", 
                status, title, page, limit);

            try
            {
                var searchResult = await publicationService.GetPublications(status, title, page, limit);
                var response = new PublicationsPaginatedResponse
                {
                    Publications = searchResult.publications,
                    Page = page,
                    Limit = limit,
                    Total = searchResult.total
                };

                logger.LogInformation(
                    "Successfully retrieved {Count} publications for Page: {Page}, Limit: {Limit}, Status: {Status}, Title: {Title}. Total: {Total}",
                    response.Publications.Count(), page, limit, status, title, response.Total);

                return Results.Ok(response);
            }
            catch (ArgumentException argumentException)
            {
                logger.LogWarning(argumentException,
                    "Invalid parameters provided when retrieving publications. Status: {Status}, Title: {Title}, Page: {Page}, Limit: {Limit}",
                    status, title, page, limit);

                return Results.BadRequest(argumentException.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "An error occurred while retrieving publications. Status: {Status}, Title: {Title}, Page: {Page}, Limit: {Limit}",
                    status, title, page, limit);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected error occurred while retrieving publications."
                );
            }
        })
        .WithTags("Publications")
        .WithName("GetAllPublications")
        .WithSummary("Retrieves paginated publications with optional filtering by status and title.")
        .WithDescription("Returns a paginated list of publications optionally filtered by a given status and/or a partial title match.")
        .WithOpenApi()
        .Produces<PublicationsPaginatedResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .RequireRateLimiting("Three requests per second per host")
        .CacheOutput("Expire in 3 minutes")
        .RequireApiKey();
    }
}
