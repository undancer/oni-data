using System;
using ImGuiNET;

public struct KImGui_ScopedTreeNode : IDisposable
{
	private bool do_pop;

	public KImGui_ScopedTreeNode(string label)
	{
		do_pop = ImGui.TreeNode(label);
	}

	public void Dispose()
	{
		if (do_pop)
		{
			ImGui.TreePop();
		}
	}

	public static implicit operator bool(KImGui_ScopedTreeNode n)
	{
		return n.do_pop;
	}

	public static bool operator ==(KImGui_ScopedTreeNode node, bool value)
	{
		return node.do_pop == value;
	}

	public static bool operator !=(KImGui_ScopedTreeNode node, bool value)
	{
		return node.do_pop != value;
	}

	public override bool Equals(object obj)
	{
		KImGui_ScopedTreeNode kImGui_ScopedTreeNode = (KImGui_ScopedTreeNode)obj;
		return this == kImGui_ScopedTreeNode;
	}

	public override int GetHashCode()
	{
		return do_pop.GetHashCode();
	}
}
