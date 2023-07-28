using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

record IndexResource([property:JsonPropertyName("@Id")] string Id, [property:JsonPropertyName("@Type")] string Type, string Comment);
record IndexResponse(string Version, List<IndexResource> Resources);

record PackageVersions(List<string> Versions);

var client = new HttpClient();

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

var nugetIndexResponse = await GetAndCache<IndexResponse>("https://api.nuget.org/v3/index.json", "nuget/index.json");
var packageVersionsBaseUrl = nugetIndexResponse.Resources.First(resource => resource.Type == "PackageBaseAddress/3.0.0").Id;

var testUrl = $"{packageVersionsBaseUrl}ironpython/index.json";
Console.WriteLine(testUrl);
var versionsResponse = await GetAndCache<PackageVersions>($"{packageVersionsBaseUrl}ironpython/index.json", "nuget/ironpythonversions.json");
var latestVersion = versionsResponse.Versions.Last();

var packageUrl = $"{packageVersionsBaseUrl}ironpython/{latestVersion}/ironpython.{latestVersion}.nupkg";

if (!File.Exists($"nuget/ironpython.{latestVersion}.nupkg"))
{
    using (var localFile = File.OpenWrite($"nuget/ironpython.{latestVersion}.nupkg"))
    {
        var packageZip = await client.GetStreamAsync(packageUrl);
        await packageZip.CopyToAsync(localFile);
    }
}

var extractDir = $"nuget/ironpython.{latestVersion}";
if (!Directory.Exists(extractDir))
{
    Directory.CreateDirectory(extractDir);
}

using (var zip = ZipFile.OpenRead($"nuget/ironpython.{latestVersion}.nupkg"))
{
    foreach (var file in zip.Entries)
    {
            var target = $"{extractDir}/{file.Name}";
            if (File.Exists(target))
            {
                continue;
            }

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