using TUNING;
using UnityEngine;

public class SolarPanelModuleConfig : IBuildingConfig
{
	public const string ID = "SolarPanelModule";

	private static readonly CellOffset PLUG_OFFSET = new CellOffset(-1, 0);

	private const float EFFICIENCY_RATIO = 0.75f;

	public const float MAX_WATTS = 60f;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolarPanelModule", 3, 1, "rocket_solar_panel_module_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1, MATERIALS.GLASSES, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.DefaultAnimState = "grounded";
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.GeneratorWattageRating = 60f;
		obj.GeneratorBaseCapacity = obj.GeneratorWattageRating;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 0f;
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
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		go.AddOrGet<ModuleSolarPanel>().showConnectedConsumerStatusItems = false;
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.INSIGNIFICANT);
		go.GetComponent<RocketModule>().operationalLandedRequired = false;
	}
}
