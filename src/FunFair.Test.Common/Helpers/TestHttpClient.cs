using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Test.Common.Helpers
{
    /// <summary>
    ///     Provides an HttpClient with programmable behaviour for testing
    /// </summary>
    public static class TestHttpClient
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions();

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseMessage">Response message string.</param>
        /// <returns></returns>
        [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
        public static HttpClient Create(HttpStatusCode httpStatusCode, string responseMessage)
        {
            return new HttpClient(new FakeHttpMessageHandler(statusCode: httpStatusCode, responseMessage: responseMessage)) {BaseAddress = new Uri("https://localhost")};
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <returns>HTTP status code to be returned.</returns>
        public static HttpClient Create(HttpStatusCode httpStatusCode)
        {
            return Create(httpStatusCode: httpStatusCode, responseMessage: string.Empty);
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseObject">Response object to return.</param>
        /// <returns>HTTP status code to be returned.</returns>
        public static HttpClient Create<T>(HttpStatusCode httpStatusCode, T responseObject)
        {
            return Create(httpStatusCode: httpStatusCode, JsonSerializer.Serialize(value: responseObject, options: SerializerOptions));
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseObject">Response object to return.</param>
        /// <param name="jsonSerializerOptions">The JSON serializer options to use.</param>
        /// <returns>HTTP status code to be returned.</returns>
        public static HttpClient Create<T>(HttpStatusCode httpStatusCode, T responseObject, JsonSerializerOptions jsonSerializerOptions)
        {
            return Create(httpStatusCode: httpStatusCode, JsonSerializer.Serialize(value: responseObject, options: jsonSerializerOptions));
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