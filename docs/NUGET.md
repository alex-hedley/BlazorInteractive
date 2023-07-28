# NuGet

- [NuGetThosePackages.csx](../src/scripts/NuGetThosePackages.csx)

Running `dotnet add package`

```dotnetcli
info :   GET https://api.nuget.org/v3/registration5-gz-semver2/ironpython/index.json
info :   OK https://api.nuget.org/v3/registration5-gz-semver2/ironpython/index.json 165ms
info : Restoring packages for /Users/robert.anderson/Documents/code/playground/nuget_testing/nuget_testing.csproj...
info :   GET https://api.nuget.org/v3-flatcontainer/ironpython/index.json
info :   OK https://api.nuget.org/v3-flatcontainer/ironpython/index.json 177ms
info :   GET https://api.nuget.org/v3-flatcontainer/ironpython/3.4.1/ironpython.3.4.1.nupkg
info :   OK https://api.nuget.org/v3-flatcontainer/ironpython/3.4.1/ironpython.3.4.1.nupkg 445ms
info :   GET https://api.nuget.org/v3-flatcontainer/dynamiclanguageruntime/index.json
info :   GET https://api.nuget.org/v3-flatcontainer/mono.unix/index.json
info :   OK https://api.nuget.org/v3-flatcontainer/dynamiclanguageruntime/index.json 158ms
info :   GET https://api.nuget.org/v3-flatcontainer/dynamiclanguageruntime/1.3.4/dynamiclanguageruntime.1.3.4.nupkg
info :   OK https://api.nuget.org/v3-flatcontainer/mono.unix/index.json 166ms
info :   GET https://api.nuget.org/v3-flatcontainer/mono.unix/7.1.0-final.1.21458.1/mono.unix.7.1.0-final.1.21458.1.nupkg
info :   OK https://api.nuget.org/v3-flatcontainer/dynamiclanguageruntime/1.3.4/dynamiclanguageruntime.1.3.4.nupkg 86ms
info :   OK https://api.nuget.org/v3-flatcontainer/mono.unix/7.1.0-final.1.21458.1/mono.unix.7.1.0-final.1.21458.1.nupkg 92ms
```