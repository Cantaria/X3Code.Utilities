#!/bin/bash

echo "Deplyoing X3-Code NuGet packages"
echo "Which version are we deploying? Please user format 1.2.3:"
read version

echo "Please enter the API-Key:"
read apikey

x3AzureUtils=X3Code.Azure.Utils.$version.nupkg
x3Utils=X3Code.Utilities.$version.nupkg
x3Infrastructure=X3Code.Infrastructure.$version.nupkg
x3Wasm=X3Code.Wasm.Utils.$version.nupkg
PACKAGE_SOURCE=github

echo "Trying to push: $x3AzureUtils"
if test -f "$x3AzureUtils"; then
    dotnet nuget push $x3AzureUtils --source $PACKAGE_SOURCE -k $apikey
else
    echo "File $x3AzureUtils does not exist. Do nothing"
fi

echo "Trying to push: $x3Utils"
if test -f "$x3Utils"; then
    dotnet nuget push $x3Utils --source $PACKAGE_SOURCE -k $apikey
else
    echo "File $x3Utils does not exist. Do nothing"
fi

echo "Trying to push: $x3Infrastructure"
if test -f "$x3Infrastructure"; then
    dotnet nuget push $x3Infrastructure --source $PACKAGE_SOURCE -k $apikey
else
    echo "File $x3Infrastructure does not exist. Do nothing"
fi

echo "Trying to push: $x3Wasm"
if test -f "$x3Wasm"; then
    dotnet nuget push $x3Wasm --source $PACKAGE_SOURCE -k $apikey
else
    echo "File $x3Wasm does not exist. Do nothing"
fi

