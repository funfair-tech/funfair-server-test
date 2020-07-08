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
        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseMessage">Response message string.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        public static HttpClient Create(HttpStatusCode httpStatusCode, string responseMessage)
        {
            return new HttpClient(new FakeHttpMessageHandler(httpStatusCode, responseMessage));
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <returns>HTTP status code to be returned.</returns>
        public static HttpClient Create(HttpStatusCode httpStatusCode)
        {
            return Create(httpStatusCode, string.Empty);
        }

        /// <summary>
        ///     Creates the client
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code to be returned.</param>
        /// <param name="responseObject">Response object to return.</param>
        /// <returns>HTTP status code to be returned.</returns>
        public static HttpClient Create<T>(HttpStatusCode httpStatusCode, T responseObject)
        {
            return Create(httpStatusCode, JsonSerializer.Serialize(responseObject, new JsonSerializerOptions()));
        }

        private sealed class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _responseMessage;

            public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseMessage)
            {
                this._statusCode = statusCode;
                this._responseMessage = responseMessage;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage httpResponseMessage;

                httpResponseMessage = new HttpResponseMessage(this._statusCode) { Content = new StringContent(this._responseMessage) };

                return Task.FromResult(httpResponseMessage);
            }
        }
    }
}