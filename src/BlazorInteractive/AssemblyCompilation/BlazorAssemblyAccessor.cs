using System.Collections.Immutable;

using BlazorInteractive.Compilation;
using BlazorInteractive.Compilation.Results;

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
            var assemblies = new List<ImmutableArray<byte>>();

            foreach(var assemblyName in importNames)
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