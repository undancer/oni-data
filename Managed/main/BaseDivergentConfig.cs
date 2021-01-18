using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseDivergentConfig
{
	public const float CROP_TENDED_MULTIPLIER_DURATION = 600f;

	public const float CROP_TENDED_MULTIPLIER_EFFECT = 0.05f;

	public static string[] ignoreEffectGroup = new string[2]
	{
		"DivergentCropTended",
		"DivergentCropTendedWorm"
	};

	public static GameObject BaseDivergent(string id, string name, string desc, float mass, string anim_file, string traitId, bool is_baby, float num_tended_per_cycle = 8f, string symbolOverridePrefix = null, string cropTendingEffect = "DivergentCropTended", int meatAmount = 1, bool is_pacifist = true)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		string navGridName = "WalkerNavGrid1x1";
		if (is_baby)
		{
			navGridName = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, navGridName, NavType.Floor, 32, 2f, "Meat", meatAmount, drownVulnerable: true, entombVulnerable: false);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<BurrowMonitor.Def>();
		CropTendingMonitor.Def def = gameObject.AddOrGetDef<CropTendingMonitor.Def>();
		def.numCropsTendedPerCycle = num_tended_per_cycle;
		ThreatMonitor.Def def2 = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def2.fleethresholdState = Health.HealthState.Dead;
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: true);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		CropTendingStates.Def def3 = new CropTendingStates.Def();
		def3.effectId = cropTendingEffect;
		def3.interests.Add("WormPlant", 10);
		def3.animSetOverrides.Add("WormPlant", new CropTendingStates.AnimSet
		{
			crop_tending_pre = "wormwood_tending_pre",
			crop_tending = "wormwood_tending",
			crop_tending_pst = "wormwood_tending_pst",
			hide_symbols_after_pre = new string[2]
			{
				"flower",
				"flower_wilted"
			}
		});
		def3.ignoreEffectGroup = ignoreEffectGroup;
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def(), !is_baby && !is_pacifist)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def())
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new EatStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.Add(new CallAdultStates.Def())
			.Add(def3, !is_baby)
			.PopInterruptGroup()
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.DivergentSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static List<Diet.Info> BasicSulfurDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Sulfur.CreateTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
		};
	}

	public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos, float referenceCaloriesPerKg, float minPoopSizeInKg)
	{
		Diet diet = new Diet(diet_infos.ToArray());
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = referenceCaloriesPerKg * minPoopSizeInKg;
		SolidConsumerMonitor.Def def2 = prefab.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		return prefab;
	}
}
