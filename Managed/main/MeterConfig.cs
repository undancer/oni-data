using UnityEngine;

public class MeterConfig : IEntityConfig
{
	public static readonly string ID = "Meter";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
		gameObject.AddOrGet<KBatchedAnimController>();
		gameObject.AddOrGet<KBatchedAnimTracker>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
