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

        // https://stackoverflow.com/a/73944260
        // using HttpResponseMessage response = await http.GetAsync($"{path}/_framework/blazor.boot.json");
        // response.EnsureSuccessStatusCode();
        // string json = await response.Content.ReadAsStringAsync();
        // BootstrapInfo bootstrap = BootstrapInfo.FromJson(json);
        // IEnumerable<string> dlls = bootstrap.Resources.Assembly.Select(assembly => assembly.Key);

        try {
            var references = new List<MetadataReference>();
            var filteredReferences = assemblies.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)).ToList();

            foreach(var reference in filteredReferences)
            {
                using var stream = await _httpClient.GetStreamAsync($"_framework/{reference.Location}", cancellationToken);
                references.Add(MetadataReference.CreateFromStream(stream));
            }

            result = references.AsReadOnly();
        } catch (Exception ex) {
            result = new Failure(ex, $"{ex.Message}");
        }

        return result;
    }
}

