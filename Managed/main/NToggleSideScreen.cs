using System.Collections.Generic;
using UnityEngine;

public class NToggleSideScreen : SideScreenContent
{
	[SerializeField]
	private KToggle buttonPrefab;

	[SerializeField]
	private LocText description;

	private INToggleSideScreenControl target;

	private List<KToggle> buttonList = new List<KToggle>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<INToggleSideScreenControl>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.target = target.GetComponent<INToggleSideScreenControl>();
		if (this.target != null)
		{
			titleKey = this.target.SidescreenTitleKey;
			base.gameObject.SetActive(value: true);
			Refresh();
		}
	}

	private void Refresh()
	{
		for (int i = 0; i < Mathf.Max(target.Options.Count, buttonList.Count); i++)
		{
			if (i >= target.Options.Count)
			{
				buttonList[i].gameObject.SetActive(value: false);
				continue;
			}
			if (i >= buttonList.Count)
			{
				KToggle kToggle = Util.KInstantiateUI<KToggle>(buttonPrefab.gameObject, ContentContainer);
				int idx = i;
				kToggle.onClick += delegate
				{
					target.QueueSelectedOption(idx);
					Refresh();
				};
				buttonList.Add(kToggle);
			}
			buttonList[i].GetComponentInChildren<LocText>().text = target.Options[i];
			buttonList[i].GetComponentInChildren<ToolTip>().toolTip = target.Tooltips[i];
			if (target.SelectedOption == i && target.QueuedOption == i)
			{
				buttonList[i].isOn = true;
				ImageToggleState[] componentsInChildren = buttonList[i].GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in componentsInChildren)
				{
					imageToggleState.SetActive();
				}
				buttonList[i].GetComponent<ImageToggleStateThrobber>().enabled = false;
			}
			else if (target.QueuedOption == i)
			{
				buttonList[i].isOn = true;
				ImageToggleState[] componentsInChildren2 = buttonList[i].GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState2 in componentsInChildren2)
				{
					imageToggleState2.SetActive();
				}
				buttonList[i].GetComponent<ImageToggleStateThrobber>().enabled = true;
			}
			else
			{
				buttonList[i].isOn = false;
				ImageToggleState[] componentsInChildren3 = buttonList[i].GetComponentsInChildren<ImageToggleState>();
				foreach (ImageToggleState imageToggleState3 in componentsInChildren3)
				{
					imageToggleState3.SetInactive();
					imageToggleState3.SetInactive();
				}
				buttonList[i].GetComponent<ImageToggleStateThrobber>().enabled = false;
			}
			buttonList[i].gameObject.SetActive(value: true);
		}
		description.text = target.Description;
		description.gameObject.SetActive(!string.IsNullOrEmpty(target.Description));
	}
}
