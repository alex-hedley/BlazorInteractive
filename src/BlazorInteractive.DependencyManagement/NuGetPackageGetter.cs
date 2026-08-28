using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using OneOf.Types;

using MsLogger = Microsoft.Extensions.Logging.ILogger;

namespace BlazorInteractive.DependencyManagement;

public class NuGetPackageGetter : IPackageGetter
{
    private readonly MsLogger _logger;

    public NuGetPackageGetter(ILogger<NuGetPackageGetter> logger)
    {
        _logger = logger;
    }

    public async Task<PackageResult> GetPackage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new Failure("Package name cannot be null or empty.");
        }

        try
        {
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var versions = await resource.GetAllVersionsAsync(name, cache, NullLogger.Instance, CancellationToken.None);

            if (versions is null || !versions.Any())
            {
                _logger.LogWarning("Package '{Name}' not found on NuGet.", name);
                return new Failure($"Package '{name}' was not found on NuGet.");
            }

            _logger.LogInformation("Package '{Name}' found on NuGet with {Count} version(s).", name, versions.Count());
            return new Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up package '{Name}'.", name);
            return new Failure(ex, ex.Message);
        }
    }
}
