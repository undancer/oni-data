using System;
using UnityEngine;

public class FoldOutPanel : KMonoBehaviour
{
	private bool panelOpen = true;

	public GameObject container;

	public bool startOpen = true;

	protected override void OnSpawn()
	{
		MultiToggle componentInChildren = GetComponentInChildren<MultiToggle>();
		componentInChildren.onClick = (System.Action)Delegate.Combine(componentInChildren.onClick, new System.Action(OnClick));
		ToggleOpen(startOpen);
	}

	private void OnClick()
	{
		ToggleOpen(!panelOpen);
	}

	private void ToggleOpen(bool open)
	{
		panelOpen = open;
		container.SetActive(panelOpen);
		GetComponentInChildren<MultiToggle>().ChangeState(panelOpen ? 1 : 0);
	}
}
