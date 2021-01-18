using System.Collections.Generic;
using UnityEngine;

public class SandboxRadsTool : BrushTool
{
	public static SandboxRadsTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
		viewMode = OverlayModes.Radiation.ID;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.radiationAdditiveSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.radiationAdditiveSlider.SetValue(5f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int recentlyAffectedCell in recentlyAffectedCells)
		{
			colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, recentlyAffectedCellColor));
		}
		foreach (int item in cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
	}

	private static void OnSimConsumeRadiationCallback(Sim.ConsumedRadiationCallback radiationCBInfo, object data)
	{
		((SandboxRadsTool)data).recentlyAffectedCells.Remove(radiationCBInfo.gameCell);
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		if (!recentlyAffectedCells.Contains(cell))
		{
			recentlyAffectedCells.Add(cell);
			float num = Grid.Radiation[cell];
			float num2 = num;
			num += SandboxToolParameterMenu.instance.settings.GetFloatSetting("SandbosTools.RadiationAdditive");
			num = Mathf.Clamp(num, 0f, 8999999f);
			float radiationDelta = num - num2;
			SimMessages.ModifyRadiationOnCell(cell, radiationDelta, Game.Instance.radiationConsumedCallbackManager.Add(OnSimConsumeRadiationCallback, this, "SandboxRadTools").index);
		}
	}
}
