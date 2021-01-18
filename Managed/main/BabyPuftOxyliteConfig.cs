using STRINGS;
using UnityEngine;

public class BabyPuftOxyliteConfig : IEntityConfig
{
	public const string ID = "PuftOxyliteBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftOxyliteConfig.CreatePuftOxylite("PuftOxyliteBaby", CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.BABY.NAME, CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.BABY.DESC, "baby_puft_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PuftOxylite");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
