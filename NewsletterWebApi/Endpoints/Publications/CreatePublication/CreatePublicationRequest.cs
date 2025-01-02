namespace NewsletterWebApi.Endpoints;

public partial class CreatePublicationRequest
{
    /// <summary>
    /// The title of the publication. It will be used as the email subject.
    /// </summary>
    /// <example>Monthly Newsletter - December Edition</example>
    public required string Title { get; init; }

    /// <summary>
    /// The main content or body of the publication. It accepts plain text or HTML.
    /// </summary>
    /// <example> Dear Subscribers, Welcome to our December edition of the Monthly Newsletter. This month, we cover...</example>
    public required string Content { get; init; }

    /// <summary>
    /// The date and time when the publication is scheduled to be sent out. If not present, the publication is sent immediately.
    /// </summary>
    /// <example>2024-12-31T18:30:00Z</example>
    public DateTime? SendingDate { get; init; }
}