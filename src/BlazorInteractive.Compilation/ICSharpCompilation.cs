using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public interface ICSharpCompilation
{
    CSharpCompilation? Value { get; set; }
}