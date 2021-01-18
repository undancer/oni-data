using STRINGS;
using UnityEngine;

public class LightBugRadioactiveBabyConfig : IEntityConfig
{
	public const string ID = "LightBugRadioactiveBaby";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugRadioactiveConfig.CreateLightBug("LightBugRadioactiveBaby", CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.BABY.NAME, CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.BABY.DESC, "baby_lightbug_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LightBugRadioactive");
		gameObject.AddTag(GameTags.DeprecatedContent);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
