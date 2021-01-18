using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class DivergentWormConfig : IEntityConfig
{
	public const string ID = "DivergentWorm";

	public const string BASE_TRAIT_ID = "DivergentWormBaseTrait";

	public const string EGG_ID = "DivergentWormEgg";

	private const float LIFESPAN = 150f;

	public const float CROP_TENDED_MULTIPLIER_EFFECT = 0.5f;

	public const float CROP_TENDED_MULTIPLIER_DURATION = 600f;

	private const int NUM_SEGMENTS = 5;

	private const SimHashes EMIT_ELEMENT = SimHashes.Mud;

	private static float KG_ORE_EATEN_PER_CYCLE = 50f;

	private static float KG_SUCROSE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = DivergentTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float CALORIES_PER_KG_OF_SUCROSE = DivergentTuning.STANDARD_CALORIES_PER_CYCLE / KG_SUCROSE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = 0;

	private static float MINI_POOP_SIZE_IN_KG = 4f;

	public static GameObject CreateWorm(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseDivergentConfig.BaseDivergent(id, name, desc, 200f, anim_file, "DivergentWormBaseTrait", is_baby, 8f, null, "DivergentCropTendedWorm", 3, is_pacifist: false);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, DivergentTuning.PEN_SIZE_PER_CREATURE_WORM);
		Trait trait = Db.Get().CreateTrait("DivergentWormBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DivergentTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - DivergentTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name));
		prefab.AddWeapon(2f, 3f);
		List<Diet.Info> list = BaseDivergentConfig.BasicSulfurDiet(SimHashes.Mud.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, null, 0f);
		list.Add(new Diet.Info(new HashSet<Tag>
		{
			SimHashes.Sucrose.CreateTag()
		}, SimHashes.Mud.CreateTag(), CALORIES_PER_KG_OF_SUCROSE));
		prefab = BaseDivergentConfig.SetupDiet(prefab, list, CALORIES_PER_KG_OF_ORE, MINI_POOP_SIZE_IN_KG);
		SegmentedCreature.Def def = prefab.AddOrGetDef<SegmentedCreature.Def>();
		def.segmentTrackerSymbol = new HashedString("segmenttracker");
		def.numBodySegments = 5;
		def.midAnim = Assets.GetAnim("worm_torso_kanim");
		def.tailAnim = Assets.GetAnim("worm_tail_kanim");
		def.animFrameOffset = 2;
		def.pathSpacing = 0.2f;
		def.numPathNodes = 15;
		def.minSegmentSpacing = 0.1f;
		def.maxSegmentSpacing = 0.4f;
		def.retractionSegmentSpeed = 1f;
		def.retractionPathSpeed = 2f;
		def.compressedMaxScale = 0.25f;
		def.headOffset = new Vector3(0.12f, 0.4f, 0f);
		return prefab;
	}

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateWorm("DivergentWorm", STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.NAME, STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.DESC, "worm_head_kanim", is_baby: false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "DivergentWormEgg", STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.EGG_NAME, STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.DESC, "egg_worm_kanim", DivergentTuning.EGG_MASS, "DivergentWormBaby", 90f, 30f, DivergentTuning.EGG_CHANCES_WORM, EGG_SORT_ORDER);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
