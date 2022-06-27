using STRINGS;
using UnityEngine;

public class BabyCrabFreshWaterConfig : IEntityConfig
{
	public const string ID = "CrabFreshWaterBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabFreshWaterConfig.CreateCrabFreshWater("CrabFreshWaterBaby", CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.BABY.NAME, CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.BABY.DESC, "baby_pincher_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "CrabFreshWater");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
