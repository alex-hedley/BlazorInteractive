
using System.Collections.ObjectModel;
using System.Reflection;

namespace BlazorInteractive.Compilation;

public sealed class LocalAppDomainAssemblyAccessor : IAssemblyAccessor<Assembly>
{
    public Task<AssemblyResult<Assembly>> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<AssemblyResult<Assembly>>(new Cancelled());
        }

        ReadOnlyCollection<Assembly> assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !string.IsNullOrEmpty(a.FullName) && importNames.Any(n => a.FullName.StartsWith(n)))
            .ToList()
            .AsReadOnly();

        if (!assemblies.Any())
        {
            return Task.FromResult<AssemblyResult<Assembly>>(new Failure(new AppDomainAssemblyException(), "Unable to load AppDomain assemblies"));
        }
        return Task.FromResult<AssemblyResult<Assembly>>(assemblies);
    }
}
