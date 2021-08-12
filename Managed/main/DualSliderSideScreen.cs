using System.Collections.Generic;
using UnityEngine;

public class DualSliderSideScreen : SideScreenContent
{
	private IDualSliderControl target;

	public List<SliderSet> sliderSets;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < sliderSets.Count; i++)
		{
			sliderSets[i].SetupSlider(i);
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IDualSliderControl>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<IDualSliderControl>();
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
