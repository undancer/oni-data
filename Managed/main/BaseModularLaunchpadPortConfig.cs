using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BaseModularLaunchpadPortConfig
{
	public static Tag LinkTag = new Tag("ModularLaunchpadPort");

	private const float STORAGE_SIZE = 10f;

	public static BuildingDef CreateBaseLaunchpadPort(string id, string anim, ConduitType conduitType)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 3, anim, 1000, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		switch (conduitType)
		{
		case ConduitType.Gas:
			buildingDef.ViewMode = OverlayModes.GasConduits.ID;
			break;
		case ConduitType.Liquid:
			buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
			break;
		case ConduitType.Solid:
			buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
			break;
		}
		buildingDef.InputConduitType = conduitType;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.OutputConduitType = conduitType;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 2);
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = true;
		buildingDef.DefaultAnimState = "idle";
		buildingDef.CanMove = false;
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		return buildingDef;
	}

	public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag, ConduitType conduitType)
	{
		go.AddOrGet<LoopingSounds>();
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		component.AddTag(LinkTag);
		go.AddOrGetDef<ModularConduitPortController.Def>();
		Storage storage = go.AddComponent<Storage>();
		storage.capacityKg = 10f;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 10f;
		storage2.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		if (conduitType == ConduitType.Solid)
		{
			SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
			solidConduitConsumer.storage = storage;
			solidConduitConsumer.capacityTag = GameTags.Any;
			solidConduitConsumer.capacityKG = 10f;
			SolidConduitDispenser solidConduitDispenser = go.AddOrGet<SolidConduitDispenser>();
			solidConduitDispenser.storage = storage2;
			solidConduitDispenser.elementFilter = null;
		}
		else
		{
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.storage = storage;
			conduitConsumer.conduitType = conduitType;
			conduitConsumer.capacityTag = GameTags.Any;
			conduitConsumer.capacityKG = 10f;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.storage = storage2;
			conduitDispenser.conduitType = conduitType;
			conduitDispenser.elementFilter = null;
		}
		ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
		def.headBuildingTag = "LaunchPad".ToTag();
		def.linkBuildingTag = LinkTag;
		def.objectLayer = ObjectLayer.Building;
		go.AddOrGet<ModularConduitPortTiler>();
	}

	public static void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}
}
