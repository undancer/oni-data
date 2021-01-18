using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TelescopeSideScreen : SideScreenContent
{
	public KButton selectStarmapScreen;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;

	public TelescopeSideScreen()
	{
		refreshDisplayStateDelegate = RefreshDisplayState;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectStarmapScreen.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		SpacecraftManager.instance.Subscribe(532901469, refreshDisplayStateDelegate);
		RefreshDisplayState();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshDisplayState();
		target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)target)
		{
			target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((bool)target)
		{
			target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Telescope>() != null;
	}

	private void RefreshDisplayState(object data = null)
	{
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		Telescope component = SelectTool.Instance.selected.GetComponent<Telescope>();
		if (!(component == null))
		{
			if (!SpacecraftManager.instance.HasAnalysisTarget())
			{
				DescriptionText.text = string.Concat("<b><color=#FF0000>", UI.UISIDESCREENS.TELESCOPESIDESCREEN.NO_SELECTED_ANALYSIS_TARGET, "</color></b>");
				return;
			}
			string text = UI.UISIDESCREENS.TELESCOPESIDESCREEN.ANALYSIS_TARGET_SELECTED;
			DescriptionText.text = text;
		}
	}
}
