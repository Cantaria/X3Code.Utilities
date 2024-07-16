using System;
using Newtonsoft.Json;

namespace X3Code.UnitTests.HTTP.Models;

[Serializable]
internal class AddObjectResponseModel
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("data")] 
    public Data? Payload { get; set; }
    
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
}