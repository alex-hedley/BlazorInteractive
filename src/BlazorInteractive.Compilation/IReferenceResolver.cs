using System.Reflection;

namespace BlazorInteractive.Compilation;

public interface IReferenceResolver
{
    Task<ReferenceResult> ResolveAsync(IEnumerable<Assembly> assemblies, CancellationToken cancellationToken = default);
}