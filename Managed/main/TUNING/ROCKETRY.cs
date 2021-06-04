using System;
using UnityEngine;

namespace TUNING
{
	public class ROCKETRY
	{
		public class DESTINATION_RESEARCH
		{
			public static int EVERGREEN = 10;

			public static int BASIC = 50;

			public static int HIGH = 150;
		}

		public class DESTINATION_ANALYSIS
		{
			public static int DISCOVERED = 50;

			public static int COMPLETE = 100;

			public static float DEFAULT_CYCLES_PER_DISCOVERY = 0.5f;
		}

		public class DESTINATION_THRUST_COSTS
		{
			public static int LOW = 3;

			public static int MID = 5;

			public static int HIGH = 7;

			public static int VERY_HIGH = 9;
		}

		public class CLUSTER_FOW
		{
			public static float POINTS_TO_REVEAL = 100f;

			public static float DEFAULT_CYCLES_PER_REVEAL = 0.5f;
		}

		public class ENGINE_EFFICIENCY
		{
			public static float WEAK = 20f;

			public static float MEDIUM = 40f;

			public static float STRONG = 60f;

			public static float BOOSTER = 30f;
		}

		public class OXIDIZER_EFFICIENCY
		{
			public static float VERY_LOW = 1f;

			public static float LOW = 2f;

			public static float HIGH = 4f;
		}

		public class CARGO_CONTAINER_MASS
		{
			public static float STATIC_MASS = 1000f;

			public static float PAYLOAD_MASS = 1000f;
		}

		public class BURDEN
		{
			public static int INSIGNIFICANT = 1;

			public static int MINOR = 2;

			public static int MINOR_PLUS = 3;

			public static int MODERATE = 4;

			public static int MODERATE_PLUS = 5;

			public static int MAJOR = 6;

			public static int MAJOR_PLUS = 7;

			public static int MEGA = 9;
		}

		public class ENGINE_POWER
		{
			public static int EARLY_WEAK = 12;

			public static int EARLY_STRONG = 20;

			public static int MID_VERY_STRONG = 42;

			public static int MID_STRONG = 27;

			public static int MID_WEAK = 6;

			public static int LATE_STRONG = 30;

			public static int LATE_VERY_STRONG = 48;
		}

		public class FUEL_COST_PER_DISTANCE
		{
			public static float VERY_LOW = 0.055555556f;

			public static float LOW = 0.0625f;

			public static float MEDIUM = 0.125f;

			public static float HIGH = 355f / (678f * (float)Math.PI);

			public static float VERY_HIGH = 0.25f;

			public static float GAS_LOW = 0.027777778f;

			public static float GAS_HIGH = 0.041666668f;

			public static float PARTICLES = 0.33333334f;
		}

		public static float MISSION_DURATION_SCALE = 1800f;

		public static float MASS_PENALTY_EXPONENT = 3.2f;

		public static float MASS_PENALTY_DIVISOR = 300f;

		public static float CARGO_CAPACITY_SCALE = 10f;

		public static float LIQUID_CARGO_BAY_CLUSTER_CAPACITY = 2700f;

		public static float SOLID_CARGO_BAY_CLUSTER_CAPACITY = 2700f;

		public static float GAS_CARGO_BAY_CLUSTER_CAPACITY = 1100f;

		public static Vector2I ROCKET_INTERIOR_SIZE = new Vector2I(32, 32);

		public static float MassFromPenaltyPercentage(float penaltyPercentage = 0.5f)
		{
			return 0f - 1f / Mathf.Pow(penaltyPercentage - 1f, 5f);
		}

		public static float CalculateMassWithPenalty(float realMass)
		{
			float b = Mathf.Pow(realMass / MASS_PENALTY_DIVISOR, MASS_PENALTY_EXPONENT);
			return Mathf.Max(realMass, b);
		}
	}
}
