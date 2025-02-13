using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Test.Common.Extensions;
using FunFair.Test.Common.Tests.Mocks;
using Xunit;

namespace FunFair.Test.Common.Tests.Extensions;

public sealed class HttpClientFactoryExtensionsTests : TestBase
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(value: 1);

    [Fact]
    public async Task ShouldHaveCorrectContentAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);

            string content = await responseMessage.Content.ReadAsStringAsync(
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: expectedContent, actual: content);
        }
    }

    [Fact]
    public async Task ShouldHaveCorrectResponseCodeAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }

    [Fact]
    public async Task MockCreateClientWithResponseWithHeadersAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedJsonSerializerOptionsAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
        {
            JsonSerializerOptions serializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersSerializerOptionsAsync()
    {
        using (CancellationTokenSource cts = new(Delay))
        {
            JsonSerializerOptions serializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
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
                cancellationToken: cts.Token
            );
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }
}
