using BlazorInteractive.DependencyManagement;
using Microsoft.Extensions.Logging;
using OneOf.Types;

namespace BlazorInteractive.ConsoleApp;

class Program
{
    private static readonly ILogger _logger;
    
    async static Task Main(string[] args)
    {
        var nugetPackageGetter = new NuGetPackageGetter(_logger);
        var package = await nugetPackageGetter.GetPackage("ironpython");
        package.Switch(
            success =>
            {
                Console.WriteLine("Success");
            },
            failure =>
            {
                Console.WriteLine("Failure");
            }
        );
    }
}