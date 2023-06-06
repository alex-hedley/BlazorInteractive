using Microsoft.CodeAnalysis.CSharp;

namespace BlazorInteractive.Compilation;

public class CSharpCompilationWrapper : ICSharpCompilation
{
    public CSharpCompilationWrapper() { }
    
    public CSharpCompilationWrapper(CSharpCompilation value)
    {
        Value = value;
    }
    
    public CSharpCompilation Value { get; set; }
}
