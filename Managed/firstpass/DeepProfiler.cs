using System.Diagnostics;

public class DeepProfiler
{
	private bool enableProfiling;

	public DeepProfiler(bool enable_profiling)
	{
		enableProfiling = enable_profiling;
	}

	[Conditional("DEEP_PROFILE")]
	public void BeginSample(string message)
	{
		_ = enableProfiling;
	}

	[Conditional("DEEP_PROFILE")]
	public void EndSample()
	{
		_ = enableProfiling;
	}
}
