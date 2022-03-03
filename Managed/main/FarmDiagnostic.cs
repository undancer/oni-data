using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FarmDiagnostic : ColonyDiagnostic
{
	private List<PlantablePlot> plots;

	public FarmDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_errand_farm";
		AddCriterion("CheckHasFarms", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKHASFARMS, CheckHasFarms));
		AddCriterion("CheckPlanted", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKPLANTED, CheckPlanted));
		AddCriterion("CheckWilting", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKWILTING, CheckWilting));
		AddCriterion("CheckOperational", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKOPERATIONAL, CheckOperational));
	}

	private void RefreshPlots()
	{
		plots = Components.PlantablePlots.GetItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
	}

	private DiagnosticResult CheckHasFarms()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (plots.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE;
		}
		return result;
	}

	private DiagnosticResult CheckPlanted()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		bool flag = false;
		foreach (PlantablePlot plot in plots)
		{
			if (plot.plant != null)
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
		foreach (PlantablePlot plot in plots)
		{
			if (plot.plant != null && plot.plant.HasTag(GameTags.Wilting))
			{
				StandardCropPlant component = plot.plant.GetComponent<StandardCropPlant>();
				if (component != null && component.smi.IsInsideState(component.smi.sm.alive.wilting) && component.smi.timeinstate > 15f)
				{
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.WILTING;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(plot.transform.position, plot.gameObject);
					return result;
				}
			}
		}
		return result;
	}

	private DiagnosticResult CheckOperational()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		foreach (PlantablePlot plot in plots)
		{
			if (plot.plant != null && !plot.HasTag(GameTags.Operational))
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.INOPERATIONAL;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(plot.transform.position, plot.gameObject);
				return result;
			}
		}
		return result;
	}

	public override string GetAverageValueString()
	{
		if (plots == null)
		{
			RefreshPlots();
		}
		return TrackerTool.Instance.GetWorldTracker<CropTracker>(base.worldID).GetCurrentValue() + "/" + plots.Count;
	}

	public override DiagnosticResult Evaluate()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.NO_MINIONS);
		if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
		{
			return result;
		}
		RefreshPlots();
		return base.Evaluate();
	}
}
