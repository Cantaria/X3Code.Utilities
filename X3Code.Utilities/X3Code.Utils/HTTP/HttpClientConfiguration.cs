using System.Collections.Generic;
using System.Text;

namespace X3Code.Utils.HTTP;

public class HttpClientConfiguration
{
    public Dictionary<string, string> Header { get; } = new();

    public string? BaseUrl { get; set; }

    public string MediaType { get; set; } = "application/json";

    public Encoding DefaultEncoding { get; set; } = Encoding.UTF8;
}