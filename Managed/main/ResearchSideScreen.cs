using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSideScreen : SideScreenContent
{
	public KButton selectResearchButton;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;

	public ResearchSideScreen()
	{
		refreshDisplayStateDelegate = RefreshDisplayState;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectResearchButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleResearch();
		};
		Research.Instance.Subscribe(-1914338957, refreshDisplayStateDelegate);
		Research.Instance.Subscribe(-125623018, refreshDisplayStateDelegate);
		RefreshDisplayState();
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshDisplayState();
		target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
		target.gameObject.Subscribe(-1852328367, refreshDisplayStateDelegate);
		target.gameObject.Subscribe(-592767678, refreshDisplayStateDelegate);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)target)
		{
			target.gameObject.Unsubscribe(-1852328367, refreshDisplayStateDelegate);
			target.gameObject.Unsubscribe(187661686, refreshDisplayStateDelegate);
			target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, refreshDisplayStateDelegate);
		Research.Instance.Unsubscribe(-125623018, refreshDisplayStateDelegate);
		if ((bool)target)
		{
			target.gameObject.Unsubscribe(-1852328367, refreshDisplayStateDelegate);
			target.gameObject.Unsubscribe(187661686, refreshDisplayStateDelegate);
			target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (!(target.GetComponent<ResearchCenter>() != null))
		{
			return target.GetComponent<NuclearResearchCenter>() != null;
		}
		return true;
	}

	private void RefreshDisplayState(object data = null)
	{
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		string text = "";
		ResearchCenter component = SelectTool.Instance.selected.GetComponent<ResearchCenter>();
		NuclearResearchCenter component2 = SelectTool.Instance.selected.GetComponent<NuclearResearchCenter>();
		if (component != null)
		{
			text = component.research_point_type_id;
		}
		if (component2 != null)
		{
			text = component2.researchTypeID;
		}
		if (component == null && component2 == null)
		{
			return;
		}
		researchButtonIcon.sprite = Research.Instance.researchTypes.GetResearchType(text).sprite;
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch == null)
		{
			DescriptionText.text = string.Concat("<b>", UI.UISIDESCREENS.RESEARCHSIDESCREEN.NOSELECTEDRESEARCH, "</b>");
			return;
		}
		string text2 = "";
		if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(text) || activeResearch.tech.costsByResearchTypeID[text] <= 0f)
		{
			text2 += "<color=#7f7f7f>";
		}
		text2 = text2 + "<b>" + activeResearch.tech.Name + "</b>";
		if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(text) || activeResearch.tech.costsByResearchTypeID[text] <= 0f)
		{
			text2 += "</color>";
		}
		foreach (KeyValuePair<string, float> item in activeResearch.tech.costsByResearchTypeID)
		{
			if (item.Value != 0f)
			{
				bool flag = item.Key == text;
				text2 += "\n   ";
				text2 += "<b>";
				if (!flag)
				{
					text2 += "<color=#7f7f7f>";
				}
				text2 = text2 + "- " + Research.Instance.researchTypes.GetResearchType(item.Key).name + ": " + activeResearch.progressInventory.PointsByTypeID[item.Key] + "/" + activeResearch.tech.costsByResearchTypeID[item.Key];
				if (!flag)
				{
					text2 += "</color>";
				}
				text2 += "</b>";
			}
		}
		DescriptionText.text = text2;
	}
}
