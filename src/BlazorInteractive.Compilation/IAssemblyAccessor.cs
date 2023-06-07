
namespace BlazorInteractive.Compilation;

public interface IAssemblyAccessor<TAssembly>
{
    Task<Results.AssemblyResult<TAssembly>> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken);
}
