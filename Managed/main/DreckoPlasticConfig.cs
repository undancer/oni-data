using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class DreckoPlasticConfig : IEntityConfig
{
	public const string ID = "DreckoPlastic";

	public const string BASE_TRAIT_ID = "DreckoPlasticBaseTrait";

	public const string EGG_ID = "DreckoPlasticEgg";

	public static Tag POOP_ELEMENT = SimHashes.Phosphorite.CreateTag();

	public static Tag EMIT_ELEMENT = SimHashes.Polypropylene.CreateTag();

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 3f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = DreckoTuning.STANDARD_CALORIES_PER_CYCLE / DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 3f;

	private static float MIN_POOP_SIZE_IN_KG = 1.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = CALORIES_PER_DAY_OF_PLANT_EATEN * MIN_POOP_SIZE_IN_KG / KG_POOP_PER_DAY_OF_PLANT;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 3f;

	public static float PLASTIC_PER_CYCLE = 50f;

	public static int EGG_SORT_ORDER = 800;

	public static GameObject CreateDrecko(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseDreckoConfig.BaseDrecko(id, name, desc, anim_file, "DreckoPlasticBaseTrait", is_baby, null, 298.15f, 333.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, DreckoTuning.PEN_SIZE_PER_CREATURE, 150f);
		Trait trait = Db.Get().CreateTrait("DreckoPlasticBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DreckoTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - DreckoTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add("BasicSingleHarvestPlant".ToTag());
		hashSet.Add("PrickleFlower".ToTag());
		Diet diet = new Diet(new Diet.Info(hashSet, POOP_ELEMENT, CALORIES_PER_DAY_OF_PLANT_EATEN, KG_POOP_PER_DAY_OF_PLANT, null, 0f, produce_solid_tile: false, eats_plants_directly: true));
		CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MIN_POOP_SIZE_IN_CALORIES;
		ScaleGrowthMonitor.Def def2 = prefab.AddOrGetDef<ScaleGrowthMonitor.Def>();
		def2.defaultGrowthRate = 1f / SCALE_GROWTH_TIME_IN_CYCLES / 600f;
		def2.dropMass = PLASTIC_PER_CYCLE * SCALE_GROWTH_TIME_IN_CYCLES;
		def2.itemDroppedOnShear = EMIT_ELEMENT;
		def2.levelCount = 6;
		def2.targetAtmosphere = SimHashes.Hydrogen;
		prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return prefab;
	}

	public virtual GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(CreateDrecko("DreckoPlastic", CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.NAME, CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC, "drecko_kanim", is_baby: false), "DreckoPlasticEgg", CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.EGG_NAME, CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC, "egg_drecko_kanim", DreckoTuning.EGG_MASS, "DreckoPlasticBaby", 90f, 30f, eggSortOrder: EGG_SORT_ORDER, egg_chances: DreckoTuning.EGG_CHANCES_PLASTIC);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
