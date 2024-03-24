#!/bin/bash

echo "Deplyoing X3-Code NuGet packages"
echo "Which version are we deploying? Please user format 1.2.3:"
read version

dotnet nuget push X3Code.Azure.Utils.$version.nupkg --source X3Get
dotnet nuget push X3Code.Utilities.$version.nupkg --source X3Get
dotnet nuget push X3Code.Infrastructure.$version.nupkg --source X3Get
dotnet nuget push X3Code.Wasm.Utils.$version.nupkg --source X3Get
