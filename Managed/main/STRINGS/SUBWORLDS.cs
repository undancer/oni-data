namespace STRINGS
{
	public class SUBWORLDS
	{
		public static class BARREN
		{
			public static LocString NAME = "Barren";

			public static LocString DESC = "Initial scans of this biome yield no signs of either " + UI.FormatAsLink("plant life", "PLANTS") + " or " + UI.FormatAsLink("critters", "CREATURES") + ". It is a land devoid of " + UI.FormatAsLink("liquids", "ELEMENTS_LIQUID") + " and minuscule " + UI.FormatAsLink("gas", "ELEMENTS_GAS") + " deposits. These dry, dusty plains can be mined for building materials but there is little in the way of life sustaining resources here for a colony.";

			public static LocString UTILITY = "The layers of sedimentary rock are predominantly made up of " + UI.FormatAsLink("Granite", "GRANITE") + " deposits, although " + UI.FormatAsLink("Obsidian", "OBSIDIAN") + " and " + UI.FormatAsLink("Igneous Rock", "IGNEOUSROCK") + " are also present. This suggests a history of volcanic activity, though no volcanoes exist here currently.\n\nVeins of " + UI.FormatAsLink("Iron Ore", "IRON") + " deposits are evident from initial scans, as are deposits of " + UI.FormatAsLink("Coal", "CARBON") + ". Both are useful in setting up a colony's power infrastructure.\n\nThough it lacks the crucial resources necessary to sustain a colony, there is nothing inherently dangerous here to harm my Duplicants. It should be safe enough to explore.";
		}

		public static class FOREST
		{
			public static LocString NAME = "Forest";

			public static LocString DESC = "Temperate and filled with unique " + UI.FormatAsLink("plant life", "PLANTS") + ", this biome contains all the necessities for life support, although not in quantities sufficient to sustain a long term colony. Exploration into neighboring biomes should be a priority.";

			public static LocString UTILITY = "Small pockets of " + UI.FormatAsLink("Oxylite", "OXYROCK") + " and " + UI.FormatAsLink("Water", "WATER") + " are present in the Forest Biome, but calculations reveal they will only sustain the colony for a limited time.\n\nAnalysis shows two native plants which should be prioritized for cultivation: The " + UI.FormatAsLink("Oxyfern", "OXYFERN") + ", which releases " + UI.FormatAsLink("Oxygen", "OXYGEN") + " but requires " + UI.FormatAsLink("Water", "WATER") + "; and the " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " which provides lumber as a fuel source.\n\nA symbiotic relationship exists with the " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + " and the native " + UI.FormatAsLink("Pips", "SQUIRRELSPECIES") + " which appear to be the only critter that can find the elusive " + UI.FormatAsLink("Arbor Acorns", "PLANTS") + ".\n\nThis biome is really quite beautiful. I've noted that " + UI.FormatAsLink("Shine Bugs", "LIGHTBUGSPECIES") + " and " + UI.FormatAsLink("Mirth Leaf", "LEAFYPLANT") + " both evoke feelings of serenity in my Duplicants.";
		}

		public static class FROZEN
		{
			public static LocString NAME = "Tundra";

			public static LocString DESC = "The sub-zero temperatures of the Tundra Biome provide rare frozen deposits of " + UI.FormatAsLink("Ice", "ICE") + " and " + UI.FormatAsLink("Snow", "SNOW") + ", necessary for a colony's " + UI.FormatAsLink("Heat", "HEAT") + " regulation needs.";

			public static LocString UTILITY = "Far from devoid of life, this biome contains some much needed plant life, ripe for cultivation. " + UI.FormatAsLink("Sleet Wheat", "COLDWHEAT") + " provides a nutrient rich ingredient for creating complex foods, though the plants do require sub-zero temperatures to thrive.\n\nFortunately " + UI.FormatAsLink("Wheezewort", "COLDBREATHER") + " can been planted on farms to lower surrounding temperatures.\n\nCrucially, small deposits of " + UI.FormatAsLink("Wolframite", "WOLFRAMITE") + " have been detected here. This is an extremely rare metal that should be preserved for " + UI.FormatAsLink("Tungsten", "TUNGSTEN") + "production.\n\nThough my Duplicants appear more than happy to work in the Tundra Biome for short periods of time, I will need to provide proper " + UI.FormatAsLink("equipment", "EQUIPMENT") + " for them to avoid adverse affects to their wellbeing if they are working here for longer periods.";
		}

		public static class JUNGLE
		{
			public static LocString NAME = "Jungle";

			public static LocString DESC = "Initial investigations of the Jungle Biome reveal an ecosystem filled with unique flora but centered around " + UI.FormatAsLink("Chlorine", "CHLORINE") + " and " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " gas, toxic to Duplicants. When exploring here, it is worth setting up a good  system.";

			public static LocString UTILITY = "The " + UI.FormatAsLink("Drecko", "DRECKOSPECIES") + " is a relatively benign critter which can be domesticated to aid in textile and food production.\n\nThe " + UI.FormatAsLink("Morb", "GLOMSPECIES") + " only function is to produce " + UI.FormatAsLink("Polluted Oxygen", "POLLUTEDOXYGEN") + " which may be useful when establishing sustainable production loops with other critters.\n\n" + UI.FormatAsLink("Balm Lilies", "SWAMPLILY") + " would be useful to cultivate for the production of critical medical materials.\n\n" + UI.FormatAsLink("Pincha Pepperplants", "SPICE_VINE") + " greatly improve the nutritional and gratification of a colony's food supply. Because of their unique relationship with gravity, the plants must be orientated upside-down for proper growing. This can be accomplished by using a " + UI.FormatAsLink("Farm Tile", "FARMTILE") + ".\n\nGiven the toxic gases I will have to build proper " + UI.FormatAsLink("Ventilation", "BUILDCATEGORYHVAC") + " system to both protect my Duplicants and provide the optimum environments for the native plants and critters.";
		}

		public static class MAGMA
		{
			public static LocString NAME = "Magma";

			public static LocString DESC = "Bring on the heat! With temperatures that can reach upwards of 1526 degrees the Magma Biome is a colony hotspot, although you may want to think twice about sending your Duplicants there without protection.";

			public static LocString UTILITY = UI.FormatAsLink("Magma", "MAGMA") + " is source of extreme " + UI.FormatAsLink("Heat", "HEAT") + " which can be used to transform " + UI.FormatAsLink("Water", "WATER") + " in to " + UI.FormatAsLink("Steam", "STEAM") + " or " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " into " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ".";
		}

		public static class MARSH
		{
			public static LocString NAME = "Marsh";

			public static LocString DESC = "Visitors to the Marsh Biome will be treated to the soft soothing sounds of " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " as it escapes the lush fields of " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " this biome is known for.";

			public static LocString UTILITY = "Be aware that Marsh Biomes frequently contain large amounts of " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " and should be contained so it doesn't infect your colony with " + UI.FormatAsLink("Slimelung", "SLIMELUNG") + ".\n\n " + UI.FormatAsLink("Dusk Caps", "MUSHROOMPLANT") + " and " + UI.FormatAsLink("Pacus", "PACUSPECIES") + " can provide some higher tier " + UI.FormatAsLink("food", "FOOD") + " ingredients.";
		}

		public static class METALLIC
		{
			public static LocString NAME = "Metallic";

			public static LocString DESC = "";

			public static LocString UTILITY = "";
		}

		public static class OCEAN
		{
			public static LocString NAME = "Ocean";

			public static LocString DESC = "Get away from it all in the " + UI.FormatAsLink("Briny", "BRINE") + " Ocean Biome. " + UI.FormatAsLink("Sand", "SAND") + ", " + UI.FormatAsLink("Salt", "SALT") + " and " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + " abound in this unique biome.";

			public static LocString UTILITY = UI.FormatAsLink("Pokeshell", "CRABSPECIES") + " molt is an excellent source of " + UI.FormatAsLink("Lime", "LIME") + " but much care must be taken with domesticating this species as it can get aggressive around its eggs. Harvesting " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + " provides " + UI.FormatAsLink("Lettuce", "LETTUCE") + " for many higher-tier foods.";
		}

		public static class OIL
		{
			public static LocString NAME = "Oily";

			public static LocString DESC = "Shimmering viscous pools of liquid " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " pepper the " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " rich environment of the Oil Biome. Everything an energy-starved colony needs to support their burgeoning oil consuming power sources.";

			public static LocString UTILITY = "Domesticate " + UI.FormatAsLink("Slicksters", "OILFLOATERSPECIES") + " to create a renewable source of oil. " + UI.FormatAsLink("Sporechids", "EVIL_FLOWER") + " are beautiful, but should only be approached if a Duplicant is properly " + UI.FormatAsLink("equipped", "EQUIPMENT") + ".";
		}

		public static class RADIOACTIVE
		{
			public static LocString NAME = "Radioactive";

			public static LocString DESC = "";

			public static LocString UTILITY = "";
		}

		public static class RUST
		{
			public static LocString NAME = "Rust";

			public static LocString DESC = "The orange-brown oasis of the Rust Biome is home to many unusual flora and fauna. It contains the resources for several intermediate technologies.";

			public static LocString UTILITY = "When combined with the " + UI.FormatAsLink("Rust Deoxidizer", "RUSTDEOXIDIZER") + ", " + UI.FormatAsLink("Rust", "RUST") + " can produce many of a colony's basic needs. " + UI.FormatAsLink("Squeaky Puft", "PUFTBLEACHSTONE") + " are a renewable source of " + UI.FormatAsLink("Bleachstone", "BLEACHSTONE") + ", while " + UI.FormatAsLink("Nosh Bean", "BEANPLANTSEED") + " can be processed to produce excellent food.";
		}

		public static class SANDSTONE
		{
			public static LocString NAME = "Sandstone";

			public static LocString DESC = "The Sandstone Biome is a temperate oasis with few inherent dangers. It's the perfect spot to get your colony up and running.";

			public static LocString UTILITY = UI.FormatAsLink("Oxylite", "OXYROCK") + " and " + UI.FormatAsLink("Buried Muckroot", "BASICFORAGEPLANTPLANTED") + " are in sufficient supply to sustain your colony while " + UI.FormatAsLink("Dirt", "DIRT") + ", " + UI.FormatAsLink("Algae", "ALGAE") + " and " + UI.FormatAsLink("Copper", "COPPER") + " provide the basic materials required to get a colony up and running.";
		}

		public static class WASTELAND
		{
			public static LocString NAME = "Wasteland";

			public static LocString DESC = "";

			public static LocString UTILITY = "";
		}

		public static class SPACE
		{
			public static LocString NAME = "Space";

			public static LocString DESC = "The Space Biome is located on the scenic surface of an asteroid. Watch for dazzling meteorites to shower elements down from the sky.";

			public static LocString UTILITY = "Setting up " + UI.FormatAsLink("Solar Panels", "SOLARPANELS") + " the surface will provide a source of renewable energy. However, much care must be taken to ensure " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " are not sucked out into the " + UI.FormatAsLink("Vacuum", "VACUUM") + " of space. " + UI.FormatAsLink("Shove Voles", "MOLE") + " are native to this biome, and need to be wrangled or contained or they will infest the colony.";
		}

		public static class SWAMP
		{
			public static LocString NAME = "Swampy";

			public static LocString DESC = "";

			public static LocString UTILITY = "";
		}

		public static class NIOBIUM
		{
			public static LocString NAME = "Niobium";

			public static LocString DESC = "";

			public static LocString UTILITY = "";
		}
	}
}
