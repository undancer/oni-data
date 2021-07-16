using System.Collections.Generic;

public static class StaterpillarTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "StaterpillarEgg".ToTag(),
			weight = 1f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 2000000f;

	public static float STANDARD_STARVE_CYCLES = 5f;

	public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;

	public static float POOP_CONVERSTION_RATE = 0.05f;

	public static float EGG_MASS = 2f;
}
