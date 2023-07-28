namespace BlazorInteractive.DependencyManagement;

public interface IPackageGetter
{
    // GetVersions()
    Task<PackageResult> GetPackage(string name);
}