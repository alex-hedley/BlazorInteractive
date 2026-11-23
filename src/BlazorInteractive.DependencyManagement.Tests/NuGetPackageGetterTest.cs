using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf.Types;

namespace BlazorInteractive.DependencyManagement.Tests;

public class NuGetPackageGetterTest
{
    private readonly Mock<ILogger<NuGetPackageGetter>> _logger;
    
    public NuGetPackageGetterTest()
    {
        _logger = new Mock<ILogger<NuGetPackageGetter>>();
    }

    [Fact]
    public async Task GetPackage_WithNullOrEmpty_ReturnsFailure()
    {
        var nugetPackageGetter = new NuGetPackageGetter(_logger.Object);
        
        var result = await nugetPackageGetter.GetPackage(string.Empty);
        result.Value.Should().BeOfType<Failure>();
    }
    
    [Fact]
    public async Task GetPackage_WithNullOrEmptyName_HasErrorMessage()
    {
        var nugetPackageGetter = new NuGetPackageGetter(_logger.Object);
        
        var result = await nugetPackageGetter.GetPackage(string.Empty);
        result.Value.As<Failure>().errorMessage.Should().NotBeNullOrEmpty();
    }
}