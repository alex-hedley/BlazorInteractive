using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Reflection;

using BlazorInteractive.Compilation;
using System.Collections.ObjectModel;

namespace BlazorInteractive.Compilation.Tests;

using static BlazorInteractive.Compilation.Tests.CodeCompilerTestData;

public class CodeCompilerTest
{
    private readonly Mock<ILogger<CodeCompiler>> _logger;
    private readonly Mock<IAssemblyAccessor<Assembly>> _assemblyAccessor;
    private readonly Mock<IReferenceResolver> _referenceResolver;
    private readonly Mock<ICSharpCompiler> _cSharpCompiler;
    private readonly Mock<IAssemblyLoader> _assemblyLoader;
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
        _defaultImports = new List<string>() { SystemAssembly };
        _assemblyAccessor = new Mock<IAssemblyAccessor<Assembly>>();
        _assemblyAccessor.Setup(a => a.GetAsync(_defaultImports, _defaultCancellationToken)).ReturnsAsync(_appDomainAssemblies.ToList().AsReadOnly());
        _defaultCancellationToken = CancellationToken.None;
        _logger = new Mock<ILogger<CodeCompiler>>();
        _cSharpCompiler = new Mock<ICSharpCompiler>();
        _assemblyLoader = new Mock<IAssemblyLoader>();
        
        _sourceCode = "Console.WriteLine(\"Hello, World!\");";

        _referenceResolver = new Mock<IReferenceResolver>();
        _compiler = new CodeCompiler(_logger.Object, _referenceResolver.Object, _cSharpCompiler.Object, _assemblyLoader.Object);
    }

    [Fact]
    public async Task CompileAsync_WithCode_ReturnsSuccessWithResult()
    {   
        var references = new Mock<IReference>();
        var roc = new ReadOnlyCollection<IReference>(new List<IReference>() { references.Object });
        _referenceResolver.Setup(r => r.ResolveAsync(_defaultImports, _defaultCancellationToken)).ReturnsAsync(roc);

        var csc = new Mock<CSharpCompilation>();
        _cSharpCompiler.Setup(c => c.Compile(_sourceCode, SystemAssembly, roc)).Returns(csc.Object);

        var sourceCode = "1 + 1";
        var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
        result.Value.Should().Be(new Success("Hello, World!"));
    }

    [Fact]
    public async Task CompileAsync_WithCodeWithNoResult_ReturnsVoid()
    {
        var result = await _compiler.CompileAsync(_sourceCode, _defaultImports);
        result.Value.Should().BeOfType<Failure>();
    }

    [Fact]
    public async Task CompileAsync_WithoutSourceCode_ReturnFailure()
    {
        var sourceCode = String.Empty;
        
        var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
        result.Value.Should().BeOfType<Failure>();
    }

    [Fact]
    public async Task CompileAsync_WithoutImports_ReturnFailure()
    {
        List<string> imports = new List<string>();
        
        var result = await _compiler.CompileAsync(_sourceCode, imports);
        result.Value.Should().BeOfType<Failure>();
    }

    [Fact]
    public async Task CompileAsync_WithCancellationToken_ReturnsCancelled()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        var result = await _compiler.CompileAsync(_sourceCode, _defaultImports, cancellationToken);
        result.Value.Should().BeOfType<Cancelled>();
    }
    
    [Fact]
    public async Task CompileAsync_WithBadCode_ThrowsCompilationErrorException()
    {
        var sourceCode = "lolCat";
        var result = await _compiler.CompileAsync(sourceCode, _defaultImports);

        result.Value.As<Failure>();
    }
}

public class CodeCompilerTestData
{
    public const string SystemAssembly = "System";
}