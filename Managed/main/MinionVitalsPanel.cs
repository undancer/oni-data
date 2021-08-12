using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/MinionVitalsPanel")]
public class MinionVitalsPanel : KMonoBehaviour
{
	[DebuggerDisplay("{amount.Name}")]
	public struct AmountLine
	{
		public Amount amount;

		public GameObject go;

		public ValueTrendImageToggle imageToggle;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AmountInstance, string> toolTipFunc;

		public bool TryUpdate(Amounts amounts)
		{
			foreach (AmountInstance amount in amounts)
			{
				if (this.amount == amount.amount && !amount.hide)
				{
					locText.SetText(this.amount.GetDescription(amount));
					toolTip.toolTip = toolTipFunc(amount);
					imageToggle.SetValue(amount);
					return true;
				}
			}
			return false;
		}
	}

	[DebuggerDisplay("{attribute.Name}")]
	public struct AttributeLine
	{
		public Klei.AI.Attribute attribute;

		public GameObject go;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AttributeInstance, string> toolTipFunc;

		public bool TryUpdate(Attributes attributes)
		{
			foreach (AttributeInstance attribute in attributes)
			{
				if (this.attribute == attribute.modifier && !attribute.hide)
				{
					locText.SetText(this.attribute.GetDescription(attribute));
					toolTip.toolTip = toolTipFunc(attribute);
					return true;
				}
			}
			return false;
		}
	}

	public struct CheckboxLine
	{
		public Amount amount;

		public GameObject go;

		public LocText locText;

		public Func<GameObject, string> tooltip;

		public Func<GameObject, bool> get_value;

		public Func<GameObject, CheckboxLineDisplayType> display_condition;

		public Func<GameObject, string> label_text_func;

		public Transform parentContainer;
	}

	public enum CheckboxLineDisplayType
	{
		Normal,
		Diminished,
		Hidden
	}

	public GameObject LineItemPrefab;

	public GameObject CheckboxLinePrefab;

	public GameObject selectedEntity;

	public List<AmountLine> amountsLines = new List<AmountLine>();

	public List<AttributeLine> attributesLines = new List<AttributeLine>();

	public List<CheckboxLine> checkboxLines = new List<CheckboxLine>();

	public Transform conditionsContainerNormal;

	public Transform conditionsContainerAdditional;

	public void Init()
	{
		AddAmountLine(Db.Get().Amounts.HitPoints);
		AddAttributeLine(Db.Get().CritterAttributes.Happiness);
		AddAmountLine(Db.Get().Amounts.Wildness);
		AddAmountLine(Db.Get().Amounts.Incubation);
		AddAmountLine(Db.Get().Amounts.Viability);
		AddAmountLine(Db.Get().Amounts.PowerCharge);
		AddAmountLine(Db.Get().Amounts.Fertility);
		AddAmountLine(Db.Get().Amounts.Age);
		AddAmountLine(Db.Get().Amounts.Stress);
		AddAttributeLine(Db.Get().Attributes.QualityOfLife);
		AddAmountLine(Db.Get().Amounts.Bladder);
		AddAmountLine(Db.Get().Amounts.Breath);
		AddAmountLine(Db.Get().Amounts.Stamina);
		AddAmountLine(Db.Get().Amounts.Calories);
		AddAmountLine(Db.Get().Amounts.ScaleGrowth);
		AddAmountLine(Db.Get().Amounts.Temperature);
		AddAmountLine(Db.Get().Amounts.Decor);
		AddAmountLine(Db.Get().Amounts.InternalBattery);
		AddAmountLine(Db.Get().Amounts.InternalChemicalBattery);
		if (DlcManager.FeatureRadiationEnabled())
		{
			AddAmountLine(Db.Get().Amounts.RadiationBalance);
		}
		AddCheckboxLine(Db.Get().Amounts.AirPressure, conditionsContainerNormal, (GameObject go) => GetAirPressureLabel(go), (GameObject go) => (!(go.GetComponent<PressureVulnerable>() != null) || !go.GetComponent<PressureVulnerable>().pressure_sensitive) ? CheckboxLineDisplayType.Hidden : CheckboxLineDisplayType.Normal, (GameObject go) => check_pressure(go), (GameObject go) => GetAirPressureTooltip(go));
		AddCheckboxLine(null, conditionsContainerNormal, (GameObject go) => GetAtmosphereLabel(go), (GameObject go) => (!(go.GetComponent<PressureVulnerable>() != null) || go.GetComponent<PressureVulnerable>().safe_atmospheres.Count <= 0) ? CheckboxLineDisplayType.Hidden : CheckboxLineDisplayType.Normal, (GameObject go) => check_atmosphere(go), (GameObject go) => GetAtmosphereTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Temperature, conditionsContainerNormal, (GameObject go) => GetInternalTemperatureLabel(go), (GameObject go) => (!(go.GetComponent<TemperatureVulnerable>() != null)) ? CheckboxLineDisplayType.Hidden : CheckboxLineDisplayType.Normal, (GameObject go) => check_temperature(go), (GameObject go) => GetInternalTemperatureTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Fertilization, conditionsContainerAdditional, (GameObject go) => GetFertilizationLabel(go), delegate(GameObject go)
		{
			if (go.GetComponent<ReceptacleMonitor>() == null)
			{
				return CheckboxLineDisplayType.Hidden;
			}
			return (!go.GetComponent<ReceptacleMonitor>().Replanted) ? CheckboxLineDisplayType.Diminished : CheckboxLineDisplayType.Normal;
		}, (GameObject go) => check_fertilizer(go), (GameObject go) => GetFertilizationTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Irrigation, conditionsContainerAdditional, (GameObject go) => GetIrrigationLabel(go), delegate(GameObject go)
		{
			ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
			return (!(component != null) || !component.Replanted) ? CheckboxLineDisplayType.Diminished : CheckboxLineDisplayType.Normal;
		}, (GameObject go) => check_irrigation(go), (GameObject go) => GetIrrigationTooltip(go));
		AddCheckboxLine(Db.Get().Amounts.Illumination, conditionsContainerNormal, (GameObject go) => GetIlluminationLabel(go), (GameObject go) => CheckboxLineDisplayType.Normal, (GameObject go) => check_illumination(go), (GameObject go) => GetIlluminationTooltip(go));
		AddCheckboxLine(null, conditionsContainerNormal, (GameObject go) => GetRadiationLabel(go), delegate(GameObject go)
		{
			AttributeInstance attributeInstance = go.GetAttributes().Get(Db.Get().PlantAttributes.MaxRadiationThreshold);
			return (attributeInstance == null || !(attributeInstance.GetTotalValue() > 0f)) ? CheckboxLineDisplayType.Hidden : CheckboxLineDisplayType.Normal;
		}, (GameObject go) => check_radiation(go), (GameObject go) => GetRadiationTooltip(go));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		SimAndRenderScheduler.instance.Add(this);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void AddAmountLine(Amount amount, Func<AmountInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(LineItemPrefab, base.gameObject);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(amount.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(value: true);
		AmountLine item = default(AmountLine);
		item.amount = amount;
		item.go = gameObject;
		item.locText = gameObject.GetComponentInChildren<LocText>();
		item.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		item.imageToggle = gameObject.GetComponentInChildren<ValueTrendImageToggle>();
		item.toolTipFunc = ((tooltip_func != null) ? tooltip_func : new Func<AmountInstance, string>(amount.GetTooltip));
		amountsLines.Add(item);
	}

	private void AddAttributeLine(Klei.AI.Attribute attribute, Func<AttributeInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(LineItemPrefab, base.gameObject);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(attribute.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(value: true);
		AttributeLine item = default(AttributeLine);
		item.attribute = attribute;
		item.go = gameObject;
		item.locText = gameObject.GetComponentInChildren<LocText>();
		item.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		gameObject.GetComponentInChildren<ValueTrendImageToggle>().gameObject.SetActive(value: false);
		item.toolTipFunc = ((tooltip_func != null) ? tooltip_func : new Func<AttributeInstance, string>(attribute.GetTooltip));
		attributesLines.Add(item);
	}

	private void AddCheckboxLine(Amount amount, Transform parentContainer, Func<GameObject, string> label_text_func, Func<GameObject, CheckboxLineDisplayType> display_condition, Func<GameObject, bool> checkbox_value_func, Func<GameObject, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(CheckboxLinePrefab, base.gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(value: true);
		CheckboxLine item = default(CheckboxLine);
		item.go = gameObject;
		item.parentContainer = parentContainer;
		item.amount = amount;
		item.locText = component.GetReference("Label") as LocText;
		item.get_value = checkbox_value_func;
		item.display_condition = display_condition;
		item.label_text_func = label_text_func;
		item.go.name = "Checkbox_";
		if (amount != null)
		{
			item.go.name += amount.Name;
		}
		else
		{
			item.go.name += "Unnamed";
		}
		if (tooltip_func != null)
		{
			item.tooltip = tooltip_func;
			ToolTip tt = item.go.GetComponent<ToolTip>();
			tt.refreshWhileHovering = true;
			tt.OnToolTip = delegate
			{
				tt.ClearMultiStringTooltip();
				tt.AddMultiStringTooltip(tooltip_func(selectedEntity), null);
				return "";
			};
		}
		checkboxLines.Add(item);
	}

	public void Refresh()
	{
		if (selectedEntity == null || selectedEntity.gameObject == null)
		{
			return;
		}
		Amounts amounts = selectedEntity.GetAmounts();
		Attributes attributes = selectedEntity.GetAttributes();
		if (amounts == null || attributes == null)
		{
			return;
		}
		WiltCondition component = selectedEntity.GetComponent<WiltCondition>();
		if (component == null)
		{
			conditionsContainerNormal.gameObject.SetActive(value: false);
			conditionsContainerAdditional.gameObject.SetActive(value: false);
			foreach (AmountLine amountsLine in amountsLines)
			{
				bool flag = amountsLine.TryUpdate(amounts);
				if (amountsLine.go.activeSelf != flag)
				{
					amountsLine.go.SetActive(flag);
				}
			}
			foreach (AttributeLine attributesLine in attributesLines)
			{
				bool flag2 = attributesLine.TryUpdate(attributes);
				if (attributesLine.go.activeSelf != flag2)
				{
					attributesLine.go.SetActive(flag2);
				}
			}
		}
		bool flag3 = false;
		for (int i = 0; i < checkboxLines.Count; i++)
		{
			CheckboxLine checkboxLine = checkboxLines[i];
			CheckboxLineDisplayType checkboxLineDisplayType = CheckboxLineDisplayType.Hidden;
			if (checkboxLines[i].amount != null)
			{
				for (int j = 0; j < amounts.Count; j++)
				{
					AmountInstance amountInstance = amounts[j];
					if (checkboxLine.amount == amountInstance.amount)
					{
						checkboxLineDisplayType = checkboxLine.display_condition(selectedEntity.gameObject);
						break;
					}
				}
			}
			else
			{
				checkboxLineDisplayType = checkboxLine.display_condition(selectedEntity.gameObject);
			}
			if (checkboxLineDisplayType != CheckboxLineDisplayType.Hidden)
			{
				checkboxLine.locText.SetText(checkboxLine.label_text_func(selectedEntity.gameObject));
				if (!checkboxLine.go.activeSelf)
				{
					checkboxLine.go.SetActive(value: true);
				}
				GameObject gameObject = checkboxLine.go.GetComponent<HierarchyReferences>().GetReference("Check").gameObject;
				gameObject.SetActive(checkboxLine.get_value(selectedEntity.gameObject));
				if (checkboxLine.go.transform.parent != checkboxLine.parentContainer)
				{
					checkboxLine.go.transform.SetParent(checkboxLine.parentContainer);
					checkboxLine.go.transform.localScale = Vector3.one;
				}
				if (checkboxLine.parentContainer == conditionsContainerAdditional)
				{
					flag3 = true;
				}
				if (checkboxLineDisplayType == CheckboxLineDisplayType.Normal)
				{
					if (checkboxLine.get_value(selectedEntity.gameObject))
					{
						checkboxLine.locText.color = Color.black;
						gameObject.transform.parent.GetComponent<Image>().color = Color.black;
					}
					else
					{
						Color color = new Color(253f / 255f, 0f, 26f / 255f);
						checkboxLine.locText.color = color;
						gameObject.transform.parent.GetComponent<Image>().color = color;
					}
				}
				else
				{
					checkboxLine.locText.color = Color.grey;
					gameObject.transform.parent.GetComponent<Image>().color = Color.grey;
				}
			}
			else if (checkboxLine.go.activeSelf)
			{
				checkboxLine.go.SetActive(value: false);
			}
		}
		if (!(component != null))
		{
			return;
		}
		Growing component2 = component.GetComponent<Growing>();
		bool flag4 = component.HasTag(GameTags.Decoration);
		conditionsContainerNormal.gameObject.SetActive(value: true);
		conditionsContainerAdditional.gameObject.SetActive(!flag4);
		if (component2 == null)
		{
			float num = 1f;
			LocText reference = conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
			reference.text = "";
			reference.text = (flag4 ? string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD_DECOR.BASE) : string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD_INSTANT.BASE, Util.FormatTwoDecimalPlace(num * 0.25f * 100f)));
			reference.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD_INSTANT.TOOLTIP));
			LocText reference2 = conditionsContainerAdditional.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
			ReceptacleMonitor component3 = selectedEntity.GetComponent<ReceptacleMonitor>();
			reference2.color = ((component3 == null || component3.Replanted) ? Color.black : Color.grey);
			reference2.text = string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC_INSTANT.BASE, Util.FormatTwoDecimalPlace(num * 100f));
			reference2.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC_INSTANT.TOOLTIP));
		}
		else
		{
			LocText reference3 = conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
			reference3.text = "";
			reference3.text = string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime()));
			reference3.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime())));
			LocText reference4 = conditionsContainerAdditional.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
			reference4.color = (selectedEntity.GetComponent<ReceptacleMonitor>().Replanted ? Color.black : Color.grey);
			reference4.text = "";
			reference4.text = (flag3 ? string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime())) : string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime())));
			reference4.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime())));
		}
		foreach (AmountLine amountsLine2 in amountsLines)
		{
			amountsLine2.go.SetActive(value: false);
		}
		foreach (AttributeLine attributesLine2 in attributesLines)
		{
			attributesLine2.go.SetActive(value: false);
		}
	}

	private string GetAirPressureTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component == null)
		{
			return "";
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_PRESSURE.text.Replace("{pressure}", GameUtil.GetFormattedMass(component.GetExternalPressure()));
	}

	private string GetInternalTemperatureTooltip(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		if (component == null)
		{
			return "";
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_TEMPERATURE.text.Replace("{temperature}", GameUtil.GetFormattedTemperature(component.InternalTemperature));
	}

	private string GetFertilizationTooltip(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		if (sMI == null)
		{
			return "";
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_FERTILIZER.text.Replace("{mass}", GameUtil.GetFormattedMass(sMI.total_fertilizer_available));
	}

	private string GetIrrigationTooltip(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		if (sMI == null)
		{
			return "";
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_IRRIGATION.text.Replace("{mass}", GameUtil.GetFormattedMass(sMI.total_fertilizer_available));
	}

	private string GetIlluminationTooltip(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		if (component == null)
		{
			return "";
		}
		if ((component.prefersDarkness && component.IsComfortable()) || (!component.prefersDarkness && !component.IsComfortable()))
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_DARK;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_LIGHT;
	}

	private string GetRadiationTooltip(GameObject go)
	{
		int num = Grid.PosToCell(go);
		float rads = (Grid.IsValidCell(num) ? Grid.Radiation[num] : 0f);
		AttributeInstance attributeInstance = go.GetAttributes().Get(Db.Get().PlantAttributes.MinRadiationThreshold);
		AttributeInstance attributeInstance2 = go.GetAttributes().Get(Db.Get().PlantAttributes.MaxRadiationThreshold);
		MutantPlant component = go.GetComponent<MutantPlant>();
		bool num2 = component != null && component.IsOriginal;
		string text = ((attributeInstance.GetTotalValue() != 0f) ? UI.TOOLTIPS.VITALS_CHECKBOX_RADIATION.Replace("{rads}", GameUtil.GetFormattedRads(rads)).Replace("{minRads}", attributeInstance.GetFormattedValue()).Replace("{maxRads}", attributeInstance2.GetFormattedValue()) : UI.TOOLTIPS.VITALS_CHECKBOX_RADIATION_NO_MIN.Replace("{rads}", GameUtil.GetFormattedRads(rads)).Replace("{maxRads}", attributeInstance2.GetFormattedValue()));
		if (num2)
		{
			text += UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_SEED_TOOLTIP;
		}
		return text;
	}

	private string GetReceptacleTooltip(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		if (component == null)
		{
			return "";
		}
		if (component.HasOperationalReceptacle())
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_OPERATIONAL;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_INOPERATIONAL;
	}

	private string GetAtmosphereTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component != null && component.currentAtmoElement != null)
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE.text.Replace("{element}", component.currentAtmoElement.name);
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE;
	}

	private string GetAirPressureLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return Db.Get().Amounts.AirPressure.Name + "\n    • " + GameUtil.GetFormattedMass(component.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram, includeSuffix: false) + " - " + GameUtil.GetFormattedMass(component.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram);
	}

	private string GetInternalTemperatureLabel(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		return Db.Get().Amounts.Temperature.Name + "\n    • " + GameUtil.GetFormattedTemperature(component.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, displayUnits: false) + " - " + GameUtil.GetFormattedTemperature(component.TemperatureWarningHigh);
	}

	private string GetFertilizationLabel(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		string text = Db.Get().Amounts.Fertilization.Name;
		float totalValue = go.GetAttributes().Get(Db.Get().PlantAttributes.FertilizerUsageMod).GetTotalValue();
		PlantElementAbsorber.ConsumeInfo[] consumedElements = sMI.def.consumedElements;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements[i];
			text = text + "\n    • " + ElementLoader.GetElement(consumeInfo.tag).name + " " + GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate * totalValue, GameUtil.TimeSlice.PerCycle);
		}
		return text;
	}

	private string GetIrrigationLabel(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		string text = Db.Get().Amounts.Irrigation.Name;
		float totalValue = go.GetAttributes().Get(Db.Get().PlantAttributes.FertilizerUsageMod).GetTotalValue();
		PlantElementAbsorber.ConsumeInfo[] consumedElements = sMI.def.consumedElements;
		for (int i = 0; i < consumedElements.Length; i++)
		{
			PlantElementAbsorber.ConsumeInfo consumeInfo = consumedElements[i];
			text = text + "\n    • " + ElementLoader.GetElement(consumeInfo.tag).name + ": " + GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate * totalValue, GameUtil.TimeSlice.PerCycle);
		}
		return text;
	}

	private string GetIlluminationLabel(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		return Db.Get().Amounts.Illumination.Name + "\n    • " + (component.prefersDarkness ? UI.GAMEOBJECTEFFECTS.DARKNESS.ToString() : GameUtil.GetFormattedLux(component.LightIntensityThreshold));
	}

	private string GetAtmosphereLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		string text = UI.VITALSSCREEN.ATMOSPHERE_CONDITION;
		foreach (Element safe_atmosphere in component.safe_atmospheres)
		{
			text = text + "\n    • " + safe_atmosphere.name;
		}
		return text;
	}

	private string GetRadiationLabel(GameObject go)
	{
		AttributeInstance attributeInstance = go.GetAttributes().Get(Db.Get().PlantAttributes.MinRadiationThreshold);
		AttributeInstance attributeInstance2 = go.GetAttributes().Get(Db.Get().PlantAttributes.MaxRadiationThreshold);
		if (attributeInstance.GetTotalValue() == 0f)
		{
			return string.Concat(UI.GAMEOBJECTEFFECTS.AMBIENT_RADIATION, "\n    • ", UI.GAMEOBJECTEFFECTS.AMBIENT_NO_MIN_RADIATION_FMT.Replace("{maxRads}", attributeInstance2.GetFormattedValue()));
		}
		return string.Concat(UI.GAMEOBJECTEFFECTS.AMBIENT_RADIATION, "\n    • ", UI.GAMEOBJECTEFFECTS.AMBIENT_RADIATION_FMT.Replace("{minRads}", attributeInstance.GetFormattedValue()).Replace("{maxRads}", attributeInstance2.GetFormattedValue()));
	}

	private bool check_pressure(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component != null)
		{
			return component.ExternalPressureState == PressureVulnerable.PressureState.Normal;
		}
		return true;
	}

	private bool check_temperature(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		if (component != null)
		{
			return component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal;
		}
		return true;
	}

	private bool check_irrigation(GameObject go)
	{
		IrrigationMonitor.Instance sMI = go.GetSMI<IrrigationMonitor.Instance>();
		if (sMI != null)
		{
			if (!sMI.IsInsideState(sMI.sm.replanted.starved))
			{
				return !sMI.IsInsideState(sMI.sm.wild);
			}
			return false;
		}
		return true;
	}

	private bool check_illumination(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		if (component != null)
		{
			return component.IsComfortable();
		}
		return true;
	}

	private bool check_radiation(GameObject go)
	{
		AttributeInstance attributeInstance = go.GetAttributes().Get(Db.Get().PlantAttributes.MinRadiationThreshold);
		if (attributeInstance != null && attributeInstance.GetTotalValue() != 0f)
		{
			int num = Grid.PosToCell(go);
			return (Grid.IsValidCell(num) ? Grid.Radiation[num] : 0f) >= attributeInstance.GetTotalValue();
		}
		return true;
	}

	private bool check_receptacle(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		if (component == null)
		{
			return false;
		}
		return component.HasOperationalReceptacle();
	}

	private bool check_fertilizer(GameObject go)
	{
		FertilizationMonitor.Instance sMI = go.GetSMI<FertilizationMonitor.Instance>();
		return sMI?.sm.hasCorrectFertilizer.Get(sMI) ?? true;
	}

	private bool check_atmosphere(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component != null)
		{
			return component.testAreaElementSafe;
		}
		return true;
	}
}
