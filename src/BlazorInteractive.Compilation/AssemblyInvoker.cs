using System.Reflection;

namespace BlazorInteractive.Compilation;

public class AssemblyInvoker : IAssemblyInvoker
{
    public string? Invoke(Assembly assembly, string typeName, string methodName, params object[] args)
    {
        var defaultWriter = Console.Out;
        try {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            
            var type = assembly.GetType(typeName);

            if (type == null) return null;
            var obj = Activator.CreateInstance(type);

            _ = type.InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, args)!;
            
            var result = writer.ToString();
            return result;
        }
        finally
        {
            Console.SetOut(defaultWriter);
        }
    }
}