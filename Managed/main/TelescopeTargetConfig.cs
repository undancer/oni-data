using UnityEngine;

public class TelescopeTargetConfig : IEntityConfig
{
	public const string ID = "TelescopeTarget";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("TelescopeTarget", "TelescopeTarget");
		gameObject.AddOrGet<TelescopeTarget>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
