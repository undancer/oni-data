using System;
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

			public const float BLADDER_INCREASE_PER_SECOND = 355f / (678f * (float)Math.PI);

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

				public const float HARD = 0.008333334f;

				public const float VERYHARD = 0.016666668f;
			}

			public class MAX_STRESS
			{
				public const float EASY = 0.016666668f;

				public const float NEUTRAL = 0.041666668f;

				public const float HARD = 0.05f;

				public const float VERYHARD = 0.083333336f;
			}

			public const float ABOVE_EXPECTATIONS = -0.016666668f;

			public const float AT_EXPECTATIONS = -0.008333334f;

			public const float MIN_STRESS = -71f / (678f * (float)Math.PI);
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
				new int[7]
				{
					5,
					4,
					4,
					3,
					3,
					2,
					1
				},
				new int[4]
				{
					5,
					3,
					2,
					1
				},
				new int[4]
				{
					5,
					2,
					2,
					1
				},
				new int[2]
				{
					5,
					1
				},
				new int[3]
				{
					5,
					3,
					1
				},
				new int[5]
				{
					3,
					3,
					3,
					3,
					1
				},
				new int[1]
				{
					4
				},
				new int[1]
				{
					3
				},
				new int[1]
				{
					2
				},
				new int[1]
				{
					1
				}
			};

			public static int[] GetRandomDistribution()
			{
				return TYPES[UnityEngine.Random.Range(0, TYPES.Count)];
			}
		}

		public struct TraitVal
		{
			public string id;

			public int statBonus;

			public float probability;

			public List<string> mutuallyExclusiveTraits;

			public List<HashedString> requiredNonPositiveAptitudes;
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

		public const float STANDARD_STRESS_PENALTY = 0.016666668f;

		public const float STANDARD_STRESS_BONUS = -71f / (678f * (float)Math.PI);

		public const float RANCHING_DURATION_MULTIPLIER_BONUS_PER_POINT = 0.1f;

		public const float STRESS_BELOW_EXPECTATIONS_FOOD = 0.25f;

		public const float STRESS_ABOVE_EXPECTATIONS_FOOD = -0.5f;

		public const float STANDARD_STRESS_PENALTY_SECOND = 0.25f;

		public const float STANDARD_STRESS_BONUS_SECOND = -0.5f;

		public const float RECOVER_BREATH_DELTA = 3f;

		public const float TRAVEL_TIME_WARNING_THRESHOLD = 0.4f;

		public static readonly string[] ALL_ATTRIBUTES = new string[11]
		{
			"Strength",
			"Caring",
			"Construction",
			"Digging",
			"Machinery",
			"Learning",
			"Cooking",
			"Botanist",
			"Art",
			"Ranching",
			"Athletics"
		};

		public static readonly string[] DISTRIBUTED_ATTRIBUTES = new string[10]
		{
			"Strength",
			"Caring",
			"Construction",
			"Digging",
			"Machinery",
			"Learning",
			"Cooking",
			"Botanist",
			"Art",
			"Ranching"
		};

		public static readonly string[] ROLLED_ATTRIBUTES = new string[1]
		{
			"Athletics"
		};

		public static readonly int[] APTITUDE_ATTRIBUTE_BONUSES = new int[3]
		{
			7,
			3,
			1
		};

		public static int ROLLED_ATTRIBUTE_MAX = 5;

		public static float ROLLED_ATTRIBUTE_POWER = 4f;

		public static float PROBABILITY_MINISCULE = 2f;

		public static float PROBABILITY_LOW = 1.5f;

		public static float PROBABILITY_MED = 1f;

		public static int TINY_STATPOINT_BONUS = 1;

		public static int SMALL_STATPOINT_BONUS = 2;

		public static int MEDIUM_STATPOINT_BONUS = 3;

		public static int MIN_STAT_POINTS = 0;

		public static int MAX_STAT_POINTS = 0;

		public static int MAX_TRAITS = 4;

		public static int APTITUDE_BONUS = 1;

		public static readonly List<string> CONTRACTEDTRAITS_HEALING = new List<string>
		{
			"IrritableBowel",
			"Aggressive",
			"SlowLearner",
			"WeakImmuneSystem",
			"Snorer",
			"CantDig"
		};

		public static readonly List<TraitVal> CONGENITALTRAITS;

		public static readonly List<TraitVal> BADTRAITS;

		public static readonly List<TraitVal> STRESSTRAITS;

		public static readonly List<TraitVal> JOYTRAITS;

		public static readonly List<TraitVal> GENESHUFFLERTRAITS;

		public static readonly List<TraitVal> GOODTRAITS;

		public static readonly List<TraitVal> NEEDTRAITS;

		static DUPLICANTSTATS()
		{
			List<TraitVal> list = new List<TraitVal>();
			TraitVal item = new TraitVal
			{
				id = "None"
			};
			list.Add(item);
			item = new TraitVal
			{
				id = "Joshua",
				mutuallyExclusiveTraits = new List<string>
				{
					"ScaredyCat",
					"Aggressive"
				}
			};
			list.Add(item);
			item = new TraitVal
			{
				id = "Ellie",
				statBonus = TINY_STATPOINT_BONUS,
				mutuallyExclusiveTraits = new List<string>
				{
					"InteriorDecorator",
					"MouthBreather",
					"Uncultured"
				}
			};
			list.Add(item);
			item = new TraitVal
			{
				id = "Stinky",
				mutuallyExclusiveTraits = new List<string>
				{
					"Flatulence",
					"InteriorDecorator"
				}
			};
			list.Add(item);
			item = new TraitVal
			{
				id = "Liam",
				mutuallyExclusiveTraits = new List<string>
				{
					"Flatulence",
					"InteriorDecorator"
				}
			};
			list.Add(item);
			CONGENITALTRAITS = list;
			List<TraitVal> list2 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "CantResearch",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Research"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "CantDig",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Mining"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "CantCook",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Cooking"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "CantBuild",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Building"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Hemophobia",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"MedicalAid"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Narcolepsy",
				statBonus = MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_LOW
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Flatulence",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "IrritableBowel",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Snorer",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "MouthBreather",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "SmallBladder",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "CalorieBurner",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Anemic",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "WeakImmuneSystem",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "SlowLearner",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Research"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "NoodleArms",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "ScaredyCat",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Mining"
				}
			};
			list2.Add(item);
			item = new TraitVal
			{
				id = "Allergies",
				statBonus = SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list2.Add(item);
			BADTRAITS = list2;
			List<TraitVal> list3 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "Aggressive"
			};
			list3.Add(item);
			item = new TraitVal
			{
				id = "StressVomiter"
			};
			list3.Add(item);
			item = new TraitVal
			{
				id = "UglyCrier"
			};
			list3.Add(item);
			item = new TraitVal
			{
				id = "BingeEater"
			};
			list3.Add(item);
			STRESSTRAITS = list3;
			List<TraitVal> list4 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "BalloonArtist"
			};
			list4.Add(item);
			item = new TraitVal
			{
				id = "SparkleStreaker"
			};
			list4.Add(item);
			item = new TraitVal
			{
				id = "StickerBomber"
			};
			list4.Add(item);
			item = new TraitVal
			{
				id = "SuperProductive"
			};
			list4.Add(item);
			JOYTRAITS = list4;
			List<TraitVal> list5 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "Regeneration"
			};
			list5.Add(item);
			item = new TraitVal
			{
				id = "DeeperDiversLungs"
			};
			list5.Add(item);
			item = new TraitVal
			{
				id = "SunnyDisposition"
			};
			list5.Add(item);
			item = new TraitVal
			{
				id = "RockCrusher"
			};
			list5.Add(item);
			GENESHUFFLERTRAITS = list5;
			List<TraitVal> list6 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "Twinkletoes",
				statBonus = -MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"Anemic"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "StrongArm",
				statBonus = -MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"NoodleArms"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "Greasemonkey",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "DiversLung",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"MouthBreather"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "IronGut",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "StrongImmuneSystem",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"WeakImmuneSystem"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "EarlyBird",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"NightOwl"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "NightOwl",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"EarlyBird"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "MoleHands",
				statBonus = -MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"CantDig"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "FastLearner",
				statBonus = -MEDIUM_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"SlowLearner",
					"CantResearch"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "InteriorDecorator",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"Uncultured"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "Uncultured",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"InteriorDecorator"
				},
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Art"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "SimpleTastes",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"Foodie"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "Foodie",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"SimpleTastes",
					"CantCook"
				},
				requiredNonPositiveAptitudes = new List<HashedString>
				{
					"Cooking"
				}
			};
			list6.Add(item);
			item = new TraitVal
			{
				id = "BedsideManner",
				statBonus = -SMALL_STATPOINT_BONUS,
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"Hemophobia"
				}
			};
			list6.Add(item);
			GOODTRAITS = list6;
			List<TraitVal> list7 = new List<TraitVal>();
			item = new TraitVal
			{
				id = "Claustrophobic",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "PrefersWarmer",
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"PrefersColder"
				}
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "PrefersColder",
				probability = PROBABILITY_MED,
				mutuallyExclusiveTraits = new List<string>
				{
					"PrefersWarmer"
				}
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "SensitiveFeet",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "Fashionable",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "Climacophobic",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "SolitarySleeper",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			item = new TraitVal
			{
				id = "Workaholic",
				probability = PROBABILITY_MED
			};
			list7.Add(item);
			NEEDTRAITS = list7;
		}
	}
}
