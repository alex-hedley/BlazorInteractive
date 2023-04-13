using AngleSharp.Dom;
using Bunit;
using FluentAssertions;

using BlazorInteractive.Components;

namespace BlazorInteractive.Tests.Components;

public class EditorComponentTests : TestContext
{
    public EditorComponentTests()
    {
        JSInterop.SetupVoid("blazorMonaco.editor.create", _ => true);
    }

    [Fact]
    public void Test1()
    {
        IRenderedComponent<EditorComponent> cut = RenderComponent<EditorComponent>();
        IElement element = cut.Find("#editor");

        element.Should().NotBeNull();
    }
}