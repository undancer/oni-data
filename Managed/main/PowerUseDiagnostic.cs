using STRINGS;
using UnityEngine;

public class PowerUseDiagnostic : ColonyDiagnostic
{
	public PowerUseDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<PowerUseTracker>(worldID);
		trackerSampleCountSeconds = 30f;
		icon = "overlay_power";
		AddCriterion("CheckOverWattage", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CRITERIA.CHECKOVERWATTAGE, CheckOverWattage));
		AddCriterion("CheckPowerUseChange", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CRITERIA.CHECKPOWERUSECHANGE, CheckPowerChange));
	}

	private DiagnosticResult CheckOverWattage()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
		foreach (ElectricalUtilityNetwork network in Game.Instance.electricalConduitSystem.GetNetworks())
		{
			if (network.allWires == null || network.allWires.Count == 0)
			{
				continue;
			}
			int num = Grid.PosToCell(network.allWires[0]);
			if (Grid.WorldIdx[num] == base.worldID)
			{
				ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num);
				float maxSafeWattageForCircuit = Game.Instance.circuitManager.GetMaxSafeWattageForCircuit(circuitID);
				float wattsUsedByCircuit = Game.Instance.circuitManager.GetWattsUsedByCircuit(circuitID);
				if (wattsUsedByCircuit > maxSafeWattageForCircuit)
				{
					GameObject gameObject = network.allWires[0].gameObject;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(gameObject.transform.position, gameObject);
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = string.Format(UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CIRCUIT_OVER_CAPACITY, GameUtil.GetFormattedWattage(wattsUsedByCircuit), GameUtil.GetFormattedWattage(maxSafeWattageForCircuit));
					return result;
				}
			}
		}
		return result;
	}

	private DiagnosticResult CheckPowerChange()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		result.opinion = DiagnosticResult.Opinion.Normal;
		result.Message = UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
		float num = 60f;
		if (tracker.GetDataTimeLength() < num)
		{
			return result;
		}
		float averageValue = tracker.GetAverageValue(1f);
		float averageValue2 = tracker.GetAverageValue(Mathf.Min(60f, trackerSampleCountSeconds));
		float num2 = 240f;
		if (averageValue < num2 && averageValue2 < num2)
		{
			return result;
		}
		float num3 = 0.5f;
		if (Mathf.Abs(averageValue - averageValue2) / averageValue2 > num3)
		{
			result.opinion = DiagnosticResult.Opinion.Concern;
			result.Message = string.Format(UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.SIGNIFICANT_POWER_CHANGE_DETECTED, GameUtil.GetFormattedWattage(averageValue2), GameUtil.GetFormattedWattage(averageValue));
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
