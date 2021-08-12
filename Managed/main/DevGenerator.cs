using UnityEngine;

public class DevGenerator : Generator
{
	public float wattageRating = 100000f;

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != ushort.MaxValue);
		if (operational.IsOperational)
		{
			float num = wattageRating;
			if (num > 0f)
			{
				num *= dt;
				num = Mathf.Max(num, 1f * dt);
				GenerateJoules(num);
			}
		}
	}
}
