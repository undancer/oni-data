using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class CustomClothingConfig : IEquipmentConfig
{
	public const string ID = "CustomClothing";

	public static ComplexRecipe recipe;

	public static List<ComplexRecipe> facadeRecipes = new List<ComplexRecipe>();

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public EquipmentDef CreateEquipmentDef()
	{
		new Dictionary<string, float>
		{
			{
				"Funky_Vest",
				TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS
			},
			{ "BasicFabric", 3f }
		};
		ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.CUSTOM_CLOTHING;
		List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
		EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("CustomClothing", TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, TUNING.EQUIPMENT.VESTS.CUSTOM_CLOTHING_MASS, "shirt_decor01_kanim", TUNING.EQUIPMENT.VESTS.SNAPON0, "body_shirt_decor01_kanim", 4, attributeModifiers, TUNING.EQUIPMENT.VESTS.SNAPON1, IsBody: true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f);
		Descriptor item = new Descriptor($"{DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME}: {GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.CUSTOM_CLOTHING.conductivityMod)}", $"{DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME}: {GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.CUSTOM_CLOTHING.conductivityMod)}");
		Descriptor item2 = new Descriptor($"{DUPLICANTS.ATTRIBUTES.DECOR.NAME}: {ClothingWearer.ClothingInfo.CUSTOM_CLOTHING.decorMod}", $"{DUPLICANTS.ATTRIBUTES.DECOR.NAME}: {ClothingWearer.ClothingInfo.CUSTOM_CLOTHING.decorMod}");
		equipmentDef.additionalDescriptors.Add(item);
		equipmentDef.additionalDescriptors.Add(item2);
		equipmentDef.OnEquipCallBack = delegate(Equippable eq)
		{
			CoolVestConfig.OnEquipVest(eq, clothingInfo);
		};
		equipmentDef.OnUnequipCallBack = CoolVestConfig.OnUnequipVest;
		equipmentDef.RecipeDescription = STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.RECIPE_DESC;
		foreach (EquippableFacadeResource resource in Db.Get().EquippableFacades.resources)
		{
			if (!(resource.DefID != "CustomClothing"))
			{
				TagManager.Create(resource.Name, EquippableFacade.GetNameOverride("CustomClothing", resource.Name));
			}
		}
		return equipmentDef;
	}

	public static void SetupVest(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes);
		Equippable equippable = go.GetComponent<Equippable>();
		if (equippable == null)
		{
			equippable = go.AddComponent<Equippable>();
		}
		equippable.SetQuality(QualityLevel.Poor);
		go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
	}

	public void DoPostConfigure(GameObject go)
	{
		SetupVest(go);
		go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable);
	}
}
