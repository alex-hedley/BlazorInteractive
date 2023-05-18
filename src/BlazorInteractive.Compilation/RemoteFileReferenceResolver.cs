using Microsoft.CodeAnalysis;

using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorInteractive.Compilation;

public class RemoteFileReferenceResolver : IReferenceResolver
{
    private readonly HttpClient _httpClient;
    private IStorageAccessor _storageAccessor;

    public RemoteFileReferenceResolver(HttpClient httpClient, IStorageAccessor storageAccessor)
    {
        _httpClient = httpClient;
        _storageAccessor = storageAccessor;
    }

    public async Task<ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return result;
        }

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

        return result;
    }
}

public class BootstrapInfo
{
    public Resources Resources { get; set; }

    public IEnumerable<string> Assemblies(IEnumerable<string> filter)
    {
        return Resources.Assembly
            .Where(a => !string.IsNullOrWhiteSpace(a.Key) || !string.IsNullOrWhiteSpace(a.Value))
            .Where(a => filter.Contains(a.Key))
            .Select(a => $"{a.Key}.{a.Value}");
    }
}

public class Resources
{
    public Dictionary<string, string> Assembly { get; set; }
}

