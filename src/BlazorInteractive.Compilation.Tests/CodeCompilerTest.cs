using System.Collections.ObjectModel;
using System.Reflection;

namespace BlazorInteractive.Compilation.Tests;

using static BlazorInteractive.Compilation.Tests.CodeCompilerTestData;

public class CodeCompilerTest
{
    private readonly Mock<IAssemblyAccessor<Assembly>> _assemblyAccessor;
    private readonly Mock<IReferenceResolver> _referenceResolver;
    private readonly Mock<ICSharpCompiler> _cSharpCompiler;
    private readonly Mock<IAssemblyLoader> _assemblyLoader;
    private readonly CancellationToken _defaultCancellationToken;
    private readonly CodeCompiler _compiler;
    private readonly List<string> _defaultImports;

    private readonly string _sourceCode = string.Empty;
    private static readonly Assembly[] _appDomainAssemblies;

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
        _cSharpCompiler = new Mock<ICSharpCompiler>();
        _assemblyLoader = new Mock<IAssemblyLoader>();

        _sourceCode = "Console.WriteLine(\"Hello, World!\");";

        _referenceResolver = new Mock<IReferenceResolver>();
        _compiler = new CodeCompiler(_referenceResolver.Object, _cSharpCompiler.Object, _assemblyLoader.Object);
    }

    [Fact]
    public async Task CompileAsync_WithCode_ReturnsSuccessWithResult()
    {
        var references = new Mock<IReference>();
        var roc = new ReadOnlyCollection<IReference>(new List<IReference>() { references.Object });
        _referenceResolver.Setup(r => r.ResolveAsync(_defaultImports, _defaultCancellationToken)).ReturnsAsync(roc);

        var csc = new Mock<ICSharpCompilation>();
        var wrapper = new CSharpCompilationWrapper();
        //_cSharpCompiler.Setup(c => c.Compile(_sourceCode, SystemAssembly, roc)).Returns(wrapper);
        _cSharpCompiler.Setup(c => c.Compile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ReadOnlyCollection<IReference>>())).Returns(wrapper);

        var dummyAssembly = typeof(CodeCompilerTest).Assembly;
        _assemblyLoader.Setup(a => a.Load(It.IsAny<ICSharpCompilation>())).Returns(dummyAssembly);

        var sourceCode = "1 + 1";
        var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
        result.Value.Should().BeAssignableTo<Assembly>();
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
        var sourceCode = string.Empty;

        var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
        result.Value.Should().BeOfType<Failure>();
    }

    [Fact]
    public async Task CompileAsync_WithoutImports_ReturnFailure()
    {
        var imports = new List<string>();

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