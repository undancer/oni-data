using TUNING;
using UnityEngine;

public class LiquidVentConfig : IBuildingConfig
{
	public const string ID = "LiquidVent";

	public const float OVERPRESSURE_MASS = 1000f;

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidVent", 1, 1, "ventliquid_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidVent");
		SoundEventVolumeCache.instance.AddVolume("ventliquid_kanim", "LiquidVent_squirt", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Exhaust>();
		go.AddOrGet<LogicOperationalController>();
		Vent vent = go.AddOrGet<Vent>();
		vent.conduitType = ConduitType.Liquid;
		vent.endpointType = Endpoint.Sink;
		vent.overpressureMass = 1000f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.ignoreMinMassCheck = true;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.showInUI = true;
		go.AddOrGet<SimpleVent>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<VentController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
