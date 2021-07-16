using TUNING;
using UnityEngine;

public class RocketInteriorGasInputConfig : IBuildingConfig
{
	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private const CargoBay.CargoType CARGO_TYPE = CargoBay.CargoType.Gasses;

	public const string ID = "RocketInteriorGasInput";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("RocketInteriorGasInput", 1, 1, "rocket_floor_plug_gas_kanim", 30, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnRocketEnvelope, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		obj.InputConduitType = ConduitType.Gas;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.Overheatable = false;
		obj.Floodable = false;
		obj.Entombable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.ObjectLayer = ObjectLayer.Building;
		obj.SceneLayer = Grid.SceneLayer.Building;
		obj.AudioCategory = "Metal";
		obj.AudioSize = "small";
		obj.BaseTimeUntilRepair = -1f;
		obj.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "RocketInteriorGasInput");
		return obj;
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
		storage.capacityKg = 1f;
		RocketConduitStorageAccess rocketConduitStorageAccess = go.AddOrGet<RocketConduitStorageAccess>();
		rocketConduitStorageAccess.storage = storage;
		rocketConduitStorageAccess.cargoType = CargoBay.CargoType.Gasses;
		rocketConduitStorageAccess.targetLevel = 0f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.ignoreMinMassCheck = true;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.capacityKG = storage.capacityKg;
	}
}
