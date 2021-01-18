using System.Collections.Generic;
using UnityEngine;

public class BatteryTracker : WorldTracker
{
	public BatteryTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		IList<UtilityNetwork> networks = Game.Instance.electricalConduitSystem.GetNetworks();
		foreach (ElectricalUtilityNetwork item in networks)
		{
			if (item.allWires == null || item.allWires.Count == 0)
			{
				continue;
			}
			int num2 = Grid.PosToCell(item.allWires[0]);
			if (Grid.WorldIdx[num2] != base.WorldID)
			{
				continue;
			}
			ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num2);
			foreach (Battery item2 in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				num += item2.JoulesAvailable;
			}
		}
		AddPoint(Mathf.Round(num));
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedJoules(value);
	}
}
