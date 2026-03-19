namespace BlazorInteractive.AssemblyCompilation;

public sealed record BootstrapInfo
{
    public BootstrapInfoResources? Resources { get; set; }

    public IEnumerable<string>? Assemblies(IEnumerable<string>? filter = default)
    {
        // Merge Assembly and CoreAssembly categories (CoreAssembly was added in .NET 10)
        var allAssemblies = new Dictionary<string, string>();

        if (Resources?.Assembly != null)
        {
            foreach (var a in Resources.Assembly)
                allAssemblies[a.Key] = a.Value;
        }

        if (Resources?.CoreAssembly != null)
        {
            foreach (var a in Resources.CoreAssembly)
                allAssemblies[a.Key] = a.Value;
        }

        if (allAssemblies.Count == 0) return null;

        var query = allAssemblies
            .Where(a => !string.IsNullOrWhiteSpace(a.Key) || !string.IsNullOrWhiteSpace(a.Value));

        if (filter is not null)
        {
            query = query.Where(a => filter.Select(filterName => $"{filterName}.dll").Contains(a.Key));
        }

        return query.Select(a => a.Key);
    }
}