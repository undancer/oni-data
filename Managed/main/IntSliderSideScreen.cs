using System.Collections.Generic;
using UnityEngine;

public class IntSliderSideScreen : SideScreenContent
{
	private IIntSliderControl target;

	public List<SliderSet> sliderSets;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < sliderSets.Count; i++)
		{
			sliderSets[i].SetupSlider(i);
			sliderSets[i].valueSlider.wholeNumbers = true;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<IIntSliderControl>() == null)
		{
			return target.GetSMI<IIntSliderControl>() != null;
		}
		return true;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<IIntSliderControl>();
		if (target == null)
		{
			target = new_target.GetSMI<IIntSliderControl>();
		}
		if (target == null)
		{
			Debug.LogError("The gameObject received does not contain a Manual Generator component");
			return;
		}
		titleKey = target.SliderTitleKey;
		for (int i = 0; i < sliderSets.Count; i++)
		{
			sliderSets[i].SetTarget(target);
		}
	}
}
