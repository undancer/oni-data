using UnityEngine;

public class ProgressBarSideScreen : SideScreenContent, IRender1000ms
{
	public LocText label;

	public GenericUIProgressBar progressBar;

	public IProgressBarSideScreen targetObject;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IProgressBarSideScreen>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetObject = target.GetComponent<IProgressBarSideScreen>();
		RefreshBar();
	}

	private void RefreshBar()
	{
		progressBar.SetMaxValue(targetObject.GetProgressBarMaxValue());
		progressBar.SetFillPercentage(targetObject.GetProgressBarFillPercentage());
		progressBar.label.SetText(targetObject.GetProgressBarLabel());
		label.SetText(targetObject.GetProgressBarTitleLabel());
		ToolTip componentInChildren = progressBar.GetComponentInChildren<ToolTip>();
		componentInChildren.SetSimpleTooltip(targetObject.GetProgressBarTooltip());
	}

	public void Render1000ms(float dt)
	{
		RefreshBar();
	}
}
