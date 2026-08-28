using BlazorInteractive.Compilation.Results;

namespace BlazorInteractive.Compilation;

public interface IILEmitter
{
    ILEmitResult Emit(ICSharpCompilation compilation);
}
