using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.AssemblyCompilation;

public class BlazorAssemblyAccessor : IAssemblyAccessor
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IStorageAccessor _storageAccessor;

    public BlazorAssemblyAccessor(HttpClient httpClient, ILogger<BlazorAssemblyAccessor> logger, IStorageAccessor storageAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _storageAccessor = storageAccessor;
    }

    public async Task<AssemblyResult> GetAsync(CancellationToken cancellationToken)
    {
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

            var assemblies = new List<Assembly>();
            foreach(var assemblyName in bootstrap.Assemblies())
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"_framework/{assemblyName}");
                var assemblyAsBytes = await _storageAccessor.GetAsBytesAsync(message);
                
                Assembly assembly = LoadFromStream(assemblyAsBytes);
                assemblies.Add(assembly);
            }
            return assemblies.AsReadOnly();
        }
        catch (Exception ex)
        {
            return new Failure(ex, $"{ex.Message}");
        }

    }

    private static Assembly LoadFromStream(byte[] assemblySource)
    {
        if (AssemblyLoadContext.Default is not null)
        {
            return AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(assemblySource));
        }
        return Assembly.Load(assemblySource);
    }
}