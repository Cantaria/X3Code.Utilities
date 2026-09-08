using System;

namespace X3Code.Infrastructure.Tests.ConstValues;

/// <summary>
/// Connection string for the design-time tooling (<c>dotnet ef migrations</c>).
/// </summary>
/// <remarks>
/// The tests themselves do NOT use this. They get their connection string from
/// <see cref="Helper.SqlServerFixture"/>, which starts a throwaway SQL Server container.
/// This class exists only so the EF tooling can construct the context.
/// No credentials belong in this file - set the environment variable instead.
/// </remarks>
public static class DbConnectionStrings
{
    /// <summary>
    /// Environment variable holding the connection string for design-time commands.
    /// </summary>
    public const string EnvironmentVariableName = "X3_TEST_DB";

    /// <summary>
    /// Placeholder used when <see cref="EnvironmentVariableName"/> is not set. Carries no
    /// credentials; enough for "dotnet ef migrations add", which never opens a connection.
    /// </summary>
    private const string DesignTimeFallback =
        "Server=localhost;Database=UnitTest;Trusted_Connection=True;TrustServerCertificate=True";

    /// <summary>
    /// Returns the connection string from <see cref="EnvironmentVariableName"/>,
    /// or a credential-free placeholder if that variable is not set.
    /// </summary>
    public static string GetConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(EnvironmentVariableName);

        return string.IsNullOrWhiteSpace(fromEnvironment) ? DesignTimeFallback : fromEnvironment;
    }
}
