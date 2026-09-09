# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This repo is the source for the **X3-Code.Utilities** NuGet package family — seven independent .NET 10 class libraries under `X3Code.Utilities/`, each built and published as its own NuGet package. There is no single "app"; the deliverables are the packages themselves.

## Common commands

Run from `X3Code.Utilities/` (where the `.sln` lives):

```bash
dotnet build X3Code.Utilities.sln
```

```bash
dotnet test X3Code.Utilities.sln
```

Run a single test project:

```bash
dotnet test X3Code.UnitTests/X3Code.UnitTests.csproj
```

Run a single test by name:

```bash
dotnet test X3Code.UnitTests/X3Code.UnitTests.csproj --filter "FullyQualifiedName~MethodOrClassName"
```

All test projects use **xunit** and need no manual setup.

- `X3Code.UnitTests` and `X3Code.Repository.Test` are fully hermetic (the latter runs on SQLite/InMemory). One exception: `HttpClientTest` talks to a public third-party API and carries `[Trait("Category", "Integration")]`; CI runs the project with `--filter "Category!=Integration"`.
- **`X3Code.Infrastructure.Tests` requires a running Docker daemon.** `X3Code.Infrastructure.Tests/Helper/SqlServerFixture.cs` starts one throwaway SQL Server container (`Testcontainers.MsSql`, image pinned to `mcr.microsoft.com/mssql/server:2022-latest`) for the whole assembly, applies the migrations against it, and removes it afterwards. There are no connection strings to edit — `ConstValues/DbConnectionStrings.cs` only serves the design-time EF tooling and reads the `X3_TEST_DB` environment variable, defaulting to a credential-free placeholder.

`BulkWriterTest` is a performance suite that goes up to ten million rows; it carries `[Trait("Category", "Slow")]`. Skip it with `--filter "Category!=Slow"` (which is what CI does) and run it deliberately when touching `BulkWrite`.

All three projects are green.

## Architecture

### The seven packable libraries

| Project | PackageId | Purpose |
|---|---|---|
| `X3Code.Utils` | `X3Code.Utilities` | General-purpose helpers: config/env loading, converters, extension methods (Collection/DataRow/DataTable/DateTime/Dictionary/EntityProperty), a basic HTTP client, XML serialization, GUID tools, MIME helpers. Its only dependency is `Microsoft.Extensions.Logging.Abstractions` — see the logging section; keep it that way, anything needing a third-party package belongs in its own library |
| `X3Code.Sql.Utils` | `X3Code.Sql.Utils` | `DatabaseConnector` — lightweight ADO.NET access to SQL Server, no ORM. Split out of `X3Code.Utilities` because it was the only type there pulling in `Microsoft.Data.SqlClient` |
| `X3Code.Infrastructure` | `X3Code.Infrastructure` | Generic repository / unit-of-work pattern (`Entity`, `IEntity`, `IRepository`, `GenericRepository`) for EF Core against SQL Server |
| `X3Code.Repository` | `X3Code.Repository` | Newer take on the same repository pattern (`Entity`, `IEntity`, `IBaseRepository`, `BaseRepository`) plus `IUnitOfWork`. `BaseRepository<TEntity, TContext>` and `UnitOfWork<TContext>` name their context so a container with several contexts can resolve them; `AddX3CodeRepository<TContext>()` does the registration. The one-parameter forms taking a bare `DbContext` are kept for existing code |
| `X3Code.Infrastructure.Mobile` | `X3Code.Infrastructure.Mobile` | Same repository pattern as `X3Code.Infrastructure`, but built against EF Core Sqlite for mobile use. Maintained by someone outside this repo and deliberately **not** pushed by the publish script — that is intentional, not an oversight |
| `X3Code.Wasm.Utils` | `X3Code.Wasm.Utils` | Blazor WASM helpers, currently just a `LocalStorageService` wrapping JSInterop |
| `X3Code.Azure.Utils` | `X3Code.Azure.Utils` | Azure helpers, currently `AzureBlobStorage`/`IAzureStorage`/`StorageBase` |

Everything targets `net10.0` and has `GeneratePackageOnBuild=true`, so a normal `dotnet build` produces a `.nupkg` per packable project. `X3Code.Utilities/Directory.Build.props` sets `PackageOutputPath` to the repo-root `NuGet/` folder, so all built packages land there together (rather than in each project's own `bin/`) — that folder is also where `NuGet/deploy_Now.sh` (the current version of the publish script, see below) expects to find them. The two `Infrastructure*` repository-pattern projects and `X3Code.Repository` are intentionally separate, near-duplicate implementations (SQL Server vs Sqlite vs the newer pattern) — don't assume they should be merged.

### Cross-project code sharing without ProjectReference

Libraries deliberately do **not** use `<ProjectReference>` between each other (that would pull one package's dependency graph into another). Instead, a handful of files are shared by linking the same `.cs` file into a second project via `<Compile Include="..\OtherProject\Path.cs"><Link>...</Link></Compile>`:

- `X3Code.Repository` links `X3Code.Infrastructure/Extensions/DecimalExtension.cs`

If you touch one of these linked files, check both csproj files that reference it — you're editing shared source, not a private copy. Test projects, by contrast, *do* use real `<ProjectReference>`s to the libraries they exercise.

**Each linked file must differ in the second assembly, or consumers get `CS0121`.** Compiling the same public type under the same full name into two packages makes every call ambiguous for anyone referencing both — this was a real, reproducible break, not a theoretical one. `DecimalExtension.cs` therefore carries a `#if X3CODE_REPOSITORY_LINK` (the symbol is set via `DefineConstants` in `X3Code.Repository.csproj`) that puts it in the `X3Code.Repository.Extensions` namespace when compiled into that package. `internal` would not work there: consumers call `ConfigureDecimalColum` in their own `OnModelCreating`.

If you ever link another file, give it the same treatment: decide whether the second copy can be `internal` (preferred, invisible to consumers) or needs its own namespace, and add the `#if` plus the symbol. Better still, check first whether the second project needs the file at all — `X3Code.Infrastructure` used to link `DataTableExtension.cs` purely for `BulkWrite`, and once that built its table from the EF model instead, the link could go away entirely.

### Versioning

All six packages are versioned **in lockstep from a single `<Version>` in `X3Code.Utilities/Directory.Build.props`**. No csproj carries a `Version`, `PackageVersion`, `AssemblyVersion`, or `FileVersion` of its own — those all derive from that one property. To cut a release, bump it once.

The scheme is `<.NET-Major>.<Minor>.<Patch>` (currently `10.0.0`), the same convention EF Core and the third-party EF providers use:

- **Major** — the .NET/EF generation, identical to the `TargetFramework`. Goes to `11` when the libraries move to `net11.0`.
- **Minor** — own features **and** breaking changes within that generation. The major slot is spent on the framework, so breaking changes are signalled here and spelled out in `PackageReleaseNotes`.
- **Patch** — bug fixes only, public API unchanged.

`PackageReleaseNotes` stays per-csproj — it is that package's changelog. Update it for whichever packages actually changed when you bump the shared version.

### Shared package metadata and dependencies

`X3Code.Utilities/Directory.Build.props` holds everything the packages have in common: version, authors, copyright, `PackageLicenseExpression`, project/repository URLs, icon, and the packaging switches (`GenerateDocumentationFile`, `IncludeSymbols`/`snupkg`, SourceLink, `ContinuousIntegrationBuild` under GitHub Actions). It also derives `IsPackable` from the project name — anything ending in `Test`/`Tests` is not packed. That derivation lives in the props file on purpose: `Directory.Build.props` is evaluated *before* the csproj body, so an `IsPackable` set inside a csproj is not yet visible to conditions there.

NuGet versions are centrally managed in `X3Code.Utilities/Directory.Packages.props` — csproj files carry `<PackageReference Include="..." />` **without** a `Version`. `CentralPackageTransitivePinningEnabled` is deliberately **off**: it attaches every centrally pinned version to the nuspec as a real dependency (`X3Code.Utilities` would end up declaring `Azure.Core`, which only arrives transitively via `Microsoft.Data.SqlClient`). Where a transitive package genuinely has to be pinned, use an explicit `PackageReference` in the affected project — as `X3Code.Infrastructure.Mobile` does for `SQLitePCLRaw.bundle_e_sqlite3`.

`X3Code.Utilities/NuGet.config` pins the package sources and maps them (`*` → nuget.org, `X3Code.*` → github); central package management warns (`NU1507`) without it.

`CS1591` ("missing XML comment") is suppressed centrally — roughly 340 public members are undocumented and the noise would bury everything else, including NuGet security warnings. Malformed doc tags (`CS1570`/`CS1584`/`CS1711`) are deliberately left visible.

### Logging

The libraries log through `Microsoft.Extensions.Logging.Abstractions` and nothing else. That package carries no sink and no configuration, so **where** anything is written stays entirely with the consuming application — Serilog (`services.AddSerilog()`), NLog (`builder.Logging.AddNLog()`), OpenTelemetry, or nothing at all. Never take a reference on a concrete logging framework here; that would force it on every consumer.

Every logging component follows the same shape:

- The constructor takes an **optional** `ILogger` / `ILogger<T>` that defaults to `NullLogger.Instance`. Nothing breaks and no container is needed when it is left out.
- The events live in an `internal static partial class …Log` next to the component and are written with `[LoggerMessage]`, so the source generator checks `IsEnabled` before building anything — a call costs nothing while the level is off.
- Event IDs are grouped per area: 1000 unit of work, 2000 HTTP client, 3000 configuration loader, 4000 blob storage.

Message templates use named placeholders (`{Context}`, `{StatusCode}`), which arrive as structured properties in Serilog rather than a flattened string.

What is covered: the unit-of-work transaction boundaries (opened, joined, committed, discarded, abandoned, retried), the HTTP error path with status and response body, the path a configuration file was resolved to, and the blob storage operations.

### Release/publish flow

Built packages (plus `.snupkg` symbol packages) are collected in the repo-root `NuGet/` folder (see `PackageOutputPath` above). `NuGet/deploy_Now.sh` is the manual publish script: it globs the `.nupkg` files actually present, shows them for confirmation, reads the API key without echoing it, and pushes each one to the NuGet source named `github` with `--skip-duplicate --no-symbols` (GitHub Packages does not accept `.snupkg`). It reports a per-package pass/fail summary and exits non-zero if anything failed.

`X3Code.Infrastructure.Mobile` is held back via the `EXCLUDED_PACKAGE_IDS` list at the top of the script — that package is maintained outside this repo. Everything else in the folder is pushed.

### CI

Two workflows, both on `ubuntu-latest` with `actions/setup-dotnet` for `10.0.x` (autobuild cannot build `net10.0` without it):

- `.github/workflows/build-and-test.yml` — restore, Release build, then all three test projects (`X3Code.Infrastructure.Tests` included: the ubuntu runners ship Docker, so Testcontainers works, and the `Slow` suite is filtered out). Runs on push to `master` and `feature/**`, on PRs to `master`, and manually. Uploads the `.trx` results and the built `.nupkg`/`.snupkg` as artifacts.
- `.github/workflows/codeql-analysis.yml` — CodeQL for C# on push/PR to `master` and weekly.
