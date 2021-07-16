using TUNING;
using UnityEngine;

public class PowerTransformerConfig : IBuildingConfig
{
	public const string ID = "PowerTransformer";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("PowerTransformer", 3, 2, "transformer_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER5, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.RequiresPowerOutput = true;
		obj.UseWhitePowerOutputConnectorColour = true;
		obj.PowerInputOffset = new CellOffset(-1, 1);
		obj.PowerOutputOffset = new CellOffset(1, 0);
		obj.ElectricalArrowOffset = new CellOffset(1, 0);
		obj.ExhaustKilowattsWhenActive = 0.25f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.ViewMode = OverlayModes.Power.ID;
		obj.AudioCategory = "Metal";
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 1f;
		obj.Entombable = true;
		obj.GeneratorWattageRating = 4000f;
		obj.GeneratorBaseCapacity = 4000f;
		obj.PermittedRotations = PermittedRotations.FlipH;
		return obj;
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
		go.AddComponent<PowerTransformer>().powerDistributionOrder = 9;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Object.DestroyImmediate(go.GetComponent<EnergyConsumer>());
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
