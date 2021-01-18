namespace STRINGS
{
	public class ITEMS
	{
		public class PILLS
		{
			public class PLACEBO
			{
				public static LocString NAME = "Placebo";

				public static LocString DESC = "A general, all-purpose " + UI.FormatAsLink("Medicine", "MEDICINE") + ".\n\nThe less one knows about it, the better it works.";

				public static LocString RECIPEDESC = "All-purpose " + UI.FormatAsLink("Medicine", "MEDICINE") + ".";
			}

			public class BASICBOOSTER
			{
				public static LocString NAME = "Vitamin Chews";

				public static LocString DESC = "Minorly reduces the chance of becoming sick.";

				public static LocString RECIPEDESC = "A supplement that minorly reduces the chance of contracting a " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nMust be taken daily.";
			}

			public class INTERMEDIATEBOOSTER
			{
				public static LocString NAME = "Immuno Booster";

				public static LocString DESC = "Significantly reduces the chance of becoming sick.";

				public static LocString RECIPEDESC = "A supplement that significantly reduces the chance of contracting a " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nMust be taken daily.";
			}

			public class ANTIHISTAMINE
			{
				public static LocString NAME = "Allergy Medication";

				public static LocString DESC = "Suppresses and prevents allergic reactions.";

				public static LocString RECIPEDESC = string.Concat("A strong antihistamine Duplicants can take to halt an allergic reaction. ", NAME, " will also prevent further reactions from occurring for a short time after ingestion.");
			}

			public class BASICCURE
			{
				public static LocString NAME = "Curative Tablet";

				public static LocString DESC = "A simple, easy-to-take remedy for minor germ-based diseases.";

				public static LocString RECIPEDESC = "Duplicants can take this to cure themselves of minor " + UI.PRE_KEYWORD + "Germ" + UI.PST_KEYWORD + "-based " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nCurative Tablets are very effective against " + UI.FormatAsLink("Food Poisoning", "FOODSICKNESS") + ".";
			}

			public class INTERMEDIATECURE
			{
				public static LocString NAME = "Medical Pack";

				public static LocString DESC = "A doctor-administered cure for moderate ailments.";

				public static LocString RECIPEDESC = string.Concat("A doctor-administered cure for moderate ", UI.FormatAsLink("Diseases", "DISEASE"), ". ", NAME, "s are very effective against ", UI.FormatAsLink("Slimelung", "SLIMESICKNESS"), ".\n\nMust be administered by a Duplicant with the ", DUPLICANTS.ROLES.MEDIC.NAME, " Skill.");
			}

			public class ADVANCEDCURE
			{
				public static LocString NAME = "Serum Vial";

				public static LocString DESC = "A doctor-administered cure for severe ailments.";

				public static LocString RECIPEDESC = string.Concat("An extremely powerful medication created to treat severe ", UI.FormatAsLink("Diseases", "DISEASE"), ". ", NAME, " is very effective against ", UI.FormatAsLink("Slimelung", "SLIMESICKNESS"), ".\n\nMust be administered by a Duplicant with the ", DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, " Skill.");
			}

			public class BASICRADPILL
			{
				public static LocString NAME = "Basic Rad Pill";

				public static LocString DESC = "Increases a Duplicant's natural radiation absorption rate.";

				public static LocString RECIPEDESC = "A supplement that speeds up the rate at which a Duplicant body absorbs radiation, allowing them to manage increased radiation exposure.\n\nMust be taken daily.";
			}

			public class INTERMEDIATERADPILL
			{
				public static LocString NAME = "Intermediate Rad Pill";

				public static LocString DESC = "Increases a Duplicant's natural radiation absorption rate.";

				public static LocString RECIPEDESC = "A supplement that speeds up the rate at which a Duplicant body absorbs radiation, allowing them to manage increased radiation exposure.\n\nMust be taken daily.";
			}
		}

		public class FOOD
		{
			public class FOODSPLAT
			{
				public static LocString NAME = "Food Splatter";

				public static LocString DESC = "Food smeared on the wall from a recent Food Fight";
			}

			public class BURGER
			{
				public static LocString NAME = UI.FormatAsLink("Frost Burger", "BURGER");

				public static LocString DESC = UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".\n\nIt's the only burger best served cold.";

				public static LocString RECIPEDESC = UI.FormatAsLink("Meat", "MEAT") + " and " + UI.FormatAsLink("Lettuce", "LETTUCE") + " on a chilled " + UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD") + ".";
			}

			public class FIELDRATION
			{
				public static LocString NAME = UI.FormatAsLink("Nutrient Bar", "FIELDRATION");

				public static LocString DESC = "A nourishing nutrient paste, sandwiched between thin wafer layers.";
			}

			public class MUSHBAR
			{
				public static LocString NAME = UI.FormatAsLink("Mush Bar", "MUSHBAR");

				public static LocString DESC = "An edible, putrefied mudslop.\n\nMush Bars are preferable to starvation, but only just barely.";

				public static LocString RECIPEDESC = string.Concat("An edible, putrefied mudslop.\n\n", NAME, "s are preferable to starvation, but only just barely.");
			}

			public class MUSHROOMWRAP
			{
				public static LocString NAME = UI.FormatAsLink("Mushroom Wrap", "MUSHROOMWRAP");

				public static LocString DESC = "Flavorful " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + " wrapped in " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".\n\nIt has an earthy flavor punctuated by a refreshing crunch.";

				public static LocString RECIPEDESC = "Flavorful " + UI.FormatAsLink("Mushrooms", "MUSHROOM") + " wrapped in " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".";
			}

			public class MICROWAVEDLETTUCE
			{
				public static LocString NAME = UI.FormatAsLink("Microwaved Lettuce", "MICROWAVEDLETTUCE");

				public static LocString DESC = string.Concat(UI.FormatAsLink("Lettuce", "LETTUCE"), " scrumptiously wilted in the ", BUILDINGS.PREFABS.GAMMARAYOVEN.NAME, ".");

				public static LocString RECIPEDESC = string.Concat(UI.FormatAsLink("Lettuce", "LETTUCE"), " scrumptiously wilted in the ", BUILDINGS.PREFABS.GAMMARAYOVEN.NAME, ".");
			}

			public class GAMMAMUSH
			{
				public static LocString NAME = UI.FormatAsLink("Gamma Mush", "GAMMAMUSH");

				public static LocString DESC = "A disturbingly delicious mixture of irradiated dirt and water.";

				public static LocString RECIPEDESC = string.Concat(UI.FormatAsLink("Mush Fry", "FRIEDMUSHBAR"), " reheated in a ", BUILDINGS.PREFABS.GAMMARAYOVEN.NAME, ".");
			}

			public class FRUITCAKE
			{
				public static LocString NAME = UI.FormatAsLink("Berry Sludge", "FRUITCAKE");

				public static LocString DESC = "A mashed up " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " sludge with an exceptionally long shelf life.\n\nIts aggressive, overbearing sweetness can leave the tongue feeling temporarily numb.";

				public static LocString RECIPEDESC = "A mashed up " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " sludge with an exceptionally long shelf life.";
			}

			public class POPCORN
			{
				public static LocString NAME = UI.FormatAsLink("Popcorn", "POPCORN");

				public static LocString DESC = string.Concat(UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED"), " popped in a ", BUILDINGS.PREFABS.GAMMARAYOVEN.NAME, ".\n\nCompletely devoid of any fancy flavorings.");

				public static LocString RECIPEDESC = "Gamma radiated " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + ".";
			}

			public class SUSHI
			{
				public static LocString NAME = UI.FormatAsLink("Sushi", "SUSHI");

				public static LocString DESC = "Raw " + UI.FormatAsLink("Pacu Fillet", "FISHMEAT") + " wrapped with fresh " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".\n\nWhile the salt of the lettuce may initially overpower the flavor, a keen palate can discern the subtle sweetness of the fillet beneath.";

				public static LocString RECIPEDESC = "Raw " + UI.FormatAsLink("Pacu Fillet", "FISHMEAT") + " wrapped with fresh " + UI.FormatAsLink("Lettuce", "LETTUCE") + ".";
			}

			public class HATCHEGG
			{
				public static LocString NAME = CREATURES.SPECIES.HATCH.EGG_NAME;

				public static LocString DESC = "An egg laid by a " + UI.FormatAsLink("Hatch", "HATCH") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Hatchling", "HATCH") + ".";

				public static LocString RECIPEDESC = "An egg laid by a " + UI.FormatAsLink("Hatch", "HATCH") + ".";
			}

			public class DRECKOEGG
			{
				public static LocString NAME = CREATURES.SPECIES.DRECKO.EGG_NAME;

				public static LocString DESC = "An egg laid by a " + UI.FormatAsLink("Drecko", "DRECKO") + ".\n\nIf incubated, it will hatch into a new " + UI.FormatAsLink("Drecklet", "DRECKO") + ".";

				public static LocString RECIPEDESC = "An egg laid by a " + UI.FormatAsLink("Drecko", "DRECKO") + ".";
			}

			public class LIGHTBUGEGG
			{
				public static LocString NAME = CREATURES.SPECIES.LIGHTBUG.EGG_NAME;

				public static LocString DESC = "An egg laid by a " + UI.FormatAsLink("Shine Bug", "LIGHTBUG") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Shine Nymph", "LIGHTBUG") + ".";

				public static LocString RECIPEDESC = "An egg laid by a " + UI.FormatAsLink("Shine Bug", "LIGHTBUG") + ".";
			}

			public class LETTUCE
			{
				public static LocString NAME = UI.FormatAsLink("Lettuce", "LETTUCE");

				public static LocString DESC = "Crunchy, slightly salty leaves from a " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + " plant.";

				public static LocString RECIPEDESC = "Edible roughage from a " + UI.FormatAsLink("Waterweed", "SEALETTUCE") + ".";
			}

			public class OILFLOATEREGG
			{
				public static LocString NAME = CREATURES.SPECIES.OILFLOATER.EGG_NAME;

				public static LocString DESC = "An egg laid by a " + UI.FormatAsLink("Slickster", "OILFLOATER") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Slickster Larva", "OILFLOATER") + ".";

				public static LocString RECIPEDESC = "An egg laid by a " + UI.FormatAsLink("Slickster", "OILFLOATER") + ".";
			}

			public class PUFTEGG
			{
				public static LocString NAME = CREATURES.SPECIES.PUFT.EGG_NAME;

				public static LocString DESC = "An egg laid by a " + UI.FormatAsLink("Puft", "PUFT") + ".\n\nIf incubated, it will hatch into a " + UI.FormatAsLink("Puftlet", "PUFT") + ".";

				public static LocString RECIPEDESC = string.Concat("An egg laid by a ", CREATURES.SPECIES.PUFT.NAME, ".");
			}

			public class FISHMEAT
			{
				public static LocString NAME = UI.FormatAsLink("Pacu Fillet", "FISHMEAT");

				public static LocString DESC = string.Concat("An uncooked fillet from a very dead ", CREATURES.SPECIES.PACU.NAME, ". Yum!");
			}

			public class MEAT
			{
				public static LocString NAME = UI.FormatAsLink("Meat", "MEAT");

				public static LocString DESC = "Uncooked meat from a very dead critter. Yum!";
			}

			public class PLANTMEAT
			{
				public static LocString NAME = UI.FormatAsLink("Plant Meat", "PLANTMEAT");

				public static LocString DESC = "Planty plant meat from a plant. How nice!";
			}

			public class MUSHROOM
			{
				public static LocString NAME = UI.FormatAsLink("Mushroom", "MUSHROOM");

				public static LocString DESC = "An edible, flavorless fungus that grew in the dark.";
			}

			public class COOKEDFISH
			{
				public static LocString NAME = UI.FormatAsLink("Cooked Fish", "COOKEDFISH");

				public static LocString DESC = string.Concat("The cooked ", UI.FormatAsLink("Fillet", "FISHMEAT"), " of a freshly caught ", CREATURES.SPECIES.PACU.NAME, ".\n\nUnsurprisingly, it tastes a bit fishy.");

				public static LocString RECIPEDESC = string.Concat("The cooked ", UI.FormatAsLink("Fillet", "FISHMEAT"), " of a freshly caught ", CREATURES.SPECIES.PACU.NAME, ".");
			}

			public class COOKEDMEAT
			{
				public static LocString NAME = UI.FormatAsLink("Barbeque", "COOKEDMEAT");

				public static LocString DESC = "The cooked meat of a defeated critter.\n\nIt has a delightful smoky aftertaste.";

				public static LocString RECIPEDESC = "The cooked meat of a defeated critter.";
			}

			public class PICKLEDMEAL
			{
				public static LocString NAME = UI.FormatAsLink("Pickled Meal", "PICKLEDMEAL");

				public static LocString DESC = "Meal Lice preserved in vinegar.\n\nIt's a rarely acquired taste.";

				public static LocString RECIPEDESC = string.Concat(BASICPLANTFOOD.NAME, " regrettably preserved in vinegar.");
			}

			public class FRIEDMUSHBAR
			{
				public static LocString NAME = UI.FormatAsLink("Mush Fry", "FRIEDMUSHBAR");

				public static LocString DESC = "Deep fried, solidified mudslop.\n\nThe inside is almost completely uncooked, despite the crunch on the outside.";

				public static LocString RECIPEDESC = "Deep fried, solidified mudslop.";
			}

			public class RAWEGG
			{
				public static LocString NAME = UI.FormatAsLink("Raw Egg", "RAWEGG");

				public static LocString DESC = "A raw Egg that has been cracked open for use in " + UI.FormatAsLink("Food", "FOOD") + " preparation.\n\nIt will never hatch.";

				public static LocString RECIPEDESC = "A raw egg that has been cracked open for use in " + UI.FormatAsLink("Food", "FOOD") + " preparation.";
			}

			public class COOKEDEGG
			{
				public static LocString NAME = UI.FormatAsLink("Omelette", "COOKEDEGG");

				public static LocString DESC = "Fluffed and folded Egg innards.\n\nIt turns out you do, in fact, have to break a few eggs to make it.";

				public static LocString RECIPEDESC = "Fluffed and folded egg innards.";
			}

			public class FRIEDMUSHROOM
			{
				public static LocString NAME = UI.FormatAsLink("Fried Mushroom", "FRIEDMUSHROOM");

				public static LocString DESC = "A fried dish made with a fruiting " + UI.FormatAsLink("Dusk Cap", "MUSHROOM") + ".\n\nIt has a thick, savory flavor with subtle earthy undertones.";

				public static LocString RECIPEDESC = "A fried dish made with a fruiting " + UI.FormatAsLink("Dusk Cap", "MUSHROOM") + ".";
			}

			public class PRICKLEFRUIT
			{
				public static LocString NAME = UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT");

				public static LocString DESC = "A sweet, mostly pleasant-tasting fruit covered in prickly barbs.";
			}

			public class GRILLEDPRICKLEFRUIT
			{
				public static LocString NAME = UI.FormatAsLink("Gristle Berry", "GRILLEDPRICKLEFRUIT");

				public static LocString DESC = "The grilled bud of a " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + ".\n\nHeat unlocked an exquisite taste in the fruit, though the burnt spines leave something to be desired.";

				public static LocString RECIPEDESC = "The grilled bud of a " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + ".";
			}

			public class SWAMPFRUIT
			{
				public static LocString NAME = UI.FormatAsLink("Bog Jelly", "SWAMPFRUIT");

				public static LocString DESC = "A fruit with an outer film that contains chewy gelatinous cubes.";
			}

			public class SWAMPDELIGHTS
			{
				public static LocString NAME = UI.FormatAsLink("Swampy Delights", "SWAMPDELIGHTS");

				public static LocString DESC = "Dried gelatinous cubes from a " + UI.FormatAsLink("Bog Jelly", "SWAMPFRUIT") + ".\n\nEach cube has a wonderfully chewy texture and is lightly coated in a delicate powder.";

				public static LocString RECIPEDESC = "Dried gelatinous cubes from a " + UI.FormatAsLink("Bog Jelly", "SWAMPFRUIT") + ".";
			}

			public class WORMBASICFRUIT
			{
				public static LocString NAME = UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT");

				public static LocString DESC = "A " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " that failed to develop properly.\n\nIt is nonetheless edible, and vaguely tasty.";
			}

			public class WORMBASICFOOD
			{
				public static LocString NAME = UI.FormatAsLink("Roast Grubfruit Nut", "WORMBASICFOOD");

				public static LocString DESC = "Slow roasted " + UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT") + ".\n\nIt has a smoky aroma and tastes of coziness.";

				public static LocString RECIPEDESC = "Slow roasted " + UI.FormatAsLink("Spindly Grubfruit", "WORMBASICFRUIT") + ".";
			}

			public class WORMSUPERFRUIT
			{
				public static LocString NAME = UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT");

				public static LocString DESC = "A plump, healthy fruit with a honey-like taste.";
			}

			public class WORMSUPERFOOD
			{
				public static LocString NAME = UI.FormatAsLink("Grubfruit Preserve", "WORMSUPERFOOD");

				public static LocString DESC = "A long lasting " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " jam preserved in " + UI.FormatAsLink("Sucrose", "SUCROSE") + ".\n\nThe thick, goopy jam retains the shape of the jar when poured out, but the sweet taste can't be matched.";

				public static LocString RECIPEDESC = "A long lasting " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " jam preserved in " + UI.FormatAsLink("Sucrose", "SUCROSE") + ".";
			}

			public class BERRYPIE
			{
				public static LocString NAME = UI.FormatAsLink("Mixed Berry Pie", "BERRYPIE");

				public static LocString DESC = "A pie made primarily of " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " and " + UI.FormatAsLink("Gristle Berries", "PRICKLEFRUIT") + ".\n\nThe mixture of berries creates a fragrant, colorful filling that packs a sweet punch.";

				public static LocString RECIPEDESC = "A pie made primarily of " + UI.FormatAsLink("Grubfruit", "WORMSUPERFRUIT") + " and " + UI.FormatAsLink("Gristle Berries", "PRICKLEFRUIT") + ".";
			}

			public class COLDWHEATBREAD
			{
				public static LocString NAME = UI.FormatAsLink("Frost Bun", "COLDWHEATBREAD");

				public static LocString DESC = "A simple bun baked from " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + " grain.\n\nEach bite leaves a mild cooling sensation in one's mouth, even when the bun itself is warm.";

				public static LocString RECIPEDESC = "A simple bun baked from " + UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED") + " grain.";
			}

			public class BEAN
			{
				public static LocString NAME = UI.FormatAsLink("Nosh Bean", "BEAN");

				public static LocString DESC = "The crisp bean of a " + UI.FormatAsLink("Nosh Sprout", "BEAN_PLANT") + ".\n\nEach bite tastes refreshingly natural and wholesome.";
			}

			public class SPICENUT
			{
				public static LocString NAME = UI.FormatAsLink("Pincha Peppernut", "SPICENUT");

				public static LocString DESC = "The flavorful nut of a " + UI.FormatAsLink("Pincha Pepperplant", "SPICE_VINE") + ".\n\nThe bitter outer rind hides a rich, peppery core that is useful in cooking.";
			}

			public class SPICEBREAD
			{
				public static LocString NAME = UI.FormatAsLink("Pepper Bread", "SPICEBREAD");

				public static LocString DESC = "A loaf of bread, lightly spiced with " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " for a mild bite.\n\nThere's a simple joy to be had in pulling it apart in one's fingers.";

				public static LocString RECIPEDESC = "A loaf of bread, lightly spiced with " + UI.FormatAsLink("Pincha Peppernut", "SPICENUT") + " for a mild bite.";
			}

			public class SURFANDTURF
			{
				public static LocString NAME = UI.FormatAsLink("Surf'n'Turf", "SURFANDTURF");

				public static LocString DESC = "A bit of " + UI.FormatAsLink("Meat", "MEAT") + " from the land and " + UI.FormatAsLink("Fish", "FISHMEAT") + " from the sea.\n\nIt hearty and satisfying.";

				public static LocString RECIPEDESC = "A bit of " + UI.FormatAsLink("Meat", "MEAT") + " from the land and " + UI.FormatAsLink("Fish", "FISHMEAT") + " from the sea.";
			}

			public class TOFU
			{
				public static LocString NAME = UI.FormatAsLink("Tofu", "TOFU");

				public static LocString DESC = "A bland curd made from " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".\n\nIt has an unusual but pleasant consistency.";

				public static LocString RECIPEDESC = "A bland curd made from " + UI.FormatAsLink("Nosh Beans", "BEAN") + ".";
			}

			public class SPICYTOFU
			{
				public static LocString NAME = UI.FormatAsLink("Spicy Tofu", "SPICYTOFU");

				public static LocString DESC = string.Concat(TOFU.NAME, " marinated in a flavorful ", UI.FormatAsLink("Pincha Pepperplant", "SPICENUT"), " sauce.\n\nIt packs a delightful punch.");

				public static LocString RECIPEDESC = string.Concat(TOFU.NAME, " marinated in a flavorful ", UI.FormatAsLink("Pincha Peppernut", "SPICENUT"), " sauce.");
			}

			public class SALSA
			{
				public static LocString NAME = UI.FormatAsLink("Stuffed Berry", "SALSA");

				public static LocString DESC = "A baked " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " stuffed with delectable spices and vibrantly flavored.";

				public static LocString RECIPEDESC = "A baked " + UI.FormatAsLink("Bristle Berry", "PRICKLEFRUIT") + " stuffed with delectable spices and vibrantly flavored.";
			}

			public class BASICPLANTFOOD
			{
				public static LocString NAME = UI.FormatAsLink("Meal Lice", "BASICPLANTFOOD");

				public static LocString DESC = "A flavorless grain that almost never wiggles on its own.";
			}

			public class BASICPLANTBAR
			{
				public static LocString NAME = UI.FormatAsLink("Liceloaf", "BASICPLANTBAR");

				public static LocString DESC = UI.FormatAsLink("Meal Lice", "BASICPLANTFOOD") + " compacted into a dense, immobile loaf.";

				public static LocString RECIPEDESC = UI.FormatAsLink("Meal Lice", "BASICPLANTFOOD") + " compacted into a dense, immobile loaf.";
			}

			public class BASICFORAGEPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Muckroot", "BASICFORAGEPLANT");

				public static LocString DESC = string.Concat("A seedless fruit with an upsettingly bland aftertaste.\n\nIt cannot be replanted.\n\nDigging up Buried Objects may uncover a ", NAME, ".");
			}

			public class FORESTFORAGEPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Hexalent Fruit", "FORESTFORAGEPLANT");

				public static LocString DESC = "A seedless fruit with an unusual rubbery texture.\n\nIt cannot be replanted.\n\nHexalent fruit is much more calorie dense than Muckroot fruit.";
			}

			public class SWAMPFORAGEPLANT
			{
				public static LocString NAME = UI.FormatAsLink("Swamp Chard Heart", "SWAMPFORAGEPLANT");

				public static LocString DESC = "A seedless plant with a squishy, juicy center and an awful smell.\n\nIt cannot be replanted.";
			}

			public class ROTPILE
			{
				public static LocString NAME = UI.FormatAsLink("Rot Pile", "COMPOST");

				public static LocString DESC = string.Concat("An inedible glop of former foodstuff.\n\n", NAME, "s break down into ", UI.FormatAsLink("Polluted Dirt", "TOXICSAND"), " over time.");
			}

			public class COLDWHEATSEED
			{
				public static LocString NAME = UI.FormatAsLink("Sleet Wheat Grain", "COLDWHEATSEED");

				public static LocString DESC = "An edible grain that leaves a cool taste on the tongue.";
			}

			public class BEANPLANTSEED
			{
				public static LocString NAME = UI.FormatAsLink("Nosh Bean", "BEANPLANTSEED");

				public static LocString DESC = "An inedible bean that can be processed into delicious foods.";
			}

			public static LocString COMPOST = "Compost";
		}

		public class INGREDIENTS
		{
			public class SWAMPLILYFLOWER
			{
				public static LocString NAME = "Balm Lily Flower";

				public static LocString DESC = "A medicinal flower that soothes most minor maladies.\n\nIt is exceptionally fragrant.";
			}
		}

		public class INDUSTRIAL_PRODUCTS
		{
			public class FUEL_BRICK
			{
				public static LocString NAME = "Fuel Brick";

				public static LocString DESC = "A densely compressed brick of combustible material.\n\nIt can be burned to produce a one-time burst of " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class BASIC_FABRIC
			{
				public static LocString NAME = "Reed Fiber";

				public static LocString DESC = "A ball of raw cellulose used in the production of " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " and textiles.";
			}

			public class TRAP_PARTS
			{
				public static LocString NAME = "Trap Components";

				public static LocString DESC = string.Concat("These components can be assembled into a ", BUILDINGS.PREFABS.CREATURETRAP.NAME, " and used to catch ", UI.FormatAsLink("Critters", "CREATURES"), ".");
			}

			public class POWER_STATION_TOOLS
			{
				public static LocString NAME = "Microchip";

				public static LocString DESC = string.Concat("A specialized ", NAME, " created by a professional engineer.\n\nTunes up ", UI.PRE_KEYWORD, "Generators", UI.PST_KEYWORD, " to increase their ", UI.FormatAsLink("Power", "POWER"), " output.");

				public static LocString TINKER_REQUIREMENT_NAME = "Skill: " + DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME;

				public static LocString TINKER_REQUIREMENT_TOOLTIP = string.Concat("Can only be used by a Duplicant with ", DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, " to apply a ", UI.PRE_KEYWORD, "Tune Up", UI.PST_KEYWORD, ".");

				public static LocString TINKER_EFFECT_NAME = "Engie's Tune-Up: {0} {1}";

				public static LocString TINKER_EFFECT_TOOLTIP = "Can be used to " + UI.PRE_KEYWORD + "Tune Up" + UI.PST_KEYWORD + " a generator, increasing its {0} by <b>{1}</b>.";
			}

			public class FARM_STATION_TOOLS
			{
				public static LocString NAME = "Micronutrient Fertilizer";

				public static LocString DESC = string.Concat("Specialized ", UI.FormatAsLink("Fertilizer", "FERTILIZER"), " mixed by a Duplicant with the ", DUPLICANTS.ROLES.FARMER.NAME, " Skill.\n\nIncreases the ", UI.PRE_KEYWORD, "Growth Rate", UI.PST_KEYWORD, " of one ", UI.FormatAsLink("Plant", "PLANTS"), ".");
			}

			public class MACHINE_PARTS
			{
				public static LocString NAME = "Custom Parts";

				public static LocString DESC = "Specialized Parts crafted by a professional engineer.\n\n" + UI.PRE_KEYWORD + "Jerry Rig" + UI.PST_KEYWORD + " machine buildings to increase their efficiency.";

				public static LocString TINKER_REQUIREMENT_NAME = "Job: " + DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME;

				public static LocString TINKER_REQUIREMENT_TOOLTIP = string.Concat("Can only be used by a Duplicant with ", DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, " to apply a ", UI.PRE_KEYWORD, "Jerry Rig", UI.PST_KEYWORD, ".");

				public static LocString TINKER_EFFECT_NAME = "Engineer's Jerry Rig: {0} {1}";

				public static LocString TINKER_EFFECT_TOOLTIP = "Can be used to " + UI.PRE_KEYWORD + "Jerry Rig" + UI.PST_KEYWORD + " upgrades to a machine building, increasing its {0} by <b>{1}</b>.";
			}

			public class RESEARCH_DATABANK
			{
				public static LocString NAME = "Data Bank";

				public static LocString DESC = "Raw data that can be processed into " + UI.PRE_KEYWORD + "Interstellar Research" + UI.PST_KEYWORD + " points.";
			}

			public class EGG_SHELL
			{
				public static LocString NAME = "Egg Shell";

				public static LocString DESC = "Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".";
			}

			public class CRAB_SHELL
			{
				public static LocString NAME = "Pokeshell Molt";

				public static LocString DESC = "Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".";
			}

			public class BABY_CRAB_SHELL
			{
				public static LocString NAME = "Small Pokeshell Molt";

				public static LocString DESC = "Can be crushed to produce " + UI.FormatAsLink("Lime", "LIME") + ".";
			}

			public class WOOD
			{
				public static LocString NAME = "Lumber";

				public static LocString DESC = "Wood harvested from an " + UI.FormatAsLink("Arbor Tree", "FOREST_TREE") + ".";
			}

			public class GENE_SHUFFLER_RECHARGE
			{
				public static LocString NAME = "Vacillator Recharge";

				public static LocString DESC = string.Concat("Replenishes one charge to a depleted ", BUILDINGS.PREFABS.GENESHUFFLER.NAME, ".");
			}

			public class TABLE_SALT
			{
				public static LocString NAME = "Table Salt";

				public static LocString DESC = string.Concat("A seasoning that Duplicants can add to their ", UI.FormatAsLink("Food", "FOOD"), " to boost ", UI.FormatAsLink("Morale", "MORALE"), ".\n\nDuplicants will automatically use Table Salt while sitting at a ", BUILDINGS.PREFABS.DININGTABLE.NAME, " during mealtime.\n\n<i>Only the finest grains are chosen.</i>");
			}

			public class REFINED_SUGAR
			{
				public static LocString NAME = "Refined Sugar";

				public static LocString DESC = string.Concat("A seasoning that Duplicants can add to their ", UI.FormatAsLink("Food", "FOOD"), " to boost ", UI.FormatAsLink("Morale", "MORALE"), ".\n\nDuplicants will automatically use Refined Sugar while sitting at a ", BUILDINGS.PREFABS.DININGTABLE.NAME, " during mealtime.\n\n<i>Only the finest grains are chosen.</i>");
			}
		}

		public class CARGO_CAPSULE
		{
			public static LocString NAME = "Care Package";

			public static LocString DESC = "A delivery system for recently printed resources.\n\nIt will dematerialize shortly.";
		}

		public class RAILGUNPAYLOAD
		{
			public static LocString NAME = UI.FormatAsLink("Interplanetary Payload", "RAILGUNPAYLOAD");

			public static LocString DESC = string.Concat("Contains resources packed for interstellar shipping.\n\nCan be launched by a ", BUILDINGS.PREFABS.RAILGUN.NAME, " or unpacked with a ", BUILDINGS.PREFABS.RAILGUNPAYLOADOPENER.NAME, ".");
		}

		public class RADIATION
		{
			public class HIGHENERGYPARITCLE
			{
				public static LocString NAME = "High Energy Particles";

				public static LocString DESC = "A concentrated field of " + UI.FormatAsLink("High Enery Particles", "HIGHENERGYPARTICLES") + " that can be largely redirected using a " + UI.FormatAsLink("Particle Reflector", "PARTICLEREFLECTOR") + ".";
			}
		}
	}
}
