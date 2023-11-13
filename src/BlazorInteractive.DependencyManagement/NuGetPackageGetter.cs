using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf.Types;

namespace BlazorInteractive.DependencyManagement;

public class NuGetPackageGetter : IPackageGetter
{
    private readonly ILogger _logger;

    record IndexResource([property:JsonPropertyName("@Id")] string Id, [property:JsonPropertyName("@Type")] string Type, string Comment);
    record IndexResponse(string Version, List<IndexResource> Resources);
    record PackageVersions(List<string> Versions);
    
    public NuGetPackageGetter(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<PackageResult> GetPackage(string name)
    {
        Directory.CreateDirectory("nuget");

        var nugetIndexResponse = await GetAndCache<IndexResponse>("https://api.nuget.org/v3/index.json", "nuget/index.json");
        var packageVersionsBaseUrl = nugetIndexResponse.Resources.First(resource => resource.Type == "PackageBaseAddress/3.0.0").Id;

        var testUrl = $"{packageVersionsBaseUrl}{name}/index.json";
        Console.WriteLine(testUrl);
        var versionsResponse = await GetAndCache<PackageVersions>($"{packageVersionsBaseUrl}{name}/index.json", $"nuget/{name}versions.json");
        var latestVersion = versionsResponse.Versions.Last();

        // https://github.com/dotnet/runtime/issues/78991
        // DOTNET_EnableAVX=0
        var packageUrl = $"{packageVersionsBaseUrl}{name}/{latestVersion}/{name}.{latestVersion}.nupkg";
        // var packageUrl = $"{packageVersionsBaseUrl.ToString()}{name.ToString()}/{latestVersion.ToString()}/{name.ToString()}.{latestVersion.ToString()}.nupkg";

        if (!File.Exists($"nuget/{name}.{latestVersion}.nupkg"))
        {
            using (var localFile = File.OpenWrite($"nuget/{name}.{latestVersion}.nupkg"))
            {
                var packageZip = await client.GetStreamAsync(packageUrl);
                await packageZip.CopyToAsync(localFile);
            }
        }

        var extractDir = $"nuget/{name}.{latestVersion}";
        if (!Directory.Exists(extractDir))
        {
            Directory.CreateDirectory(extractDir);
        }

        using (var zip = ZipFile.OpenRead($"nuget/{name}.{latestVersion}.nupkg"))
        {
            foreach (var file in zip.Entries)
            {
                var target = $"{extractDir}/{file.Name}";
                if (File.Exists(target))
                {
                    continue;
                }

                // Check folders

                if (file.Name.EndsWith(".dll") && file.FullName.Contains("net6"))
                {
                    file.ExtractToFile(target);
                    Console.WriteLine($"Extracted {file.FullName}");
                }
                else if (file.Name.EndsWith("nuspec"))
                {
                    file.ExtractToFile(target);
                    Console.WriteLine($"Extracted {file.FullName}");
                }
            }
        }

        return new PackageResult(new Success());
    }

    HttpClient client = new();
    
    async Task<T> GetAndCache<T>(string url, string file)
    {
        string json;
        if (!File.Exists(file))
        {
            json = await client.GetStringAsync(url);
            await File.WriteAllTextAsync(file, json);
        }
        else
        {
            json = await File.ReadAllTextAsync(file);
        }

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    }
}