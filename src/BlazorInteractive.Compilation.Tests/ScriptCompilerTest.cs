// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.Scripting;
// using System.Collections.Immutable;

// using BlazorInteractive.Compilation;
// using System.Reflection;

// namespace BlazorInteractive.Compilation.Tests;

// public class ScriptCompilerTest
// {
//     private readonly Mock<IAssemblyAccessor<Assembly>> _assemblyAccessor;
//     private readonly CancellationToken _defaultCancellationToken;
//     private readonly ScriptCompiler _compiler;
//     private readonly List<string> _defaultImports;

//     private readonly string _sourceCode = string.Empty;
//     private static Assembly[] _appDomainAssemblies;

//     static ScriptCompilerTest()
//     {
//         _appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
//     }
    
//     public ScriptCompilerTest()
//     {
//         _defaultImports = new List<string>() { "System" };
//         _assemblyAccessor = new Mock<IAssemblyAccessor<Assembly>>();
//         _assemblyAccessor.Setup(a => a.GetAsync(_defaultImports, _defaultCancellationToken)).ReturnsAsync(_appDomainAssemblies.ToList().AsReadOnly());
//         _defaultCancellationToken = CancellationToken.None;
//         _compiler = new ScriptCompiler(new LocalFileReferenceResolver(_assemblyAccessor.Object));
//         _sourceCode = "Console.WriteLine(\"Hello, World!\");";
//     }

//     [Fact]
//     public async Task CompileAsync_WithCode_ReturnsSuccessWithResult()
//     {
//         // _assemblyAccessor.Setup(a => a.GetAsync(_defaultCancellationToken)).ReturnsAsync(_appDomainAssemblies.ToList().AsReadOnly());

//         var sourceCode = "1 + 1";

//         var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
//         result.Value.Should().Be(new Success("2"));
//     }

//     [Fact]
//     public async Task CompileAsync_WithCodeWithNoResult_ReturnsVoid()
//     {
//         var result = await _compiler.CompileAsync(_sourceCode, _defaultImports);
//         result.Value.Should().BeOfType<Void>();
//     }

//     [Fact]
//     public async Task CompileAsync_WithoutCode_ReturnFailure()
//     {
//         var sourceCode = String.Empty;
        
//         var result = await _compiler.CompileAsync(sourceCode, _defaultImports);
//         result.Value.Should().Be(new Failure($"{nameof(sourceCode)} cannot be null or empty"));
//     }

//     [Fact]
//     public async Task CompileAsync_WithoutImports_ReturnFailure()
//     {
//         List<string> imports = new List<string>();
        
//         var result = await _compiler.CompileAsync(_sourceCode, imports);
//         result.Value.Should().Be(new Failure($"{nameof(imports)} cannot be null or empty"));
//     }

//     [Fact]
//     public async Task CompileAsync_WithCancellationToken_ReturnsCancelled()
//     {
//         var cancellationTokenSource = new CancellationTokenSource();
//         var cancellationToken = cancellationTokenSource.Token;
        
//         cancellationTokenSource.Cancel();
//         var result = await _compiler.CompileAsync(_sourceCode, _defaultImports, cancellationToken);
//         result.Value.Should().BeOfType<Cancelled>();
//     }
    
//     [Fact]
//     public async Task CompileAsync_WithBadCode_ThrowsCompilationErrorException()
//     {
//         var sourceCode = "lolCat";
//         var result = await _compiler.CompileAsync(sourceCode, _defaultImports);

//         result.Value.As<Failure>().Exception.Should().BeOfType<CompilationErrorException>();
//     }
// }
