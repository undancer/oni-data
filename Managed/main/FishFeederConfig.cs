using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class FishFeederConfig : IBuildingConfig
{
	public const string ID = "FishFeeder";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishFeeder", 1, 3, "fishfeeder_kanim", 100, 120f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = true;
		buildingDef.Floodable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CreatureFeeder);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 200f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 200f;
		storage2.showInUI = true;
		storage2.showDescriptor = true;
		storage2.allowItemRemoval = false;
		StorageLocker storageLocker = go.AddOrGet<StorageLocker>();
		storageLocker.choreTypeID = Db.Get().ChoreTypes.RanchingFetch.Id;
		go.AddOrGet<UserNameable>();
		Effect effect = new Effect("AteFromFeeder", STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.TOOLTIP, 600f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		effect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -71f / (678f * (float)Math.PI), STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME));
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 2f, STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME));
		Db.Get().effects.Add(effect);
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<CreatureFeeder>().effectId = effect.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
		go.AddOrGetDef<FishFeeder.Def>();
		go.AddOrGetDef<MakeBaseSolid.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Diet> item in DietManager.CollectDiets(new Tag[1]
		{
			GameTags.Creatures.Species.PacuSpecies
		}))
		{
			list.Add(item.Key);
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}
}
