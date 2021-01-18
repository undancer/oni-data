using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftBleachstoneConfig : IEntityConfig
{
	public const string ID = "PuftBleachstone";

	public const string BASE_TRAIT_ID = "PuftBleachstoneBaseTrait";

	public const string EGG_ID = "PuftBleachstoneEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ChlorineGas;

	public const SimHashes EMIT_ELEMENT = SimHashes.BleachStone;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 15f;

	public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 3;

	public static GameObject CreatePuftBleachstone(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePuftConfig.BasePuft(id, name, desc, "PuftBleachstoneBaseTrait", anim_file, is_baby, "anti_", 258.15f, 308.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("PuftBleachstoneBaseTrait", name, name, null, should_save: false, null, positive_trait: true, is_valid_starter_trait: true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - PuftTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name));
		prefab = BasePuftConfig.SetupDiet(prefab, SimHashes.ChlorineGas.CreateTag(), SimHashes.BleachStone.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, null, 0f, MIN_POOP_SIZE_IN_KG);
		prefab.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[1]
		{
			SimHashes.BleachStone.CreateTag()
		};
		return prefab;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(CreatePuftBleachstone("PuftBleachstone", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC, "puft_kanim", is_baby: false), "PuftBleachstoneEgg", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.EGG_NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftBleachstoneBaby", 45f, 15f, PuftTuning.EGG_CHANCES_BLEACHSTONE, EGG_SORT_ORDER);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
