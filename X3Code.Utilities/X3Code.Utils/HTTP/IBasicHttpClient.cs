using System.IO;
using System.Threading.Tasks;

namespace X3Code.Utils.HTTP;

public interface IBasicHttpClient
{
    /// <summary>
    /// Adds a header to the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    void AddHeader(string header, string value);

    /// <summary>
    /// Removes a header from the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header to remove.</param>
    void RemoveHeader(string header);

    /// <summary>
    /// Adds username and password authentication to the HTTP client.
    /// </summary>
    /// <param name="username">User's username.</param>
    /// <param name="password">User's password.</param>
    void AddBasicAuthentication(string username, string password);

    /// <summary>
    /// Updates username and password authentication in the HTTP client.
    /// </summary>
    /// <param name="username">User's new username.</param>
    /// <param name="password">User's new password.</param>
    void UpdateBasicAuthentication(string username, string password);

    /// <summary>
    /// Adds a bearer token to the default request headers of the HTTP client.
    /// <remarks>Adds "Bearer " at the beginning if missing.</remarks>
    /// </summary>
    /// <param name="token">The bearer token.</param>
    void AddBearerToken(string token);

    /// <summary>
    /// Renews the bearer token in the default request headers of the HTTP client.
    /// </summary>
    /// <param name="token">The new bearer token to be added.</param>
    void RenewBearerToken(string token);

    /// <summary>
    /// Removes the bearer token from the default request headers of the HTTP client.
    /// </summary>
    void RemoveAuthentication();

    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
    /// </summary>
    /// <param name="uri">The URI of the resource to request.</param>
    /// <returns>
    /// The response body as a string.
    /// </returns>
    Task<string> GetAsync(string uri);

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided JSON data.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON data to send in the request body.</param>
    /// <returns>The response content as a string.</returns>
    Task<string> PutAsync(string uri, string json);

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided data object.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The object data to send in the request body.</param>
    /// <returns>On success the cast object</returns>
    Task<TResponse?> PutAsync<TMessage, TResponse>(string uri, TMessage content) where TMessage : class where TResponse : class;

    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided JSON payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON payload to include in the request body.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    Task<string> PostAsync(string uri, string json);

    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided data payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The data payload to include in the request body.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    Task<TResponse?> PostAsync<TMessage, TResponse>(string uri, TMessage content) where TMessage : class where TResponse : class;

    /// <summary>
    /// Sends a DELETE request to the specified URI and returns the response content as a string.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <returns>The response content as a string.</returns>
    Task<string> DeleteAsync(string uri);

    /// <summary>
    /// Downloads a stream from the specified URI.
    /// </summary>
    /// <param name="uri">The URI to download the stream from.</param>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains the stream that was downloaded.</returns>
    Task<Stream> DownloadStreamAsync(string uri);

    /// <summary>
    /// Uploads a stream to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to upload the stream to.</param>
    /// <param name="stream">The Stream to be uploaded.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UploadStreamAsync(string uri, Stream stream);
}