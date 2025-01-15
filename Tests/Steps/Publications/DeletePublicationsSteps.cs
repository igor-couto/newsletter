using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TechTalk.SpecFlow;
using Bogus;
using NewsletterIntegrationTests.TestSetupHooks;
using NewsletterSDK.Models;
using NewsletterIntegrationTests.Extension;
using NewsletterWebApi.Endpoints;
using System.Text.Json.Serialization;

namespace NewsletterIntegrationTests;

[Binding]
public class DeletePublicationsSteps(TestContext context)
{
    private string _publicationId;

    [Given(@"I have a valid unsent scheduled publication created")]
    public async Task GivenIHaveAValidUnsentScheduledPpublicationCreated()
    {
        var createPublicationPayload = new
        {
            title = "Test Publication",
            content = "<html><head></head><body><p>Hello, world!</p></body></html>",
            sendingDate = DateTime.Today.AddDays(1).GetNextHalfHour()
        };

        context.Response = await context.Client.PostAsJsonAsync("publications", createPublicationPayload);
        var responseContent = await context.Response.Content.ReadAsStringAsync();
        var publication = JsonSerializer.Deserialize<Publication>(responseContent, TestContext.JsonSerializerOptions);
        _publicationId = publication.Id.ToString();
    }

    [Given(@"I do not have any publication")]
    public static void GivenIDoNotHaveAnyPublication() { }

    [When(@"I delete this publication")]
    public async Task WhenIDeleteThisPublication()
    {
        context.Response = await context.Client.DeleteAsync($"publications/{_publicationId}");
    }

    [When(@"I delete this publication without the API Key")]
    public async Task WhenIDeleteThisPublicationWithoutTheApiKey()
    {
        context.Response = await context.ClientWithoutApiKey.DeleteAsync($"publications/{_publicationId}");
    }

    [When(@"I delete a publication that does not exist")]
    public async Task WhenIDeleteAPublicationThatDoesNotExist()
    {
        var aaa = $"publications/{Guid.CreateVersion7()}";
        context.Response = await context.Client.DeleteAsync($"publications/{Guid.CreateVersion7()}");
    }

    [Then(@"the publication should be deleted successfully")]
    public void ThenThePublicationShouldBeDeletedSuccessfully()
    {
        context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        context.Response.ReasonPhrase.Should().Be("No Content");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Then(@"the request should fail")]
    public void ThenTheRequestShouldFail()
    {
        context.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        context.Response.ReasonPhrase.Should().Be("Bad Request");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);
    }
}