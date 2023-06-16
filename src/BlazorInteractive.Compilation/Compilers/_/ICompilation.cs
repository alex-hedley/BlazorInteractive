using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public interface ICompilation
{
    Microsoft.CodeAnalysis.Compilation? Value { get; set; }
}