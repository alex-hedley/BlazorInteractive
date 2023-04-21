using AngleSharp.Dom;
using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class EditorComponentTests : TestContext
{
    public EditorComponentTests()
    {
        JSInterop.SetupVoid("blazorMonaco.editor.create", _ => true);
    }

    [Fact]
    public void GetContentFromEditor()
    {
        JSInterop.Setup<String>("blazorMonaco.editor.getValue", _ => true);

        IRenderedComponent<EditorComponent> cut = RenderComponent<EditorComponent>();
        IElement element = cut.Find("#editor");

        IElement button = cut.Find("button");

        button.Click();

        JSInterop.VerifyInvoke("blazorMonaco.editor.getValue", calledTimes: 1);

        element.Should().NotBeNull();
    }
}