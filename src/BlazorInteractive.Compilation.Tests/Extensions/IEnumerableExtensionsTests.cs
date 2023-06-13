using BlazorInteractive.Compilation.Extensions;

namespace BlazorInteractive.Compilation.Tests.Extensions;

public class IEnumerableExtensionsTests
{
    public IEnumerableExtensionsTests() {}

    [Fact]
    public void Join()
    {
        var strings = new[] { "a", "b"};
        var separator = ",";

        var expectedResult = "a,b";
        var result = strings.Join(separator);

        result.Should().Be(expectedResult);
    }
}