using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BaseModularLaunchpadPortConfig
{
	public static Tag LinkTag = new Tag("ModularLaunchpadPort");

	public static BuildingDef CreateBaseLaunchpadPort(string id, string anim, ConduitType conduitType, bool isLoader)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 3, anim, 1000, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
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
		if (isLoader)
		{
			buildingDef.InputConduitType = conduitType;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		}
		else
		{
			buildingDef.OutputConduitType = conduitType;
			buildingDef.UtilityOutputOffset = new CellOffset(1, 2);
		}
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = true;
		buildingDef.DefaultAnimState = "idle";
		buildingDef.CanMove = false;
		return buildingDef;
	}

	public static void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag, ConduitType conduitType, float storageSize, bool isLoader)
	{
		go.AddOrGet<LoopingSounds>();
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		component.AddTag(LinkTag);
		component.AddTag(GameTags.ModularConduitPort);
		component.AddTag(GameTags.NotRocketInteriorBuilding);
		ModularConduitPortController.Def def = go.AddOrGetDef<ModularConduitPortController.Def>();
		def.mode = (isLoader ? ModularConduitPortController.Mode.Load : ModularConduitPortController.Mode.Unload);
		if (!isLoader)
		{
			Storage storage = go.AddComponent<Storage>();
			storage.capacityKg = storageSize;
			storage.allowSettingOnlyFetchMarkedItems = false;
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			});
			switch (conduitType)
			{
			case ConduitType.Gas:
				storage.storageFilters = STORAGEFILTERS.GASES;
				break;
			case ConduitType.Liquid:
				storage.storageFilters = STORAGEFILTERS.LIQUIDS;
				break;
			default:
				storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
				break;
			}
			TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
			treeFilterable.dropIncorrectOnFilterChange = false;
			treeFilterable.autoSelectStoredOnLoad = false;
			if (conduitType == ConduitType.Solid)
			{
				SolidConduitDispenser solidConduitDispenser = go.AddOrGet<SolidConduitDispenser>();
				solidConduitDispenser.storage = storage;
				solidConduitDispenser.elementFilter = null;
			}
			else
			{
				ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
				conduitDispenser.storage = storage;
				conduitDispenser.conduitType = conduitType;
				conduitDispenser.elementFilter = null;
				conduitDispenser.alwaysDispense = true;
			}
		}
		else
		{
			Storage storage2 = go.AddComponent<Storage>();
			storage2.capacityKg = storageSize;
			storage2.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
			{
				Storage.StoredItemModifier.Hide,
				Storage.StoredItemModifier.Seal,
				Storage.StoredItemModifier.Insulate
			});
			if (conduitType == ConduitType.Solid)
			{
				SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
				solidConduitConsumer.storage = storage2;
				solidConduitConsumer.capacityTag = GameTags.Any;
				solidConduitConsumer.capacityKG = storageSize;
			}
			else
			{
				ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
				conduitConsumer.storage = storage2;
				conduitConsumer.conduitType = conduitType;
				conduitConsumer.capacityTag = GameTags.Any;
				conduitConsumer.capacityKG = storageSize;
			}
		}
		ChainedBuilding.Def def2 = go.AddOrGetDef<ChainedBuilding.Def>();
		def2.headBuildingTag = "LaunchPad".ToTag();
		def2.linkBuildingTag = LinkTag;
		def2.objectLayer = ObjectLayer.Building;
		go.AddOrGet<LogicOperationalController>();
	}

	public static void DoPostConfigureComplete(GameObject go, bool isLoader)
	{
	}
}
