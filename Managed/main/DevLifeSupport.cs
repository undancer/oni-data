using UnityEngine;

public class DevLifeSupport : KMonoBehaviour, ISim200ms
{
	[MyCmpReq]
	private ElementConsumer elementConsumer;

	public float targetTemperature = 303.15f;

	public int effectRadius = 7;

	private const float temperatureControlK = 0.2f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (elementConsumer != null)
		{
			elementConsumer.EnableConsumption(enabled: true);
		}
	}

	public void Sim200ms(float dt)
	{
		Vector2I vector2I = new Vector2I(-effectRadius, -effectRadius);
		Vector2I vector2I2 = new Vector2I(effectRadius, effectRadius);
		Grid.PosToXY(base.transform.GetPosition(), out var x, out var y);
		int num = Grid.XYToCell(x, y);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		int world = Grid.WorldIdx[num];
		for (int i = vector2I.y; i <= vector2I2.y; i++)
		{
			for (int j = vector2I.x; j <= vector2I2.x; j++)
			{
				int num2 = Grid.XYToCell(x + j, y + i);
				if (Grid.IsValidCellInWorld(num2, world))
				{
					float num3 = (targetTemperature - Grid.Temperature[num2]) * Grid.Element[num2].specificHeatCapacity * Grid.Mass[num2];
					if (!Mathf.Approximately(0f, num3))
					{
						SimMessages.ModifyEnergy(num2, num3 * 0.2f, 5000f, (num3 > 0f) ? SimMessages.EnergySourceID.DebugHeat : SimMessages.EnergySourceID.DebugCool);
					}
				}
			}
		}
	}
}
