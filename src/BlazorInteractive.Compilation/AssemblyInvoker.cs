using System.Reflection;

namespace BlazorInteractive.Compilation;

public class AssemblyInvoker : IAssemblyInvoker
{
    public async Task<string?> InvokeAsync(Assembly assembly)
    {
        var defaultWriter = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Script compilations generate a static <Factory> entry point on the first type
            // (class Submission#0).  The factory takes an object[] of previous submission
            // states; pass a single-element array (null) for a fresh first submission.
            var scriptType = assembly.GetTypes().FirstOrDefault();
            if (scriptType is null)
            {
                return writer.ToString();
            }

            var factory = scriptType.GetMethod(
                "<Factory>",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (factory != null)
            {
                var task = factory.Invoke(null, new object[] { new object?[1] }) as Task;
                if (task != null)
                {
                    await task;
                }
            }

            return writer.ToString();
        }
        finally
        {
            Console.SetOut(defaultWriter);
        }
    }
}