using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ResearchDatabankConfig : IEntityConfig
{
	public const string ID = "ResearchDatabank";

	public static readonly Tag TAG = TagManager.Create("ResearchDatabank");

	public const float MASS = 1f;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("ResearchDatabank", ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME, ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.DESC, 1f, unitMass: true, Assets.GetAnim("floppy_disc_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Experimental
		});
		gameObject.AddOrGet<EntitySplitter>().maxStackSize = ROCKETRY.DESTINATION_RESEARCH.BASIC;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
