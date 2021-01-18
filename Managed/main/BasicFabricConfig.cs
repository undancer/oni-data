using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicFabricConfig : IEntityConfig
{
	public static string ID = "BasicFabric";

	private AttributeModifier decorModifier = new AttributeModifier("Decor", 0.1f, ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME, is_multiplier: true);

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME, ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.DESC, 1f, unitMass: true, Assets.GetAnim("swampreedwool_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, isPickupable: true, SORTORDER.BUILDINGELEMENTS + BasicFabricTuning.SORTORDER, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.BuildingFiber
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<PrefabAttributeModifiers>().AddAttributeDescriptor(decorModifier);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
