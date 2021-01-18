using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SandboxSampleTool : InterfaceTool
{
	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		colors.Add(new ToolMenu.CellColorData(currentCell, radiusIndicatorColor));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		currentCell = Grid.PosToCell(cursorPos);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		int cell = Grid.PosToCell(cursor_pos);
		if (!Grid.IsValidCell(cell))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, cursor_pos, 1.5f, track_target: false, force_spawn: true);
		}
		else
		{
			Sample(cell);
		}
	}

	private void Sample(int cell)
	{
		UISounds.PlaySound(UISounds.Sound.ClickObject);
		SandboxToolParameterMenu.instance.settings.SetIntSetting("SandboxTools.SelectedElement", Grid.Element[cell].idx);
		SandboxToolParameterMenu.instance.settings.SetFloatSetting("SandboxTools.Mass", Mathf.Round(Grid.Mass[cell] * 100f) / 100f);
		SandboxToolParameterMenu.instance.settings.SetFloatSetting("SandbosTools.Temperature", Mathf.Round(Grid.Temperature[cell] * 10f) / 10f);
		SandboxToolParameterMenu.instance.settings.SetIntSetting("SandboxTools.DiseaseCount", Grid.DiseaseCount[cell]);
		SandboxToolParameterMenu.instance.RefreshDisplay();
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(value: true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}
}
