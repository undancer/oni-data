public static class BeeHiveTuning
{
	public static float ORE_DELIVERY_AMOUNT = 1f;

	public static float KG_ORE_EATEN_PER_CYCLE = ORE_DELIVERY_AMOUNT * 10f;

	public static float STANDARD_CALORIES_PER_CYCLE = 1500000f;

	public static float STANDARD_STARVE_CYCLES = 30f;

	public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;

	public static float CALORIES_PER_KG_OF_ORE = STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static float POOP_CONVERSTION_RATE = 0.9f;

	public static Tag CONSUMED_ORE = SimHashes.UraniumOre.CreateTag();

	public static Tag PRODUCED_ORE = SimHashes.EnrichedUranium.CreateTag();

	public static float HIVE_GROWTH_TIME = 2f;

	public static float WASTE_DROPPED_ON_DEATH = 5f;

	public static int GERMS_DROPPED_ON_DEATH = 10000;
}
