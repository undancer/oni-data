using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen<EntryType> : KScreen where EntryType : CrewListEntry
{
	public GameObject Prefab_CrewEntry;

	public List<EntryType> EntryObjects = new List<EntryType>();

	public Transform ScrollRectTransform;

	public Transform EntriesPanelTransform;

	protected Vector2 EntryRectSize = new Vector2(750f, 64f);

	public int maxEntriesBeforeScroll = 5;

	public Scrollbar PanelScrollbar;

	protected ToggleGroup sortToggleGroup;

	protected Toggle lastSortToggle;

	protected bool lastSortReversed;

	public GameObject Prefab_ColumnTitle;

	public Transform ColumnTitlesContainer;

	public bool autoColumn;

	public float columnTitleHorizontalOffset;

	protected override void OnActivate()
	{
		base.OnActivate();
		ClearEntries();
		SpawnEntries();
		PositionColumnTitles();
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		base.ConsumeMouseScroll = true;
	}

	protected override void OnCmpEnable()
	{
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		Reconstruct();
	}

	private void ClearEntries()
	{
		for (int num = EntryObjects.Count - 1; num > -1; num--)
		{
			Util.KDestroyGameObject(EntryObjects[num]);
		}
		EntryObjects.Clear();
	}

	protected void RefreshCrewPortraitContent()
	{
		EntryObjects.ForEach(delegate(EntryType eo)
		{
			eo.RefreshCrewPortraitContent();
		});
	}

	protected virtual void SpawnEntries()
	{
		if (EntryObjects.Count != 0)
		{
			ClearEntries();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (autoColumn)
		{
			UpdateColumnTitles();
		}
		bool flag = false;
		List<MinionIdentity> liveIdentities = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		if (EntryObjects.Count != liveIdentities.Count || EntryObjects.FindAll((EntryType o) => liveIdentities.Contains(o.Identity)).Count != EntryObjects.Count)
		{
			flag = true;
		}
		if (flag)
		{
			Reconstruct();
		}
		UpdateScroll();
	}

	public void Reconstruct()
	{
		ClearEntries();
		SpawnEntries();
	}

	private void UpdateScroll()
	{
		if ((bool)PanelScrollbar)
		{
			if (EntryObjects.Count <= maxEntriesBeforeScroll)
			{
				PanelScrollbar.value = Mathf.Lerp(PanelScrollbar.value, 1f, 10f);
				PanelScrollbar.gameObject.SetActive(value: false);
			}
			else
			{
				PanelScrollbar.gameObject.SetActive(value: true);
			}
		}
	}

	private void SetHeadersActive(bool state)
	{
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			ColumnTitlesContainer.GetChild(i).gameObject.SetActive(state);
		}
	}

	protected virtual void PositionColumnTitles()
	{
		if (ColumnTitlesContainer == null)
		{
			return;
		}
		if (EntryObjects.Count <= 0)
		{
			SetHeadersActive(state: false);
			return;
		}
		SetHeadersActive(state: true);
		int childCount = EntryObjects[0].transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			OverviewColumnIdentity component = EntryObjects[0].transform.GetChild(i).GetComponent<OverviewColumnIdentity>();
			if (component != null)
			{
				GameObject gameObject = Util.KInstantiate(Prefab_ColumnTitle);
				gameObject.name = component.Column_DisplayName;
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				gameObject.transform.SetParent(ColumnTitlesContainer);
				componentInChildren.text = (component.StringLookup ? ((string)Strings.Get(component.Column_DisplayName)) : component.Column_DisplayName);
				gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.SORTCOLUMN, componentInChildren.text);
				gameObject.rectTransform().anchoredPosition = new Vector2(component.rectTransform().anchoredPosition.x, 0f);
				OverviewColumnIdentity overviewColumnIdentity = gameObject.GetComponent<OverviewColumnIdentity>();
				if (overviewColumnIdentity == null)
				{
					overviewColumnIdentity = gameObject.AddComponent<OverviewColumnIdentity>();
				}
				overviewColumnIdentity.Column_DisplayName = component.Column_DisplayName;
				overviewColumnIdentity.columnID = component.columnID;
				overviewColumnIdentity.xPivot = component.xPivot;
				overviewColumnIdentity.Sortable = component.Sortable;
				if (overviewColumnIdentity.Sortable)
				{
					overviewColumnIdentity.GetComponentInChildren<ImageToggleState>(includeInactive: true).gameObject.SetActive(value: true);
				}
			}
		}
		UpdateColumnTitles();
		sortToggleGroup = base.gameObject.AddComponent<ToggleGroup>();
		sortToggleGroup.allowSwitchOff = true;
	}

	protected void SortByName(bool reverse)
	{
		List<EntryType> list = new List<EntryType>(EntryObjects);
		list.Sort(delegate(EntryType a, EntryType b)
		{
			string text = a.Identity.GetProperName() + a.gameObject.GetInstanceID();
			string strB = b.Identity.GetProperName() + b.gameObject.GetInstanceID();
			return text.CompareTo(strB);
		});
		ReorderEntries(list, reverse);
	}

	protected void UpdateColumnTitles()
	{
		if (EntryObjects.Count <= 0 || !EntryObjects[0].gameObject.activeSelf)
		{
			SetHeadersActive(state: false);
			return;
		}
		SetHeadersActive(state: true);
		for (int i = 0; i < ColumnTitlesContainer.childCount; i++)
		{
			RectTransform rectTransform = ColumnTitlesContainer.GetChild(i).rectTransform();
			for (int j = 0; j < EntryObjects[0].transform.childCount; j++)
			{
				OverviewColumnIdentity component = EntryObjects[0].transform.GetChild(j).GetComponent<OverviewColumnIdentity>();
				if (component != null && component.Column_DisplayName == rectTransform.name)
				{
					rectTransform.pivot = new Vector2(component.xPivot, rectTransform.pivot.y);
					rectTransform.anchoredPosition = new Vector2(component.rectTransform().anchoredPosition.x + columnTitleHorizontalOffset, 0f);
					rectTransform.sizeDelta = new Vector2(component.rectTransform().sizeDelta.x, rectTransform.sizeDelta.y);
					if (rectTransform.anchoredPosition.x == 0f)
					{
						rectTransform.gameObject.SetActive(value: false);
					}
					else
					{
						rectTransform.gameObject.SetActive(value: true);
					}
				}
			}
		}
	}

	protected void ReorderEntries(List<EntryType> sortedEntries, bool reverse)
	{
		for (int i = 0; i < sortedEntries.Count; i++)
		{
			if (reverse)
			{
				sortedEntries[i].transform.SetSiblingIndex(sortedEntries.Count - 1 - i);
			}
			else
			{
				sortedEntries[i].transform.SetSiblingIndex(i);
			}
		}
	}
}
