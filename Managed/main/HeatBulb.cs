using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HeatBulb")]
public class HeatBulb : KMonoBehaviour, ISim200ms
{
	[SerializeField]
	private float minTemperature;

	[SerializeField]
	private float kjConsumptionRate;

	[SerializeField]
	private float lightKJConsumptionRate;

	[SerializeField]
	private Vector2I minCheckOffset;

	[SerializeField]
	private Vector2I maxCheckOffset;

	[MyCmpGet]
	private Light2D lightSource;

	[MyCmpGet]
	private KBatchedAnimController kanim;

	[Serialize]
	private float kjConsumed;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		kanim.Play("off");
	}

	public void Sim200ms(float dt)
	{
		float num = kjConsumptionRate * dt;
		Vector2I vector2I = maxCheckOffset - minCheckOffset + 1;
		int num2 = vector2I.x * vector2I.y;
		float num3 = num / (float)num2;
		Grid.PosToXY(base.transform.GetPosition(), out var x, out var y);
		for (int i = minCheckOffset.y; i <= maxCheckOffset.y; i++)
		{
			for (int j = minCheckOffset.x; j <= maxCheckOffset.x; j++)
			{
				int num4 = Grid.XYToCell(x + j, y + i);
				if (Grid.IsValidCell(num4) && Grid.Temperature[num4] > minTemperature)
				{
					kjConsumed += num3;
					SimMessages.ModifyEnergy(num4, 0f - num3, 5000f, SimMessages.EnergySourceID.HeatBulb);
				}
			}
		}
		float num5 = lightKJConsumptionRate * dt;
		if (kjConsumed > num5)
		{
			if (!lightSource.enabled)
			{
				kanim.Play("open");
				kanim.Queue("on");
				lightSource.enabled = true;
			}
			kjConsumed -= num5;
		}
		else
		{
			if (lightSource.enabled)
			{
				kanim.Play("close");
				kanim.Queue("off");
			}
			lightSource.enabled = false;
		}
	}
}
