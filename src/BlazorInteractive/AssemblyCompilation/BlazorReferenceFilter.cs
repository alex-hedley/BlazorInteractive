using System.Text.Json;

using BlazorInteractive.Compilation;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.AssemblyCompilation;

public class BlazorReferenceFilter : IReferenceFilter
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public BlazorReferenceFilter(HttpClient httpClient, ILogger<BlazorReferenceFilter> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ReferenceFilterResult> GetReferencesAsync(CancellationToken cancellationToken)
    {
        return await GetBootstrapReferences(cancellationToken);
    }

    private async Task<ReferenceFilterResult> GetBootstrapReferences(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new Cancelled();
        }

        // .NET 10: the assembly manifest is embedded in _framework/dotnet.js
        // between /*json-start*/ and /*json-end*/ markers.
        using var dotnetJsResponse = await _httpClient.GetAsync("_framework/dotnet.js", cancellationToken);
        if (dotnetJsResponse.IsSuccessStatusCode)
        {
            var js = await dotnetJsResponse.Content.ReadAsStringAsync(cancellationToken);
            const string startMarker = "/*json-start*/";
            const string endMarker = "/*json-end*/";
            var start = js.IndexOf(startMarker, StringComparison.Ordinal);
            var end = js.IndexOf(endMarker, StringComparison.Ordinal);
            if (start >= 0 && end > start)
            {
                var json = js[(start + startMarker.Length)..end];
                return ParseBootstrapJson(json);
            }
        }

        // .NET 8 fallback: blazor.boot.json
        // https://stackoverflow.com/a/73944260
        using var response = await _httpClient.GetAsync("_framework/blazor.boot.json", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new Failure("blazor.boot.json failed to load");
        }

        var bootJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return ParseBootstrapJson(bootJson);
    }

    private ReferenceFilterResult ParseBootstrapJson(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var bootstrap = JsonSerializer.Deserialize<BootstrapInfo>(json, options);
        if (bootstrap is null)
        {
            return new Failure("No Assemblies found in bootstrap JSON");
        }

        _logger.LogInformation("Assemblies total {Count}", (bootstrap.Assemblies() ?? Array.Empty<string>()).Count());

        return bootstrap.Assemblies()?
            .Where(n => n.StartsWith("System."))
            .Distinct()
            .ToList()
            .AsReadOnly();
    }
}
