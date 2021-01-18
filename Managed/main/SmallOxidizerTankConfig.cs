using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SmallOxidizerTankConfig : IBuildingConfig
{
	public const string ID = "SmallOxidizerTank";

	public const float FuelCapacity = 900f;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SmallOxidizerTank", 3, 2, "rocket_oxidizer_tank_small_kanim", 1000, 30f, TUNING.BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER0, MATERIALS.ALL_METALS, 9999f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NOISY.TIER2, decor: TUNING.BUILDINGS.DECOR.NONE);
		buildingDef.RequiredDlcId = "EXPANSION1_ID";
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.DefaultAnimState = "grounded";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		buildingDef.ShowInBuildMenu = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, null)
		};
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 900f;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		FlatTagFilterable flatTagFilterable = go.AddOrGet<FlatTagFilterable>();
		flatTagFilterable.tagOptions = new List<Tag>
		{
			SimHashes.OxyRock.CreateTag(),
			SimHashes.Fertilizer.CreateTag()
		};
		flatTagFilterable.headerText = STRINGS.BUILDINGS.PREFABS.OXIDIZERTANK.UI_FILTER_CATEGORY;
		OxidizerTank oxidizerTank = go.AddOrGet<OxidizerTank>();
		oxidizerTank.consumeOnLand = !DlcManager.IsExpansion1Active();
		oxidizerTank.storage = storage;
		go.AddOrGet<Prioritizable>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<DropToUserCapacity>();
		BuildingTemplates.ExtendBuildingToRocketModule(go, ROCKETRY.BURDEN.MINOR, null);
	}
}
