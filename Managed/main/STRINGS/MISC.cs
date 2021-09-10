namespace STRINGS
{
	public class MISC
	{
		public class TAGS
		{
			public static LocString OTHER = "Miscellaneous";

			public static LocString FILTER = "Filtration Medium";

			public static LocString ICEORE = "Ice";

			public static LocString PHOSPHORUS = "Phosphorus";

			public static LocString BUILDABLERAW = "Raw Mineral";

			public static LocString BUILDABLEPROCESSED = "Refined Mineral";

			public static LocString BUILDABLEANY = "Generic Buildable";

			public static LocString REFINEDMETAL = "Refined Metal";

			public static LocString METAL = "Metal Ore";

			public static LocString PRECIOUSMETAL = "Precious Metal";

			public static LocString RAWPRECIOUSMETAL = "Precious Metal Ore";

			public static LocString PRECIOUSROCK = "Precious Rock";

			public static LocString ALLOY = "Alloy";

			public static LocString BUILDINGFIBER = "Fiber";

			public static LocString BUILDINGWOOD = "Wood";

			public static LocString CRUSHABLE = "Crushable";

			public static LocString BAGABLECREATURE = "Critter";

			public static LocString SWIMMINGCREATURE = "Aquatic Critter";

			public static LocString LIFE = "Life";

			public static LocString LIQUIFIABLE = "Liquefiable";

			public static LocString LIQUID = "Liquid";

			public static LocString SPECIAL = "Special";

			public static LocString FARMABLE = "Cultivable Soil";

			public static LocString AGRICULTURE = "Agriculture";

			public static LocString COAL = "Coal";

			public static LocString BLEACHSTONE = "Bleach Stone";

			public static LocString ORGANICS = "Organic";

			public static LocString CONSUMABLEORE = "Consumable Ore";

			public static LocString ORE = "Ore";

			public static LocString BREATHABLE = "Breathable Gas";

			public static LocString UNBREATHABLE = "Unbreathable Gas";

			public static LocString GAS = "Gas";

			public static LocString BURNS = "Flammable";

			public static LocString UNSTABLE = "Unstable";

			public static LocString TOXIC = "Toxic";

			public static LocString MIXTURE = "Mixture";

			public static LocString SOLID = "Solid";

			public static LocString FLYINGCRITTEREDIBLE = "Bait";

			public static LocString INDUSTRIALPRODUCT = "Industrial Product";

			public static LocString INDUSTRIALINGREDIENT = "Industrial Ingredient";

			public static LocString MEDICALSUPPLIES = "Medical Supplies";

			public static LocString CLOTHES = "Clothing";

			public static LocString EMITSLIGHT = "Light Emitter";

			public static LocString BED = "Bed";

			public static LocString MESSSTATION = "Dining Table";

			public static LocString TOY = "Toy";

			public static LocString SUIT = "Suit";

			public static LocString MULTITOOL = "Multitool";

			public static LocString CLINIC = "Clinic";

			public static LocString RELAXATION_POINT = "Leisure Area";

			public static LocString SOLIDMATERIAL = "Solid Material";

			public static LocString EXTRUDABLE = "Extrudable";

			public static LocString PLUMBABLE = "Plumbable";

			public static LocString COMPOSTABLE = "Compostable";

			public static LocString COMPOSTBASICPLANTFOOD = "Compost Muckroot";

			public static LocString EDIBLE = "Edible";

			public static LocString OXIDIZER = "Oxidizer";

			public static LocString COOKINGINGREDIENT = "Cooking Ingredient";

			public static LocString MEDICINE = "Medicine";

			public static LocString SEED = "Seed";

			public static LocString ANYWATER = "Water Based";

			public static LocString MARKEDFORCOMPOST = "Marked For Compost";

			public static LocString MARKEDFORCOMPOSTINSTORAGE = "In Compost Storage";

			public static LocString COMPOSTMEAT = "Compost Meat";

			public static LocString PICKLED = "Pickled";

			public static LocString PLASTIC = "Plastic";

			public static LocString TOILET = "Toilet";

			public static LocString MASSAGE_TABLE = "Massage Table";

			public static LocString POWERSTATION = "Power Station";

			public static LocString FARMSTATION = "Farm Station";

			public static LocString MACHINE_SHOP = "Machine Shop";

			public static LocString ANTISEPTIC = "Antiseptic";

			public static LocString OIL = "Hydrocarbon";

			public static LocString DECORATION = "Decoration";

			public static LocString EGG = "Critter Egg";

			public static LocString EGGSHELL = "Egg Shell";

			public static LocString MANUFACTUREDMATERIAL = "Manufactured Material";

			public static LocString STEEL = "Steel";

			public static LocString RAW = "Raw Animal Product";

			public static LocString ANY = "Any";

			public static LocString TRANSPARENT = "Transparent";

			public static LocString RAREMATERIALS = "Rare Resource";

			public static LocString FARMINGMATERIAL = "Fertilizer";

			public static LocString INSULATOR = "Insulator";

			public static LocString RAILGUNPAYLOADEMPTYABLE = "Payload";

			public static LocString COMMAND_MODULE = "Command Module";

			public static LocString HABITAT_MODULE = "Habitat Module";

			public static LocString COMBUSTIBLEGAS = "Combustible Gas";

			public static LocString COMBUSTIBLELIQUID = "Combustible Liquid";

			public static LocString COMBUSTIBLESOLID = "Combustible Solid";

			public static LocString UNIDENTIFIEDSEED = "Seed (Unidentified Mutation)";

			public static LocString CHARMEDARTIFACT = "Artifact of Interest";

			public static LocString GENE_SHUFFLER = "Neural Vacillator";

			public static LocString WARP_PORTAL = "Teleportal";

			public static LocString FARMING = "Farm Build-Delivery";

			public static LocString RESEARCH = "Research Delivery";

			public static LocString POWER = "Generator Delivery";

			public static LocString BUILDING = "Build Dig-Delivery";

			public static LocString COOKING = "Cook Delivery";

			public static LocString FABRICATING = "Fabricate Delivery";

			public static LocString WIRING = "Wire Build-Delivery";

			public static LocString ART = "Art Build-Delivery";

			public static LocString DOCTORING = "Treatment Delivery";

			public static LocString CONVEYOR = "Shipping Build";

			public static LocString COMPOST_FORMAT = "{Item}";
		}

		public class STATUSITEMS
		{
			public class SUBLIMATIONBLOCKED
			{
				public static LocString NAME = "{SubElement} emission blocked";

				public static LocString TOOLTIP = "This {Element} deposit is not exposed to air and cannot emit {SubElement}";
			}

			public class SUBLIMATIONOVERPRESSURE
			{
				public static LocString NAME = "Inert";

				public static LocString TOOLTIP = "Environmental " + UI.PRE_KEYWORD + "Gas Pressure" + UI.PST_KEYWORD + " is too high for this {Element} deposit to emit {SubElement}";
			}

			public class SUBLIMATIONEMITTING
			{
				public static LocString NAME = BUILDING.STATUSITEMS.EMITTINGGASAVG.NAME;

				public static LocString TOOLTIP = BUILDING.STATUSITEMS.EMITTINGGASAVG.TOOLTIP;
			}

			public class SPACE
			{
				public static LocString NAME = "Space exposure";

				public static LocString TOOLTIP = "This region is exposed to the vacuum of space and will result in the loss of " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " resources";
			}

			public class EDIBLE
			{
				public static LocString NAME = "Rations: {0}";

				public static LocString TOOLTIP = "Can provide " + UI.FormatAsLink("{0}", "KCAL") + " of energy to Duplicants";
			}

			public class MARKEDFORDISINFECTION
			{
				public static LocString NAME = "Disinfect Errand";

				public static LocString TOOLTIP = "Building will be disinfected once a Duplicant is available";
			}

			public class PENDINGCLEAR
			{
				public static LocString NAME = "Sweep Errand";

				public static LocString TOOLTIP = "Debris will be swept once a Duplicant is available";
			}

			public class PENDINGCLEARNOSTORAGE
			{
				public static LocString NAME = "Storage Unavailable";

				public static LocString TOOLTIP = string.Concat("No available ", BUILDINGS.PREFABS.STORAGELOCKER.NAME, " can accept this item\n\nMake sure the filter on your storage is correctly set and there is sufficient space remaining");
			}

			public class MARKEDFORCOMPOST
			{
				public static LocString NAME = "Compost Errand";

				public static LocString TOOLTIP = string.Concat("Object is marked and will be moved to ", BUILDINGS.PREFABS.COMPOST.NAME, " once a Duplicant is available");
			}

			public class NOCLEARLOCATIONSAVAILABLE
			{
				public static LocString NAME = "No Sweep Destination";

				public static LocString TOOLTIP = "There are no valid destinations for this object to be swept to";
			}

			public class PENDINGHARVEST
			{
				public static LocString NAME = "Harvest Errand";

				public static LocString TOOLTIP = "Plant will be harvested once a Duplicant is available";
			}

			public class PENDINGUPROOT
			{
				public static LocString NAME = "Uproot Errand";

				public static LocString TOOLTIP = "Plant will be uprooted once a Duplicant is available";
			}

			public class WAITINGFORDIG
			{
				public static LocString NAME = "Dig Errand";

				public static LocString TOOLTIP = "Tile will be dug out once a Duplicant is available";
			}

			public class WAITINGFORMOP
			{
				public static LocString NAME = "Mop Errand";

				public static LocString TOOLTIP = "Spill will be mopped once a Duplicant is available";
			}

			public class NOTMARKEDFORHARVEST
			{
				public static LocString NAME = "No Harvest Pending";

				public static LocString TOOLTIP = "Use the " + UI.FormatAsTool("Harvest Tool", "[Y]") + " to mark this plant for harvest";
			}

			public class GROWINGBRANCHES
			{
				public static LocString NAME = "Growing Branches";

				public static LocString TOOLTIP = "This tree is working hard to grow new branches right now";
			}

			public class ELEMENTALCATEGORY
			{
				public static LocString NAME = "{Category}";

				public static LocString TOOLTIP = "The selected object belongs to the <b>{Category}</b> resource category";
			}

			public class ELEMENTALMASS
			{
				public static LocString NAME = "{Mass}";

				public static LocString TOOLTIP = "The selected object has a mass of <b>{Mass}</b>";
			}

			public class ELEMENTALDISEASE
			{
				public static LocString NAME = "{Disease}";

				public static LocString TOOLTIP = "Current disease: {Disease}";
			}

			public class ELEMENTALTEMPERATURE
			{
				public static LocString NAME = "{Temp}";

				public static LocString TOOLTIP = "The selected object is currently <b>{Temp}</b>";
			}

			public class MARKEDFORCOMPOSTINSTORAGE
			{
				public static LocString NAME = "Composted";

				public static LocString TOOLTIP = "The selected object is currently in the compost";
			}

			public class BURIEDITEM
			{
				public static LocString NAME = "Buried Object";

				public static LocString TOOLTIP = "Something seems to be hidden here";

				public static LocString NOTIFICATION = "Buried object discovered";

				public static LocString NOTIFICATION_TOOLTIP = "My Duplicants have uncovered a {Uncoverable}!\n\nClick to jump to its location.";
			}

			public class GENETICANALYSISCOMPLETED
			{
				public static LocString NAME = "Genome Sequenced";

				public static LocString TOOLTIP = "This Station has sequenced a new seed mutation";
			}

			public class HEALTHSTATUS
			{
				public class PERFECT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is in peak condition";
				}

				public class ALRIGHT
				{
					public static LocString NAME = "None";

					public static LocString TOOLTIP = "This Duplicant is none the worse for wear";
				}

				public class SCUFFED
				{
					public static LocString NAME = "Minor";

					public static LocString TOOLTIP = "This Duplicant has a few scrapes and bruises";
				}

				public class INJURED
				{
					public static LocString NAME = "Moderate";

					public static LocString TOOLTIP = "This Duplicant needs some patching up";
				}

				public class CRITICAL
				{
					public static LocString NAME = "Severe";

					public static LocString TOOLTIP = "This Duplicant is in serious need of medical attention";
				}

				public class INCAPACITATED
				{
					public static LocString NAME = "Paralyzing";

					public static LocString TOOLTIP = "This Duplicant will die if they do not receive medical attention";
				}

				public class DEAD
				{
					public static LocString NAME = "Conclusive";

					public static LocString TOOLTIP = "This Duplicant won't be getting back up";
				}
			}

			public class HIT
			{
				public static LocString NAME = "{targetName} took {damageAmount} damage from {attackerName}'s attack!";
			}

			public class OREMASS
			{
				public static LocString NAME = ELEMENTALMASS.NAME;

				public static LocString TOOLTIP = ELEMENTALMASS.TOOLTIP;
			}

			public class ORETEMP
			{
				public static LocString NAME = ELEMENTALTEMPERATURE.NAME;

				public static LocString TOOLTIP = ELEMENTALTEMPERATURE.TOOLTIP;
			}

			public class TREEFILTERABLETAGS
			{
				public static LocString NAME = "{Tags}";

				public static LocString TOOLTIP = "{Tags}";
			}

			public class SPOUTOVERPRESSURE
			{
				public static LocString NAME = "Overpressure {StudiedDetails}";

				public static LocString TOOLTIP = "Spout cannot vent due to high environmental pressure";

				public static LocString STUDIED = "(idle in <b>{Time}</b>)";
			}

			public class SPOUTEMITTING
			{
				public static LocString NAME = "Venting {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is erupting";

				public static LocString STUDIED = "(idle in <b>{Time}</b>)";
			}

			public class SPOUTPRESSUREBUILDING
			{
				public static LocString NAME = "Rising pressure {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser's internal pressure is steadily building";

				public static LocString STUDIED = "(erupts in <b>{Time}</b>)";
			}

			public class SPOUTIDLE
			{
				public static LocString NAME = "Idle {StudiedDetails}";

				public static LocString TOOLTIP = "This geyser is not currently erupting";

				public static LocString STUDIED = "(erupts in <b>{Time}</b>)";
			}

			public class SPOUTDORMANT
			{
				public static LocString NAME = "Dormant";

				public static LocString TOOLTIP = "This geyser's geoactivity has halted" + UI.HORIZONTAL_BR_RULE + "It won't erupt again for some time";
			}

			public class PICKUPABLEUNREACHABLE
			{
				public static LocString NAME = "Unreachable";

				public static LocString TOOLTIP = "Duplicants cannot reach this object";
			}

			public class PRIORITIZED
			{
				public static LocString NAME = "High Priority";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Errand" + UI.PST_KEYWORD + " has been marked as important and will be preferred over other pending " + UI.PRE_KEYWORD + "Errands" + UI.PST_KEYWORD;
			}

			public class USING
			{
				public static LocString NAME = "Using {Target}";

				public static LocString TOOLTIP = "{Target} is currently in use";
			}

			public class ORDERATTACK
			{
				public static LocString NAME = "Pending Attack";

				public static LocString TOOLTIP = "Waiting for a Duplicant to murderize this defenseless " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD;
			}

			public class ORDERCAPTURE
			{
				public static LocString NAME = "Pending Wrangle";

				public static LocString TOOLTIP = string.Concat("Waiting for a Duplicant to capture this ", UI.PRE_KEYWORD, "Critter", UI.PST_KEYWORD, UI.HORIZONTAL_BR_RULE, "Only Duplicants with the ", DUPLICANTS.ROLES.RANCHER.NAME, " Skill can catch critters without traps");
			}

			public class OPERATING
			{
				public static LocString NAME = "In Use";

				public static LocString TOOLTIP = "This object is currently being used";
			}

			public class CLEANING
			{
				public static LocString NAME = "Cleaning";

				public static LocString TOOLTIP = "This building is currently being cleaned";
			}

			public class REGIONISBLOCKED
			{
				public static LocString NAME = "Blocked";

				public static LocString TOOLTIP = "Undug material is blocking off an essential tile";
			}

			public class STUDIED
			{
				public static LocString NAME = "Analysis Complete";

				public static LocString TOOLTIP = "Information on this Natural Feature has been compiled below.";
			}

			public class AWAITINGSTUDY
			{
				public static LocString NAME = "Analysis Pending";

				public static LocString TOOLTIP = "New information on this Natural Feature will be compiled once the field study is complete";
			}

			public class DURABILITY
			{
				public static LocString NAME = "Durability: {durability}";

				public static LocString TOOLTIP = "Items lose durability each time they are equipped, and can no longer be put on by a Duplicant once they reach 0% durability\n\nItems can be repaired using the " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE");
			}

			public class STOREDITEMDURABILITY
			{
				public static LocString NAME = "Durability: {durability}";

				public static LocString TOOLTIP = "Items lose durability each time they are equipped, and can no longer be put on by a Duplicant once they reach 0% durability\n\nItems can be repaired using the " + UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE");
			}

			public class ARTIFACTENTOMBED
			{
				public static LocString NAME = "Entombed Artifact";

				public static LocString TOOLTIP = "This artifact is trapped in an obscuring shell limiting its decor. A skilled artist can remove it at the " + BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.NAME;
			}

			public class TEAROPEN
			{
				public static LocString NAME = "Temporal Tear open";

				public static LocString TOOLTIP = "An open passage through spacetime";
			}

			public class TEARCLOSED
			{
				public static LocString NAME = "Temporal Tear closed";

				public static LocString TOOLTIP = "Perhaps some technology could open the passage";
			}
		}

		public class POPFX
		{
			public static LocString RESOURCE_EATEN = "Resource Eaten";
		}

		public class NOTIFICATIONS
		{
			public class BASICCONTROLS
			{
				public static LocString NAME = "Tutorial: Basic Controls";

				public static LocString MESSAGEBODY = "• The " + UI.FormatAsHotkey("[WASD]") + " keys pan my view and the " + UI.FormatAsHotkey("[MOUSE WHEEL]") + " zooms it in and out.\n\n• " + UI.FormatAsHotkey("[H]") + " returns my view to the Printing Pod.\n\n• I can speed or slow my perception of time using the top left corner buttons, or by pressing " + UI.FormatAsHotkey("[TAB]") + ", while pressing " + UI.FormatAsHotkey("[SPACE]") + " will pause the flow of time entirely.\n\n• I'll keep records of everything I discover in my personal DATABASE " + UI.FormatAsHotkey("[U]") + " to refer back to if I forget anything important.";

				public static LocString TOOLTIP = "Notes on using my HUD";
			}

			public class CODEXUNLOCK
			{
				public static LocString NAME = "New Log Entry";

				public static LocString MESSAGEBODY = "I've added a new log entry to my Database";

				public static LocString TOOLTIP = "I've added a new log entry to my Database";
			}

			public class WELCOMEMESSAGE
			{
				public static LocString NAME = "Tutorial: Colony Management";

				public static LocString MESSAGEBODY = "I can use the " + UI.FormatAsTool("Dig Tool", "[G]") + " and the " + UI.FormatAsBuildMenuTab("Build Menu") + " in the lower left of the screen to begin planning my first construction tasks.\n\nOnce I've placed a few errands my Duplicants will automatically get to work, without me needing to direct them individually.";

				public static LocString TOOLTIP = "Notes on getting Duplicants to do my bidding";
			}

			public class STRESSMANAGEMENTMESSAGE
			{
				public static LocString NAME = "Tutorial: Stress Management";

				public static LocString MESSAGEBODY = "At 100% " + UI.FormatAsLink("Stress", "STRESS") + ", a Duplicant will have a nervous breakdown and be unable to work.\n\nBreakdowns can manifest in different colony-threatening ways, such as the destruction of buildings or the binge eating of food.\n\nI can select a Duplicant and mouse over " + UI.FormatAsLink("Stress", "STRESS") + " in their STATUS TAB to view their individual " + UI.FormatAsLink("Stress Factors", "STRESS") + ", and hopefully minimize them before they become a problem.";

				public static LocString TOOLTIP = "Notes on keeping Duplicants happy and productive";
			}

			public class TASKPRIORITIESMESSAGE
			{
				public static LocString NAME = "Tutorial: Priority";

				public static LocString MESSAGEBODY = "Duplicants always perform errands in order of highest to lowest priority. They will harvest " + UI.FormatAsLink("Food", "FOOD") + " before they build, for example, or always build new structures before they mine materials.\n\nI can open the " + UI.FormatAsManagementMenu("Priorities Screen", "[L]") + " to set which Errand Types Duplicants may or may not perform, or to specialize skilled Duplicants for particular Errand Types.";

				public static LocString TOOLTIP = "Notes on managing Duplicants' errands";
			}

			public class MOPPINGMESSAGE
			{
				public static LocString NAME = "Tutorial: Polluted Water";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " slowly emits " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " which accelerates the spread of " + UI.FormatAsLink("Disease", "DISEASE") + ".\n\nDuplicants will also be " + UI.FormatAsLink("Stressed", "STRESS") + " by walking through Polluted Water, so I should have my Duplicants clean up spills by clicking and dragging the " + UI.FormatAsTool("Mop Tool", "[M]");

				public static LocString TOOLTIP = "Notes on handling polluted materials";
			}

			public class LOCOMOTIONMESSAGE
			{
				public static LocString NAME = "Video: Duplicant Movement";

				public static LocString MESSAGEBODY = "Duplicants have limited jumping and climbing abilities. They can only climb two tiles high and cannot fit into spaces shorter than two tiles, or cross gaps wider than one tile. I should keep this in mind while placing errands.\n\nTo check if an errand I've placed is accessible, I can select a Duplicant and click <b>Show Navigation</b> to view all areas within their reach.";

				public static LocString TOOLTIP = "Notes on my Duplicants' maneuverability";
			}

			public class PRIORITIESMESSAGE
			{
				public static LocString NAME = "Tutorial: Errand Priorities";

				public static LocString MESSAGEBODY = "Duplicants will choose where to work based on the priority of the errands that I give them. I can open the " + UI.FormatAsManagementMenu("Priorities Screen", "[L]") + " to set their " + UI.PRE_KEYWORD + "Duplicant Priorities" + UI.PST_KEYWORD + ", and the " + UI.FormatAsTool("Priority Tool", "[P]") + " to fine tune " + UI.PRE_KEYWORD + "Building Priority" + UI.PST_KEYWORD + ". Many buildings will also let me change their Priority level when I select them.";

				public static LocString TOOLTIP = "Notes on my Duplicants' priorities";
			}

			public class FETCHINGWATERMESSAGE
			{
				public static LocString NAME = "Tutorial: Fetching Water";

				public static LocString MESSAGEBODY = "By building a " + UI.FormatAsLink("Pitcher Pump", "LIQUIDPUMPINGSTATION") + " from the " + UI.FormatAsBuildMenuTab("Plumbing Tab", "[5]") + " over a pool of liquid, my Duplicants will be able to bottle it up and manually deliver it wherever it needs to go.";

				public static LocString TOOLTIP = "Notes on liquid resource gathering";
			}

			public class SCHEDULEMESSAGE
			{
				public static LocString NAME = "Tutorial: Scheduling";

				public static LocString MESSAGEBODY = "My Duplicants will only eat, sleep, work, or bathe during the times I allot for such activities.\n\nTo make the best use of their time, I can open the " + UI.FormatAsManagementMenu("Schedule Tab", "[PERIOD KEY]") + " to adjust the colony's schedule and plan how they should utilize their day.";

				public static LocString TOOLTIP = "Notes on scheduling my Duplicants' time";
			}

			public class THERMALCOMFORT
			{
				public static LocString NAME = "Tutorial: Duplicant Temperature";

				public static LocString TOOLTIP = "Notes on helping Duplicants keep their cool";

				public static LocString MESSAGEBODY = "Environments that are extremely " + UI.FormatAsLink("Hot", "HEAT") + " or " + UI.FormatAsLink("Cold", "HEAT") + " affect my Duplicants' internal body temperature and cause undue " + UI.FormatAsLink("Stress", "STRESS") + ".\n\nOpening the " + UI.FormatAsOverlay("Temperature Overlay", "[F3]") + " and checking the <b>Thermal Tolerance</b> box allows me to view all areas where my Duplicants will feel discomfort and be unable to regulate their internal body temperature.";
			}

			public class TUTORIAL_OVERHEATING
			{
				public static LocString NAME = "Tutorial: Building Temperature";

				public static LocString TOOLTIP = "Notes on preventing building from breaking";

				public static LocString MESSAGEBODY = "When constructing buildings, I should always take note of their " + UI.FormatAsLink("Overheat Temperature", "HEAT") + " and plan their locations accordingly. Maintaining low ambient temperatures and good ventilation in the colony will also help keep building temperatures down.\n\nIf I allow buildings to exceed their Overheat Temperature they will begin to take damage, and if left unattended, they will break down and be unusable until repaired.";
			}

			public class LOTS_OF_GERMS
			{
				public static LocString NAME = "Tutorial: Germs and Disease";

				public static LocString TOOLTIP = "Notes on Duplicant disease risks";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Germs", "DISEASE") + " such as " + UI.FormatAsLink("Food Poisoning", "FOODSICKNESS") + " and " + UI.FormatAsLink("Slimelung", "SLIMESICKNESS") + " can cause " + UI.FormatAsLink("Disease", "DISEASE") + " in my Duplicants. I can use the " + UI.FormatAsOverlay("Germ Overlay", "[F9]") + " to view all germ concentrations in my colony, and even detect the sources spawning them.\n\nBuilding Wash Basins from the " + UI.FormatAsBuildMenuTab("Medicine Tab", "[8]") + " by colony toilets will tell my Duplicants they need to wash up.";
			}

			public class BEING_INFECTED
			{
				public static LocString NAME = "Tutorial: Immune Systems";

				public static LocString TOOLTIP = "Notes on keeping Duplicants in peak health";

				public static LocString MESSAGEBODY = "When Duplicants come into contact with various " + UI.FormatAsLink("Germs", "DISEASE") + ", they'll need to expend points of " + UI.FormatAsLink("Immunity", "IMMUNE SYSTEM") + " to resist them and remain healthy. If repeated exposes causes their Immunity to drop to 0%, they'll be unable to resist germs and will contract the next disease they encounter.\n\nDoors with Access Permissions can be built from the BASE TAB<color=#F44A47> <b>[1]</b></color> of the " + UI.FormatAsLink("Build menu", "misc") + " to block Duplicants from entering biohazardous areas while they recover their spent immunity points.";
			}

			public class DISEASE_COOKING
			{
				public static LocString NAME = "Tutorial: Food Safety";

				public static LocString TOOLTIP = "Notes on managing food contamination";

				public static LocString MESSAGEBODY = string.Concat("The ", UI.FormatAsLink("Food", "FOOD"), " my Duplicants cook will only ever be as clean as the ingredients used to make it. Storing food in sterile or ", UI.FormatAsLink("Refrigerated", "REFRIGERATOR"), " environments will keep food free of ", UI.FormatAsLink("Germs", "DISEASE"), ", while carefully placed hygiene stations like ", BUILDINGS.PREFABS.WASHBASIN.NAME, " or ", BUILDINGS.PREFABS.SHOWER.NAME, " will prevent the cooks from infecting the food by handling it.\n\nDangerously contaminated food can be sent to compost by clicking the <b>Compost</b> button on the selected item.");
			}

			public class SUITS
			{
				public static LocString NAME = "Tutorial: Atmo Suits";

				public static LocString TOOLTIP = "Notes on using atmo suits";

				public static LocString MESSAGEBODY = UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + " can be equipped to protect my Duplicants from environmental hazards like extreme " + UI.FormatAsLink("Heat", "Heat") + ", airborne " + UI.FormatAsLink("Germs", "DISEASE") + ", or unbreathable " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ". In order to utilize these suits, I'll need to hook up an Atmo Suit Dock to an Atmo Suit Checkpoint, then store one of the suits inside.\n\nDuplicants will equip a suit when they walk past the checkpoint in the chosen direction, and will unequip their suit when walking back the opposite way.";
			}

			public class RADIATION
			{
				public static LocString NAME = "Tutorial: Radiation";

				public static LocString TOOLTIP = "Notes on managing radiation";

				public static LocString MESSAGEBODY = "Objects such as " + UI.FormatAsLink("Uranium Ore", "URANIUMORE") + " and " + UI.FormatAsLink("Beeta Hives", "BEE") + " emit a " + UI.FormatAsLink("Radioactive", "RADIOACTIVE") + " energy that can be toxic to my Duplicants.\n\nI can use the " + UI.FormatAsOverlay("Radiation Overlay") + " " + UI.FormatAsHotkey("[L-SHIFT + F4]") + " to check the scope of the Radiation field. Building thick walls around radiation emitters will dampen the field and protect my Duplicants from getting " + UI.FormatAsLink("Radiation Sickness", "RADIATIONSICKNESS") + " .";
			}

			public class SPACETRAVEL
			{
				public static LocString NAME = "Tutorial: Space Travel";

				public static LocString TOOLTIP = "Notes on traveling in space";

				public static LocString MESSAGEBODY = "Building a rocket first requires constructing a " + UI.FormatAsLink("Rocket Platform", "LAUNCHPLATFORM") + " and adding modules from the menu. All components of the Rocket Checklist will need to be complete before being capable of launching.\n\nA " + UI.FormatAsLink("Telescope", "TELESCOPE") + " needs to built on the surface of a Planetoid in order to use the " + UI.PRE_KEYWORD + "Starmap Screen" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[Z]") + " to see and set course for new destinations.";
			}

			public class MORALE
			{
				public static LocString NAME = "Video: Duplicant Morale";

				public static LocString TOOLTIP = "Notes on Duplicant expectations";

				public static LocString MESSAGEBODY = "Food, Rooms, Decor, and Recreation all have an effect on Duplicant Morale. Good experiences improve their Morale, while poor experiences lower it. When a Duplicant's Morale is below their Expectations, they will become Stressed.\n\nDuplicants' Expectations will get higher as they are given new Skills, and the colony will have to be improved to keep up their Morale. An overview of Morale and Stress can be viewed on the Vitals screen.";
			}

			public class POWER
			{
				public static LocString NAME = "Video: Power Circuits";

				public static LocString TOOLTIP = "Notes on managing electricity";

				public static LocString MESSAGEBODY = "Generators are considered \"Producers\" of Power, while the various buildings and machines in the colony are considered \"Consumers\". Each Consumer will pull a certain wattage from the power circuit it is connected to, which can be checked at any time by clicking the building and going to the Energy Tab.\n\nI can use the Power Overlay " + UI.FormatAsHotkey("[F2]") + " to quickly check the status of all my circuits. If the Consumers are taking more wattage than the Generators are creating, the Batteries will drain and there will be brownouts.\n\nAdditionally, if the Consumers are pulling more wattage through the Wires than the Wires can handle, they will overload and burn out. To correct both these situations, I will need to reorganize my Consumers onto separate circuits.";
			}

			public class DIGGING
			{
				public static LocString NAME = "Video: Digging for Resources";

				public static LocString TOOLTIP = "Notes on buried riches";

				public static LocString MESSAGEBODY = "Everything a colony needs to get going is found in the ground. Instructing Duplicants to dig out areas means we can find food, mine resources to build infrastructure, and clear space for the colony to grow. I can access the Dig Tool with " + UI.FormatAsHotkey("[G]") + ", which allows me to select the area where I want my Duplicants to dig.\n\nDuplicants will need to gain the Superhard Digging skill to mine Abyssalite and the Superduperhard Digging skill to mine Diamond and Obsidian. Without the proper skills, these materials will be undiggable.";
			}

			public class INSULATION
			{
				public static LocString NAME = "Video: Insulation";

				public static LocString TOOLTIP = "Notes on effective temperature management";

				public static LocString MESSAGEBODY = "The temperature of an environment can have positive or negative effects on the wellbeing of my Duplicants, as well as the plants and critters in my colony. Selecting " + UI.FormatAsHotkey("[F3]") + " will open the Temperature Overlay where I can check for any hot or cold spots.\n\nI can use a Utility building like an Ice-E Fan or a Space Heater to make an area colder or warmer. However, I will have limited success changing the temperature of a room unless I build the area with insulating tiles to prevent cold or warm air from escaping.";
			}

			public class PLUMBING
			{
				public static LocString NAME = "Video: Plumbing and Ventilation";

				public static LocString TOOLTIP = "Notes on connecting buildings with pipes";

				public static LocString MESSAGEBODY = "When connecting pipes it is useful to have the Plumbing Overlay " + UI.FormatAsHotkey("[F6]") + " selected. Each building which requires plumbing must have their Building Intake connected to the Output Pipe from a source such as a Liquid Pump. Liquid Pumps must be submerged in liquid and attached to a power source to function.\n\nBuildings often output contaminated water which must flow out of the building through piping from the Output Pipe. The water can then be expelled through a Liquid Vent, or filtered through a Liquid Filter for reuse.";
			}

			public class NEW_AUTOMATION_WARNING
			{
				public static LocString NAME = "New Automation Port";

				public static LocString TOOLTIP = "This building has a new automation port and is unintentionally connected to an existing " + BUILDINGS.PREFABS.LOGICWIRE.NAME;
			}

			public class DTU
			{
				public static LocString NAME = "Tutorial: Duplicant Thermal Units";

				public static LocString TOOLTIP = "Notes on measuring heat energy";

				public static LocString MESSAGEBODY = "My Duplicants measure heat energy in Duplicant Thermal Units or DTU.\n\n1 DTU = 1055.06 J";
			}

			public class NOMESSAGES
			{
				public static LocString NAME = "";

				public static LocString TOOLTIP = "";
			}

			public class NOALERTS
			{
				public static LocString NAME = "";

				public static LocString TOOLTIP = "";
			}

			public class NEWTRAIT
			{
				public static LocString NAME = "{0} has developed a trait";

				public static LocString TOOLTIP = "{0} has developed the trait(s):\n    • {1}";
			}

			public class RESEARCHCOMPLETE
			{
				public static LocString NAME = "Research Complete";

				public static LocString MESSAGEBODY = "Eureka! We've discovered {0} Technology.\n\nNew buildings have become available:\n  • {1}";

				public static LocString TOOLTIP = "{0} research complete!";
			}

			public class WORLDDETECTED
			{
				public static LocString NAME = string.Concat("New ", UI.CLUSTERMAP.PLANETOID, " detected");

				public static LocString MESSAGEBODY = string.Concat("My Duplicants' astronomical efforts have uncovered a new ", UI.CLUSTERMAP.PLANETOID, ":\n{0}");

				public static LocString TOOLTIP = "{0} discovered";
			}

			public class SKILL_POINT_EARNED
			{
				public static LocString NAME = "{Duplicant} earned a skill point!";

				public static LocString MESSAGEBODY = "These Duplicants have Skill Points that can be spent on new abilities:\n{0}";

				public static LocString LINE = "\n• <b>{0}</b>";

				public static LocString TOOLTIP = "{Duplicant} has been working hard and is ready to learn a new skill";
			}

			public class DUPLICANTABSORBED
			{
				public static LocString NAME = "Printables have been reabsorbed";

				public static LocString MESSAGEBODY = "The Printing Pod is no longer available for printing.\nCountdown to the next production has been rebooted.";

				public static LocString TOOLTIP = "Printing countdown rebooted";
			}

			public class DUPLICANTDIED
			{
				public static LocString NAME = "Duplicants have died";

				public static LocString TOOLTIP = "These Duplicants have died:";
			}

			public class FOODROT
			{
				public static LocString NAME = "Food has decayed";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have rotted and are no longer edible:{0}";
			}

			public class FOODSTALE
			{
				public static LocString NAME = "Food has become stale";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items have become stale and could rot if not stored:";
			}

			public class YELLOWALERT
			{
				public static LocString NAME = "Yellow Alert";

				public static LocString TOOLTIP = "The colony has some top priority tasks to complete before resuming a normal schedule";
			}

			public class REDALERT
			{
				public static LocString NAME = "Red Alert";

				public static LocString TOOLTIP = "The colony is prioritizing work over their individual well-being";
			}

			public class REACTORMELTDOWN
			{
				public static LocString NAME = "Reactor Meltdown";

				public static LocString TOOLTIP = "A Research Reactor has overheated and is melting down! Extreme radiation is flooding the area";
			}

			public class HEALING
			{
				public static LocString NAME = "Healing";

				public static LocString TOOLTIP = "This Duplicant is recovering from an injury";
			}

			public class UNREACHABLEITEM
			{
				public static LocString NAME = "Unreachable resources";

				public static LocString TOOLTIP = "Duplicants cannot retrieve these resources:";
			}

			public class INVALIDCONSTRUCTIONLOCATION
			{
				public static LocString NAME = "Invalid construction location";

				public static LocString TOOLTIP = "These buildings cannot be constructed in the planned areas:";
			}

			public class MISSINGMATERIALS
			{
				public static LocString NAME = "Missing materials";

				public static LocString TOOLTIP = "These resources are not available:";
			}

			public class BUILDINGOVERHEATED
			{
				public static LocString NAME = "Damage: Overheated";

				public static LocString TOOLTIP = "Extreme heat is damaging these buildings:";
			}

			public class TILECOLLAPSE
			{
				public static LocString NAME = "Ceiling Collapse!";

				public static LocString TOOLTIP = "Falling material fell on these Duplicants and displaced them:";
			}

			public class NO_OXYGEN_GENERATOR
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Oxygen Generator", "OXYGEN") + " built";

				public static LocString TOOLTIP = "My colony is not producing any new " + UI.FormatAsLink("Oxygen", "OXYGEN") + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Oxygen Diffusers", "MINERALDEOXIDIZER") + " can be built from the " + UI.FormatAsBuildMenuTab("Oxygen Tab", "[2]");
			}

			public class INSUFFICIENTOXYGENLASTCYCLE
			{
				public static LocString NAME = "Insufficient Oxygen generation";

				public static LocString TOOLTIP = "My colony is consuming more " + UI.FormatAsLink("Oxygen", "OXYGEN") + " than it is producing, and will run out air if I do not increase production.\n\nI should check my existing oxygen production buildings to ensure they're operating correctly" + UI.HORIZONTAL_BR_RULE + "• " + UI.FormatAsLink("Oxygen", "OXYGEN") + " produced last cycle: {EmittingRate}\n• Consumed last cycle: {ConsumptionRate}";
			}

			public class UNREFRIGERATEDFOOD
			{
				public static LocString NAME = "Unrefrigerated Food";

				public static LocString TOOLTIP = "These " + UI.FormatAsLink("Food", "FOOD") + " items are stored but not refrigerated:\n";
			}

			public class FOODLOW
			{
				public static LocString NAME = "Food shortage";

				public static LocString TOOLTIP = "The colony's " + UI.FormatAsLink("Food", "FOOD") + " reserves are low:" + UI.HORIZONTAL_BR_RULE + "    • {0} are currently available\n    • {1} is being consumed per cycle\n\n" + UI.FormatAsLink("Microbe Mushers", "MICROBEMUSHER") + " can be built from the " + UI.FormatAsBuildMenuTab("Food Tab", "[4]");
			}

			public class NO_MEDICAL_COTS
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Sick Bay", "DOCTORSTATION") + " built";

				public static LocString TOOLTIP = "There is nowhere for sick Duplicants receive medical care" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Sick Bays", "DOCTORSTATION") + " can be built from the " + UI.FormatAsBuildMenuTab("Medicine Tab", "[8]");
			}

			public class NEEDTOILET
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Outhouse", "OUTHOUSE") + " built";

				public static LocString TOOLTIP = "My Duplicants have nowhere to relieve themselves" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Outhouses", "OUTHOUSE") + " can be built from the " + UI.FormatAsBuildMenuTab("Plumbing Tab", "[5]");
			}

			public class NEEDFOOD
			{
				public static LocString NAME = "Colony requires a food source";

				public static LocString TOOLTIP = "The colony will exhaust their supplies without a new " + UI.FormatAsLink("Food", "FOOD") + " source" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Microbe Mushers", "MICROBEMUSHER") + " can be built from the " + UI.FormatAsBuildMenuTab("Food Tab", "[4]");
			}

			public class HYGENE_NEEDED
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Wash Basin", "WASHBASIN") + " built";

				public static LocString TOOLTIP = UI.FormatAsLink("Germs", "DISEASE") + " are spreading in the colony because my Duplicants have nowhere to clean up" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Wash Basins", "WASHBASIN") + " can be built from the " + UI.FormatAsBuildMenuTab("Medicine Tab", "[8]");
			}

			public class NEEDSLEEP
			{
				public static LocString NAME = "No " + UI.FormatAsLink("Cots", "COT") + " built";

				public static LocString TOOLTIP = "My Duplicants would appreciate a place to sleep" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Cots", "COTS") + " can be built from the " + UI.FormatAsBuildMenuTab("Furniture Tab", "[9]");
			}

			public class NEEDENERGYSOURCE
			{
				public static LocString NAME = "Colony requires a " + UI.FormatAsLink("Power", "POWER") + " source";

				public static LocString TOOLTIP = UI.FormatAsLink("Power", "POWER") + " is required to operate electrical buildings" + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Manual Generators", "MANUALGENERATOR") + " and " + UI.FormatAsLink("Wire", "WIRE") + " can be built from the " + UI.FormatAsLink("Power Tab", "[3]");
			}

			public class RESOURCEMELTED
			{
				public static LocString NAME = "Resources melted";

				public static LocString TOOLTIP = "These resources have melted:";
			}

			public class VENTOVERPRESSURE
			{
				public static LocString NAME = "Vent overpressurized";

				public static LocString TOOLTIP = "These " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " systems have exited the ideal " + UI.PRE_KEYWORD + "Pressure" + UI.PST_KEYWORD + " range:";
			}

			public class VENTBLOCKED
			{
				public static LocString NAME = "Vent blocked";

				public static LocString TOOLTIP = "Blocked " + UI.PRE_KEYWORD + "Pipes" + UI.PST_KEYWORD + " have stopped these systems from functioning:";
			}

			public class OUTPUTBLOCKED
			{
				public static LocString NAME = "Output blocked";

				public static LocString TOOLTIP = "Blocked " + UI.PRE_KEYWORD + "Pipes" + UI.PST_KEYWORD + " have stopped these systems from functioning:";
			}

			public class BROKENMACHINE
			{
				public static LocString NAME = "Building broken";

				public static LocString TOOLTIP = "These buildings have taken significant damage and are nonfunctional:";
			}

			public class STRUCTURALDAMAGE
			{
				public static LocString NAME = "Structural damage";

				public static LocString TOOLTIP = "These buildings' structural integrity has been compromised";
			}

			public class STRUCTURALCOLLAPSE
			{
				public static LocString NAME = "Structural collapse";

				public static LocString TOOLTIP = "These buildings have collapsed:";
			}

			public class GASCLOUDWARNING
			{
				public static LocString NAME = "A gas cloud approaches";

				public static LocString TOOLTIP = "A toxic " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " cloud will soon envelop the colony";
			}

			public class GASCLOUDARRIVING
			{
				public static LocString NAME = "The colony is entering a cloud of gas";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDPEAK
			{
				public static LocString NAME = "The gas cloud is at its densest point";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDDEPARTING
			{
				public static LocString NAME = "The gas cloud is receding";

				public static LocString TOOLTIP = "";
			}

			public class GASCLOUDGONE
			{
				public static LocString NAME = "The colony is once again in open space";

				public static LocString TOOLTIP = "";
			}

			public class AVAILABLE
			{
				public static LocString NAME = "Resource available";

				public static LocString TOOLTIP = "These resources have become available:";
			}

			public class ALLOCATED
			{
				public static LocString NAME = "Resource allocated";

				public static LocString TOOLTIP = "These resources are reserved for a planned building:";
			}

			public class INCREASEDEXPECTATIONS
			{
				public static LocString NAME = "Duplicants' expectations increased";

				public static LocString TOOLTIP = "Duplicants require better amenities over time.\nThese Duplicants have increased their expectations:";
			}

			public class NEARLYDRY
			{
				public static LocString NAME = "Nearly dry";

				public static LocString TOOLTIP = "These Duplicants will dry off soon:";
			}

			public class IMMIGRANTSLEFT
			{
				public static LocString NAME = "Printables have been reabsorbed";

				public static LocString TOOLTIP = "The care packages have been disintegrated and printable Duplicants have been Oozed";
			}

			public class LEVELUP
			{
				public static LocString NAME = "Attribute increase";

				public static LocString TOOLTIP = "These Duplicants' " + UI.PRE_KEYWORD + "Attributes" + UI.PST_KEYWORD + " have improved:";

				public static LocString SUFFIX = " - {0} Skill Level modifier raised to +{1}";
			}

			public class RESETSKILL
			{
				public static LocString NAME = "Skills reset";

				public static LocString TOOLTIP = "These Duplicants have had their " + UI.PRE_KEYWORD + "Skill Points" + UI.PST_KEYWORD + " refunded:";
			}

			public class BADROCKETPATH
			{
				public static LocString NAME = "Flight Path Obstructed";

				public static LocString TOOLTIP = "A rocket's flight path has been interrupted by a new astronomical discovery.\nOpen the " + UI.PRE_KEYWORD + "Starmap Screen" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[Z]") + " to reassign rocket paths";
			}

			public class SCHEDULE_CHANGED
			{
				public static LocString NAME = "{0}: {1}!";

				public static LocString TOOLTIP = "Duplicants assigned to " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + " have started their <b>{1}</b> block.\n\n{2}\n\nOpen the " + UI.PRE_KEYWORD + "Schedule Screen" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[PERIOD KEY]") + " to change blocks or assignments";
			}

			public class GENESHUFFLER
			{
				public static LocString NAME = "Genes Shuffled";

				public static LocString TOOLTIP = "These Duplicants had their genetic makeup modified:";

				public static LocString SUFFIX = " has developed " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD;
			}

			public class HEALINGTRAITGAIN
			{
				public static LocString NAME = "New trait";

				public static LocString TOOLTIP = "These Duplicants' injuries weren't set and healed improperly.\nThey developed " + UI.PRE_KEYWORD + "Traits" + UI.PST_KEYWORD + " as a result:";

				public static LocString SUFFIX = " has developed " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD;
			}

			public class COLONYLOST
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "All Duplicants are dead or incapacitated";
			}

			public class FABRICATOREMPTY
			{
				public static LocString NAME = "Fabricator idle";

				public static LocString TOOLTIP = "These fabricators have no recipes queued:";
			}

			public class SUIT_DROPPED
			{
				public static LocString NAME = "No Docks available";

				public static LocString TOOLTIP = "An exosuit was dropped because there were no empty docks available";
			}

			public class DEATH_SUFFOCATION
			{
				public static LocString NAME = "Duplicants suffocated";

				public static LocString TOOLTIP = string.Concat("These Duplicants died from a lack of ", ELEMENTS.OXYGEN.NAME, ":");
			}

			public class DEATH_FROZENSOLID
			{
				public static LocString NAME = "Duplicants have frozen";

				public static LocString TOOLTIP = "These Duplicants died from extremely low " + UI.PRE_KEYWORD + "Temperatures" + UI.PST_KEYWORD + ":";
			}

			public class DEATH_OVERHEATING
			{
				public static LocString NAME = "Duplicants have overheated";

				public static LocString TOOLTIP = "These Duplicants died from extreme " + UI.PRE_KEYWORD + "Heat" + UI.PST_KEYWORD + ":";
			}

			public class DEATH_STARVATION
			{
				public static LocString NAME = "Duplicants have starved";

				public static LocString TOOLTIP = "These Duplicants died from a lack of " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + ":";
			}

			public class DEATH_FELL
			{
				public static LocString NAME = "Duplicants splattered";

				public static LocString TOOLTIP = "These Duplicants fell to their deaths:";
			}

			public class DEATH_CRUSHED
			{
				public static LocString NAME = "Duplicants crushed";

				public static LocString TOOLTIP = "These Duplicants have been crushed:";
			}

			public class DEATH_SUFFOCATEDTANKEMPTY
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants were unable to reach " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and died:";
			}

			public class DEATH_SUFFOCATEDAIRTOOHOT
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.PRE_KEYWORD + "Hot" + UI.PST_KEYWORD + " air:";
			}

			public class DEATH_SUFFOCATEDAIRTOOCOLD
			{
				public static LocString NAME = "Duplicants have suffocated";

				public static LocString TOOLTIP = "These Duplicants have asphyxiated in " + UI.FormatAsLink("Cold", "HEAT") + " air:";
			}

			public class DEATH_DROWNED
			{
				public static LocString NAME = "Duplicants have drowned";

				public static LocString TOOLTIP = "These Duplicants have drowned:";
			}

			public class DEATH_ENTOUMBED
			{
				public static LocString NAME = "Duplicants have been entombed";

				public static LocString TOOLTIP = "These Duplicants are trapped and need assistance:";
			}

			public class DEATH_RAPIDDECOMPRESSION
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a low pressure environment:";
			}

			public class DEATH_OVERPRESSURE
			{
				public static LocString NAME = "Duplicants pressurized";

				public static LocString TOOLTIP = "These Duplicants died in a high pressure environment:";
			}

			public class DEATH_POISONED
			{
				public static LocString NAME = "Duplicants poisoned";

				public static LocString TOOLTIP = "These Duplicants died as a result of poisoning:";
			}

			public class DEATH_DISEASE
			{
				public static LocString NAME = "Duplicants have succumbed to disease";

				public static LocString TOOLTIP = "These Duplicants died from an untreated " + UI.FormatAsLink("Disease", "DISEASE") + ":";
			}

			public class CIRCUIT_OVERLOADED
			{
				public static LocString NAME = "Circuit Overloaded";

				public static LocString TOOLTIP = string.Concat("These ", BUILDINGS.PREFABS.WIRE.NAME, "s melted due to excessive current demands on their circuits");
			}

			public class LOGIC_CIRCUIT_OVERLOADED
			{
				public static LocString NAME = "Logic Circuit Overloaded";

				public static LocString TOOLTIP = string.Concat("These ", BUILDINGS.PREFABS.LOGICWIRE.NAME, "s melted due to more bits of data being sent over them than they can support");
			}

			public class DISCOVERED_SPACE
			{
				public static LocString NAME = "ALERT - Surface Breach";

				public static LocString TOOLTIP = "Amazing!\n\nMy Duplicants have managed to breach the surface of our rocky prison.\n\nI should be careful; the region is extremely inhospitable and I could easily lose resources to the vacuum of space.";
			}

			public class COLONY_ACHIEVEMENT_EARNED
			{
				public static LocString NAME = "Colony Achievement earned";

				public static LocString TOOLTIP = "The colony has earned a new achievement.";
			}

			public class WARP_PORTAL_DUPE_READY
			{
				public static LocString NAME = "Duplicant warp ready";

				public static LocString TOOLTIP = "{dupe} is ready to warp from the " + BUILDINGS.PREFABS.WARPPORTAL.NAME;
			}

			public class GENETICANALYSISCOMPLETE
			{
				public static LocString NAME = "Seed Analysis Complete";

				public static LocString MESSAGEBODY = "Deeply probing the genes of the {Plant} plant have led to the discovery of a promising new cultivatable mutation:\n\n<b>{Subspecies}</b>\n\n{Info}";

				public static LocString TOOLTIP = "{Plant} Analysis complete!";
			}

			public class NEWMUTANTSEED
			{
				public static LocString NAME = "New Mutant Seed Discovered";

				public static LocString TOOLTIP = string.Concat("A new mutant variety of the {Plant} has been found. Analyze it at the ", BUILDINGS.PREFABS.GENETICANALYSISSTATION.NAME, " to learn more!");
			}

			public class DUPLICANT_CRASH_LANDED
			{
				public static LocString NAME = "Duplicant Crash Landed!";

				public static LocString TOOLTIP = "A Duplicant in an escape pod has made it to the surface of the nearest asteroid.";
			}
		}

		public class TUTORIAL
		{
			public static LocString DONT_SHOW_AGAIN = "Don't Show Again";
		}

		public class PLACERS
		{
			public class DIGPLACER
			{
				public static LocString NAME = "Dig";
			}

			public class MOPPLACER
			{
				public static LocString NAME = "Mop";
			}
		}

		public class MONUMENT_COMPLETE
		{
			public static LocString NAME = "Great Monument";

			public static LocString DESC = "A feat of artistic vision and expert engineering that will doubtless inspire Duplicants for thousands of cycles to come";
		}
	}
}
