using TUNING;
using UnityEngine;

public class GasVentHighPressureConfig : IBuildingConfig
{
	public const string ID = "GasVentHighPressure";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public const float OVERPRESSURE_MASS = 20f;

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2] { "RefinedMetal", "Plastic" };
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, construction_materials: array, melting_point: 1600f, build_location_rule: BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, id: "GasVentHighPressure", width: 1, height: 1, anim: "ventgas_powered_kanim", hitpoints: 30, construction_time: 30f, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.InputConduitType = ConduitType.Gas;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasVentHighPressure");
		SoundEventVolumeCache.instance.AddVolume("ventgas_kanim", "GasVent_clunk", NOISE_POLLUTION.NOISY.TIER0);
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Exhaust>();
		go.AddOrGet<LogicOperationalController>();
		Vent vent = go.AddOrGet<Vent>();
		vent.conduitType = ConduitType.Gas;
		vent.endpointType = Endpoint.Sink;
		vent.overpressureMass = 20f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.ignoreMinMassCheck = true;
		BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
		go.AddOrGet<SimpleVent>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<VentController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
	}
}
