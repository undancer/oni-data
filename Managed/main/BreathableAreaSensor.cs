public class BreathableAreaSensor : Sensor
{
	private bool isBreathable;

	private OxygenBreather breather;

	public BreathableAreaSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		if (breather == null)
		{
			breather = GetComponent<OxygenBreather>();
		}
		bool flag = isBreathable;
		isBreathable = breather.IsBreathableElement;
		if (isBreathable != flag)
		{
			if (isBreathable)
			{
				Trigger(99949694);
			}
			else
			{
				Trigger(-1189351068);
			}
		}
	}

	public bool IsBreathable()
	{
		return isBreathable;
	}

	public bool IsUnderwater()
	{
		return breather.IsUnderLiquid;
	}
}
