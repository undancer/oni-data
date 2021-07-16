using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public static class KProfiler
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Region : IDisposable
	{
		public Region(string region_name, UnityEngine.Object profiler_obj = null)
		{
		}

		public void Dispose()
		{
		}
	}

	private static bool enabled = false;

	public static int counter = 0;

	public static Thread main_thread;

	public static KProfilerEndpoint AppEndpoint = new KProfilerPluginEndpoint();

	public static KProfilerEndpoint UnityEndpoint = new KProfilerEndpoint();

	public static KProfilerEndpoint ChromeEndpoint = new KProfilerEndpoint();

	private static string pattern = "<link=\"(.+)\">(.+)<\\/link>";

	private static Regex re = new Regex(pattern);

	public static bool IsEnabled()
	{
		return enabled;
	}

	public static void Enable()
	{
		enabled = true;
	}

	public static void Disable()
	{
	}

	public static void BeginThread(string name, string group)
	{
	}

	public static void BeginFrame()
	{
	}

	public static void EndFrame()
	{
	}

	public static int BeginSampleI(string region_name, string group = "Game")
	{
		int result = counter;
		counter++;
		return result;
	}

	public static int EndSampleI(string region_name = null)
	{
		counter--;
		return counter;
	}

	public static string SanitizeName(string name)
	{
		return re.Replace(name, "${1}");
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void Ping(string display, string group, double value)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginAsync(string display, string group = "Game")
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndAsync(string display)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, string group = "Game")
	{
		BeginSampleI(region_name, group);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSample(string region_name = null)
	{
		EndSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSample(string region_name, int count)
	{
		EndSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, int count)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, string group, int count)
	{
		BeginSampleI(region_name, group);
	}

	public static int BeginSampleI(string region_name, UnityEngine.Object profiler_obj)
	{
		int result = counter;
		counter++;
		return result;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, UnityEngine.Object profiler_obj)
	{
		BeginSampleI(region_name, profiler_obj);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void AddEvent(string event_name)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void AddCounter(string event_name, List<KeyValuePair<string, int>> series_name_counts)
	{
		foreach (KeyValuePair<string, int> series_name_count in series_name_counts)
		{
			_ = series_name_count;
			EndSampleI();
		}
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void AddCounter(string event_name, string series_name, int count)
	{
		EndSampleI(series_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void AddCounter(string event_name, int count)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginThreadProfiling(string threadGroupName, string threadName)
	{
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndThreadProfiling()
	{
	}
}
