using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public class CompilationWrapper : ICompilation
{
    public CompilationWrapper() { }
    
    public CompilationWrapper(Microsoft.CodeAnalysis.Compilation? value)
    {
        Value = value;
    }
    
    public Microsoft.CodeAnalysis.Compilation? Value { get; set; }
}
