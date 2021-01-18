using Klei.AI;
using TUNING;
using UnityEngine;

public class StaterpillarGeneratorConfig : IBuildingConfig
{
	public static readonly string ID = "StaterpillarGenerator";

	private const int WIDTH = 1;

	private const int HEIGHT = 2;

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, construction_materials: aLL_METALS, melting_point: 9999f, build_location_rule: BuildLocationRule.OnFoundationRotatable, noise: NOISE_POLLUTION.NOISY.TIER0, id: iD, width: 1, height: 2, anim: "egg_caterpillar_kanim", hitpoints: 100, construction_time: 10f, decor: BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		buildingDef.GeneratorWattageRating = 1600f;
		buildingDef.GeneratorBaseCapacity = 5000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.Overheatable = true;
		buildingDef.Floodable = false;
		buildingDef.OverheatTemperature = 423.15f;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Plastic";
		buildingDef.PowerOutputOffset = new CellOffset(0, 1);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		StaterpillarGenerator staterpillarGenerator = go.AddOrGet<StaterpillarGenerator>();
		staterpillarGenerator.powerDistributionOrder = 9;
		Deconstructable component = go.GetComponent<Deconstructable>();
		component.SetAllowDeconstruction(allow: false);
		go.AddOrGet<Modifiers>();
		go.AddOrGet<Effects>();
		go.GetComponent<KSelectable>().IsSelectable = false;
	}
}
