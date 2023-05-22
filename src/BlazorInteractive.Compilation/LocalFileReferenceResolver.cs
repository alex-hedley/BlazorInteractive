using Microsoft.CodeAnalysis;
using System.Reflection;

namespace BlazorInteractive.Compilation;

public class LocalFileReferenceResolver : IReferenceResolver
{
    private readonly IAssemblyAccessor _assemblyAccessor;

    public LocalFileReferenceResolver(IAssemblyAccessor assemblyAccessor)
    {
        _assemblyAccessor = assemblyAccessor;
    }

    public async Task<ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            return new Cancelled();
        }
        
        try
        {    
            AssemblyResult assembliesResult = await _assemblyAccessor.GetAsync(cancellationToken);

            result = assembliesResult
                .Match<ReferenceResult>(
                    assemblies => {
                        return assemblies
                            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                            .Select(a => MetadataReference.CreateFromFile(a.Location))
                            .Cast<MetadataReference>()
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