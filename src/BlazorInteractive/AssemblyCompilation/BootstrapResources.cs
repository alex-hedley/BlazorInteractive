namespace BlazorInteractive.AssemblyCompilation;

public sealed record BootstrapInfoResources
{
    public Dictionary<string, string>? Assembly { get; set; }
    public Dictionary<string, string>? CoreAssembly { get; set; }
}