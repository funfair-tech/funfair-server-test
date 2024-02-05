using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace FunFair.Test.Common.Extensions;

public static class HttpClientFactoryExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new();
    private static readonly IReadOnlyDictionary<string, string> NoHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private static Uri LocalHostUri { get; } = new("https://localhost");

    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, string responseMessage)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory, clientName: clientName, httpStatusCode: httpStatusCode, responseMessage: responseMessage, headers: NoHeaders);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0022:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory,
                                                    string clientName,
                                                    HttpStatusCode httpStatusCode,
                                                    string responseMessage,
                                                    IReadOnlyDictionary<string, string> headers)
    {
        HttpClient client = CreateFakeClient(httpStatusCode: httpStatusCode, responseMessage: responseMessage, headers: headers);

        httpClientFactory.CreateClient(clientName)
                         .Returns(client);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0022:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    private static HttpClient CreateFakeClient(HttpStatusCode httpStatusCode, string responseMessage, IReadOnlyDictionary<string, string> headers)
    {
        return new(new FakeHttpMessageHandler(statusCode: httpStatusCode, responseMessage: responseMessage, headers: headers)) { BaseAddress = LocalHostUri };
    }

    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory, clientName: clientName, httpStatusCode: httpStatusCode, responseMessage: string.Empty);
    }

    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, IReadOnlyDictionary<string, string> headers)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory, clientName: clientName, httpStatusCode: httpStatusCode, responseMessage: string.Empty, headers: headers);
    }

    public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, T responseObject)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                     clientName: clientName,
                                     httpStatusCode: httpStatusCode,
                                     JsonSerializer.Serialize(value: responseObject, options: SerializerOptions),
                                     headers: NoHeaders);
    }

    public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory,
                                                       string clientName,
                                                       HttpStatusCode httpStatusCode,
                                                       T responseObject,
                                                       IReadOnlyDictionary<string, string> headers)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                     clientName: clientName,
                                     httpStatusCode: httpStatusCode,
                                     JsonSerializer.Serialize(value: responseObject, options: SerializerOptions),
                                     headers: headers);
    }

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

    public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory,
                                                       string clientName,
                                                       HttpStatusCode httpStatusCode,
                                                       T responseObject,
                                                       JsonSerializerOptions jsonSerializerOptions,
                                                       IReadOnlyDictionary<string, string> headers)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                     clientName: clientName,
                                     httpStatusCode: httpStatusCode,
                                     JsonSerializer.Serialize(value: responseObject, options: jsonSerializerOptions),
                                     headers: headers);
    }

    [DebuggerDisplay("HTTP: {_statusCode}: {_responseMessage}")]
    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly IReadOnlyDictionary<string, string> _headers;
        private readonly string _responseMessage;
        private readonly HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseMessage, IReadOnlyDictionary<string, string> headers)
        {
            this._statusCode = statusCode;
            this._responseMessage = responseMessage;
            this._headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage = new(this._statusCode) { Content = new StringContent(this._responseMessage) };

            foreach ((string key, string value) in this._headers)
            {
                httpResponseMessage.Headers.Add(name: key, value: value);
            }

            return Task.FromResult(httpResponseMessage);
        }
    }
}