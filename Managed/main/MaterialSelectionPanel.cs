using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialSelectionPanel : KScreen
{
	public delegate bool GetBuildableStateDelegate(BuildingDef def);

	public delegate string GetBuildableTooltipDelegate(BuildingDef def);

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

	private GetBuildableStateDelegate GetBuildableState;

	private GetBuildableTooltipDelegate GetBuildableTooltip;

	private List<int> gameSubscriptionHandles = new List<int>();

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
		gameSubscriptionHandles.Add(Game.Instance.Subscribe(-107300940, delegate
		{
			RefreshSelectors();
		}));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		activateOnSpawn = true;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (int gameSubscriptionHandle in gameSubscriptionHandles)
		{
			Game.Instance.Unsubscribe(gameSubscriptionHandle);
		}
		gameSubscriptionHandles.Clear();
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

	public void ConfigureScreen(Recipe recipe, GetBuildableStateDelegate buildableStateCB, GetBuildableTooltipDelegate buildableTooltipCB)
	{
		activeRecipe = recipe;
		GetBuildableState = buildableStateCB;
		GetBuildableTooltip = buildableTooltipCB;
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
		if (activeRecipe == null || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.gameObject.SetActive(value: false);
		});
		BuildingDef buildingDef = activeRecipe.GetBuildingDef();
		bool flag = GetBuildableState(buildingDef);
		string text = GetBuildableTooltip(buildingDef);
		if (!flag)
		{
			ResearchRequired.SetActive(value: true);
			LocText[] componentsInChildren = ResearchRequired.GetComponentsInChildren<LocText>();
			componentsInChildren[0].text = "";
			componentsInChildren[1].text = text;
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
			MaterialSelector materialSelector = MaterialSelectors[i];
			if (!materialSelector.AutoSelectAvailableMaterial())
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

	public static SelectedElemInfo Filter(Tag materialCategoryTag)
	{
		SelectedElemInfo result = default(SelectedElemInfo);
		result.element = null;
		result.kgAvailable = 0f;
		if (DiscoveredResources.Instance == null || ElementLoader.elements == null || ElementLoader.elements.Count == 0)
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
			float amount = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(item2, includeRelatedWorlds: true);
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
