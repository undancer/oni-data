using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseDreckoConfig
{
	public static GameObject BaseDrecko(string id, string name, string desc, string anim_file, string trait_id, bool is_baby, string symbol_override_prefix, float warnLowTemp, float warnHighTemp)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, 200f, decor: DECOR.BONUS.TIER0, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1, noise: default(EffectorValues), element: SimHashes.Creature, additionalTags: null, defaultTemperature: (warnLowTemp + warnHighTemp) / 2f);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Walker);
		component.prefabInitFn += delegate(GameObject inst)
		{
			inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost);
		};
		string navGridName = "DreckoNavGrid";
		if (is_baby)
		{
			navGridName = "DreckoBabyNavGrid";
		}
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Pest, trait_id, navGridName, NavType.Floor, 32, 1f, "Meat", 2, drownVulnerable: true, entombVulnerable: false, warnLowTemp, warnHighTemp, warnLowTemp - 20f, warnHighTemp + 20f);
		if (!string.IsNullOrEmpty(symbol_override_prefix))
		{
			gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(anim_file), symbol_override_prefix);
		}
		gameObject.AddOrGet<Trappable>();
		gameObject.AddOrGetDef<CreatureFallMonitor.Def>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
		gameObject.AddWeapon(1f, 1f);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, must_stand_on_top_for_pickup: true, allow_mark_for_capture: true);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new EatStates.Def())
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.Add(new CallAdultStates.Def())
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = CustomIdleAnim
			});
		EntityTemplates.AddCreatureBrain(gameObject, chore_table, GameTags.Creatures.Species.DreckoSpecies, symbol_override_prefix);
		return gameObject;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		CellOffset offset = new CellOffset(0, -1);
		bool facing = smi.GetComponent<Facing>().GetFacing();
		switch (smi.GetComponent<Navigator>().CurrentNavType)
		{
		case NavType.Floor:
			offset = (facing ? new CellOffset(1, -1) : new CellOffset(-1, -1));
			break;
		case NavType.Ceiling:
			offset = (facing ? new CellOffset(1, 1) : new CellOffset(-1, 1));
			break;
		}
		HashedString result = "idle_loop";
		int num = Grid.OffsetCell(Grid.PosToCell(smi), offset);
		if (Grid.IsValidCell(num) && !Grid.Solid[num])
		{
			pre_anim = "idle_loop_hang_pre";
			result = "idle_loop_hang";
		}
		return result;
	}
}
