
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
        
        ReadOnlyCollection<Assembly> assemblies = new(AppDomain.CurrentDomain.GetAssemblies().ToList().AsReadOnly());
        if (!assemblies.Any())
        {
            return Task.FromResult<AssemblyResult>(new Failure(new AppDomainAssemblyException(), "Unable to load AppDomain assemblies"));
        }
        return Task.FromResult<AssemblyResult>(assemblies);
    }
}
