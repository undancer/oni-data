using System.Collections.Generic;
using UnityEngine;

public class SandboxHeatTool : BrushTool
{
	public static SandboxHeatTool instance;

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
		viewMode = OverlayModes.Temperature.ID;
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
		SandboxToolParameterMenu.instance.temperatureAdditiveSlider.row.SetActive(value: true);
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

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		if (!recentlyAffectedCells.Contains(cell))
		{
			recentlyAffectedCells.Add(cell);
			Game.CallbackInfo item = new Game.CallbackInfo(delegate
			{
				recentlyAffectedCells.Remove(cell);
			});
			int index = Game.Instance.callbackManager.Add(item).index;
			float num = Grid.Temperature[cell];
			num += SandboxToolParameterMenu.instance.settings.GetFloatSetting("SandbosTools.TemperatureAdditive");
			switch (GameUtil.temperatureUnit)
			{
			case GameUtil.TemperatureUnit.Celsius:
				num -= 273.15f;
				break;
			case GameUtil.TemperatureUnit.Fahrenheit:
				num -= 255.372f;
				break;
			}
			num = Mathf.Clamp(num, 1f, 9999f);
			int gameCell = cell;
			SimHashes id = Grid.Element[cell].id;
			CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
			float mass = Grid.Mass[cell];
			float temperature = num;
			int callbackIdx = index;
			SimMessages.ReplaceElement(gameCell, id, sandBoxTool, mass, temperature, Grid.DiseaseIdx[cell], Grid.DiseaseCount[cell], callbackIdx);
		}
	}
}
