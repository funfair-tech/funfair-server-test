using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FunFair.Test.Common.Extensions;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests.Extensions
{
    public sealed class HttpClientFactoryExtensionsTests : LoggingTestBase
    {
        public HttpClientFactoryExtensionsTests(ITestOutputHelper output)
            : base(output)
        {
        }

        private void Dump(HttpResponseHeaders headers)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
            {
                this.Output.WriteLine($"{header.Key}: {header.Value.FirstOrDefault() ?? string.Empty}");
            }
        }

        private void Dump(HttpContentHeaders headers)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
            {
                this.Output.WriteLine($"{header.Key}: {header.Value.FirstOrDefault() ?? string.Empty}");
            }
        }

        [Fact]
        public async Task ShouldHaveCorrectContentAsync()
        {
            const string clientName = @"TestExample";
            const string expectedContent = "Hello World!";

            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();

            httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway, responseMessage: expectedContent);

            HttpClient client = httpClientFactory.CreateClient(clientName);

            HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);

            var content = await responseMessage.Content.ReadAsStringAsync();
            Assert.Equal(expected: expectedContent, actual: content);
        }

        [Fact]
        public async Task ShouldHaveCorrectResponseCodeAsync()
        {
            const string clientName = @"TestExample";

            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();

            httpClientFactory.MockCreateClientWithResponse(clientName: clientName, httpStatusCode: HttpStatusCode.BadGateway);

            HttpClient client = httpClientFactory.CreateClient(clientName);

            HttpResponseMessage responseMessage = await client.GetAsync(new Uri(uriString: "/test", uriKind: UriKind.Relative));
            Assert.Equal(expected: HttpStatusCode.BadGateway, actual: responseMessage.StatusCode);
        }
    }
}