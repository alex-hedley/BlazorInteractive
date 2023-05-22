
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.AssemblyCompilation;

public class BlazorAssemblyAccessor : IAssemblyAccessor
{
    private readonly HttpClient _httpClient;

    public BlazorAssemblyAccessor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AssemblyResult> GetAsync(CancellationToken cancellationToken)
    {
        // https://stackoverflow.com/a/73944260
        using HttpResponseMessage response = await _httpClient.GetAsync($"_framework/blazor.boot.json");
        response.EnsureSuccessStatusCode();
        string json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var bootstrap = JsonSerializer.Deserialize<BootstrapInfo>(json, options);

        // Response status code does not indicate success: 404 (Not Found).
        Console.WriteLine("================================");
        try {
            var bootstrapAssemblies = bootstrap.Assemblies(importNames);
            var references = new List<MetadataReference>();
            foreach(var assembly in bootstrapAssemblies)
            {
                Console.WriteLine(assembly);
                var message = new HttpRequestMessage(HttpMethod.Get, $"_framework/{assembly}");
                var file = await _storageAccessor.GetAsync(message);

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(file)))
                {
                    references.Add(MetadataReference.CreateFromStream(stream));
                }
            }
            // Serialization and deserialization of 'System.IntPtr' instances are not supported. Path: $.WaitHandle.Handle.
            // Can't create a metadata reference to an assembly without location.

            result = references.AsReadOnly();
        } catch (Exception ex) {
            result = new Failure(ex, $"{ex.Message}");
        }
        Console.WriteLine("--------------------------------");
    }
}