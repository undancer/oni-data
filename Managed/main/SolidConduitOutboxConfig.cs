using TUNING;
using UnityEngine;

public class SolidConduitOutboxConfig : IBuildingConfig
{
	public const string ID = "SolidConduitOutbox";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidConduitOutbox", 1, 2, "conveyorout_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.AudioCategory = "Metal";
		obj.InputConduitType = ConduitType.Solid;
		obj.UtilityInputOffset = new CellOffset(0, 0);
		obj.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitOutbox");
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<SolidConduitOutbox>();
		go.AddOrGet<SolidConduitConsumer>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.capacityKg = 100f;
		storage.showInUI = true;
		storage.allowItemRemoval = true;
		go.AddOrGet<SimpleVent>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Prioritizable.AddRef(go);
		go.AddOrGet<Automatable>();
	}
}
