using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public class ProfilerBase
{
	protected struct ThreadInfo
	{
		public Stack<string> regionStack;

		public StringBuilder sb;

		public int id;

		public string name;

		public ThreadInfo(int id)
		{
			regionStack = new Stack<string>();
			sb = new StringBuilder();
			this.id = id;
			name = string.Empty;
		}

		public void Reset()
		{
			regionStack.Clear();
			sb.Length = 0;
		}

		public void WriteLine(string category, string region_name, Stopwatch sw, string ph, string suffix)
		{
			ProfilerBase.WriteLine(sb, category, region_name, id, sw, ph, suffix);
		}

		public void StartLine(string category, string region_name, Stopwatch sw, string ph)
		{
			ProfilerBase.StartLine(sb, category, region_name, id, sw, ph);
		}
	}

	private bool initialised;

	private int idx;

	protected StreamWriter proFile;

	private string category = "GAME";

	private string filePrefix;

	protected Dictionary<int, ThreadInfo> threadInfos;

	public Stopwatch sw;

	public static void StartLine(StringBuilder sb, string category, string region_name, int tid, Stopwatch sw, string ph)
	{
		sb.Append("{\"cat\":\"").Append(category).Append("\"");
		sb.Append(",\"name\":\"").Append(region_name).Append("\"");
		sb.Append(",\"pid\":0");
		sb.Append(",\"tid\":").Append(tid);
		long elapsedTicks = sw.ElapsedTicks;
		long frequency = Stopwatch.Frequency;
		long value = elapsedTicks * 1000000 / frequency;
		sb.Append(",\"ts\":").Append(value);
		sb.Append(",\"ph\":\"").Append(ph).Append("\"");
	}

	public static void WriteLine(StringBuilder sb, string category, string region_name, int tid, Stopwatch sw, string ph, string suffix)
	{
		StartLine(sb, category, region_name, tid, sw, ph);
		sb.Append(suffix).Append("\n");
	}

	protected bool IsRecording()
	{
		return proFile != null;
	}

	public ProfilerBase(string file_prefix)
	{
		filePrefix = file_prefix;
		threadInfos = new Dictionary<int, ThreadInfo>();
		sw = new Stopwatch();
	}

	public void Init()
	{
		proFile = null;
	}

	public void Finalise()
	{
		if (IsRecording())
		{
			StopRecording();
		}
	}

	public void ToggleRecording(string category = "GAME")
	{
		this.category = "G";
		if (!initialised)
		{
			initialised = true;
			Init();
		}
		if (IsRecording())
		{
			StopRecording();
		}
		else
		{
			StartRecording();
		}
	}

	public virtual void StartRecording()
	{
		foreach (KeyValuePair<int, ThreadInfo> threadInfo in threadInfos)
		{
			threadInfo.Value.Reset();
		}
		proFile = new StreamWriter(filePrefix + idx + ".json");
		idx++;
		if (proFile != null)
		{
			proFile.WriteLine("{\"traceEvents\":[");
		}
		sw.Start();
	}

	public virtual void StopRecording()
	{
		sw.Stop();
		if (proFile == null)
		{
			return;
		}
		foreach (KeyValuePair<int, ThreadInfo> threadInfo2 in threadInfos)
		{
			proFile.Write(threadInfo2.Value.sb.ToString());
			threadInfo2.Value.Reset();
		}
		ThreadInfo threadInfo = ManifestThreadInfo("Main");
		threadInfo.WriteLine(category, "end", sw, "B", "},");
		threadInfo.WriteLine(category, "end", sw, "E", "}]}");
		proFile.Write(threadInfo.sb.ToString());
		threadInfo.Reset();
		proFile.Close();
		proFile = null;
	}

	public virtual void BeginThreadProfiling(string threadGroupName, string threadName)
	{
		ManifestThreadInfo(threadName);
	}

	public virtual void EndThreadProfiling()
	{
		if (proFile != null)
		{
			proFile.Write(ManifestThreadInfo().sb.ToString());
		}
		lock (threadInfos)
		{
			threadInfos.Remove(Thread.CurrentThread.ManagedThreadId);
		}
	}

	protected ThreadInfo ManifestThreadInfo(string name = null)
	{
		if (!threadInfos.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var value))
		{
			value = new ThreadInfo(Thread.CurrentThread.ManagedThreadId);
			if (name != null)
			{
				value.name = name;
			}
			Debug.LogFormat("ManifestThreadInfo: {0}, {1}", name, Thread.CurrentThread.ManagedThreadId);
			lock (threadInfos)
			{
				threadInfos.Add(Thread.CurrentThread.ManagedThreadId, value);
			}
		}
		if (name != null && value.name != name)
		{
			Debug.LogFormat("ManifestThreadInfo: change name {0} to {1}, {2}", name, value.name, Thread.CurrentThread.ManagedThreadId);
			value.name = name;
			lock (threadInfos)
			{
				threadInfos[value.id] = value;
				return value;
			}
		}
		return value;
	}

	[Conditional("KPROFILER_VALIDATE_REGION_NAME")]
	private void ValidateRegionName(string region_name)
	{
		DebugUtil.Assert(!region_name.Contains("\""));
		region_name = "InvalidRegionName";
	}

	protected void Push(string region_name, string file, uint line)
	{
		if (IsRecording())
		{
			ThreadInfo threadInfo = ManifestThreadInfo();
			threadInfo.regionStack.Push(region_name);
			threadInfo.WriteLine(category, region_name, sw, "B", "},");
		}
	}

	protected void Pop()
	{
		if (IsRecording())
		{
			ThreadInfo threadInfo = ManifestThreadInfo();
			if (threadInfo.regionStack.Count != 0)
			{
				threadInfo.WriteLine(category, threadInfo.regionStack.Pop(), sw, "E", "},");
			}
		}
	}
}
