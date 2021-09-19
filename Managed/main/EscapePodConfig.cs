using STRINGS;
using TUNING;
using UnityEngine;

public class EscapePodConfig : IEntityConfig
{
	public const string ID = "EscapePod";

	public const float MASS = 100f;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("EscapePod", STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.NAME, STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.DESC, 100f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("escape_pod_kanim"), initialAnim: "grounded", sceneLayer: Grid.SceneLayer.Building, width: 1, height: 2);
		obj.AddOrGet<KBatchedAnimController>().fgLayer = Grid.SceneLayer.BuildingFront;
		TravellingCargoLander.Def def = obj.AddOrGetDef<TravellingCargoLander.Def>();
		def.landerWidth = 1;
		def.landingSpeed = 15f;
		def.deployOnLanding = true;
		CargoDropperMinion.Def def2 = obj.AddOrGetDef<CargoDropperMinion.Def>();
		def2.kAnimName = "anim_interacts_escape_pod_kanim";
		def2.animName = "deploying";
		def2.animLayer = Grid.SceneLayer.BuildingUse;
		def2.notifyOnJettison = true;
		BallisticClusterGridEntity ballisticClusterGridEntity = obj.AddOrGet<BallisticClusterGridEntity>();
		ballisticClusterGridEntity.clusterAnimName = "escape_pod01_kanim";
		ballisticClusterGridEntity.isWorldEntity = true;
		ballisticClusterGridEntity.nameKey = new StringKey("STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.NAME");
		ClusterDestinationSelector clusterDestinationSelector = obj.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = false;
		clusterDestinationSelector.shouldPointTowardsPath = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		clusterDestinationSelector.canNavigateFogOfWar = true;
		obj.AddOrGet<ClusterTraveler>();
		obj.AddOrGet<MinionStorage>();
		obj.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(obj);
		obj.AddOrGet<Operational>();
		obj.AddOrGet<Deconstructable>().audioSize = "large";
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.ApplyToCells = false;
		component.objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
