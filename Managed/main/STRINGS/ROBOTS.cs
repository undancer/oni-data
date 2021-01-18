namespace STRINGS
{
	public class ROBOTS
	{
		public class STATS
		{
			public class INTERNALBATTERY
			{
				public static LocString NAME = "Rechargeable Battery";

				public static LocString TOOLTIP = "When this bot's battery runs out it must temporarily stop working to go recharge";
			}

			public class INTERNALCHEMICALBATTERY
			{
				public static LocString NAME = "Chemical Battery";

				public static LocString TOOLTIP = "This bot will shut down permanently when its battery runs out";
			}
		}

		public class ATTRIBUTES
		{
			public class INTERNALBATTERYDELTA
			{
				public static LocString NAME = "Rechargeable Battery Drain";

				public static LocString TOOLTIP = "The rate at which battery life is depleted";
			}
		}

		public class STATUSITEMS
		{
			public class CANTREACHSTATION
			{
				public static LocString NAME = "Unreachable Dock";

				public static LocString DESC = "Obstacles are preventing {0} from heading home";

				public static LocString TOOLTIP = "Obstacles are preventing {0} from heading home";
			}

			public class MOVINGTOCHARGESTATION
			{
				public static LocString NAME = "Traveling to Dock";

				public static LocString DESC = "{0} is on its way home to recharge";

				public static LocString TOOLTIP = "{0} is on its way home to recharge";
			}

			public class LOWBATTERY
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "{0}'s battery is low and needs to recharge";

				public static LocString TOOLTIP = "{0}'s battery is low and needs to recharge";
			}

			public class LOWBATTERYNOCHARGE
			{
				public static LocString NAME = "Low Battery";

				public static LocString DESC = "{0}'s battery is low\n\nThe internal battery can not be recharged and this robot will cease functioning after it is depleted.";

				public static LocString TOOLTIP = "{0}'s battery is low\n\nThe internal battery can not be recharged and this robot will cease functioning after it is depleted.";
			}

			public class DEADBATTERY
			{
				public static LocString NAME = "Shut Down";

				public static LocString DESC = "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";

				public static LocString TOOLTIP = "RIP {0}\n\n{0}'s battery has been depleted and cannot be recharged";
			}

			public class DUSTBINFULL
			{
				public static LocString NAME = "Dust Bin Full";

				public static LocString DESC = "{0} must return to its dock to unload";

				public static LocString TOOLTIP = "{0} must return to its dock to unload";
			}

			public class WORKING
			{
				public static LocString NAME = "Working";

				public static LocString DESC = "{0} is working diligently. Great job, {0}!";

				public static LocString TOOLTIP = "{0} is working diligently. Great job, {0}!";
			}

			public class UNLOADINGSTORAGE
			{
				public static LocString NAME = "Unloading";

				public static LocString DESC = "{0} is emptying out its dust bin";

				public static LocString TOOLTIP = "{0} is emptying out its dust bin";
			}

			public class CHARGING
			{
				public static LocString NAME = "Charging";

				public static LocString DESC = "{0} is recharging its battery";

				public static LocString TOOLTIP = "{0} is recharging its battery";
			}

			public class REACTPOSITIVE
			{
				public static LocString NAME = "Happy Reaction";

				public static LocString DESC = "This bot saw something nice!";

				public static LocString TOOLTIP = "This bot saw something nice!";
			}

			public class REACTNEGATIVE
			{
				public static LocString NAME = "Bothered Reaction";

				public static LocString DESC = "This bot saw something upsetting";

				public static LocString TOOLTIP = "This bot saw something upsetting";
			}
		}

		public class MODELS
		{
			public class SCOUT
			{
				public static LocString NAME = "Rover";

				public static LocString DESC = string.Concat("A curious bot that can remotely explore new ", UI.CLUSTERMAP.PLANETOID_KEYWORD, " locations.");
			}

			public class SWEEPBOT
			{
				public static LocString NAME = "Sweepy";

				public static LocString DESC = "An automated sweeping robot.\n\nSweeps up " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " debris and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " spills and stores the material back in its " + UI.FormatAsLink("Sweepy Dock", "SWEEPBOTSTATION") + ".";
			}
		}

		public static LocString CATEGORY_NAME = "Robots";
	}
}
