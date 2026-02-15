using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using X3Code.Utils.Serializing;

namespace X3Code.Utils.Environment;

public class ConfigurationLoader : IConfigurationLoader
{
    private SoftwareEnvironment _environmentByEnum = SoftwareEnvironment.Unknown;
    private string? _environmentName;

    public string Environment
    {
        get
        {
            if (_environmentByEnum != SoftwareEnvironment.Unknown) return _environmentByEnum.ToString();
            return _environmentName ?? string.Empty;
        }
    }

    /// <summary>
    /// Initializes a new instance of ConfigurationLoader with X3-Code default environment.
    /// </summary>
    /// <param name="environmentByEnum">The environment to set. Can be used for selection configuration files.</param>
    public ConfigurationLoader(SoftwareEnvironment environmentByEnum)
    {
        _environmentByEnum = environmentByEnum;
    }

    /// <summary>
    /// Initializes a new instance of ConfigurationLoader with X3-Code default environment.
    /// Use this overload if you want to overwrite the default environment.
    /// <remarks>
    /// Not allowed: whitespaces, special characters without "." "-" "_"
    /// max length 20 digits
    /// </remarks>
    /// </summary>
    /// <param name="environment"></param>
    public ConfigurationLoader(string environment)
    {
        if (string.IsNullOrWhiteSpace(environment)) throw new ArgumentNullException(nameof(environment));
        if (environment.Length > 20) throw new ArgumentException("Environment name cannot be longer than 20 characters.");
        if (environment.Contains(' ')) throw new ArgumentException("Environment name cannot contain whitespaces.");
        if (environment.Any(c => !char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_')) throw new ArgumentException("Environment name cannot contain special characters other than . - _");
        
        _environmentName = environment;
    }

    public T Load<T>(string pathToConfigFile, string fileName, bool useEnvironment = true) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(pathToConfigFile)) throw new ArgumentNullException(nameof(pathToConfigFile));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        
        var fileExtension = Path.GetExtension(fileName);

        if (useEnvironment)
        {
            var fileNameWithouthExtension = Path.GetFileNameWithoutExtension(fileName);
            fileName = $"{fileNameWithouthExtension}-{Environment.ToString().ToLowerInvariant()}.{fileExtension}";
        }
        
        var fullPath = GetFullPath(pathToConfigFile, fileName);
        
        if (fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase)) 
            return LoadConfigurationFromJson<T>(fullPath);
        if (fileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase)) 
            return LoadConfigurationFromXml<T>(fullPath);
        
        throw new Exception($"Unsupported file extension [{fileExtension}] for configuration file [{fileName}]. This Loader only supports .json and .xml files.");
    }
    
    private T LoadConfigurationFromJson<T>(string fullPath) where T : class, new()
    {
        if (!File.Exists(fullPath)) throw new FileNotFoundException($"Configuration file [{fullPath}] not found.");
        
        var json = File.ReadAllText(fullPath);
        if (string.IsNullOrWhiteSpace(json)) throw new Exception($"Configuration file  [{fullPath}] is empty.");
        
        var result = JsonSerializer.Deserialize<T>(json);
        if (result == null) 
            throw new Exception($"Failed to deserialize configuration file at [{fullPath}].");
        
        return result;
    }
    
    private T LoadConfigurationFromXml<T>(string fullPath) where T : class, new()
    {
        if (!File.Exists(fullPath)) throw new FileNotFoundException($"Configuration file [{fullPath}] not found.");

        var result = GenericXmlSerializer.LoadXml<T>(fullPath);
        if (result == null) 
            throw new Exception($"Failed to deserialize configuration file at [{fullPath}].");
        
        return result;
    }

    private string GetFullPath(string pathToConfigFile, string path)
    {
        string configPath = pathToConfigFile;

        // On Linux/macOS, if path starts with ".", resolve it relative to user home directory
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && configPath.StartsWith("."))
        {
            string homeDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            configPath = Path.Combine(homeDirectory, configPath);
        }

        string fullPath = Path.Combine(configPath, path);
        return fullPath;
    }
}