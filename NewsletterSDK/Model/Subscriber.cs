namespace NewsletterSDK.Models;

public record Subscriber
{
    public required string Email { get; init; }
    public string? Name { get; init; }
    public required DateTime CreatedAt { get; init; }
}