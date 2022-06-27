using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabWoodConfig : IEntityConfig
{
	public const string ID = "CrabWood";

	public const string BASE_TRAIT_ID = "CrabWoodBaseTrait";

	public const string EGG_ID = "CrabWoodEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Sand;

	private static float KG_ORE_EATEN_PER_CYCLE = 70f;

	private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = 0;

	private static string animPrefix = "wood_";

	public static GameObject CreateCrabWood(string id, string name, string desc, string anim_file, bool is_baby, string deathDropID = "CrabWoodShell")
	{
		GameObject prefab = EntityTemplates.ExtendEntityToWildCreature(BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabWoodBaseTrait", is_baby, animPrefix, deathDropID), CrabTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("CrabWoodBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - CrabTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
		List<Diet.Info> diet_infos = BaseCrabConfig.DietWithSlime(SimHashes.Sand.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_1, null, 0f);
		return BaseCrabConfig.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateCrabWood("CrabWood", STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.NAME, STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.DESC, "pincher_kanim", is_baby: false);
		prefab = EntityTemplates.ExtendEntityToFertileCreature(prefab, "CrabWoodEgg", STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.EGG_NAME, STRINGS.CREATURES.SPECIES.CRAB.VARIANT_WOOD.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabWoodBaby", 60.000004f, 20f, CrabTuning.EGG_CHANCES_WOOD, EGG_SORT_ORDER);
		EggProtectionMonitor.Def def = prefab.AddOrGetDef<EggProtectionMonitor.Def>();
		def.allyTags = new Tag[1] { GameTags.Creatures.CrabFriend };
		def.animPrefix = animPrefix;
		MoltDropperMonitor.Def def2 = prefab.AddOrGetDef<MoltDropperMonitor.Def>();
		def2.onGrowDropID = "CrabWoodShell";
		def2.massToDrop = 100f;
		def2.blockedElement = SimHashes.Ethanol;
		return prefab;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
