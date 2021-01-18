using UnityEngine;

public class TemporalTearConfig : IEntityConfig
{
	public const string ID = "TemporalTear";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("TemporalTear", "TemporalTear");
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<TemporalTear>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
