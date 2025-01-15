using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using NewsletterIntegrationTests.TestSetupHooks;

namespace NewsletterIntegrationTests.Steps;

[Binding]
internal class VersionSteps(TestContext context)
{
    [Given(@"I want to check the Web API Version")]
    public static void GivenIWantToCheckTheWebApiVersion() { }

    [When(@"I call the version endpoint")]
    public async Task WhenICallTheVersionEndpoint()
    {
        context.Response = await context.Client.GetAsync("version");
    }

    [Then(@"the version should be correct")]
    public async Task ThenTheVersionShouldBeCorrect()
    {
        context.Response.EnsureSuccessStatusCode();
        context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        context.Response.ReasonPhrase.Should().Be("OK");
        context.Response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);
        
        var responseContent = await context.Response.Content.ReadAsStringAsync();
        responseContent.Should().NotBeNullOrWhiteSpace();
        responseContent.Should().Be("{\"version\":\"1.0\"}");
    }
}