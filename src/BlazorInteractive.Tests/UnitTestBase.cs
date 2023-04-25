using Bunit;

namespace BlazorInteractive.Tests;

public class UnitTestBase : TestContext
{
    public Task InvokeAsync(Action workItem)
    {
        return Renderer.Dispatcher.InvokeAsync(workItem);
    }

    public Task InvokeAsync(Func<Task> workItem)
    {
        return Renderer.Dispatcher.InvokeAsync(workItem);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
    {
        return Renderer.Dispatcher.InvokeAsync(workItem);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
    {
        return Renderer.Dispatcher.InvokeAsync(workItem);
    }
}