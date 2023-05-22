namespace BlazorInteractive.Compilation;

public class AppDomainAssemblyException : Exception
{
    public AppDomainAssemblyException()
    {
    }

    public AppDomainAssemblyException(string message)
        : base(message)
    {
    }

    public AppDomainAssemblyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}