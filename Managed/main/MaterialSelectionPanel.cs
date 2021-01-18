using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class MaterialSelectionPanel : KScreen
{
	public delegate void SelectElement(Element element, float kgAvailable, float recipe_amount);

	public struct SelectedElemInfo
	{
		public Tag element;

		public float kgAvailable;
	}

	public Dictionary<KToggle, Tag> ElementToggles = new Dictionary<KToggle, Tag>();

	private List<MaterialSelector> MaterialSelectors = new List<MaterialSelector>();

	private List<Tag> currentSelectedElements = new List<Tag>();

	[SerializeField]
	protected PriorityScreen priorityScreenPrefab;

	[SerializeField]
	protected GameObject priorityScreenParent;

	private PriorityScreen priorityScreen;

	public GameObject MaterialSelectorTemplate;

	public GameObject ResearchRequired;

	private Recipe activeRecipe;

	private static Dictionary<Tag, List<Tag>> elementsWithTag = new Dictionary<Tag, List<Tag>>();

	public Tag CurrentSelectedElement => MaterialSelectors[0].CurrentSelectedElement;

	public IList<Tag> GetSelectedElementAsList
	{
		get
		{
			currentSelectedElements.Clear();
			foreach (MaterialSelector materialSelector in MaterialSelectors)
			{
				if (materialSelector.gameObject.activeSelf)
				{
					Debug.Assert(materialSelector.CurrentSelectedElement != null);
					currentSelectedElements.Add(materialSelector.CurrentSelectedElement);
				}
			}
			return currentSelectedElements;
		}
	}

	public PriorityScreen PriorityScreen => priorityScreen;

	public static void ClearStatics()
	{
		elementsWithTag.Clear();
	}

	protected override void OnPrefabInit()
	{
		elementsWithTag.Clear();
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
		for (int i = 0; i < 3; i++)
		{
			MaterialSelector materialSelector = Util.KInstantiateUI<MaterialSelector>(MaterialSelectorTemplate, base.gameObject);
			materialSelector.selectorIndex = i;
			MaterialSelectors.Add(materialSelector);
		}
		MaterialSelectors[0].gameObject.SetActive(value: true);
		MaterialSelectorTemplate.SetActive(value: false);
		ResearchRequired.SetActive(value: false);
		priorityScreen = Util.KInstantiateUI<PriorityScreen>(priorityScreenPrefab.gameObject, priorityScreenParent);
		priorityScreen.InstantiateButtons(OnPriorityClicked);
		Game.Instance.Subscribe(-107300940, delegate
		{
			RefreshSelectors();
		});
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activateOnSpawn = true;
	}

	public void AddSelectAction(MaterialSelector.SelectMaterialActions action)
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = (MaterialSelector.SelectMaterialActions)Delegate.Combine(selector.selectMaterialActions, action);
		});
	}

	public void ClearSelectActions()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = null;
		});
	}

	public void ClearMaterialToggles()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.ClearMaterialToggles();
		});
	}

	public void ConfigureScreen(Recipe recipe)
	{
		activeRecipe = recipe;
		RefreshSelectors();
	}

	public bool AllSelectorsSelected()
	{
		foreach (MaterialSelector materialSelector in MaterialSelectors)
		{
			if (materialSelector.gameObject.activeInHierarchy && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public void RefreshSelectors()
	{
		if (activeRecipe == null)
		{
			return;
		}
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.gameObject.SetActive(value: false);
		});
		TechItem techItem = Db.Get().TechItems.TryGet(activeRecipe.GetBuildingDef().PrefabID);
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && techItem != null && !techItem.IsComplete())
		{
			ResearchRequired.SetActive(value: true);
			LocText[] componentsInChildren = ResearchRequired.GetComponentsInChildren<LocText>();
			componentsInChildren[0].text = UI.PRODUCTINFO_RESEARCHREQUIRED;
			componentsInChildren[1].text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
			componentsInChildren[1].color = Constants.NEGATIVE_COLOR;
			priorityScreen.gameObject.SetActive(value: false);
			return;
		}
		ResearchRequired.SetActive(value: false);
		for (int i = 0; i < activeRecipe.Ingredients.Count; i++)
		{
			MaterialSelectors[i].gameObject.SetActive(value: true);
			MaterialSelectors[i].ConfigureScreen(activeRecipe.Ingredients[i], activeRecipe);
		}
		priorityScreen.gameObject.SetActive(value: true);
		priorityScreen.gameObject.transform.SetAsLastSibling();
	}

	public void UpdateResourceToggleValues()
	{
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			if (selector.gameObject.activeSelf)
			{
				selector.RefreshToggleContents();
			}
		});
	}

	public bool AutoSelectAvailableMaterial()
	{
		bool result = true;
		for (int i = 0; i < MaterialSelectors.Count; i++)
		{
			if (!MaterialSelectors[i].AutoSelectAvailableMaterial())
			{
				result = false;
			}
		}
		return result;
	}

	public void SelectSourcesMaterials(Building building)
	{
		Tag[] array = null;
		Deconstructable component = building.gameObject.GetComponent<Deconstructable>();
		if (component != null)
		{
			array = component.constructionElements;
		}
		Constructable component2 = building.GetComponent<Constructable>();
		if (component2 != null)
		{
			array = component2.SelectedElementsTags.ToArray();
		}
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < Mathf.Min(array.Length, MaterialSelectors.Count); i++)
		{
			if (MaterialSelectors[i].ElementToggles.ContainsKey(array[i]))
			{
				MaterialSelectors[i].OnSelectMaterial(array[i], activeRecipe);
			}
		}
	}

	public bool CanBuild(Recipe recipe)
	{
		foreach (MaterialSelector materialSelector in MaterialSelectors)
		{
			if (materialSelector.gameObject.activeSelf && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public static SelectedElemInfo Filter(Tag materialCategoryTag)
	{
		SelectedElemInfo result = default(SelectedElemInfo);
		result.element = null;
		result.kgAvailable = 0f;
		if (WorldInventory.Instance == null || ElementLoader.elements == null || ElementLoader.elements.Count == 0)
		{
			return result;
		}
		List<Tag> value = null;
		if (!elementsWithTag.TryGetValue(materialCategoryTag, out value))
		{
			value = new List<Tag>();
			foreach (Element element in ElementLoader.elements)
			{
				if (element.tag == materialCategoryTag || element.HasTag(materialCategoryTag))
				{
					value.Add(element.tag);
				}
			}
			foreach (Tag materialBuildingElement in GameTags.MaterialBuildingElements)
			{
				if (!(materialBuildingElement == materialCategoryTag))
				{
					continue;
				}
				foreach (GameObject item in Assets.GetPrefabsWithTag(materialBuildingElement))
				{
					KPrefabID component = item.GetComponent<KPrefabID>();
					if (component != null && !value.Contains(component.PrefabTag))
					{
						value.Add(component.PrefabTag);
					}
				}
			}
			elementsWithTag[materialCategoryTag] = value;
		}
		foreach (Tag item2 in value)
		{
			float amount = WorldInventory.Instance.GetAmount(item2);
			if (amount > result.kgAvailable)
			{
				result.kgAvailable = amount;
				result.element = item2;
			}
		}
		return result;
	}

	public void ToggleShowDescriptorPanels(bool show)
	{
		for (int i = 0; i < MaterialSelectors.Count; i++)
		{
			if (MaterialSelectors[i] != null)
			{
				MaterialSelectors[i].ToggleShowDescriptorsPanel(show);
			}
		}
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		priorityScreen.SetScreenPriority(priority);
	}
}
