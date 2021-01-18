using UnityEngine;

public class ResearchType
{
	private string _id;

	private string _name;

	private string _description;

	private Recipe _recipe;

	private Sprite _sprite;

	private Color _color;

	public string id => _id;

	public string name => _name;

	public string description => _description;

	public string recipe => recipe;

	public Color color => _color;

	public Sprite sprite => _sprite;

	public ResearchType(string id, string name, string description, Sprite sprite, Color color, Recipe.Ingredient[] fabricationIngredients, float fabricationTime, HashedString kAnim_ID, string[] fabricators, string recipeDescription)
	{
		_id = id;
		_name = name;
		_description = description;
		_sprite = sprite;
		_color = color;
		CreatePrefab(fabricationIngredients, fabricationTime, kAnim_ID, fabricators, recipeDescription, color);
	}

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab(Recipe.Ingredient[] fabricationIngredients, float fabricationTime, HashedString kAnim_ID, string[] fabricators, string recipeDescription, Color color)
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity(id, name, description, 1f, unitMass: true, Assets.GetAnim(kAnim_ID), "ui", Grid.SceneLayer.BuildingFront);
		ResearchPointObject researchPointObject = gameObject.AddOrGet<ResearchPointObject>();
		researchPointObject.TypeID = id;
		_recipe = new Recipe(id, 1f, (SimHashes)0, name, recipeDescription);
		_recipe.SetFabricators(fabricators, fabricationTime);
		_recipe.SetIcon(Assets.GetSprite("research_type_icon"), color);
		if (fabricationIngredients != null)
		{
			foreach (Recipe.Ingredient ingredient in fabricationIngredients)
			{
				_recipe.AddIngredient(ingredient);
			}
		}
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
