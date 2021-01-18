using UnityEngine;

public class MinionVitalsScreen : TargetScreen
{
	public MinionVitalsPanel panel;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MinionIdentity>();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
	}

	public override void OnSelectTarget(GameObject target)
	{
		panel.selectedEntity = target;
		panel.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (panel == null)
		{
			panel = GetComponent<MinionVitalsPanel>();
		}
		panel.Init();
	}
}
