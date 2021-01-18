using STRINGS;
using UnityEngine;

public class BabyHatchVeggieConfig : IEntityConfig
{
	public const string ID = "HatchVeggieBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = HatchVeggieConfig.CreateHatch("HatchVeggieBaby", CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.NAME, CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.DESC, "baby_hatch_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "HatchVeggie");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
