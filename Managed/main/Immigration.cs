using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Immigration")]
public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms, IPersonalPriorityManager
{
	public float[] spawnInterval;

	public int[] spawnTable;

	[Serialize]
	private Dictionary<HashedString, int> defaultPersonalPriorities = new Dictionary<HashedString, int>();

	[Serialize]
	public float timeBeforeSpawn = float.PositiveInfinity;

	[Serialize]
	private bool bImmigrantAvailable;

	[Serialize]
	private int spawnIdx;

	private CarePackageInfo[] carePackages;

	public static Immigration Instance;

	private const int CYCLE_THRESHOLD_A = 6;

	private const int CYCLE_THRESHOLD_B = 12;

	private const int CYCLE_THRESHOLD_C = 24;

	private const int CYCLE_THRESHOLD_D = 48;

	public bool ImmigrantsAvailable => bImmigrantAvailable;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		bImmigrantAvailable = false;
		Instance = this;
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		timeBeforeSpawn = spawnInterval[num];
		ResetPersonalPriorities();
		ConfigureCarePackages();
	}

	private void ConfigureCarePackages()
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			ConfigureMultiWorldCarePackages();
		}
		else
		{
			ConfigureBaseGameCarePackages();
		}
	}

	private void ConfigureBaseGameCarePackages()
	{
		carePackages = new CarePackageInfo[58]
		{
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => CycleCondition(12)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => CycleCondition(12) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => CycleCondition(12) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => CycleCondition(24) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => CycleCondition(24) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)),
			new CarePackageInfo("PrickleGrassSeed", 3f, null),
			new CarePackageInfo("LeafyPlantSeed", 3f, null),
			new CarePackageInfo("CactusPlantSeed", 3f, null),
			new CarePackageInfo("MushroomSeed", 1f, null),
			new CarePackageInfo("PrickleFlowerSeed", 2f, null),
			new CarePackageInfo("OxyfernSeed", 1f, null),
			new CarePackageInfo("ForestTreeSeed", 1f, null),
			new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => CycleCondition(24)),
			new CarePackageInfo("SwampLilySeed", 1f, () => CycleCondition(24)),
			new CarePackageInfo("ColdBreatherSeed", 1f, () => CycleCondition(24)),
			new CarePackageInfo("SpiceVineSeed", 1f, () => CycleCondition(24)),
			new CarePackageInfo("FieldRation", 5f, null),
			new CarePackageInfo("BasicForagePlant", 6f, null),
			new CarePackageInfo("CookedEgg", 3f, () => CycleCondition(6)),
			new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => CycleCondition(12)),
			new CarePackageInfo("FriedMushroom", 3f, () => CycleCondition(24)),
			new CarePackageInfo("CookedMeat", 3f, () => CycleCondition(48)),
			new CarePackageInfo("SpicyTofu", 3f, () => CycleCondition(48)),
			new CarePackageInfo("LightBugBaby", 1f, null),
			new CarePackageInfo("HatchBaby", 1f, null),
			new CarePackageInfo("PuftBaby", 1f, null),
			new CarePackageInfo("SquirrelBaby", 1f, null),
			new CarePackageInfo("CrabBaby", 1f, null),
			new CarePackageInfo("DreckoBaby", 1f, () => CycleCondition(24)),
			new CarePackageInfo("Pacu", 8f, () => CycleCondition(24)),
			new CarePackageInfo("MoleBaby", 1f, () => CycleCondition(48)),
			new CarePackageInfo("OilfloaterBaby", 1f, () => CycleCondition(48)),
			new CarePackageInfo("LightBugEgg", 3f, null),
			new CarePackageInfo("HatchEgg", 3f, null),
			new CarePackageInfo("PuftEgg", 3f, null),
			new CarePackageInfo("OilfloaterEgg", 3f, () => CycleCondition(12)),
			new CarePackageInfo("MoleEgg", 3f, () => CycleCondition(24)),
			new CarePackageInfo("DreckoEgg", 3f, () => CycleCondition(24)),
			new CarePackageInfo("SquirrelEgg", 2f, null),
			new CarePackageInfo("BasicCure", 3f, null),
			new CarePackageInfo("Funky_Vest", 1f, null)
		};
	}

	private void ConfigureMultiWorldCarePackages()
	{
		carePackages = new CarePackageInfo[65]
		{
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, () => CycleCondition(12)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, null),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, () => CycleCondition(12) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, () => CycleCondition(12) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, () => CycleCondition(24) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, () => CycleCondition(24) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag)),
			new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, () => CycleCondition(48) && DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag)),
			new CarePackageInfo("PrickleGrassSeed", 3f, null),
			new CarePackageInfo("LeafyPlantSeed", 3f, null),
			new CarePackageInfo("CactusPlantSeed", 3f, null),
			new CarePackageInfo("MushroomSeed", 1f, () => DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.SlimeMold).tag)),
			new CarePackageInfo("PrickleFlowerSeed", 2f, () => DiscoveredCondition("PrickleFlowerSeed")),
			new CarePackageInfo("OxyfernSeed", 1f, null),
			new CarePackageInfo("ForestTreeSeed", 1f, () => DiscoveredCondition("ForestTreeSeed")),
			new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, () => CycleCondition(24) && DiscoveredCondition(BasicFabricMaterialPlantConfig.SEED_ID)),
			new CarePackageInfo("SwampLilySeed", 1f, () => CycleCondition(24) && DiscoveredCondition("SwampLilySeed")),
			new CarePackageInfo("ColdBreatherSeed", 1f, () => CycleCondition(24) && DiscoveredCondition("ColdBreatherSeed")),
			new CarePackageInfo("SpiceVineSeed", 1f, () => CycleCondition(24) && DiscoveredCondition("SpiceVineSeed")),
			new CarePackageInfo("FieldRation", 5f, null),
			new CarePackageInfo("BasicForagePlant", 6f, () => DiscoveredCondition("BasicForagePlant")),
			new CarePackageInfo("ForestForagePlant", 2f, () => DiscoveredCondition("ForestForagePlant")),
			new CarePackageInfo("SwampForagePlant", 2f, () => DiscoveredCondition("SwampForagePlant")),
			new CarePackageInfo("CookedEgg", 3f, () => CycleCondition(6)),
			new CarePackageInfo(PrickleFruitConfig.ID, 3f, () => CycleCondition(12) && DiscoveredCondition(PrickleFruitConfig.ID)),
			new CarePackageInfo("FriedMushroom", 3f, () => CycleCondition(24) && DiscoveredCondition("FriedMushroom")),
			new CarePackageInfo("CookedMeat", 3f, () => CycleCondition(48)),
			new CarePackageInfo("SpicyTofu", 3f, () => CycleCondition(48) && DiscoveredCondition("SpicyTofu")),
			new CarePackageInfo("WormSuperFood", 2f, () => DiscoveredCondition("WormPlantSeed")),
			new CarePackageInfo("LightBugBaby", 1f, () => DiscoveredCondition("LightBugEgg")),
			new CarePackageInfo("HatchBaby", 1f, () => DiscoveredCondition("HatchEgg")),
			new CarePackageInfo("PuftBaby", 1f, () => DiscoveredCondition("PuftEgg")),
			new CarePackageInfo("SquirrelBaby", 1f, () => DiscoveredCondition("SquirrelEgg") || CycleCondition(24)),
			new CarePackageInfo("CrabBaby", 1f, () => DiscoveredCondition("CrabEgg")),
			new CarePackageInfo("DreckoBaby", 1f, () => CycleCondition(24) && DiscoveredCondition("DreckoEgg")),
			new CarePackageInfo("Pacu", 8f, () => CycleCondition(24) && DiscoveredCondition("PacuEgg")),
			new CarePackageInfo("MoleBaby", 1f, () => CycleCondition(48) && DiscoveredCondition("MoleEgg")),
			new CarePackageInfo("OilfloaterBaby", 1f, () => CycleCondition(48) && DiscoveredCondition("OilfloaterEgg")),
			new CarePackageInfo("DivergentBeetleBaby", 1f, () => CycleCondition(48) && DiscoveredCondition("DivergentBeetleEgg")),
			new CarePackageInfo("StaterpillarBaby", 1f, () => CycleCondition(48) && DiscoveredCondition("StaterpillarEgg")),
			new CarePackageInfo("LightBugEgg", 3f, () => DiscoveredCondition("LightBugEgg")),
			new CarePackageInfo("HatchEgg", 3f, () => DiscoveredCondition("HatchEgg")),
			new CarePackageInfo("PuftEgg", 3f, () => DiscoveredCondition("PuftEgg")),
			new CarePackageInfo("OilfloaterEgg", 3f, () => CycleCondition(12) && DiscoveredCondition("OilfloaterEgg")),
			new CarePackageInfo("MoleEgg", 3f, () => CycleCondition(24) && DiscoveredCondition("MoleEgg")),
			new CarePackageInfo("DreckoEgg", 3f, () => CycleCondition(24) && DiscoveredCondition("DreckoEgg")),
			new CarePackageInfo("SquirrelEgg", 2f, () => DiscoveredCondition("SquirrelEgg") || CycleCondition(24)),
			new CarePackageInfo("DivergentBeetleEgg", 2f, () => CycleCondition(48) && DiscoveredCondition("DivergentBeetleEgg")),
			new CarePackageInfo("StaterpillarEgg", 2f, () => CycleCondition(48) && DiscoveredCondition("StaterpillarEgg")),
			new CarePackageInfo("BasicCure", 3f, null),
			new CarePackageInfo("Funky_Vest", 1f, null)
		};
	}

	private bool CycleCondition(int cycle)
	{
		return GameClock.Instance.GetCycle() >= cycle;
	}

	private bool DiscoveredCondition(Tag tag)
	{
		return DiscoveredResources.Instance.IsDiscovered(tag);
	}

	public int EndImmigration()
	{
		bImmigrantAvailable = false;
		spawnIdx++;
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		timeBeforeSpawn = spawnInterval[num];
		return spawnTable[num];
	}

	public float GetTimeRemaining()
	{
		return timeBeforeSpawn;
	}

	public float GetTotalWaitTime()
	{
		int num = Math.Min(spawnIdx, spawnInterval.Length - 1);
		return spawnInterval[num];
	}

	public void Sim200ms(float dt)
	{
		if (!IsHalted() && !bImmigrantAvailable)
		{
			timeBeforeSpawn -= dt;
			timeBeforeSpawn = Math.Max(timeBeforeSpawn, 0f);
			if (timeBeforeSpawn <= 0f)
			{
				bImmigrantAvailable = true;
			}
		}
	}

	private bool IsHalted()
	{
		foreach (Telepad item in Components.Telepads.Items)
		{
			Operational component = item.GetComponent<Operational>();
			if (component != null && component.IsOperational)
			{
				return false;
			}
		}
		return true;
	}

	public int GetPersonalPriority(ChoreGroup group)
	{
		if (!defaultPersonalPriorities.TryGetValue(group.IdHash, out var value))
		{
			return 3;
		}
		return value;
	}

	public CarePackageInfo RandomCarePackage()
	{
		List<CarePackageInfo> list = new List<CarePackageInfo>();
		CarePackageInfo[] array = carePackages;
		foreach (CarePackageInfo carePackageInfo in array)
		{
			if (carePackageInfo.requirement == null || carePackageInfo.requirement())
			{
				list.Add(carePackageInfo);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
		defaultPersonalPriorities[group.IdHash] = value;
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return 0;
	}

	public void ApplyDefaultPersonalPriorities(GameObject minion)
	{
		IPersonalPriorityManager instance = Instance;
		IPersonalPriorityManager component = minion.GetComponent<ChoreConsumer>();
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			int personalPriority = instance.GetPersonalPriority(resource);
			component.SetPersonalPriority(resource, personalPriority);
		}
	}

	public void ResetPersonalPriorities()
	{
		bool advancedPersonalPriorities = Game.Instance.advancedPersonalPriorities;
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			defaultPersonalPriorities[resource.IdHash] = (advancedPersonalPriorities ? resource.DefaultPersonalPriority : 3);
		}
	}

	public bool IsChoreGroupDisabled(ChoreGroup g)
	{
		return false;
	}
}
