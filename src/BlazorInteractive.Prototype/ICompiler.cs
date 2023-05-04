public interface ICompiler
{
    Task<string> Compile(string sourceCode, IEnumerable<string> imports);
}
