using UnityEngine;

public class AsteroidConfig : IEntityConfig
{
	public const string ID = "Asteroid";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("Asteroid", "Asteroid");
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<WorldInventory>();
		gameObject.AddOrGet<WorldContainer>();
		gameObject.AddOrGet<AsteroidGridEntity>();
		gameObject.AddOrGetDef<GameplaySeasonManager.Def>();
		gameObject.AddOrGetDef<AlertStateManager.Def>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
