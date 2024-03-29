using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class TeleportalPadConfig : IBuildingConfig
{
	public const string ID = "TeleportalPad";

	public const string PORTAL_ID_PORT_0 = "TeleportalPad_ID_PORT_0";

	public const string PORTAL_ID_PORT_1 = "TeleportalPad_ID_PORT_1";

	public const string PORTAL_ID_PORT_2 = "TeleportalPad_ID_PORT_2";

	public const string PORTAL_ID_PORT_3 = "TeleportalPad_ID_PORT_3";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("TeleportalPad", 4, 4, "hqbase_kanim", 250, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER5);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.AudioCategory = "Metal";
		obj.BaseTimeUntilRepair = 400f;
		obj.DefaultAnimState = "idle";
		obj.RequiresPowerInput = true;
		obj.PowerInputOffset = new CellOffset(2, 0);
		obj.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort("TeleportalPad_ID_PORT_0", new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
			LogicPorts.Port.InputPort("TeleportalPad_ID_PORT_1", new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
			LogicPorts.Port.InputPort("TeleportalPad_ID_PORT_2", new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
			LogicPorts.Port.InputPort("TeleportalPad_ID_PORT_3", new CellOffset(2, 0), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE),
			LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 1), STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.TELEPORTALPAD.LOGIC_PORT_INACTIVE)
		};
		obj.EnergyConsumptionWhenActive = 1600f;
		obj.ExhaustKilowattsWhenActive = 16f;
		obj.SelfHeatKilowattsWhenActive = 64f;
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
		SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<TeleportalPad>();
		go.AddOrGet<Teleporter>();
		go.AddOrGet<PrimaryElement>().SetElement(SimHashes.Unobtanium);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}
}
