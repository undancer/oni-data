using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicHammerConfig : IBuildingConfig
{
	public static string ID = "LogicHammer";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hammer_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicHammer.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.INPUT_PORT_INACTIVE)
		};
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicHammer>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
