using AngleSharp.Dom;
using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Output;

namespace BlazorInteractive.Tests.Components.Output;

public class OutputComponentTests : TestContext
{
    public OutputComponentTests()
    {
    }

    [Fact]
    public void GetContentFromOutput()
    {
        string content = "Hello, World!";
        IRenderedComponent<OutputComponent> cut = RenderComponent<OutputComponent>(parameters => parameters.Add(p => p.Content, content));

        IElement element = cut.Find("#Output");
        element.Should().NotBeNull();
        element.InnerHtml.Equals(content);
    }
}