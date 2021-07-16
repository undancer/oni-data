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
		AddCriterion("CheckCapacity", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.CRITERIA.CHECKCAPACITY, CheckCapacity));
		AddCriterion("CheckDead", new DiagnosticCriterion(UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.CRITERIA.CHECKDEAD, CheckDead));
	}

	public DiagnosticResult CheckCapacity()
	{
		DiagnosticResult result = new DiagnosticResult(DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
		int num = 5;
		foreach (ElectricalUtilityNetwork network in Game.Instance.electricalConduitSystem.GetNetworks())
		{
			if (network.allWires == null || network.allWires.Count == 0)
			{
				continue;
			}
			float num2 = 0f;
			int num3 = Grid.PosToCell(network.allWires[0]);
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
			foreach (Battery item in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NORMAL;
				num2 += item.capacity;
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
		foreach (ElectricalUtilityNetwork network in Game.Instance.electricalConduitSystem.GetNetworks())
		{
			if (network.allWires == null || network.allWires.Count == 0)
			{
				continue;
			}
			int num = Grid.PosToCell(network.allWires[0]);
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
			foreach (Battery item in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				if (ColonyDiagnosticUtility.PastNewBuildingGracePeriod(item.transform) && item.CircuitID != ushort.MaxValue && item.JoulesAvailable == 0f)
				{
					result.opinion = DiagnosticResult.Opinion.Concern;
					result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.DEAD_BATTERY;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.transform.position, item.gameObject);
					break;
				}
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
