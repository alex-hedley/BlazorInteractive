using Microsoft.CodeAnalysis.Scripting;

using FluentAssertions;

namespace BlazorInteractive.Compilation.Tests;

public class ScriptCompilerTest
{
    private readonly ScriptCompiler _compiler;
    private readonly List<string> _defaultImports;

    private readonly string _sourceCode = string.Empty;
    
    public ScriptCompilerTest()
    {
        _compiler = new ScriptCompiler();
        _defaultImports = new List<string>() { "System" };
        _sourceCode = "Console.WriteLine(\"Hello, World!\");";
    }

    [Fact]
    public async Task CompileAsync_WithoutCode_ThrowsException()
    {
        var source = String.Empty;
        
        Func<Task> act = () => _compiler.CompileAsync(source, _defaultImports);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CompileAsync_WithoutImports_ThrowsException()
    {
        Func<Task> act = () => _compiler.CompileAsync(_sourceCode, null);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task CompileAsync_WithCancellationToken_ThrowsException()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        
        Func<Task> act = () => _compiler.CompileAsync(_sourceCode, _defaultImports, cancellationToken);

        cancellationTokenSource.Cancel();
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
    
    [Fact]
    public async Task CompileAsync_WithCodeWithNoResult_ReturnsEmptyString()
    {
        var result = await _compiler.CompileAsync(_sourceCode, _defaultImports);
        result.Should().Be(string.Empty);
    }

    [Fact]
    public async Task CompileAsync_WithBadCode_ThrowsCompilationErrorException()
    {
        var source = "lolCat";

        Func<Task> act = () => _compiler.CompileAsync(source, _defaultImports);
        await act.Should().ThrowAsync<CompilationErrorException>();
    }

    [Fact]
    public async Task CompileAsync_WithCode_ReturnsResult()
    {
        var source = "1 + 1";

        var result = await _compiler.CompileAsync(source, _defaultImports);
        result.Should().Be("2");
    }
}
