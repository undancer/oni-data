using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ScoutModuleConfig : IBuildingConfig
{
	public const string ID = "ScoutModule";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("ScoutModule", 3, 3, "rocket_scout_cargo_module_kanim", 1000, 30f, BUILDINGS.ROCKETRY_MASS_KG.HOLLOW_TIER1, MATERIALS.RAW_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateRocketBuildingDef(obj);
		obj.DefaultAnimState = "deployed";
		obj.AttachmentSlotTag = GameTags.Rocket;
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.OverheatTemperature = 2273.15f;
		obj.Floodable = false;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.RequiresPowerInput = false;
		obj.CanMove = true;
		obj.Cancellable = false;
		return obj;
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
		def.outputIDs = new List<string> { "ScoutLander", "ScoutRover" };
		def.spawnIntoStorage = true;
		def.storage = storage;
		def.constructionSymbol = "under_construction";
		go.AddOrGet<BuildingInternalConstructorWorkable>().SetWorkTime(30f);
		JettisonableCargoModule.Def def2 = go.AddOrGetDef<JettisonableCargoModule.Def>();
		def2.landerPrefabID = "ScoutLander".ToTag();
		def2.landerContainer = storage;
		def2.clusterMapFXPrefabID = "DeployingScoutLanderFXConfig";
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MODERATE);
	}
}
