
using System.Collections.ObjectModel;
using System.Reflection;

namespace BlazorInteractive.Compilation;

public sealed class LocalAppDomainAssemblyAccessor : IAssemblyAccessor
{
    public Task<AssemblyResult> GetAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<AssemblyResult>(new Cancelled());
        }

        ReadOnlyCollection<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList().AsReadOnly();

        if (!assemblies.Any())
        {
            return Task.FromResult<AssemblyResult>(new Failure(new AppDomainAssemblyException(), "Unable to load AppDomain assemblies"));
        }
        return Task.FromResult<AssemblyResult>(assemblies);
    }

    public Task<AssemblyResult> GetAsync(IEnumerable<string> importNames, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<AssemblyResult>(new Cancelled());
        }

        ReadOnlyCollection<Assembly> assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !string.IsNullOrEmpty(a.FullName) && importNames.Any(n => a.FullName.Contains(n)))
            .ToList()
            .AsReadOnly();

        if (!assemblies.Any())
        {
            return Task.FromResult<AssemblyResult>(new Failure(new AppDomainAssemblyException(), "Unable to load AppDomain assemblies"));
        }
        return Task.FromResult<AssemblyResult>(assemblies);
    }
}
