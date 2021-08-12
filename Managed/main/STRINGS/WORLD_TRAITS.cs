namespace STRINGS
{
	public static class WORLD_TRAITS
	{
		public static class NO_TRAITS
		{
			public static LocString NAME = "<i>This world is stable and has no unusual features.</i>";

			public static LocString DESCRIPTION = "This world exists in a particularly stable configuration each time it is encountered";
		}

		public static class BOULDERS_LARGE
		{
			public static LocString NAME = "Large Boulders";

			public static LocString DESCRIPTION = "Huge boulders make digging through this world more difficult";
		}

		public static class BOULDERS_MEDIUM
		{
			public static LocString NAME = "Medium Boulders";

			public static LocString DESCRIPTION = "Mid-sized boulders make digging through this world more difficult";
		}

		public static class BOULDERS_MIXED
		{
			public static LocString NAME = "Mixed Boulders";

			public static LocString DESCRIPTION = "Boulders of various sizes make digging through this world more difficult";
		}

		public static class BOULDERS_SMALL
		{
			public static LocString NAME = "Small Boulders";

			public static LocString DESCRIPTION = "Tiny boulders make digging through this world more difficult";
		}

		public static class DEEP_OIL
		{
			public static LocString NAME = "Trapped Oil";

			public static LocString DESCRIPTION = string.Concat("Most of the ", UI.PRE_KEYWORD, "Oil", UI.PST_KEYWORD, " in this world will need to be extracted with ", BUILDINGS.PREFABS.OILWELLCAP.NAME, "s");
		}

		public static class FROZEN_CORE
		{
			public static LocString NAME = "Frozen Core";

			public static LocString DESCRIPTION = "This world has a chilly core of solid " + ELEMENTS.ICE.NAME;
		}

		public static class GEOACTIVE
		{
			public static LocString NAME = "Geoactive";

			public static LocString DESCRIPTION = "This world has more " + UI.PRE_KEYWORD + "Geysers" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Vents" + UI.PST_KEYWORD + " than usual";
		}

		public static class GEODES
		{
			public static LocString NAME = "Geodes";

			public static LocString DESCRIPTION = "Large geodes containing rare material caches are deposited across this world";
		}

		public static class GEODORMANT
		{
			public static LocString NAME = "Geodormant";

			public static LocString DESCRIPTION = "This world has fewer " + UI.PRE_KEYWORD + "Geysers" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Vents" + UI.PST_KEYWORD + " than usual";
		}

		public static class GLACIERS_LARGE
		{
			public static LocString NAME = "Large Glaciers";

			public static LocString DESCRIPTION = string.Concat("Huge chunks of primordial ", ELEMENTS.ICE.NAME, " are scattered across this world");
		}

		public static class IRREGULAR_OIL
		{
			public static LocString NAME = "Irregular Oil";

			public static LocString DESCRIPTION = "The " + UI.PRE_KEYWORD + "Oil" + UI.PST_KEYWORD + " on this asteroid is anything but regular!";
		}

		public static class MAGMA_VENTS
		{
			public static LocString NAME = "Magma Channels";

			public static LocString DESCRIPTION = string.Concat("The ", ELEMENTS.MAGMA.NAME, " from this world's core has leaked into the mantle and crust");
		}

		public static class METAL_POOR
		{
			public static LocString NAME = "Metal Poor";

			public static LocString DESCRIPTION = "There is a reduced amount of " + UI.PRE_KEYWORD + "Metal Ore" + UI.PST_KEYWORD + " on this world, proceed with caution!";
		}

		public static class METAL_RICH
		{
			public static LocString NAME = "Metal Rich";

			public static LocString DESCRIPTION = "This asteroid is an abundant source of " + UI.PRE_KEYWORD + "Metal Ore" + UI.PST_KEYWORD;
		}

		public static class MISALIGNED_START
		{
			public static LocString NAME = "Alternate Pod Location";

			public static LocString DESCRIPTION = string.Concat("The ", BUILDINGS.PREFABS.HEADQUARTERSCOMPLETE.NAME, " didn't end up in the asteroid's exact center this time... but it's still nowhere near the surface");
		}

		public static class SLIME_SPLATS
		{
			public static LocString NAME = "Slime Molds";

			public static LocString DESCRIPTION = string.Concat("Sickly ", ELEMENTS.SLIMEMOLD.NAME, " growths have crept all over this world");
		}

		public static class SUBSURFACE_OCEAN
		{
			public static LocString NAME = "Subsurface Ocean";

			public static LocString DESCRIPTION = string.Concat("Below the crust of this world is a ", ELEMENTS.SALTWATER.NAME, " sea");
		}

		public static class VOLCANOES
		{
			public static LocString NAME = "Volcanic Activity";

			public static LocString DESCRIPTION = "Several active " + UI.PRE_KEYWORD + "Volcanoes" + UI.PST_KEYWORD + " have been detected in this world";
		}

		public static LocString MISSING_TRAIT = "<missing trait>";
	}
}
