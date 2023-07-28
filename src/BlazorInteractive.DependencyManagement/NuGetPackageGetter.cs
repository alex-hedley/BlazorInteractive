using Microsoft.Extensions.Logging;

namespace BlazorInteractive.DependencyManagement;

public class NuGetPackageGetter : IPackageGetter
{
    private readonly ILogger _logger;

    public NuGetPackageGetter(ILogger<NuGetPackageGetter> logger)
    {
        _logger = logger;
    }

    public Task<PackageResult> GetPackage(string name)
    {
        throw new NotImplementedException();
    }
}