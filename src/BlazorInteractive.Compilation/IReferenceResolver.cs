namespace BlazorInteractive.Compilation;

public interface IReferenceResolver
{
    Task<Results.ReferenceResult> ResolveAsync(IEnumerable<string> importNames, CancellationToken cancellationToken = default);
}