using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SandboxStressTool : BrushTool
{
	public static SandboxStressTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

	private Dictionary<MinionIdentity, AttributeModifier> moraleAdjustments = new Dictionary<MinionIdentity, AttributeModifier>();

	public override string[] DlcIDs => DlcManager.AVAILABLE_ALL_VERSIONS;

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
		SandboxToolParameterMenu.instance.stressAdditiveSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.stressAdditiveSlider.SetValue(5f);
		SandboxToolParameterMenu.instance.moraleSlider.SetValue(0f);
		if (DebugHandler.InstantBuildMode)
		{
			SandboxToolParameterMenu.instance.moraleSlider.row.SetActive(value: true);
		}
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
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			GameObject gameObject = Components.LiveMinionIdentities[i].gameObject;
			if (Grid.PosToCell(gameObject) != cell)
			{
				continue;
			}
			float num = -1f * SandboxToolParameterMenu.instance.settings.GetFloatSetting("SandbosTools.StressAdditive");
			Db.Get().Amounts.Stress.Lookup(Components.LiveMinionIdentities[i].gameObject).ApplyDelta(num);
			PopFXManager.Instance.SpawnFX((num >= 0f) ? Assets.GetSprite("crew_state_angry") : Assets.GetSprite("crew_state_happy"), GameUtil.GetFormattedPercent(num), gameObject.transform);
			int intSetting = SandboxToolParameterMenu.instance.settings.GetIntSetting("SandbosTools.MoraleAdjustment");
			AttributeInstance attributeInstance = gameObject.GetAttributes().Get(Db.Get().Attributes.QualityOfLife);
			MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
			if (moraleAdjustments.ContainsKey(component))
			{
				attributeInstance.Remove(moraleAdjustments[component]);
				moraleAdjustments.Remove(component);
			}
			if (intSetting != 0)
			{
				AttributeModifier attributeModifier = new AttributeModifier(attributeInstance.Id, intSetting, () => DUPLICANTS.MODIFIERS.SANDBOXMORALEADJUSTMENT.NAME);
				attributeModifier.SetValue(intSetting);
				attributeInstance.Add(attributeModifier);
				moraleAdjustments.Add(component, attributeModifier);
			}
		}
	}
}
