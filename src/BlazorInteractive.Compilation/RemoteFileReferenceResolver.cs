using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public class RemoteFileReferenceResolver : IReferenceResolver
{
    private readonly IAssemblyAccessor<ImmutableArray<byte>> _assemblyAccessor;
    private readonly ILogger _logger;

    public RemoteFileReferenceResolver(IAssemblyAccessor<ImmutableArray<byte>> assemblyAccessor, ILogger<RemoteFileReferenceResolver> logger)
    {
        _assemblyAccessor = assemblyAccessor;
        _logger = logger;
    }

    public async Task<Results.ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        Results.ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return result;
        }

        _logger.LogInformation("Loading assemblies");

        var assemblyBytes = await _assemblyAccessor.GetAsync(importNames, cancellationToken);

        _logger.LogInformation("Assemblies loaded");

        return assemblyBytes.Match<Results.ReferenceResult>(
            assemblies => {
                return assemblies
                    .Select(a => new Reference(MetadataReference.CreateFromImage(a)))
                    .Cast<IReference>()
                    .ToList()
                    .AsReadOnly();
            },
            failure => failure,
            cancelled => cancelled
        );
    }
}
