using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BabyCrabShellConfig : IEntityConfig
{
	public const string ID = "BabyCrabShell";

	public static readonly Tag TAG = TagManager.Create("BabyCrabShell");

	public const float MASS = 5f;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BabyCrabShell", ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.NAME, ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.DESC, 5f, unitMass: true, Assets.GetAnim("crabshells_small_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Organics
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
