using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;

namespace BlazorInteractive.Compilation;

public interface IAssemblyLoader
{
     AssemblyLoaderResult Load(ICSharpCompilation compilation);
}