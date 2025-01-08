using Hangfire;
using Microsoft.Extensions.Options;
using NewsletterWorker.Configuration;
using NewsletterWorker.Services;

namespace NewsletterWorker.Jobs;

public class NewsletterRecurringJob(ILogger<NewsletterRecurringJob> logger, IOptions<CronExpressionOptions> cronExpressionOptions) : IHostedService
{
    private readonly ILogger<NewsletterRecurringJob> _logger = logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<NewsletterEmailService>(
            "newsletter-recurring-job",
            service => service.SendNewsletterAsync(),
            cronExpressionOptions.Value.CronExpression
        );

        _logger.LogInformation("Email Recurring Job scheduled to run with the cron expression: {cronExpression}. Description: {cronDescription}", cronExpressionOptions.Value.CronExpression, cronExpressionOptions.Value.CronDescription);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email Recurring Job stopped at: {date}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}