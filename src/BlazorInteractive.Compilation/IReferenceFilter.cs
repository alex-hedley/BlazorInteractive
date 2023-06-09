using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public interface IReferenceFilter
{
    Task<ReferenceFilterResult> GetReferencesAsync(CancellationToken cancellationToken);
}