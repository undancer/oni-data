using System.Collections.Generic;
using TUNING;

public static class DreckoTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "DreckoEgg".ToTag(),
			weight = 0.98f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "DreckoPlasticEgg".ToTag(),
			weight = 0.02f
		}
	};

	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_PLASTIC = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "DreckoEgg".ToTag(),
			weight = 0.35f
		},
		new FertilityMonitor.BreedingChance
		{
			egg = "DreckoPlasticEgg".ToTag(),
			weight = 0.65f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 2000000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public static float EGG_MASS = 2f;
}
