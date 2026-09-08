# X3Code.Sql.Utils

Schlanker ADO.NET-Zugriff auf SQL Server, ohne ORM.

```shell
dotnet add package X3Code.Sql.Utils
```

## Inhalt

`DatabaseConnector` — Abfragen in eine `DataTable` lesen und Kommandos ausführen, synchron
und asynchron, wahlweise innerhalb einer bestehenden `SqlTransaction`.

```csharp
var table = DatabaseConnector.SelectAsDataTable(
    connectionString,
    "SELECT * FROM Person WHERE Name = @name",
    new DataTable(),
    new SqlParameter("@name", name));
```

> Werte gehören als `SqlParameter` übergeben. Eine aus Benutzereingaben zusammengesetzte
> Abfrage ist eine SQL-Injection.

## Warum ein eigenes Paket

Dieser Typ war früher Teil von `X3Code.Utilities` und hat als einziger
`Microsoft.Data.SqlClient` gebraucht — damit erbte jedes Projekt, das nur die
Allzweck-Helfer wollte, den kompletten SqlClient-Graph. `X3Code.Utilities` ist seit der
Trennung abhängigkeitsfrei.

## Ziel-Framework

`net10.0`

## Lizenz

MIT
