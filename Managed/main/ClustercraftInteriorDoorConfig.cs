using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClustercraftInteriorDoorConfig : IEntityConfig
{
	public static string ID = "ClustercraftInteriorDoor";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.NAME, STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.DESC, 400f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_hatch_door_kanim"), initialAnim: "closed", sceneLayer: Grid.SceneLayer.TileFront, width: 1, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		gameObject.AddTag(GameTags.NotRoomAssignable);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		gameObject.AddOrGet<ClustercraftInteriorDoor>();
		AssignmentGroupController assignmentGroupController = gameObject.AddOrGet<AssignmentGroupController>();
		assignmentGroupController.generateGroupOnStart = false;
		NavTeleporter navTeleporter = gameObject.AddOrGet<NavTeleporter>();
		navTeleporter.offset = new CellOffset(1, 0);
		gameObject.AddOrGet<AccessControl>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
