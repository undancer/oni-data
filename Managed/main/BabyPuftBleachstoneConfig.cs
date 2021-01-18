using STRINGS;
using UnityEngine;

public class BabyPuftBleachstoneConfig : IEntityConfig
{
	public const string ID = "PuftBleachstoneBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftBleachstoneConfig.CreatePuftBleachstone("PuftBleachstoneBaby", CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.BABY.NAME, CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.BABY.DESC, "baby_puft_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PuftBleachstone");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
