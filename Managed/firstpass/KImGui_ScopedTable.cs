using System;
using ImGuiNET;

public struct KImGui_ScopedTable : IDisposable
{
	private bool do_pop;

	public KImGui_ScopedTable(string label, int num_columns, ImGuiTableFlags flags = ImGuiTableFlags.None)
	{
		if (flags == ImGuiTableFlags.None)
		{
			do_pop = ImGui.BeginTable(label, num_columns);
		}
		else
		{
			do_pop = ImGui.BeginTable(label, num_columns, flags);
		}
	}

	public void Dispose()
	{
		if (do_pop)
		{
			ImGui.EndTable();
		}
	}

	public static implicit operator bool(KImGui_ScopedTable n)
	{
		return n.do_pop;
	}

	public static bool operator ==(KImGui_ScopedTable node, bool value)
	{
		return node.do_pop == value;
	}

	public static bool operator !=(KImGui_ScopedTable node, bool value)
	{
		return node.do_pop != value;
	}

	public override bool Equals(object obj)
	{
		KImGui_ScopedTable kImGui_ScopedTable = (KImGui_ScopedTable)obj;
		return this == kImGui_ScopedTable;
	}

	public override int GetHashCode()
	{
		return do_pop.GetHashCode();
	}
}
