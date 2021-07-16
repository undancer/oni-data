using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public static class BaseMoleConfig
{
	private static readonly string[] SolidIdleAnims = new string[4]
	{
		"idle1",
		"idle2",
		"idle3",
		"idle4"
	};

	public static GameObject BaseMole(string id, string name, string desc, string traitId, string anim_file, bool is_baby)
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity(id, name, desc, 25f, decor: TUNING.BUILDINGS.DECOR.NONE, anim: Assets.GetAnim(anim_file), initialAnim: "idle_loop", sceneLayer: Grid.SceneLayer.Creatures, width: 1, height: 1);
		EntityTemplates.ExtendEntityToBasicCreature(obj, FactionManager.FactionID.Pest, traitId, "DiggerNavGrid", NavType.Floor, 32, 2f, "Meat", 10, drownVulnerable: true, entombVulnerable: false, 123.149994f, 673.15f, 73.149994f, 773.15f);
		obj.AddOrGetDef<CreatureFallMonitor.Def>();
		obj.AddOrGet<Trappable>();
		obj.AddOrGetDef<DiggerMonitor.Def>().depthToDig = MoleTuning.DEPTH_TO_HIDE;
		EntityTemplates.CreateAndRegisterBaggedCreature(obj, must_stand_on_top_for_pickup: true, allow_mark_for_capture: true);
		obj.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker);
		ChoreTable.Builder chore_table = new ChoreTable.Builder().Add(new DeathStates.Def()).Add(new AnimInterruptStates.Def()).Add(new FallStates.Def())
			.Add(new StunnedStates.Def())
			.Add(new DrowningStates.Def())
			.Add(new DiggerStates.Def())
			.Add(new GrowUpStates.Def())
			.Add(new TrappedStates.Def())
			.Add(new IncubatingStates.Def())
			.Add(new BaggedStates.Def())
			.Add(new DebugGoToStates.Def())
			.Add(new FleeStates.Def())
			.Add(new AttackStates.Def(), !is_baby)
			.PushInterruptGroup()
			.Add(new FixedCaptureStates.Def())
			.Add(new RanchedStates.Def())
			.Add(new LayEggStates.Def())
			.Add(new CreatureSleepStates.Def())
			.Add(new EatStates.Def())
			.Add(new NestingPoopState.Def(is_baby ? Tag.Invalid : SimHashes.Regolith.CreateTag()))
			.Add(new PlayAnimsStates.Def(GameTags.Creatures.Poop, loop: false, "poop", STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
			.PopInterruptGroup()
			.Add(new IdleStates.Def
			{
				customIdleAnim = CustomIdleAnim
			});
		EntityTemplates.AddCreatureBrain(obj, chore_table, GameTags.Creatures.Species.MoleSpecies, null);
		return obj;
	}

	public static List<Diet.Info> SimpleOreDiet(List<Tag> elementTags, float caloriesPerKg, float producedConversionRate)
	{
		List<Diet.Info> list = new List<Diet.Info>();
		foreach (Tag elementTag in elementTags)
		{
			list.Add(new Diet.Info(new HashSet<Tag>
			{
				elementTag
			}, elementTag, caloriesPerKg, producedConversionRate, null, 0f, produce_solid_tile: true));
		}
		return list;
	}

	private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
	{
		if (smi.gameObject.GetComponent<Navigator>().CurrentNavType == NavType.Solid)
		{
			int num = Random.Range(0, SolidIdleAnims.Length);
			return SolidIdleAnims[num];
		}
		if (smi.gameObject.GetDef<BabyMonitor.Def>() != null && Random.Range(0, 100) >= 90)
		{
			return "drill_fail";
		}
		return "idle_loop";
	}
}
