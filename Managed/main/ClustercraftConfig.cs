using UnityEngine;

public class ClustercraftConfig : IEntityConfig
{
	public const string ID = "Clustercraft";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("Clustercraft", "Clustercraft");
		SaveLoadRoot saveLoadRoot = gameObject.AddOrGet<SaveLoadRoot>();
		saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
		saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
		saveLoadRoot.DeclareOptionalComponent<OrbitalMechanics>();
		gameObject.AddOrGet<Clustercraft>();
		gameObject.AddOrGet<CraftModuleInterface>();
		RocketClusterDestinationSelector rocketClusterDestinationSelector = gameObject.AddOrGet<RocketClusterDestinationSelector>();
		rocketClusterDestinationSelector.requireLaunchPadOnAsteroidDestination = true;
		rocketClusterDestinationSelector.assignable = true;
		rocketClusterDestinationSelector.shouldPointTowardsPath = true;
		gameObject.AddOrGet<ClusterTraveler>().stopAndNotifyWhenPathChanges = true;
		gameObject.AddOrGetDef<AlertStateManager.Def>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGetDef<RocketSelfDestructMonitor.Def>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
