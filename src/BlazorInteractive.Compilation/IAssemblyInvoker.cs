using System.Reflection;

namespace BlazorInteractive.Compilation;

public interface IAssemblyInvoker
{
    Task<string?> InvokeAsync(Assembly assembly);
}