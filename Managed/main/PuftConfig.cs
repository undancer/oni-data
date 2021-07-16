using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public const string ID = "Puft";

	public const string BASE_TRAIT_ID = "PuftBaseTrait";

	public const string EGG_ID = "PuftEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;

	public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;

	public const string EMIT_DISEASE = "SlimeLung";

	public const float EMIT_DISEASE_PER_KG = 1000f;

	private static float KG_ORE_EATEN_PER_CYCLE = 50f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 15f;

	public static int EGG_SORT_ORDER = 300;

	public static GameObject CreatePuft(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePuftConfig.BasePuft(id, name, STRINGS.CREATURES.SPECIES.PUFT.DESC, "PuftBaseTrait", anim_file, is_baby, null, 288.15f, 328.15f);
		EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("PuftBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - PuftTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name));
		GameObject gameObject = BasePuftConfig.SetupDiet(prefab, SimHashes.ContaminatedOxygen.CreateTag(), SimHashes.SlimeMold.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, "SlimeLung", 1000f, MIN_POOP_SIZE_IN_KG);
		gameObject.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(CreatePuft("Puft", STRINGS.CREATURES.SPECIES.PUFT.NAME, STRINGS.CREATURES.SPECIES.PUFT.DESC, "puft_kanim", is_baby: false), "PuftEgg", STRINGS.CREATURES.SPECIES.PUFT.EGG_NAME, STRINGS.CREATURES.SPECIES.PUFT.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftBaby", 45f, 15f, PuftTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
