using System.Collections.Generic;
using UnityEngine;

public class ResourceRemainingDisplayScreen : KScreen
{
	public static ResourceRemainingDisplayScreen instance;

	public GameObject dispayPrefab;

	public LocText label;

	private Recipe currentRecipe;

	private List<Tag> selected_elements = new List<Tag>();

	private int numberOfPendingConstructions;

	private int displayedConstructionCostMultiplier;

	private RectTransform rect;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Activate();
		instance = this;
		dispayPrefab.SetActive(value: false);
	}

	public void ActivateDisplay(GameObject target)
	{
		numberOfPendingConstructions = 0;
		dispayPrefab.SetActive(value: true);
	}

	public void DeactivateDisplay()
	{
		dispayPrefab.SetActive(value: false);
	}

	public void SetResources(IList<Tag> _selected_elements, Recipe recipe)
	{
		selected_elements.Clear();
		foreach (Tag _selected_element in _selected_elements)
		{
			selected_elements.Add(_selected_element);
		}
		currentRecipe = recipe;
		Debug.Assert(selected_elements.Count == recipe.Ingredients.Count);
	}

	public void SetNumberOfPendingConstructions(int number)
	{
		numberOfPendingConstructions = number;
	}

	public void Update()
	{
		if (!dispayPrefab.activeSelf)
		{
			return;
		}
		if (base.canvas != null)
		{
			if (rect == null)
			{
				rect = GetComponent<RectTransform>();
			}
			rect.anchoredPosition = WorldToScreen(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
		}
		if (displayedConstructionCostMultiplier == numberOfPendingConstructions)
		{
			label.text = "";
		}
		else
		{
			displayedConstructionCostMultiplier = numberOfPendingConstructions;
		}
	}

	public string GetString()
	{
		string text = "";
		if (selected_elements != null && currentRecipe != null)
		{
			for (int i = 0; i < currentRecipe.Ingredients.Count; i++)
			{
				Tag tag = selected_elements[i];
				float num = currentRecipe.Ingredients[i].amount * (float)numberOfPendingConstructions;
				float num2 = WorldInventory.Instance.GetTotalAmount(tag) - WorldInventory.Instance.GetAmount(tag);
				float num3 = WorldInventory.Instance.GetTotalAmount(tag) - (num2 + num);
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				text = text + tag.ProperName() + ": " + GameUtil.GetFormattedMass(num3) + " / " + GameUtil.GetFormattedMass(currentRecipe.Ingredients[i].amount);
				if (i < selected_elements.Count - 1)
				{
					text += "\n";
				}
			}
		}
		return text;
	}
}
