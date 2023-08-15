using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class CheckboxListTests : TestContext
{
    public CheckboxListTests() {}

    // [Fact]
    // public void Test1()
    // {
    //     var cut = RenderComponent<CheckboxList>(parameters => parameters
    //         .Add(p => p.Data, null)
    //         .Add(p => p.TextField, null)
    //         .Add(p => p.ValueField, null)
    //         .Add(p => p.Style, null)
    //         .Add(p => p.ChildContent, null)
    //         .Add(p => p.SelectedValues, null)
    //         .Add(p => p.DisabledValues, null)
    //     );
    //     
    //     // cut.FindAll("button")[1].Click();
    //     
    //     var checkboxList = cut.Instance;
    //     Assert.Equal(checkboxList.SelectedValues, null);
    // }

    // [Fact]
    // public void CheckMarkup()
    // {
    //     var cut = RenderComponent<CheckboxList>(parameters => parameters
    //         .Add(p => p.Data, null)
    //     );
    //
    //     var markupCheckboxList = "";
    //     cut.FindAll("input")[0].MarkupMatches(markupCheckboxList);
    // }
}