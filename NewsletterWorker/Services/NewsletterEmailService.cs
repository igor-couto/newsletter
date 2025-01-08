using NewsletterSDK.Services;
using NewsletterSDK.Models;

namespace NewsletterWorker.Services;

public class NewsletterEmailService(
    ILogger<NewsletterEmailService> logger,
    EmailSender emailSender,
    IServiceProvider serviceProvider)
{
    private readonly ILogger<NewsletterEmailService> _logger = logger;
    private readonly EmailSender _emailSender = emailSender;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task SendNewsletterAsync()
    {
        _logger.LogInformation("Start Recurring job execution at: {date}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var _subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
        var _publicationService = scope.ServiceProvider.GetRequiredService<IPublicationService>();

        try
        {
            var publications = await _publicationService.GetUnsentPublications();
            if (!publications.Any())
            {
                _logger.LogInformation("No unsent publications found at {Date}, skipping execution.", DateTime.UtcNow);
                return;
            }

            var subscribers = await _subscriptionService.GetAllSubscribers();
            if (!subscribers.Any())
                _logger.LogWarning("No subscribers found at {Date}. Publications will not be sent.", DateTime.UtcNow);

            foreach (var publication in publications)
            {
                var errorCount = 0;
                foreach (var subscriber in subscribers)
                {
                    var personalizedContent = publication.Content.Replace("[Name]", subscriber.Name);
                    try
                    {
                        _emailSender.SendEmail(subscriber.Email, subscriber.Name, publication.Title, personalizedContent);

                        _logger.LogInformation(
                        "Successfully sent publication '{PublicationTitle}' to subscriber '{SubscriberEmail}' at {Date}",
                        publication.Title, subscriber.Email, DateTime.UtcNow);

                        await _publicationService.RegisterSuccessfullDelivery(publication, subscriber.Email);
                    }
                    catch (Exception exception)
                    {
                        errorCount++;

                        _logger.LogError(
                            exception,
                            "Failed to send publication '{PublicationTitle}' to subscriber '{SubscriberEmail}' at {Date}",
                            publication.Title, subscriber.Email, DateTime.UtcNow);

                        await _publicationService.RegisterFailedDelivery(publication, subscriber.Email);
                    }
                }

                if (errorCount == subscribers.Count())
                    publication.PublicationStatus = PublicationStatus.Failed;

                if (errorCount > 0)
                    publication.PublicationStatus = PublicationStatus.PartiallyDelivered;

                publication.PublicationStatus = PublicationStatus.Delivered;

                await _publicationService.CompletePublication(publication);

                _logger.LogInformation(
                    "Completed processing publication '{PublicationTitle}' with status '{PublicationStatus}' at {Date}",
                    publication.Title, publication.PublicationStatus, DateTime.UtcNow);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unexpected error during recurring job execution at {Date}. Error message: {ErrorMessage}",
                DateTime.UtcNow, exception.Message);
        }

        _logger.LogInformation("Finished recurring job execution at: {Date}", DateTime.UtcNow);
    }
}
