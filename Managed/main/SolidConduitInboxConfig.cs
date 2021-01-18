using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SolidConduitInboxConfig : IBuildingConfig
{
	public const string ID = "SolidConduitInbox";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidConduitInbox", 1, 2, "conveyorin_kanim", 100, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitInbox");
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		Constructable component = go.GetComponent<Constructable>();
		component.requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
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
