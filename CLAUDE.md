# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This repo is the source for the **X3-Code.Utilities** NuGet package family — six independent .NET 10 class libraries under `X3Code.Utilities/`, each built and published as its own NuGet package. There is no single "app"; the deliverables are the packages themselves.

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

All test projects use **xunit**. `X3Code.Repository.Test` uses EF Core's InMemory provider and needs nothing external. **`X3Code.Infrastructure.Tests` requires a real, reachable SQL Server instance** — connection strings are hardcoded in `X3Code.Infrastructure.Tests/ConstValues/DbConnectionStrings.cs` for specific dev machines/VMs; update that file to point at a SQL Server you control before running this project's tests.

## Architecture

### The six packable libraries

| Project | PackageId | Purpose |
|---|---|---|
| `X3Code.Utils` | `X3Code.Utilities` | General-purpose helpers: config/env loading, converters, a lightweight `DatabaseConnector`, extension methods (Collection/DataRow/DataTable/DateTime/Dictionary/EntityProperty), a basic HTTP client, XML serialization, GUID tools, MIME helpers |
| `X3Code.Infrastructure` | `X3Code.Infrastructure` | Generic repository / unit-of-work pattern (`Entity`, `IEntity`, `IRepository`, `GenericRepository`) for EF Core against SQL Server |
| `X3Code.Repository` | `X3Code.Repository` | Newer take on the same repository pattern (`Entity`, `IEntity`, `IBaseRepository`, `BaseRepository`) |
| `X3Code.Infrastructure.Mobile` | — | Same repository pattern as `X3Code.Infrastructure`, but built against EF Core Sqlite for mobile use |
| `X3Code.Wasm.Utils` | — | Blazor WASM helpers, currently just a `LocalStorageService` wrapping JSInterop |
| `X3Code.Azure.Utils` | — | Azure helpers, currently `AzureBlobStorage`/`IAzureStorage`/`StorageBase` |

Everything targets `net10.0` and has `GeneratePackageOnBuild=true`, so a normal `dotnet build` produces a `.nupkg` per packable project in that project's own `bin/<Configuration>/` folder. The two `Infrastructure*` repository-pattern projects and `X3Code.Repository` are intentionally separate, near-duplicate implementations (SQL Server vs Sqlite vs the newer pattern) — don't assume they should be merged.

### Cross-project code sharing without ProjectReference

Libraries deliberately do **not** use `<ProjectReference>` between each other (that would pull one package's dependency graph into another). Instead, a handful of files are shared by linking the same `.cs` file into a second project via `<Compile Include="..\OtherProject\Path.cs"><Link>...</Link></Compile>`:

- `X3Code.Infrastructure` links `X3Code.Utils/Extensions/DataTableExtension.cs`
- `X3Code.Repository` links `X3Code.Infrastructure/Extensions/DecimalExtension.cs`

If you touch one of these linked files, check both csproj files that reference it — you're editing shared source, not a private copy. Test projects, by contrast, *do* use real `<ProjectReference>`s to the libraries they exercise.

### Versioning

Each packable csproj carries its own `Version`, `PackageVersion`, `AssemblyVersion`, and `FileVersion`, bumped manually per release and **frequently inconsistent with each other** (e.g. `X3Code.Utils` has had `Version=1.3.4` alongside `PackageVersion=1.10.4`). `PackageVersion`/`AssemblyVersion`/`FileVersion` is the one that actually drives the published NuGet version — don't assume `Version` reflects the real package version. `PackageReleaseNotes` in each csproj is the per-package changelog; update it when bumping a package's version.

### Release/publish flow

`deploy_Now.sh` at the repo root is a manual publish script: it prompts for a version string and an API key, then runs `dotnet nuget push` for each package's `.nupkg` (by convention `<PackageId>.<version>.nupkg` in the current directory) against a NuGet source named `github`. It only knows about `X3Code.Azure.Utils`, `X3Code.Utilities`, `X3Code.Infrastructure`, and `X3Code.Wasm.Utils` — it does not currently push `X3Code.Repository` or `X3Code.Infrastructure.Mobile`.

### CI

`.github/workflows/codeql-analysis.yml` runs CodeQL static analysis (autobuild + analyze for C#) on push/PR to `master` and on a weekly schedule. It does not run the test suite — there is no CI job that runs `dotnet test`.
