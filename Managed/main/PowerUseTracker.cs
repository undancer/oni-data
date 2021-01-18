using System.Collections.Generic;
using UnityEngine;

public class PowerUseTracker : WorldTracker
{
	public PowerUseTracker(int worldID)
		: base(worldID)
	{
	}

	public override void UpdateData()
	{
		float num = 0f;
		IList<UtilityNetwork> networks = Game.Instance.electricalConduitSystem.GetNetworks();
		foreach (ElectricalUtilityNetwork item in networks)
		{
			if (item.allWires != null && item.allWires.Count != 0)
			{
				int num2 = Grid.PosToCell(item.allWires[0]);
				if (Grid.WorldIdx[num2] == base.WorldID)
				{
					num += Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(num2));
				}
			}
		}
		AddPoint(Mathf.Round(num));
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedWattage(value);
	}
}
