using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ReactorDiagnostic : ColonyDiagnostic
{
	public ReactorDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.ALL_NAME)
	{
		icon = "overlay_radiation";
		AddCriterion("CheckReactor", new DiagnosticCriterion(CheckTemperature));
	}

	private DiagnosticResult CheckTemperature()
	{
		List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
		foreach (Reactor item in worldItems)
		{
			float fuelTemperature = item.GetFuelTemperature();
			if (fuelTemperature > 798.15f)
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_TEMPERATURE_WARNING;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
			}
		}
		return result;
	}
}
