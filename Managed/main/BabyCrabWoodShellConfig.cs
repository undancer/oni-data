using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BabyCrabWoodShellConfig : IEntityConfig
{
	public const string ID = "BabyCrabWoodShell";

	public static readonly Tag TAG = TagManager.Create("BabyCrabWoodShell");

	public const float MASS = 50f;

	public const string symbolPrefix = "wood_";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("BabyCrabWoodShell", ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.VARIANT_WOOD.NAME, ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.VARIANT_WOOD.DESC, 50f, unitMass: true, Assets.GetAnim("crabshells_small_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Organics,
			GameTags.MoltShell
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>().symbolPrefix = "wood_";
		SymbolOverrideControllerUtil.AddToPrefab(gameObject).ApplySymbolOverridesByAffix(Assets.GetAnim("crabshells_small_kanim"), "wood_");
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
