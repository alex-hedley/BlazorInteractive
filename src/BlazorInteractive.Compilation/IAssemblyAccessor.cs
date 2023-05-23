
namespace BlazorInteractive.Compilation;

public interface IAssemblyAccessor
{
    Task<AssemblyResult> GetAsync(CancellationToken cancellationToken);

    // Task<AssemblyResult> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken);
}
