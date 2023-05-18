namespace BlazorInteractive.Compilation;

public interface IReferenceResolver
{
    Task<ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default);
}