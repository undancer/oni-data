using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicHammerConfig : IBuildingConfig
{
	public static string ID = "LogicHammer";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "hammer_kanim", 30, 10f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.PermittedRotations = PermittedRotations.R360;
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 60f;
		obj.SelfHeatKilowattsWhenActive = 0f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.ViewMode = OverlayModes.Logic.ID;
		obj.AudioCategory = "Metal";
		obj.SceneLayer = Grid.SceneLayer.BuildingFront;
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort(LogicHammer.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.INPUT_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.LOGICHAMMER.INPUT_PORT_INACTIVE)
		};
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return obj;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicHammer>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
