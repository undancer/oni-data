using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantRadiationMonitor : KMonoBehaviour, ISlicedSim1000ms
{
	public const float ABSORBTION_FACTOR = 1f;

	public const float MAX_MUTATION_PROBABILITY = 0.33f;

	public const float MAX_RADIATION = 500f;

	[Serialize]
	public float totalRadiationExposure;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SlicedUpdaterSim1000ms<PlantRadiationMonitor>.instance.RegisterUpdate1000ms(this);
	}

	protected override void OnCleanUp()
	{
		SlicedUpdaterSim1000ms<PlantRadiationMonitor>.instance.UnregisterUpdate1000ms(this);
		base.OnCleanUp();
	}

	private void CheckRadiationLevel(float dt)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (Grid.IsValidCell(num))
		{
			totalRadiationExposure += Grid.Radiation[num] * 1f;
		}
	}

	public void SlicedSim1000ms(float dt)
	{
		CheckRadiationLevel(dt);
	}

	public bool ShouldMutate()
	{
		return false;
	}
}
