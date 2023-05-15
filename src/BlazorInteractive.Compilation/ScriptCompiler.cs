using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace BlazorInteractive.Compilation;

public class ScriptCompiler : ICompiler
{
    private IReferenceResolver _referenceResolver;

    public ScriptCompiler(IReferenceResolver referenceResolver)
    {
        _referenceResolver = referenceResolver;
    }


    public async Task<CompilationResult> CompileAsync(string sourceCode, ICollection<string>? imports, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            return new Failure($"{nameof(sourceCode)} cannot be null or empty");
        }

        if (imports is null || !imports.Any())
        {
            return new Failure($"{nameof(imports)} cannot be null or empty");
        }

        // // Grab any Console info.
        // var defaultWriter = Console.Out;
        // using var writer = new StringWriter();
        // Console.SetOut(writer);
        // var result = writer.ToString();
        // Console.SetOut(defaultWriter);

        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var references = await _referenceResolver.ResolveAsync(assemblies, cancellationToken);

            return await references.Match<Task<CompilationResult>>(
                async refs =>
                {
                    var scriptOptions = ScriptOptions.Default.WithImports(imports).WithReferences(refs);
                    object result = await CSharpScript.EvaluateAsync(sourceCode, scriptOptions, cancellationToken: cancellationToken);
                    return (result is null) ? new Void() : new Success(result.ToString()!);
                },
                async failure => await Task.FromResult(failure),
                async cancelled => await Task.FromResult(cancelled));
        }
        catch (CompilationErrorException cee)
        {
            return new Failure(cee, cee.Message);
        }
        catch (OperationCanceledException)
        {
            return new Cancelled();
        }
    }

    // private IEnumerable<MetadataReference> _references;

    // protected async override Task OnInitializedAsync()
    // {
    //     var refs = AppDomain.CurrentDomain.GetAssemblies();
    //     var client = new HttpClient 
    //     {
    //             BaseAddress = new Uri(navigationManager.BaseUri)
    //     };

    //     var references = new List<MetadataReference>();

    //     foreach(var reference in refs.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)))
    //     {
    //         var stream = await client.GetStreamAsync($"_framework/{reference.Location}");
    //         references.Add(MetadataReference.CreateFromStream(stream));
    //     }
    //     Disabled = false;
    //     _references = references;
    // }
}