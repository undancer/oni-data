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

		public class ENGINE_EFFICIENCY
		{
			public static float WEAK = 20f;

			public static float MEDIUM = 40f;

			public static float STRONG = 60f;

			public static float BOOSTER = 30f;
		}

		public class OXIDIZER_EFFICIENCY
		{
			public static float LOW = 1f;

			public static float HIGH = 1.33f;
		}

		public class CARGO_CONTAINER_MASS
		{
			public static float STATIC_MASS = 1000f;

			public static float PAYLOAD_MASS = 1000f;
		}

		public static float MISSION_DURATION_SCALE = 1800f;

		public static float MASS_PENALTY_EXPONENT = 3.2f;

		public static float MASS_PENALTY_DIVISOR = 300f;

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
