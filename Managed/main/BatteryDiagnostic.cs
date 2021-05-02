using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BatteryDiagnostic : ColonyDiagnostic
{
	public BatteryDiagnostic(int worldID)
		: base(worldID, UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.ALL_NAME)
	{
		tracker = TrackerTool.Instance.GetWorldTracker<BatteryTracker>(worldID);
		trackerSampleCountSeconds = 4f;
		icon = "overlay_power";
		AddCriterion(UI.COLONY_DIAGNOSTICS.NO_MINIONS, ColonyDiagnosticUtility.GetWorldHasMinionCriterion(worldID));
		AddCriterion("CheckCapacity", new DiagnosticCriterion(CheckCapacity));
		AddCriterion("CheckDead", new DiagnosticCriterion(CheckDead));
	}

	public DiagnosticResult CheckCapacity()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		int num = 5;
		IList<UtilityNetwork> networks = Game.Instance.electricalConduitSystem.GetNetworks();
		foreach (ElectricalUtilityNetwork item in networks)
		{
			if (item.allWires == null || item.allWires.Count == 0)
			{
				continue;
			}
			float num2 = 0f;
			int num3 = Grid.PosToCell(item.allWires[0]);
			if (Grid.WorldIdx[num3] != base.worldID)
			{
				continue;
			}
			ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num3);
			List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
			if (batteriesOnCircuit == null || batteriesOnCircuit.Count == 0)
			{
				continue;
			}
			foreach (Battery item2 in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NORMAL;
				num2 += item2.capacity;
			}
			if (num2 < Game.Instance.circuitManager.GetWattsUsedByCircuit(circuitID) * (float)num)
			{
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.LIMITED_CAPACITY;
				Battery battery = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID)[0];
				if (battery != null)
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(battery.transform.position, battery.gameObject);
				}
			}
		}
		return result;
	}

	public DiagnosticResult CheckDead()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		IList<UtilityNetwork> networks = Game.Instance.electricalConduitSystem.GetNetworks();
		foreach (ElectricalUtilityNetwork item in networks)
		{
			if (item.allWires == null || item.allWires.Count == 0)
			{
				continue;
			}
			int num = Grid.PosToCell(item.allWires[0]);
			if (Grid.WorldIdx[num] != base.worldID)
			{
				continue;
			}
			ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num);
			List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
			if (batteriesOnCircuit == null || batteriesOnCircuit.Count == 0)
			{
				continue;
			}
			foreach (Battery item2 in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				if (!ColonyDiagnosticUtility.PastNewBuildingGracePeriod(item2.transform) || item2.CircuitID == ushort.MaxValue || item2.JoulesAvailable != 0f)
				{
					continue;
				}
				result.opinion = DiagnosticResult.Opinion.Concern;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.DEAD_BATTERY;
				result.clickThroughTarget = new Tuple<Vector3, GameObject>(item2.transform.position, item2.gameObject);
				break;
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
}
