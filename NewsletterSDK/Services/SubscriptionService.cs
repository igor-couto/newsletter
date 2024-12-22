using NewsletterSDK.Models;
using NewsletterSDK.Repositories;

namespace NewsletterSDK.Services;

internal class SubscriptionService(SubscriptionsRepository subscriptionsRepository) : ISubscriptionService
{
    private readonly SubscriptionsRepository _subscriptionsRepository = subscriptionsRepository;

    public async Task Subscribe(string email, string? name, CancellationToken cancellationToken = default)
    {
        if(!EmailValidator.IsValid(email))
            throw new ArgumentException("The email is not valid.");

        if(name is not null && name.Length > 255 && name.All(char.IsWhiteSpace))
            throw new ArgumentException("The name is not valid.");

        var subscriberAlreadyExists = await _subscriptionsRepository.SubscriberExists(email);
        if(subscriberAlreadyExists)
            throw new ArgumentException("The email is already registered in the newsletter.");

        var subscriber = new Subscriber
        {
            Email = email,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        var success = await _subscriptionsRepository.Insert(subscriber, cancellationToken);

        if(!success)
            throw new Exception("Failed to inser subscribe in database.");
    }
    public async Task Unsubscribe(string email, string? reason, CancellationToken cancellationToken = default)
    {
        if(!EmailValidator.IsValid(email))
            throw new ArgumentException("The email is not valid.");

        if(reason is not null && reason.Length > Unsubscription.ReasonMaxLength && reason.All(char.IsWhiteSpace))
            throw new ArgumentException("The reason is not valid.");

        var subscriber = await _subscriptionsRepository.FindByEmail(email, cancellationToken);
        
        if(subscriber is null)
            throw new ArgumentException("The email is not registered in the newsletter thus can not be unsubscribed.");

        var success = await _subscriptionsRepository.Unsubscribe(subscriber, reason);    

        if(!success)
            throw new Exception("Failed to inser subscribe in database.");
    }

    public async Task<IEnumerable<Subscriber>> GetAllSubscribers(CancellationToken cancellationToken = default)
    {
        return await _subscriptionsRepository.ReadAllSubscribers(cancellationToken);
    }

    public async Task<IEnumerable<Unsubscription>> GetAllUnsubscriptions(CancellationToken cancellationToken = default)
    {
        return await _subscriptionsRepository.ReadAllUnsubscribed(cancellationToken);
    }

    public async Task<(IEnumerable<Subscriber> subscribers, int total)> GetSubscribers(int page, int limit, CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentException("Page number must be greater than or equal to 1.");

        if (limit < 1)
            throw new ArgumentException("Limit must be greater than 0.");

        const int MAX_LIMIT = 100;
        if (limit > MAX_LIMIT)
            throw new ArgumentException($"Limit cannot be greater than {MAX_LIMIT}.");
        
        return await _subscriptionsRepository.ReadAllSubscribersPaginated(page, limit);
    }
}