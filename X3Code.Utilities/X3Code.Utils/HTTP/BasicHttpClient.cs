using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace X3Code.Utils.HTTP;

/// <summary>
/// A basic HTTP client encapsulation for performing HTTP requests.
/// </summary>
public abstract class BasicHttpClient : IDisposable, IBasicHttpClient
{
    private  readonly HttpClient _client;
    
    protected BasicHttpClient(HttpClientConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.BaseUrl)) throw new Exception("No Base-URL set in HTTP Client configuration. Please set Base-URL first.");

        Configuration = configuration;
        _client = new HttpClient();
        _client.BaseAddress = new Uri(configuration.BaseUrl);
        _client.Timeout = configuration.TimeOut;
        foreach (var header in configuration.Header)
        {
            _client.DefaultRequestHeaders.Add(header.Key, header.Value);   
        }
    }

    /// <summary>
    /// The configuration which is used for the client
    /// </summary>
    protected HttpClientConfiguration Configuration { get; }

    /// <summary>
    /// The underlying HTTP-Client if needed directly
    /// </summary>
    protected HttpClient Client { get; set; }

    #region Client
    
    /// <summary>
    /// Adds a header to the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    public void AddHeader(string header, string value)
    {
        if (string.IsNullOrWhiteSpace(header)) throw new Exception("Empty header is not allowed.");
        if (string.IsNullOrWhiteSpace(value)) throw new Exception("Empty value is not allowed.");

        if (_client.DefaultRequestHeaders.Contains(header))
        {
            _client.DefaultRequestHeaders.Remove(header);
        }

        _client.DefaultRequestHeaders.Add(header, value);
    }

    /// <summary>
    /// Removes a header from the default request headers of the HTTP client.
    /// </summary>
    /// <param name="header">The name of the header to remove.</param>
    public void RemoveHeader(string header)
    {
        if (string.IsNullOrWhiteSpace(header)) throw new Exception("Empty header is not allowed.");
        if (_client.DefaultRequestHeaders.Contains(header))
        {
            _client.DefaultRequestHeaders.Remove(header);
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
        if (string.IsNullOrWhiteSpace(username)) throw new Exception("Username is null or empty.");
        if (string.IsNullOrEmpty(password)) throw new Exception("Password is null or empty.");

        var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        if (_client.DefaultRequestHeaders.Authorization != null)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
        }

        _client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedCredentials}");
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
        if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token is null or empty.");
        var bearerToken = token.StartsWith("Bearer ") ? token : $"Bearer {token}";

        // Remove the bearer token header if it exists
        if (_client.DefaultRequestHeaders.Authorization != null)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
        }

        // Add the new bearer token header
        _client.DefaultRequestHeaders.Add("Authorization", bearerToken);
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
        if (_client.DefaultRequestHeaders.Authorization != null)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
        }
    }
    
    #endregion

    #region HTTP Methods
    
    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
    /// </summary>
    /// <param name="uri">The URI of the resource to request.</param>
    /// <returns>
    /// The response body as a string.
    /// </returns>
    public async Task<string> GetAsync(string uri)
    {
        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Sends a GET request to the specified <paramref name="uri"/> and returns the response as an object of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response object.</typeparam>
    /// <param name="uri">The URI of the request.</param>
    /// <returns>The response object of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse?> GetAsync<TResponse>(string uri) where TResponse : class
    {
        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var res = response.Content.ReadAsStringAsync();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided JSON data.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON data to send in the request body.</param>
    /// <returns>The response content as a string.</returns>
    public async Task<string> PutAsync(string uri, string json)
    {
        var data = new StringContent(json, Configuration.DefaultEncoding, Configuration.MediaType);

        var response = await _client.PutAsync(uri, data);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Sends an HTTP PUT request to the specified URI with the provided data object.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The object data to send in the request body.</param>
    /// <returns>On success the cast object</returns>
    public async Task<TResponse?> PutAsync<TMessage, TResponse>(string uri, TMessage content) where TMessage : class where TResponse : class
    {
        var response = await _client.PutAsJsonAsync(uri, content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided JSON payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="json">The JSON payload to include in the request body.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    public async Task<string> PostAsync(string uri, string json)
    {
        var data = new StringContent(json, Configuration.DefaultEncoding, Configuration.MediaType);

        var response = await _client.PostAsync(uri, data);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
    
    /// <summary>
    /// Sends an HTTP POST request to the specified URI, with the provided data payload.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <param name="content">The data payload to include in the request body.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the response content as a string.</returns>
    public async Task<TResponse?> PostAsync<TMessage, TResponse>(string uri, TMessage content) where TMessage : class where TResponse : class
    {
        var response = await _client.PostAsJsonAsync(uri, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and returns the response content as a string.
    /// </summary>
    /// <param name="uri">The URI to send the request to.</param>
    /// <returns>The response content as a string.</returns>
    public async Task<string> DeleteAsync(string uri)
    {
        var response = await _client.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and returns the deserialized response object of type TResponse.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response object.</typeparam>
    /// <param name="uri">The URI to send the DELETE request to.</param>
    /// <returns>The deserialized response object of type TResponse.</returns>
    public async Task<TResponse?> DeleteAsync<TResponse>(string uri) where TResponse : class
    {
        var response = await _client.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
    
    /// <summary>
    /// Downloads a stream from the specified URI.
    /// </summary>
    /// <param name="uri">The URI to download the stream from.</param>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains the stream that was downloaded.</returns>
    public async Task<Stream> DownloadStreamAsync(string uri)
    {
        return await _client.GetStreamAsync(uri);
    }


    /// <summary>
    /// Uploads a stream to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to upload the stream to.</param>
    /// <param name="stream">The Stream to be uploaded.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UploadStreamAsync(string uri, Stream stream)
    {
        var content = new StreamContent(stream);
        var response = await _client.PostAsync(uri, content);

        response.EnsureSuccessStatusCode();
    }

    #endregion
    
    public void Dispose()
    {
        _client.Dispose();
    }
}