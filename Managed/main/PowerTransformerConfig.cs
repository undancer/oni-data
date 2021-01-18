using TUNING;
using UnityEngine;

public class PowerTransformerConfig : IBuildingConfig
{
	public const string ID = "PowerTransformer";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PowerTransformer", 3, 2, "transformer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.UseWhitePowerOutputConnectorColour = true;
		buildingDef.PowerInputOffset = new CellOffset(-1, 1);
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.ElectricalArrowOffset = new CellOffset(1, 0);
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.Entombable = true;
		buildingDef.GeneratorWattageRating = 4000f;
		buildingDef.GeneratorBaseCapacity = 4000f;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddComponent<RequireInputs>();
		BuildingDef def = go.GetComponent<Building>().Def;
		Battery battery = go.AddOrGet<Battery>();
		battery.powerSortOrder = 1000;
		battery.capacity = def.GeneratorWattageRating;
		battery.chargeWattage = def.GeneratorWattageRating;
		PowerTransformer powerTransformer = go.AddComponent<PowerTransformer>();
		powerTransformer.powerDistributionOrder = 9;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<EnergyConsumer>());
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
