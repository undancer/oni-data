using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FarmDiagnostic : ColonyDiagnostic
{
	public FarmDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_errand_farm";
		AddCriterion(UI.COLONY_DIAGNOSTICS.NO_MINIONS, ColonyDiagnosticUtility.GetWorldHasMinionCriterion(worldID));
		AddCriterion("CheckHasFarms", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckHasFarms));
		AddCriterion("CheckPlanted", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckPlanted));
		AddCriterion("CheckWilting", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckWilting));
		AddCriterion("CheckOperational", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckOperational));
	}

	private DiagnosticResult CheckHasFarms()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
		if (list.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE;
		}
		return result;
	}

	private DiagnosticResult CheckPlanted()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
		bool flag = false;
		foreach (PlantablePlot item in list)
		{
			if (item.plant != null)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE_PLANTED;
		}
		return result;
	}

	private DiagnosticResult CheckWilting()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
		foreach (PlantablePlot item in list)
		{
			if (item.plant != null && item.plant.HasTag(GameTags.Wilting))
			{
				StandardCropPlant component = item.plant.GetComponent<StandardCropPlant>();
				if (component != null && component.smi.IsInsideState(component.smi.sm.alive.wilting) && component.smi.timeinstate > 15f)
				{
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.WILTING;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.transform.position, item.gameObject);
					break;
				}
			}
		}
		return result;
	}

	private DiagnosticResult CheckOperational()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
		foreach (PlantablePlot item in list)
		{
			if (item.plant != null && !item.HasTag(GameTags.Operational))
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.INOPERATIONAL;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.transform.position, item.gameObject);
				break;
			}
		}
		return result;
	}

	public override string GetAverageValueString()
	{
		return TrackerTool.Instance.GetWorldTracker<CropTracker>(base.worldID).GetCurrentValue() + "/" + Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed)).Count.ToString();
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
		{
			return result;
		}
		return base.Evaluate();
	}
}
