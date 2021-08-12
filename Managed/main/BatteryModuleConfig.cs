using TUNING;
using UnityEngine;

public class BatteryModuleConfig : IBuildingConfig
{
	public const string ID = "BatteryModule";

	public const float NUM_CAPSULES = 3f;

	private static readonly CellOffset PLUG_OFFSET = new CellOffset(-1, 0);

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("BatteryModule", 3, 2, "rocket_battery_pack_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER2, MATERIALS.RAW_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.DefaultAnimState = "grounded";
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.ForegroundLayer = Grid.SceneLayer.Front;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.PowerInputOffset = PLUG_OFFSET;
		obj.PowerOutputOffset = PLUG_OFFSET;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerOutput = true;
		obj.UseWhitePowerOutputConnectorColour = true;
		obj.CanMove = true;
		obj.Cancellable = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddComponent<RequireInputs>();
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		ModuleBattery moduleBattery = go.AddOrGet<ModuleBattery>();
		moduleBattery.capacity = 100000f;
		moduleBattery.joulesLostPerSecond = 2f / 3f;
		WireUtilitySemiVirtualNetworkLink wireUtilitySemiVirtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
		wireUtilitySemiVirtualNetworkLink.link1 = PLUG_OFFSET;
		wireUtilitySemiVirtualNetworkLink.visualizeOnly = true;
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR);
	}
}
