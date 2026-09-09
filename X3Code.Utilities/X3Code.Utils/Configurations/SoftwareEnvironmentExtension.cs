using System;

namespace X3Code.Utils.Configurations;

public static class SoftwareEnvironmentExtension
{
    public static SoftwareEnvironment ToEnum(this string source)
    {
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source));
        
        return Enum.Parse<SoftwareEnvironment>(source);
    }
}