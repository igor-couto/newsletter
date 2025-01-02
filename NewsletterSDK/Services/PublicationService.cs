using NewsletterSDK.Models;
using NewsletterSDK.Repositories;

namespace NewsletterSDK.Services;

internal class PublicationService(PublicationRepository publicationRepository, DeliveriesRepository deliveriesRepository) : IPublicationService
{
    private readonly PublicationRepository _publicationRepository = publicationRepository;
    private readonly DeliveriesRepository _deliveriesRepository = deliveriesRepository;

    public async Task<Publication> Publish(string content, string title, DateTime? sendingDate = null, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(title) || title.Length > Publication.TitleMaxLength)
            throw new ArgumentException("The title is invalid. It should not be empty or white space and should be less than 512 charactes long.");

        if(string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("The content is null, empty or white space.");

        if(sendingDate is not null)
        {
            if (sendingDate.Value.Minute % 30 != 0)
                throw new ArgumentException("The given time must be on a 30-minute interval (e.g., 02:00 or 02:30).");

            if (sendingDate.Value < DateTime.Now)
                throw new ArgumentException("The given time must not be in the past.");
        }
            
        var cleanContent = ContentSanitizer.Sanitize(content);

        if(string.IsNullOrWhiteSpace(cleanContent))
            throw new ArgumentException("Error while processing the content of publication.");

        var publication = new Publication
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Content = cleanContent,
            PublicationStatus = PublicationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SendingDate = sendingDate?? DateTime.UtcNow
        };

        await _publicationRepository.Insert(publication, cancellationToken);

        return publication;
    }

    public async Task<IEnumerable<Publication>> GetAllPublications(CancellationToken cancellationToken = default)
    {
        return await _publicationRepository.ReadAll(cancellationToken);
    }

    public async Task<(IEnumerable<Publication> publications, int total)> GetPublications(PublicationStatus? status, string? title, int page, int limit)
    {
        if (page < 1)
            throw new ArgumentException("Page number must be greater than or equal to 1.");

        if (limit < 1)
            throw new ArgumentException("Limit must be greater than 0.");

        const int MAX_LIMIT = 100;
        if (limit > MAX_LIMIT)
            throw new ArgumentException($"Limit cannot be greater than {MAX_LIMIT}.");

        if(status is PublicationStatus.Deleted)
            throw new ArgumentException($"It is not possible to see deleted publications.");
        
        return await _publicationRepository.ReadFiltered(status, title, page, limit);
    }

    public async Task<IEnumerable<Publication>> GetUnsentPublications(CancellationToken cancellationToken = default)
    {
        return await _publicationRepository.ReadUnsent(cancellationToken);
    }

    public async Task CompletePublication(Publication publication, CancellationToken cancellationToken = default)
    {
        if(publication.PublicationStatus == PublicationStatus.Pending)
            throw new ArgumentException("Could not finish a pending publication. It need to be at a finishing state");

        publication.SendingDate = DateTime.UtcNow;
        await _publicationRepository.UpdatePublication(publication, cancellationToken);
    }

    public async Task RegisterSuccessfullDelivery(Publication publication, string email, CancellationToken cancellationToken = default)
    {
        var publicationDelivery = new PublicationDelivery
        {
            SubscriberEmail = email,
            PublicationId = publication.Id,
            DeliveryStatus = DeliveryStatus.Sent,
            SendingDate = DateTime.UtcNow
        };

        await _deliveriesRepository.Insert(publicationDelivery, cancellationToken);
    }

    public async Task RegisterFailedDelivery(Publication publication, string email, CancellationToken cancellationToken = default)
    {
        var publicationDelivery = new PublicationDelivery
        {
            SubscriberEmail = email,
            PublicationId = publication.Id,
            DeliveryStatus = DeliveryStatus.Failed,
            SendingDate = DateTime.UtcNow
        };

        await _deliveriesRepository.Insert(publicationDelivery, cancellationToken);
    }

    public async Task DeletePublication(Guid id, CancellationToken cancellationToken = default)
    {
        await _publicationRepository.Delete(id);
    }
}