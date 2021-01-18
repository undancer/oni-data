using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRecipeQueueScreen : KScreen
{
	public Image recipeIcon;

	public LocText recipeName;

	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public KNumberInputField QueueCount;

	public MultiToggle DecrementButton;

	public MultiToggle IncrementButton;

	public KButton InfiniteButton;

	public GameObject InfiniteIcon;

	private ComplexFabricator target = null;

	private ComplexFabricatorSideScreen ownerScreen = null;

	private ComplexRecipe selectedRecipe;

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
		recipeIcon.sprite = uISprite.first;
		recipeIcon.color = uISprite.second;
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
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(GetResultDescriptions(selectedRecipe));
		list.AddRange(target.AdditionalEffectsForRecipe(selectedRecipe));
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list);
			list.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS));
			EffectsDescriptorPanel.gameObject.SetActive(value: true);
			EffectsDescriptorPanel.SetDescriptors(list);
		}
	}

	public List<Descriptor> GetResultDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe.RecipeElement[] results = recipe.results;
		foreach (ComplexRecipe.RecipeElement recipeElement in results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount);
			list.Add(new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), Descriptor.DescriptorType.Requirement));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
				GameUtil.IndentListOfDescriptors(materialDescriptors);
				list.AddRange(materialDescriptors);
			}
			else
			{
				List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(GameUtil.GetAllDescriptors(prefab));
				GameUtil.IndentListOfDescriptors(effectDescriptors);
				list.AddRange(effectDescriptors);
			}
		}
		return list;
	}

	private void RefreshIngredientDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement));
		List<Descriptor> ingredientDescriptions = GetIngredientDescriptions(selectedRecipe);
		GameUtil.IndentListOfDescriptors(ingredientDescriptions);
		list.AddRange(ingredientDescriptions);
		IngredientsDescriptorPanel.gameObject.SetActive(value: true);
		IngredientsDescriptorPanel.SetDescriptors(list);
	}

	public List<Descriptor> GetIngredientDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe.RecipeElement[] ingredients = recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount);
			float amount = target.GetMyWorld().worldInventory.GetAmount(recipeElement.material, includeRelatedWorlds: true);
			string formattedByTag2 = GameUtil.GetFormattedByTag(recipeElement.material, amount);
			string text = ((amount >= recipeElement.amount) ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) : ("<color=#F44A47>" + string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) + "</color>"));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Requirement));
		}
		return list;
	}
}
