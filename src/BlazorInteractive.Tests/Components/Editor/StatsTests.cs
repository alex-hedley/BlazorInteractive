using Bunit;
using FluentAssertions;

using BlazorInteractive.Components.Editor;

namespace BlazorInteractive.Tests.Components.Editor;

public class StatsTests : TestContext
{
    public StatsTests() {}

    [Fact]
    public void Stats_RendersNothing_WhenStatsIsNull()
    {
        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, null)
        );

        cut.Markup.Should().BeEmpty();
    }

    [Fact]
    public void Stats_RendersTable_WhenStatsIsProvided()
    {
        var stats = new ExecutionStats
        {
            LastRun = new DateTime(2024, 1, 1, 10, 16, 27),
            CompileTime = TimeSpan.FromSeconds(0.156),
            ExecuteTime = TimeSpan.FromSeconds(0.016),
            Memory = 0,
            CpuTime = TimeSpan.FromSeconds(0.016)
        };

        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, stats)
        );

        cut.Find("table").Should().NotBeNull();
    }

    [Fact]
    public void Stats_DisplaysCompileTime()
    {
        var stats = new ExecutionStats
        {
            LastRun = DateTime.Now,
            CompileTime = TimeSpan.FromSeconds(0.156),
            ExecuteTime = TimeSpan.FromSeconds(0.016),
            Memory = 0,
            CpuTime = TimeSpan.FromSeconds(0.016)
        };

        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, stats)
        );

        cut.Find("table").TextContent.Should().Contain("0.156s");
    }

    [Fact]
    public void Stats_DisplaysMemoryInBytes_WhenLessThan1KB()
    {
        var stats = new ExecutionStats
        {
            LastRun = DateTime.Now,
            Memory = 512
        };

        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, stats)
        );

        cut.Find("table").TextContent.Should().Contain("512b");
    }

    [Fact]
    public void Stats_DisplaysMemoryInKB_WhenBetween1KBAnd1MB()
    {
        var stats = new ExecutionStats
        {
            LastRun = DateTime.Now,
            Memory = 2048
        };

        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, stats)
        );

        cut.Find("table").TextContent.Should().Contain("KB");
    }

    [Fact]
    public void Stats_DisplaysMemoryInMB_WhenGreaterThan1MB()
    {
        var stats = new ExecutionStats
        {
            LastRun = DateTime.Now,
            Memory = 2 * 1024 * 1024
        };

        var cut = RenderComponent<Stats>(parameters => parameters
            .Add(p => p.ExecutionStats, stats)
        );

        cut.Find("table").TextContent.Should().Contain("MB");
    }
}