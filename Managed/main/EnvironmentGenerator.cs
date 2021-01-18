public class EnvironmentGenerator : Generator
{
	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (operational.IsOperational)
		{
			ApplyDeltaJoules(base.WattageRating * dt);
			operational.SetActive(operational.IsOperational);
		}
	}
}
