
namespace BlazorInteractive.Compilation;

public interface IAssemblyAccessor
{
    Task<AssemblyResult> GetAsync(CancellationToken cancellationToken);
}
