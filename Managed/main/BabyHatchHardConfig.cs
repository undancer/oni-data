using STRINGS;
using UnityEngine;

public class BabyHatchHardConfig : IEntityConfig
{
	public const string ID = "HatchHardBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = HatchHardConfig.CreateHatch("HatchHardBaby", CREATURES.SPECIES.HATCH.VARIANT_HARD.BABY.NAME, CREATURES.SPECIES.HATCH.VARIANT_HARD.BABY.DESC, "baby_hatch_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "HatchHard");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
