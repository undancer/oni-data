using System.Collections.Generic;
using UnityEngine;

public class SingleSliderSideScreen : SideScreenContent
{
	private ISingleSliderControl target;

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
		KPrefabID component = target.GetComponent<KPrefabID>();
		if (target.GetComponent<ISingleSliderControl>() != null && !component.HasTag("HydrogenGenerator".ToTag()) && !component.HasTag("MethaneGenerator".ToTag()) && !component.HasTag("PetroleumGenerator".ToTag()) && !component.HasTag("DevGenerator".ToTag()))
		{
			return !component.HasTag(GameTags.DeadReactor);
		}
		return false;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<ISingleSliderControl>();
		if (target == null)
		{
			Debug.LogError("The gameObject received does not contain a ISingleSliderControl implementation");
			return;
		}
		titleKey = target.SliderTitleKey;
		for (int i = 0; i < sliderSets.Count; i++)
		{
			sliderSets[i].SetTarget(target);
		}
	}
}
