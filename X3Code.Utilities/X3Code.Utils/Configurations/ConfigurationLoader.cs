using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using X3Code.Utils.Serializing;

namespace X3Code.Utils.Configurations;

/// <summary>
/// Provides functionality to load configuration files in different formats,
/// specifically for different software environments.
/// </summary>
public class ConfigurationLoader : IConfigurationLoader
{
    private readonly SoftwareEnvironment _environmentByEnum = SoftwareEnvironment.Unknown;

    /// <summary>
    /// Gets the name of the current software environment. This can be used to determine
    /// the specific configuration or behavior based on the environment type (e.g., Development,
    /// Test, Production). The environment name is represented as a <see cref="string"/>.
    /// </summary>
    /// <remarks>
    /// - If the environment is set using an enum value, this property returns the corresponding name.
    /// - If the environment is manually specified as a string, it strictly follows the allowed format:
    /// - Maximum length of 20 characters
    /// - No whitespaces
    /// - Allowed characters: alphanumeric, '.', '-', '_'.
    /// - If the environment is not explicitly set, it defaults to an empty string.
    /// </remarks>
    public string Environment
    {
        get
        {
            if (_environmentByEnum != SoftwareEnvironment.Unknown) return _environmentByEnum.ToString();
            return field ?? string.Empty;
        }
    }

    /// <summary>
    /// Initializes a new instance of ConfigurationLoader with X3-Code default environment.
    /// </summary>
    /// <param name="environmentByEnum">The environment to set. Can be used for selection configuration files.</param>
    public ConfigurationLoader(SoftwareEnvironment environmentByEnum)
    {
        if (environmentByEnum == SoftwareEnvironment.Unknown) throw new ArgumentException("Environment cannot be Unknown and should be known!");
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
        
        Environment = environment;
    }

    /// <summary>
    /// Loads a configuration of the specified type from the provided file and path.
    /// Supports JSON and XML file formats.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object to load. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="fileName">The name of the configuration file to be loaded.</param>
    /// <param name="pathToConfigFile">The absolute or relative path to the directory containing the configuration file. If relative, '~' can be used to denote the home directory.</param>
    /// <param name="useEnvironment">A boolean indicating whether to use the environment setting while loading the configuration.</param>
    /// <returns>An instance of the specified configuration type loaded from the provided file.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> or <paramref name="pathToConfigFile"/> is null or whitespace.</exception>
    /// <exception cref="Exception">Thrown if the file extension is unsupported or the file cannot be loaded.</exception>
    public T Load<T>(string fileName, string pathToConfigFile, bool useEnvironment = true) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(pathToConfigFile)) throw new ArgumentNullException(nameof(pathToConfigFile));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

        if (pathToConfigFile.StartsWith("~")) pathToConfigFile = pathToConfigFile.Replace("~", DetermineHomePath());

        var fileExtension = Path.GetExtension(fileName);

        if (useEnvironment)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            fileName = $"{fileNameWithoutExtension}.{Environment.ToLowerInvariant()}{fileExtension}";
        }

        var fullPath = GetFullPath(pathToConfigFile, fileName);

        if (fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            return LoadConfigurationFromJson<T>(fullPath);
        if (fileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
            return LoadConfigurationFromXml<T>(fullPath);

        throw new Exception(
            $"Unsupported file extension [{fileExtension}] for configuration file [{fileName}]. This Loader only supports .json and .xml files.");
    }

    private string DetermineHomePath()
    {
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
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

    private string GetFullPath(string pathToConfigFile, string filename)
    {
        string configPath = pathToConfigFile;

        // On Linux/macOS, if the path starts with ".", resolve it relative to the user home directory
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && configPath.StartsWith("."))
        {
            string homeDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            configPath = Path.Combine(homeDirectory, configPath);
        }

        string fullPath = Path.Combine(configPath, filename);
        return fullPath;
    }
}