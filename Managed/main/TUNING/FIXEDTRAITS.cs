namespace TUNING
{
	public class FIXEDTRAITS
	{
		public class SUNLIGHT
		{
			public class NAME
			{
				public static string NONE = "sunlightNone";

				public static string VERY_VERY_LOW = "sunlightVeryVeryLow";

				public static string VERY_LOW = "sunlightVeryLow";

				public static string LOW = "sunlightLow";

				public static string MED_LOW = "sunlightMedLow";

				public static string MED = "sunlightMed";

				public static string MED_HIGH = "sunlightMedHigh";

				public static string HIGH = "sunlightHigh";

				public static string VERY_HIGH = "sunlightVeryHigh";

				public static string VERY_VERY_HIGH = "sunlightVeryVeryHigh";

				public static string VERY_VERY_VERY_HIGH = "sunlightVeryVeryVeryHigh";

				public static string DEFAULT = VERY_HIGH;
			}

			public static int DEFAULT_SPACED_OUT_SUNLIGHT = 40000;

			public static int NONE = 0;

			public static int VERY_VERY_LOW = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 0.25f);

			public static int VERY_LOW = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 0.5f);

			public static int LOW = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 0.75f);

			public static int MED_LOW = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 0.875f);

			public static int MED = DEFAULT_SPACED_OUT_SUNLIGHT;

			public static int MED_HIGH = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 1.25f);

			public static int HIGH = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 1.5f);

			public static int VERY_HIGH = DEFAULT_SPACED_OUT_SUNLIGHT * 2;

			public static int VERY_VERY_HIGH = (int)((float)DEFAULT_SPACED_OUT_SUNLIGHT * 2.5f);

			public static int VERY_VERY_VERY_HIGH = DEFAULT_SPACED_OUT_SUNLIGHT * 3;

			public static int DEFAULT_VALUE = VERY_HIGH;
		}

		public class COSMICRADIATION
		{
			public class NAME
			{
				public static string NONE = "cosmicRadiationNone";

				public static string VERY_VERY_LOW = "cosmicRadiationVeryVeryLow";

				public static string VERY_LOW = "cosmicRadiationVeryLow";

				public static string LOW = "cosmicRadiationLow";

				public static string MED_LOW = "cosmicRadiationMedLow";

				public static string MED = "cosmicRadiationMed";

				public static string MED_HIGH = "cosmicRadiationMedHigh";

				public static string HIGH = "cosmicRadiationHigh";

				public static string VERY_HIGH = "cosmicRadiationVeryHigh";

				public static string VERY_VERY_HIGH = "cosmicRadiationVeryVeryHigh";

				public static string DEFAULT = MED;
			}

			public static int BASELINE = 25;

			public static int NONE = 0;

			public static int VERY_VERY_LOW = (int)((float)BASELINE * 0.25f);

			public static int VERY_LOW = (int)((float)BASELINE * 0.5f);

			public static int LOW = (int)((float)BASELINE * 0.75f);

			public static int MED_LOW = (int)((float)BASELINE * 0.875f);

			public static int MED = BASELINE;

			public static int MED_HIGH = (int)((float)BASELINE * 1.25f);

			public static int HIGH = (int)((float)BASELINE * 1.5f);

			public static int VERY_HIGH = BASELINE * 2;

			public static int VERY_VERY_HIGH = BASELINE * 3;

			public static int DEFAULT_VALUE = MED;
		}
	}
}
