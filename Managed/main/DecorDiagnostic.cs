using System.Collections.Generic;
using STRINGS;

public class DecorDiagnostic : ColonyDiagnostic
{
	public DecorDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.DECORDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_category_decor";
		AddCriterion("CheckDecor", new DiagnosticCriterion(CheckDecor));
	}

	private DiagnosticResult CheckDecor()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		List<PlantablePlot> worldItems2 = Components.PlantablePlots.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
		}
		return result;
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
