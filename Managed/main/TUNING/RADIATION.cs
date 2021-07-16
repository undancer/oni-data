namespace TUNING
{
	public class RADIATION
	{
		public class STANDARD_EMITTER
		{
			public const float STEADY_PULSE_RATE = 0.2f;

			public const float DOUBLE_SPEED_PULSE_RATE = 0.1f;

			public const float RADIUS_SCALE = 1f;
		}

		public class RADIATION_PER_SECOND
		{
			public const float TRIVIAL = 6f;

			public const float VERY_LOW = 12f;

			public const float LOW = 24f;

			public const float MODERATE = 60f;

			public const float HIGH = 180f;

			public const float VERY_HIGH = 420f;

			public const int EXTREME = 840;
		}

		public class RADIATION_CONSTANT_RADS_PER_CYCLE
		{
			public const float TRIVIAL = 12f;

			public const float VERY_LOW = 24f;

			public const float LOW = 48f;

			public const float MODERATE = 120f;

			public const float MODERATE_PLUS = 240f;

			public const float HIGH = 360f;

			public const float VERY_HIGH = 840f;

			public const int EXTREME = 1680;
		}

		public const float GERM_RAD_SCALE = 0.001f;

		public const float STANDARD_DAILY_RECOVERY = 60f;

		public const float EXTRA_VOMIT_RECOVERY = 20f;
	}
}
