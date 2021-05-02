using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LaunchPadConfig : IBuildingConfig
{
	public const string ID = "LaunchPad";

	private const int WIDTH = 7;

	private const string TRIGGER_LAUNCH_PORT_ID = "TriggerLaunch";

	private const string LAUNCH_READY_PORT_ID = "LaunchReady";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LaunchPad", 7, 2, "rocket_launchpad_kanim", 1000, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: TUNING.BUILDINGS.DECOR.NONE);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.UseStructureTemperature = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.DefaultAnimState = "idle";
		buildingDef.CanMove = false;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.InputPort("TriggerLaunch", new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_ACTIVE, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_INACTIVE)
		};
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort("LaunchReady", new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_ACTIVE, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_INACTIVE)
		};
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		LaunchPad launchPad = go.AddOrGet<LaunchPad>();
		launchPad.triggerPort = "TriggerLaunch";
		launchPad.statusPort = "LaunchReady";
		MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
		def.solidOffsets = new CellOffset[7];
		for (int i = 0; i < 7; i++)
		{
			def.solidOffsets[i] = new CellOffset(i - 3, 1);
		}
		go.AddOrGet<LaunchPadConditions>();
		ChainedBuilding.Def def2 = go.AddOrGetDef<ChainedBuilding.Def>();
		def2.headBuildingTag = "LaunchPad".ToTag();
		def2.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
		def2.objectLayer = ObjectLayer.Building;
		go.AddOrGetDef<LaunchPadMaterialDistributor.Def>();
		go.AddOrGet<UserNameable>();
		CharacterOverlay characterOverlay = go.AddOrGet<CharacterOverlay>();
		characterOverlay.shouldShowName = true;
		ModularConduitPortTiler modularConduitPortTiler = go.AddOrGet<ModularConduitPortTiler>();
		modularConduitPortTiler.manageRightCap = true;
		modularConduitPortTiler.manageLeftCap = false;
		modularConduitPortTiler.leftCapDefaultSceneLayerAdjust = 1;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.DontBlockRockets);
	}
}
