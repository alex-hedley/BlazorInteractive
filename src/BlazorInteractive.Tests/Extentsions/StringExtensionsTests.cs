using FluentAssertions;

using BlazorInteractive.Extensions;

namespace BlazorInteractive.Tests.Extensions;

public class StringExtensionsTests : UnitTestBase
{
    public StringExtensionsTests() {}

    [Fact]
    public void TrimEnd()
    {
        var ext = ".dll";
        var assembly = "System.dll";

        var expectedResult = "System";
        var result = assembly.TrimEnd(ext);

        result.Should().Be(expectedResult);
    }
}