using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public MultiToggle continuousToggle;

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<EggIncubator>() != null;
	}

	protected override void SetResultDescriptions(GameObject go)
	{
		string text = "";
		InfoDescription component = go.GetComponent<InfoDescription>();
		if ((bool)component)
		{
			text += component.description;
		}
		descriptionLabel.SetText(text);
	}

	protected override bool RequiresAvailableAmountToDeposit()
	{
		return false;
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		return Def.GetUISprite(Assets.GetPrefab(prefabTag)).first;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		EggIncubator incubator = target.GetComponent<EggIncubator>();
		continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		continuousToggle.onClick = delegate
		{
			incubator.autoReplaceEntity = !incubator.autoReplaceEntity;
			continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		};
	}
}
