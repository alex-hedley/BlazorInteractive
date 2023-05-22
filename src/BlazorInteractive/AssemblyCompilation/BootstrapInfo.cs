namespace BlazorInteractive.AssemblyCompilation;

public sealed record BootstrapInfo
{
    public BootstrapInfoResources Resources { get; set; }

    public IEnumerable<string> Assemblies(IEnumerable<string> filter)
    {
        return Resources.Assembly
            .Where(a => !string.IsNullOrWhiteSpace(a.Key) || !string.IsNullOrWhiteSpace(a.Value))
            .Where(a => filter.Contains(a.Key))
            .Select(a => $"{a.Key}.{a.Value}");
    }
}