using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace FunFair.Test.Common.Extensions;

/// <summary>
///     Extensions on <see cref="IHttpClientFactory" />
/// </summary>
public static class HttpClientFactoryExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new();
    private static readonly IReadOnlyDictionary<string, string> NoHeaders = new Dictionary<string, string>();

    /// <summary>
    ///     Mocks the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseMessage">Response message string.</param>
    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, string responseMessage)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                     clientName: clientName,
                                     httpStatusCode: httpStatusCode,
                                     responseMessage: responseMessage,
                                     headers: NoHeaders);
    }

    /// <summary>
    ///     Mocks the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseMessage">Response message string.</param>
    /// <param name="headers">Headers to add to the response.</param>
    [SuppressMessage(category: "Reliability", checkId: "CA2000:Dispose objects before losing scope", Justification = "For unit tests caller to dispose")]
    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory,
                                                    string clientName,
                                                    HttpStatusCode httpStatusCode,
                                                    string responseMessage,
                                                    IReadOnlyDictionary<string, string> headers)
    {
        HttpClient client =
            new(new FakeHttpMessageHandler(statusCode: httpStatusCode, responseMessage: responseMessage, headers: headers)) { BaseAddress = new("https://localhost") };

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
    ///     Mocks the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="headers">Headers to add to the response.</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    public static void MockCreateClientWithResponse(this IHttpClientFactory httpClientFactory,
                                                    string clientName,
                                                    HttpStatusCode httpStatusCode,
                                                    IReadOnlyDictionary<string, string> headers)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory, clientName: clientName, httpStatusCode: httpStatusCode, responseMessage: string.Empty, headers: headers);
    }

    /// <summary>
    ///     Creates the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseObject">Response object to return.</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    public static void MockCreateClientWithResponse<T>(this IHttpClientFactory httpClientFactory, string clientName, HttpStatusCode httpStatusCode, T responseObject)
    {
        MockCreateClientWithResponse(httpClientFactory: httpClientFactory,
                                     clientName: clientName,
                                     httpStatusCode: httpStatusCode,
                                     JsonSerializer.Serialize(value: responseObject, options: SerializerOptions),
                                     headers: NoHeaders);
    }

    /// <summary>
    ///     Creates the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseObject">Response object to return.</param>
    /// <param name="headers">Headers to add to the response.</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
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

    /// <summary>
    ///     Creates the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseObject">Response object to return.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options to use.</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
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

    /// <summary>
    ///     Creates the client
    /// </summary>
    /// <param name="httpClientFactory">The Http Client Factory</param>
    /// <param name="clientName">The client name.</param>
    /// <param name="httpStatusCode">HTTP status code to be returned.</param>
    /// <param name="responseObject">Response object to return.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options to use.</param>
    /// <param name="headers">Headers to add to the response.</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
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