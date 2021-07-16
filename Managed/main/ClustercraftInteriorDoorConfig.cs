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
		GameObject obj = EntityTemplates.CreatePlacedEntity(ID, STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.NAME, STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.DESC, 400f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("rocket_hatch_door_kanim"), initialAnim: "closed", sceneLayer: Grid.SceneLayer.TileFront, width: 1, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		obj.AddTag(GameTags.NotRoomAssignable);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		obj.AddOrGet<Operational>();
		obj.AddOrGet<LoopingSounds>();
		obj.AddOrGet<Prioritizable>();
		KBatchedAnimController kBatchedAnimController = obj.AddOrGet<KBatchedAnimController>();
		kBatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kBatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		obj.AddOrGet<ClustercraftInteriorDoor>();
		obj.AddOrGet<AssignmentGroupController>().generateGroupOnStart = false;
		obj.AddOrGet<NavTeleporter>().offset = new CellOffset(1, 0);
		obj.AddOrGet<AccessControl>();
		return obj;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
