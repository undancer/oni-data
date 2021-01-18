using STRINGS;
using UnityEngine;

public class BabyPuftConfig : IEntityConfig
{
	public const string ID = "PuftBaby";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftConfig.CreatePuft("PuftBaby", CREATURES.SPECIES.PUFT.BABY.NAME, CREATURES.SPECIES.PUFT.BABY.DESC, "baby_puft_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Puft");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
