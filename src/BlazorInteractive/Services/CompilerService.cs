using System;
using System.Diagnostics;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting.Hosting;

using OneOf;
using OneOf.Types;

namespace BlazorInteractive.Services;

public class CompilerService
{
    private CSharpCompilation _previousCompilation;
    private IEnumerable<MetadataReference> _references;
    private object[] _submissionStates = new object[] { null, null };
    private int _submissionIndex = 0;

    public CompilerService()
	{
        TryLoadReferences();
    }

    public async Task Run(string code)
    {
        await RunSubmission(code);
    }


    // https://github.com/filipw/Strathweb.Samples.BlazorCSharpInteractive/blob/35242dcd2d615ef885b736b4b8bb7a4f0ee82a34/Pages/Index.razor#L139
    private Task <OneOf<Assembly, IList<Diagnostic>>> TryCompile(string source)
    {
        var scriptCompilation = CSharpCompilation.CreateScriptCompilation(
            Path.GetRandomFileName(),
            CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script).WithLanguageVersion(LanguageVersion.Preview)),
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: new[]
            {
                    "System",
                    "System.IO",
                    "System.Collections.Generic",
                    "System.Console",
                    "System.Diagnostics",
                    "System.Dynamic",
                    "System.Linq",
                    "System.Linq.Expressions",
                    "System.Net.Http",
                    "System.Text",
                    "System.Threading.Tasks"
            }),
            _previousCompilation
        );

        var errorDiagnostics = scriptCompilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).ToList();
        if (errorDiagnostics.Any())
        {
            return Task.FromResult<OneOf<Assembly, IList<Diagnostic>>>(errorDiagnostics);
        }

        using (var peStream = new MemoryStream())
        {
            var emitResult = scriptCompilation.Emit(peStream);

            if (emitResult.Success)
            {
                _submissionIndex++;
                _previousCompilation = scriptCompilation;
                Task.FromResult<OneOf<Assembly, IList<Diagnostic>>>(Assembly.Load(peStream.ToArray()));
            }
        }

        return Task.FromResult<OneOf<Assembly, IList<Diagnostic>>>(errorDiagnostics);
    }

    private async Task<OneOf<string, None>> RunSubmission(string code)
    {
        //Output += $@"<br /><span class=""info"">{HttpUtility.HtmlEncode(code)}</span>";

        var result = await TryCompile(code);
        return await result.Match<Task<OneOf<string, None>>>(
            async assembly => {
                string output = string.Empty;
                //var writer = new StringWriter();

                //var entryPoint = _previousCompilation.GetEntryPoint(CancellationToken.None);
                //var type = assembly.GetType($"{entryPoint?.ContainingNamespace.MetadataName}.{entryPoint?.ContainingType.MetadataName}");
                //var entryPointMethod = type?.GetMethod(entryPoint.MetadataName);

                //var submission = (Func<object[], Task>)entryPointMethod.CreateDelegate(typeof(Func<object[], Task>));

                //if (_submissionIndex >= _submissionStates.Length)
                //{
                //    Array.Resize(ref _submissionStates, Math.Max(_submissionIndex, _submissionStates.Length * 2));
                //}

                //var returnValue = await ((Task<object>)submission(_submissionStates));
                //if (returnValue != null)
                //{
                //    Console.WriteLine(CSharpObjectFormatter.Instance.FormatObject(returnValue));
                //}

                //var output = HttpUtility.HtmlEncode(writer.ToString());
                //if (!string.IsNullOrWhiteSpace(output))
                //{
                //    Output += $"<br />{output}";
                //}

                return await Task.FromResult(output);
            },
            diagnostics => {
                //foreach (var diag in errorDiagnostics)
                //{
                //    Output += $@"<br / ><span class=""error"">{HttpUtility.HtmlEncode(diag)}</span>";
                //}

                return Task.FromResult<OneOf<string, None>>(new None());
            }
        );
    }


    private void TryLoadReferences()
    {
        if (_references is not null) return;

        var refs = AppDomain.CurrentDomain.GetAssemblies();
        var references = new List<MetadataReference>();

        foreach (var reference in refs.Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)))
        {
            references.Add(MetadataReference.CreateFromFile(reference.Location));
        }

        _references = references;
    }

}

