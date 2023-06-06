using System.Reflection;

namespace BlazorInteractive.Compilation;

public interface IAssemblyInvoker
{
    string? Invoke(Assembly assembly, string typeName, string methodName, params object[] args);
}