using STRINGS;
using UnityEngine;

public class BabyCrabWoodConfig : IEntityConfig
{
	public const string ID = "CrabWoodBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabWoodConfig.CreateCrabWood("CrabWoodBaby", CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.NAME, CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.DESC, "baby_pincher_kanim", is_baby: true, "BabyCrabWoodShell");
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "CrabWood", "BabyCrabWoodShell");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
