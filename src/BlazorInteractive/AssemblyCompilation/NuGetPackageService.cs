using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace BlazorInteractive.AssemblyCompilation;

public class NuGetPackageService : INuGetPackageService
{
    private readonly HttpClient _httpClient;

    private static readonly string[] SupportedTfms =
    [
        "net10.0", "net9.0", "net8.0", "net7.0", "net6.0", "net5.0",
        "netcoreapp3.1", "netstandard2.1", "netstandard2.0"
    ];

    private const string SearchBaseUrl = "https://azuresearch-usnc.nuget.org/query";
    private const string FlatContainerBaseUrl = "https://api.nuget.org/v3-flatcontainer";

    public NuGetPackageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<string>> SearchPackagesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<string>();
        }

        var url = $"{SearchBaseUrl}?q={Uri.EscapeDataString(query)}&prerelease=false&take=20&semVerLevel=2.0.0";
        var response = await _httpClient.GetStringAsync(url, cancellationToken);
        using var doc = JsonDocument.Parse(response);
        return doc.RootElement.GetProperty("data")
            .EnumerateArray()
            .Select(p => p.GetProperty("id").GetString()!)
            .Where(id => !string.IsNullOrEmpty(id))
            .ToList();
    }

    public async Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var id = packageId.ToLowerInvariant();
        var url = $"{FlatContainerBaseUrl}/{id}/index.json";
        var response = await _httpClient.GetStringAsync(url, cancellationToken);
        using var doc = JsonDocument.Parse(response);
        return doc.RootElement.GetProperty("versions")
            .EnumerateArray()
            .Select(v => v.GetString()!)
            .Where(v => !string.IsNullOrEmpty(v))
            .Reverse()
            .ToList();
    }

    public async Task<IEnumerable<ImmutableArray<byte>>> DownloadPackageAssembliesAsync(string packageId, string version, CancellationToken cancellationToken = default)
    {
        var id = packageId.ToLowerInvariant();
        var ver = version.ToLowerInvariant();
        var url = $"{FlatContainerBaseUrl}/{id}/{ver}/{id}.{ver}.nupkg";

        var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);

        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var libEntries = archive.Entries
            .Where(e => e.FullName.StartsWith("lib/", StringComparison.OrdinalIgnoreCase) &&
                        e.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Find entries for the best matching TFM
        foreach (var tfm in SupportedTfms)
        {
            var tfmEntries = libEntries
                .Where(e => e.FullName.StartsWith($"lib/{tfm}/", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (tfmEntries.Count > 0)
            {
                return await ReadEntriesAsync(tfmEntries, cancellationToken);
            }
        }

        // Fall back to any DLL in lib/
        return libEntries.Count > 0
            ? await ReadEntriesAsync(libEntries, cancellationToken)
            : [];
    }

    private static async Task<IEnumerable<ImmutableArray<byte>>> ReadEntriesAsync(
        IEnumerable<ZipArchiveEntry> entries, CancellationToken cancellationToken)
    {
        var result = new List<ImmutableArray<byte>>();
        foreach (var entry in entries)
        {
            using var entryStream = entry.Open();
            using var ms = new MemoryStream();
            await entryStream.CopyToAsync(ms, cancellationToken);
            result.Add(ImmutableArray.Create(ms.ToArray()));
        }
        return result;
    }
}
