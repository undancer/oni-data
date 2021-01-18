using UnityEngine;

public class TargetLocator : IEntityConfig
{
	public static readonly string ID = "TargetLocator";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
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
