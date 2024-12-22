
using NewsletterSDK.Models;

namespace NewsletterSDK.Services;

public interface IPublicationService 
{
    public Task<Publication> Publish(string content, string title, DateTime? sendingDate = null, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Publication>> GetAllPublications(CancellationToken cancellationToken = default);
    public Task<(IEnumerable<Publication> publications, int total)> GetPublications(PublicationStatus? status, string? title, int page, int limit);
    public Task<IEnumerable<Publication>> GetUnsentPublications(CancellationToken cancellationToken = default);
    public Task CompletePublication(Publication publication, CancellationToken cancellationToken = default);
    public Task RegisterSuccessfullDelivery(Publication publication, string email, CancellationToken cancellationToken = default);
    public Task RegisterFailedDelivery(Publication publication, string email, CancellationToken cancellationToken = default);
    public Task DeletePublication(Guid id, CancellationToken cancellationToken = default);
}