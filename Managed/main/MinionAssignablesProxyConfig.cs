using UnityEngine;

public class MinionAssignablesProxyConfig : IEntityConfig
{
	public static string ID = "MinionAssignablesProxy";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<Ownables>();
		gameObject.AddOrGet<Equipment>();
		gameObject.AddOrGet<MinionAssignablesProxy>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
