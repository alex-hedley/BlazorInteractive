using Microsoft.CodeAnalysis;

using System.Reflection;

namespace BlazorInteractive.Compilation;

public class RemoteFileReferenceResolver : IReferenceResolver
{
    private readonly HttpClient _httpClient;

    public RemoteFileReferenceResolver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReferenceResult> ResolveAsync(IEnumerable<Assembly> assemblies, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return result;
        }

        try {
            var references = new List<MetadataReference>();

            foreach(var reference in assemblies.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)))
            {
                using var stream = await _httpClient.GetStreamAsync($"_framework/{reference.Location}");
                references.Add(MetadataReference.CreateFromStream(stream));
            }
            
            result = references.AsReadOnly();
        } catch (Exception ex) {
            result = new Failure(ex, $"{ex.Message}");
        }
        
        return result;
    }
}

