
using NewsletterSDK.Models;

namespace NewsletterSDK.Services;

public interface ISubscriptionService 
{
    public Task Subscribe(string email, string? name, CancellationToken cancellationToken = default);
    public Task Unsubscribe(string email, string? reason, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Subscriber>> GetAllSubscribers(CancellationToken cancellationToken = default);
    public Task<(IEnumerable<Subscriber> subscribers, int total)> GetSubscribers(int page, int limit, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Unsubscription>> GetAllUnsubscriptions(CancellationToken cancellationToken = default);
}