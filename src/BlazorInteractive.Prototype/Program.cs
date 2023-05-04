using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

string sourceCode = "Console.WriteLine(\"Hello, World!\");";
IList<string> imports = new List<string>
{
    "System"
};

var sc = new ScriptCompiler();
var scriptOutput = sc.Compile(sourceCode, imports);
Console.WriteLine(scriptOutput);

sourceCode = @"class Program 
{
    static void Main() => System.Console.WriteLine(""Hello"");
}";

var cc = new CodeCompiler();
var codeOutput = cc.Compile(sourceCode, imports);
Console.WriteLine(scriptOutput);

Console.ReadLine();
