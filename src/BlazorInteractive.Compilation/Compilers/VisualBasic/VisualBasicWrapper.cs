using Microsoft.CodeAnalysis.VisualBasic;

namespace BlazorInteractive.Compilation;

public class VisualBasicCompilationWrapper : IVisualBasicCompilation
{
    public VisualBasicCompilationWrapper() { }
    
    public VisualBasicCompilationWrapper(VisualBasicCompilation? value)
    {
        Value = value;
    }
    
    public VisualBasicCompilation? Value { get; set; }
}
