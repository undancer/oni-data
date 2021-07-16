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
		foreach (ElectricalUtilityNetwork network in Game.Instance.electricalConduitSystem.GetNetworks())
		{
			if (network.allWires == null || network.allWires.Count == 0)
			{
				continue;
			}
			int num2 = Grid.PosToCell(network.allWires[0]);
			if (Grid.WorldIdx[num2] != base.WorldID)
			{
				continue;
			}
			ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num2);
			foreach (Battery item in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
			{
				num += item.JoulesAvailable;
			}
		}
		AddPoint(Mathf.Round(num));
	}

	public override string FormatValueString(float value)
	{
		return GameUtil.GetFormattedJoules(value);
	}
}
