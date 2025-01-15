using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Bogus;
using TechTalk.SpecFlow;

namespace NewsletterIntegrationTests.TestSetupHooks;

[Binding]
public class TestContext
{
    private const string WebApiUrl = "http://localhost:5275/api/";
    public HttpClient Client { get; private set; }
    public HttpClient ClientWithoutApiKey { get; private set; }
    public Faker Faker { get; private set; }
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    public HttpResponseMessage Response;

    [BeforeScenario]
    public void BeforeScenario()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        Client = new HttpClient { BaseAddress = new Uri(WebApiUrl) };
        Client.DefaultRequestHeaders.Add("X-Api-Key", configuration["Authentication:ApiKey"]);

        ClientWithoutApiKey = new HttpClient { BaseAddress = new Uri(WebApiUrl) };

        Faker = new Faker();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        Client?.Dispose();
        ClientWithoutApiKey?.Dispose();
    }
}
