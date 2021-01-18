using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SideScreenContent
{
	public HierarchyReferences categoryFoldoutPrefab;

	public FilterSideScreenRow elementEntryPrefab;

	public RectTransform elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText outputElementHeaderLabel;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	public LocText currentSelectionLabel;

	private static TagNameComparer comparer = new TagNameComparer(GameTags.Void);

	public Dictionary<Tag, HierarchyReferences> categoryToggles = new Dictionary<Tag, HierarchyReferences>();

	public SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>> filterRowMap = new SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>>(comparer);

	public bool isLogicFilter;

	private Filterable targetFilterable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		filterRowMap.Clear();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		bool flag = false;
		if ((!isLogicFilter) ? (target.GetComponent<ElementFilter>() != null) : (target.GetComponent<ConduitElementSensor>() != null || target.GetComponent<LogicElementSensor>() != null))
		{
			return target.GetComponent<Filterable>() != null;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetFilterable = target.GetComponent<Filterable>();
		if (!(targetFilterable == null))
		{
			switch (targetFilterable.filterElementState)
			{
			case Filterable.ElementState.Solid:
				everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.SOLID;
				break;
			case Filterable.ElementState.Gas:
				everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS;
				break;
			default:
				everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID;
				break;
			}
			Configure(targetFilterable);
			SetFilterTag(targetFilterable.SelectedTag);
		}
	}

	private void ToggleCategory(Tag tag, bool forceOn = false)
	{
		HierarchyReferences hierarchyReferences = categoryToggles[tag];
		if (hierarchyReferences != null)
		{
			MultiToggle reference = hierarchyReferences.GetReference<MultiToggle>("Toggle");
			if (!forceOn)
			{
				reference.NextState();
			}
			else
			{
				reference.ChangeState(1);
			}
			hierarchyReferences.GetReference<RectTransform>("Entries").gameObject.SetActive(reference.CurrentState != 0);
		}
	}

	private void Configure(Filterable filterable)
	{
		Dictionary<Tag, HashSet<Tag>> tagOptions = filterable.GetTagOptions();
		foreach (KeyValuePair<Tag, HashSet<Tag>> category_tags in tagOptions)
		{
			if (!filterRowMap.ContainsKey(category_tags.Key))
			{
				if (category_tags.Key != GameTags.Void)
				{
					HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(categoryFoldoutPrefab.gameObject, elementEntryContainer.gameObject);
					hierarchyReferences.GetReference<LocText>("Label").text = category_tags.Key.ProperName();
					hierarchyReferences.GetReference<MultiToggle>("Toggle").onClick = delegate
					{
						ToggleCategory(category_tags.Key);
					};
					categoryToggles.Add(category_tags.Key, hierarchyReferences);
				}
				filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>(comparer);
			}
			else if (category_tags.Key == GameTags.Void && !filterRowMap.ContainsKey(category_tags.Key))
			{
				filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>(comparer);
			}
			foreach (Tag item in category_tags.Value)
			{
				if (!filterRowMap[category_tags.Key].ContainsKey(item))
				{
					RectTransform rectTransform = ((category_tags.Key != GameTags.Void) ? categoryToggles[category_tags.Key].GetReference<RectTransform>("Entries") : elementEntryContainer);
					FilterSideScreenRow row = Util.KInstantiateUI<FilterSideScreenRow>(elementEntryPrefab.gameObject, rectTransform.gameObject);
					row.SetTag(item);
					row.button.onClick += delegate
					{
						SetFilterTag(row.tag);
					};
					filterRowMap[category_tags.Key].Add(row.tag, row);
				}
			}
		}
		int num = 0;
		filterRowMap[GameTags.Void][GameTags.Void].transform.SetSiblingIndex(num++);
		foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> item2 in filterRowMap)
		{
			if (tagOptions.ContainsKey(item2.Key) && tagOptions[item2.Key].Count > 0)
			{
				if (item2.Key != GameTags.Void)
				{
					categoryToggles[item2.Key].name = "CATE " + num;
					categoryToggles[item2.Key].transform.SetSiblingIndex(num++);
					categoryToggles[item2.Key].gameObject.SetActive(value: true);
				}
				int num2 = 0;
				foreach (KeyValuePair<Tag, FilterSideScreenRow> item3 in item2.Value)
				{
					item3.Value.name = "ELE " + num2;
					item3.Value.transform.SetSiblingIndex(num2++);
					item3.Value.gameObject.SetActive(tagOptions[item2.Key].Contains(item3.Value.tag));
					if (item3.Key != GameTags.Void && item3.Key == targetFilterable.SelectedTag)
					{
						ToggleCategory(item2.Key, forceOn: true);
					}
				}
			}
			else if (item2.Key != GameTags.Void)
			{
				categoryToggles[item2.Key].gameObject.SetActive(value: false);
			}
		}
		RefreshUI();
	}

	private void SetFilterTag(Tag tag)
	{
		if (!(targetFilterable == null))
		{
			if (tag.IsValid)
			{
				targetFilterable.SelectedTag = tag;
			}
			RefreshUI();
		}
	}

	private void RefreshUI()
	{
		LocString loc_string = targetFilterable.filterElementState switch
		{
			Filterable.ElementState.Solid => UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.SOLID, 
			Filterable.ElementState.Gas => UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS, 
			_ => UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID, 
		};
		currentSelectionLabel.text = string.Format(loc_string, UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
		foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> item in filterRowMap)
		{
			foreach (KeyValuePair<Tag, FilterSideScreenRow> item2 in item.Value)
			{
				bool flag = item2.Key == targetFilterable.SelectedTag;
				item2.Value.SetSelected(flag);
				if (flag)
				{
					if (item2.Value.tag != GameTags.Void)
					{
						currentSelectionLabel.text = string.Format(loc_string, targetFilterable.SelectedTag.ProperName());
					}
					else
					{
						currentSelectionLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
					}
				}
			}
		}
	}
}
