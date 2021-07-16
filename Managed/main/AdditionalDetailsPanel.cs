using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
	public GameObject attributesLabelTemplate;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;

	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		detailsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		drawer = new DetailsPanelDrawer(attributesLabelTemplate, detailsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	private void Update()
	{
		Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		drawer.BeginDrawing();
		RefreshDetails();
		drawer.EndDrawing();
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private void RefreshDetails()
	{
		detailsPanel.SetActive(value: true);
		detailsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS;
		PrimaryElement component = selectedTarget.GetComponent<PrimaryElement>();
		CellSelectionObject component2 = selectedTarget.GetComponent<CellSelectionObject>();
		float mass;
		float temperature;
		Element element;
		byte diseaseIdx;
		int diseaseCount;
		if (component != null)
		{
			mass = component.Mass;
			temperature = component.Temperature;
			element = component.Element;
			diseaseIdx = component.DiseaseIdx;
			diseaseCount = component.DiseaseCount;
		}
		else
		{
			if (!(component2 != null))
			{
				return;
			}
			mass = component2.Mass;
			temperature = component2.temperature;
			element = component2.element;
			diseaseIdx = component2.diseaseIdx;
			diseaseCount = component2.diseaseCount;
		}
		bool flag = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
		float specificHeatCapacity = element.specificHeatCapacity;
		float highTemp = element.highTemp;
		float lowTemp = element.lowTemp;
		BuildingComplete component3 = selectedTarget.GetComponent<BuildingComplete>();
		float num = ((!(component3 != null)) ? (-1f) : component3.creationTime);
		LogicPorts component4 = selectedTarget.GetComponent<LogicPorts>();
		EnergyConsumer component5 = selectedTarget.GetComponent<EnergyConsumer>();
		Operational component6 = selectedTarget.GetComponent<Operational>();
		Battery component7 = selectedTarget.GetComponent<Battery>();
		drawer.NewLabel(drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name)).Tooltip(drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name)).NewLabel(drawer.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(mass)))
			.Tooltip(drawer.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(mass)));
		if (num > 0f)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.AGE.NAME, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num) / 600f))).Tooltip(drawer.Format(UI.ELEMENTAL.AGE.TOOLTIP, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num) / 600f)));
		}
		int num_cycles = 5;
		float num2;
		float num3;
		float num4;
		if (component6 != null && (component4 != null || component5 != null || component7 != null))
		{
			num2 = component6.GetCurrentCycleUptime();
			num3 = component6.GetLastCycleUptime();
			num4 = component6.GetUptimeOverCycles(num_cycles);
		}
		else
		{
			num2 = -1f;
			num3 = -1f;
			num4 = -1f;
		}
		if (num2 >= 0f)
		{
			string text = UI.ELEMENTAL.UPTIME.NAME;
			text = text.Replace("{0}", "    • ");
			text = text.Replace("{1}", UI.ELEMENTAL.UPTIME.THIS_CYCLE);
			text = text.Replace("{2}", GameUtil.GetFormattedPercent(num2 * 100f));
			text = text.Replace("{3}", UI.ELEMENTAL.UPTIME.LAST_CYCLE);
			text = text.Replace("{4}", GameUtil.GetFormattedPercent(num3 * 100f));
			text = text.Replace("{5}", UI.ELEMENTAL.UPTIME.LAST_X_CYCLES.Replace("{0}", num_cycles.ToString()));
			text = text.Replace("{6}", GameUtil.GetFormattedPercent(num4 * 100f));
			drawer.NewLabel(text);
		}
		if (!flag)
		{
			bool flag2 = false;
			float num5 = element.thermalConductivity;
			Building component8 = selectedTarget.GetComponent<Building>();
			if (component8 != null)
			{
				num5 *= component8.Def.ThermalConductivity;
				flag2 = component8.Def.ThermalConductivity < 1f;
			}
			string temperatureUnitSuffix = GameUtil.GetTemperatureUnitSuffix();
			float shc = specificHeatCapacity * 1f;
			string text2 = string.Format(UI.ELEMENTAL.SHC.NAME, GameUtil.GetDisplaySHC(shc).ToString("0.000"));
			string text3 = UI.ELEMENTAL.SHC.TOOLTIP;
			text3 = text3.Replace("{SPECIFIC_HEAT_CAPACITY}", text2 + GameUtil.GetSHCSuffix());
			text3 = text3.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			string text4 = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, GameUtil.GetDisplayThermalConductivity(num5).ToString("0.000"));
			string text5 = UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP;
			text5 = text5.Replace("{THERMAL_CONDUCTIVITY}", text4 + GameUtil.GetThermalConductivitySuffix());
			text5 = text5.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(temperature))).Tooltip(drawer.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(temperature))).NewLabel(drawer.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount)))
				.Tooltip(drawer.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount, color: true)))
				.NewLabel(text2)
				.Tooltip(text3)
				.NewLabel(text4)
				.Tooltip(text5);
			if (flag2)
			{
				drawer.NewLabel(UI.GAMEOBJECTEFFECTS.INSULATED.NAME).Tooltip(UI.GAMEOBJECTEFFECTS.INSULATED.TOOLTIP);
			}
		}
		if (element.IsSolid)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp))).Tooltip(drawer.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp)));
			if (selectedTarget.GetComponent<ElementChunk>() != null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					drawer.NewLabel(drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString())).Tooltip(drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString()));
				}
			}
		}
		else if (element.IsLiquid)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp))).Tooltip(drawer.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp))).NewLabel(drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp)))
				.Tooltip(drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp)));
		}
		else if (!flag)
		{
			drawer.NewLabel(drawer.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp))).Tooltip(drawer.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp)));
		}
		Attributes attributes = selectedTarget.GetAttributes();
		if (attributes != null)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				AttributeInstance attributeInstance = attributes.AttributeTable[i];
				if (attributeInstance.Attribute.ShowInUI == Attribute.Display.Details || attributeInstance.Attribute.ShowInUI == Attribute.Display.Expectation)
				{
					drawer.NewLabel(attributeInstance.modifier.Name + ": " + attributeInstance.GetFormattedValue()).Tooltip(attributeInstance.GetAttributeValueTooltip());
				}
			}
		}
		List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(selectedTarget));
		for (int j = 0; j < detailDescriptors.Count; j++)
		{
			Descriptor descriptor = detailDescriptors[j];
			drawer.NewLabel(descriptor.text).Tooltip(descriptor.tooltipText);
		}
	}
}
