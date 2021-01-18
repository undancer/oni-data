using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class StaterpillarConfig : IEntityConfig
{
	public const string ID = "Staterpillar";

	public const string BASE_TRAIT_ID = "StaterpillarBaseTrait";

	public const string EGG_ID = "StaterpillarEgg";

	public static int EGG_SORT_ORDER = 0;

	private static float KG_ORE_EATEN_PER_CYCLE = 60f;

	private static float CALORIES_PER_KG_OF_ORE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static GameObject CreateStaterpillar(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseStaterpillarConfig.BaseStaterpillar(id, name, desc, anim_file, "StaterpillarBaseTrait", is_baby);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3);
		Trait trait = Db.Get().CreateTrait("StaterpillarBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StaterpillarTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(SimHashes.Cuprite.CreateTag());
		hashSet.Add(SimHashes.IronOre.CreateTag());
		hashSet.Add(SimHashes.GoldAmalgam.CreateTag());
		hashSet.Add(SimHashes.Wolframite.CreateTag());
		hashSet.Add(SimHashes.AluminumOre.CreateTag());
		hashSet.Add(SimHashes.Electrum.CreateTag());
		hashSet.Add(SimHashes.Cobaltite.CreateTag());
		return BaseStaterpillarConfig.SetupDiet(prefab, hashSet, SimHashes.Hydrogen.CreateTag(), CALORIES_PER_KG_OF_ORE);
	}

	public string GetDlcId()
	{
		return "EXPANSION1_ID";
	}

	public virtual GameObject CreatePrefab()
	{
		GameObject prefab = CreateStaterpillar("Staterpillar", STRINGS.CREATURES.SPECIES.STATERPILLAR.NAME, STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC, "caterpillar_kanim", is_baby: false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "StaterpillarEgg", STRINGS.CREATURES.SPECIES.STATERPILLAR.EGG_NAME, STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC, "egg_caterpillar_kanim", StaterpillarTuning.EGG_MASS, "StaterpillarBaby", 60.000004f, 20f, StaterpillarTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
