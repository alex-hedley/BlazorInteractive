using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public interface IReference 
{
    MetadataReference Value { get; set; }
}
