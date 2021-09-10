using STRINGS;
using UnityEngine;

public class SelfDestructButtonSideScreen : SideScreenContent
{
	public KButton button;

	public LocText statusText;

	private CraftModuleInterface craftInterface;

	private bool acknowledgeWarnings;

	private static readonly EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen> TagsChangedDelegate = new EventSystem.IntraObjectHandler<SelfDestructButtonSideScreen>(delegate(SelfDestructButtonSideScreen cmp, object data)
	{
		cmp.OnTagsChanged(data);
	});

	protected override void OnSpawn()
	{
		Refresh();
		button.onClick += TriggerDestruct;
	}

	public override int GetSideScreenSortOrder()
	{
		return -150;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (target.GetComponent<CraftModuleInterface>() != null)
		{
			return target.HasTag(GameTags.RocketInSpace);
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		craftInterface = target.GetComponent<CraftModuleInterface>();
		acknowledgeWarnings = false;
		craftInterface.Subscribe(-1582839653, TagsChangedDelegate);
		Refresh();
	}

	public override void ClearTarget()
	{
		if (craftInterface != null)
		{
			craftInterface.Unsubscribe(-1582839653, TagsChangedDelegate);
			craftInterface = null;
		}
	}

	private void OnTagsChanged(object data)
	{
		if (((TagChangedEventData)data).tag == GameTags.RocketStranded)
		{
			Refresh();
		}
	}

	private void TriggerDestruct()
	{
		if (acknowledgeWarnings)
		{
			craftInterface.gameObject.Trigger(-1061799784);
			acknowledgeWarnings = false;
		}
		else
		{
			acknowledgeWarnings = true;
		}
		Refresh();
	}

	private void Refresh()
	{
		if (!(craftInterface == null))
		{
			statusText.text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.MESSAGE_TEXT;
			if (acknowledgeWarnings)
			{
				button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT_CONFIRM;
				button.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP_CONFIRM;
			}
			else
			{
				button.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TEXT;
				button.GetComponentInChildren<ToolTip>().toolTip = UI.UISIDESCREENS.SELFDESTRUCTSIDESCREEN.BUTTON_TOOLTIP;
			}
		}
	}
}
