namespace NewsletterSDK.Models;

public record PublicationDelivery
{
    public required string SubscriberEmail { get; set; }
    public required Guid PublicationId { get; set; }
    public required DeliveryStatus DeliveryStatus { get; set; }
    public required DateTime SendingDate { get; set; }
}