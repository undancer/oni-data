namespace STRINGS
{
	public class EQUIPMENT
	{
		public class PREFABS
		{
			public class OXYGEN_MASK
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Mask", "OXYGEN_MASK");

				public static LocString DESC = "Ensures my Duplicants can breathe easy... for a little while, anyways.";

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";

				public static LocString GENERICNAME = "Suit";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Oxygen Mask", "OXYGEN_MASK");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Oxygen Mask", "OXYGEN_MASK") + ".\n\nMasks can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".";
			}

			public class ATMO_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT");

				public static LocString DESC = "Ensures my Duplicants can breathe easy, anytime, anywhere.";

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.\n\nMust be refilled with oxygen at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in toxic and low breathability environments.";

				public static LocString GENERICNAME = "Suit";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Atmo Suit", "ATMO_SUIT");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Atmo Suit", "ATMO_SUIT") + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".";
			}

			public class AQUA_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Aqua Suit", "AQUA_SUIT");

				public static LocString DESC = "Because breathing underwater is better than... not.";

				public static LocString EFFECT = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " at an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with <style=\"oxygen\">Oxygen</style> in underwater environments.";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "AQUA_SUIT");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Aqua Suit", "AQUA_SUIT") + ".\n\nSuits can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".";
			}

			public class TEMPERATURE_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Suit", "TEMPERATURE_SUIT");

				public static LocString DESC = "Keeps my Duplicants cool in case things heat up.";

				public static LocString EFFECT = "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.\n\nMust be powered at a Thermo Suit Dock when depleted.";

				public static LocString RECIPE_DESC = "Provides insulation in regions with extreme <style=\"heat\">Temperatures</style>.";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "TEMPERATURE_SUIT");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Thermo Suit", "TEMPERATURE_SUIT") + ".\n\nSuits can be repaired at a " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE") + ".";
			}

			public class JET_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit", "JET_SUIT");

				public static LocString DESC = "Allows my Duplicants to take to the skies, for a time.";

				public static LocString EFFECT = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " at a " + UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nAllows Duplicant flight.";

				public static LocString GENERICNAME = "Jet Suit";

				public static LocString TANK_EFFECT_NAME = "Fuel Tank";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Jet Suit", "JET_SUIT");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Jet Suit", "JET_SUIT") + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".";
			}

			public class LEAD_SUIT
			{
				public static LocString NAME = UI.FormatAsLink("Lead Suit", "LEAD_SUIT");

				public static LocString DESC = "Because exposure to radiation doesn't grant Duplicants superpowers.";

				public static LocString EFFECT = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and protection in areas with " + UI.FormatAsLink("Radiation", "RADIATION") + ".\n\nMust be refilled with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " at a " + UI.FormatAsLink("Lead Suit Dock", "LEADSUITLOCKER") + " when depleted.";

				public static LocString RECIPE_DESC = "Supplies Duplicants with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " in toxic and low breathability environments.\n\nProtects Duplicants from " + UI.FormatAsLink("Radiation", "RADIATION") + ".";

				public static LocString GENERICNAME = "Lead Suit";

				public static LocString BATTERY_EFFECT_NAME = "Suit Battery";

				public static LocString SUIT_OUT_OF_BATTERIES = "Suit Batteries Empty";

				public static LocString WORN_NAME = UI.FormatAsLink("Worn Lead Suit", "LEAD_SUIT");

				public static LocString WORN_DESC = "A worn out " + UI.FormatAsLink("Lead Suit", "LEAD_SUIT") + ".\n\nSuits can be repaired at an " + UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR") + ".";
			}

			public class COOL_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Cool Vest", "COOL_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Don't sweat it!";

				public static LocString EFFECT = "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation.";

				public static LocString RECIPE_DESC = "Protects the wearer from <style=\"heat\">Heat</style> by decreasing insulation";
			}

			public class WARM_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Warm Sweater", "WARM_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "Happiness is a warm Duplicant.";

				public static LocString EFFECT = "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation.";

				public static LocString RECIPE_DESC = "Protects the wearer from <style=\"heat\">Cold</style> by increasing insulation";
			}

			public class FUNKY_VEST
			{
				public static LocString NAME = UI.FormatAsLink("Snazzy Suit", "FUNKY_VEST");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "This transforms my Duplicant into a walking beacon of charm and style.";

				public static LocString EFFECT = "Increases Decor in a small area effect around the wearer. Can be upgraded to " + UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING") + " at the " + UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION") + ".";

				public static LocString RECIPE_DESC = "Increases Decor in a small area effect around the wearer. Can be upgraded to " + UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING") + " at the " + UI.FormatAsLink("Clothing Refashionator", "CLOTHINGALTERATIONSTATION");
			}

			public class CUSTOMCLOTHING
			{
				public class FACADES
				{
					public static LocString DECOR_02 = UI.FormatAsLink("Snazzier Red Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_03 = UI.FormatAsLink("Snazzier Blue Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_04 = UI.FormatAsLink("Snazzier Green Suit", "CUSTOMCLOTHING");

					public static LocString DECOR_05 = UI.FormatAsLink("Snazzier Violet Suit", "CUSTOMCLOTHING");

					public static LocString VELOUR01 = UI.FormatAsLink("Midnight Velour", "CUSTOMCLOTHING");

					public static LocString VELOUR02 = UI.FormatAsLink("Blue Velour", "CUSTOMCLOTHING");

					public static LocString VELOUR03 = UI.FormatAsLink("Bubblegum Velour", "CUSTOMCLOTHING");

					public static LocString CUMMERBUND = UI.FormatAsLink("Classic Cummerbund", "CUSTOMCLOTHING");

					public static LocString SMOKINGJACKET = UI.FormatAsLink("Smoking Jacket", "CUSTOMCLOTHING");

					public static LocString PINSTRIPEWAISTCOAT = UI.FormatAsLink("Pinstripe Waistcoat", "CUSTOMCLOTHING");

					public static LocString OVERALLS = UI.FormatAsLink("Spiffy Overalls", "CUSTOMCLOTHING");

					public static LocString TRIANGLES = UI.FormatAsLink("Confetti Suit", "CUSTOMCLOTHING");

					public static LocString LIMONE = UI.FormatAsLink("Citrus Spandex", "CUSTOMCLOTHING");

					public static LocString GAUDYSWEATER = UI.FormatAsLink("Pompom Knit", "CUSTOMCLOTHING");

					public static LocString MONDRIAN = UI.FormatAsLink("Cubist Knit", "CUSTOMCLOTHING");

					public static LocString WORKOUT = UI.FormatAsLink("Pink Unitard", "CUSTOMCLOTHING");

					public static LocString CLUBSHIRT = UI.FormatAsLink("Purple Polyester", "CUSTOMCLOTHING");
				}

				public static LocString NAME = UI.FormatAsLink("Primo Garb", "CUSTOMCLOTHING");

				public static LocString GENERICNAME = "Clothing";

				public static LocString DESC = "This transforms my Duplicant into a colony-inspiring fashion icon.";

				public static LocString EFFECT = "Increases Decor in a small area effect around the wearer.";

				public static LocString RECIPE_DESC = "Increases Decor in a small area effect around the wearer";
			}

			public class OXYGEN_TANK
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Tank", "OXYGEN_TANK");

				public static LocString GENERICNAME = "Equipment";

				public static LocString DESC = "It's like a to-go bag for your lungs.";

				public static LocString EFFECT = "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>.";

				public static LocString RECIPE_DESC = "Allows Duplicants to breathe in hazardous environments.\n\nDoes not work when submerged in <style=\"liquid\">Liquid</style>";
			}

			public class OXYGEN_TANK_UNDERWATER
			{
				public static LocString NAME = "Oxygen Rebreather";

				public static LocString GENERICNAME = "Equipment";

				public static LocString DESC = "";

				public static LocString EFFECT = "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid.";

				public static LocString RECIPE_DESC = "Allows Duplicants to breathe while submerged in <style=\"liquid\">Liquid</style>.\n\nDoes not work outside of liquid";
			}

			public class EQUIPPABLEBALLOON
			{
				public static LocString NAME = UI.FormatAsLink("Balloon Friend", "EQUIPPABLEBALLOON");

				public static LocString DESC = "A floating friend to reassure my Duplicants they are so very, very clever.";

				public static LocString EFFECT = "Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response.";

				public static LocString RECIPE_DESC = "Gives Duplicants a boost in brain function.\n\nSupplied by Duplicants with the Balloon Artist " + UI.FormatAsLink("Overjoyed", "MORALE") + " response";

				public static LocString GENERICNAME = "Balloon Friend";
			}
		}
	}
}
