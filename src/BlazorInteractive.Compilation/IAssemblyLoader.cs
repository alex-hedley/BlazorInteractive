namespace BlazorInteractive.Compilation;

public interface IAssemblyLoader
{
     AssemblyLoaderResult Load(ICSharpCompilation compilation);
}