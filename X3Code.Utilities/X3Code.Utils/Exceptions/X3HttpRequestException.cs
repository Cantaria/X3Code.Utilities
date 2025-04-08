using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace X3Code.Utils.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an HTTP request fails in the context of X3Code-HttpClient.
/// This exception is a specialized form of <see cref="HttpRequestException"/> that provides access to the response content of the failed HTTP request.
/// </summary>
public class X3HttpRequestException : HttpRequestException
{
    /// <summary>
    /// Represents an exception that occurs during the execution of an HTTP request in the X3Code-HttpClient context.
    /// </summary>
    /// <remarks>
    /// This exception extends <see cref="HttpRequestException"/> and includes additional details such as the HTTP response content,
    /// providing a more comprehensive context for handling request failures in HTTP operations.
    /// </remarks>
    public X3HttpRequestException(string message, Exception exception, HttpResponseMessage response) : base(message, exception)
    {
        ResponseContent = response;
    }

    /// <summary>
    /// Gets the HTTP response message returned by the server in the event of a failed HTTP request.
    /// </summary>
    /// <remarks>
    /// This property provides direct access to the HTTP response, enabling inspection of the status code, headers,
    /// and other response details. It is particularly useful for analyzing the server's response to diagnose issues
    /// or for extracting content from the failed response.
    /// </remarks>
    public HttpResponseMessage ResponseContent { get; }

    /// <summary>
    /// Attempts to parse the HTTP response content into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type into which the response content should be deserialized.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized object of type <typeparamref name="T"/>,
    /// or null if the content cannot be deserialized.
    /// </returns>
    public async Task<T?> TryParseResponseContent<T>()
    {
        return await ResponseContent.Content.ReadFromJsonAsync<T>();
    } 
}