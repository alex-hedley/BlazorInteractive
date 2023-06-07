using Microsoft.CodeAnalysis;
using System.Reflection;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public class LocalFileReferenceResolver : IReferenceResolver
{
    private readonly IAssemblyAccessor<Assembly> _assemblyAccessor;

    public LocalFileReferenceResolver(IAssemblyAccessor<Assembly> assemblyAccessor)
    {
        _assemblyAccessor = assemblyAccessor;
    }

    public async Task<Results.ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        Results.ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            return new Cancelled();
        }
        
        try
        {    
            var assembliesResult = await _assemblyAccessor.GetAsync(importNames, cancellationToken);

            result = assembliesResult
                .Match<Results.ReferenceResult>(
                    assemblies => {
                        return assemblies
                            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                            .Select(a => new Reference(MetadataReference.CreateFromFile(a.Location)))
                            .Cast<IReference>()
                            .ToList()
                            .AsReadOnly();
                    }, 
                    failure => failure,
                    cancelled => cancelled
                );

        } catch (Exception ex) {
            result = new Failure(ex, $"{ex.Message}");
        }
        
        return result;
    }
}