using System.Collections.Immutable;

namespace BlazorInteractive.AssemblyCompilation;

public interface INuGetPackageService
{
    Task<IEnumerable<string>> SearchPackagesAsync(string query, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ImmutableArray<byte>>> DownloadPackageAssembliesAsync(string packageId, string version, CancellationToken cancellationToken = default);
}
