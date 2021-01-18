using UnityEngine;

public class ClustercraftConfig : IEntityConfig
{
	public const string ID = "Clustercraft";

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("Clustercraft", "Clustercraft");
		SaveLoadRoot saveLoadRoot = gameObject.AddOrGet<SaveLoadRoot>();
		saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
		saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
		gameObject.AddOrGet<Clustercraft>();
		gameObject.AddOrGet<CraftModuleInterface>();
		RocketClusterDestinationSelector rocketClusterDestinationSelector = gameObject.AddOrGet<RocketClusterDestinationSelector>();
		rocketClusterDestinationSelector.requireLaunchPadOnAsteroidDestination = true;
		rocketClusterDestinationSelector.assignable = true;
		rocketClusterDestinationSelector.shouldPointTowardsPath = true;
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
