---
name: review-profile-x3code-utilities
description: Review-Profil für X3Code.Utilities — die sieben NuGet-Bibliotheken, ihre bewusste Trennung ohne ProjectReference, geteilte Quelldateien per Compile-Link, Versionierungs- und Packaging-Regeln, Logging-Konvention sowie Build-/Test-Kommandos. Wird vom code-reviewer-Agenten geladen, bevor er Änderungen an diesem Repository reviewt.
---

# Review-Profil: X3Code.Utilities

Dieses Repo ist **keine Anwendung**, sondern die Quelle für sieben unabhängige .NET-10-Klassenbibliotheken unter `X3Code.Utilities/`, die jeweils als eigenes NuGet-Paket veröffentlicht werden. Das Lieferobjekt ist das Paket — entsprechend wiegt die öffentliche API schwerer als in einer App.

Alle Pfade sind relativ zu `X3Code.Utilities/` (dort liegt die `.sln`; die Repo-Wurzel liegt eine Ebene darüber).

> Bei Widersprüchen zwischen diesem Profil und `CLAUDE.md` im Repo-Wurzelverzeichnis gilt `CLAUDE.md` — sie wird bei jeder Änderung mitgepflegt.

## Build und Test

```bash
dotnet build X3Code.Utilities.sln
dotnet test X3Code.Utilities.sln
```

| Testprojekt | Deckt ab | Voraussetzung |
|---|---|---|
| `X3Code.UnitTests` | `X3Code.Utils`, `X3Code.Azure.Utils` | keine |
| `X3Code.Repository.Test` | `X3Code.Repository` | keine (SQLite in-memory) |
| `X3Code.Infrastructure.Tests` | `X3Code.Infrastructure`, `X3Code.Sql.Utils` | **laufender Docker-Daemon** |

`X3Code.Infrastructure.Tests` startet über `Helper/SqlServerFixture.cs` einen Wegwerf-SQL-Server im Container (`Testcontainers.MsSql`, Image auf `mcr.microsoft.com/mssql/server:2022-latest` gepinnt), spielt die Migrationen ein und räumt ihn wieder ab. **Es gibt keine Connection Strings mehr zu pflegen** — `ConstValues/DbConnectionStrings.cs` dient nur noch dem EF-Design-Time-Tooling und liest die Umgebungsvariable `X3_TEST_DB`. Hartkodierte Zugangsdaten sind hier ein **Blocker**, kein Bestand.

Zwei Test-Kategorien laufen nicht in der CI und müssen bewusst gestartet werden:

- `[Trait("Category", "Slow")]` — `BulkWriterTest`, geht bis zehn Millionen Zeilen
- `[Trait("Category", "Integration")]` — `HttpClientTest`, spricht eine öffentliche Fremd-API an

Schlägt eine dieser Suiten aus Umgebungsgründen fehl, ist das **kein Review-Befund**.

CI fährt zwei Workflows: `.github/workflows/build-and-test.yml` (Restore, Release-Build, alle drei Testprojekte) und `codeql-analysis.yml`. Ein Testbruch wird also von der CI gefangen.

## Die sieben Bibliotheken

| Projekt | PackageId | Inhalt |
|---|---|---|
| `X3Code.Utils` | `X3Code.Utilities` | Allgemeine Helfer: Config-/Env-Loading, Converter, Extensions, HTTP-Client, XML-Serialisierung, GUID- und MIME-Helfer |
| `X3Code.Sql.Utils` | `X3Code.Sql.Utils` | `DatabaseConnector` — schlanker ADO.NET-Zugriff auf SQL Server, ohne ORM |
| `X3Code.Infrastructure` | `X3Code.Infrastructure` | Generisches Repository (`Entity`, `IEntity`, `IRepository`, `GenericRepository`) für EF Core + SQL Server |
| `X3Code.Repository` | `X3Code.Repository` | Neuere Fassung desselben Patterns plus `IUnitOfWork` und DI-Registrierung |
| `X3Code.Infrastructure.Mobile` | `X3Code.Infrastructure.Mobile` | Wie `X3Code.Infrastructure`, aber gegen EF Core Sqlite |
| `X3Code.Wasm.Utils` | `X3Code.Wasm.Utils` | Blazor-WASM-Helfer, derzeit nur `LocalStorageService` (JSInterop) |
| `X3Code.Azure.Utils` | `X3Code.Azure.Utils` | Azure-Helfer, derzeit `AzureBlobStorage` / `IAzureStorage` / `StorageBase` |

**Wichtig — kein falscher Wiederverwendungs-Befund**: `X3Code.Infrastructure`, `X3Code.Infrastructure.Mobile` und `X3Code.Repository` sind **absichtlich** nahezu duplizierte Implementierungen desselben Patterns. Melde diese Duplizierung **nicht** und schlage keine Zusammenführung vor.

`X3Code.Infrastructure.Mobile` wird von jemandem außerhalb dieses Repos gepflegt und bewusst **nicht** veröffentlicht (steht in `EXCLUDED_PACKAGE_IDS` in `NuGet/deploy_Now.sh`). Das ist Absicht und kein Befund. API-Änderungen an `X3Code.Infrastructure` sollen dort aber mitgezogen werden.

## Abhängigkeiten

Der Graph ist bewusst schmal gehalten. Eine **neue** Paketreferenz ist begründungspflichtig:

| Paket | Abhängigkeiten |
|---|---|
| `X3Code.Utilities` | nur `Microsoft.Extensions.Logging.Abstractions` |
| `X3Code.Sql.Utils` | `Microsoft.Data.SqlClient` |
| `X3Code.Azure.Utils` | `Azure.Storage.Blobs`, `Logging.Abstractions` |
| `X3Code.Repository` | EF Core (+ Relational), `DependencyInjection.Abstractions`, `Logging.Abstractions` |

**Befund**, wenn eine Bibliothek eine Referenz trägt, die im Quellcode nicht vorkommt — genau dafür wurden `Microsoft.Data.SqlClient` und `Azure.Core` aus `X3Code.Azure.Utils` und der `DatabaseConnector` aus `X3Code.Utilities` entfernt.

Versionen stehen **zentral** in `Directory.Packages.props`; ein `Version`-Attribut an einem `PackageReference` in einer csproj ist ein Befund. `CentralPackageTransitivePinningEnabled` steht bewusst auf `false` — ein transitives Paket wird über einen expliziten `PackageReference` im betroffenen Projekt gepinnt.

## Projekttrennung und geteilte Quelldateien

Die Bibliotheken benutzen untereinander **bewusst keine `<ProjectReference>`**. Ein neuer `ProjectReference` zwischen zwei packbaren Bibliotheken ist ein **Blocker**. (Testprojekte referenzieren die Bibliotheken normal — das ist korrekt.)

Aktuell gelinkt wird genau eine Bibliotheksdatei:

- `X3Code.Repository` linkt `X3Code.Infrastructure/Extensions/DecimalExtension.cs`

**Blocker**: Wurde diese Datei geändert, ohne beide konsumierenden Projekte zu prüfen? Das ist geteilte Quelle, keine private Kopie.

**Jede gelinkte Datei muss sich in der zweiten Assembly unterscheiden, sonst bekommen Konsumenten `CS0121`.** Derselbe öffentliche Typ unter demselben vollen Namen in zwei Paketen macht jeden Aufruf mehrdeutig, sobald jemand beide referenziert — das ist hier real passiert. `DecimalExtension.cs` trägt deshalb ein `#if X3CODE_REPOSITORY_LINK` (Symbol via `DefineConstants` in `X3Code.Repository.csproj`), das sie dort in den Namespace `X3Code.Repository.Extensions` legt. Wird eine weitere Datei gelinkt, ohne dass sie entweder `internal` kompiliert wird oder einen eigenen Namespace bekommt: **Blocker**.

Bevor du einen neuen Link vorschlägst, prüfe, ob das zweite Projekt die Datei überhaupt braucht — `X3Code.Infrastructure` linkte `DataTableExtension.cs` allein für `BulkWrite`, und als das seine DataTable aus dem EF-Modell baute, konnte der Link ersatzlos weg.

`TestSupport/InterfaceContract.cs` wird in alle drei Testprojekte gelinkt. Dort gilt die CS0121-Regel nicht: Testassemblies sind `IsPackable=false` und werden von niemandem referenziert.

## Öffentliche API

Jede Änderung an einem `public`-Member ist eine potenzielle Breaking Change. Als **Blocker** melden, solange sie nicht in `PackageReleaseNotes` mit `BREAKING:` ausgewiesen ist:

- Entfernte oder umbenannte `public` Typen/Member
- Geänderte Signaturen (Parametertyp, -reihenfolge, Rückgabetyp)
- Ein neuer Pflichtparameter ohne Default
- Verengte Sichtbarkeit (`public` → `internal`)

**Schnittstelle und Implementierung müssen zusammenpassen — auch dort, wo der Compiler es nicht prüft.** Parameternamen, Defaultwerte, Nullability und generische Einschränkungen sind nicht Teil der Signatur, auf die er matcht. In diesem Repo sind dadurch drei Fehler entstanden (`IConfigurationLoader.Load` mit vertauschten Parameternamen, `IRepository.BulkWrite` mit Pflicht- statt optionalen Parametern, `IAzureStorage.ListFiles` ohne den Default der Implementierung). Es gibt dafür jetzt Vertragstests: `Contract_specs` in allen drei Testprojekten, auf Basis von `TestSupport/InterfaceContract.cs`. **Kommt eine neue öffentliche Schnittstelle dazu, gehört sie in die passende `Contracts`-Liste** — fehlt sie dort, ist das ein Befund.

Bibliothekscode wird von außen aufgerufen: `public`-Methoden validieren ihre Eingaben und werfen aussagekräftige Exceptions (`ArgumentNullException`/`ArgumentException` mit Parametername) statt später mit einer `NullReferenceException` zu scheitern. Ein `throw new Exception(...)` in Bibliothekscode ist ein Befund — der Bestand wurde vollständig darauf umgestellt.

Alle asynchronen Methoden der öffentlichen Schnittstellen nehmen einen `CancellationToken` (optional, am Ende) und reichen ihn durch. Eine neue async-Methode ohne Token ist ein Befund.

## Logging

Die Bibliotheken loggen über `Microsoft.Extensions.Logging.Abstractions` und **nichts anderes**. Eine Referenz auf ein konkretes Logging-Framework (Serilog, NLog) in einer Bibliothek ist ein **Blocker** — sie zwänge es jedem Konsumenten auf.

Das Muster ist überall gleich und bei neuem Logging einzuhalten:

- Optionaler `ILogger`/`ILogger<T>`-Konstruktorparameter mit `NullLogger`-Default
- Ereignisse in einer `internal static partial class …Log` neben der Komponente, geschrieben mit `[LoggerMessage]`
- Event-ID-Bereiche: 1000 Unit of Work, 2000 HTTP-Client, 3000 ConfigurationLoader, 4000 Blob Storage
- Benannte Platzhalter (`{Context}`), keine String-Interpolation

## Versionierung und Packaging

Alle Pakete werden **im Gleichschritt aus einer einzigen `<Version>` in `X3Code.Utilities/Directory.Build.props`** versioniert. Kein csproj trägt eine eigene `Version`, `PackageVersion`, `AssemblyVersion` oder `FileVersion` — taucht eine davon in einer csproj auf, ist das ein Befund.

Schema: `<.NET-Major>.<Minor>.<Patch>`, aktuell `10.0.0`. Der Major folgt der .NET-Generation, **Breaking Changes stecken im Minor** und müssen deshalb in `PackageReleaseNotes` mit `BREAKING:` ausgewiesen werden.

Beim Review:

- Ändert sich die öffentliche API einer Bibliothek, ohne dass `PackageReleaseNotes` des Pakets ergänzt wurde → **Befund**. Das ist der Changelog des Pakets.
- `PackageReleaseNotes` sind XML-Inhalt: Generics wie `List<T>` darin brechen die csproj. Umformulieren statt escapen.

Gemeinsame Metadaten (Autor, Lizenz, Icon, `GenerateDocumentationFile`, snupkg, SourceLink) stehen zentral in `Directory.Build.props`. Dort wird `IsPackable` aus dem Projektnamen abgeleitet — alles auf `Test`/`Tests` wird nicht gepackt.

`CS1591` („XML-Kommentar fehlt") ist zentral unterdrückt; kaputte Doku-Tags (`CS1570`/`CS1584`/`CS1711`/`CS1573`) bleiben bewusst sichtbar und sind ein Befund.

Publish läuft manuell über `NuGet/deploy_Now.sh`: globt die vorhandenen `.nupkg`, zeigt sie zur Bestätigung, liest den API-Key verdeckt und pusht mit `--skip-duplicate --no-symbols`.

## Wiederverwendungs-Register

**Extensions** in `X3Code.Utils/Extensions/`: `CollectionExtension`, `DataRowExtension`, `DataTableExtension`, `DateTimeExtension`, `DictionaryExtension`, `EntityPropertyExtension`.
Weitere: `X3Code.Utils/Configurations/SoftwareEnvironmentExtension.cs`, `X3Code.Utils/Web/MimeTypeFileExtension.cs`, `X3Code.Infrastructure/Extensions/DecimalExtension.cs`.

Prüfe vor jedem neu geschriebenen Helfer, ob eine dieser Extensions ihn bereits abdeckt — und ob eine neue Methode in die vorhandene Klasse gehört statt in eine neue daneben.

**Konstanten**: Dieses Repo hat keine zentrale ConstValues-Konvention. Magic Numbers/Strings in Bibliothekscode trotzdem als Befund melden — mit dem Vorschlag, sie als `const` nahe am Verwendungsort zu benennen, nicht mit dem Vorschlag einer neuen zentralen Const-Klasse.

## Tests

Neue oder geänderte Logik gehört in das zur Bibliothek passende Testprojekt. Fehlender Test bei neuer Logik ist ein Befund für `X3Code.Utils`, `X3Code.Sql.Utils`, `X3Code.Repository`, `X3Code.Infrastructure` und `X3Code.Azure.Utils`.

`X3Code.Infrastructure.Mobile` und `X3Code.Wasm.Utils` haben **kein** Testprojekt — fordere dort keinen Test in einem bestehenden Projekt ein, sondern weise auf die Lücke hin, wenn die Änderung substanziell ist.

**Prüfe bei jedem neuen Test, ob er auch gegen die alte Implementierung grün wäre.** In diesem Repo sind mehrfach Tests entstanden, die den gemeldeten Fehler gar nicht finden konnten — ein `Ignore()` im Modell, das genau den interessanten Fall wegnahm, und `ExecuteAsync`-Tests auf einem Provider, der nie wiederholt. Ein Test, der den Fehler nicht reproduziert, ist ein Befund.
