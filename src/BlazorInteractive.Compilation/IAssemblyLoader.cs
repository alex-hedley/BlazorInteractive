namespace BlazorInteractive.Compilation;

public interface IAssemblyLoader
{
     Results.AssemblyLoaderResult Load(ICSharpCompilation compilation);
}