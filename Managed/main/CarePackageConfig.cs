using STRINGS;
using UnityEngine;

public class CarePackageConfig : IEntityConfig
{
	public static readonly string ID = "CarePackage";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateLooseEntity(ID, ITEMS.CARGO_CAPSULE.NAME, ITEMS.CARGO_CAPSULE.DESC, 1f, unitMass: true, Assets.GetAnim("portal_carepackage_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE);
	}

	public void OnPrefabInit(GameObject go)
	{
		go.AddOrGet<CarePackage>();
	}

	public void OnSpawn(GameObject go)
	{
	}
}
