using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EquipmentConfigManager")]
public class EquipmentConfigManager : KMonoBehaviour
{
	public static EquipmentConfigManager Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void RegisterEquipment(IEquipmentConfig config)
	{
		EquipmentDef equipmentDef = config.CreateEquipmentDef();
		if (DlcManager.IsContentActive(equipmentDef.RequiredDlcId))
		{
			GameObject gameObject = EntityTemplates.CreateLooseEntity(equipmentDef.Id, equipmentDef.Name, equipmentDef.RecipeDescription, equipmentDef.Mass, unitMass: true, equipmentDef.Anim, "object", Grid.SceneLayer.Ore, equipmentDef.CollisionShape, equipmentDef.width, equipmentDef.height, isPickupable: true, 0, equipmentDef.OutputElement);
			Equippable equippable = gameObject.AddComponent<Equippable>();
			equippable.def = equipmentDef;
			Debug.Assert(equippable.def != null);
			equippable.slotID = equipmentDef.Slot;
			Debug.Assert(equippable.slot != null);
			config.DoPostConfigure(gameObject);
			Assets.AddPrefab(gameObject.GetComponent<KPrefabID>());
		}
	}

	private void LoadRecipe(EquipmentDef def, Equippable equippable)
	{
		Recipe recipe = new Recipe(def.Id, 1f, (SimHashes)0, null, def.RecipeDescription);
		recipe.SetFabricator(def.FabricatorId, def.FabricationTime);
		recipe.TechUnlock = def.RecipeTechUnlock;
		foreach (KeyValuePair<string, float> item in def.InputElementMassMap)
		{
			recipe.AddIngredient(new Recipe.Ingredient(item.Key, item.Value));
		}
	}
}
