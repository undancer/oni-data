using STRINGS;
using UnityEngine;

public class BabyWormConfig : IEntityConfig
{
	public const string ID = "DivergentWormBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = DivergentWormConfig.CreateWorm("DivergentWormBaby", CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.BABY.NAME, CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.BABY.DESC, "baby_worm_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "DivergentWorm");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
