using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseSquirrelConfig
{
	public static GameObject BaseSquirrel(string id, string name, string desc, string anim_file, string traitId, bool is_baby, string symbolOverridePrefix = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 100f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		string navGridName = "SquirrelNavGrid";
		if (is_baby)
		{
			navGridName = "DreckoBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, traitId, navGridName, NavType.Floor, 32, 2f, "Meat", 1, drownVulnerable: true, entombVulnerable: false);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
		}
		gameObject.AddComponent<Storage>();
		if (!is_baby)
		{
			gameObject.AddOrGetDef<SeedPlantingMonitor.Def>();
			gameObject.AddOrGetDef<ClimbableTreeMonitor.Def>();
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
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
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def())
			.PushInterruptGroup()
			.Add(new CreatureSleepStates.Def())
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new TreeClimbStates.Def())
			.Add(new EatStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.Add(new CallAdultStates.Def())
			.Add(new SeedPlantingStates.Def())
			.PopInterruptGroup()
			.Add(new IdleStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.SquirrelSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static Diet.Info[] BasicWoodDiet(Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced)
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("ForestTree");
		return new Diet.Info[1]
		{
			new Diet.Info(hashSet, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced, produce_solid_tile: false, eats_plants_directly: true)
		};
	}

	public static GameObject SetupDiet(GameObject prefab, Diet.Info[] diet_infos, float minPoopSizeInKg)
	{
		Diet diet = new Diet(diet_infos);
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = minPoopSizeInKg;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
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
