using UnityEngine;

public class RepairableStorageProxy : IEntityConfig
{
	public static string ID = "RepairableStorageProxy";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID);
		gameObject.AddOrGet<Storage>();
		gameObject.AddTag(GameTags.NotConversationTopic);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
