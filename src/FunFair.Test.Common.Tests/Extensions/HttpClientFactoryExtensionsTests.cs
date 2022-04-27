using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FunFair.Test.Common.Extensions;
using FunFair.Test.Common.Tests.Mocks;
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

    [Fact]
    public async Task MockCreateClientWithResponseWithHeadersAsync()
    {
        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase) { ["Content-Type"] = "application/json", ["Authorization"] = "Bearer 12345" };
        httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway, headers: headers);

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedAsync()
    {
        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway, new ExampleObject { Name = "Banana" });

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersAsync()
    {
        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase) { ["Content-Type"] = "application/json", ["Authorization"] = "Bearer 12345" };
        httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway, headers: headers, responseObject: new ExampleObject { Name = "Banana" });

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedJsonSerializerOptionsAsync()
    {
        JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        httpClientFactory.MockCreateClientWithResponse(clientName: clientName,
                                                       httpStatusCode: HttpStatusCode.BadGateway,
                                                       new ExampleObject { Name = "Banana" },
                                                       jsonSerializerOptions: serializerOptions);

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }

    [Fact]
    public async Task MockCreateClientWithResponseTypedWithHeadersSerializerOptionsAsync()
    {
        JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        const string clientName = @"TestExample";

        IHttpClientFactory httpClientFactory = GetSubstitute<IHttpClientFactory>();

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase) { ["Content-Type"] = "application/json", ["Authorization"] = "Bearer 12345" };
        httpClientFactory.MockCreateClientWithResponse(clientName: clientName,
                                                       httpStatusCode: HttpStatusCode.BadGateway,
                                                       headers: headers,
                                                       responseObject: new ExampleObject { Name = "Banana" },
                                                       jsonSerializerOptions: serializerOptions);

        HttpClient client = httpClientFactory.CreateClient(clientName);

        HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
        Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
    }
}