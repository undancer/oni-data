using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class BabyPacuConfig : IEntityConfig
{
	public const string ID = "PacuBaby";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PacuConfig.CreatePacu("PacuBaby", CREATURES.SPECIES.PACU.BABY.NAME, CREATURES.SPECIES.PACU.BABY.DESC, "baby_pacu_kanim", is_baby: true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Pacu");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
