using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class CompilerVersionTests : TestContext
{
    public CompilerVersionTests() {}

    // [Fact]
    public void Test1()
    {
        var cut = RenderComponent<CompilerVersion>(parameters => parameters
            .Add(p => p.OnCompilerChanged, null)
        );
        
        // cut.FindAll("button")[1].Click();
        
        var compilerVersion = cut.Instance;
        Assert.Equal(compilerVersion.OnCompilerChanged, null);
    }

    // [Fact]
    public void CheckMarkup()
    {
        var cut = RenderComponent<CompilerVersion>(parameters => parameters
            .Add(p => p.OnCompilerChanged, null)
        );

        var markupSelect = "";
        cut.FindAll("select")[0].MarkupMatches(markupSelect);
    }
}