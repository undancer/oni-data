using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EggIncubatorConfig : IBuildingConfig
{
	public const string ID = "EggIncubator";

	public static readonly List<Storage.StoredItemModifier> IncubatorStorage = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Preserve
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("EggIncubator", 2, 3, "incubator_kanim", 30, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.OverheatTemperature = 363.15f;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go);
		storage.SetDefaultStoredItemModifiers(IncubatorStorage);
		EggIncubator eggIncubator = go.AddOrGet<EggIncubator>();
		eggIncubator.AddDepositTag(GameTags.Egg);
		eggIncubator.SetWorkTime(5f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
