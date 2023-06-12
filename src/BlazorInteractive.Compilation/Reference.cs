using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public class Reference : IReference
{
    public Reference(IEnumerable<string?> namespaces, MetadataReference value)
    {
        Namespaces = namespaces;
        Value = value;
    }

    public IEnumerable<string?> Namespaces { get; set; }

    public MetadataReference Value { get; set; }

    public IEnumerable<string?> DistinctNamespaces()
    {
        return Namespaces.Distinct();
    }
}