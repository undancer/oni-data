using System;
using ImGuiNET;

public struct KImGui_ScopedIndent : IDisposable
{
	private float indent_amount;

	public KImGui_ScopedIndent(float indent_amount = 0f)
	{
		this.indent_amount = indent_amount;
		ImGui.Indent(indent_amount);
	}

	public void Dispose()
	{
		ImGui.Unindent(indent_amount);
	}
}
