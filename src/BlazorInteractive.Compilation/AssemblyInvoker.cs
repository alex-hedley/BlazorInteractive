using System.Reflection;

namespace BlazorInteractive.Compilation;

public class AssemblyInvoker : IAssemblyInvoker
{
    public string Invoke(Assembly assembly, string typeName, string methodName, params object[] args)
    {
        var type = assembly.GetType(typeName);

        var obj = Activator.CreateInstance(type);

        return (string)type.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, args);
    }
}