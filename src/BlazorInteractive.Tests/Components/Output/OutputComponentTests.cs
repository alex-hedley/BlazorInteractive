using AngleSharp.Dom;
using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Output;

namespace BlazorInteractive.Tests.Components.Output;

public class OutputComponentTests : UnitTestBase
{
    private const string _helloWorld = "Hello, World!";
    private const string _outputDivId = "#Output";
    
    [Fact]
    public void GetContentFromOutput()
    {
        const string expectedContent = _helloWorld;
        IRenderedComponent<OutputComponent> cut = RenderComponent<OutputComponent>(parameters => parameters.Add(p => p.Content, expectedContent));

        IElement element = cut.Find(_outputDivId);
        element.Should().NotBeNull();
        element.InnerHtml.Should().Be(expectedContent);
    }

    [Fact]
    public void SetContent()
    {
        const string expectedContent = _helloWorld;
        IRenderedComponent<OutputComponent> cut = RenderComponent<OutputComponent>(parameters => parameters.Add(p => p.Content, expectedContent));

        IElement element = cut.Find(_outputDivId);
        element.Should().NotBeNull();
        element.InnerHtml.Should().Be(expectedContent);
    }
}