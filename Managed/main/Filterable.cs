using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Filterable")]
public class Filterable : KMonoBehaviour
{
	public enum ElementState
	{
		None,
		Solid,
		Liquid,
		Gas
	}

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	public ElementState filterElementState = ElementState.None;

	[Serialize]
	private Tag selectedTag = GameTags.Void;

	private static TagSet filterableCategories = new TagSet(GameTags.CalorieCategories, GameTags.UnitCategories, GameTags.MaterialCategories, GameTags.MaterialBuildingElements);

	private static readonly Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<Filterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Filterable>(delegate(Filterable component, object data)
	{
		component.OnCopySettings(data);
	});

	public Tag SelectedTag
	{
		get
		{
			return selectedTag;
		}
		set
		{
			selectedTag = value;
			OnFilterChanged();
		}
	}

	public event Action<Tag> onFilterChanged;

	public Dictionary<Tag, HashSet<Tag>> GetTagOptions()
	{
		Dictionary<Tag, HashSet<Tag>> dictionary = new Dictionary<Tag, HashSet<Tag>>();
		if (filterElementState == ElementState.Solid)
		{
			dictionary = DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(filterableCategories);
		}
		else
		{
			foreach (Element element in ElementLoader.elements)
			{
				if (!element.disabled && ((element.IsGas && filterElementState == ElementState.Gas) || (element.IsLiquid && filterElementState == ElementState.Liquid)))
				{
					Tag materialCategoryTag = element.GetMaterialCategoryTag();
					if (!dictionary.ContainsKey(materialCategoryTag))
					{
						dictionary[materialCategoryTag] = new HashSet<Tag>();
					}
					Tag item = GameTagExtensions.Create(element.id);
					dictionary[materialCategoryTag].Add(item);
				}
			}
		}
		dictionary.Add(GameTags.Void, new HashSet<Tag>
		{
			GameTags.Void
		});
		return dictionary;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Filterable component = gameObject.GetComponent<Filterable>();
		if (component != null)
		{
			SelectedTag = component.SelectedTag;
		}
	}

	protected override void OnSpawn()
	{
		OnFilterChanged();
	}

	private void OnFilterChanged()
	{
		if (this.onFilterChanged != null)
		{
			this.onFilterChanged(selectedTag);
		}
		Operational component = GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(filterSelected, selectedTag.IsValid);
		}
	}
}
