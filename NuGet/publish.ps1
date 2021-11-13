nuget source Add -Name X3Lab -Source "https://gitlab.x3code.de/api/v4/projects/8/packages/nuget/index.json" -UserName Clemens -Password 

./NuGet.exe push X3Code.Utilities.1.3.1.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.3.2.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.3.3.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.5.0.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.5.1.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.5.2.nupkg -Source X3Lab
./NuGet.exe push X3Code.Utilities.1.5.3.nupkg -Source X3Lab

./NuGet.exe push X3Code.Infrastructure.1.3.1.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.3.2.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.3.3.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.3.4.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.5.1.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.5.2.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.1.5.4.nupkg -Source X3Lab

./NuGet.exe push X3Code.Infrastructure.RavenDb.1.0.0.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.RavenDb.1.0.2.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.RavenDb.1.0.3.nupkg -Source X3Lab
./NuGet.exe push X3Code.Infrastructure.RavenDb.1.0.4.nupkg -Source X3Lab