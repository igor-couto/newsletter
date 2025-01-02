namespace NewsletterWebApi.Endpoints;

public partial class UnsubcribeRequest
{
    public required string Email { get; init; }
    
    public string? Reason { get; init; }
}