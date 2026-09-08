# X3Code.Infrastructure

Generisches Repository- und Unit-of-Work-Pattern für Entity Framework Core gegen SQL Server.

```shell
dotnet add package X3Code.Infrastructure
```

## Inhalt

`Entity`, `IEntity`, `IRepository<TEntity>`, `GenericRepository<TEntity>` sowie `DecimalExtension`
zum Konfigurieren von Dezimalspalten.

## Verhältnis zu X3Code.Repository

Dieses Paket ist die ältere, SQL-Server-gebundene Ausprägung des Patterns und wird primär
gepflegt. Für neue Projekte ist `X3Code.Repository` der aktuellere, providerunabhängige Ansatz.
Beide Pakete existieren bewusst nebeneinander.

## Ziel-Framework

`net10.0`

## Lizenz

MIT
