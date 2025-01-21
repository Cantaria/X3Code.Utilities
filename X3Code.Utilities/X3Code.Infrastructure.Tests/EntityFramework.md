## Install dotnet EF

```bash
dotnet tool install --global dotnet-ef --version <Versionsnr des NuGet Paketes>
```
## Update dotnet EF

```bash
dotnet tool update --global dotnet-ef
```

----
## how to migrate:

PowerShell:
```powershell
$env:X3_Environment = "Local"
```

```bash
X3_Environment=Local
```
```bash
export X3_Environment
```
change "X3_Environment" to needed environment (Local, Dev, Int, Prod)
user commands below...

---
```bash
cd X3Code.Infrastructure.Tests
```
```bash
dotnet ef migrations add S002
```
```bash
dotnet ef migrations remove <Name>
```
```bash
dotnet ef database update
```