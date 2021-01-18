using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FabricatorIngredientStatusManager")]
public class FabricatorIngredientStatusManager : KMonoBehaviour, ISim1000ms
{
	private KSelectable selectable;

	private ComplexFabricator fabricator;

	private Dictionary<ComplexRecipe, Guid> statusItems = new Dictionary<ComplexRecipe, Guid>();

	private Dictionary<ComplexRecipe, Dictionary<Tag, float>> recipeRequiredResourceBalances = new Dictionary<ComplexRecipe, Dictionary<Tag, float>>();

	private List<ComplexRecipe> deadOrderKeys = new List<ComplexRecipe>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		selectable = GetComponent<KSelectable>();
		fabricator = GetComponent<ComplexFabricator>();
		InitializeBalances();
	}

	private void InitializeBalances()
	{
		ComplexRecipe[] recipes = fabricator.GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			recipeRequiredResourceBalances.Add(complexRecipe, new Dictionary<Tag, float>());
			ComplexRecipe.RecipeElement[] ingredients = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				recipeRequiredResourceBalances[complexRecipe].Add(recipeElement.material, 0f);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		RefreshStatusItems();
	}

	private void RefreshStatusItems()
	{
		foreach (KeyValuePair<ComplexRecipe, Guid> statusItem in statusItems)
		{
			if (!fabricator.IsRecipeQueued(statusItem.Key))
			{
				deadOrderKeys.Add(statusItem.Key);
			}
		}
		foreach (ComplexRecipe deadOrderKey in deadOrderKeys)
		{
			recipeRequiredResourceBalances[deadOrderKey].Clear();
			ComplexRecipe.RecipeElement[] ingredients = deadOrderKey.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
			{
				recipeRequiredResourceBalances[deadOrderKey].Add(recipeElement.material, 0f);
			}
			selectable.RemoveStatusItem(statusItems[deadOrderKey]);
			statusItems.Remove(deadOrderKey);
		}
		deadOrderKeys.Clear();
		ComplexRecipe[] recipes = fabricator.GetRecipes();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			if (!fabricator.IsRecipeQueued(complexRecipe))
			{
				continue;
			}
			bool flag = false;
			ComplexRecipe.RecipeElement[] ingredients2 = complexRecipe.ingredients;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
			{
				float newBalance = fabricator.inStorage.GetAmountAvailable(recipeElement2.material) + fabricator.buildStorage.GetAmountAvailable(recipeElement2.material) + fabricator.GetMyWorld().worldInventory.GetTotalAmount(recipeElement2.material, includeRelatedWorlds: true) - recipeElement2.amount;
				flag = flag || ChangeRecipeRequiredResourceBalance(complexRecipe, recipeElement2.material, newBalance) || (statusItems.ContainsKey(complexRecipe) && fabricator.GetRecipeQueueCount(complexRecipe) == 0);
			}
			if (!flag)
			{
				continue;
			}
			if (statusItems.ContainsKey(complexRecipe))
			{
				selectable.RemoveStatusItem(statusItems[complexRecipe]);
				statusItems.Remove(complexRecipe);
			}
			if (!fabricator.IsRecipeQueued(complexRecipe))
			{
				continue;
			}
			foreach (float value2 in recipeRequiredResourceBalances[complexRecipe].Values)
			{
				if (!(value2 < 0f))
				{
					continue;
				}
				Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
				foreach (KeyValuePair<Tag, float> item in recipeRequiredResourceBalances[complexRecipe])
				{
					if (item.Value < 0f)
					{
						dictionary.Add(item.Key, 0f - item.Value);
					}
				}
				Guid value = selectable.AddStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, dictionary);
				statusItems.Add(complexRecipe, value);
				break;
			}
		}
	}

	private bool ChangeRecipeRequiredResourceBalance(ComplexRecipe recipe, Tag tag, float newBalance)
	{
		bool result = false;
		if (recipeRequiredResourceBalances[recipe][tag] >= 0f != newBalance >= 0f)
		{
			result = true;
		}
		recipeRequiredResourceBalances[recipe][tag] = newBalance;
		return result;
	}
}
