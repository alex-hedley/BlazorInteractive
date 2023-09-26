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

        // https://stackoverflow.com/a/73944260
        using var response = await _httpClient.GetAsync($"_framework/blazor.boot.json", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new Failure("blazor.boot.json failed to load");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var bootstrap = JsonSerializer.Deserialize<BootstrapInfo>(json, options);
        if (bootstrap is null)
        {
            return new Failure("No Assemblies found in blazor.boot.json");
        }

        _logger.LogInformation("Assemblies total {Count}", (bootstrap.Assemblies() ?? Array.Empty<string>()).Count());

        return bootstrap.Assemblies()?
            .Where(n => n.StartsWith("System."))
            .Distinct()
            .ToList()
            .AsReadOnly();
    }
}
