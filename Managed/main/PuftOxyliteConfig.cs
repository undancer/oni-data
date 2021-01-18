using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftOxyliteConfig : IEntityConfig
{
	public const string ID = "PuftOxylite";

	public const string BASE_TRAIT_ID = "PuftOxyliteBaseTrait";

	public const string EGG_ID = "PuftOxyliteEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.Oxygen;

	public const SimHashes EMIT_ELEMENT = SimHashes.OxyRock;

	private static float KG_ORE_EATEN_PER_CYCLE = 50f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 25f;

	public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 2;

	public static GameObject CreatePuftOxylite(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePuftConfig.BasePuft(id, name, desc, "PuftOxyliteBaseTrait", anim_file, is_baby, "com_", 303.15f, 338.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("PuftOxyliteBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - PuftTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name));
		prefab = BasePuftConfig.SetupDiet(prefab, SimHashes.Oxygen.CreateTag(), SimHashes.OxyRock.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, null, 0f, MIN_POOP_SIZE_IN_KG);
		LureableMonitor.Def def = prefab.AddOrGetDef<LureableMonitor.Def>();
		def.lures = new Tag[1]
		{
			SimHashes.OxyRock.CreateTag()
		};
		return prefab;
	}

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePuftOxylite("PuftOxylite", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.DESC, "puft_kanim", is_baby: false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PuftOxyliteEgg", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.EGG_NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftOxyliteBaby", 45f, 15f, PuftTuning.EGG_CHANCES_OXYLITE, EGG_SORT_ORDER);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
