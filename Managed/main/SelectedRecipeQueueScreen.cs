using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRecipeQueueScreen : KScreen
{
	private class DescriptorWithSprite
	{
		public bool showFilterRow;

		public Descriptor descriptor { get; }

		public Tuple<Sprite, Color> tintedSprite { get; }

		public DescriptorWithSprite(Descriptor desc, Tuple<Sprite, Color> sprite, bool filterRowVisible = false)
		{
			descriptor = desc;
			tintedSprite = sprite;
			showFilterRow = filterRowVisible;
		}
	}

	public Image recipeIcon;

	public LocText recipeName;

	public GameObject IngredientsDescriptorPanel;

	public GameObject buildingGlobalRecipeFilters;

	public GameObject EffectsDescriptorPanel;

	public KNumberInputField QueueCount;

	public MultiToggle DecrementButton;

	public MultiToggle IncrementButton;

	public KButton InfiniteButton;

	public GameObject InfiniteIcon;

	private ComplexFabricator target;

	private ComplexFabricatorSideScreen ownerScreen;

	private ComplexRecipe selectedRecipe;

	[SerializeField]
	private GameObject recipeElementDescriptorPrefab;

	private Dictionary<DescriptorWithSprite, GameObject> recipeIngredientDescriptorRows = new Dictionary<DescriptorWithSprite, GameObject>();

	private Dictionary<DescriptorWithSprite, GameObject> recipeEffectsDescriptorRows = new Dictionary<DescriptorWithSprite, GameObject>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DecrementButton.onClick = delegate
		{
			target.DecrementRecipeQueueCount(selectedRecipe, respectInfinite: false);
			RefreshQueueCountDisplay();
			ownerScreen.RefreshQueueCountDisplayForRecipe(selectedRecipe, target);
		};
		IncrementButton.onClick = delegate
		{
			target.IncrementRecipeQueueCount(selectedRecipe);
			RefreshQueueCountDisplay();
			ownerScreen.RefreshQueueCountDisplayForRecipe(selectedRecipe, target);
		};
		InfiniteButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPE_FOREVER;
		InfiniteButton.onClick += delegate
		{
			if (target.GetRecipeQueueCount(selectedRecipe) != ComplexFabricator.QUEUE_INFINITE)
			{
				target.SetRecipeQueueCount(selectedRecipe, ComplexFabricator.QUEUE_INFINITE);
			}
			else
			{
				target.SetRecipeQueueCount(selectedRecipe, 0);
			}
			RefreshQueueCountDisplay();
			ownerScreen.RefreshQueueCountDisplayForRecipe(selectedRecipe, target);
		};
		QueueCount.onEndEdit += delegate
		{
			base.isEditing = false;
			target.SetRecipeQueueCount(selectedRecipe, Mathf.RoundToInt(QueueCount.currentValue));
			RefreshQueueCountDisplay();
			ownerScreen.RefreshQueueCountDisplayForRecipe(selectedRecipe, target);
		};
		QueueCount.onStartEdit += delegate
		{
			base.isEditing = true;
			KScreenManager.Instance.RefreshStack();
		};
	}

	public void SetRecipe(ComplexFabricatorSideScreen owner, ComplexFabricator target, ComplexRecipe recipe)
	{
		ownerScreen = owner;
		this.target = target;
		selectedRecipe = recipe;
		recipeName.text = recipe.GetUIName(includeAmounts: false);
		Tuple<Sprite, Color> uISprite = Def.GetUISprite((recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient) ? recipe.ingredients[0].material : recipe.results[0].material);
		if (recipe.nameDisplay == ComplexRecipe.RecipeNameDisplay.HEP)
		{
			recipeIcon.sprite = owner.radboltSprite;
		}
		else
		{
			recipeIcon.sprite = uISprite.first;
			recipeIcon.color = uISprite.second;
		}
		RefreshIngredientDescriptors();
		RefreshResultDescriptors();
		RefreshQueueCountDisplay();
	}

	private void RefreshQueueCountDisplay()
	{
		bool flag = target.GetRecipeQueueCount(selectedRecipe) == ComplexFabricator.QUEUE_INFINITE;
		if (!flag)
		{
			QueueCount.SetAmount(target.GetRecipeQueueCount(selectedRecipe));
		}
		else
		{
			QueueCount.SetDisplayValue("");
		}
		InfiniteIcon.gameObject.SetActive(flag);
	}

	private void RefreshResultDescriptors()
	{
		List<DescriptorWithSprite> list = new List<DescriptorWithSprite>();
		list.AddRange(GetResultDescriptions(selectedRecipe));
		foreach (Descriptor item in target.AdditionalEffectsForRecipe(selectedRecipe))
		{
			list.Add(new DescriptorWithSprite(item, null));
		}
		if (list.Count <= 0)
		{
			return;
		}
		EffectsDescriptorPanel.gameObject.SetActive(value: true);
		foreach (KeyValuePair<DescriptorWithSprite, GameObject> recipeEffectsDescriptorRow in recipeEffectsDescriptorRows)
		{
			Util.KDestroyGameObject(recipeEffectsDescriptorRow.Value);
		}
		recipeEffectsDescriptorRows.Clear();
		foreach (DescriptorWithSprite item2 in list)
		{
			GameObject gameObject = Util.KInstantiateUI(recipeElementDescriptorPrefab, EffectsDescriptorPanel.gameObject, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").SetText(item2.descriptor.IndentedText());
			component.GetReference<Image>("Icon").sprite = ((item2.tintedSprite == null) ? null : item2.tintedSprite.first);
			component.GetReference<Image>("Icon").color = ((item2.tintedSprite == null) ? Color.white : item2.tintedSprite.second);
			component.GetReference<RectTransform>("FilterControls").gameObject.SetActive(value: false);
			component.GetReference<ToolTip>("Tooltip").SetSimpleTooltip(item2.descriptor.tooltipText);
			recipeEffectsDescriptorRows.Add(item2, gameObject);
		}
	}

	private List<DescriptorWithSprite> GetResultDescriptions(ComplexRecipe recipe)
	{
		List<DescriptorWithSprite> list = new List<DescriptorWithSprite>();
		if (recipe.producedHEP > 0)
		{
			list.Add(new DescriptorWithSprite(new Descriptor(string.Format("<b>{0}</b>: {1}", UI.FormatAsLink(ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, "HEP"), recipe.producedHEP), $"<b>{ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME}</b>: {recipe.producedHEP}", Descriptor.DescriptorType.Requirement), new Tuple<Sprite, Color>(Assets.GetSprite("radbolt"), Color.white)));
		}
		ComplexRecipe.RecipeElement[] results = recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement in results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount);
			list.Add(new DescriptorWithSprite(new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), Descriptor.DescriptorType.Requirement), Def.GetUISprite(recipeElement.material)));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<DescriptorWithSprite> list2 = new List<DescriptorWithSprite>();
				foreach (Descriptor materialDescriptor in GameUtil.GetMaterialDescriptors(element))
				{
					list2.Add(new DescriptorWithSprite(materialDescriptor, null));
				}
				foreach (DescriptorWithSprite item in list2)
				{
					item.descriptor.IncreaseIndent();
				}
				list.AddRange(list2);
				continue;
			}
			List<DescriptorWithSprite> list3 = new List<DescriptorWithSprite>();
			foreach (Descriptor effectDescriptor in GameUtil.GetEffectDescriptors(GameUtil.GetAllDescriptors(prefab)))
			{
				list3.Add(new DescriptorWithSprite(effectDescriptor, null));
			}
			foreach (DescriptorWithSprite item2 in list3)
			{
				item2.descriptor.IncreaseIndent();
			}
			list.AddRange(list3);
		}
		return list;
	}

	private void RefreshIngredientDescriptors()
	{
		new List<DescriptorWithSprite>();
		List<DescriptorWithSprite> ingredientDescriptions = GetIngredientDescriptions(selectedRecipe);
		IngredientsDescriptorPanel.gameObject.SetActive(value: true);
		foreach (KeyValuePair<DescriptorWithSprite, GameObject> recipeIngredientDescriptorRow in recipeIngredientDescriptorRows)
		{
			Util.KDestroyGameObject(recipeIngredientDescriptorRow.Value);
		}
		recipeIngredientDescriptorRows.Clear();
		foreach (DescriptorWithSprite item in ingredientDescriptions)
		{
			GameObject gameObject = Util.KInstantiateUI(recipeElementDescriptorPrefab, IngredientsDescriptorPanel.gameObject, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").SetText(item.descriptor.IndentedText());
			component.GetReference<Image>("Icon").sprite = ((item.tintedSprite == null) ? null : item.tintedSprite.first);
			component.GetReference<Image>("Icon").color = ((item.tintedSprite == null) ? Color.white : item.tintedSprite.second);
			component.GetReference<RectTransform>("FilterControls").gameObject.SetActive(value: false);
			component.GetReference<ToolTip>("Tooltip").SetSimpleTooltip(item.descriptor.tooltipText);
			recipeIngredientDescriptorRows.Add(item, gameObject);
		}
	}

	private List<DescriptorWithSprite> GetIngredientDescriptions(ComplexRecipe recipe)
	{
		List<DescriptorWithSprite> list = new List<DescriptorWithSprite>();
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount);
			float amount = target.GetMyWorld().worldInventory.GetAmount(recipeElement.material, includeRelatedWorlds: true);
			string formattedByTag2 = GameUtil.GetFormattedByTag(recipeElement.material, amount);
			string text = ((amount >= recipeElement.amount) ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) : ("<color=#F44A47>" + string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) + "</color>"));
			list.Add(new DescriptorWithSprite(new Descriptor(text, text, Descriptor.DescriptorType.Requirement), Def.GetUISprite(recipeElement.material), Assets.GetPrefab(recipeElement.material).GetComponent<MutantPlant>() != null));
		}
		if (recipe.consumedHEP > 0)
		{
			HighEnergyParticleStorage component = target.GetComponent<HighEnergyParticleStorage>();
			list.Add(new DescriptorWithSprite(new Descriptor(string.Format("<b>{0}</b>: {1} / {2}", UI.FormatAsLink(ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, "HEP"), recipe.consumedHEP, component.Particles), $"<b>{ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME}</b>: {recipe.consumedHEP} / {component.Particles}", Descriptor.DescriptorType.Requirement), new Tuple<Sprite, Color>(Assets.GetSprite("radbolt"), Color.white)));
		}
		return list;
	}
}
