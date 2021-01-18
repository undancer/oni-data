namespace STRINGS
{
	public class ROBOTS
	{
		public class STATUSITEMS
		{
			public class CANTREACHSTATION
			{
				public static LocString NAME = "Can't Reach Sweepy Dock";

				public static LocString DESC = "Something is blocking this robot from reaching its base station";

				public static LocString TOOLTIP = "Something is blocking this robot from reaching its base station";
			}

			public class MOVINGTOCHARGESTATION
			{
				public static LocString NAME = "Travelling to Sweepy Dock";

				public static LocString DESC = "This robot is on its way to a battery recharge";

				public static LocString TOOLTIP = "This robot is on its way to a battery recharge";
			}

			public class LOWBATTERY
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "This robot's battery is low and needs to recharge";

				public static LocString TOOLTIP = "This robot's battery is low and needs to recharge";
			}

			public class DUSTBINFULL
			{
				public static LocString NAME = "Dust Bin Full";

				public static LocString DESC = "This robot must return to its base station to unload";

				public static LocString TOOLTIP = "This robot must return to its base station to unload";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString DESC = "This robot is working diligently";

				public static LocString TOOLTIP = "This robot is working diligently";
			}

			public class UNLOADINGSTORAGE
			{
				public static LocString NAME = "Unloading";

				public static LocString DESC = "This robot is unloading its storage";

				public static LocString TOOLTIP = "This robot unloading its storage";
			}

			public class CHARGING
			{
				public static LocString NAME = "Charging";

				public static LocString DESC = "This robot is charging its battery";

				public static LocString TOOLTIP = "This robot is charging its battery";
			}

			public class REACTPOSITIVE
			{
				public static LocString NAME = "Positive Reaction";

				public static LocString DESC = "This robot is reacting positively to something";

				public static LocString TOOLTIP = "This robot is reacting positively to something";
			}

			public class REACTNEGATIVE
			{
				public static LocString NAME = "Negative Reaction";

				public static LocString DESC = "This robot is reacting negatively to something";

				public static LocString TOOLTIP = "This robot is reacting negatively to something";
			}
		}

		public class MODELS
		{
			public class SWEEPBOT
			{
				public static LocString NAME = "Sweepy";

				public static LocString DESC = "An automated sweeping robot.\n\nPicks up both " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " debris and stores it in a " + UI.FormatAsLink("Sweepy Dock", "SWEEPBOTSTATION") + ".";
			}
		}

		public static LocString CATEGORY_NAME = "Robots";
	}
}
