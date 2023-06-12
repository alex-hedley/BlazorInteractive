using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public interface IReference
{
    IEnumerable<string?> DistinctNamespaces();

    IEnumerable<string?> Namespaces { get; set; }

    MetadataReference Value { get; set; }
}
