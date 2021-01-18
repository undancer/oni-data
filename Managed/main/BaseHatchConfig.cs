using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseHatchConfig
{
	public static GameObject BaseHatch(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 100f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		string navGridName = "WalkerNavGrid1x1";
		if (is_baby)
		{
			navGridName = "WalkerBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, navGridName, NavType.Floor, 32, 2f, "Meat", 2, drownVulnerable: true, entombVulnerable: false);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<BurrowMonitor.Def>();
		WorldSpawnableMonitor.Def def = gameObject.AddOrGetDef<WorldSpawnableMonitor.Def>();
		def.adjustSpawnLocationCb = AdjustSpawnLocationCB;
		ThreatMonitor.Def def2 = gameObject.AddOrGetDef<ThreatMonitor.Def>();
		def2.fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
		SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
		SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: true);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		bool condition = !is_baby;
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new ExitBurrowStates.Def(), condition)
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, loop: true, "idle_mound", STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), condition)
			.Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def(), condition)
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def())
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, loop: false, "hide", STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), condition)
			.Add(new LayEggStates.Def())
			.Add(new EatStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.Add(new CallAdultStates.Def())
			.PopInterruptGroup()
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.HatchSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static List<Diet.Info> BasicRockDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Sand.CreateTag());
		hashSet.Add(SimHashes.SandStone.CreateTag());
		hashSet.Add(SimHashes.Clay.CreateTag());
		hashSet.Add(SimHashes.CrushedRock.CreateTag());
		hashSet.Add(SimHashes.Dirt.CreateTag());
		hashSet.Add(SimHashes.SedimentaryRock.CreateTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
		};
	}

	public static List<Diet.Info> HardRockDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.SedimentaryRock.CreateTag());
		hashSet.Add(SimHashes.IgneousRock.CreateTag());
		hashSet.Add(SimHashes.Obsidian.CreateTag());
		hashSet.Add(SimHashes.Granite.CreateTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
		};
	}

	public static List<Diet.Info> MetalDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.Cuprite.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Copper.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.GoldAmalgam.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Gold.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.IronOre.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Iron.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.Wolframite.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Tungsten.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.AluminumOre.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Aluminum.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		list.Add(new Diet.Info(new HashSet<Tag>(new Tag[1]
		{
			SimHashes.Electrum.CreateTag()
		}), (poopTag == GameTags.Metal) ? SimHashes.Gold.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
		return list;
	}

	public static List<Diet.Info> VeggieDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Dirt.CreateTag());
		hashSet.Add(SimHashes.SlimeMold.CreateTag());
		hashSet.Add(SimHashes.Algae.CreateTag());
		hashSet.Add(SimHashes.Fertilizer.CreateTag());
		hashSet.Add(SimHashes.ToxicSand.CreateTag());
		return new List<Diet.Info>
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
		};
	}

	public static List<Diet.Info> FoodDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
		{
			if (allFoodType.CaloriesPerUnit > 0f)
			{
				HashSet<Tag> hashSet = new HashSet<Tag>();
				hashSet.Add(new Tag(allFoodType.Id));
				list.Add(new Diet.Info(hashSet, poopTag, allFoodType.CaloriesPerUnit, producedConversionRate, diseaseId, diseasePerKgProduced));
			}
		}
		return list;
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

	private static int AdjustSpawnLocationCB(int cell)
	{
		while (!Grid.Solid[cell])
		{
			int num = Grid.CellBelow(cell);
			if (!Grid.IsValidCell(cell))
			{
				break;
			}
			cell = num;
		}
		return cell;
	}
}
