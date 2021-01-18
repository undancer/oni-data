using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilFloaterHighTempConfig : IEntityConfig
{
	public const string ID = "OilfloaterHighTemp";

	public const string BASE_TRAIT_ID = "OilfloaterHighTempBaseTrait";

	public const string EGG_ID = "OilfloaterHighTempEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.CarbonDioxide;

	public const SimHashes EMIT_ELEMENT = SimHashes.Petroleum;

	private static float KG_ORE_EATEN_PER_CYCLE = 20f;

	private static float CALORIES_PER_KG_OF_ORE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 0.5f;

	public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 1;

	public static GameObject CreateOilFloater(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, "OilfloaterHighTempBaseTrait", 363.15f, 523.15f, is_baby, "hot_");
		EntityTemplates.ExtendEntityToWildCreature(prefab, OilFloaterTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("OilfloaterHighTempBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
		return BaseOilFloaterConfig.SetupDiet(prefab, SimHashes.CarbonDioxide.CreateTag(), SimHashes.Petroleum.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateOilFloater("OilfloaterHighTemp", STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.NAME, STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.DESC, "oilfloater_kanim", is_baby: false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "OilfloaterHighTempEgg", STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.EGG_NAME, STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.DESC, "egg_oilfloater_kanim", OilFloaterTuning.EGG_MASS, "OilfloaterHighTempBaby", 60.000004f, 20f, OilFloaterTuning.EGG_CHANCES_HIGHTEMP, EGG_SORT_ORDER);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
