using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SandboxClearFloorTool : BrushTool
{
	public static SandboxClearFloorTool instance;

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
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
		SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(settings.GetIntSetting("SandboxTools.BrushSize"));
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
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
		bool flag = false;
		foreach (Pickupable pickup in Components.Pickupables.Items)
		{
			if (!(pickup.storage != null) && Grid.PosToCell(pickup) == cell && Components.LiveMinionIdentities.Items.Find((MinionIdentity match) => match.gameObject == pickup.gameObject) == null)
			{
				if (!flag)
				{
					UISounds.PlaySound(UISounds.Sound.Negative);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.SANDBOXTOOLS.CLEARFLOOR.DELETED, pickup.transform);
					flag = true;
				}
				Util.KDestroyGameObject(pickup.gameObject);
			}
		}
	}
}
