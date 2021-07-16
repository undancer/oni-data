using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SolidConduitInboxConfig : IBuildingConfig
{
	public const string ID = "SolidConduitInbox";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("SolidConduitInbox", 1, 2, "conveyorin_kanim", 100, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		obj.RequiresPowerInput = true;
		obj.EnergyConsumptionWhenActive = 120f;
		obj.ExhaustKilowattsWhenActive = 0f;
		obj.SelfHeatKilowattsWhenActive = 2f;
		obj.Floodable = false;
		obj.ViewMode = OverlayModes.SolidConveyor.ID;
		obj.AudioCategory = "Metal";
		obj.OutputConduitType = ConduitType.Solid;
		obj.PowerInputOffset = new CellOffset(0, 1);
		obj.UtilityOutputOffset = new CellOffset(0, 0);
		obj.PermittedRotations = PermittedRotations.R360;
		obj.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitInbox");
		return obj;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<Automatable>();
		List<Tag> list = new List<Tag>();
		list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
		list.AddRange(STORAGEFILTERS.FOOD);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = list;
		storage.allowItemRemoval = false;
		storage.onlyTransferFromLowerPriority = true;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<SolidConduitInbox>();
		go.AddOrGet<SolidConduitDispenser>();
	}
}
