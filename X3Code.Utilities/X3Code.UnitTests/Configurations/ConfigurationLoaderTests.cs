using System;
using System.IO;
using System.Text.Json;
using X3Code.Utils.Configurations;
using X3Code.Utils.Serializing;
using Xunit;

namespace X3Code.UnitTests.Configurations;

public class ConfigurationLoaderTests : IDisposable
{
    private readonly string _testPath;

    public ConfigurationLoaderTests()
    {
        _testPath = Path.Combine(Path.GetTempPath(), "ConfigurationLoaderTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }

    public class TestConfig
    {
        public string Key { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [Fact]
    public void Constructor_WithEnum_SetsEnvironment()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        Assert.Equal("Development", loader.Environment);
    }

    [Fact]
    public void Constructor_WithUnknownEnum_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ConfigurationLoader(SoftwareEnvironment.Unknown));
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("dev-test")]
    [InlineData("v1.0_stable")]
    public void Constructor_WithString_SetsEnvironment(string envName)
    {
        var loader = new ConfigurationLoader(envName);
        Assert.Equal(envName, loader.Environment);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithString_NullOrEmpty_ThrowsArgumentNullException(string? envName)
    {
        Assert.Throws<ArgumentNullException>(() => new ConfigurationLoader(envName!));
    }

    [Fact]
    public void Constructor_WithString_TooLong_ThrowsArgumentException()
    {
        var longName = new string('a', 21);
        Assert.Throws<ArgumentException>(() => new ConfigurationLoader(longName));
    }

    [Fact]
    public void Constructor_WithString_Whitespaces_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ConfigurationLoader("Dev Env"));
    }

    [Theory]
    [InlineData("dev!")]
    [InlineData("dev@test")]
    [InlineData("dev#")]
    public void Constructor_WithString_InvalidCharacters_ThrowsArgumentException(string invalidName)
    {
        Assert.Throws<ArgumentException>(() => new ConfigurationLoader(invalidName));
    }

    [Fact]
    public void Load_Json_WithEnvironmentSuffix()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        var config = new TestConfig { Key = "Test", Value = 42 };
        var fileName = "config.json";
        var expectedFileName = "config.development.json";
        var fullPath = Path.Combine(_testPath, expectedFileName);
        
        File.WriteAllText(fullPath, JsonSerializer.Serialize(config));

        var loadedConfig = loader.Load<TestConfig>(_testPath, fileName, useEnvironment: true);

        Assert.NotNull(loadedConfig);
        Assert.Equal(config.Key, loadedConfig.Key);
        Assert.Equal(config.Value, loadedConfig.Value);
    }

    [Fact]
    public void Load_Json_WithoutEnvironmentSuffix()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        var config = new TestConfig { Key = "TestNoEnv", Value = 100 };
        var fileName = "direct.json";
        var fullPath = Path.Combine(_testPath, fileName);
        
        File.WriteAllText(fullPath, JsonSerializer.Serialize(config));

        var loadedConfig = loader.Load<TestConfig>(_testPath, fileName, useEnvironment: false);

        Assert.NotNull(loadedConfig);
        Assert.Equal(config.Key, loadedConfig.Key);
        Assert.Equal(config.Value, loadedConfig.Value);
    }

    [Fact]
    public void Load_Xml_WithEnvironmentSuffix()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Staging);
        var config = new TestConfig { Key = "XmlTest", Value = 77 };
        var fileName = "settings.xml";
        var expectedFileName = "settings.staging.xml";
        var fullPath = Path.Combine(_testPath, expectedFileName);
        
        GenericXmlSerializer.SaveXml(fullPath, config);

        var loadedConfig = loader.Load<TestConfig>(_testPath, fileName, useEnvironment: true);

        Assert.NotNull(loadedConfig);
        Assert.Equal(config.Key, loadedConfig.Key);
        Assert.Equal(config.Value, loadedConfig.Value);
    }

    [Fact]
    public void Load_Throws_FileNotFound()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        Assert.Throws<FileNotFoundException>(() => loader.Load<TestConfig>(_testPath, "nonexistent.json", useEnvironment: false));
    }

    [Fact]
    public void Load_Throws_UnsupportedExtension()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        var fileName = "config.txt";
        var fullPath = Path.Combine(_testPath, fileName);
        File.WriteAllText(fullPath, "some content");

        var exception = Assert.Throws<Exception>(() => loader.Load<TestConfig>(_testPath, fileName, useEnvironment: false));
        Assert.Contains("Unsupported file extension", exception.Message);
    }

    [Fact]
    public void Load_Json_Throws_EmptyFile()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        var fileName = "empty.json";
        var fullPath = Path.Combine(_testPath, fileName);
        File.WriteAllText(fullPath, "");

        var exception = Assert.Throws<Exception>(() => loader.Load<TestConfig>(_testPath, fileName, useEnvironment: false));
        Assert.Contains("is empty", exception.Message);
    }

    [Fact]
    public void Load_InvalidJson_Throws_Exception()
    {
        var loader = new ConfigurationLoader(SoftwareEnvironment.Development);
        var fileName = "invalid.json";
        var fullPath = Path.Combine(_testPath, fileName);
        File.WriteAllText(fullPath, "{ invalid json }");

        Assert.Throws<JsonException>(() => loader.Load<TestConfig>(_testPath, fileName, useEnvironment: false));
    }
}
