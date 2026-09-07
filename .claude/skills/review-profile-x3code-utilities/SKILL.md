---
name: review-profile-x3code-utilities
description: Review-Profil für X3Code.Utilities — die sechs NuGet-Bibliotheken, ihre bewusste Trennung ohne ProjectReference, geteilte Quelldateien per Compile-Link, Versionierungs- und Packaging-Regeln sowie Build-/Test-Kommandos. Wird vom code-reviewer-Agenten geladen, bevor er Änderungen an diesem Repository reviewt.
---

# Review-Profil: X3Code.Utilities

Dieses Repo ist **keine Anwendung**, sondern die Quelle für sechs unabhängige .NET-10-Klassenbibliotheken unter `X3Code.Utilities/`, die jeweils als eigenes NuGet-Paket veröffentlicht werden. Das Lieferobjekt ist das Paket — entsprechend wiegt die öffentliche API schwerer als in einer App.

Alle Pfade sind relativ zu `X3Code.Utilities/` (dort liegt die `.sln`; die Repo-Wurzel liegt eine Ebene darüber).

## Build und Test

```bash
dotnet build X3Code.Utilities.sln
dotnet test X3Code.Utilities.sln
```

Alle Testprojekte nutzen **xunit**:

| Testprojekt | Deckt ab | Voraussetzung |
|---|---|---|
| `X3Code.UnitTests` | `X3Code.Utils` (Extensions, HTTP, Konfiguration) | keine |
| `X3Code.Repository.Test` | `X3Code.Repository` | keine (EF Core InMemory) |
| `X3Code.Infrastructure.Tests` | `X3Code.Infrastructure` | **erreichbare SQL-Server-Instanz** |

`X3Code.Infrastructure.Tests` hat hartkodierte Connection Strings in `X3Code.Infrastructure.Tests/ConstValues/DbConnectionStrings.cs`, die auf bestimmte Dev-Maschinen zeigen. Schlägt dieses Projekt mangels SQL Server fehl, ist das **kein Review-Befund** — erwähne es höchstens als Hinweis, warum du die Tests nicht verifizieren konntest.

CI (`.github/workflows/codeql-analysis.yml`) fährt nur CodeQL, **keine** Tests. Verlass dich also nicht darauf, dass CI einen Testbruch fängt.

## Die sechs Bibliotheken

| Projekt | PackageId | Inhalt |
|---|---|---|
| `X3Code.Utils` | `X3Code.Utilities` | Allgemeine Helfer: Config-/Env-Loading, Converter, `DatabaseConnector`, Extensions, HTTP-Client, XML-Serialisierung, GUID- und MIME-Helfer |
| `X3Code.Infrastructure` | `X3Code.Infrastructure` | Generisches Repository / Unit of Work (`Entity`, `IEntity`, `IRepository`, `GenericRepository`) für EF Core + SQL Server |
| `X3Code.Repository` | `X3Code.Repository` | Neuere Fassung desselben Patterns (`Entity`, `IEntity`, `IBaseRepository`, `BaseRepository`) |
| `X3Code.Infrastructure.Mobile` | — | Wie `X3Code.Infrastructure`, aber gegen EF Core Sqlite |
| `X3Code.Wasm.Utils` | — | Blazor-WASM-Helfer, derzeit nur `LocalStorageService` (JSInterop) |
| `X3Code.Azure.Utils` | — | Azure-Helfer, derzeit `AzureBlobStorage` / `IAzureStorage` / `StorageBase` |

**Wichtig — kein falscher Wiederverwendungs-Befund**: `X3Code.Infrastructure`, `X3Code.Infrastructure.Mobile` und `X3Code.Repository` sind **absichtlich** nahezu duplizierte Implementierungen desselben Patterns (SQL Server vs. Sqlite vs. neueres Pattern). Melde diese Duplizierung **nicht** als Befund und schlage keine Zusammenführung vor.

## Projekttrennung und geteilte Quelldateien

Die Bibliotheken benutzen untereinander **bewusst keine `<ProjectReference>`** — das würde den Abhängigkeitsgraph eines Pakets in ein anderes ziehen. Ein neuer `<ProjectReference>` zwischen zwei packbaren Bibliotheken ist ein **Blocker**. (Testprojekte referenzieren die Bibliotheken dagegen ganz normal per `ProjectReference` — das ist korrekt.)

Stattdessen werden einzelne Dateien per Compile-Link in ein zweites Projekt eingebunden:

- `X3Code.Infrastructure` linkt `X3Code.Utils/Extensions/DataTableExtension.cs`
- `X3Code.Repository` linkt `X3Code.Infrastructure/Extensions/DecimalExtension.cs`

**Blocker**: Wurde eine dieser Dateien geändert, ohne beide konsumierenden Projekte zu prüfen? Das ist geteilte Quelle, keine private Kopie — eine Änderung schlägt in zwei Pakete durch. Prüfe in dem Fall, ob der Code in beiden Kontexten noch kompiliert und semantisch stimmt.

Braucht neuer Code eine Datei aus einer anderen Bibliothek, ist der etablierte Weg ein weiterer `<Compile Include>`-Link — nicht ein `ProjectReference` und nicht eine wortgleiche Kopie.

## Öffentliche API

Jede Änderung an einem `public`-Member ist eine potenzielle Breaking Change für Paketkonsumenten. Als **Blocker** melden, solange nicht ein bewusster Major-Bump dokumentiert ist:

- Entfernte oder umbenannte `public` Typen/Member
- Geänderte Signaturen (Parametertyp, -reihenfolge, Rückgabetyp)
- Ein neuer Pflichtparameter ohne Default
- Verengte Sichtbarkeit (`public` → `internal`)

Additive Änderungen (neue Überladung, neuer optionaler Parameter, neuer Typ) sind unkritisch.

Bibliothekscode wird von außen aufgerufen: `public`-Methoden validieren ihre Eingaben und werfen aussagekräftige Exceptions (`ArgumentNullException` mit Parametername), statt später mit einer `NullReferenceException` zu scheitern.

## Versionierung und Packaging

Jedes packbare `.csproj` trägt eigene `Version`, `PackageVersion`, `AssemblyVersion`, `FileVersion` — die werden **manuell** gepflegt und sind untereinander häufig inkonsistent (z. B. `Version=1.3.4` neben `PackageVersion=1.10.4`). Maßgeblich für das veröffentlichte Paket ist `PackageVersion`/`AssemblyVersion`/`FileVersion`, **nicht** `Version`.

Beim Review:

- Ändert sich die öffentliche API einer Bibliothek, ohne dass `PackageVersion` gebumpt wurde → Befund („sollte behoben werden").
- Wird eine Version gebumpt, ohne dass `PackageReleaseNotes` im selben csproj aktualisiert wurde → Befund. Das ist der Changelog des Pakets.
- Melde die allgemeine Inkonsistenz zwischen `Version` und `PackageVersion` **nicht** als Befund, solange die Änderung sie nicht verschlimmert — das ist Bestand.

Alle Projekte zielen auf `net10.0` mit `GeneratePackageOnBuild=true`. `X3Code.Utilities/Directory.Build.props` setzt `PackageOutputPath` auf den Repo-Wurzelordner `NuGet/`. Änderungen an `Directory.Build.props` oder an `PackageOutputPath` betreffen alle Pakete und den Publish-Flow — genau hinsehen.

Publish läuft manuell über `NuGet/deploy_Now.sh` (die gültige Fassung; die Kopie in der Repo-Wurzel ist veraltet und pusht `X3Code.Repository` nicht mit). Das Skript pusht `X3Code.Azure.Utils`, `X3Code.Utilities`, `X3Code.Infrastructure`, `X3Code.Wasm.Utils` und `X3Code.Repository` — **nicht** `X3Code.Infrastructure.Mobile`.

## Wiederverwendungs-Register

**Extensions** in `X3Code.Utils/Extensions/`: `CollectionExtension`, `DataRowExtension`, `DataTableExtension`, `DateTimeExtension`, `DictionaryExtension`, `EntityPropertyExtension`.
Weitere: `X3Code.Utils/Configurations/SoftwareEnvironmentExtension.cs`, `X3Code.Utils/Web/MimeTypeFileExtension.cs`, `X3Code.Infrastructure/Extensions/DecimalExtension.cs`.

Prüfe vor jedem neu geschriebenen Helfer, ob eine dieser Extensions ihn bereits abdeckt — und ob eine neue Methode in die vorhandene Extension-Klasse gehört statt in eine neue daneben.

**Konstanten**: Dieses Repo hat keine zentrale ConstValues-Konvention. Const-Klassen existieren nur in Testprojekten (`X3Code.UnitTests/HTTP/HttpConstValues.cs`, `X3Code.Infrastructure.Tests/ConstValues/`). Magic Numbers/Strings in Bibliothekscode trotzdem als Befund melden — mit dem Vorschlag, sie als `const`/`static readonly` nahe am Verwendungsort zu benennen, nicht mit dem Vorschlag einer neuen zentralen Const-Klasse.

## Tests

Neue oder geänderte Logik gehört in das zur Bibliothek passende Testprojekt (siehe Tabelle oben). Für `X3Code.Utils` und `X3Code.Repository` ist fehlender Test bei neuer Logik ein Befund — beide laufen ohne externe Abhängigkeit. Für `X3Code.Infrastructure` ist er ein abgeschwächter Befund, weil das Testprojekt eine SQL-Server-Instanz braucht. `X3Code.Infrastructure.Mobile`, `X3Code.Wasm.Utils` und `X3Code.Azure.Utils` haben derzeit **kein** Testprojekt — fordere dort keinen Test in einem bestehenden Projekt ein, sondern weise auf die Lücke hin, wenn die Änderung substanziell ist.
