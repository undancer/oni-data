using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugPinkConfig : IEntityConfig
{
	public const string ID = "LightBugPink";

	public const string BASE_TRAIT_ID = "LightBugPinkBaseTrait";

	public const string EGG_ID = "LightBugPinkEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 3;

	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugPinkBaseTrait", LIGHT2D.LIGHTBUG_COLOR_PINK, DECOR.BONUS.TIER6, is_baby, "pnk_");
		EntityTemplates.ExtendEntityToWildCreature(prefab, LightBugTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("LightBugPinkBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - LightBugTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(TagManager.Create("FriedMushroom"));
		hashSet.Add(TagManager.Create("SpiceBread"));
		hashSet.Add(TagManager.Create(PrickleFruitConfig.ID));
		hashSet.Add(TagManager.Create("GrilledPrickleFruit"));
		hashSet.Add(TagManager.Create("Salsa"));
		hashSet.Add(SimHashes.Phosphorite.CreateTag());
		return BaseLightBugConfig.SetupDiet(prefab, hashSet, Tag.Invalid, CALORIES_PER_KG_OF_ORE);
	}

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateLightBug("LightBugPink", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.DESC, "lightbug_kanim", is_baby: false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "LightBugPinkEgg", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.EGG_NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_PINK.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugPinkBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_PINK, EGG_SORT_ORDER);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}
}
