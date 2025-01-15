using System.Net;
using System.Text.Json;
using FluentAssertions;
using TechTalk.SpecFlow;
using NewsletterIntegrationTests.TestSetupHooks;

namespace NewsletterIntegrationTests.Steps;

[Binding]
internal class HealthCheckSteps(TestContext context)
{
    private HealthCheck _healthCheckResponse;

    [Given(@"I want to check the Web API Health")]
    public static void GivenIHaveAValidPublicationPayload() { }

    [When(@"I call the health check endpoint")]
    public async Task WhenICreateANewPublication()
    {
        context.Response = await context.Client.GetAsync("health");

        var responseContent = await context.Response.Content.ReadAsStringAsync();
        _healthCheckResponse = JsonSerializer.Deserialize<HealthCheck>(responseContent, TestContext.JsonSerializerOptions);
    }

    [Then(@"the overall status must be healthy")]
    public async Task ThenTheOverallStatusMustBeHealthy()
    {
        context.Response.EnsureSuccessStatusCode();
        context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        context.Response.ReasonPhrase.Should().Be("OK");
        context.Response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);

        _healthCheckResponse.Status.Should().Be("Healthy");
    }

    [Then(@"the Web API status should be heathy")]
    public async Task ThenTheWebApiStatusShouldBeHealthy()
    {
        var apiCheck = _healthCheckResponse.Checks.First(x => x.Name == "Web API");
        apiCheck.Status.Should().Be("Healthy");
        apiCheck.Description.Should().Be("The web api application is up and running");
    }

    [Then(@"the database status should be heathy")]
    public async Task ThenTheDatabaseStatusShouldBeHealthy()
    {
        var databaseCheck = _healthCheckResponse.Checks.First(x => x.Name == "PostgreSQL Database");
        databaseCheck.Status.Should().Be("Healthy");
        databaseCheck.Description.Should().Be("The system is operating normally and is fully functional.");
    }
}

public class Check
{
    public string Name { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
}

public class HealthCheck
{
    public string Status { get; set; }
    public List<Check> Checks { get; set; }
}