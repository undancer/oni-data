using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LogicBitSelectorSideScreen : SideScreenContent, IRenderEveryTick
{
	private ILogicRibbonBitSelector target;

	public GameObject rowPrefab;

	public KImage inputDisplayIcon;

	public KImage outputDisplayIcon;

	public GameObject readerDescriptionContainer;

	public GameObject writerDescriptionContainer;

	[NonSerialized]
	public Dictionary<int, MultiToggle> toggles_by_int = new Dictionary<int, MultiToggle>();

	private Color activeColor;

	private Color inactiveColor;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activeColor = GlobalAssets.Instance.colorSet.logicOnText;
		inactiveColor = GlobalAssets.Instance.colorSet.logicOffText;
	}

	public void SelectToggle(int bit)
	{
		target.SetBitSelection(bit);
		target.UpdateVisuals();
		RefreshToggles();
	}

	private void RefreshToggles()
	{
		for (int j = 0; j < target.GetBitDepth(); j++)
		{
			int i = j;
			if (!toggles_by_int.ContainsKey(j))
			{
				GameObject gameObject = Util.KInstantiateUI(rowPrefab, rowPrefab.transform.parent.gameObject, force_active: true);
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("bitName").SetText(string.Format(UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.BIT, j + 1));
				gameObject.GetComponent<HierarchyReferences>().GetReference<KImage>("stateIcon").color = (target.IsBitActive(j) ? activeColor : inactiveColor);
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("stateText").SetText(target.IsBitActive(j) ? UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_ACTIVE : UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE);
				MultiToggle component = gameObject.GetComponent<MultiToggle>();
				toggles_by_int.Add(j, component);
			}
			toggles_by_int[j].onClick = delegate
			{
				SelectToggle(i);
			};
		}
		foreach (KeyValuePair<int, MultiToggle> item in toggles_by_int)
		{
			if (target.GetBitSelection() == item.Key)
			{
				item.Value.ChangeState(0);
			}
			else
			{
				item.Value.ChangeState(1);
			}
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ILogicRibbonBitSelector>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Debug.LogError("Invalid gameObject received");
			return;
		}
		target = new_target.GetComponent<ILogicRibbonBitSelector>();
		if (target == null)
		{
			Debug.LogError("The gameObject received is not an ILogicRibbonBitSelector");
			return;
		}
		titleKey = target.SideScreenTitle;
		readerDescriptionContainer.SetActive(target.SideScreenDisplayReaderDescription());
		writerDescriptionContainer.SetActive(target.SideScreenDisplayWriterDescription());
		RefreshToggles();
		UpdateInputOutputDisplay();
		foreach (KeyValuePair<int, MultiToggle> item in toggles_by_int)
		{
			UpdateStateVisuals(item.Key);
		}
	}

	public void RenderEveryTick(float dt)
	{
		if (target.Equals(null))
		{
			return;
		}
		foreach (KeyValuePair<int, MultiToggle> item in toggles_by_int)
		{
			UpdateStateVisuals(item.Key);
		}
		UpdateInputOutputDisplay();
	}

	private void UpdateInputOutputDisplay()
	{
		if (target.SideScreenDisplayReaderDescription())
		{
			outputDisplayIcon.color = ((target.GetOutputValue() > 0) ? activeColor : inactiveColor);
		}
		if (target.SideScreenDisplayWriterDescription())
		{
			inputDisplayIcon.color = ((target.GetInputValue() > 0) ? activeColor : inactiveColor);
		}
	}

	private void UpdateStateVisuals(int bit)
	{
		MultiToggle multiToggle = toggles_by_int[bit];
		multiToggle.gameObject.GetComponent<HierarchyReferences>().GetReference<KImage>("stateIcon").color = (target.IsBitActive(bit) ? activeColor : inactiveColor);
		multiToggle.gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("stateText").SetText(target.IsBitActive(bit) ? UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_ACTIVE : UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE);
	}
}
