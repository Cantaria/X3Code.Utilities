namespace X3Code.Utils.Configurations;

/// <summary>
/// Loads a configuration object from a JSON or XML file, optionally picking the file that belongs
/// to the current environment.
/// </summary>
public interface IConfigurationLoader
{
    /// <summary>
    /// The environment the loader was built for. Used to pick an environment-specific file.
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Loads a configuration of the specified type from the given file.
    /// </summary>
    /// <typeparam name="T">The configuration type to deserialize into.</typeparam>
    /// <param name="fileName">The file name including its extension, .json or .xml.</param>
    /// <param name="pathToConfigFile">The directory holding the file. A leading '~' resolves to the home directory.</param>
    /// <param name="useEnvironment">Whether the environment is taken into account when picking the file.</param>
    /// <returns>The deserialized configuration.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fileName"/> or <paramref name="pathToConfigFile"/> is null or whitespace.</exception>
    /// <exception cref="System.IO.FileNotFoundException">Thrown if the file does not exist.</exception>
    /// <exception cref="System.NotSupportedException">Thrown if the file extension is neither .json nor .xml.</exception>
    /// <exception cref="System.IO.InvalidDataException">Thrown if the file is empty or cannot be deserialized.</exception>
    /// <remarks>
    /// The parameter order is fileName first, then the path. It used to be the other way round here
    /// while the implementation had it as it is now - calling through this interface with named
    /// arguments bound the two across, and the compiler could not see it because both are strings.
    /// </remarks>
    T Load<T>(string fileName, string pathToConfigFile, bool useEnvironment = true) where T : class, new();
}
