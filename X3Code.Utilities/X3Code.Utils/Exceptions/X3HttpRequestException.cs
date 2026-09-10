using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace X3Code.Utils.Exceptions;

/// <summary>
/// Thrown when an HTTP request made through the X3Code HTTP client comes back with a failure status.
/// Carries the response body that came with it.
/// </summary>
/// <remarks>
/// The body is captured as a string while the response is still alive, rather than the
/// <see cref="HttpResponseMessage"/> itself. Holding on to the response meant holding an
/// <see cref="IDisposable"/> that nobody disposed and that could already be gone by the time
/// somebody looked at it - an exception is often inspected far away from where it was thrown.
/// </remarks>
public class X3HttpRequestException : HttpRequestException
{
    /// <summary>
    /// Creates the exception for a failed request.
    /// </summary>
    /// <param name="message">The message describing the failure.</param>
    /// <param name="innerException">The exception raised by <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/>.</param>
    /// <param name="statusCode">The status code the server answered with.</param>
    /// <param name="responseContent">The response body, or null if it could not be read.</param>
    public X3HttpRequestException(string message, Exception? innerException, HttpStatusCode? statusCode, string? responseContent)
        : base(message, innerException, statusCode)
    {
        ResponseContent = responseContent;
    }

    /// <summary>
    /// The response body as it came from the server, or null if there was none, or it could not be read.
    /// The status code is on <see cref="HttpRequestException.StatusCode"/>.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Deserializes <see cref="ResponseContent"/> into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type the body should be read as.</typeparam>
    /// <param name="options">Optional serializer options.</param>
    /// <returns>The deserialized object, or the default of <typeparamref name="T"/> if the body is empty or does not fit.</returns>
    /// <remarks>
    /// Actually a "Try" now: it returns the default instead of throwing, and it is synchronous
    /// because the body was already read when the exception was built. The previous version
    /// promised "Try" in its name while going back to the network and throwing on both counts.
    /// </remarks>
    public T? TryParseResponseContent<T>(JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(ResponseContent)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(ResponseContent, options);
        }
        catch (JsonException)
        {
            return default;
        }
    }
}
