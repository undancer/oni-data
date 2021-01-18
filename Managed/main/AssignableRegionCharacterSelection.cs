using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AssignableRegionCharacterSelection")]
public class AssignableRegionCharacterSelection : KMonoBehaviour
{
	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	private UIPool<KButton> buttonPool;

	private Dictionary<KButton, MinionIdentity> buttonIdentityMap = new Dictionary<KButton, MinionIdentity>();

	private List<CrewPortrait> portraitList = new List<CrewPortrait>();

	public event Action<MinionIdentity> OnDuplicantSelected;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		buttonPool = new UIPool<KButton>(buttonPrefab);
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		buttonPool.ClearAll();
		foreach (MinionIdentity item in Components.MinionIdentities.Items)
		{
			KButton btn = buttonPool.GetFreeElement(buttonParent, forceActive: true);
			CrewPortrait componentInChildren = btn.GetComponentInChildren<CrewPortrait>();
			componentInChildren.SetIdentityObject(item);
			portraitList.Add(componentInChildren);
			btn.ClearOnClick();
			btn.onClick += delegate
			{
				SelectDuplicant(btn);
			};
			buttonIdentityMap.Add(btn, item);
		}
	}

	public void Close()
	{
		buttonPool.DestroyAllActive();
		buttonIdentityMap.Clear();
		portraitList.Clear();
		base.gameObject.SetActive(value: false);
	}

	private void SelectDuplicant(KButton btn)
	{
		if (this.OnDuplicantSelected != null)
		{
			this.OnDuplicantSelected(buttonIdentityMap[btn]);
		}
		Close();
	}
}
