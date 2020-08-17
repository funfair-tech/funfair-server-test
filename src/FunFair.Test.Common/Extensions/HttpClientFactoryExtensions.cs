using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace FunFair.Test.Common.Extensions
{
    /// <summary>
    ///     Extensions on <see cref="IHttpClientFactory" />
    /// </summary>
    public static class HttpClientFactoryExtensions
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions();

        /// <summary>
        ///     Mocks the client
        /// </summary>
        /// <param name="httpClientFactory">The Http Client Factory</param>
        /// <param name="clientName">The client name.</param>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseMessage">Response message string.</param>
        [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
        public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, string responseMessage)
        {
            HttpClient client = new HttpClient(new FakeHttpMessageHandler(statusCode: httpStatusCode, responseMessage: responseMessage)) {BaseAddress = new Uri("https://localhost")};

            httpClientFactory.CreateClient(clientName)
                             .Returns(client);
        }

        /// <summary>
        ///     Mocks the client
        /// </summary>
        /// <param name="httpClientFactory">The Http Client Factory</param>
        /// <param name="clientName">The client name.</param>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode)
        {
            MockCreateClientWithResponse(httpClientFactory: httpClientFactory, clientName: clientName, httpStatusCode: httpStatusCode, responseMessage: string.Empty);
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpClientFactory">The Http Client Factory</param>
        /// <param name="clientName">The client name.</param>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseObject">Response object to return.</param>
        public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, T responseObject)
        {
            MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                         clientName: clientName,
                                         httpStatusCode: httpStatusCode,
                                         JsonSerializer.Serialize(value: responseObject, options: SerializerOptions));
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpClientFactory">The Http Client Factory</param>
        /// <param name="clientName">The client name.</param>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseObject">Response object to return.</param>
        /// <param name="jsonSerializerOptions">The JSON serializer options to use.</param>
        public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory,
                                                           string clientName,
                                                           HttpStatusCode httpStatusCode,
                                                           T responseObject,
                                                           JsonSerializerOptions jsonSerializerOptions)
        {
            MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                         clientName: clientName,
                                         httpStatusCode: httpStatusCode,
                                         JsonSerializer.Serialize(value: responseObject, options: jsonSerializerOptions));
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _responseMessage;
            private readonly HttpStatusCode _statusCode;

            public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseMessage)
            {
                this._statusCode = statusCode;
                this._responseMessage = responseMessage;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(this._statusCode) {Content = new StringContent(this._responseMessage)};

                return Task.FromResult(httpResponseMessage);
            }
        }
    }
}