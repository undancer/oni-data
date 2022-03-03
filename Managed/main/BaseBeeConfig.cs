using Klei.AI;
using TUNING;
using UnityEngine;

public static class BaseBeeConfig
{
	public static GameObject BaseBee(string id, string name, string desc, string anim_file, string traitId, EffectorValues decor, bool is_baby, string symbolOverridePrefix = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 5f, Assets.GetAnim(anim_file), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, default(EffectorValues), SimHashes.Creature, null, CREATURES.TEMPERATURE.FREEZING_3);
		string text = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		int num = 5;
		if (is_baby)
		{
			text = "WalkerBabyNavGrid";
			navType = NavType.Floor;
			num = 1;
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Hostile, traitId, text, navType, 32, num, "Meat", 0, drownVulnerable: true, entombVulnerable: true, lethalLowTemperature: CREATURES.TEMPERATURE.FREEZING_10, warningLowTemperature: CREATURES.TEMPERATURE.FREEZING_9, warningHighTemperature: CREATURES.TEMPERATURE.FREEZING_1, lethalHighTemperature: CREATURES.TEMPERATURE.FREEZING);
		if (symbolOverridePrefix != null)
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbolOverridePrefix);
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>();
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: false);
		gameObject.AddOrGetDef<AgeMonitor.Def>();
		Bee bee = gameObject.AddOrGet<Bee>();
		RadiationEmitter radiationEmitter = gameObject.AddComponent<RadiationEmitter>();
		radiationEmitter.emitRate = 0.1f;
		if (!is_baby)
		{
			component.AddTag(GameTags.Creatures.Flyer);
			bee.radiationOutputAmount = 240f;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 3;
			radiationEmitter.emitRadiusY = 3;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			gameObject.AddOrGetDef<SubmergedMonitor.Def>();
			gameObject.AddWeapon(2f, 3f);
		}
		else
		{
			bee.radiationOutputAmount = 120f;
			radiationEmitter.radiusProportionalToRads = false;
			radiationEmitter.emitRadiusX = 2;
			radiationEmitter.emitRadiusY = 2;
			radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
			gameObject.AddOrGetDef<BeeHiveMonitor.Def>();
			gameObject.AddOrGet<Trappable>();
			EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: true);
		}
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER1;
		gameObject.AddOrGetDef<BeeHappinessMonitor.Def>();
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.1f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = false;
		elementConsumer.showDescriptor = true;
		elementConsumer.EnableConsumption(enabled: false);
		gameObject.AddOrGetDef<BeeSleepMonitor.Def>();
		gameObject.AddOrGetDef<BeeForagingMonitor.Def>();
		gameObject.AddOrGet<Storage>();
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new BeeSleepStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def("attack_pre", "attack_pst", new CellOffset[3]
			{
				new CellOffset(0, 1),
				new CellOffset(1, 1),
				new CellOffset(-1, 1)
			}), !is_baby)
			.Add(new FixedCaptureStates.Def())
			.Add(new BeeMakeHiveStates.Def())
			.Add(new BeeForageStates.Def(SimHashes.UraniumOre.CreateTag(), BeeHiveTuning.ORE_DELIVERY_AMOUNT))
			.Add(new BuzzStates.Def());
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.BeetaSpecies, symbolOverridePrefix);
		return gameObject;
	}

	public static void SetupLoopingSounds(GameObject inst)
	{
		inst.GetComponent<LoopingSounds>().StartSound(GlobalAssets.GetSound("Bee_wings_LP"));
	}
}
