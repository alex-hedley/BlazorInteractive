using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;

public class CodeCompiler : ICompiler
{
    public async Task<string> Compile(string sourceCode, IEnumerable<string> imports)
    {
        // See https://aka.ms/new-console-template for more information
        //Console.WriteLine("Hello, World!");

        // cmd /c c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /reference:/out:TEST.exe /win32icon:favicon.ico TEST.cs | %no_obs% | %no_pol% | %no_prv%

        Assembly assembly;
        // string source = "Console.WriteLine(\"Hello, World!\");";

        // var tree = CSharpSyntaxTree.ParseText (@"class Program 
        // {
        //   static void Main() => System.Console.WriteLine(""Hello"");
        // }");

        int _submissionIndex = 0;
        object[] _submissionStates = new object[] { null, null };

        string randomAssemblyName = Path.GetRandomFileName();

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            sourceCode,
            CSharpParseOptions.Default.WithKind(SourceCodeKind.Regular).WithLanguageVersion(LanguageVersion.Default)
        );

        IEnumerable<SyntaxTree> syntaxTrees = new List<SyntaxTree>() { syntaxTree };

        // string[] defaultNamespaces = new[] {
        //     "System",
        //     "System.IO",
        //     "System.Collections.Generic",
        //     "System.Console",
        //     "System.Diagnostics",
        //     "System.Dynamic",
        //     "System.Linq",
        //     "System.Linq.Expressions",
        //     "System.Net.Http",
        //     "System.Text",
        //     "System.Threading.Tasks"
        // };

        // string[] defaultNamespaces = new[] {
        //     "System"
        // };

        CSharpCompilationOptions? options = new(
            outputKind: OutputKind.ConsoleApplication,
            usings: imports
        );
        // OutputKind.ConsoleApplication

        Assembly[] appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        //// Print Assemblies
        //foreach (var assembly in appDomainAssemblies)
        //{
        //    Console.WriteLine(assembly.FullName);
        //}

        IList<MetadataReference> metadataReferences = new List<MetadataReference>();

        foreach (Assembly? reference in appDomainAssemblies.Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)))
        {
            //Console.WriteLine(reference.FullName);
            PortableExecutableReference metadataReference = MetadataReference.CreateFromFile(reference.Location);
            metadataReferences.Add(metadataReference);
        }

        CSharpCompilation compilation = CSharpCompilation.Create(
            randomAssemblyName,
            syntaxTrees,
            metadataReferences,
            options);

        // compilation = CSharpCompilation
        //   .Create ("test")
        //   .WithOptions (new CSharpCompilationOptions (OutputKind.ConsoleApplication))
        //   .AddSyntaxTrees (tree)
        //   .AddReferences (references);

        var errorDiagnostics = compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error);
        // Print Error Diagnostics
        foreach (var diag in errorDiagnostics)
        {
            Console.WriteLine(diag);
        }

        using MemoryStream peStream = new();

        var emitResult = compilation.Emit(peStream);

        // if (emitResult.Success)
        // {
        //     _submissionIndex++;
        //     _previousCompilation = scriptCompilation;
        //     assembly = Assembly.Load(peStream.ToArray());
        // }

        if (emitResult.Success)
        {
            Console.WriteLine("Success");
            assembly = Assembly.Load(peStream.ToArray());
        }
        else
        {
            Console.WriteLine("Failure");
            return string.Empty;
        }

        // if (assembly is null) return;

        var fullname = assembly.FullName;
        Console.WriteLine(fullname);

        foreach(Type exportedType in assembly.GetExportedTypes())
        {
            string fullName = exportedType.FullName;
            string nameSpace = exportedType.Namespace;
            Console.WriteLine($"Fullname: {fullName} Namespace: {nameSpace}");
        }

        var entryPoint = compilation.GetEntryPoint(CancellationToken.None);
        Console.WriteLine(entryPoint);
        Console.WriteLine(entryPoint.ContainingNamespace.MetadataName);
        Console.WriteLine(entryPoint.ContainingType.MetadataName);

        var type = assembly.GetType($"{entryPoint.ContainingNamespace.MetadataName}.{entryPoint.ContainingType.MetadataName}");

        //var type = assembly.GetType("Test.Program"); // "Script+Program"
        Console.WriteLine(type);
        //var entryPointMethod = type.GetMethod(entryPoint.MetadataName); // <Factory>

        // var methods = type.GetMethods();
        //
        //var entryPointMethod = type.GetMethod("Main"); // "Void Main()"

        var entryPointMethod = type.GetRuntimeMethods().FirstOrDefault();

        Console.WriteLine(entryPointMethod);

        var instance = Activator.CreateInstance(type);

        var defaultWriter = Console.Out;
        
        using var writer = new StringWriter();
        Console.SetOut(writer);
        
        var output = entryPointMethod.Invoke(instance, default);
        
        var result = writer.ToString();
        
        Console.SetOut(defaultWriter);

        //var submission = (Func<object[], Task>)entryPointMethod.CreateDelegate(typeof(Func<object[], Task>));

        var submission = entryPointMethod.CreateDelegate<Func<int>>();

        //var returnValue = await ((Task<object>)submission(_submissionStates));
        var returnValue = submission();
        if (returnValue != null)
        {
            // Console.WriteLine(CSharpObjectFormatter.Instance.FormatObject(returnValue));
            //return CSharpObjectFormatter.Instance.FormatObject(returnValue);
        }

        // var output = HttpUtility.HtmlEncode(writer.ToString());
        // if (!string.IsNullOrWhiteSpace(output))
        // {
        //     Output += $"<br />{output}";
        // }

        // Console.ReadLine();
        return string.Empty;
    }

}