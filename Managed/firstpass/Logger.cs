using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;

public abstract class Logger
{
	public static uint NextIdx;

	protected string name;

	public bool enableConsoleLogging { get; set; }

	public bool breakOnLog { get; set; }

	public abstract int Count { get; }

	public Logger(string name)
	{
		this.name = name;
	}

	public string GetName()
	{
		return name;
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public virtual void DebugDisplayLog()
	{
	}

	public virtual void DebugDevTool()
	{
	}
}
public class Logger<EntryType> : Logger
{
	private List<EntryType> entries;

	public Action<EntryType> OnLog;

	public override int Count
	{
		get
		{
			if (entries == null)
			{
				return 0;
			}
			return entries.Count;
		}
	}

	public IEnumerator<EntryType> GetEnumerator()
	{
		if (entries == null)
		{
			entries = new List<EntryType>();
		}
		return entries.GetEnumerator();
	}

	public void SetMaxEntries(int new_max)
	{
	}

	public Logger(string name, int new_max = 35)
		: base(name)
	{
		SetMaxEntries(new_max);
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
	}

	public override void DebugDevTool()
	{
		bool v = base.enableConsoleLogging;
		if (ImGui.Checkbox("Console Logging:", ref v))
		{
			base.enableConsoleLogging = v;
		}
		v = base.breakOnLog;
		if (ImGui.Checkbox("Break On Log:", ref v))
		{
			base.breakOnLog = v;
		}
		ImGui.Text(name + " Log:");
		if (ImGui.Button("Clear"))
		{
			entries.Clear();
		}
		if (entries == null)
		{
			return;
		}
		ImGui.Indent();
		foreach (EntryType entry in entries)
		{
			ImGui.Text(entry.ToString());
		}
		ImGui.Unindent();
	}
}
