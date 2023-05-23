namespace BlazorInteractive.AssemblyCompilation;

public sealed record BootstrapInfo
{
    public BootstrapInfoResources Resources { get; set; }

    public IEnumerable<string> Assemblies(IEnumerable<string>? filter = default)
    {
        var query = Resources.Assembly
            .Where(a => !string.IsNullOrWhiteSpace(a.Key) || !string.IsNullOrWhiteSpace(a.Value));

        if (filter is not null)
        {
            query = query.Where(a => filter.Contains(a.Key));
        }
        
        return query.Select(a => $"{a.Key}.{a.Value}");
    }
}