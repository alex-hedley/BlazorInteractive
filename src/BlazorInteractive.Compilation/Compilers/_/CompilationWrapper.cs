using Microsoft.CodeAnalysis;

namespace BlazorInteractive.Compilation;

public class CompilationWrapper : ICompilation
{
    public CompilationWrapper() { }
    
    public CompilationWrapper(Compilation? value)
    {
        Value = value;
    }
    
    public Compilation? Value { get; set; }
}
