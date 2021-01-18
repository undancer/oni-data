using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SwampLilyFlowerConfig : IEntityConfig
{
	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "SwampLilyFlower";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(ID, ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME, ITEMS.INGREDIENTS.SWAMPLILYFLOWER.DESC, 1f, unitMass: false, Assets.GetAnim("swamplilyflower_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient
		});
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
