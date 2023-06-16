using Microsoft.CodeAnalysis.VisualBasic;

namespace BlazorInteractive.Compilation;

public interface IVisualBasicCompilation
{
    VisualBasicCompilation? Value { get; set; }
}