using System.Collections.Generic;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

public class DiseaseInfoScreen : TargetScreen
{
	private CollapsibleDetailContentPanel infectionPanel;

	private CollapsibleDetailContentPanel immuneSystemPanel;

	private CollapsibleDetailContentPanel diseaseSourcePanel;

	private CollapsibleDetailContentPanel currentGermsPanel;

	private CollapsibleDetailContentPanel infoPanel;

	private static readonly EventSystem.IntraObjectHandler<DiseaseInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DiseaseInfoScreen>(delegate(DiseaseInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	public override bool IsValidForTarget(GameObject target)
	{
		if (!CellSelectionObject.IsSelectionObject(target))
		{
			return target.GetComponent<PrimaryElement>() != null;
		}
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		diseaseSourcePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject).GetComponent<CollapsibleDetailContentPanel>();
		diseaseSourcePanel.SetTitle(UI.DETAILTABS.DISEASE.DISEASE_SOURCE);
		immuneSystemPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject).GetComponent<CollapsibleDetailContentPanel>();
		immuneSystemPanel.SetTitle(UI.DETAILTABS.DISEASE.IMMUNE_SYSTEM);
		currentGermsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject).GetComponent<CollapsibleDetailContentPanel>();
		currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.CURRENT_GERMS);
		infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject).GetComponent<CollapsibleDetailContentPanel>();
		infoPanel.SetTitle(UI.DETAILTABS.DISEASE.GERMS_INFO);
		infectionPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject).GetComponent<CollapsibleDetailContentPanel>();
		infectionPanel.SetTitle(UI.DETAILTABS.DISEASE.INFECTION_INFO);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void OnRefreshData(object obj)
	{
		Refresh();
	}

	private void Refresh()
	{
		if (selectedTarget == null)
		{
			return;
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(selectedTarget, simpleInfoScreen: true);
		Sicknesses sicknesses = selectedTarget.GetSicknesses();
		if (sicknesses != null)
		{
			for (int i = 0; i < sicknesses.Count; i++)
			{
				allDescriptors.AddRange(sicknesses[i].GetDescriptors());
			}
		}
		allDescriptors = allDescriptors.FindAll((Descriptor e) => e.type == Descriptor.DescriptorType.DiseaseSource);
		if (allDescriptors.Count > 0)
		{
			for (int j = 0; j < allDescriptors.Count; j++)
			{
				diseaseSourcePanel.SetLabel("source_" + j, allDescriptors[j].text, allDescriptors[j].tooltipText);
			}
		}
		CreateImmuneInfo();
		if (!CreateDiseaseInfo())
		{
			currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.NO_CURRENT_GERMS);
			currentGermsPanel.SetLabel("nodisease", UI.DETAILTABS.DISEASE.DETAILS.NODISEASE, UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP);
		}
		diseaseSourcePanel.Commit();
		immuneSystemPanel.Commit();
		currentGermsPanel.Commit();
		infoPanel.Commit();
		infectionPanel.Commit();
	}

	private bool CreateImmuneInfo()
	{
		GermExposureMonitor.Instance sMI = selectedTarget.GetSMI<GermExposureMonitor.Instance>();
		if (sMI != null)
		{
			immuneSystemPanel.SetTitle(UI.DETAILTABS.DISEASE.CONTRACTION_RATES);
			immuneSystemPanel.SetLabel("germ_resistance", Db.Get().Attributes.GermResistance.Name + ": " + sMI.GetGermResistance(), DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.DESC);
			for (int i = 0; i < Db.Get().Diseases.Count; i++)
			{
				Disease disease = Db.Get().Diseases[i];
				ExposureType exposureTypeForDisease = GameUtil.GetExposureTypeForDisease(disease);
				Sickness sicknessForDisease = GameUtil.GetSicknessForDisease(disease);
				if (sicknessForDisease == null)
				{
					continue;
				}
				bool flag = true;
				List<string> list = new List<string>();
				if (exposureTypeForDisease.required_traits != null && exposureTypeForDisease.required_traits.Count > 0)
				{
					for (int j = 0; j < exposureTypeForDisease.required_traits.Count; j++)
					{
						if (!selectedTarget.GetComponent<Traits>().HasTrait(exposureTypeForDisease.required_traits[j]))
						{
							list.Add(exposureTypeForDisease.required_traits[j]);
						}
					}
					if (list.Count > 0)
					{
						flag = false;
					}
				}
				bool flag2 = false;
				List<string> list2 = new List<string>();
				if (exposureTypeForDisease.excluded_effects != null && exposureTypeForDisease.excluded_effects.Count > 0)
				{
					for (int k = 0; k < exposureTypeForDisease.excluded_effects.Count; k++)
					{
						if (selectedTarget.GetComponent<Effects>().HasEffect(exposureTypeForDisease.excluded_effects[k]))
						{
							list2.Add(exposureTypeForDisease.excluded_effects[k]);
						}
					}
					if (list2.Count > 0)
					{
						flag2 = true;
					}
				}
				bool flag3 = false;
				List<string> list3 = new List<string>();
				if (exposureTypeForDisease.excluded_traits != null && exposureTypeForDisease.excluded_traits.Count > 0)
				{
					for (int l = 0; l < exposureTypeForDisease.excluded_traits.Count; l++)
					{
						if (selectedTarget.GetComponent<Traits>().HasTrait(exposureTypeForDisease.excluded_traits[l]))
						{
							list3.Add(exposureTypeForDisease.excluded_traits[l]);
						}
					}
					if (list3.Count > 0)
					{
						flag3 = true;
					}
				}
				string text = "";
				float num;
				if (!flag)
				{
					num = 0f;
					string text2 = "";
					for (int m = 0; m < list.Count; m++)
					{
						if (text2 != "")
						{
							text2 += ", ";
						}
						text2 += Db.Get().traits.Get(list[m]).Name;
					}
					text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_MISSING_REQUIRED_TRAIT, text2);
				}
				else if (flag3)
				{
					num = 0f;
					string text3 = "";
					for (int n = 0; n < list3.Count; n++)
					{
						if (text3 != "")
						{
							text3 += ", ";
						}
						text3 += Db.Get().traits.Get(list3[n]).Name;
					}
					if (text != "")
					{
						text += "\n";
					}
					text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_HAVING_EXLCLUDED_TRAIT, text3);
				}
				else if (!flag2)
				{
					num = ((!exposureTypeForDisease.infect_immediately) ? GermExposureMonitor.GetContractionChance(sMI.GetResistanceToExposureType(exposureTypeForDisease, 3f)) : 1f);
				}
				else
				{
					num = 0f;
					string text4 = "";
					for (int num2 = 0; num2 < list2.Count; num2++)
					{
						if (text4 != "")
						{
							text4 += ", ";
						}
						text4 += Db.Get().effects.Get(list2[num2]).Name;
					}
					if (text != "")
					{
						text += "\n";
					}
					text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_HAVING_EXCLUDED_EFFECT, text4);
				}
				string arg = ((text != "") ? text : string.Format(DUPLICANTS.DISEASES.CONTRACTION_PROBABILITY, GameUtil.GetFormattedPercent(num * 100f), selectedTarget.GetProperName(), sicknessForDisease.Name));
				immuneSystemPanel.SetLabel("disease_" + disease.Id, "    • " + disease.Name + ": " + GameUtil.GetFormattedPercent(num * 100f), string.Format(DUPLICANTS.DISEASES.RESISTANCES_PANEL_TOOLTIP, arg, sicknessForDisease.Name));
			}
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo()
	{
		if (selectedTarget.GetComponent<PrimaryElement>() != null)
		{
			return CreateDiseaseInfo_PrimaryElement();
		}
		CellSelectionObject component = selectedTarget.GetComponent<CellSelectionObject>();
		if (component != null)
		{
			return CreateDiseaseInfo_CellSelectionObject(component);
		}
		return false;
	}

	private string GetFormattedHalfLife(float hl)
	{
		return GetFormattedGrowthRate(Disease.HalfLifeToGrowthRate(hl, 600f));
	}

	private string GetFormattedGrowthRate(float rate)
	{
		if (rate < 1f)
		{
			return string.Format(UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT, GameUtil.GetFormattedPercent(100f * (1f - rate)), UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT_TOOLTIP);
		}
		if (rate > 1f)
		{
			return string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT, GameUtil.GetFormattedPercent(100f * (rate - 1f)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT_TOOLTIP);
		}
		return string.Format(UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT, UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT_TOOLTIP);
	}

	private string GetFormattedGrowthEntry(string name, float halfLife, string dyingFormat, string growingFormat, string neutralFormat)
	{
		string format = ((halfLife == float.PositiveInfinity) ? neutralFormat : ((!(halfLife > 0f)) ? growingFormat : dyingFormat));
		return string.Format(format, name, GetFormattedHalfLife(halfLife));
	}

	private void BuildFactorsStrings(int diseaseCount, int elementIdx, int environmentCell, float environmentMass, float temperature, HashSet<Tag> tags, Disease disease, bool isCell = false)
	{
		currentGermsPanel.SetTitle(string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper()));
		currentGermsPanel.SetLabel("currentgerms", string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount)));
		Element e = ElementLoader.elements[elementIdx];
		CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(e);
		float tags_multiplier_base = 1f;
		if (tags != null && tags.Count > 0)
		{
			tags_multiplier_base = disease.GetGrowthRateForTags(tags, (float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass);
		}
		float num = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentMass, environmentCell, temperature, tags_multiplier_base, disease, 1f, Sim.IsRadiationEnabled());
		currentGermsPanel.SetLabel("finaldelta", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.PerSecond, "F0")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.PerSecond, "F0")));
		float num2 = Disease.GrowthRateToHalfLife(1f - num / (float)diseaseCount);
		if (num2 > 0f)
		{
			currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num2)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num2)));
		}
		else if (num2 < 0f)
		{
			currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(0f - num2)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num2)));
		}
		else
		{
			currentGermsPanel.SetLabel("finalhalflife", UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP);
		}
		currentGermsPanel.SetLabel("factors", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP);
		bool flag = false;
		if ((float)diseaseCount < growthRuleForElement.minCountPerKG * environmentMass)
		{
			currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TITLE, GetFormattedGrowthRate(0f - growthRuleForElement.underPopulationDeathRate)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.minCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass), growthRuleForElement.minCountPerKG));
			flag = true;
		}
		else if ((float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass)
		{
			currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TITLE, GetFormattedHalfLife(growthRuleForElement.overPopulationHalfLife)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.maxCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass), growthRuleForElement.maxCountPerKG));
			flag = true;
		}
		if (!flag)
		{
			currentGermsPanel.SetLabel("substrate", GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
		}
		int num3 = 0;
		if (tags != null)
		{
			foreach (Tag tag in tags)
			{
				TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tag);
				if (growthRuleForTag != null)
				{
					currentGermsPanel.SetLabel("tag_" + num3, GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
				}
				num3++;
			}
		}
		if (Grid.IsValidCell(environmentCell))
		{
			if (!isCell)
			{
				CompositeExposureRule exposureRuleForElement = disease.GetExposureRuleForElement(Grid.Element[environmentCell]);
				if (exposureRuleForElement != null && exposureRuleForElement.populationHalfLife != float.PositiveInfinity)
				{
					if (exposureRuleForElement.GetHalfLifeForCount(diseaseCount) > 0f)
					{
						currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.DIE_TOOLTIP);
					}
					else
					{
						currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.GROW_TOOLTIP);
					}
				}
			}
			if (Sim.IsRadiationEnabled())
			{
				float num4 = Grid.Radiation[environmentCell];
				if (num4 > 0f)
				{
					float num5 = disease.radiationKillRate * num4;
					float hl = (float)diseaseCount * 0.5f / num5;
					currentGermsPanel.SetLabel("radiation", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RADIATION.TITLE, Mathf.RoundToInt(num4), GetFormattedHalfLife(hl)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RADIATION.DIE_TOOLTIP);
				}
			}
		}
		float num6 = disease.CalculateTemperatureHalfLife(temperature);
		if (num6 != float.PositiveInfinity)
		{
			if (num6 > 0f)
			{
				currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature), GetFormattedHalfLife(num6)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP);
			}
			else
			{
				currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature), GetFormattedHalfLife(num6)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP);
			}
		}
	}

	private bool CreateDiseaseInfo_PrimaryElement()
	{
		if (selectedTarget == null)
		{
			return false;
		}
		PrimaryElement component = selectedTarget.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return false;
		}
		if (component.DiseaseIdx != byte.MaxValue && component.DiseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[component.DiseaseIdx];
			int environmentCell = Grid.PosToCell(component.transform.GetPosition());
			KPrefabID component2 = component.GetComponent<KPrefabID>();
			BuildFactorsStrings(component.DiseaseCount, component.Element.idx, environmentCell, component.Mass, component.Temperature, component2.Tags, disease);
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo_CellSelectionObject(CellSelectionObject cso)
	{
		if (cso.diseaseIdx != byte.MaxValue && cso.diseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[cso.diseaseIdx];
			int idx = cso.element.idx;
			BuildFactorsStrings(cso.diseaseCount, idx, cso.SelectedCell, cso.Mass, cso.temperature, null, disease, isCell: true);
			return true;
		}
		return false;
	}
}
