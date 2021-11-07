using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class OrbitalResearchDatabankConfig : IEntityConfig
{
	public const string ID = "OrbitalResearchDatabank";

	public static readonly Tag TAG = TagManager.Create("OrbitalResearchDatabank");

	public const float MASS = 1f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("OrbitalResearchDatabank", ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.NAME, ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.DESC, 1f, unitMass: true, Assets.GetAnim("floppy_disc_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
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
