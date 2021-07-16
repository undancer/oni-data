using TUNING;
using UnityEngine;

public class GasVentConfig : IBuildingConfig
{
	public const string ID = "GasVent";

	public const float OVERPRESSURE_MASS = 2f;

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GasVent", 1, 1, "ventgas_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.InputConduitType = ConduitType.Gas;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.GasConduits.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasVent");
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
		vent.overpressureMass = 2f;
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
