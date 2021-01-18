using UnityEngine;

public class CritterSensorSideScreen : SideScreenContent
{
	public LogicCritterCountSensor targetSensor;

	public KToggle countCrittersToggle;

	public KToggle countEggsToggle;

	public KImage crittersCheckmark;

	public KImage eggsCheckmark;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		countCrittersToggle.onClick += ToggleCritters;
		countEggsToggle.onClick += ToggleEggs;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicCritterCountSensor>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetSensor = target.GetComponent<LogicCritterCountSensor>();
		crittersCheckmark.enabled = targetSensor.countCritters;
		eggsCheckmark.enabled = targetSensor.countEggs;
	}

	private void ToggleCritters()
	{
		targetSensor.countCritters = !targetSensor.countCritters;
		crittersCheckmark.enabled = targetSensor.countCritters;
	}

	private void ToggleEggs()
	{
		targetSensor.countEggs = !targetSensor.countEggs;
		eggsCheckmark.enabled = targetSensor.countEggs;
	}
}
