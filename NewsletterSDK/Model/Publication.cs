namespace NewsletterSDK.Models;

public record Publication
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required PublicationStatus PublicationStatus { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime SendingDate { get; set; }
    public DateTime? DeletedAt { get; set; }
}