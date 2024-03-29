using System.Collections.Generic;
using UnityEngine;

namespace TUNING
{
	public class DUPLICANTSTATS
	{
		public class BASESTATS
		{
			public const float STAMINA_USED_PER_SECOND = -7f / 60f;

			public const float MAX_CALORIES = 4000000f;

			public const float CALORIES_BURNED_PER_CYCLE = -1000000f;

			public const float CALORIES_BURNED_PER_SECOND = -1666.6666f;

			public const float GUESSTIMATE_CALORIES_PER_CYCLE = -1600000f;

			public const float GUESSTIMATE_CALORIES_BURNED_PER_SECOND = -1666.6666f;

			public const float OXYGEN_USED_PER_SECOND = 0.1f;

			public const float OXYGEN_TO_CO2_CONVERSION = 0.02f;

			public const float LOW_OXYGEN_THRESHOLD = 0.52f;

			public const float NO_OXYGEN_THRESHOLD = 0.05f;

			public const float MIN_CO2_TO_EMIT = 0.02f;

			public const float BLADDER_INCREASE_PER_SECOND = 1f / 6f;

			public const float DECOR_EXPECTATION = 0f;

			public const float FOOD_QUALITY_EXPECTATION = 0f;

			public const float RECREATION_EXPECTATION = 2f;

			public const float MAX_PROFESSION_DECOR_EXPECTATION = 75f;

			public const float MAX_PROFESSION_FOOD_EXPECTATION = 0f;

			public const int MAX_UNDERWATER_TRAVEL_COST = 8;

			public const float TOILET_EFFICIENCY = 1f;

			public const float ROOM_TEMPERATURE_PREFERENCE = 0f;

			public const int BUILDING_DAMAGE_ACTING_OUT = 100;

			public const float IMMUNE_LEVEL_MAX = 100f;

			public const float IMMUNE_LEVEL_RECOVERY = 0.025f;

			public const float CARRY_CAPACITY = 200f;

			public const float HIT_POINTS = 100f;

			public const float RADIATION_RESISTANCE = 0f;
		}

		public class RADIATION_DIFFICULTY_MODIFIERS
		{
			public static float HARDEST = 0.33f;

			public static float HARDER = 0.66f;

			public static float DEFAULT = 1f;

			public static float EASIER = 2f;

			public static float EASIEST = 100f;
		}

		public class RADIATION_EXPOSURE_LEVELS
		{
			public const float LOW = 100f;

			public const float MODERATE = 300f;

			public const float HIGH = 600f;

			public const float DEADLY = 900f;
		}

		public class CALORIES
		{
			public const float SATISFIED_THRESHOLD = 0.95f;

			public const float HUNGRY_THRESHOLD = 0.825f;

			public const float STARVING_THRESHOLD = 0.25f;
		}

		public class TEMPERATURE
		{
			public class EXTERNAL
			{
				public const float THRESHOLD_COLD = 283.15f;

				public const float THRESHOLD_HOT = 306.15f;

				public const float THRESHOLD_SCALDING = 345f;
			}

			public class INTERNAL
			{
				public const float IDEAL = 310.15f;

				public const float THRESHOLD_HYPOTHERMIA = 308.15f;

				public const float THRESHOLD_HYPERTHERMIA = 312.15f;

				public const float THRESHOLD_FATAL_HOT = 320.15f;

				public const float THRESHOLD_FATAL_COLD = 300.15f;
			}

			public class CONDUCTIVITY_BARRIER_MODIFICATION
			{
				public const float SKINNY = -0.005f;

				public const float PUDGY = 0.005f;
			}

			public const float SKIN_THICKNESS = 0.002f;

			public const float SURFACE_AREA = 1f;

			public const float GROUND_TRANSFER_SCALE = 0f;
		}

		public class NOISE
		{
			public const int THRESHOLD_PEACEFUL = 0;

			public const int THRESHOLD_QUIET = 36;

			public const int THRESHOLD_TOSS_AND_TURN = 45;

			public const int THRESHOLD_WAKE_UP = 60;

			public const int THRESHOLD_MINOR_REACTION = 80;

			public const int THRESHOLD_MAJOR_REACTION = 106;

			public const int THRESHOLD_EXTREME_REACTION = 125;
		}

		public class BREATH
		{
			private const float BREATH_BAR_TOTAL_SECONDS = 110f;

			private const float RETREAT_AT_SECONDS = 80f;

			private const float SUFFOCATION_WARN_AT_SECONDS = 50f;

			public const float BREATH_BAR_TOTAL_AMOUNT = 100f;

			public const float RETREAT_AMOUNT = 72.72727f;

			public const float SUFFOCATE_AMOUNT = 45.454548f;

			public const float BREATH_RATE = 0.90909094f;
		}

		public class LIGHT
		{
			public const int LUX_SUNBURN = 72000;

			public const float SUNBURN_DELAY_TIME = 120f;

			public const int LUX_PLEASANT_LIGHT = 40000;

			public static float LIGHT_WORK_EFFICIENCY_BONUS = 0.15f;

			public const int NO_LIGHT = 0;

			public const int VERY_LOW_LIGHT = 1;

			public const int LOW_LIGHT = 100;

			public const int MEDIUM_LIGHT = 1000;

			public const int HIGH_LIGHT = 10000;

			public const int VERY_HIGH_LIGHT = 50000;

			public const int MAX_LIGHT = 100000;
		}

		public class MOVEMENT
		{
			public static float NEUTRAL = 1f;

			public static float BONUS_1 = 1.1f;

			public static float BONUS_2 = 1.25f;

			public static float BONUS_3 = 1.5f;

			public static float BONUS_4 = 1.75f;

			public static float PENALTY_1 = 0.9f;

			public static float PENALTY_2 = 0.75f;

			public static float PENALTY_3 = 0.5f;

			public static float PENALTY_4 = 0.25f;
		}

		public class QOL_STRESS
		{
			public class BELOW_EXPECTATIONS
			{
				public const float EASY = 0.0033333334f;

				public const float NEUTRAL = 0.004166667f;

				public const float HARD = 1f / 120f;

				public const float VERYHARD = 1f / 60f;
			}

			public class MAX_STRESS
			{
				public const float EASY = 1f / 60f;

				public const float NEUTRAL = 1f / 24f;

				public const float HARD = 0.05f;

				public const float VERYHARD = 1f / 12f;
			}

			public const float ABOVE_EXPECTATIONS = -1f / 60f;

			public const float AT_EXPECTATIONS = -1f / 120f;

			public const float MIN_STRESS = -1f / 30f;
		}

		public class COMBAT
		{
			public class BASICWEAPON
			{
				public const float ATTACKS_PER_SECOND = 2f;

				public const float MIN_DAMAGE_PER_HIT = 1f;

				public const float MAX_DAMAGE_PER_HIT = 1f;

				public const AttackProperties.TargetType TARGET_TYPE = AttackProperties.TargetType.Single;

				public const AttackProperties.DamageType DAMAGE_TYPE = AttackProperties.DamageType.Standard;

				public const int MAX_HITS = 1;

				public const float AREA_OF_EFFECT_RADIUS = 0f;
			}

			public const Health.HealthState FLEE_THRESHOLD = Health.HealthState.Critical;
		}

		public class CLOTHING
		{
			public class DECOR_MODIFICATION
			{
				public const int NEGATIVE_SIGNIFICANT = -30;

				public const int NEGATIVE_MILD = -10;

				public const int BASIC = -5;

				public const int POSITIVE_MILD = 10;

				public const int POSITIVE_SIGNIFICANT = 30;

				public const int POSITIVE_MAJOR = 40;
			}

			public class CONDUCTIVITY_BARRIER_MODIFICATION
			{
				public const float THIN = 0.0005f;

				public const float BASIC = 0.0025f;

				public const float THICK = 0.01f;
			}

			public class SWEAT_EFFICIENCY_MULTIPLIER
			{
				public const float DIMINISH_SIGNIFICANT = -2.5f;

				public const float DIMINISH_MILD = -1.25f;

				public const float NEUTRAL = 0f;

				public const float IMPROVE = 2f;
			}
		}

		public class DISTRIBUTIONS
		{
			public static readonly List<int[]> TYPES = new List<int[]>
			{
				new int[7] { 5, 4, 4, 3, 3, 2, 1 },
				new int[4] { 5, 3, 2, 1 },
				new int[4] { 5, 2, 2, 1 },
				new int[2] { 5, 1 },
				new int[3] { 5, 3, 1 },
				new int[5] { 3, 3, 3, 3, 1 },
				new int[1] { 4 },
				new int[1] { 3 },
				new int[1] { 2 },
				new int[1] { 1 }
			};

			public static int[] GetRandomDistribution()
			{
				return TYPES[Random.Range(0, TYPES.Count)];
			}
		}

		public struct TraitVal
		{
			public string id;

			public int statBonus;

			public int impact;

			public int rarity;

			public string dlcId;

			public List<string> mutuallyExclusiveTraits;

			public List<HashedString> mutuallyExclusiveAptitudes;

			public bool doNotGenerateTrait;
		}

		public class ATTRIBUTE_LEVELING
		{
			public static int MAX_GAINED_ATTRIBUTE_LEVEL = 20;

			public static int TARGET_MAX_LEVEL_CYCLE = 400;

			public static float EXPERIENCE_LEVEL_POWER = 1.7f;

			public static float FULL_EXPERIENCE = 1f;

			public static float ALL_DAY_EXPERIENCE = FULL_EXPERIENCE / 0.8f;

			public static float MOST_DAY_EXPERIENCE = FULL_EXPERIENCE / 0.5f;

			public static float PART_DAY_EXPERIENCE = FULL_EXPERIENCE / 0.25f;

			public static float BARELY_EVER_EXPERIENCE = FULL_EXPERIENCE / 0.1f;
		}

		public const float DEFAULT_MASS = 30f;

		public const float PEE_FUSE_TIME = 120f;

		public const float PEE_PER_FLOOR_PEE = 2f;

		public const float PEE_PER_TOILET_PEE = 6.7f;

		public const string PEE_DISEASE = "FoodPoisoning";

		public const int DISEASE_PER_PEE = 100000;

		public const int DISEASE_PER_VOMIT = 100000;

		public const float KCAL2JOULES = 4184f;

		public const float COOLING_EFFICIENCY = 0.08f;

		public const float DUPLICANT_COOLING_KILOWATTS = 0.5578667f;

		public const float WARMING_EFFICIENCY = 0.08f;

		public const float DUPLICANT_WARMING_KILOWATTS = 0.5578667f;

		public const float HEAT_GENERATION_EFFICIENCY = 0.012f;

		public const float DUPLICANT_BASE_GENERATION_KILOWATTS = 0.08368001f;

		public const float STANDARD_STRESS_PENALTY = 1f / 60f;

		public const float STANDARD_STRESS_BONUS = -1f / 30f;

		public const float RANCHING_DURATION_MULTIPLIER_BONUS_PER_POINT = 0.1f;

		public const float FARMING_DURATION_MULTIPLIER_BONUS_PER_POINT = 0.1f;

		public const float POWER_DURATION_MULTIPLIER_BONUS_PER_POINT = 0.025f;

		public const float RANCHING_CAPTURABLE_MULTIPLIER_BONUS_PER_POINT = 0.05f;

		public const float STRESS_BELOW_EXPECTATIONS_FOOD = 0.25f;

		public const float STRESS_ABOVE_EXPECTATIONS_FOOD = -0.5f;

		public const float STANDARD_STRESS_PENALTY_SECOND = 0.25f;

		public const float STANDARD_STRESS_BONUS_SECOND = -0.5f;

		public const float RECOVER_BREATH_DELTA = 3f;

		public const float TRAVEL_TIME_WARNING_THRESHOLD = 0.4f;

		public static readonly string[] ALL_ATTRIBUTES = new string[12]
		{
			"Strength", "Caring", "Construction", "Digging", "Machinery", "Learning", "Cooking", "Botanist", "Art", "Ranching",
			"Athletics", "SpaceNavigation"
		};

		public static readonly string[] DISTRIBUTED_ATTRIBUTES = new string[10] { "Strength", "Caring", "Construction", "Digging", "Machinery", "Learning", "Cooking", "Botanist", "Art", "Ranching" };

		public static readonly string[] ROLLED_ATTRIBUTES = new string[1] { "Athletics" };

		public static readonly int[] APTITUDE_ATTRIBUTE_BONUSES = new int[3] { 7, 3, 1 };

		public static int ROLLED_ATTRIBUTE_MAX = 5;

		public static float ROLLED_ATTRIBUTE_POWER = 4f;

		public static Dictionary<string, List<string>> ARCHETYPE_TRAIT_EXCLUSIONS = new Dictionary<string, List<string>>
		{
			{
				"Mining",
				new List<string> { "Anemic", "DiggingDown", "Narcolepsy" }
			},
			{
				"Building",
				new List<string> { "Anemic", "NoodleArms", "ConstructionDown", "DiggingDown", "Narcolepsy" }
			},
			{
				"Farming",
				new List<string> { "Anemic", "NoodleArms", "BotanistDown", "RanchingDown", "Narcolepsy" }
			},
			{
				"Ranching",
				new List<string> { "RanchingDown", "BotanistDown", "Narcolepsy" }
			},
			{
				"Cooking",
				new List<string> { "NoodleArms", "CookingDown" }
			},
			{
				"Art",
				new List<string> { "ArtDown", "DecorDown" }
			},
			{
				"Research",
				new List<string> { "SlowLearner" }
			},
			{
				"Suits",
				new List<string> { "Anemic", "NoodleArms" }
			},
			{
				"Hauling",
				new List<string> { "Anemic", "NoodleArms", "Narcolepsy" }
			},
			{
				"Technicals",
				new List<string> { "MachineryDown" }
			},
			{
				"MedicalAid",
				new List<string> { "CaringDown", "WeakImmuneSystem" }
			},
			{
				"Basekeeping",
				new List<string> { "Anemic", "NoodleArms" }
			},
			{
				"Rocketry",
				new List<string>()
			}
		};

		public static int RARITY_LEGENDARY = 5;

		public static int RARITY_EPIC = 4;

		public static int RARITY_RARE = 3;

		public static int RARITY_UNCOMMON = 2;

		public static int RARITY_COMMON = 1;

		public static int NO_STATPOINT_BONUS = 0;

		public static int TINY_STATPOINT_BONUS = 1;

		public static int SMALL_STATPOINT_BONUS = 2;

		public static int MEDIUM_STATPOINT_BONUS = 3;

		public static int LARGE_STATPOINT_BONUS = 4;

		public static int HUGE_STATPOINT_BONUS = 5;

		public static int COMMON = 1;

		public static int UNCOMMON = 2;

		public static int RARE = 3;

		public static int EPIC = 4;

		public static int LEGENDARY = 5;

		public static Tuple<int, int> TRAITS_ONE_POSITIVE_ONE_NEGATIVE = new Tuple<int, int>(1, 1);

		public static Tuple<int, int> TRAITS_TWO_POSITIVE_ONE_NEGATIVE = new Tuple<int, int>(2, 1);

		public static Tuple<int, int> TRAITS_ONE_POSITIVE_TWO_NEGATIVE = new Tuple<int, int>(1, 2);

		public static Tuple<int, int> TRAITS_TWO_POSITIVE_TWO_NEGATIVE = new Tuple<int, int>(2, 2);

		public static Tuple<int, int> TRAITS_THREE_POSITIVE_ONE_NEGATIVE = new Tuple<int, int>(3, 1);

		public static Tuple<int, int> TRAITS_ONE_POSITIVE_THREE_NEGATIVE = new Tuple<int, int>(1, 3);

		public static int MIN_STAT_POINTS = 0;

		public static int MAX_STAT_POINTS = 0;

		public static int MAX_TRAITS = 4;

		public static int APTITUDE_BONUS = 1;

		public static List<int> RARITY_DECK = new List<int>
		{
			RARITY_COMMON, RARITY_COMMON, RARITY_COMMON, RARITY_COMMON, RARITY_COMMON, RARITY_COMMON, RARITY_COMMON, RARITY_UNCOMMON, RARITY_UNCOMMON, RARITY_UNCOMMON,
			RARITY_UNCOMMON, RARITY_UNCOMMON, RARITY_UNCOMMON, RARITY_RARE, RARITY_RARE, RARITY_RARE, RARITY_RARE, RARITY_EPIC, RARITY_EPIC, RARITY_LEGENDARY
		};

		public static List<int> rarityDeckActive = new List<int>(RARITY_DECK);

		public static List<Tuple<int, int>> POD_TRAIT_CONFIGURATIONS_DECK = new List<Tuple<int, int>>
		{
			TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_ONE_NEGATIVE, TRAITS_TWO_POSITIVE_ONE_NEGATIVE, TRAITS_TWO_POSITIVE_ONE_NEGATIVE, TRAITS_TWO_POSITIVE_ONE_NEGATIVE, TRAITS_TWO_POSITIVE_ONE_NEGATIVE,
			TRAITS_TWO_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_TWO_NEGATIVE, TRAITS_ONE_POSITIVE_TWO_NEGATIVE, TRAITS_ONE_POSITIVE_TWO_NEGATIVE, TRAITS_ONE_POSITIVE_TWO_NEGATIVE, TRAITS_TWO_POSITIVE_ONE_NEGATIVE, TRAITS_TWO_POSITIVE_TWO_NEGATIVE, TRAITS_TWO_POSITIVE_TWO_NEGATIVE, TRAITS_THREE_POSITIVE_ONE_NEGATIVE, TRAITS_ONE_POSITIVE_THREE_NEGATIVE
		};

		public static List<Tuple<int, int>> podTraitConfigurationsActive = new List<Tuple<int, int>>(POD_TRAIT_CONFIGURATIONS_DECK);

		public static readonly List<string> CONTRACTEDTRAITS_HEALING = new List<string> { "IrritableBowel", "Aggressive", "SlowLearner", "WeakImmuneSystem", "Snorer", "CantDig" };

		public static readonly List<TraitVal> CONGENITALTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "None"
			},
			new TraitVal
			{
				id = "Joshua",
				mutuallyExclusiveTraits = new List<string> { "ScaredyCat", "Aggressive" }
			},
			new TraitVal
			{
				id = "Ellie",
				statBonus = TINY_STATPOINT_BONUS,
				mutuallyExclusiveTraits = new List<string> { "InteriorDecorator", "MouthBreather", "Uncultured" }
			},
			new TraitVal
			{
				id = "Stinky",
				mutuallyExclusiveTraits = new List<string> { "Flatulence", "InteriorDecorator" }
			},
			new TraitVal
			{
				id = "Liam",
				mutuallyExclusiveTraits = new List<string> { "Flatulence", "InteriorDecorator" }
			}
		};

		public static readonly TraitVal INVALID_TRAIT_VAL = new TraitVal
		{
			id = "INVALID"
		};

		public static readonly List<TraitVal> BADTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "CantResearch",
				statBonus = NO_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "Research" }
			},
			new TraitVal
			{
				id = "CantDig",
				statBonus = LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "Mining" }
			},
			new TraitVal
			{
				id = "CantCook",
				statBonus = NO_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "Cooking" }
			},
			new TraitVal
			{
				id = "CantBuild",
				statBonus = LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "Building" },
				mutuallyExclusiveTraits = new List<string> { "GrantSkill_Engineering1" }
			},
			new TraitVal
			{
				id = "Hemophobia",
				statBonus = NO_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "MedicalAid" }
			},
			new TraitVal
			{
				id = "ScaredyCat",
				statBonus = NO_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveAptitudes = new List<HashedString> { "Mining" }
			},
			new TraitVal
			{
				id = "ConstructionDown",
				statBonus = MEDIUM_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "ConstructionUp", "CantBuild" }
			},
			new TraitVal
			{
				id = "RanchingDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "RanchingUp" }
			},
			new TraitVal
			{
				id = "CaringDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Hemophobia" }
			},
			new TraitVal
			{
				id = "BotanistDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "ArtDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "CookingDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Foodie", "CantCook" }
			},
			new TraitVal
			{
				id = "MachineryDown",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "DiggingDown",
				statBonus = MEDIUM_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "MoleHands", "CantDig" }
			},
			new TraitVal
			{
				id = "SlowLearner",
				statBonus = MEDIUM_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "FastLearner", "CantResearch" }
			},
			new TraitVal
			{
				id = "NoodleArms",
				statBonus = MEDIUM_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			},
			new TraitVal
			{
				id = "DecorDown",
				statBonus = TINY_STATPOINT_BONUS,
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Anemic",
				statBonus = HUGE_STATPOINT_BONUS,
				rarity = RARITY_LEGENDARY,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Flatulence",
				statBonus = MEDIUM_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			},
			new TraitVal
			{
				id = "IrritableBowel",
				statBonus = TINY_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Snorer",
				statBonus = TINY_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			},
			new TraitVal
			{
				id = "MouthBreather",
				statBonus = HUGE_STATPOINT_BONUS,
				rarity = RARITY_LEGENDARY,
				dlcId = ""
			},
			new TraitVal
			{
				id = "SmallBladder",
				statBonus = TINY_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "CalorieBurner",
				statBonus = LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "WeakImmuneSystem",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_UNCOMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Allergies",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			},
			new TraitVal
			{
				id = "NightLight",
				statBonus = SMALL_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Narcolepsy",
				statBonus = HUGE_STATPOINT_BONUS,
				rarity = RARITY_RARE,
				dlcId = ""
			}
		};

		public static readonly List<TraitVal> STRESSTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "Aggressive",
				dlcId = ""
			},
			new TraitVal
			{
				id = "StressVomiter",
				dlcId = ""
			},
			new TraitVal
			{
				id = "UglyCrier",
				dlcId = ""
			},
			new TraitVal
			{
				id = "BingeEater",
				dlcId = ""
			},
			new TraitVal
			{
				id = "Banshee",
				dlcId = ""
			}
		};

		public static readonly List<TraitVal> JOYTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "BalloonArtist",
				dlcId = ""
			},
			new TraitVal
			{
				id = "SparkleStreaker",
				dlcId = ""
			},
			new TraitVal
			{
				id = "StickerBomber",
				dlcId = ""
			},
			new TraitVal
			{
				id = "SuperProductive",
				dlcId = ""
			},
			new TraitVal
			{
				id = "HappySinger",
				dlcId = ""
			}
		};

		public static readonly List<TraitVal> GENESHUFFLERTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "Regeneration",
				dlcId = ""
			},
			new TraitVal
			{
				id = "DeeperDiversLungs",
				dlcId = ""
			},
			new TraitVal
			{
				id = "SunnyDisposition",
				dlcId = ""
			},
			new TraitVal
			{
				id = "RockCrusher",
				dlcId = ""
			}
		};

		public static readonly List<TraitVal> SPECIALTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "AncientKnowledge",
				rarity = RARITY_LEGENDARY,
				dlcId = "EXPANSION1_ID",
				doNotGenerateTrait = true,
				mutuallyExclusiveTraits = new List<string>
				{
					"CantResearch", "CantBuild", "CantCook", "CantDig", "Hemophobia", "ScaredyCat", "Anemic", "SlowLearner", "NoodleArms", "ConstructionDown",
					"RanchingDown", "DiggingDown", "MachineryDown", "CookingDown", "ArtDown", "CaringDown", "BotanistDown"
				}
			}
		};

		public static readonly List<TraitVal> GOODTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "Twinkletoes",
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Anemic" }
			},
			new TraitVal
			{
				id = "StrongArm",
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "NoodleArms" }
			},
			new TraitVal
			{
				id = "Greasemonkey",
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "MachineryDown" }
			},
			new TraitVal
			{
				id = "DiversLung",
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "MouthBreather" }
			},
			new TraitVal
			{
				id = "IronGut",
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "StrongImmuneSystem",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "WeakImmuneSystem" }
			},
			new TraitVal
			{
				id = "EarlyBird",
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "NightOwl" }
			},
			new TraitVal
			{
				id = "NightOwl",
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "EarlyBird" }
			},
			new TraitVal
			{
				id = "MoleHands",
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "CantDig", "DiggingDown" }
			},
			new TraitVal
			{
				id = "FastLearner",
				rarity = RARITY_RARE,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "SlowLearner", "CantResearch" }
			},
			new TraitVal
			{
				id = "InteriorDecorator",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Uncultured", "ArtDown" }
			},
			new TraitVal
			{
				id = "Uncultured",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "InteriorDecorator" },
				mutuallyExclusiveAptitudes = new List<HashedString> { "Art" }
			},
			new TraitVal
			{
				id = "SimpleTastes",
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Foodie" }
			},
			new TraitVal
			{
				id = "Foodie",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "SimpleTastes", "CantCook", "CookingDown" },
				mutuallyExclusiveAptitudes = new List<HashedString> { "Cooking" }
			},
			new TraitVal
			{
				id = "BedsideManner",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Hemophobia", "CaringDown" }
			},
			new TraitVal
			{
				id = "DecorUp",
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "DecorDown" }
			},
			new TraitVal
			{
				id = "Thriver",
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GreenThumb",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "BotanistDown" }
			},
			new TraitVal
			{
				id = "ConstructionUp",
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "ConstructionDown" }
			},
			new TraitVal
			{
				id = "RanchingUp",
				rarity = RARITY_UNCOMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "RanchingDown" }
			},
			new TraitVal
			{
				id = "Loner",
				rarity = RARITY_EPIC,
				dlcId = "EXPANSION1_ID"
			},
			new TraitVal
			{
				id = "StarryEyed",
				rarity = RARITY_RARE,
				dlcId = "EXPANSION1_ID"
			},
			new TraitVal
			{
				id = "GlowStick",
				rarity = RARITY_EPIC,
				dlcId = "EXPANSION1_ID"
			},
			new TraitVal
			{
				id = "RadiationEater",
				rarity = RARITY_EPIC,
				dlcId = "EXPANSION1_ID"
			},
			new TraitVal
			{
				id = "GrantSkill_Mining1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_LEGENDARY,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "CantDig" }
			},
			new TraitVal
			{
				id = "GrantSkill_Mining2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_LEGENDARY,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "CantDig" }
			},
			new TraitVal
			{
				id = "GrantSkill_Mining3",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_LEGENDARY,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "CantDig" }
			},
			new TraitVal
			{
				id = "GrantSkill_Farming2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GrantSkill_Ranching1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GrantSkill_Cooking1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "CantCook" }
			},
			new TraitVal
			{
				id = "GrantSkill_Arting1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Uncultured" }
			},
			new TraitVal
			{
				id = "GrantSkill_Arting2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Uncultured" }
			},
			new TraitVal
			{
				id = "GrantSkill_Arting3",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Uncultured" }
			},
			new TraitVal
			{
				id = "GrantSkill_Suits1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GrantSkill_Technicals2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GrantSkill_Engineering1",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = ""
			},
			new TraitVal
			{
				id = "GrantSkill_Basekeeping2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Anemic" }
			},
			new TraitVal
			{
				id = "GrantSkill_Medicine2",
				statBonus = -LARGE_STATPOINT_BONUS,
				rarity = RARITY_EPIC,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "Hemophobia" }
			}
		};

		public static readonly List<TraitVal> NEEDTRAITS = new List<TraitVal>
		{
			new TraitVal
			{
				id = "Claustrophobic",
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "PrefersWarmer",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "PrefersColder" }
			},
			new TraitVal
			{
				id = "PrefersColder",
				rarity = RARITY_COMMON,
				dlcId = "",
				mutuallyExclusiveTraits = new List<string> { "PrefersWarmer" }
			},
			new TraitVal
			{
				id = "SensitiveFeet",
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Fashionable",
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "Climacophobic",
				rarity = RARITY_COMMON,
				dlcId = ""
			},
			new TraitVal
			{
				id = "SolitarySleeper",
				rarity = RARITY_COMMON,
				dlcId = ""
			}
		};

		public static TraitVal GetTraitVal(string id)
		{
			foreach (TraitVal sPECIALTRAIT in SPECIALTRAITS)
			{
				if (id == sPECIALTRAIT.id)
				{
					return sPECIALTRAIT;
				}
			}
			foreach (TraitVal gOODTRAIT in GOODTRAITS)
			{
				if (id == gOODTRAIT.id)
				{
					return gOODTRAIT;
				}
			}
			foreach (TraitVal bADTRAIT in BADTRAITS)
			{
				if (id == bADTRAIT.id)
				{
					return bADTRAIT;
				}
			}
			foreach (TraitVal cONGENITALTRAIT in CONGENITALTRAITS)
			{
				if (id == cONGENITALTRAIT.id)
				{
					return cONGENITALTRAIT;
				}
			}
			DebugUtil.Assert(test: true, "Could not find TraitVal with ID: " + id);
			return INVALID_TRAIT_VAL;
		}
	}
}
