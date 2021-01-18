using UnityEngine;

public class ApproachableLocator : IEntityConfig
{
	public static readonly string ID = "ApproachableLocator";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
		gameObject.AddTag(GameTags.NotConversationTopic);
		gameObject.AddOrGet<Approachable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
