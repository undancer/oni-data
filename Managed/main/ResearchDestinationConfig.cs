using UnityEngine;

public class ResearchDestinationConfig : IEntityConfig
{
	public const string ID = "ResearchDestination";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("ResearchDestination", "ResearchDestination");
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<ResearchDestination>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
