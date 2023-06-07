
using System.Reflection;
using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public sealed class LocalAppDomainAssemblyAccessor : IAssemblyAccessor<Assembly>
{
    public Task<Results.AssemblyResult<Assembly>> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<Results.AssemblyResult<Assembly>>(new Cancelled());
        }

        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !string.IsNullOrEmpty(a.FullName) && importNames.Any(n => a.FullName.StartsWith(n)))
            .ToList()
            .AsReadOnly();

        return !assemblies.Any()
            ? Task.FromResult<Results.AssemblyResult<Assembly>>(new Failure(new AppDomainAssemblyException(), "Unable to load AppDomain assemblies"))
            : Task.FromResult<Results.AssemblyResult<Assembly>>(assemblies);
    }
}
