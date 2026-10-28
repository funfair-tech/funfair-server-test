using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FunFair.Test.Common;
using FunFair.Test.Infrastructure.Extensions;
using FunFair.Test.Infrastructure.Tests.Mocks;
using Xunit;

namespace FunFair.Test.Infrastructure.Tests.Extensions;

public sealed class HttpClientFactoryExtensionsTests : TestBase
{
    [Fact]
    public async Task ShouldHaveCorrectContentAsync()
    {
        const string clientName = "TestExample";
        const string expectedContent = "Hello World!";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            responseMessage: expectedContent
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);

        string content = await responseMessage.Content.ReadAsStringAsync(cancellationToken: this.CancellationToken());
        Assert.Equal(expected: expectedContent, actual: content);
    }

    [Fact]
    public async Task ShouldHaveCorrectResponseCodeAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseWithHeadersAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = "Bearer 12345",
        };
        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            headers: headers
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            responseObject: MockReferenceData.ExampleObject
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = "Bearer 12345",
        };
        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            responseObject: MockReferenceData.ExampleObject,
            headers: headers
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedJsonSerializerOptionsAsync()
    {
        JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            responseObject: MockReferenceData.ExampleObject,
            jsonSerializerOptions: serializerOptions
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersSerializerOptionsAsync()
    {
        JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = "Bearer 12345",
        };
        httpClientFactory.MockCreateClientWithResponse(
            clientName: clientName,
            httpStatusCode: HttpStatusCode.BadGateway,
            MockReferenceData.ExampleObject.Next(),
            jsonSerializerOptions: serializerOptions,
            headers: headers
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(
            new Uri(uriString: "/test", uriKind: UriKind.Relative),
            this.CancellationToken()
        );
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithSequentialResponsesReturnsInOrderThenRepeatsLastAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithSequentialResponses(
            clientName: clientName,
            responses: [(HttpStatusCode.ServiceUnavailable, "first"), (HttpStatusCode.OK, "second")]
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.ServiceUnavailable,
            expectedContent: "first"
        );
        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.OK,
            expectedContent: "second"
        );
        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.OK,
            expectedContent: "second"
        );
    }

    [Fact]
    public async Task MockCreateClientWithSequentialResponsesWithHeadersAsync()
    {
        const string clientName = "TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> firstHeaders = new(StringComparer.OrdinalIgnoreCase) { ["X-Sequence"] = "1" };
        Dictionary<string, string> secondHeaders = new(StringComparer.OrdinalIgnoreCase) { ["X-Sequence"] = "2" };

        httpClientFactory.MockCreateClientWithSequentialResponses(
            clientName: clientName,
            responses: [(HttpStatusCode.Accepted, "first", firstHeaders), (HttpStatusCode.OK, "second", secondHeaders)]
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        await this.AssertNextResponseWithSequenceHeaderAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.Accepted,
            expectedContent: "first",
            expectedSequence: "1"
        );
        await this.AssertNextResponseWithSequenceHeaderAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.OK,
            expectedContent: "second",
            expectedSequence: "2"
        );
    }

    [Fact]
    public async Task MockCreateClientWithSequentialResponsesTypedAsync()
    {
        const string clientName = "TestExample";

        ExampleObject firstObject = MockReferenceData.ExampleObject.Next();
        ExampleObject secondObject = MockReferenceData.ExampleObject.Next();

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithSequentialResponses(
            clientName: clientName,
            responses: [(HttpStatusCode.Created, firstObject), (HttpStatusCode.OK, secondObject)]
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.Created,
            expectedContent: "{\"Name\":\"" + firstObject.Name + "\"}"
        );
        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.OK,
            expectedContent: "{\"Name\":\"" + secondObject.Name + "\"}"
        );
    }

    [Fact]
    public async Task MockCreateClientWithSequentialResponsesTypedJsonSerializerOptionsAsync()
    {
        JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        const string clientName = "TestExample";

        ExampleObject firstObject = MockReferenceData.ExampleObject.Next();
        ExampleObject secondObject = MockReferenceData.ExampleObject.Next();

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithSequentialResponses(
            clientName: clientName,
            responses: [(HttpStatusCode.Created, firstObject), (HttpStatusCode.OK, secondObject)],
            jsonSerializerOptions: serializerOptions
        );

        HttpClient client = httpClientFactory.CreateClient(clientName);

        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.Created,
            expectedContent: "{\"name\":\"" + firstObject.Name + "\"}"
        );
        await this.AssertNextResponseAsync(
            client: client,
            expectedStatusCode: HttpStatusCode.OK,
            expectedContent: "{\"name\":\"" + secondObject.Name + "\"}"
        );
    }

    private async Task AssertNextResponseAsync(
        HttpClient client,
        HttpStatusCode expectedStatusCode,
        string expectedContent
    )
    {
        using HttpResponseMessage responseMessage = await this.GetNextResponseAsync(client);

        await this.AssertResponseAsync(
            responseMessage: responseMessage,
            expectedStatusCode: expectedStatusCode,
            expectedContent: expectedContent
        );
    }

    private async Task AssertNextResponseWithSequenceHeaderAsync(
        HttpClient client,
        HttpStatusCode expectedStatusCode,
        string expectedContent,
        string expectedSequence
    )
    {
        using HttpResponseMessage responseMessage = await this.GetNextResponseAsync(client);

        await this.AssertResponseAsync(
            responseMessage: responseMessage,
            expectedStatusCode: expectedStatusCode,
            expectedContent: expectedContent
        );

        string sequence = Assert.Single(responseMessage.Headers.GetValues("X-Sequence"));
        Assert.Equal(expected: expectedSequence, actual: sequence);
    }

    private Task<HttpResponseMessage> GetNextResponseAsync(HttpClient client)
    {
        return client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative), this.CancellationToken());
    }

    private async Task AssertResponseAsync(
        HttpResponseMessage responseMessage,
        HttpStatusCode expectedStatusCode,
        string expectedContent
    )
    {
        Assert.Equal(expected: expectedStatusCode, actual: responseMessage.StatusCode);

        string content = await responseMessage.Content.ReadAsStringAsync(cancellationToken: this.CancellationToken());
        Assert.Equal(expected: expectedContent, actual: content);
    }
}
