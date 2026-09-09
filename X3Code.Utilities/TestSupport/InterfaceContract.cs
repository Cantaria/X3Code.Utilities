using System;
using System.Collections.Generic;
using System.Reflection;

namespace X3Code.TestSupport;

/// <summary>
/// Compares what an interface declares against what the implementing class actually offers.
/// </summary>
/// <remarks>
/// Two defects slipped through in this repository because interface and implementation were
/// maintained separately and no test ever went through the interface:
/// <list type="bullet">
/// <item><c>IConfigurationLoader.Load</c> declared <c>(pathToConfigFile, fileName)</c> while the
/// implementation had <c>(fileName, pathToConfigFile)</c>. Both are strings, so it compiled - and a
/// caller using named arguments got them bound across at runtime.</item>
/// <item><c>IRepository.BulkWrite</c> kept two required, non-nullable parameters after the
/// implementation had made them optional and nullable.</item>
/// </list>
/// The compiler cannot see either: parameter names, default values and nullability are not part of
/// the signature it matches on. This helper checks exactly those three.
/// <para>
/// The file is shared into the test projects with <c>&lt;Compile Include&gt;</c>. Unlike the shared
/// sources of the libraries it needs no namespace or visibility trick — test assemblies carry
/// <c>IsPackable=false</c> and are never referenced by anyone, so no consumer can see it twice.
/// </para>
/// </remarks>
internal static class InterfaceContract
{
    /// <summary>
    /// Every place where <paramref name="implementationType"/> differs from what
    /// <paramref name="interfaceType"/> promises. Empty means they agree.
    /// </summary>
    /// <param name="interfaceType">The declared contract. A closed type for generic interfaces.</param>
    /// <param name="implementationType">The class implementing it, closed as well.</param>
    /// <returns>One readable line per difference.</returns>
    internal static IReadOnlyList<string> FindDrift(Type interfaceType, Type implementationType)
    {
        var drift = new List<string>();
        var map = implementationType.GetInterfaceMap(interfaceType);
        var nullability = new NullabilityInfoContext();

        for (var i = 0; i < map.InterfaceMethods.Length; i++)
        {
            var declared = map.InterfaceMethods[i];
            var implemented = map.TargetMethods[i];

            var declaredParameters = declared.GetParameters();
            var implementedParameters = implemented.GetParameters();

            for (var p = 0; p < declaredParameters.Length; p++)
            {
                var expected = declaredParameters[p];
                var actual = implementedParameters[p];
                var where = $"{interfaceType.Name}.{declared.Name}, parameter {p}";

                if (expected.Name != actual.Name)
                {
                    drift.Add($"{where}: called '{expected.Name}' on the interface but '{actual.Name}' on {implementationType.Name}. " +
                              "A caller using named arguments through the interface binds to the wrong parameter.");
                }

                if (expected.HasDefaultValue != actual.HasDefaultValue)
                {
                    var optionalOn = expected.HasDefaultValue ? "the interface" : implementationType.Name;
                    drift.Add($"{where} ('{expected.Name}'): optional on {optionalOn} only.");
                }
                else if (expected.HasDefaultValue && !Equals(expected.DefaultValue, actual.DefaultValue))
                {
                    drift.Add($"{where} ('{expected.Name}'): defaults to '{expected.DefaultValue}' on the interface " +
                              $"but to '{actual.DefaultValue}' on {implementationType.Name}.");
                }

                var expectedNullable = nullability.Create(expected).WriteState == NullabilityState.Nullable;
                var actualNullable = nullability.Create(actual).WriteState == NullabilityState.Nullable;
                if (expectedNullable != actualNullable)
                {
                    var nullableOn = expectedNullable ? "the interface" : implementationType.Name;
                    drift.Add($"{where} ('{expected.Name}'): nullable on {nullableOn} only.");
                }
            }
        }

        return drift;
    }
}
