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

	private const string LANDED_ROCKET_ID = "LandedRocket";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("LaunchPad", 7, 2, "rocket_launchpad_kanim", 1000, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: TUNING.BUILDINGS.DECOR.NONE);
		obj.SceneLayer = Grid.SceneLayer.BuildingBack;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.UseStructureTemperature = false;
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.attachablePosition = new CellOffset(0, 0);
		obj.RequiresPowerInput = false;
		obj.DefaultAnimState = "idle";
		obj.CanMove = false;
		obj.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort("TriggerLaunch", new CellOffset(-1, 0), STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_ACTIVE, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH_INACTIVE) };
		obj.LogicOutputPorts = new List<LogicPorts.Port>
		{
			LogicPorts.Port.OutputPort("LaunchReady", new CellOffset(1, 0), STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_ACTIVE, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_READY_INACTIVE),
			LogicPorts.Port.OutputPort("LandedRocket", new CellOffset(0, 1), STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_ACTIVE, STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LANDED_ROCKET_INACTIVE)
		};
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.GetComponent<KPrefabID>().AddTag(GameTags.NotRocketInteriorBuilding);
		go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		LaunchPad launchPad = go.AddOrGet<LaunchPad>();
		launchPad.triggerPort = "TriggerLaunch";
		launchPad.statusPort = "LaunchReady";
		launchPad.landedRocketPort = "LandedRocket";
		FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
		fakeFloorAdder.floorOffsets = new CellOffset[7];
		for (int i = 0; i < 7; i++)
		{
			fakeFloorAdder.floorOffsets[i] = new CellOffset(i - 3, 1);
		}
		go.AddOrGet<LaunchPadConditions>();
		ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
		def.headBuildingTag = "LaunchPad".ToTag();
		def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
		def.objectLayer = ObjectLayer.Building;
		go.AddOrGetDef<LaunchPadMaterialDistributor.Def>();
		go.AddOrGet<UserNameable>();
		go.AddOrGet<CharacterOverlay>().shouldShowName = true;
		ModularConduitPortTiler modularConduitPortTiler = go.AddOrGet<ModularConduitPortTiler>();
		modularConduitPortTiler.manageRightCap = true;
		modularConduitPortTiler.manageLeftCap = false;
		modularConduitPortTiler.leftCapDefaultSceneLayerAdjust = 1;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
