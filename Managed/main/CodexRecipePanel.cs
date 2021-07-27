using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CodexRecipePanel : CodexWidget<CodexRecipePanel>
{
	private LocText title;

	private GameObject materialPrefab;

	private GameObject fabricatorPrefab;

	private GameObject ingredientsContainer;

	private GameObject resultsContainer;

	private GameObject fabricatorContainer;

	private ComplexRecipe complexRecipe;

	private Recipe recipe;

	public string linkID { get; set; }

	public CodexRecipePanel()
	{
	}

	public CodexRecipePanel(ComplexRecipe recipe)
	{
		complexRecipe = recipe;
	}

	public CodexRecipePanel(Recipe rec)
	{
		recipe = rec;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
		title = component.GetReference<LocText>("Title");
		materialPrefab = component.GetReference<RectTransform>("MaterialPrefab").gameObject;
		fabricatorPrefab = component.GetReference<RectTransform>("FabricatorPrefab").gameObject;
		ingredientsContainer = component.GetReference<RectTransform>("IngredientsContainer").gameObject;
		resultsContainer = component.GetReference<RectTransform>("ResultsContainer").gameObject;
		fabricatorContainer = component.GetReference<RectTransform>("FabricatorContainer").gameObject;
		ClearPanel();
		if (recipe != null)
		{
			ConfigureRecipe();
		}
		else if (complexRecipe != null)
		{
			ConfigureComplexRecipe();
		}
	}

	private void ConfigureRecipe()
	{
		title.text = recipe.Result.ProperName();
		foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
		{
			GameObject gameObject = Util.KInstantiateUI(materialPrefab, ingredientsContainer, force_active: true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(ingredient.tag);
			component.GetReference<Image>("Icon").sprite = uISprite.first;
			component.GetReference<Image>("Icon").color = uISprite.second;
			component.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(ingredient.tag, ingredient.amount);
			component.GetReference<LocText>("Amount").color = Color.black;
			string text = ingredient.tag.ProperName();
			GameObject prefab = Assets.GetPrefab(ingredient.tag);
			if (prefab.GetComponent<Edible>() != null)
			{
				text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
			}
			gameObject.GetComponent<ToolTip>().toolTip = text;
		}
		GameObject gameObject2 = Util.KInstantiateUI(materialPrefab, resultsContainer, force_active: true);
		HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
		Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(recipe.Result);
		component2.GetReference<Image>("Icon").sprite = uISprite2.first;
		component2.GetReference<Image>("Icon").color = uISprite2.second;
		component2.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(recipe.Result, recipe.OutputUnits);
		component2.GetReference<LocText>("Amount").color = Color.black;
		string text2 = recipe.Result.ProperName();
		GameObject prefab2 = Assets.GetPrefab(recipe.Result);
		if (prefab2.GetComponent<Edible>() != null)
		{
			text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
		}
		gameObject2.GetComponent<ToolTip>().toolTip = text2;
	}

	private void ConfigureComplexRecipe()
	{
		title.text = complexRecipe.results[0].material.ProperName();
		ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
		foreach (ComplexRecipe.RecipeElement ing in ingredients)
		{
			HierarchyReferences component = Util.KInstantiateUI(materialPrefab, ingredientsContainer, force_active: true).GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(ing.material);
			component.GetReference<Image>("Icon").sprite = uISprite.first;
			component.GetReference<Image>("Icon").color = uISprite.second;
			component.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(ing.material, ing.amount);
			component.GetReference<LocText>("Amount").color = Color.black;
			string text = ing.material.ProperName();
			GameObject prefab = Assets.GetPrefab(ing.material);
			if (prefab.GetComponent<Edible>() != null)
			{
				text = text + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
			}
			component.GetReference<ToolTip>("Tooltip").toolTip = text;
			component.GetReference<KButton>("Button").onClick += delegate
			{
				ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(ing.material.ToString()));
			};
		}
		ingredients = complexRecipe.results;
		foreach (ComplexRecipe.RecipeElement res in ingredients)
		{
			HierarchyReferences component2 = Util.KInstantiateUI(materialPrefab, resultsContainer, force_active: true).GetComponent<HierarchyReferences>();
			Tuple<Sprite, Color> uISprite2 = Def.GetUISprite(res.material);
			component2.GetReference<Image>("Icon").sprite = uISprite2.first;
			component2.GetReference<Image>("Icon").color = uISprite2.second;
			component2.GetReference<LocText>("Amount").text = GameUtil.GetFormattedByTag(res.material, res.amount);
			component2.GetReference<LocText>("Amount").color = Color.black;
			string text2 = res.material.ProperName();
			GameObject prefab2 = Assets.GetPrefab(res.material);
			if (prefab2.GetComponent<Edible>() != null)
			{
				text2 = text2 + "\n    • " + string.Format(UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, GameUtil.GetFormattedFoodQuality(prefab2.GetComponent<Edible>().GetQuality()));
			}
			component2.GetReference<ToolTip>("Tooltip").toolTip = text2;
			component2.GetReference<KButton>("Button").onClick += delegate
			{
				ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(res.material.ToString()));
			};
		}
		string fabricatorId = complexRecipe.id.Substring(0, complexRecipe.id.IndexOf('_'));
		HierarchyReferences component3 = Util.KInstantiateUI(fabricatorPrefab, fabricatorContainer, force_active: true).GetComponent<HierarchyReferences>();
		Tuple<Sprite, Color> uISprite3 = Def.GetUISprite(fabricatorId);
		component3.GetReference<Image>("Icon").sprite = uISprite3.first;
		component3.GetReference<Image>("Icon").color = uISprite3.second;
		component3.GetReference<LocText>("Time").text = GameUtil.GetFormattedTime(complexRecipe.time);
		component3.GetReference<LocText>("Time").color = Color.black;
		GameObject prefab3 = Assets.GetPrefab(fabricatorId.ToTag());
		component3.GetReference<ToolTip>("Tooltip").toolTip = prefab3.GetProperName();
		component3.GetReference<KButton>("Button").onClick += delegate
		{
			ManagementMenu.Instance.codexScreen.ChangeArticle(CodexCache.FormatLinkID(fabricatorId));
		};
	}

	private void ClearPanel()
	{
		foreach (Transform item in ingredientsContainer.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in resultsContainer.transform)
		{
			Object.Destroy(item2.gameObject);
		}
		foreach (Transform item3 in fabricatorContainer.transform)
		{
			Object.Destroy(item3.gameObject);
		}
	}
}
