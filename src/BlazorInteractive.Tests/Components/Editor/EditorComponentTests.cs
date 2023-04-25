using AngleSharp.Dom;
using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Contracts;

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

    [Fact]
    public void CheckThemes()
    {
        IRenderedComponent<EditorComponent> cut = RenderComponent<EditorComponent>();
        //IElement element = cut.Find("#editor");

        //cut.Find("#theme").Change("vs"); // Default - Visual Studio
        IElement element = cut.Find(".vs");
        element.Should().NotBeNull();

        cut.Find("#theme").Change("vs-dark"); // Visual Studio Dark
        element = cut.Find(".vs-dark");
        element.Should().NotBeNull();

        cut.Find("#theme").Change("hc-black"); // High Contrast Black
        element = cut.Find(".hc-black");
        element.Should().NotBeNull();
    }
}