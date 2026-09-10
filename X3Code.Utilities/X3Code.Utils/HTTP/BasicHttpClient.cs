using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using X3Code.Utils.Exceptions;

namespace X3Code.Utils.HTTP;

/// <summary>
/// A basic HTTP client encapsulation for performing HTTP requests.
/// </summary>
public abstract class BasicHttpClient : IDisposable, IBasicHttpClient
{
    /// <param name="configuration">The client configuration. BaseUrl has to be set.</param>
    /// <param name="logger">
    /// Optional. Without it nothing is logged and nothing is allocated for it. Pass one - or let the
    /// container hand one in - to see failed requests with their status and response body. Which
    /// logger sits behind the abstraction is the application's decision; this library brings none.
    /// </param>
    protected BasicHttpClient(HttpClientConfiguration configuration, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(configuration.BaseUrl)) throw new ArgumentException("No Base-URL set in HTTP client configuration. Set BaseUrl first.", nameof(configuration));

        Logger = logger ?? NullLogger.Instance;
        Configuration = configuration;
        Client = CreateHttpClient(configuration);
        Client.BaseAddress = new Uri(configuration.BaseUrl);
        Client.Timeout = configuration.TimeOut;
        foreach (var header in configuration.Header)
        {
            Client.DefaultRequestHeaders.Add(header.Key, header.Value);   
        }
    }

    /// <summary>
    /// The logger of this client. Never null - it is the no-op logger when none was handed in.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The configuration which is used for the client
    /// </summary>
    protected HttpClientConfiguration Configuration { get; }

    /// <summary>
    /// The underlying HTTP-Client if needed directly
    /// </summary>
    protected HttpClient Client { get; }

    private static HttpClient CreateHttpClient(HttpClientConfiguration configuration)
    {
        if (!configuration.IgnoreCertificateErrors) return new HttpClient();

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        return new HttpClient(handler);
    }

    #region Client
    
    /// <summary>
    /// Adds a header to the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    public void AddHeader(string header, string value)
    {
        if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException("Empty header is not allowed.", nameof(header));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Empty value is not allowed.", nameof(value));

        if (Client.DefaultRequestHeaders.Contains(header))
        {
            Client.DefaultRequestHeaders.Remove(header);
        }

        Client.DefaultRequestHeaders.Add(header, value);
    }

    /// <summary>
    /// Removes a header from the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header to remove.</param>
    public void RemoveHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException("Empty header is not allowed.", nameof(header));
        if (Client.DefaultRequestHeaders.Contains(header))
        {
            Client.DefaultRequestHeaders.Remove(header);
        }
    }
    
    #endregion

    #region Authentication
    
    #region Username & Password Authentication
    
    /// <summary>
    /// Adds username and password authentication to the HTTP client.
    /// </summary>
    /// <param name="username">User's username.</param>
    /// <param name="password">User's password.</param>
    public void AddBasicAuthentication(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is null or empty.", nameof(username));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is null or empty.", nameof(password));

        var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        if (Client.DefaultRequestHeaders.Authorization != null)
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
        }

        Client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedCredentials}");
    }


    /// <summary>
    /// Updates username and password authentication in the HTTP client.
    /// </summary>
    /// <param name="username">User's new username.</param>
    /// <param name="password">User's new password.</param>
    public void UpdateBasicAuthentication(string username, string password)
    {
        AddBasicAuthentication(username, password);
    }

    #endregion

    #region Bearer Authentication

    /// <summary>
    /// Adds a bearer token to the default request headers of the HTTP client.
    /// <remarks>Adds "Bearer " at the beginning if missing.</remarks>
    /// </summary>
    /// <param name="token">The bearer token.</param>
    public void AddBearerToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token is null or empty.", nameof(token));
        var bearerToken = token.StartsWith("Bearer ") ? token : $"Bearer {token}";

        // Remove the bearer token header if it exists
        if (Client.DefaultRequestHeaders.Authorization != null)
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
        }

        // Add the new bearer token header
        Client.DefaultRequestHeaders.Add("Authorization", bearerToken);
    }

    /// <summary>
    /// Renews the bearer token in the default request headers of the HTTP client.
    /// </summary>
    /// <param name="token">The new bearer token to be added.</param>
    public void RenewBearerToken(string token)
    {
        AddBearerToken(token);
    }
    
    #endregion
    
    /// <summary>
    /// Removes the bearer token from the default request headers of the HTTP client.
    /// </summary>
    public void RemoveAuthentication()
    {
        // Remove the bearer token header if it exists
        if (Client.DefaultRequestHeaders.Authorization != null)
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
        }
    }
    
    #endregion

    #region HTTP Methods
    
    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
    /// </summary>
    /// <param name="uri">The URI of the resource to request.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>
    /// The response body as a string.
    /// </returns>
    public async Task<string> GetAsync(string uri, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync(uri, cancellationToken);
        await CheckSuccessCodeAsync(response, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends a GET request to the specified <paramref name="uri"/> and returns the response as an object of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response object.</typeparam>
    /// <param name="uri">The URI of the request.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The response object of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse?> GetAsync<TResponse>(string uri, CancellationToken cancellationToken = default) where TResponse : class
    {
        var response = await Client.GetAsync(uri, cancellationToken);
        await CheckSuccessCodeAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided JSON data.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON data to send in the request body.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>The response content as a string.</returns>
    public async Task<string> PutAsync(string uri, string json, CancellationToken cancellationToken = default)
    {
        var data = new StringContent(json, Configuration.DefaultEncoding, Configuration.MediaType);

        var response = await Client.PutAsync(uri, data, cancellationToken);

        await CheckSuccessCodeAsync(response, cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided data object.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The object data to send in the request body.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>On success the cast object</returns>
    public async Task<TResponse?> PutAsync<TMessage, TResponse>(string uri, TMessage content, CancellationToken cancellationToken = default) where TMessage : class where TResponse : class
    {
        var response = await Client.PutAsJsonAsync(uri, content, cancellationToken);

        await CheckSuccessCodeAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided JSON payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON payload to include in the request body.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    public async Task<string> PostAsync(string uri, string json, CancellationToken cancellationToken = default)
    {
        var data = new StringContent(json, Configuration.DefaultEncoding, Configuration.MediaType);

        var response = await Client.PostAsync(uri, data, cancellationToken);

        await CheckSuccessCodeAsync(response, cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
    
    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided data payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The data payload to include in the request body.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    public async Task<TResponse?> PostAsync<TMessage, TResponse>(string uri, TMessage content, CancellationToken cancellationToken = default) where TMessage : class where TResponse : class
    {
        var response = await Client.PostAsJsonAsync(uri, content, cancellationToken);
        await CheckSuccessCodeAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and returns the response content as a string.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>The response content as a string.</returns>
    public async Task<string> DeleteAsync(string uri, CancellationToken cancellationToken = default)
    {
        var response = await Client.DeleteAsync(uri, cancellationToken);
        await CheckSuccessCodeAsync(response, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and returns the deserialized response object of type TResponse.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response object.</typeparam>
    /// <param name="uri">The URI to send the DELETE request to.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>The deserialized response object of type TResponse.</returns>
    public async Task<TResponse?> DeleteAsync<TResponse>(string uri, CancellationToken cancellationToken = default) where TResponse : class
    {
        var response = await Client.DeleteAsync(uri, cancellationToken);
        await CheckSuccessCodeAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
    
    /// <summary>
    /// Downloads a stream from the specified URI.
    /// </summary>
    /// <param name="uri">The URI to download the stream from.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains the stream that was downloaded.</returns>
    public async Task<Stream> DownloadStreamAsync(string uri, CancellationToken cancellationToken = default)
    {
        return await Client.GetStreamAsync(uri, cancellationToken);
    }


    /// <summary>
    /// Uploads a stream to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to upload the stream to.</param>
    /// <param name="stream">The Stream to be uploaded.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UploadStreamAsync(string uri, Stream stream, CancellationToken cancellationToken = default)
    {
        // Kein using auf den StreamContent: Dessen Dispose schließt den umschlossenen Stream,
        // also den des Aufrufers, der ihn nach dem Upload womöglich noch braucht. Disposed
        // wird stattdessen die Response, die HttpClient uns übergibt.
        using var response = await Client.PostAsync(uri, new StreamContent(stream), cancellationToken);

        await CheckSuccessCodeAsync(response, cancellationToken);
    }

    #endregion
    
    public void Dispose()
    {
        Client.Dispose();
    }

    private async Task CheckSuccessCodeAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        var uri = response.RequestMessage?.RequestUri?.ToString();

        // Den Body jetzt lesen, solange die Response noch lebt. Die Exception wird oft weit
        // entfernt ausgewertet, wo die Response längst verworfen ist.
        string? content = null;
        try
        {
            content = await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Body nicht lesbar - Statuscode und Meldung müssen dann reichen.
            HttpClientLog.ResponseBodyUnreadable(Logger, uri, exception);
        }

        HttpClientLog.RequestFailed(Logger, uri, response.StatusCode, content);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new X3HttpRequestException(ex.Message, ex, response.StatusCode, content);
        }
    }
}