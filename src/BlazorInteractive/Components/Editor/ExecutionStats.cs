namespace BlazorInteractive.Components.Editor;

public class ExecutionStats
{
    public DateTime? LastRun { get; set; }
    public TimeSpan CompileTime { get; set; }
    public TimeSpan ExecuteTime { get; set; }
    public long Memory { get; set; }
    public TimeSpan CpuTime { get; set; }
}
