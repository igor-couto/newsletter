using NewsletterSDK.Models;

namespace NewsletterWebApi.Endpoints;

public class PublicationsPaginatedResponse
{
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required int Total { get; init; }
    public required IEnumerable<Publication> Publications { get; init; }
}