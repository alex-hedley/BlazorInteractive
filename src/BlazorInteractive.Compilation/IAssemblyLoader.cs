namespace BlazorInteractive.Compilation;

public interface IAssemblyLoader
{
     Results.AssemblyLoaderResult Load(ICompilation compilation);
     Results.AssemblyLoaderResult Load(ICSharpCompilation compilation);
     Results.AssemblyLoaderResult Load(IVisualBasicCompilation compilation);
}