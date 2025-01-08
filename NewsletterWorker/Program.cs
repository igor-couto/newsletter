using Microsoft.AspNetCore.Builder;
using NewsletterWorker.Configuration;
using NewsletterWorker.Jobs;

var builder = WebApplication.CreateBuilder(args);
builder.AddLogging();
builder.Services
    .AddOptions(builder.Configuration)
    .AddDependencyInjection()
    .AddHangfireConfiguration()
    .AddHostedService<NewsletterRecurringJob>();

var app = builder.Build();
app
    .UseHangfireConfiguration()
    .Run();