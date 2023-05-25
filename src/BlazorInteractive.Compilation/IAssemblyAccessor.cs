
namespace BlazorInteractive.Compilation;

public interface IAssemblyAccessor<TAssembly>
{
    Task<AssemblyResult<TAssembly>> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken);
}
