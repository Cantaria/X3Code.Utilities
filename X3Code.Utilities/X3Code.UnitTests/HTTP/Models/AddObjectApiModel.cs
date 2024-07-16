using System;
using Newtonsoft.Json;

namespace X3Code.UnitTests.HTTP.Models;

[Serializable]
internal class AddObjectApiModel
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("data")]
    public Data? Payload { get; set; }
}

[Serializable]
internal class Data
{
    [JsonProperty("year")]
    public int Year { get; set; }
    
    [JsonProperty("price")]
    public double Price { get; set; }
    
    [JsonProperty("color")]
    public string? Color { get; set; }
    
    [JsonProperty("CPU_model")]
    public string? CpuModel { get; set; }
    
    [JsonProperty("Hard_disk_size")]
    public string? HardDiskSpace { get; set; }
}