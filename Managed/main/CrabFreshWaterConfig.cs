using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class CrabFreshWaterConfig : IEntityConfig
{
	public const string ID = "CrabFreshWater";

	public const string BASE_TRAIT_ID = "CrabFreshWaterBaseTrait";

	public const string EGG_ID = "CrabFreshWaterEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Sand;

	private static float KG_ORE_EATEN_PER_CYCLE = 70f;

	private static float CALORIES_PER_KG_OF_ORE = CrabTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = 0;

	private static string animPrefix = "fresh_";

	public static GameObject CreateCrabFreshWater(string id, string name, string desc, string anim_file, bool is_baby, string deathDropID = null)
	{
		GameObject prefab = BaseCrabConfig.BaseCrab(id, name, desc, anim_file, "CrabFreshWaterBaseTrait", is_baby, animPrefix, deathDropID);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, CrabTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("CrabFreshWaterBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - CrabTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
		List<Diet.Info> diet_infos = BaseCrabConfig.DietWithSlime(SimHashes.Sand.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
		prefab = BaseCrabConfig.SetupDiet(prefab, diet_infos, CALORIES_PER_KG_OF_ORE, MIN_POOP_SIZE_IN_KG);
		Butcherable component = prefab.GetComponent<Butcherable>();
		if (component != null)
		{
			string[] drops = new string[4] { "ShellfishMeat", "ShellfishMeat", "ShellfishMeat", "ShellfishMeat" };
			component.SetDrops(drops);
		}
		return prefab;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreateCrabFreshWater("CrabFreshWater", STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.NAME, STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.DESC, "pincher_kanim", is_baby: false);
		prefab = EntityTemplates.ExtendEntityToFertileCreature(prefab, "CrabFreshWaterEgg", STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.EGG_NAME, STRINGS.CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.DESC, "egg_pincher_kanim", CrabTuning.EGG_MASS, "CrabFreshWaterBaby", 60.000004f, 20f, CrabTuning.EGG_CHANCES_FRESH, EGG_SORT_ORDER);
		EggProtectionMonitor.Def def = prefab.AddOrGetDef<EggProtectionMonitor.Def>();
		def.allyTags = new Tag[1] { GameTags.Creatures.CrabFriend };
		def.animPrefix = animPrefix;
		DiseaseEmitter diseaseEmitter = prefab.AddComponent<DiseaseEmitter>();
		List<Disease> list = new List<Disease>
		{
			Db.Get().Diseases.FoodGerms,
			Db.Get().Diseases.PollenGerms,
			Db.Get().Diseases.SlimeGerms,
			Db.Get().Diseases.ZombieSpores
		};
		if (DlcManager.IsExpansion1Active())
		{
			list.Add(Db.Get().Diseases.RadiationPoisoning);
		}
		diseaseEmitter.SetDiseases(list);
		diseaseEmitter.emitRange = 2;
		diseaseEmitter.emitCount = -1 * Mathf.RoundToInt(888.8889f);
		CleaningMonitor.Def def2 = prefab.AddOrGetDef<CleaningMonitor.Def>();
		def2.elementState = Element.State.Liquid;
		def2.cellOffsets = new CellOffset[5]
		{
			new CellOffset(1, 0),
			new CellOffset(-1, 0),
			new CellOffset(0, 1),
			new CellOffset(-1, 1),
			new CellOffset(1, 1)
		};
		def2.coolDown = 30f;
		return prefab;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
