using Microsoft.CodeAnalysis;
using System.Net;
using System.Reflection;

namespace BlazorInteractive.Compilation;

public class RemoteFileReferenceResolver : IReferenceResolver
{
    private readonly IAssemblyAccessor _assemblyAccessor;
    private readonly HttpClient _httpClient;
    private IStorageAccessor _storageAccessor;

    public RemoteFileReferenceResolver(IAssemblyAccessor assemblyAccessor, HttpClient httpClient, IStorageAccessor storageAccessor)
    {
        _assemblyAccessor = assemblyAccessor;
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

        var assemblies = await _assemblyAccessor.GetAsync(cancellationToken);
        
        return assemblies.Match<ReferenceResult>(
            assemblies => {
                return assemblies
                        //TODO: Localtion doesn't exist on files loaded from cachestorage in blazor
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                    .Cast<MetadataReference>()
                    .ToList()
                    .AsReadOnly();
                    
            }, 
            failure => failure,
            cancelled => cancelled
        );
    }

}
