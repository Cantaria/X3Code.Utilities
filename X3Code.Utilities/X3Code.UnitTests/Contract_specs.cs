using System;
using System.IO;
using System.Text.Json;
using X3Code.Azure.Utils.Storage;
using X3Code.TestSupport;
using X3Code.Utils.Configurations;
using X3Code.Utils.HTTP;
using Xunit;

namespace X3Code.UnitTests;

/// <summary>
/// Holds the interfaces of X3Code.Utilities and X3Code.Azure.Utils against their implementations.
/// The parameters of IConfigurationLoader.Load were declared in the opposite order to the
/// implementation for a long time; the compiler cannot see that, because parameter names are not
/// part of the signature it matches on. Only a call through the interface shows it.
/// </summary>
public class Contract_specs
{
    #region The detector has to be able to fail

    private interface IDeliberatelyDrifted
    {
        void Do(string first, string second, int count = 1);
    }

    private class DeliberatelyDrifted : IDeliberatelyDrifted
    {
        public void Do(string second, string first, int count = 2) { }
    }

    [Fact]
    public void The_detector_finds_swapped_names_and_differing_defaults()
    {
        var drift = InterfaceContract.FindDrift(typeof(IDeliberatelyDrifted), typeof(DeliberatelyDrifted));

        // Ohne diesen Test koennten die Vertragstests unten stumm gruen sein, weil der Detektor
        // gar nichts prueft.
        Assert.Contains(drift, d => d.Contains("'first'") && d.Contains("'second'"));
        Assert.Contains(drift, d => d.Contains("defaults to"));
    }

    #endregion

    [Theory]
    [InlineData(typeof(IConfigurationLoader), typeof(ConfigurationLoader))]
    [InlineData(typeof(IBasicHttpClient), typeof(BasicHttpClient))]
    [InlineData(typeof(IAzureStorage), typeof(AzureBlobStorage))]
    [InlineData(typeof(IAzureStorage), typeof(StorageBase))]
    public void Interface_and_implementation_agree(Type interfaceType, Type implementationType)
    {
        var drift = InterfaceContract.FindDrift(interfaceType, implementationType);

        Assert.True(drift.Count == 0, string.Join(Environment.NewLine, drift));
    }

    [Fact]
    public void Loading_through_the_interface_with_named_arguments_finds_the_file()
    {
        var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directory);
        try
        {
            File.WriteAllText(Path.Combine(directory, "settings.json"), JsonSerializer.Serialize(new Settings { Name = "from-file" }));

            IConfigurationLoader loader = new ConfigurationLoader(SoftwareEnvironment.Development);

            // Benannte Argumente sind der Fall, den die vertauschte Deklaration still zerlegte:
            // der Pfad landete im fileName-Parameter und umgekehrt.
            var settings = loader.Load<Settings>(fileName: "settings.json", pathToConfigFile: directory, useEnvironment: false);

            Assert.Equal("from-file", settings.Name);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private class Settings
    {
        public string Name { get; set; } = string.Empty;
    }
}
