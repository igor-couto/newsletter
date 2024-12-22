namespace NewsletterSDK.Models;

public record Unsubscription
{
    public required string Email { get; init; }
    public string? Name { get; init; }
    public DateTime? SubscribedAt { get; init; }
    public required DateTime UnsubscribedAt { get; init; }
    public string? Reason { get; init; }
    public const short ReasonMaxLength = 800; 
}