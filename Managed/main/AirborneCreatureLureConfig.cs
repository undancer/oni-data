using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AirborneCreatureLureConfig : IBuildingConfig
{
	public const string ID = "AirborneCreatureLure";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AirborneCreatureLure", 1, 4, "airbornecreaturetrap_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Deprecated = true;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject prefab, Tag prefab_tag)
	{
		CreatureLure creatureLure = prefab.AddOrGet<CreatureLure>();
		creatureLure.baitStorage = prefab.AddOrGet<Storage>();
		creatureLure.baitTypes = new List<Tag>
		{
			GameTags.SlimeMold,
			GameTags.Phosphorite
		};
		creatureLure.baitStorage.storageFilters = creatureLure.baitTypes;
		creatureLure.baitStorage.allowItemRemoval = false;
		creatureLure.baitStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		prefab.AddOrGet<Operational>();
	}

	public override void DoPostConfigureComplete(GameObject prefab)
	{
		BuildingTemplates.DoPostConfigure(prefab);
		SymbolOverrideControllerUtil.AddToPrefab(prefab);
		prefab.AddOrGet<LogicOperationalController>();
		Lure.Def def = prefab.AddOrGetDef<Lure.Def>();
		def.lurePoints = new CellOffset[13]
		{
			new CellOffset(0, 0),
			new CellOffset(-1, 4),
			new CellOffset(0, 4),
			new CellOffset(1, 4),
			new CellOffset(-2, 3),
			new CellOffset(-1, 3),
			new CellOffset(0, 3),
			new CellOffset(1, 3),
			new CellOffset(2, 3),
			new CellOffset(-1, 2),
			new CellOffset(0, 2),
			new CellOffset(1, 2),
			new CellOffset(0, 1)
		};
		def.radius = 32;
		Prioritizable.AddRef(prefab);
	}
}
