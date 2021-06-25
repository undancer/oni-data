using STRINGS;
using TUNING;
using UnityEngine;

public class BasicPlantBarConfig : IEntityConfig
{
	public const string ID = "BasicPlantBar";

	public static ComplexRecipe recipe;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject template = EntityTemplates.CreateLooseEntity("BasicPlantBar", ITEMS.FOOD.BASICPLANTBAR.NAME, ITEMS.FOOD.BASICPLANTBAR.DESC, 1f, unitMass: false, Assets.GetAnim("liceloaf_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true);
		template = EntityTemplates.ExtendEntityToFood(template, FOOD.FOOD_TYPES.BASICPLANTBAR);
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
