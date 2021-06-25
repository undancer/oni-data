using STRINGS;
using UnityEngine;

public class BabyBeeConfig : IEntityConfig
{
	public const string ID = "BeeBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = BeeConfig.CreateBee("BeeBaby", CREATURES.SPECIES.BEE.BABY.NAME, CREATURES.SPECIES.BEE.BABY.DESC, "baby_blarva_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Bee", null, force_adult_nav_type: true, 2f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseBeeConfig.SetupLoopingSounds(inst);
	}
}
