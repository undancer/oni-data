using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class PacuCleanerConfig : IEntityConfig
{
	public const string ID = "PacuCleaner";

	public const string BASE_TRAIT_ID = "PacuCleanerBaseTrait";

	public const string EGG_ID = "PacuCleanerEgg";

	public const float POLLUTED_WATER_CONVERTED_PER_CYCLE = 120f;

	public const SimHashes INPUT_ELEMENT = SimHashes.DirtyWater;

	public const SimHashes OUTPUT_ELEMENT = SimHashes.Water;

	public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public const int EGG_SORT_ORDER = 501;

	public static GameObject CreatePacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePacuConfig.CreatePrefab(id, "PacuCleanerBaseTrait", name, desc, anim_file, is_baby, "glp_", 243.15f, 278.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE);
		if (!is_baby)
		{
			Storage storage = prefab.AddComponent<Storage>();
			storage.capacityKg = 10f;
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			PassiveElementConsumer passiveElementConsumer = prefab.AddOrGet<PassiveElementConsumer>();
			passiveElementConsumer.elementToConsume = SimHashes.DirtyWater;
			passiveElementConsumer.consumptionRate = 0.2f;
			passiveElementConsumer.capacityKG = 10f;
			passiveElementConsumer.consumptionRadius = 3;
			passiveElementConsumer.showInStatusPanel = true;
			passiveElementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
			passiveElementConsumer.isRequired = false;
			passiveElementConsumer.storeOnConsume = true;
			passiveElementConsumer.showDescriptor = false;
			prefab.AddOrGet<UpdateElementConsumerPosition>();
			BubbleSpawner bubbleSpawner = prefab.AddComponent<BubbleSpawner>();
			bubbleSpawner.element = SimHashes.Water;
			bubbleSpawner.emitMass = 2f;
			bubbleSpawner.emitVariance = 0.5f;
			bubbleSpawner.initialVelocity = new Vector2f(0, 1);
			ElementConverter elementConverter = prefab.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
			{
				new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 0.2f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[1]
			{
				new ElementConverter.OutputElement(0.2f, SimHashes.Water, 0f, useEntityTemperature: true, storeOutput: true)
			};
		}
		return prefab;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(EntityTemplates.ExtendEntityToWildCreature(CreatePacu("PacuCleaner", STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "pacu_kanim", is_baby: false), PacuTuning.PEN_SIZE_PER_CREATURE), "PacuCleanerEgg", STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.EGG_NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuCleanerBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_CLEANER, 501, is_ranchable: false, add_fish_overcrowding_monitor: true, add_fixed_capturable_monitor: false, 0.75f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		ElementConsumer component = inst.GetComponent<ElementConsumer>();
		if (component != null)
		{
			component.EnableConsumption(enabled: true);
		}
	}
}
