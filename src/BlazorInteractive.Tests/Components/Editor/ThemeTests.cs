using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class ThemeTests : TestContext
{
    public ThemeTests() {}

    // [Fact]
    public void Test1()
    {
        var cut = RenderComponent<Theme>(parameters => parameters
            .Add(p => p.OnThemeChanged, null)
        );
        
        // cut.FindAll("button")[1].Click();
        
        var sample = cut.Instance;
        Assert.Equal(sample.OnThemeChanged, null);
    }

    // [Fact]
    public void CheckMarkup()
    {
        var cut = RenderComponent<Theme>(parameters => parameters
            .Add(p => p.OnThemeChanged, null)
        );

        var markupSelect = "";
        cut.FindAll("select")[0].MarkupMatches(markupSelect);
    }
}