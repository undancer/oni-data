using STRINGS;

public class DevRadiationEmitter : KMonoBehaviour, ISingleSliderControl, ISliderControl
{
	[MyCmpReq]
	private RadiationEmitter radiationEmitter;

	public string SliderTitleKey => BUILDINGS.PREFABS.DEVRADIATIONGENERATOR.NAME;

	public string SliderUnits => UI.UNITSUFFIXES.RADIATION.RADS;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (radiationEmitter != null)
		{
			radiationEmitter.SetEmitting(emitting: true);
		}
	}

	public float GetSliderMax(int index)
	{
		return 5000f;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public string GetSliderTooltip()
	{
		return "";
	}

	public string GetSliderTooltipKey(int index)
	{
		return "";
	}

	public float GetSliderValue(int index)
	{
		return radiationEmitter.emitRads;
	}

	public void SetSliderValue(float value, int index)
	{
		radiationEmitter.emitRads = value;
		radiationEmitter.Refresh();
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}
}
