using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf.Types;

namespace BlazorInteractive.DependencyManagement.Tests;

public class NuGetPackageGetterTest
{
    private readonly Mock<ILogger<NuGetPackageGetter>> _logger;
    // private readonly CancellationToken _defaultCancellationToken;
    
    public NuGetPackageGetterTest()
    {
        _logger = new Mock<ILogger<NuGetPackageGetter>>();
        // _defaultCancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task GetPackage_WithValidName_ReturnsSuccess()
    {
        var packageName = "Package";
        var nugetPackageGetter = new NuGetPackageGetter(_logger.Object);
        
        var result = await nugetPackageGetter.GetPackage(packageName);
        result.Value.Should().BeOfType<Success>();
    }
    
    [Fact]
    public async Task GetPackage_WithInvalidName_ReturnsFailure()
    {
        var packageName = "Package";
        var nugetPackageGetter = new NuGetPackageGetter(_logger.Object);
        
        var result = await nugetPackageGetter.GetPackage(packageName);
        result.Value.Should().BeOfType<Failure>();
    }
    
}