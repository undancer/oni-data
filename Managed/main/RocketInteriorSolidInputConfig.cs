using TUNING;
using UnityEngine;

public class RocketInteriorSolidInputConfig : IBuildingConfig
{
	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	private const CargoBay.CargoType CARGO_TYPE = CargoBay.CargoType.Solids;

	public const string ID = "RocketInteriorSolidInput";

	protected virtual string GetID()
	{
		return "RocketInteriorSolidInput";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(GetID(), 1, 1, "rocket_floor_plug_solid_kanim", 30, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnRocketEnvelope, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "RocketInteriorSolidInput");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		go.GetComponent<KPrefabID>().AddTag(GameTags.RocketInteriorBuilding);
		go.AddComponent<RequireInputs>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<ActiveController.Def>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 20f;
		RocketConduitStorageAccess rocketConduitStorageAccess = go.AddOrGet<RocketConduitStorageAccess>();
		rocketConduitStorageAccess.storage = storage;
		rocketConduitStorageAccess.cargoType = CargoBay.CargoType.Solids;
		rocketConduitStorageAccess.targetLevel = 0f;
		SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
		solidConduitConsumer.alwaysConsume = true;
		solidConduitConsumer.capacityKG = storage.capacityKg;
	}
}
