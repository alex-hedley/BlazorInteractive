using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class SampleTests : TestContext
{
    public SampleTests() {}

    // [Fact]
    // public void Test1()
    // {
    //     var cut = RenderComponent<Sample>(parameters => parameters
    //         .Add(p => p.OnSampleChanged, null)
    //     );
    //     
    //     // cut.FindAll("button")[1].Click();
    //     
    //     var sample = cut.Instance;
    //     Assert.Equal(sample.OnSampleChanged, null);
    // }

    // [Fact]
    // public void CheckMarkup()
    // {
    //     var cut = RenderComponent<Sample>(parameters => parameters
    //         .Add(p => p.OnSampleChanged, null)
    //     );
    //
    //     var markupSelect = "";
    //     cut.FindAll("select")[0].MarkupMatches(markupSelect);
    // }
}