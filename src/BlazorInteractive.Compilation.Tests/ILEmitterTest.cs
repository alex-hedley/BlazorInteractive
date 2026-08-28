using BlazorInteractive.Compilation.Results;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;

namespace BlazorInteractive.Compilation.Tests;

public class ILEmitterTest
{
    private readonly ILEmitter _emitter;

    public ILEmitterTest()
    {
        var logger = new Mock<ILogger<ILEmitter>>();
        _emitter = new ILEmitter(logger.Object);
    }

    [Fact]
    public void Emit_WithValidCompilation_ReturnsILString()
    {
        var compilation = BuildCompilation("var x = 1 + 1;");
        var result = _emitter.Emit(compilation);

        result.IsT0.Should().BeTrue();
        result.AsT0.Should().NotBeNullOrWhiteSpace();
        result.AsT0.Should().Contain(".method");
    }

    [Fact]
    public void Emit_WithInvalidCode_ReturnsFailure()
    {
        var compilation = BuildCompilation("this is not valid C#!!!!");
        var result = _emitter.Emit(compilation);

        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeOfType<Failure>();
    }

    [Fact]
    public void Emit_WithNullCompilationValue_ReturnsFailure()
    {
        var wrapper = new CSharpCompilationWrapper();
        var result = _emitter.Emit(wrapper);

        result.IsT1.Should().BeTrue();
    }

    private static CSharpCompilationWrapper BuildCompilation(string userCode)
    {
        var sourceCodeFormat = @"
        namespace BlazorInteractive
        {{
            public class Program
            {{
                public static void Main()
                {{
                    {0}
                }}
            }}
        }}
        ";
        var sourceCode = string.Format(sourceCodeFormat, userCode);

        var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var references = appDomainAssemblies
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var csharpCompilation = CSharpCompilation.Create(
            "test",
            [syntaxTree],
            references,
            compilationOptions);

        return new CSharpCompilationWrapper(csharpCompilation);
    }
}
