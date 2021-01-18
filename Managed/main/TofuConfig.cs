using STRINGS;
using TUNING;
using UnityEngine;

public class TofuConfig : IEntityConfig
{
	public const string ID = "Tofu";

	public static ComplexRecipe recipe;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("Tofu", ITEMS.FOOD.TOFU.NAME, ITEMS.FOOD.TOFU.DESC, 1f, unitMass: false, Assets.GetAnim("loafu_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.TOFU);
		ComplexRecipeManager.Get().GetRecipe(recipe.id).FabricationVisualizer = MushBarConfig.CreateFabricationVisualizer(template);
		return template;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
