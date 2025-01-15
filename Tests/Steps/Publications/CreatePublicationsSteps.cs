using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using TechTalk.SpecFlow;
using NewsletterIntegrationTests.TestSetupHooks;
using NewsletterSDK.Models;
using NewsletterIntegrationTests.Extension;

namespace NewsletterIntegrationTests;

[Binding]
public class CreatePublicationsSteps(TestContext context)
{
    private List<HttpResponseMessage> _responses;
    private string _publicationContent;
    private string _publicationTitle;
    private string _publicationSendingDate;
    private List<(dynamic Payload, string ExpectedStatus, string Reason)> _payloads;
    

    [Given(@"I have a valid publication payload")]
    public void GivenIHaveAValidPublicationPayload()
    {
        _publicationContent = "<html><head></head><body><p>Hello, world!</p></body></html>";
        _publicationTitle = "Test Publication";
    }

    [Given(@"I have a valid scheduled publication payload for (.*)")]
    public void GivenIHaveAValidScheduledPublicationPayloadFor(string scheduledTime)
    {
        GivenIHaveAValidPublicationPayload();
        _publicationSendingDate = DateTime.Now.TomorrowAt(scheduledTime).ToString("o");
    }

    [Given(@"I have the following invalid publication payloads:")]
    public void GivenIHaveTheFollowingInvalidPublicationPayloads(Table table)
    {
        var invalidPayloads = new List<(dynamic Payload, string ExpectedStatus, string Reason)>();

        foreach (var row in table.Rows)
        {
            var payload = new
            {
                title = row["Title"].GetString(),
                content = row["Content"].GetString()
            };

            var expectedStatus = row["ExpectedStatus"];
            var reason = row["Reason"];

            invalidPayloads.Add((payload, expectedStatus, reason));
        }
        _payloads = invalidPayloads;
    }

    [When(@"I attempt to create these invalid publications")]
    public async Task WhenIAttemptToCreateTheseInvalidPublications()
    {
        _responses = [];
        
        foreach (var (payload, expectedStatus, reason) in _payloads)
        {
            var response = await context.Client.PostAsJsonAsync("publications", (object)payload);
            _responses.Add(response);
        }
    }

    [Then(@"I should get the respective error responses")]
    public void ThenIShouldGetTheRespectiveErrorResponses()
    {
        for (var i = 0; i < _responses.Count; i++)
        {
            var response = _responses[i];
            var (payload, expectedStatus, reason) = _payloads[i];

            var actualStatus = (int)response.StatusCode;
            actualStatus.Should().Be(int.Parse(expectedStatus));
        }
    }

    [Given(@"I have an invalid scheduled publication payload with (.*)")]
    public void GivenIHaveAnInvalidScheduledPublicationPayload(string scheduledTime)
    {
        _publicationTitle = "Invalid Scheduled Title";
        _publicationContent = "Invalid Scheduled Content";

        if (scheduledTime == "Yesterday")
            _publicationSendingDate = DateTime.Today.AddDays(-1).ToString("o");
        else
            _publicationSendingDate = DateTime.Today.TomorrowAt(scheduledTime).ToString("o");
    }

    [Given(@"I have an invalid publication payload with Title (.*) and Content (.*)")]
    public void GivenIHaveAnInvalidPublicationPayload(string title, string content)
    {
        _publicationTitle = title.GetString();
        _publicationContent = content.GetString();
    }

    [When(@"I create a new publication")]
    public async Task WhenICreateANewPublication()
    {
        var publication = new
        {
            title = _publicationTitle,
            content = _publicationContent,
            sendingDate = _publicationSendingDate ?? null
        };

        context.Response = await context.Client.PostAsJsonAsync("publications", publication);
    }

    [When(@"I create a new publication without the API Key")]
    public async Task WhenICreateANewPublicationWithoutTheApiKey()
    {
        var publication = new { title = _publicationTitle, content = _publicationContent };
        context.Response = await context.ClientWithoutApiKey.PostAsJsonAsync("publications", publication);
    }

    [Then(@"the publication should be created successfully")]
    public async Task ThenThePublicationShouldBeCreatedSuccessfully()
    {
        context.Response.EnsureSuccessStatusCode();
        context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
        context.Response.ReasonPhrase.Should().Be("Created");

        var responseContent = await context.Response.Content.ReadAsStringAsync();
        var publication = JsonSerializer.Deserialize<Publication>(responseContent, TestContext.JsonSerializerOptions);

        publication.Should().NotBeNull();
        publication.Id.Should().NotBe(new Guid());
        publication.Title.Should().Be(_publicationTitle);
        publication.Content.Should().Be(_publicationContent);
        publication.PublicationStatus.Should().Be(PublicationStatus.Pending);
        publication.CreatedAt.Date.Should().Be(DateTime.UtcNow.Date);

        if (!string.IsNullOrWhiteSpace(_publicationSendingDate))
            publication.SendingDate.ToString().Should().Be(_publicationSendingDate);

        context.Response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);
        context.Response.Headers.Location.ToString().Should().Be($"/api/publications/id={publication.Id}");
    }

    [Then(@"the publication scheduled at (.*) should be created successfully")]
    public void ThenThePublicationScheduledAtTimeShouldBeCreatedSuccessfully(string scheduledTime)
    {
        context.Response.EnsureSuccessStatusCode();
        context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
        context.Response.ReasonPhrase.Should().Be("Created");
    }

    [Then(@"the reason is (.*)")]
    public async Task ThenTheReasonIs(string reason)
    {
        var responseContent = await context.Response.Content.ReadAsStringAsync();
        responseContent.RemoveDoubleQuotes().Should().Be(reason);
    }
}