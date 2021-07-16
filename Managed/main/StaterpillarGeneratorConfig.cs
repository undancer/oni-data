using Klei.AI;
using TUNING;
using UnityEngine;

public class StaterpillarGeneratorConfig : IBuildingConfig
{
	public static readonly string ID = "StaterpillarGenerator";

	private const int WIDTH = 1;

	private const int HEIGHT = 2;

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string iD = ID;
		string[] aLL_METALS = MATERIALS.ALL_METALS;
		BuildingDef obj = BuildingTemplates.CreateBuildingDef(construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, construction_materials: aLL_METALS, melting_point: 9999f, build_location_rule: BuildLocationRule.OnFoundationRotatable, noise: NOISE_POLLUTION.NOISY.TIER0, id: iD, width: 1, height: 2, anim: "egg_caterpillar_kanim", hitpoints: 100, construction_time: 10f, decor: BUILDINGS.DECOR.NONE);
		obj.GeneratorWattageRating = 1600f;
		obj.GeneratorBaseCapacity = 5000f;
		obj.ExhaustKilowattsWhenActive = 2f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.Overheatable = true;
		obj.Floodable = false;
		obj.OverheatTemperature = 423.15f;
		obj.PermittedRotations = PermittedRotations.FlipV;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Plastic";
		obj.RequiresPowerOutput = true;
		obj.PowerOutputOffset = new CellOffset(0, 1);
		obj.PlayConstructionSounds = false;
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<StaterpillarGenerator>().powerDistributionOrder = 9;
		go.GetComponent<Deconstructable>().SetAllowDeconstruction(allow: false);
		go.AddOrGet<Modifiers>();
		go.AddOrGet<Effects>();
		go.GetComponent<KSelectable>().IsSelectable = false;
	}
}
