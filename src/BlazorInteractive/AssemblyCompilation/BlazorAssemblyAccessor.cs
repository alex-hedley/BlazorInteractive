using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text.Json;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.AssemblyCompilation;

public class BlazorAssemblyAccessor : IAssemblyAccessor<ImmutableArray<byte>>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public BlazorAssemblyAccessor(HttpClient httpClient, ILogger<BlazorAssemblyAccessor> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AssemblyResult<ImmutableArray<byte>>> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ImportNames total {Count}", importNames.Count());
        
        if (cancellationToken.IsCancellationRequested)
        {
            return new Cancelled();
        }

        try
        {
            // https://stackoverflow.com/a/73944260
            using HttpResponseMessage response = await _httpClient.GetAsync($"_framework/blazor.boot.json", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new Failure("blazor.boot.json failed to load");
            }

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var bootstrap = JsonSerializer.Deserialize<BootstrapInfo>(json, options);
            if (bootstrap is null)
            {
                return new Failure("No Assemblies found in blazor.boot.json");
            }

            _logger.LogInformation("Assemblies total {Count}", bootstrap.Assemblies().Count());

            var assemblies = new List<ImmutableArray<byte>>();

            var filteredAssemblies = bootstrap.Assemblies(importNames);
            _logger.LogInformation("Filtered assemblies total {Count}", filteredAssemblies.Count());

            foreach(var assemblyName in filteredAssemblies)
            {
                var assemblyUrl = $"./_framework/{assemblyName}";
                _logger.LogInformation($"Loading DLL from {assemblyUrl}");
                var assemblyResponse = await _httpClient.GetAsync(assemblyUrl);
                if (assemblyResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("🟢 Success");
                    var assemblyBytes = await assemblyResponse.Content.ReadAsByteArrayAsync();
                    if (assemblyBytes is null) continue;
                    var assemblyArray = ImmutableArray.Create(assemblyBytes);
                    assemblies.Add(assemblyArray);
                    _logger.LogInformation("Added to Assemblies list");
                }
                else
                {
                    _logger.LogInformation("🔴 Failed");
                }
            }
            return assemblies.AsReadOnly();
        }
        catch (Exception ex)
        {
            return new Failure(ex, $"{ex.Message}");
        }
    }
}