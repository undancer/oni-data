using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ScoutModuleConfig : IBuildingConfig
{
	public const string ID = "ScoutModule";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ScoutModule", 3, 3, "rocket_scout_cargo_module_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1, MATERIALS.RAW_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.DefaultAnimState = "deployed";
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		BuildingInternalConstructor.Def def = go.AddOrGetDef<BuildingInternalConstructor.Def>();
		def.constructionMass = 500f;
		def.outputIDs = new List<string>
		{
			"ScoutLander",
			"ScoutRover"
		};
		def.spawnIntoStorage = true;
		def.storage = storage;
		BuildingInternalConstructorWorkable buildingInternalConstructorWorkable = go.AddOrGet<BuildingInternalConstructorWorkable>();
		buildingInternalConstructorWorkable.SetWorkTime(30f);
		JettisonableCargoModule.Def def2 = go.AddOrGetDef<JettisonableCargoModule.Def>();
		def2.landerPrefabID = "ScoutLander".ToTag();
		def2.landerContainer = storage;
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		BuildingTemplates.ExtendBuildingToRocketModule(go, ROCKETRY.BURDEN.MODERATE, null);
	}
}
