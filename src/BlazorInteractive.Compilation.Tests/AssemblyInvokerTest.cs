using System.Reflection;
using Dumpify;

namespace BlazorInteractive.Compilation.Tests;

public class AssemblyInvokerTest
{
    private readonly AssemblyInvoker _invoker;
    private readonly Assembly _testAssembly;

    public AssemblyInvokerTest()
    {
        _invoker = new AssemblyInvoker();
        _testAssembly = Assembly.GetExecutingAssembly();
    }

    [Fact]
    public void Invoke_WithConsoleWriteOutput_CapturesResult()
    {
        var result = _invoker.Invoke(_testAssembly, typeof(AssemblyInvokerTestHelper).FullName!, nameof(AssemblyInvokerTestHelper.WriteOutput));
        result.Should().NotBeNull();
        result.Should().Contain("Hello Invoker Test");
    }

    [Fact]
    public void Invoke_WithDumpifyOutput_CapturesResult()
    {
        var result = _invoker.Invoke(_testAssembly, typeof(AssemblyInvokerTestHelper).FullName!, nameof(AssemblyInvokerTestHelper.DumpOutput));
        result.Should().NotBeNull();
        result.Should().Contain("TestValue");
    }

    [Fact]
    public void Invoke_WithNoOutput_ReturnsEmpty()
    {
        var result = _invoker.Invoke(_testAssembly, typeof(AssemblyInvokerTestHelper).FullName!, nameof(AssemblyInvokerTestHelper.NoOutput));
        result.Should().BeEmpty();
    }

    [Fact]
    public void Invoke_RestoresConsoleOut_AfterInvocation()
    {
        var originalOut = Console.Out;

        _invoker.Invoke(_testAssembly, typeof(AssemblyInvokerTestHelper).FullName!, nameof(AssemblyInvokerTestHelper.WriteOutput));

        Console.Out.Should().BeSameAs(originalOut);
    }
}

public class AssemblyInvokerTestHelper
{
    public void WriteOutput()
    {
        Console.Write("Hello Invoker Test");
    }

    public void DumpOutput()
    {
        new { Property = "TestValue" }.Dump();
    }

    public void NoOutput()
    {
    }
}
