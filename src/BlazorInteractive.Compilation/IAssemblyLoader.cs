namespace BlazorInteractive.Compilation;

public interface IAssemblyLoader
{
     Results.AssemblyLoaderResult Load(ICompilation compilation);
}