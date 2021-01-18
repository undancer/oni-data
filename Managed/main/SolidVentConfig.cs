using TUNING;
using UnityEngine;

public class SolidVentConfig : IBuildingConfig
{
	public const string ID = "SolidVent";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidVent", 1, 1, "conveyer_dropper_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.InputConduitType = ConduitType.Solid;
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.AudioCategory = "Metal";
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidVent");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LogicOperationalController>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<SimpleVent>();
		go.AddOrGet<SolidConduitConsumer>();
		go.AddOrGet<SolidConduitDropper>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.capacityKg = 100f;
		storage.showInUI = true;
	}
}
