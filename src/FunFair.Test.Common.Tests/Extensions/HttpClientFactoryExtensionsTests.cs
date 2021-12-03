using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FunFair.Test.Common.Extensions;
using Xunit;

namespace FunFair.Test.Common.Tests.Extensions;

public sealed class HttpClientFactoryExtensionsTests : TestBase
{
    [Fact]
    public async Task ShouldHaveCorrectContentAsync()
    {
        const string clientName = @"TestExample";
        const string expectedContent = "Hello World!";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway, responseMessage: expectedContent);

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);

        string content = await responseMessage.Content.ReadAsStringAsync();
        Assert.Equal(expected: expectedContent, actual: content);
    }

    [Fact]
    public async Task ShouldHaveCorrectResponseCodeAsync()
    {
        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway);

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }
}