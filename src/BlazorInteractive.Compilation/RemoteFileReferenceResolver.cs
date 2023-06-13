using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Reflection;

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

    public async Task<ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default)
    {
        ReferenceResult result;

        if (cancellationToken.IsCancellationRequested) {
            result = new Cancelled();
            return result;
        }

        _logger.LogInformation("Loading assemblies");

        var assemblyBytes = await _assemblyAccessor.GetAsync(importNames, cancellationToken);

        _logger.LogInformation("Assemblies loaded");

        return assemblyBytes.Match<ReferenceResult>(
            assemblies => {
                var references = assemblies
                    .Select(a => new Reference(GetAssemblyNamespaces(a), MetadataReference.CreateFromImage(a)))
                    .Cast<IReference>()
                    .ToList();
                return new ReferenceCollection(references);
            },
            failure => failure,
            cancelled => cancelled
        );
    }

    private IEnumerable<string?> GetAssemblyNamespaces(ImmutableArray<byte> assemblyBytes)
    {
        return Assembly
            .Load(assemblyBytes.ToArray()).GetTypes()
            .Select(t => t.Namespace);
    }
}
