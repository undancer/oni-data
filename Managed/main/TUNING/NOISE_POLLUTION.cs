namespace TUNING
{
	public class NOISE_POLLUTION
	{
		public class LENGTHS
		{
			public static float VERYSHORT = 0.25f;

			public static float SHORT = 0.5f;

			public static float NORMAL = 1f;

			public static float LONG = 1.5f;

			public static float VERYLONG = 2f;
		}

		public class NOISY
		{
			public static readonly EffectorValues TIER0;

			public static readonly EffectorValues TIER1;

			public static readonly EffectorValues TIER2;

			public static readonly EffectorValues TIER3;

			public static readonly EffectorValues TIER4;

			public static readonly EffectorValues TIER5;

			public static readonly EffectorValues TIER6;

			static NOISY()
			{
				EffectorValues effectorValues = new EffectorValues
				{
					amount = 45,
					radius = 10
				};
				TIER0 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 55,
					radius = 10
				};
				TIER1 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 65,
					radius = 10
				};
				TIER2 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 75,
					radius = 15
				};
				TIER3 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 90,
					radius = 15
				};
				TIER4 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 105,
					radius = 20
				};
				TIER5 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 125,
					radius = 20
				};
				TIER6 = effectorValues;
			}
		}

		public class CREATURES
		{
			public static readonly EffectorValues TIER0;

			public static readonly EffectorValues TIER1;

			public static readonly EffectorValues TIER2;

			public static readonly EffectorValues TIER3;

			public static readonly EffectorValues TIER4;

			public static readonly EffectorValues TIER5;

			public static readonly EffectorValues TIER6;

			public static readonly EffectorValues TIER7;

			static CREATURES()
			{
				EffectorValues effectorValues = new EffectorValues
				{
					amount = 30,
					radius = 5
				};
				TIER0 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 35,
					radius = 5
				};
				TIER1 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 45,
					radius = 5
				};
				TIER2 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 55,
					radius = 5
				};
				TIER3 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 65,
					radius = 5
				};
				TIER4 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 75,
					radius = 5
				};
				TIER5 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 90,
					radius = 10
				};
				TIER6 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 105,
					radius = 10
				};
				TIER7 = effectorValues;
			}
		}

		public class DAMPEN
		{
			public static readonly EffectorValues TIER0;

			public static readonly EffectorValues TIER1;

			public static readonly EffectorValues TIER2;

			public static readonly EffectorValues TIER3;

			public static readonly EffectorValues TIER4;

			public static readonly EffectorValues TIER5;

			static DAMPEN()
			{
				EffectorValues effectorValues = new EffectorValues
				{
					amount = -5,
					radius = 1
				};
				TIER0 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -10,
					radius = 2
				};
				TIER1 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -15,
					radius = 3
				};
				TIER2 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -20,
					radius = 4
				};
				TIER3 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -20,
					radius = 5
				};
				TIER4 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -25,
					radius = 6
				};
				TIER5 = effectorValues;
			}
		}

		public static readonly EffectorValues NONE;

		public static readonly EffectorValues CONE_OF_SILENCE;

		public static float DUPLICANT_TIME_THRESHOLD;

		static NOISE_POLLUTION()
		{
			EffectorValues effectorValues = new EffectorValues
			{
				amount = 0,
				radius = 0
			};
			NONE = effectorValues;
			effectorValues = new EffectorValues
			{
				amount = -120,
				radius = 5
			};
			CONE_OF_SILENCE = effectorValues;
			DUPLICANT_TIME_THRESHOLD = 3f;
		}
	}
}
