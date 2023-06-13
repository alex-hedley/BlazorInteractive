using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class EditorComponentTests : UnitTestBase
{
    private const string _editorDivId = "#editor";
    private const string _monacoCreateFunction = "blazorMonaco.editor.create";
    private const string _monacoGetValueFunction = "blazorMonaco.editor.getValue";
    
    public EditorComponentTests()
    {
        JSInterop.SetupVoid(_monacoCreateFunction, _ => true);
    }

    //[Fact]
    public void GetContentFromEditor()
    {
        JSInterop.Setup<string>(_monacoGetValueFunction, _ => true);

        var cut = RenderComponent<EditorComponent>();
        var element = cut.Find(_editorDivId);

        var button = cut.Find("button");

        button.Click();

        JSInterop.VerifyInvoke(_monacoGetValueFunction, calledTimes: 1);

        element.Should().NotBeNull();
    }

    // TODO: Investigate how to stub or fully render the StandaloneCodeEditor, so we may change the themes.
    // [Fact]
    // public void CheckThemes()
    // {
    //     IRenderedComponent<EditorComponent> cut = RenderComponent<EditorComponent>();
    //     
    //     IElement element = cut.Find(".vs");
    //     element.Should().NotBeNull();
    //     
    //     cut.Find("#theme").Change("vs-dark"); // Visual Studio Dark
    //     element = cut.Find(".vs-dark");
    //     element.Should().NotBeNull();
    //     
    //     cut.Find("#theme").Change("hc-black"); // High Contrast Black
    //     element = cut.Find(".hc-black");
    //     element.Should().NotBeNull();
    // }
}