using FluentAssertions;
using NewsletterIntegrationTests.TestSetupHooks;
using System.Net;
using TechTalk.SpecFlow;

namespace NewsletterIntegrationTests.Steps;

[Binding]
internal class BaseSteps(TestContext context)
{
    [Then(@"the request should be unauthorized")]
    public void ThenTheRequestShouldBeUnauthorized()
    {
        context.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        context.Response.ReasonPhrase.Should().Be("Unauthorized");
        context.Response.Headers.Date.Value.UtcDateTime.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Then(@"I should get a status code (.*)")]
    public void ThenIShouldGetAStatusCode(int expectedStatusCode)
    {
        var actualStatus = (int) context.Response.StatusCode;
        actualStatus.Should().Be(expectedStatusCode);
    }
}
