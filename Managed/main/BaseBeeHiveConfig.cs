using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BaseBeeHiveConfig : IEntityConfig
{
	public const string ID = "BeeHive";

	public const string BASE_TRAIT_ID = "BeeHiveBaseTrait";

	private const int WIDTH = 2;

	private const int HEIGHT = 3;

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BeeHive", STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC, 100f, Assets.GetAnim("beehive_kanim"), "grow_pre", Grid.SceneLayer.BuildingBack, 2, 3, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, TUNING.CREATURES.TEMPERATURE.FREEZING_3);
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Experimental);
		kPrefabID.AddTag(GameTags.Creature);
		if (Sim.IsRadiationEnabled())
		{
			Storage storage = gameObject.AddOrGet<Storage>();
			storage.storageFXOffset = new Vector3(1f, 1f, 0f);
			BeeHive.Def def = gameObject.AddOrGetDef<BeeHive.Def>();
			def.beePrefabID = "Bee";
			def.larvaPrefabID = "BeeBaby";
			KAnimFile[] overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_beehive_kanim")
			};
			HiveWorkableEmpty hiveWorkableEmpty = gameObject.AddOrGet<HiveWorkableEmpty>();
			hiveWorkableEmpty.workTime = 15f;
			hiveWorkableEmpty.overrideAnims = overrideAnims;
			hiveWorkableEmpty.workLayer = Grid.SceneLayer.BuildingFront;
			RadiationEmitter radiationEmitter = gameObject.AddComponent<RadiationEmitter>();
			radiationEmitter.emitRadiusX = 7;
			radiationEmitter.emitRadiusY = 6;
			radiationEmitter.emitRate = 3f;
			radiationEmitter.emitRads = 0f;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Pulsing;
			radiationEmitter.emissionOffset = new Vector3(0.5f, 1f, 0f);
			gameObject.AddOrGet<Traits>();
			gameObject.AddOrGet<Health>();
			gameObject.AddOrGet<CharacterOverlay>();
			gameObject.AddOrGet<RangedAttackable>();
			gameObject.AddOrGet<FactionAlignment>().Alignment = FactionManager.FactionID.Hostile;
			gameObject.AddOrGet<Prioritizable>();
			Prioritizable.AddRef(gameObject);
			gameObject.AddOrGet<Effects>();
			gameObject.AddOrGet<TemperatureVulnerable>().Configure(TUNING.CREATURES.TEMPERATURE.FREEZING_9, TUNING.CREATURES.TEMPERATURE.FREEZING_10, TUNING.CREATURES.TEMPERATURE.FREEZING_1, TUNING.CREATURES.TEMPERATURE.FREEZING);
			DrowningMonitor drowningMonitor = gameObject.AddOrGet<DrowningMonitor>();
			drowningMonitor.canDrownToDeath = false;
			gameObject.AddOrGet<EntombVulnerable>();
			gameObject.AddOrGetDef<DeathMonitor.Def>();
			gameObject.AddOrGetDef<AnimInterruptMonitor.Def>();
			gameObject.AddOrGetDef<HiveGrowthMonitor.Def>();
			FoundationMonitor foundationMonitor = gameObject.AddOrGet<FoundationMonitor>();
			foundationMonitor.monitorCells = new CellOffset[2]
			{
				new CellOffset(0, -1),
				new CellOffset(1, -1)
			};
			HiveEatingMonitor.Def def2 = gameObject.AddOrGetDef<HiveEatingMonitor.Def>();
			def2.consumedOre = BeeHiveTuning.CONSUMED_ORE;
			HiveHarvestMonitor.Def def3 = gameObject.AddOrGetDef<HiveHarvestMonitor.Def>();
			def3.producedOre = BeeHiveTuning.PRODUCED_ORE;
			def3.harvestThreshold = 10f;
			HashSet<Tag> hashSet = new HashSet<Tag>();
			hashSet.Add(BeeHiveTuning.CONSUMED_ORE);
			Diet.Info[] infos = new Diet.Info[1]
			{
				new Diet.Info(hashSet, BeeHiveTuning.PRODUCED_ORE, BeeHiveTuning.CALORIES_PER_KG_OF_ORE, BeeHiveTuning.POOP_CONVERSTION_RATE)
			};
			Diet diet = new Diet(infos);
			CreatureCalorieMonitor.Def def4 = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
			def4.diet = diet;
			def4.storePoop = true;
			Trait trait = Db.Get().CreateTrait("BeeHiveBaseTrait", STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, BeeHiveTuning.STANDARD_STOMACH_SIZE, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - BeeHiveTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
			trait.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 100f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC));
			Modifiers modifiers = gameObject.AddOrGet<Modifiers>();
			modifiers.initialTraits.Add("BeeHiveBaseTrait");
			modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
			modifiers.initialAttributes.Add(Db.Get().CritterAttributes.Metabolism.Id);
			ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new DisabledCreatureStates.Def("inactive"))
				.PushInterruptGroup()
				.Add(new HiveGrowingStates.Def())
				.Add(new HiveHarvestStates.Def())
				.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
				.Add(new HiveEatingStates.Def(BeeHiveTuning.CONSUMED_ORE))
				.PopInterruptGroup()
				.Add(new IdleStandStillStates.Def());
			EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.BeetaSpecies, null);
		}
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
