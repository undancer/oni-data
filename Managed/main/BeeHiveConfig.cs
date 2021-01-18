using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BeeHiveConfig : IEntityConfig
{
	public const string ID = "BeeHive";

	public const string BASE_TRAIT_ID = "BeeHiveBaseTrait";

	private const int WIDTH = 2;

	private const int HEIGHT = 3;

	private static float KG_ORE_EATEN_PER_CYCLE = 100f;

	private static float CALORIES_PER_KG_OF_ORE = BeeHiveTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BeeHive", STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC, 100f, Assets.GetAnim("beehive_kanim"), "out_of_order", Grid.SceneLayer.BuildingBack, 2, 3, TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0);
		gameObject.AddOrGet<Operational>();
		Storage storage = gameObject.AddOrGet<Storage>();
		if (Sim.IsRadiationEnabled())
		{
			BeeHive beeHive = gameObject.AddOrGet<BeeHive>();
			beeHive.beePrefabID = "Bee";
			beeHive.larvaPrefabID = "BeeBaby";
			beeHive.storage = storage;
			KAnimFile[] overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_outhouse_kanim")
			};
			HiveWorkableEmpty hiveWorkableEmpty = gameObject.AddOrGet<HiveWorkableEmpty>();
			hiveWorkableEmpty.workTime = 90f;
			hiveWorkableEmpty.overrideAnims = overrideAnims;
			hiveWorkableEmpty.workLayer = Grid.SceneLayer.BuildingFront;
			Prioritizable.AddRef(gameObject);
			Beefinery beefinery = gameObject.AddOrGet<Beefinery>();
			beefinery.storage = storage;
			RadiationEmitter radiationEmitter = gameObject.AddComponent<RadiationEmitter>();
			radiationEmitter.emitRadiusX = 7;
			radiationEmitter.emitRadiusY = 6;
			radiationEmitter.emitRads = 5f;
			radiationEmitter.emitRate = 3f;
		}
		KPrefabID kPrefabID = gameObject.AddOrGet<KPrefabID>();
		kPrefabID.AddTag(GameTags.Experimental);
		kPrefabID.AddTag(GameTags.Creature);
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Health>();
		gameObject.AddOrGet<CharacterOverlay>();
		gameObject.AddOrGet<RangedAttackable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Effects>();
		gameObject.AddOrGetDef<DeathMonitor.Def>();
		gameObject.AddOrGetDef<AnimInterruptMonitor.Def>();
		if (Sim.IsRadiationEnabled())
		{
			HashSet<Tag> hashSet = new HashSet<Tag>();
			hashSet.Add(SimHashes.Radium.CreateTag());
			Diet.Info[] infos = new Diet.Info[1]
			{
				new Diet.Info(hashSet, SimHashes.UraniumOre.CreateTag(), CALORIES_PER_KG_OF_ORE, BeeHiveTuning.POOP_CONVERSTION_RATE)
			};
			Diet diet = new Diet(infos);
			CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
			def.diet = diet;
		}
		Trait trait = Db.Get().CreateTrait("BeeHiveBaseTrait", STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, BeeHiveTuning.STANDARD_STOMACH_SIZE, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - BeeHiveTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.NAME));
		trait.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 25f, STRINGS.BUILDINGS.PREFABS.BEEHIVE.DESC));
		Modifiers modifiers = gameObject.AddOrGet<Modifiers>();
		modifiers.initialTraits.Add("BeeHiveBaseTrait");
		modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
		modifiers.initialAttributes.Add(Db.Get().CritterAttributes.Metabolism.Id);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP));
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.BeetaSpecies, null);
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
