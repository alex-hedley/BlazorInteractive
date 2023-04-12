using BlazorInteractive.Components;
using Bunit;
using FluentAssertions;

namespace BlazorInteractive.Tests.Components;

public class EditorComponentTests : TestContext
{
    [Fact]
    public void Test1()
    {
        IRenderedComponent<EditorComponent> cut = RenderComponent<EditorComponent>();
        cut.Find("div.monaco-editor");

        cut.Should().NotBeNull();
    }
}