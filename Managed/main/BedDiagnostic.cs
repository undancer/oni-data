using System.Collections.Generic;
using STRINGS;

public class BedDiagnostic : ColonyDiagnostic
{
	public BedDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.ALL_NAME)
	{
		icon = "icon_action_region_bedroom";
		AddCriterion("CheckEnoughBeds", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.CRITERIA.CHECKENOUGHBEDS, CheckEnoughBeds));
	}

	private DiagnosticResult CheckEnoughBeds()
	{
		List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		if (worldItems.Count == 0)
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
		}
		else
		{
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NORMAL;
			int num = 0;
			List<Sleepable> worldItems2 = Components.Sleepables.GetWorldItems(base.worldID);
			for (int i = 0; i < worldItems2.Count; i++)
			{
				if (worldItems2[i].GetComponent<Assignable>() != null && worldItems2[i].GetComponent<Clinic>() == null)
				{
					num++;
				}
			}
			if (num < worldItems.Count)
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NOT_ENOUGH_BEDS;
			}
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

	public override string GetAverageValueString()
	{
		return Components.Sleepables.GetWorldItems(base.worldID).FindAll((Sleepable match) => match.GetComponent<Assignable>() != null).Count + "/" + Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count;
	}
}
