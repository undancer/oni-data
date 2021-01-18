using STRINGS;
using UnityEngine;

public class LightBugCrystalBabyConfig : IEntityConfig
{
	public const string ID = "LightBugCrystalBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugCrystalConfig.CreateLightBug("LightBugCrystalBaby", CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.BABY.NAME, CREATURES.SPECIES.LIGHTBUG.VARIANT_CRYSTAL.BABY.DESC, "baby_lightbug_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LightBugCrystal");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
