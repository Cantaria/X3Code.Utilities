using System;
using System.Collections.Generic;
using System.Text;

namespace X3Code.Utils.HTTP;

/// <summary>
/// Configuration class for HttpClient.
/// </summary>
public class HttpClientConfiguration
{
    /// <summary>
    /// Sets the default headers the HTTP client should use oin any request.
    /// </summary>
    public Dictionary<string, string> Header { get; } = new();

    /// <summary>
    /// The base url the HTTP client should use. for example https://www.google.com/api
    /// This is the base url all requests should have in common. The API paths after the base url are set in the specific HTTP-Methods.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// The media type which should be used by default.
    /// <remarks>Default: application/json</remarks>
    /// </summary>
    public string MediaType { get; set; } = "application/json";

    /// <summary>
    /// The default encoding which should be used.
    /// <remarks>Default: UTF8</remarks>
    /// </summary>
    public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
    
    /// <summary>
    /// Connection time out for every request.
    ///
    /// With this property you can set the timeout which should be used by the HTTP client. If you don't set this explicit,
    /// the Microsoft default of 100 seconds will be used.
    /// <remarks>Default: 100 Seconds</remarks>
    /// </summary>
    public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(100);
}