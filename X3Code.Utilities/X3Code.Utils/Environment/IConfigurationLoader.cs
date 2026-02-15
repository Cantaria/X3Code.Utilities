namespace X3Code.Utils.Environment;

public interface IConfigurationLoader
{
    string Environment { get; }
    T Load<T>(string pathToConfigFile, string fileName, bool useEnvironment = true) where T : class, new();
}