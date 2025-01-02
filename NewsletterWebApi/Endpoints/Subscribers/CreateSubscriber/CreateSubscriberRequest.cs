namespace NewsletterWebApi.Endpoints;

public class CreateSubscriberRequest
{
    /// <summary>
    /// The subscriber email to be added in the newsletter mailing list
    /// </summary>
    /// <example>person@email.com</example>
    public required string Email { get; init; }

    /// <summary>
    /// The subscriber name or what they would like to be called. Optional
    /// </summary>
    /// <example>John Yang</example>
    public string? Name { get; init; }
}