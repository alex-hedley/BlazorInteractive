using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public class Reference : IReference
{
    public Reference(MetadataReference value)
    {
        Value = value;
    }

    public MetadataReference Value { get; set; }
}