using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class BaseOilFloaterConfig
{
	public static GameObject BaseOilFloater(string id, string name, string desc, string anim_file, string traitId, float warnLowTemp, float warnHighTemp, bool is_baby, string symbolOverridePrefix = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 50f, decor: DECOR.BONUS.TIER1, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: (warnLowTemp + warnHighTemp) / 2f);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Hoverer);
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, "FloaterNavGrid", NavType.Hover, 32, 2f, "Meat", 2, drownVulnerable: true, entombVulnerable: false, warnLowTemp, warnHighTemp, warnLowTemp - 15f, warnHighTemp + 20f);
		if (!string.IsNullOrEmpty(symbolOverridePrefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		gameObject.AddOrGetDef<SubmergedMonitor.Def>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
		gameObject.AddWeapon(1f, 1f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: false);
		string inhaleSound = "OilFloater_intake_air";
		if (is_baby)
		{
			inhaleSound = "OilFloaterBaby_intake_air";
		}
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def())
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new InhaleStates.Def
			{
				inhaleSound = inhaleSound
			})
			.Add(new SameSpotPoopStates.Def())
			.Add(new CallAdultStates.Def())
			.PopInterruptGroup()
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.OilFloaterSpecies, symbolOverridePrefix);
		string sound = "OilFloater_move_LP";
		if (is_baby)
		{
			sound = "OilFloaterBaby_move_LP";
		}
		gameObject.AddOrGet<OilFloaterMovementSound>().sound = sound;
		return gameObject;
	}

	public static GameObject SetupDiet(GameObject prefab, Tag consumed_tag, Tag producedTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced, float minPoopSizeInKg)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(consumed_tag);
		Diet diet = new Diet(new Diet.Info(hashSet, producedTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = minPoopSizeInKg * caloriesPerKg;
		prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}
}
