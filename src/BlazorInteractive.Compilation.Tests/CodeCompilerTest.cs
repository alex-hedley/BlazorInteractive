using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;

using BlazorInteractive.Compilation;

namespace BlazorInteractive.Compilation.Tests;

public class CodeCompilerTest
{
    private readonly Mock<ILogger<CodeCompiler>> _logger;
    private readonly Mock<IAssemblyAccessor<Assembly>> _assemblyAccessor;
    private readonly CancellationToken _defaultCancellationToken;
    private readonly CodeCompiler _compiler;
    private readonly List<string> _defaultImports;

    private readonly string _sourceCode = string.Empty;
    private static Assembly[] _appDomainAssemblies;

    static CodeCompilerTest()
    {
        _appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    }
    
    public CodeCompilerTest()
    {
        _defaultImports = new List<string>() { "System" };
        _assemblyAccessor = new Mock<IAssemblyAccessor<Assembly>>();
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultImports, _defaultCancellationToken)).ReturnsAsync(_appDomainAssemblies.ToList().AsReadOnly());
        _defaultCancellationToken = CancellationToken.None;
        
        _sourceCode = "Console.WriteLine(\"Hello, World!\");";
        // _compiler = new CodeCompiler(_logger, _referenceResolver);
        
        //_sourceCode, _defaultImports
    }

    // [Fact]

}