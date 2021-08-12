using STRINGS;
using UnityEngine;

public class LightBugPinkBabyConfig : IEntityConfig
{
	public const string ID = "LightBugPinkBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugPinkConfig.CreateLightBug("LightBugPinkBaby", CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.BABY.NAME, CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.BABY.DESC, "baby_lightbug_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LightBugPink");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
