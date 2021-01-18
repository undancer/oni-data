using System.Diagnostics;

public class KProfilerEndpoint
{
	[Conditional("ENABLE_KPROFILER")]
	public virtual void Begin(string name, string group)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void End(string name)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void BeginFrame()
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void Ping(string display, string group, double value)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void BeginAsync(string display, string group)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void EndAsync(string display)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public virtual void EndFrame()
	{
	}
}
