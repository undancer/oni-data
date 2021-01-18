using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugRadioactiveConfig : IEntityConfig
{
	public const string ID = "LightBugRadioactive";

	public const string BASE_TRAIT_ID = "LightBugRadioactiveBaseTrait";

	public const string EGG_ID = "LightBugRadioactiveEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 2;

	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugRadioactiveBaseTrait", LIGHT2D.LIGHTBUG_COLOR_GREEN, DECOR.BONUS.TIER6, is_baby, "prp_");
		EntityTemplates.ExtendEntityToWildCreature(prefab, LightBugTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("LightBugRadioactiveBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - LightBugTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Regolith.CreateTag());
		hashSet.Add(SimHashes.DepletedUranium.CreateTag());
		hashSet.Add(SimHashes.UraniumOre.CreateTag());
		return BaseLightBugConfig.SetupDiet(prefab, hashSet, Tag.Invalid, CALORIES_PER_KG_OF_ORE);
	}

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateLightBug("LightBugRadioactive", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.DESC, "lightbug_kanim", is_baby: false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "LightBugRadioactiveEgg", STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.EGG_NAME, STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_RADIOACTIVE.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugRadioactiveBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, is_ranchable: true, add_fish_overcrowding_monitor: false, add_fixed_capturable_monitor: true, 1f, deprecated: true);
		gameObject.AddTag(GameTags.DeprecatedContent);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		Radiator radiator = inst.AddOrGet<Radiator>();
		radiator.intensity = 24;
		radiator.projectionCount = 24;
		radiator.angle = 360;
		radiator.direction = 0;
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}
}
