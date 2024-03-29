using STRINGS;
using UnityEngine;

public class BabyDivergentBeetleConfig : IEntityConfig
{
	public const string ID = "DivergentBeetleBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = DivergentBeetleConfig.CreateDivergentBeetle("DivergentBeetleBaby", CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.BABY.NAME, CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.BABY.DESC, "baby_critter_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "DivergentBeetle");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
