using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ReactorDiagnostic : ColonyDiagnostic
{
	public ReactorDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.ALL_NAME)
	{
		icon = "overlay_radiation";
		AddCriterion("CheckTemperature", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckTemperature));
		AddCriterion("CheckCoolant", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.PLACEHOLDER_CRITERIA_NAME, CheckCoolant));
	}

	private DiagnosticResult CheckTemperature()
	{
		List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
		foreach (Reactor item in worldItems)
		{
			float fuelTemperature = item.FuelTemperature;
			if (fuelTemperature > 1254.8625f)
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_TEMPERATURE_WARNING;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
			}
		}
		return result;
	}

	private DiagnosticResult CheckCoolant()
	{
		List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(base.worldID);
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
		foreach (Reactor item in worldItems)
		{
			if (item.On)
			{
				float reserveCoolantMass = item.ReserveCoolantMass;
				if (reserveCoolantMass <= 45f)
				{
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_COOLANT_WARNING;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
				}
			}
		}
		return result;
	}
}
