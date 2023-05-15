using Microsoft.CodeAnalysis;

using System.Reflection;

namespace BlazorInteractive.Compilation;

public class LocalFileReferenceResolver : IReferenceResolver
{
    public Task<ReferenceResult> ResolveAsync(IEnumerable<Assembly> assemblies, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return Task.FromResult(result);
        }

        try {
            result = assemblies
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Cast<MetadataReference>()
                .ToList()
                .AsReadOnly();

        } catch (Exception ex) {
            result = new Failure(ex, $"{ex.Message}");
        }
        
        return Task.FromResult(result);
    }
}

// private IEnumerable<MetadataReference> _references;

// var refs = AppDomain.CurrentDomain.GetAssemblies();
// var client = new HttpClient 
// {
//         BaseAddress = new Uri(navigationManager.BaseUri)
// };

// var references = new List<MetadataReference>();

// foreach(var reference in refs.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)))
// {
//     var stream = await client.GetStreamAsync($"_framework/{reference.Location}");
//     references.Add(MetadataReference.CreateFromStream(stream));
// }
// _references = references;