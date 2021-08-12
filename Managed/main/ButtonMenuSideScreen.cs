using System.Collections.Generic;
using UnityEngine;

public class ButtonMenuSideScreen : SideScreenContent
{
	public const int DefaultButtonMenuSideScreenSortOrder = 20;

	public GameObject buttonPrefab;

	public RectTransform buttonContainer;

	private List<GameObject> liveButtons = new List<GameObject>();

	private List<ISidescreenButtonControl> targets;

	public override bool IsValidForTarget(GameObject target)
	{
		ISidescreenButtonControl sidescreenButtonControl = target.GetComponent<ISidescreenButtonControl>();
		if (sidescreenButtonControl == null)
		{
			sidescreenButtonControl = target.GetSMI<ISidescreenButtonControl>();
		}
		return sidescreenButtonControl?.SidescreenEnabled() ?? false;
	}

	public override int GetSideScreenSortOrder()
	{
		if (targets == null)
		{
			return 20;
		}
		return targets[0].ButtonSideScreenSortOrder();
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		targets = new_target.GetAllSMI<ISidescreenButtonControl>();
		targets.AddRange(new_target.GetComponents<ISidescreenButtonControl>());
		Refresh();
	}

	private void Refresh()
	{
		while (liveButtons.Count < targets.Count)
		{
			liveButtons.Add(Util.KInstantiateUI(buttonPrefab, buttonContainer.gameObject, force_active: true));
		}
		for (int i = 0; i < liveButtons.Count; i++)
		{
			if (i >= targets.Count)
			{
				liveButtons[i].SetActive(value: false);
				continue;
			}
			if (!liveButtons[i].activeSelf)
			{
				liveButtons[i].SetActive(value: true);
			}
			KButton componentInChildren = liveButtons[i].GetComponentInChildren<KButton>();
			ToolTip componentInChildren2 = liveButtons[i].GetComponentInChildren<ToolTip>();
			LocText componentInChildren3 = liveButtons[i].GetComponentInChildren<LocText>();
			componentInChildren.isInteractable = targets[i].SidescreenButtonInteractable();
			componentInChildren.ClearOnClick();
			componentInChildren.onClick += targets[i].OnSidescreenButtonPressed;
			componentInChildren.onClick += Refresh;
			componentInChildren3.SetText(targets[i].SidescreenButtonText);
			componentInChildren2.SetSimpleTooltip(targets[i].SidescreenButtonTooltip);
		}
	}
}
