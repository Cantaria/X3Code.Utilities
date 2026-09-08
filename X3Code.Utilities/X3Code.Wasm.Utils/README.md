# X3Code.Wasm.Utils

Helfer für Blazor WebAssembly.

```shell
dotnet add package X3Code.Wasm.Utils
```

## Inhalt

`ILocalStorageService` und `LocalStorageService` — typisierter Zugriff auf den Local Storage
des Browsers über JSInterop.

```csharp
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
```

## Ziel-Framework

`net10.0`

## Lizenz

MIT
