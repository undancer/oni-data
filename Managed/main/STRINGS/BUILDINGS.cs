namespace STRINGS
{
	public class BUILDINGS
	{
		public class PREFABS
		{
			public class HEADQUARTERSCOMPLETE
			{
				public static LocString NAME = UI.FormatAsLink("Printing Pod", "HEADQUARTERS");

				public static LocString UNIQUE_POPTEXT = "A clone of the cloning machine? What a novel thought.\n\nAlas, it won't work.";
			}

			public class EXOBASEHEADQUARTERS
			{
				public static LocString NAME = UI.FormatAsLink("Mini-Pod", "EXOBASEHEADQUARTERS");

				public static LocString DESC = "A quick and easy substitute, though it'll never live up to the original.";

				public static LocString EFFECT = "A portable bioprinter that produces new Duplicants or care packages containing resources.\n\nOnly one Printing Pod or Mini-Pod is permitted per Planetoid.";
			}

			public class AIRCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Regulator", "AIRCONDITIONER");

				public static LocString DESC = "A thermo regulator doesn't remove heat, but relocates it to a new area.";

				public static LocString EFFECT = "Cools the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " piped through it, but outputs " + UI.FormatAsLink("Heat", "HEAT") + " in its immediate vicinity.";
			}

			public class STATERPILLAREGG
			{
				public static LocString NAME = UI.FormatAsLink("Slug Egg", "STATERPILLAREGG");

				public static LocString DESC = "The electrifying egg of the " + UI.FormatAsLink("Plug Slug", "STATERPILLAR") + ".";

				public static LocString EFFECT = "Slug Eggs can be connected to a " + UI.FormatAsLink("Power", "POWER") + " circuit as an energy source.";
			}

			public class STATERPILLARGENERATOR
			{
				public class MODIFIERS
				{
					public static LocString WILD = "Wild!";

					public static LocString HUNGRY = "Hungry!";
				}

				public static LocString NAME = UI.FormatAsLink("Plug Slug", "STATERPILLAR");
			}

			public class BEEHIVE
			{
				public static LocString NAME = UI.FormatAsLink("Beeta Hive", "BEEHIVE");

				public static LocString DESC = "A moderately " + UI.FormatAsLink("Radioactive", "RADIATION") + " nest made by " + UI.FormatAsLink("Beetas", "BEE") + ".\n\nConverts " + UI.FormatAsLink("Uranium", "URANIUMORE") + " into " + UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM") + " when worked by a Beeta.\nWill not function if ground below has been destroyed.";

				public static LocString EFFECT = "The cozy home of a Beeta.";
			}

			public class ETHANOLDISTILLERY
			{
				public static LocString NAME = UI.FormatAsLink("Ethanol Distiller", "ETHANOLDISTILLERY");

				public static LocString DESC = string.Concat("Ethanol distillers convert ", ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME, " into burnable ", ELEMENTS.ETHANOL.NAME, " fuel.");

				public static LocString EFFECT = string.Concat("Refines ", ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME, " into ", UI.FormatAsLink("Ethanol", "ETHANOL"), ".");
			}

			public class ALGAEDISTILLERY
			{
				public static LocString NAME = UI.FormatAsLink("Algae Distiller", "ALGAEDISTILLERY");

				public static LocString DESC = "Algae distillers convert disease-causing slime into algae for oxygen production.";

				public static LocString EFFECT = "Refines " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " into " + UI.FormatAsLink("Algae", "ALGAE") + ".";
			}

			public class OXYLITEREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Oxylite Refinery", "OXYLITEREFINERY");

				public static LocString DESC = "Oxylite is a solid and easily transportable source of consumable oxygen.";

				public static LocString EFFECT = "Synthesizes " + UI.FormatAsLink("Oxylite", "OXYROCK") + " using " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and a small amount of " + UI.FormatAsLink("Gold", "GOLD") + ".";
			}

			public class FERTILIZERMAKER
			{
				public static LocString NAME = UI.FormatAsLink("Fertilizer Synthesizer", "FERTILIZERMAKER");

				public static LocString DESC = "Fertilizer synthesizers convert polluted dirt into fertilizer for domestic plants.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " and " + UI.FormatAsLink("Phosphorite", "PHOSPHORITE") + " to produce " + UI.FormatAsLink("Fertilizer", "FERTILIZER") + ".";
			}

			public class ALGAEHABITAT
			{
				public static LocString NAME = UI.FormatAsLink("Algae Terrarium", "ALGAEHABITAT");

				public static LocString DESC = "Algae colony, Duplicant colony... we're more alike than we are different.";

				public static LocString EFFECT = "Consumes " + UI.FormatAsLink("Algae", "ALGAE") + " to produce " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and remove some " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".\n\nGains a 10% efficiency boost in direct " + UI.FormatAsLink("Light", "LIGHT") + ".";

				public static LocString SIDESCREEN_TITLE = "Empty " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " Threshold";
			}

			public class BATTERY
			{
				public static LocString NAME = UI.FormatAsLink("Battery", "BATTERY");

				public static LocString DESC = "Batteries allow power from generators to be stored for later.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Power", "POWER") + " from generators, then provides that power to buildings.\n\nLoses charge over time.";

				public static LocString CHARGE_LOSS = "{Battery} charge loss";
			}

			public class FLYINGCREATUREBAIT
			{
				public static LocString NAME = UI.FormatAsLink("Airborne Critter Bait", "FLYINGCREATUREBAIT");

				public static LocString DESC = "The type of critter attracted by critter bait depends on the construction material.";

				public static LocString EFFECT = "Attracts one type of airborne critter.\n\nSingle use.";
			}

			public class AIRBORNECREATURELURE
			{
				public static LocString NAME = UI.FormatAsLink("Airborne Critter Lure", "AIRBORNECREATURELURE");

				public static LocString DESC = "Lures can relocate Pufts or Shine Bugs to specific locations in my colony.";

				public static LocString EFFECT = "Attracts one type of airborne critter at a time.\n\nMust be baited with " + UI.FormatAsLink("Slime", "SLIMEMOLD") + " or " + UI.FormatAsLink("Phosphorite", "PHOSPHORITE") + ".";
			}

			public class BATTERYMEDIUM
			{
				public static LocString NAME = UI.FormatAsLink("Jumbo Battery", "BATTERYMEDIUM");

				public static LocString DESC = "Larger batteries hold more power and keep systems running longer before recharging.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Power", "POWER") + " from generators, then provides that power to buildings.\n\nSlightly loses charge over time.";
			}

			public class BATTERYSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Battery", "BATTERYSMART");

				public static LocString DESC = "Smart batteries send a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when they require charging.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Power", "POWER") + " from generators, then provides that power to buildings.\n\nSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the configuration of the Logic Activation Parameters.\n\nVery slightly loses charge over time.";

				public static LocString LOGIC_PORT = "Charge Parameters";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when battery is less than <b>Low Threshold</b> charged, until <b>High Threshold</b> is reached again";

				public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the battery is more than <b>High Threshold</b> charged, until <b>Low Threshold</b> is reached again";

				public static LocString ACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when battery is less than <b>{0}%</b> charged, until it is <b>{1}% (High Threshold)</b> charged";

				public static LocString DEACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when battery is <b>{0}%</b> charged, until it is less than <b>{1}% (Low Threshold)</b> charged";

				public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";

				public static LocString SIDESCREEN_ACTIVATE = "Low Threshold:";

				public static LocString SIDESCREEN_DEACTIVATE = "High Threshold:";
			}

			public class BED
			{
				public static LocString NAME = UI.FormatAsLink("Cot", "BED");

				public static LocString DESC = "Duplicants without a bed will develop sore backs from sleeping on the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to sleep.\n\nDuplicants will automatically return to their cots to sleep at night.";
			}

			public class BOTTLEEMPTIER
			{
				public static LocString NAME = UI.FormatAsLink("Bottle Emptier", "BOTTLEEMPTIER");

				public static LocString DESC = "A bottle emptier's Element Filter can be used to designate areas for specific liquid storage.";

				public static LocString EFFECT = "Empties bottled " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " back into the world.";
			}

			public class BOTTLEEMPTIERGAS
			{
				public static LocString NAME = UI.FormatAsLink("Canister Emptier", "BOTTLEEMPTIERGAS");

				public static LocString DESC = "A canister emptier's Element Filter can designate areas for specific gas storage.";

				public static LocString EFFECT = "Empties " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " canisters back into the world.";
			}

			public class ARTIFACTCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Artifact Transport Module", "ARTIFACTCARGOBAY");

				public static LocString DESC = "Holds artifacts found in space.";

				public static LocString EFFECT = "Allows Duplicants to store any artifacts they uncover during space missions.\n\nArtifacts become available to the colony upon the rocket's return.";
			}

			public class CARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Cargo Bay", "CARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo bays with any resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class CARGOBAYCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Large Cargo Bay", "CARGOBAY");

				public static LocString DESC = "Holds more than a regular cargo bay.";

				public static LocString EFFECT = "Allows Duplicants to store most of the " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class SOLIDCARGOBAYSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Cargo Bay", "SOLIDCARGOBAYSMALL");

				public static LocString DESC = "Duplicants will fill cargo bays with any resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store some of the " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class SPECIALCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Biological Cargo Bay", "SPECIALCARGOBAY");

				public static LocString DESC = "Biological cargo bays allow Duplicants to retrieve alien plants and wildlife from space.";

				public static LocString EFFECT = "Allows Duplicants to store unusual or organic resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class COMMANDMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Command Capsule", "COMMANDMODULE");

				public static LocString DESC = "At least one astronaut must be assigned to the command module to pilot a rocket.";

				public static LocString EFFECT = "Contains passenger seating for Duplicant " + UI.FormatAsLink("Astronauts", "ASTRONAUT") + ".\n\nA Command Capsule must be the last module installed at the top of a rocket.";

				public static LocString LOGIC_PORT_READY = "Rocket Checklist";

				public static LocString LOGIC_PORT_READY_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when its rocket launch checklist is complete";

				public static LocString LOGIC_PORT_READY_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_LAUNCH = "Launch Rocket";

				public static LocString LOGIC_PORT_LAUNCH_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Launch rocket";

				public static LocString LOGIC_PORT_LAUNCH_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Awaits launch command";
			}

			public class CLUSTERCOMMANDMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Command Capsule", "CLUSTERCOMMANDMODULE");

				public static LocString DESC = "";

				public static LocString EFFECT = "";

				public static LocString LOGIC_PORT_READY = "Rocket Checklist";

				public static LocString LOGIC_PORT_READY_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when its rocket launch checklist is complete";

				public static LocString LOGIC_PORT_READY_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_LAUNCH = "Launch Rocket";

				public static LocString LOGIC_PORT_LAUNCH_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Launch rocket";

				public static LocString LOGIC_PORT_LAUNCH_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Awaits launch command";
			}

			public class CLUSTERCRAFTINTERIORDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Interior Hatch", "CLUSTERCRAFTINTERIORDOOR");

				public static LocString DESC = "A hatch for getting in and out of the rocket.";

				public static LocString EFFECT = "Warning: Do not open mid-flight.";
			}

			public class ROCKETCONTROLSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Control Station", "ROCKETCONTROLSTATION");

				public static LocString DESC = "Someone needs to be around to jiggle the controls when the screensaver comes on.";

				public static LocString EFFECT = "Allows Duplicants to use pilot-operated rockets.\n\nAssigned Duplicants must have the " + UI.FormatAsLink("Rocket Piloting", "PILOT") + " skill.";
			}

			public class RESEARCHMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Research Module", "RESEARCHMODULE");

				public static LocString DESC = "Data banks can be used at virtual planetariums to produce additional research.";

				public static LocString EFFECT = "Completes one " + UI.FormatAsLink("Research Task", "RESEARCH") + " per space mission.\n\nProduces a small Data Bank regardless of mission destination.\n\nGenerated " + UI.FormatAsLink("Research Points", "RESEARCH") + " become available upon the rocket's return.";
			}

			public class TOURISTMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Sight-Seeing Module", "TOURISTMODULE");

				public static LocString DESC = "An astronaut must accompany sight seeing Duplicants on rocket flights.";

				public static LocString EFFECT = "Allows one non-Astronaut Duplicant to visit space.\n\nSight-Seeing Rocket flights decrease " + UI.FormatAsLink("Stress", "STRESS") + ".";
			}

			public class SCANNERMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Cartographic Module", "SCANNERMODULE");

				public static LocString DESC = "Allows Duplicants to boldly go where other Duplicants haven't been yet.";

				public static LocString EFFECT = "Automatically analyzes adjacent space while on a voyage.";
			}

			public class HABITATMODULESMALL
			{
				public static LocString NAME = UI.FormatAsLink("Solo Spacefarer Nosecone", "HABITATMODULESMALL");

				public static LocString DESC = "One lucky Duplicant gets the best view from the whole rocket.";

				public static LocString EFFECT = "Functions as a Command Module and a Nosecone.\n\nHolds one Duplicant traveller.\n\nOne Command Module may be installed per rocket.\n\nMust be built at the top of a rocket.";
			}

			public class HABITATMODULEMEDIUM
			{
				public static LocString NAME = UI.FormatAsLink("Spacefarer Module", "HABITATMODULEMEDIUM");

				public static LocString DESC = "Duplicants can survive space travel inside this protective nosecone... Hopefully.";

				public static LocString EFFECT = "Functions as a Command Module.\n\nHolds up to ten Duplicant travellers.\n\nOne Command Module may be installed per rocket.";
			}

			public class NOSECONEBASIC
			{
				public static LocString NAME = UI.FormatAsLink("Basic Nosecone", "NOSECONEBASIC");

				public static LocString DESC = "Every rocket requires a nosecone to fly.";

				public static LocString EFFECT = "Protects a rocket during takeoff and entry, enabling space travel.\n\nMust be built at the top of a rocket.";
			}

			public class NOSECONEHARVEST
			{
				public static LocString NAME = UI.FormatAsLink("Drillcone", "NOSECONEHARVEST");

				public static LocString DESC = "Harvests resources from the universe.";

				public static LocString EFFECT = "Enables a rocket to drill into interstellar debris and collect " + UI.FormatAsLink("gas", "ELEMENTS_GAS") + ", " + UI.FormatAsLink("liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("solid", "ELEMENTS_SOLID") + " resources from space.\n\nMust be built at the top of a rocket with " + UI.FormatAsLink("gas", "ELEMENTS_GAS") + ", " + UI.FormatAsLink("liquid", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("solid", "ELEMENTS_SOLID") + " Cargo Module attached to store the appropriate resources.";
			}

			public class CO2ENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Carbon Dioxide Engine", "CO2ENGINE");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Uses pressurized " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " to propel rockets for short range space exploration.\n\nCarbon Dioxide Engines are relatively fast engine for their size but with limited height restrictions.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class KEROSENEENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Engine", "KEROSENEENGINE");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " to propel rockets for mid-range space exploration.\n\nPetroleum Engines have generous height restrictions, ideal for hauling many modules.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class KEROSENEENGINECLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Engine", "KEROSENEENGINECLUSTER");

				public static LocString DESC = "More powerful rocket engines can propel heavier burdens.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " to propel rockets for mid-range space exploration.\n\nPetroleum Engines have generous height restrictions, ideal for hauling many modules.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class KEROSENEENGINECLUSTERSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Small Petroleum Engine", "KEROSENEENGINECLUSTERSMALL");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " to propel rockets for mid-range space exploration.\n\nSmall Petroleum Engines possess the same speed as a " + UI.FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but have smaller height restrictions.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class HYDROGENENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Engine", "HYDROGENENGINE");

				public static LocString DESC = "Hydrogen engines can propel rockets further than steam or petroleum engines.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Liquid Hydrogen", "LIQUIDHYDROGEN") + " to propel rockets for long-range space exploration.\n\nHydrogen Engines have the same generous height restrictions as " + UI.FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but are slightly faster.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class HYDROGENENGINECLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Engine", "HYDROGENENGINE");

				public static LocString DESC = "Hydrogen engines can propel rockets further than steam or petroleum engines.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Liquid Hydrogen", "LIQUIDHYDROGEN") + " to propel rockets for long-range space exploration.\n\nHydrogen Engines have the same generous height restrictions as " + UI.FormatAsLink("Petroleum Engines", "KEROSENEENGINE") + " but are slightly faster.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class SUGARENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Sugar Engine", "SUGARENGINE");

				public static LocString DESC = "Not the most stylish way to travel space, but certainly the tastiest.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Sucrose", "SUCROSE") + " to propel rockets for short range space exploration.\n\nSugar Engines have higher height restrictions than " + UI.FormatAsLink("Carbon Dioxide Engines", "CO2ENGINE") + ", but move slower.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class HEPENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Radbolt Engine", "HEPENGINE");

				public static LocString DESC = "Radbolt-fueled rockets support few modules, but travel exceptionally far.";

				public static LocString EFFECT = "Injects " + UI.FormatAsLink("Radbolts", "HEP") + " into a reaction chamber to propel rockets for long-range space exploration.\n\nRadbolt Engines are faster than " + UI.FormatAsLink("Hydrogen Engines", "HYDROGENENGINE") + " but with a more restrictive height allowance.\n\nThe engine of a rocket must be built first before more rocket modules can be added.";
			}

			public class ORBITALCARGOMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Orbital Cargo Module", "ORBITALCARGOMODULE");

				public static LocString DESC = "It's a generally good idea to pack some supplies when exploring unknown worlds.";

				public static LocString EFFECT = string.Concat("Delivers cargo to the surface of Planetoids that do not yet have a ", LAUNCHPAD.NAME, ".");
			}

			public class BATTERYMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Battery Module", "BATTERYMODULE");

				public static LocString DESC = "Charging a battery module before takeoff makes it easier to power buildings during flight.";

				public static LocString EFFECT = string.Concat("Stores the excess ", UI.FormatAsLink("Power", "POWER"), " generated by a Rocket Engine or ", LAUNCHPAD.NAME, ".\n\nProvides stored power to ", UI.FormatAsLink("Interior Rocket Outlets", "ROCKETINTERIORPOWERPLUG"), ".\n\nLoses charge over time.");
			}

			public class PIONEERMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Trailblazer Module", "PIONEERMODULE");

				public static LocString DESC = "That's one small step for Dupekind.";

				public static LocString EFFECT = string.Concat("Enables travel to Planetoids that do not yet have a ", LAUNCHPAD.NAME, ".\n\nCan hold one Duplicant traveller.\n\nDeployment is available while in a Starmap hex adjacent to a Planetoid.");
			}

			public class SOLARPANELMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Solar Panel Module", "SOLARPANELMODULE");

				public static LocString DESC = "Collect solar energy before takeoff and during flight.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Sunlight", "LIGHT") + " into electrical " + UI.FormatAsLink("Power", "POWER") + " for use on rockets.\n\nMust be exposed to space.";
			}

			public class SCOUTMODULE
			{
				public static LocString NAME = UI.FormatAsLink("Rover's Module", "SCOUTMODULE");

				public static LocString DESC = "Rover can conduct explorations of planetoids that don't have rocket platforms built.";

				public static LocString EFFECT = "Deploys one " + UI.FormatAsLink("Rover Bot", "SCOUT") + " for remote Planetoid exploration.\n\nDeployment is available while in a Starmap hex adjacent to a Planetoid.";
			}

			public class PIONEERLANDER
			{
				public static LocString NAME = UI.FormatAsLink("Trailblazer Lander", "PIONEERLANDER");

				public static LocString DESC = string.Concat("Lands a Duplicant on a Planetoid from an orbiting ", PIONEERMODULE.NAME, ".");
			}

			public class SCOUTLANDER
			{
				public static LocString NAME = UI.FormatAsLink("Rover's Lander", "SCOUTLANDER");

				public static LocString DESC = string.Concat("Lands ", UI.FormatAsLink("Rover", "SCOUT"), " on a Planetoid when ", SCOUTMODULE.NAME, " is in orbit.");
			}

			public class GANTRY
			{
				public static LocString NAME = UI.FormatAsLink("Gantry", "GANTRY");

				public static LocString DESC = "A gantry can be built over rocket pieces where ladders and tile cannot.";

				public static LocString EFFECT = "Provides scaffolding across rocket modules to allow Duplicant access.";

				public static LocString LOGIC_PORT = "Extend/Retract";

				public static LocString LOGIC_PORT_ACTIVE = "<b>Extends gantry</b> when a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " signal is received";

				public static LocString LOGIC_PORT_INACTIVE = "<b>Retracts gantry</b> when a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " signal is received";
			}

			public class ROCKETINTERIORPOWERPLUG
			{
				public static LocString NAME = UI.FormatAsLink("Power Outlet Fitting", "ROCKETINTERIORPOWERPLUG");

				public static LocString DESC = "Outlets conveniently power buildings inside a cockpit using their rocket's power stores.";

				public static LocString EFFECT = "Provides " + UI.FormatAsLink("Power", "POWER") + " to connected buildings.\n\nPulls power from " + UI.FormatAsLink("Battery Modules", "BATTERYMODULE") + " and Rocket Engines.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORLIQUIDINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Intake Fitting", "ROCKETINTERIORLIQUIDINPUT");

				public static LocString DESC = "Begone, foul waters!";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " to be pumped into rocket storage via " + UI.FormatAsLink("Pipes", "LIQUIDCONDUIT") + ".\n\nSends liquid to the first Rocket Module with available space.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORLIQUIDOUTPUT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Output Fitting", "ROCKETINTERIORLIQUIDOUTPUT");

				public static LocString DESC = "Now if only we had some water balloons...";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " to be drawn from rocket storage via " + UI.FormatAsLink("Pipes", "LIQUIDCONDUIT") + ".\n\nDraws liquid from the first Rocket Module with the requested material.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORGASINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Intake Fitting", "ROCKETINTERIORGASINPUT");

				public static LocString DESC = "It's basically central-vac.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " to be pumped into rocket storage via " + UI.FormatAsLink("Pipes", "GASCONDUIT") + ".\n\nSends gas to the first Rocket Module with available space.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORGASOUTPUT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Output Fitting", "ROCKETINTERIORGASOUTPUT");

				public static LocString DESC = "Refreshing breezes, on-demand.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " to be drawn from rocket storage via " + UI.FormatAsLink("Pipes", "GASCONDUIT") + ".\n\nDraws gas from the first Rocket Module with the requested material.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORSOLIDINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Receptacle Fitting", "ROCKETINTERIORSOLIDINPUT");

				public static LocString DESC = "Why organize your shelves when you can just shove everything in here?";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " to be moved into rocket storage via " + UI.FormatAsLink("Conveyor Rails", "SOLIDCONDUIT") + ".\n\nSends solid material to the first Rocket Module with available space.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class ROCKETINTERIORSOLIDOUTPUT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Loader Fitting", "ROCKETINTERIORSOLIDOUTPUT");

				public static LocString DESC = "For accessing your stored luggage mid-flight.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " to be moved out of rocket storage via " + UI.FormatAsLink("Conveyor Rails", "SOLIDCONDUIT") + ".\n\nDraws solid material from the first Rocket Module with the requested material.\n\nMust be built within the interior of a Rocket Module.";
			}

			public class WATERCOOLER
			{
				public static LocString NAME = UI.FormatAsLink("Water Cooler", "WATERCOOLER");

				public static LocString DESC = "Chatting with friends improves Duplicants' moods and reduces their stress.";

				public static LocString EFFECT = "Provides a gathering place for Duplicants during Downtime.\n\nImproves Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class ARCADEMACHINE
			{
				public static LocString NAME = UI.FormatAsLink("Arcade Cabinet", "ARCADEMACHINE");

				public static LocString DESC = "Komet Kablam-O!\nFor up to two players.";

				public static LocString EFFECT = "Allows Duplicants to play video games on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class PHONOBOX
			{
				public static LocString NAME = UI.FormatAsLink("Jukebot", "PHONOBOX");

				public static LocString DESC = "Dancing helps Duplicants get their innermost feelings out.";

				public static LocString EFFECT = "Plays music for Duplicants to dance to on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class JUICER
			{
				public static LocString NAME = UI.FormatAsLink("Juicer", "JUICER");

				public static LocString DESC = "Fruity juice can really brighten a Duplicant's breaktime";

				public static LocString EFFECT = "Provides refreshment for Duplicants on their breaks.\n\nDrinking juice increases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class ESPRESSOMACHINE
			{
				public static LocString NAME = UI.FormatAsLink("Espresso Machine", "ESPRESSOMACHINE");

				public static LocString DESC = "A shot of espresso helps Duplicants relax after a long day.";

				public static LocString EFFECT = "Provides refreshment for Duplicants on their breaks.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class MODULARLIQUIDINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Input Hub", "MODULARLIQUIDINPUT");

				public static LocString DESC = "A hub from which to input " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + ".";
			}

			public class MODULARSOLIDINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Solid Input Hub", "MODULARSOLIDINPUT");

				public static LocString DESC = "A hub from which to input " + UI.FormatAsLink("Solids", "ELEMENTS_SOLID") + ".";
			}

			public class MODULARGASINPUT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Input Hub", "MODULARGASINPUT");

				public static LocString DESC = "A hub from which to input " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class MECHANICALSURFBOARD
			{
				public static LocString NAME = UI.FormatAsLink("Mechanical Surfboard", "MECHANICALSURFBOARD");

				public static LocString DESC = "Mechanical waves make for radical relaxation time.";

				public static LocString EFFECT = "Increases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nSome " + UI.FormatAsLink("Water", "WATER") + " gets splashed on the floor during use.";

				public static LocString WATER_REQUIREMENT = "{element}: {amount}";

				public static LocString WATER_REQUIREMENT_TOOLTIP = "This building must be filled with {amount} {element} in order to function.";

				public static LocString LEAK_REQUIREMENT = "Spillage: {amount}";

				public static LocString LEAK_REQUIREMENT_TOOLTIP = "This building will spill {amount} of its contents on to the floor during use, which must be replenished.";
			}

			public class SAUNA
			{
				public static LocString NAME = UI.FormatAsLink("Sauna", "SAUNA");

				public static LocString DESC = "A steamy sauna soothes away all the aches and pains.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Steam", "STEAM") + " to create a relaxing atmosphere.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class BEACHCHAIR
			{
				public static LocString NAME = UI.FormatAsLink("Beach Chair", "BEACHCHAIR");

				public static LocString DESC = "Soak up some relaxing sun rays.";

				public static LocString EFFECT = "Duplicants can relax by lounging in " + UI.FormatAsLink("Sunlight", "LIGHT") + ".\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";

				public static LocString LIGHTEFFECT_LOW = "{attrib}: {amount} (Dim Light)";

				public static LocString LIGHTEFFECT_LOW_TOOLTIP = "Duplicants will gain {amount} {attrib} if this building is in light dimmer than {lux}.";

				public static LocString LIGHTEFFECT_HIGH = "{attrib}: {amount} (Bright Light)";

				public static LocString LIGHTEFFECT_HIGH_TOOLTIP = "Duplicants will gain {amount} {attrib} if this building is in at least {lux} light.";
			}

			public class SUNLAMP
			{
				public static LocString NAME = UI.FormatAsLink("Sun Lamp", "SUNLAMP");

				public static LocString DESC = "An artificial ray of sunshine.";

				public static LocString EFFECT = "Gives off " + UI.FormatAsLink("Sunlight", "LIGHT") + " level Lux.\n\nCan be paired with " + UI.FormatAsLink("Beach Chairs", "BEACHCHAIR") + ".";
			}

			public class VERTICALWINDTUNNEL
			{
				public static LocString NAME = UI.FormatAsLink("Vertical Wind Tunnel", "VERTICALWINDTUNNEL");

				public static LocString DESC = "Duplicants love the feeling of high-powered wind through their hair.";

				public static LocString EFFECT = "Must be connected to a " + UI.FormatAsLink("Power Source", "POWER") + ". To properly function, the area under this building must be left vacant.\n\nIncreases Duplicants " + UI.FormatAsLink("Morale", "MORALE") + ".";

				public static LocString DISPLACEMENTEFFECT = "Gas Displacement: {amount}";

				public static LocString DISPLACEMENTEFFECT_TOOLTIP = "This building will displace {amount} Gas while in use.";
			}

			public class TELEPORTALPAD
			{
				public static LocString NAME = UI.FormatAsLink("Teleporter Pad", "TELEPORTALPAD");

				public static LocString DESC = "Duplicants are just atoms as far as the pad's concerned.";

				public static LocString EFFECT = "Instantly transports Duplicants and items to another portal with the same portal code.";

				public static LocString LOGIC_PORT = "Portal Code Input";

				public static LocString LOGIC_PORT_ACTIVE = "1";

				public static LocString LOGIC_PORT_INACTIVE = "0";
			}

			public class CHECKPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Duplicant Checkpoint", "CHECKPOINT");

				public static LocString DESC = "Checkpoints can be connected to automated sensors to determine when it's safe to enter.";

				public static LocString EFFECT = "Allows Duplicants to pass when receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".\n\nPrevents Duplicants from passing when receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".";

				public static LocString LOGIC_PORT = "Duplicant Stop/Go";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow Duplicant passage";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Prevent Duplicant passage";
			}

			public class FIREPOLE
			{
				public static LocString NAME = UI.FormatAsLink("Fire Pole", "FIREPOLE");

				public static LocString DESC = "Build these in addition to ladders for efficient upward and downward movement.";

				public static LocString EFFECT = "Allows rapid Duplicant descent.\n\nSignificantly slows upward climbing.";
			}

			public class FLOORSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Weight Plate", "FLOORSWITCH");

				public static LocString DESC = "Weight plates can be used to turn on amenities only when Duplicants pass by.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when an object or Duplicant is placed atop of it.\n\nCannot be triggered by " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " or " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + ".";

				public static LocString LOGIC_PORT_DESC = UI.FormatAsLink("Active", "LOGIC") + "/" + UI.FormatAsLink("Inactive", "LOGIC");
			}

			public class KILN
			{
				public static LocString NAME = UI.FormatAsLink("Kiln", "KILN");

				public static LocString DESC = "Kilns can also be used to refine coal into pure carbon.";

				public static LocString EFFECT = "Fires " + UI.FormatAsLink("Clay", "CLAY") + " to produce " + UI.FormatAsLink("Ceramic", "CERAMIC") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class LIQUIDFUELTANK
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Fuel Tank", "LIQUIDFUELTANK");

				public static LocString DESC = "Storing additional fuel increases the distance a rocket can travel before returning.";

				public static LocString EFFECT = "Stores the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " fuel piped into it to supply rocket engines.\n\nThe stored fuel type is determined by the rocket engine it is built upon.";
			}

			public class LIQUIDFUELTANKCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Large Liquid Fuel Tank", "LIQUIDFUELTANKCLUSTER");

				public static LocString DESC = "Storing additional fuel increases the distance a rocket can travel before returning.";

				public static LocString EFFECT = "Stores the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " fuel piped into it to supply rocket engines.\n\nThe stored fuel type is determined by the rocket engine it is built upon.";
			}

			public class LANDING_POD
			{
				public static LocString NAME = "Spacefarer Deploy Pod";

				public static LocString DESC = "Geronimo!";

				public static LocString EFFECT = "Contains a Duplicant deployed from orbit.\n\nPod will disintegrate on arrival.";
			}

			public class ROCKETPOD
			{
				public static LocString NAME = UI.FormatAsLink("Trailblazer Deploy Pod", "ROCKETPOD");

				public static LocString DESC = "The Duplicant inside is equal parts nervous and excited.";

				public static LocString EFFECT = string.Concat("Contains a Duplicant deployed from orbit by a ", PIONEERMODULE.NAME, ".\n\nPod will disintegrate on arrival.");
			}

			public class SCOUTROCKETPOD
			{
				public static LocString NAME = UI.FormatAsLink("Rover's Doghouse", "SCOUTROCKETPOD");

				public static LocString DESC = "Good luck out there, boy!";

				public static LocString EFFECT = string.Concat("Contains a ", UI.FormatAsLink("Rover", "SCOUT"), " deployed from an orbiting ", SCOUTMODULE.NAME, ".\n\nPod will disintegrate on arrival.");
			}

			public class ROCKETCOMMANDCONSOLE
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Cockpit", "ROCKETCOMMANDCONSOLE");

				public static LocString DESC = "Looks kinda fun.";

				public static LocString EFFECT = "Allows a Duplicant to pilot a rocket.\n\nCargo rockets must possess a Rocket Cockpit in order to function.";
			}

			public class ROCKETENVELOPETILE
			{
				public static LocString NAME = UI.FormatAsLink("Rocket", "ROCKETENVELOPETILE");

				public static LocString DESC = "Keeps the space out.";

				public static LocString EFFECT = "The walls of a rocket.";
			}

			public class ROCKETENVELOPEWINDOWTILE
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Window", "ROCKETENVELOPEWINDOWTILE");

				public static LocString DESC = "I can see my asteroid from here!";

				public static LocString EFFECT = "The window of a rocket.";
			}

			public class ROCKETWALLTILE
			{
				public static LocString NAME = UI.FormatAsLink("Rocket", "ROCKETENVELOPETILE");

				public static LocString DESC = "Keeps the space out.";

				public static LocString EFFECT = "The walls of a rocket.";
			}

			public class SMALLOXIDIZERTANK
			{
				public static LocString NAME = UI.FormatAsLink("Small Solid Oxidizer Tank", "SMALLOXIDIZERTANK");

				public static LocString DESC = "Solid oxidizers allows rocket fuel to be efficiently burned in the vacuum of space.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Fertilizer", "Fertilizer") + " and " + UI.FormatAsLink("Oxylite", "OXYROCK") + " for burning rocket fuels.";

				public static LocString UI_FILTER_CATEGORY = "Accepted Oxidizers";
			}

			public class OXIDIZERTANK
			{
				public static LocString NAME = UI.FormatAsLink("Solid Oxidizer Tank", "OXIDIZERTANK");

				public static LocString DESC = "Solid oxidizers allows rocket fuel to be efficiently burned in the vacuum of space.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Oxylite", "OXYROCK") + " and other oxidizers for burning rocket fuels.";

				public static LocString UI_FILTER_CATEGORY = "Accepted Oxidizers";
			}

			public class OXIDIZERTANKCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Large Solid Oxidizer Tank", "OXIDIZERTANKCLUSTER");

				public static LocString DESC = "Solid oxidizers allows rocket fuel to be efficiently burned in the vacuum of space.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Oxylite", "OXYROCK") + " and other oxidizers for burning rocket fuels.";

				public static LocString UI_FILTER_CATEGORY = "Accepted Oxidizers";
			}

			public class OXIDIZERTANKLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Oxidizer Tank", "LIQUIDOXIDIZERTANK");

				public static LocString DESC = "Liquid oxygen improves the thrust-to-mass ratio of rocket fuels.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Liquid Oxygen", "LIQUIDOXYGEN") + " for burning rocket fuels.";
			}

			public class OXIDIZERTANKLIQUIDCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Oxidizer Tank", "LIQUIDOXIDIZERTANKCLUSTER");

				public static LocString DESC = "Liquid oxygen improves the thrust-to-mass ratio of rocket fuels.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Liquid Oxygen", "LIQUIDOXYGEN") + " for burning rocket fuels.";
			}

			public class LIQUIDCONDITIONER
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Aquatuner", "LIQUIDCONDITIONER");

				public static LocString DESC = "A thermo aquatuner cools liquid and outputs the heat elsewhere.";

				public static LocString EFFECT = "Cools the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped through it, but outputs " + UI.FormatAsLink("Heat", "HEAT") + " in its immediate vicinity.";
			}

			public class LIQUIDCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Cargo Tank", "LIQUIDCARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo bays with any resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class LIQUIDCARGOBAYCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Large Liquid Cargo Tank", "LIQUIDCARGOBAY");

				public static LocString DESC = "Holds more than a regular cargo tank.";

				public static LocString EFFECT = "Allows Duplicants to store most of the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class LIQUIDCARGOBAYSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Cargo Tank", "LIQUIDCARGOBAYSMALL");

				public static LocString DESC = "Duplicants will fill cargo tanks with whatever resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store some of the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class LUXURYBED
			{
				public static LocString NAME = UI.FormatAsLink("Comfy Bed", "LUXURYBED");

				public static LocString DESC = "Duplicants prefer comfy beds to cots and gain more stamina from sleeping in them.";

				public static LocString EFFECT = "Provides a sleeping area for one Duplicant and restores additional " + UI.FormatAsLink("Stamina", "STAMINA") + ".\n\nDuplicants will automatically sleep in their assigned beds at night.";
			}

			public class MEDICALCOT
			{
				public static LocString NAME = UI.FormatAsLink("Triage Cot", "MEDICALCOT");

				public static LocString DESC = "Duplicants use triage cots to recover from physical injuries and receive aid from peers.";

				public static LocString EFFECT = "Accelerates " + UI.FormatAsLink("Health", "HEALTH") + " restoration and the healing of physical injuries.\n\nRevives incapacitated Duplicants.";
			}

			public class DOCTORSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Sick Bay", "DOCTORSTATION");

				public static LocString DESC = "Sick bays can be placed in hospital rooms to decrease the likelihood of disease spreading.";

				public static LocString EFFECT = "Allows Duplicants to administer basic treatments to sick Duplicants.\n\nDuplicants must possess the Bedside Manner " + UI.FormatAsLink("Skill", "ROLES") + " to treat peers.";
			}

			public class ADVANCEDDOCTORSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Disease Clinic", "ADVANCEDDOCTORSTATION");

				public static LocString DESC = "Disease clinics require power, but treat more serious illnesses than sick bays alone.";

				public static LocString EFFECT = "Allows Duplicants to administer powerful treatments to sick Duplicants.\n\nDuplicants must possess the Advanced Medical Care " + UI.FormatAsLink("Skill", "ROLES") + " to treat peers.";
			}

			public class MASSAGETABLE
			{
				public static LocString NAME = UI.FormatAsLink("Massage Table", "MASSAGETABLE");

				public static LocString DESC = "Massage tables quickly reduce extreme stress, at the cost of power production.";

				public static LocString EFFECT = "Rapidly reduces " + UI.FormatAsLink("Stress", "STRESS") + " for the Duplicant user.\n\nDuplicants will automatically seek a massage table when " + UI.FormatAsLink("Stress", "STRESS") + " exceeds breaktime range.";

				public static LocString ACTIVATE_TOOLTIP = "Duplicants must take a massage break when their " + UI.FormatAsKeyWord("Stress") + " reaches {0}%";

				public static LocString DEACTIVATE_TOOLTIP = "Breaktime ends when " + UI.FormatAsKeyWord("Stress") + " is reduced to {0}%";
			}

			public class CEILINGLIGHT
			{
				public static LocString NAME = UI.FormatAsLink("Ceiling Light", "CEILINGLIGHT");

				public static LocString DESC = "Light reduces Duplicant stress and is required to grow certain plants.";

				public static LocString EFFECT = "Provides " + UI.FormatAsLink("Light", "LIGHT") + " when " + UI.FormatAsLink("Powered", "POWER") + ".\n\nIncreases Duplicant workspeed within light radius.";
			}

			public class AIRFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Deodorizer", "AIRFILTER");

				public static LocString DESC = "Oh! Citrus scented!";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Sand", "SAND") + " to filter " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + " from the air, reducing " + UI.FormatAsLink("Disease", "DISEASE") + " spread.";
			}

			public class ARTIFACTANALYSISSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Artifact Analysis Station", "ARTIFACTANALYSISSTATION");

				public static LocString DESC = "Discover the mysteries of the past.";

				public static LocString EFFECT = "Analyses and extracts " + UI.FormatAsLink("Neutronium", "UNOBTANIUM") + " from artifacts of interest.";

				public static LocString PAYLOAD_DROP_RATE = string.Concat(ITEMS.INDUSTRIAL_PRODUCTS.GENE_SHUFFLER_RECHARGE.NAME, " drop chance: {chance}");

				public static LocString PAYLOAD_DROP_RATE_TOOLTIP = string.Concat("This artifact has a {chance} to drop a ", ITEMS.INDUSTRIAL_PRODUCTS.GENE_SHUFFLER_RECHARGE.NAME, " when analyzed at the ", NAME);
			}

			public class CANVAS
			{
				public static LocString NAME = UI.FormatAsLink("Blank Canvas", "CANVAS");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CANVASWIDE
			{
				public static LocString NAME = UI.FormatAsLink("Landscape Canvas", "CANVASWIDE");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CANVASTALL
			{
				public static LocString NAME = UI.FormatAsLink("Portrait Canvas", "CANVASTALL");

				public static LocString DESC = "Once built, a Duplicant can paint a blank canvas to produce a decorative painting.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be painted by a Duplicant.";

				public static LocString POORQUALITYNAME = "Crude Painting";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Painting";

				public static LocString EXCELLENTQUALITYNAME = "Masterpiece";
			}

			public class CO2SCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Carbon Skimmer", "CO2SCRUBBER");

				public static LocString DESC = "Skimmers remove large amounts of carbon dioxide, but produce no breathable air.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Water", "WATER") + " to filter " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " from the air.";
			}

			public class COMPOST
			{
				public static LocString NAME = UI.FormatAsLink("Compost", "COMPOST");

				public static LocString DESC = "Composts safely deal with biological waste, producing fresh dirt.";

				public static LocString EFFECT = "Reduces " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " and other compostables down into " + UI.FormatAsLink("Dirt", "DIRT") + ".";
			}

			public class COOKINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Electric Grill", "COOKINGSTATION");

				public static LocString DESC = "Proper cooking eliminates foodborne disease and produces tasty, stress-relieving meals.";

				public static LocString EFFECT = "Cooks a wide variety of improved " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class CRYOTANK
			{
				public static LocString NAME = UI.FormatAsLink("Cryotank 3000", "CRYOTANK");

				public static LocString DESC = "The tank appears impossibly old, but smells crisp and brand new.\n\nA silhouette just barely visible through the frost of the glass.";

				public static LocString DEFROSTBUTTON = "Defrost Friend";

				public static LocString DEFROSTBUTTONTOOLTIP = "A new pal is just an icebreaker away";
			}

			public class GOURMETCOOKINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Gas Range", "GOURMETCOOKINGSTATION");

				public static LocString DESC = "Luxury meals increase Duplicants morale and prevents them from becoming stressed.";

				public static LocString EFFECT = "Cooks a wide variety of quality " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class DININGTABLE
			{
				public static LocString NAME = UI.FormatAsLink("Mess Table", "DININGTABLE");

				public static LocString DESC = "Duplicants prefer to dine at a table, rather than eat off the floor.";

				public static LocString EFFECT = "Gives one Duplicant a place to eat.\n\nDuplicants will automatically eat at their assigned table when hungry.";
			}

			public class DOOR
			{
				public static class CONTROL_STATE
				{
					public class OPEN
					{
						public static LocString NAME = "Open";

						public static LocString TOOLTIP = "This door will remain open";
					}

					public class CLOSE
					{
						public static LocString NAME = "Lock";

						public static LocString TOOLTIP = "Nothing may pass through";
					}

					public class AUTO
					{
						public static LocString NAME = "Auto";

						public static LocString TOOLTIP = "Duplicants open and close this door as needed";
					}
				}

				public static LocString NAME = UI.FormatAsLink("Pneumatic Door", "DOOR");

				public static LocString DESC = "Door controls can be used to prevent Duplicants from entering restricted areas.";

				public static LocString EFFECT = "Encloses areas without blocking " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow.\n\nWild " + UI.FormatAsLink("Critters", "CRITTERS") + " cannot pass through doors.";

				public static LocString PRESSURE_SUIT_REQUIRED = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT") + " required {0}";

				public static LocString PRESSURE_SUIT_NOT_REQUIRED = UI.FormatAsLink("Atmo Suit", "ATMO_SUIT") + " not required {0}";

				public static LocString ABOVE = "above";

				public static LocString BELOW = "below";

				public static LocString LEFT = "on left";

				public static LocString RIGHT = "on right";

				public static LocString LOGIC_OPEN = "Open/Close";

				public static LocString LOGIC_OPEN_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Open";

				public static LocString LOGIC_OPEN_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Close and lock";
			}

			public class ELECTROLYZER
			{
				public static LocString NAME = UI.FormatAsLink("Electrolyzer", "ELECTROLYZER");

				public static LocString DESC = "Water goes in one end, life sustaining oxygen comes out the other.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Water", "WATER") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class RUSTDEOXIDIZER
			{
				public static LocString NAME = UI.FormatAsLink("Rust Deoxidizer", "RUSTDEOXIDIZER");

				public static LocString DESC = "Rust and salt goes in, oxygen comes out.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Rust", "RUST") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Chlorine", "CHLORINE") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class DESALINATOR
			{
				public static LocString NAME = UI.FormatAsLink("Desalinator", "DESALINATOR");

				public static LocString DESC = "Salt can be refined into table salt for a mealtime morale boost.";

				public static LocString EFFECT = "Removes " + UI.FormatAsLink("Salt", "SALT") + " from " + UI.FormatAsLink("Brine", "BRINE") + " or " + UI.FormatAsLink("Salt Water", "SALTWATER") + ", producing " + UI.FormatAsLink("Water", "WATER") + ".";
			}

			public class POWERTRANSFORMERSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Power Transformer", "POWERTRANSFORMERSMALL");

				public static LocString DESC = "Limiting the power drawn by wires prevents them from incurring overload damage.";

				public static LocString EFFECT = "Limits " + UI.FormatAsLink("Power", "POWER") + " flowing through the Transformer to 1000 W.\n\nConnect " + UI.FormatAsLink("Batteries", "BATTERY") + " on the large side to act as a valve and prevent " + UI.FormatAsLink("Wires", "WIRE") + " from drawing more than 1000 W.\n\nCan be rotated before construction.";
			}

			public class POWERTRANSFORMER
			{
				public static LocString NAME = UI.FormatAsLink("Large Power Transformer", "POWERTRANSFORMER");

				public static LocString DESC = "Limiting the power drawn by wires prevents them from incurring overload damage.";

				public static LocString EFFECT = "Limits " + UI.FormatAsLink("Power", "POWER") + " flowing through the Transformer to 4 kW.\n\nConnect " + UI.FormatAsLink("Batteries", "BATTERY") + " on the large side to act as a valve and prevent " + UI.FormatAsLink("Wires", "WIRE") + " from drawing more than 4 kW.\n\nCan be rotated before construction.";
			}

			public class FLOORLAMP
			{
				public static LocString NAME = UI.FormatAsLink("Lamp", "FLOORLAMP");

				public static LocString DESC = "Any building's light emitting radius can be viewed in the light overlay.";

				public static LocString EFFECT = "Provides " + UI.FormatAsLink("Light", "LIGHT") + " when " + UI.FormatAsLink("Powered", "POWER") + ".\n\nIncreases Duplicant workspeed within light radius.";
			}

			public class FLOWERVASE
			{
				public static LocString NAME = UI.FormatAsLink("Flower Pot", "FLOWERVASE");

				public static LocString DESC = "Flower pots allow decorative plants to be moved to new locations.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class FLOWERVASEWALL
			{
				public static LocString NAME = UI.FormatAsLink("Wall Pot", "FLOWERVASEWALL");

				public static LocString DESC = "Placing a plant in a wall pot can add a spot of Decor to otherwise bare walls.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a wall.";
			}

			public class FLOWERVASEHANGING
			{
				public static LocString NAME = UI.FormatAsLink("Hanging Pot", "FLOWERVASEHANGING");

				public static LocString DESC = "Hanging pots can add some Decor to a room, without blocking buildings on the floor.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a ceiling.";
			}

			public class FLOWERVASEHANGINGFANCY
			{
				public static LocString NAME = UI.FormatAsLink("Aero Pot", "FLOWERVASEHANGINGFANCY");

				public static LocString DESC = "Aero pots can be hung from the ceiling and have extremely high Decor.";

				public static LocString EFFECT = "Houses a single " + UI.FormatAsLink("Plant", "PLANTS") + " when sown with a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be hung from a ceiling.";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = UI.FormatAsLink("Lavatory", "FLUSHTOILET");

				public static LocString DESC = "Lavatories transmit fewer germs to Duplicants' skin and require no emptying.";

				public static LocString EFFECT = "Gives Duplicants a place to relieve themselves.\n\nSpreads very few " + UI.FormatAsLink("Germs", "DISEASE") + ".";
			}

			public class SHOWER
			{
				public static LocString NAME = UI.FormatAsLink("Shower", "SHOWER");

				public static LocString DESC = "Regularly showering will prevent Duplicants spreading germs to the things they touch.";

				public static LocString EFFECT = "Improves Duplicant " + UI.FormatAsLink("Morale", "MORALE") + " and removes surface " + UI.FormatAsLink("Germs", "DISEASE") + ".";
			}

			public class CONDUIT
			{
				public class STATUS_ITEM
				{
					public static LocString NAME = "Marked for Emptying";

					public static LocString TOOLTIP = "Awaiting a " + UI.FormatAsLink("Plumber", "PLUMBER") + " to clear this pipe";
				}
			}

			public class GAMMARAYOVEN
			{
				public static LocString NAME = UI.FormatAsLink("Gamma Ray Oven", "GAMMARAYOVEN");

				public static LocString DESC = "Nuke your food";

				public static LocString EFFECT = "Cooks a variety of " + UI.FormatAsLink("Foods", "FOOD") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class GASCARGOBAY
			{
				public static LocString NAME = UI.FormatAsLink("Gas Cargo Canister", "GASCARGOBAY");

				public static LocString DESC = "Duplicants will fill cargo bays with any resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class GASCARGOBAYCLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Large Gas Cargo Canister", "GASCARGOBAY");

				public static LocString DESC = "Holds more than a typical gas cargo canister.";

				public static LocString EFFECT = "Allows Duplicants to store most of the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class GASCARGOBAYSMALL
			{
				public static LocString NAME = UI.FormatAsLink("Gas Cargo Canister", "GASCARGOBAYSMALL");

				public static LocString DESC = "Duplicants fill cargo canisters with any resources they find during space missions.";

				public static LocString EFFECT = "Allows Duplicants to store some of the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources found during space missions.\n\nStored resources become available to the colony upon the rocket's return.";
			}

			public class GASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe", "GASCONDUIT");

				public static LocString DESC = "Gas pipes are used to connect the inputs and outputs of ventilated buildings.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " between " + UI.FormatAsLink("Outputs", "GASPIPING") + " and " + UI.FormatAsLink("Intakes", "GASPIPING") + ".\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Bridge", "GASCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems prevent mingled contents from causing building damage.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Gas Pipe", "GASPIPING") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Gas Flow", "GASCONDUITPREFERENTIALFLOW");

				public static LocString DESC = "Priority flows ensure important buildings are filled first when on a system with other buildings.";

				public static LocString EFFECT = "Diverts " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " to a secondary input when its primary input overflows.";
			}

			public class LIQUIDCONDUITPREFERENTIALFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Priority Liquid Flow", "LIQUIDCONDUITPREFERENTIALFLOW");

				public static LocString DESC = "Priority flows ensure important buildings are filled first when on a system with other buildings.";

				public static LocString EFFECT = "Diverts " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " to a secondary input when its primary input overflows.";
			}

			public class GASCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Gas Overflow Valve", "GASCONDUITOVERFLOW");

				public static LocString DESC = "Overflow valves can be used to prioritize which buildings should receive precious resources first.";

				public static LocString EFFECT = "Fills a secondary" + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " output only when its primary output is blocked.";
			}

			public class LIQUIDCONDUITOVERFLOW
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Overflow Valve", "LIQUIDCONDUITOVERFLOW");

				public static LocString DESC = "Overflow valves can be used to prioritize which buildings should receive precious resources first.";

				public static LocString EFFECT = "Fills a secondary" + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " output only when its primary output is blocked.";
			}

			public class LAUNCHPAD
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Platform", "LAUNCHPLATFORM");

				public static LocString DESC = "A platform from which rockets can be launched and on which they can land.";

				public static LocString EFFECT = string.Concat("Enables Rocket construction.\n\nAllows Rockets to launch from or land on the host Planetoid.\n\nAutomatically links up to ", MODULARLAUNCHPADPORT.NAME, UI.FormatAsLink("s", "MODULARLAUNCHPADPORTSOLID"), " built to either side of the platform.");

				public static LocString LOGIC_PORT_READY = "Rocket Checklist";

				public static LocString LOGIC_PORT_READY_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when its rocket is ready for flight";

				public static LocString LOGIC_PORT_READY_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_LAUNCH = "Launch Rocket";

				public static LocString LOGIC_PORT_LAUNCH_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Launch rocket";

				public static LocString LOGIC_PORT_LAUNCH_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Cancel launch";
			}

			public class GASFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Gas Filter", "GASFILTER");

				public static LocString DESC = "All gases are sent into the building's output pipe, except the gas chosen for filtering.";

				public static LocString EFFECT = "Sieves one " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from the air, sending it into a dedicated " + UI.FormatAsLink("Pipe", "GASPIPING") + ".";

				public static LocString STATUS_ITEM = "Filters: {0}";

				public static LocString ELEMENT_NOT_SPECIFIED = "Not Specified";
			}

			public class SOLIDFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Solid Filter", "SOLIDFILTER");

				public static LocString DESC = "All solids are sent into the building's output conveyor, except the solid chosen for filtering.";

				public static LocString EFFECT = string.Concat("Separates one ", UI.FormatAsLink("Solid Material", "ELEMENTS_SOLID"), " from the conveyor, sending it into a dedicated ", SOLIDCONDUIT.NAME, ".");

				public static LocString STATUS_ITEM = "Filters: {0}";

				public static LocString ELEMENT_NOT_SPECIFIED = "Not Specified";
			}

			public class GASPERMEABLEMEMBRANE
			{
				public static LocString NAME = UI.FormatAsLink("Airflow Tile", "GASPERMEABLEMEMBRANE");

				public static LocString DESC = "Building with airflow tile promotes better gas circulation within a colony.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nBlocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " flow without obstructing " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pump", "GASPUMP");

				public static LocString DESC = "Piping a pump's output to a building's intake will send gas to that building.";

				public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " and runs it through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".\n\nMust be immersed in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Gas Pump", "GASMINIPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of gas with minimum power.";

				public static LocString EFFECT = "Draws in a small amount of " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " and runs it through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".\n\nMust be immersed in " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ".";
			}

			public class GASVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Valve", "GASVALVE");

				public static LocString DESC = "Valves control the amount of gas that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " volume permitted through " + UI.FormatAsLink("Pipes", "GASPIPING") + ".";
			}

			public class GASLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Shutoff", "GASLOGICVALVE");

				public static LocString DESC = "Automated piping saves power and time by removing the need for Duplicant input.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow on or off.";

				public static LocString LOGIC_PORT = "Open/Close";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow gas flow";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Prevent gas flow";
			}

			public class GASLIMITVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Gas Meter Valve", "GASLIMITVALVE");

				public static LocString DESC = "Meter Valves let an exact amount of gas pass through before shutting off.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow off when the specified amount has passed through it.";

				public static LocString LOGIC_PORT_OUTPUT = "Limit Reached";

				public static LocString OUTPUT_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if limit has been reached";

				public static LocString OUTPUT_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_RESET = "Reset Meter";

				public static LocString RESET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reset the amount";

				public static LocString RESET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";
			}

			public class GASVENT
			{
				public static LocString NAME = UI.FormatAsLink("Gas Vent", "GASVENT");

				public static LocString DESC = "Vents are an exit point for gases from ventilation systems.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from " + UI.FormatAsLink("Gas Pipes", "GASPIPING") + ".";
			}

			public class GASVENTHIGHPRESSURE
			{
				public static LocString NAME = UI.FormatAsLink("High Pressure Gas Vent", "GASVENTHIGHPRESSURE");

				public static LocString DESC = "High pressure vents can expel gas into more highly pressurized environments.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " from " + UI.FormatAsLink("Gas Pipes", "GASPIPING") + " into high pressure locations.";
			}

			public class GASBOTTLER
			{
				public static LocString NAME = UI.FormatAsLink("Canister Filler", "GASBOTTLER");

				public static LocString DESC = "Canisters allow Duplicants to manually deliver gases from place to place.";

				public static LocString EFFECT = "Automatically stores piped " + UI.FormatAsLink("Gases", "ELEMENTS_GAS") + " into canisters for manual transport.";
			}

			public class GENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Coal Generator", "GENERATOR");

				public static LocString DESC = "Burning coal produces more energy than manual power, but emits heat and exhaust.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Coal", "CARBON") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".";

				public static LocString OVERPRODUCTION = "{Generator} overproduction";
			}

			public class GENETICANALYSISSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Botanical Analyzer", "GENETICANALYSISSTATION");

				public static LocString DESC = "Would a mutated rose still smell as sweet?";

				public static LocString EFFECT = "Identifies new " + UI.FormatAsLink("Seed", "PLANTS") + " subspecies.";
			}

			public class DEVGENERATOR
			{
				public static LocString NAME = "Dev Generator";

				public static LocString DESC = "Runs on coffee.";

				public static LocString EFFECT = "Generates testing power for late nights.";
			}

			public class DEVRADIATIONGENERATOR
			{
				public static LocString NAME = "Dev Radiation Emitter";

				public static LocString DESC = "That's some <i>strong</i> coffee.";

				public static LocString EFFECT = "Generates on-demand radiation to keep you cosy.";
			}

			public class GENERICFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Omniprinter", "GENERICFABRICATOR");

				public static LocString DESC = "Omniprinters are incapable of printing organic matter.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Raw Mineral", "RAWMINERAL") + " into unique materials and objects.";
			}

			public class GRAVE
			{
				public static LocString NAME = UI.FormatAsLink("Tasteful Memorial", "GRAVE");

				public static LocString DESC = "Burying dead Duplicants reduces health hazards and stress on the colony.";

				public static LocString EFFECT = "Provides a final resting place for deceased Duplicants.\n\nLiving Duplicants will automatically place an unburied corpse inside.";
			}

			public class HEADQUARTERS
			{
				public static LocString NAME = UI.FormatAsLink("Printing Pod", "HEADQUARTERS");

				public static LocString DESC = "New Duplicants come out here, but thank goodness, they never go back in.";

				public static LocString EFFECT = "An exceptionally advanced bioprinter of unknown origin.\n\nIt periodically produces new Duplicants or care packages containing resources.";
			}

			public class HYDROGENGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Hydrogen Generator", "HYDROGENGENERATOR");

				public static LocString DESC = "Hydrogen generators are extremely efficient, emitting next to no waste.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class METHANEGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Natural Gas Generator", "METHANEGENERATOR");

				public static LocString DESC = "Natural gas generators leak polluted water and are best built above a waste reservoir.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Natural Gas", "METHANE") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".";
			}

			public class NUCLEARREACTOR
			{
				public static LocString NAME = UI.FormatAsLink("Research Reactor", "NUCLEARREACTOR");

				public static LocString DESC = "Radbolt generators and reflectors make radiation useable by other buildings.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM") + " to produce " + UI.FormatAsLink("Radiation", "RADIATION") + " for Radbolt production.\n\nGenerates a massive amount of " + UI.FormatAsLink("Heat", "HEAT") + ".";

				public static LocString LOGIC_PORT = "Fuel Delivery Control";

				public static LocString INPUT_PORT_ACTIVE = "Fuel Delivery Enabled";

				public static LocString INPUT_PORT_INACTIVE = "Fuel Delivery Disabled";
			}

			public class WOODGASGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Wood Burner", "WOODGASGENERATOR");

				public static LocString DESC = "Wood burners are small and easy to maintain, but produce a fair amount of heat.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Lumber", "WOOD") + " to produce electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class PETROLEUMGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Petroleum Generator", "PETROLEUMGENERATOR");

				public static LocString DESC = "Petroleum generators have a high energy output but produce a great deal of waste.";

				public static LocString EFFECT = "Converts either " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " or " + UI.FormatAsLink("Ethanol", "ETHANOL") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nProduces " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + " and " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".";
			}

			public class HYDROPONICFARM
			{
				public static LocString NAME = UI.FormatAsLink("Hydroponic Farm", "HYDROPONICFARM");

				public static LocString DESC = "Hydroponic farms reduce Duplicant traffic by automating irrigating crops.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nCan be used as floor tile and rotated before construction.\n\nMust be irrigated through " + UI.FormatAsLink("Liquid Piping", "LIQUIDPIPING") + ".";
			}

			public class INSULATEDGASCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Gas Pipe", "INSULATEDGASCONDUIT");

				public static LocString DESC = "Pipe insulation prevents gas contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " with minimal change in " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class GASCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Gas Pipe", "GASCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes pumping cold gas can be run through hot areas to help cool them down.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + ", allowing extreme " + UI.FormatAsLink("Temperature", "HEAT") + " exchange with the surrounding environment.\n\nCan be run through wall and floor tile.";
			}

			public class INSULATEDLIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Liquid Pipe", "INSULATEDLIQUIDCONDUIT");

				public static LocString DESC = "Pipe insulation prevents liquid contents from significantly changing temperature in transit.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " with minimal change in " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LIQUIDCONDUITRADIANT
			{
				public static LocString NAME = UI.FormatAsLink("Radiant Liquid Pipe", "LIQUIDCONDUITRADIANT");

				public static LocString DESC = "Radiant pipes pumping cold liquid can be run through hot areas to help cool them down.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", allowing extreme " + UI.FormatAsLink("Temperature", "HEAT") + " exchange with the surrounding environment.\n\nCan be run through wall and floor tile.";
			}

			public class INSULATEDWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Wire", "INSULATEDWIRE");

				public static LocString DESC = "This stuff won't go melting if things get heated.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources in extreme " + UI.FormatAsLink("Heat", "HEAT") + ".\n\nCan be run through wall and floor tile.";
			}

			public class INSULATIONTILE
			{
				public static LocString NAME = UI.FormatAsLink("Insulated Tile", "INSULATIONTILE");

				public static LocString DESC = "The low thermal conductivity of insulated tiles slows any heat passing through them.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nReduces " + UI.FormatAsLink("Heat", "HEAT") + " transfer between walls, retaining ambient heat in an area.";
			}

			public class EXTERIORWALL
			{
				public static LocString NAME = UI.FormatAsLink("Drywall", "EXTERIORWALL");

				public static LocString DESC = "Drywall can be used in conjunction with tiles to build airtight rooms on the surface.";

				public static LocString EFFECT = "Prevents " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " loss in space.\n\nBuilds an insulating backwall behind buildings.";
			}

			public class FARMTILE
			{
				public static LocString NAME = UI.FormatAsLink("Farm Tile", "FARMTILE");

				public static LocString DESC = "Duplicants can deliver fertilizer and liquids to farm tiles, accelerating plant growth.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".\n\nCan be used as floor tile and rotated before construction.";
			}

			public class LADDER
			{
				public static LocString NAME = UI.FormatAsLink("Ladder", "LADDER");

				public static LocString DESC = "(That means they climb it.)";

				public static LocString EFFECT = "Enables vertical mobility for Duplicants.";
			}

			public class LADDERFAST
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Ladder", "LADDERFAST");

				public static LocString DESC = "Plastic ladders are mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Increases Duplicant climbing speed.";
			}

			public class LIQUIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe", "LIQUIDCONDUIT");

				public static LocString DESC = "Liquid pipes are used to connect the inputs and outputs of plumbed buildings.";

				public static LocString EFFECT = "Carries " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " between " + UI.FormatAsLink("Outputs", "LIQUIDPIPING") + " and " + UI.FormatAsLink("Intakes", "LIQUIDPIPING") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LIQUIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Bridge", "LIQUIDCONDUITBRIDGE");

				public static LocString DESC = "Separate pipe systems help prevent building damage caused by mingled pipe contents.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Liquid Pipe", "LIQUIDPIPING") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class ICECOOLEDFAN
			{
				public static LocString NAME = UI.FormatAsLink("Ice-E Fan", "ICECOOLEDFAN");

				public static LocString DESC = "A Duplicant can work an Ice-E fan to temporarily cool small areas as needed.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Ice", "ICE") + " to dissipate a small amount of the " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class ICEMACHINE
			{
				public static LocString NAME = UI.FormatAsLink("Ice Maker", "ICEMACHINE");

				public static LocString DESC = "Ice makers can be used as a small renewable source of ice.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Water", "WATER") + " into " + UI.FormatAsLink("Ice", "ICE") + ".";
			}

			public class LIQUIDCOOLEDFAN
			{
				public static LocString NAME = UI.FormatAsLink("Hydrofan", "LIQUIDCOOLEDFAN");

				public static LocString DESC = "A Duplicant can work a hydrofan to temporarily cool small areas as needed.";

				public static LocString EFFECT = "Dissipates a small amount of the " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class CREATURETRAP
			{
				public static LocString NAME = UI.FormatAsLink("Critter Trap", "CREATURETRAP");

				public static LocString DESC = "Critter traps cannot catch swimming or flying critters.";

				public static LocString EFFECT = "Captures a living " + UI.FormatAsLink("Critter", "CRITTERS") + " for transport.\n\nSingle use.";
			}

			public class CREATUREDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Critter Drop-Off", "CREATUREDELIVERYPOINT");

				public static LocString DESC = "Duplicants automatically bring captured critters to these relocation points for release.";

				public static LocString EFFECT = "Releases trapped " + UI.FormatAsLink("Critters", "CRITTERS") + " back into the world.\n\nCan be used multiple times.";
			}

			public class LIQUIDFILTER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Filter", "LIQUIDFILTER");

				public static LocString DESC = "All liquids are sent into the building's output pipe, except the liquid chosen for filtering.";

				public static LocString EFFECT = "Sieves one " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " out of a mix, sending it into a dedicated " + UI.FormatAsLink("Filtered Output Pipe", "LIQUIDPIPING") + ".\n\nCan only filter one liquid type at a time.";
			}

			public class LIQUIDPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pump", "LIQUIDPUMP");

				public static LocString DESC = "Piping a pump's output to a building's intake will send liquid to that building.";

				public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and runs it through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class LIQUIDMINIPUMP
			{
				public static LocString NAME = UI.FormatAsLink("Mini Liquid Pump", "LIQUIDMINIPUMP");

				public static LocString DESC = "Mini pumps are useful for moving small quantities of liquid with minimum power.";

				public static LocString EFFECT = "Draws in a small amount of " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and runs it through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class LIQUIDPUMPINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Pitcher Pump", "LIQUIDPUMPINGSTATION");

				public static LocString DESC = "Pitcher pumps allow Duplicants to bottle and deliver liquids from place to place.";

				public static LocString EFFECT = "Manually pumps " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " into bottles for transport.\n\nDuplicants can only carry liquids that are bottled.";
			}

			public class LIQUIDVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Valve", "LIQUIDVALVE");

				public static LocString DESC = "Valves control the amount of liquid that moves through pipes, preventing waste.";

				public static LocString EFFECT = "Controls the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " volume permitted through " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + ".";
			}

			public class LIQUIDLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Shutoff", "LIQUIDLOGICVALVE");

				public static LocString DESC = "Automated piping saves power and time by removing the need for Duplicant input.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " flow on or off.";

				public static LocString LOGIC_PORT = "Open/Close";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow Liquid flow";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Prevent Liquid flow";
			}

			public class LIQUIDLIMITVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Meter Valve", "LIQUIDLIMITVALVE");

				public static LocString DESC = "Meter Valves let an exact amount of liquid pass through before shutting off.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " flow off when the specified amount has passed through it.";

				public static LocString LOGIC_PORT_OUTPUT = "Limit Reached";

				public static LocString OUTPUT_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if limit has been reached";

				public static LocString OUTPUT_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_RESET = "Reset Meter";

				public static LocString RESET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reset the amount";

				public static LocString RESET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";
			}

			public class LIQUIDVENT
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Vent", "LIQUIDVENT");

				public static LocString DESC = "Vents are an exit point for liquids from plumbing systems.";

				public static LocString EFFECT = "Releases " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " from " + UI.FormatAsLink("Liquid Pipes", "LIQUIDPIPING") + ".";
			}

			public class MANUALGENERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Generator", "MANUALGENERATOR");

				public static LocString DESC = "Watching Duplicants run on it is adorable... the electrical power is just an added bonus.";

				public static LocString EFFECT = "Converts manual labor into electrical " + UI.FormatAsLink("Power", "POWER") + ".";
			}

			public class MANUALPRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Manual Airlock", "MANUALPRESSUREDOOR");

				public static LocString DESC = "Airlocks can quarter off dangerous areas and prevent gases from seeping into the colony.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nWild " + UI.FormatAsLink("Critters", "CRITTERS") + " cannot pass through doors.";
			}

			public class MESHTILE
			{
				public static LocString NAME = UI.FormatAsLink("Mesh Tile", "MESHTILE");

				public static LocString DESC = "Mesh tile can be used to make Duplicant pathways in areas where liquid flows.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nDoes not obstruct " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " or " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow.";
			}

			public class PLASTICTILE
			{
				public static LocString NAME = UI.FormatAsLink("Plastic Tile", "PLASTICTILE");

				public static LocString DESC = "Plastic tile is mildly antiseptic and can help limit the spread of germs in a colony.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class GLASSTILE
			{
				public static LocString NAME = UI.FormatAsLink("Window Tile", "GLASSTILE");

				public static LocString DESC = "Window tiles provide a barrier against liquid and gas and are completely transparent.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nAllows " + UI.FormatAsLink("Light", "LIGHT") + " and " + UI.FormatAsLink("Decor", "DECOR") + " to pass through.";
			}

			public class METALTILE
			{
				public static LocString NAME = UI.FormatAsLink("Metal Tile", "METALTILE");

				public static LocString DESC = "Heat travels much more quickly through metal tile than other types of flooring.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nSignificantly increases Duplicant runspeed.";
			}

			public class BUNKERTILE
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Tile", "BUNKERTILE");

				public static LocString DESC = "Bunker tile can build strong shelters in otherwise dangerous environments.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nCan withstand extreme pressures and impacts.";
			}

			public class CARPETTILE
			{
				public static LocString NAME = UI.FormatAsLink("Carpeted Tile", "CARPETTILE");

				public static LocString DESC = "Soft on little Duplicant toesies.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class MOULDINGTILE
			{
				public static LocString NAME = UI.FormatAsLink("Trimming Tile", "MOUDLINGTILE");

				public static LocString DESC = "Trimming is used as purely decorative lining for walls and structures.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class MONUMENTBOTTOM
			{
				public static LocString NAME = UI.FormatAsLink("Monument Base", "MONUMENTBOTTOM");

				public static LocString DESC = "The base of a monument must be constructed first.";

				public static LocString EFFECT = "Builds the bottom section of a Great Monument.\n\nCan be customized.\n\nA Great Monument must be built to achieve the Colonize Imperative.";
			}

			public class MONUMENTMIDDLE
			{
				public static LocString NAME = UI.FormatAsLink("Monument Midsection", "MONUMENTMIDDLE");

				public static LocString DESC = "Customized sections of a Great Monument can be mixed and matched.";

				public static LocString EFFECT = "Builds the middle section of a Great Monument.\n\nCan be customized.\n\nA Great Monument must be built to achieve the Colonize Imperative.";
			}

			public class MONUMENTTOP
			{
				public static LocString NAME = UI.FormatAsLink("Monument Top", "MONUMENTTOP");

				public static LocString DESC = "Building a Great Monument will declare to the universe that this hunk of rock is your own.";

				public static LocString EFFECT = "Builds the top section of a Great Monument.\n\nCan be customized.\n\nA Great Monument must be built to achieve the Colonize Imperative.";
			}

			public class MICROBEMUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Microbe Musher", "MICROBEMUSHER");

				public static LocString DESC = "Musher recipes will keep Duplicants fed, but may impact health and morale over time.";

				public static LocString EFFECT = "Produces low quality " + UI.FormatAsLink("Food", "FOOD") + " using common ingredients.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class MINERALDEOXIDIZER
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Diffuser", "MINERALDEOXIDIZER");

				public static LocString DESC = "Oxygen diffusers are inefficient, but output enough oxygen to keep a colony breathing.";

				public static LocString EFFECT = "Converts large amounts of " + UI.FormatAsLink("Algae", "ALGAE") + " into " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class SUBLIMATIONSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Sublimation Station", "SUBLIMATIONSTATION");

				public static LocString DESC = "Sublimation is the sublime process by which solids convert directly into gas.";

				public static LocString EFFECT = "Speeds up the conversion of " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + " into " + UI.FormatAsLink("Polluted Oxygen", "CONTAMINATEDOXYGEN") + ".\n\nBecomes idle when the area reaches maximum pressure capacity.";
			}

			public class ORESCRUBBER
			{
				public static LocString NAME = UI.FormatAsLink("Ore Scrubber", "ORESCRUBBER");

				public static LocString DESC = "Scrubbers sanitize freshly mined materials before they're brought into the colony.";

				public static LocString EFFECT = "Kills a significant amount of " + UI.FormatAsLink("Germs", "DISEASE") + " present on " + UI.FormatAsLink("Raw Ore", "RAWMINERAL") + ".";
			}

			public class OUTHOUSE
			{
				public static LocString NAME = UI.FormatAsLink("Outhouse", "OUTHOUSE");

				public static LocString DESC = "The colony that eats together, excretes together.";

				public static LocString EFFECT = "Gives Duplicants a place to relieve themselves.\n\nRequires no " + UI.FormatAsLink("Piping", "LIQUIDPIPING") + ".\n\nMust be periodically emptied of " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ".";
			}

			public class APOTHECARY
			{
				public static LocString NAME = UI.FormatAsLink("Apothecary", "APOTHECARY");

				public static LocString DESC = "Some medications help prevent diseases, while others aim to alleviate existing illness.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Medicine", "MEDICINE") + " to cure most basic " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nDuplicants must possess the Medicine Compounding " + UI.FormatAsLink("Skill", "ROLES") + " to fabricate medicines.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class ADVANCEDAPOTHECARY
			{
				public static LocString NAME = UI.FormatAsLink("Nuclear Apothecary", "ADVANCEDAPOTHECARY");

				public static LocString DESC = "Some medications help prevent diseases, while others aim to alleviate existing illness.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Medicine", "MEDICINE") + " to cure most basic " + UI.FormatAsLink("Diseases", "DISEASE") + ".\n\nDuplicants must possess the Medicine Compounding " + UI.FormatAsLink("Skill", "ROLES") + " to fabricate medicines.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class PLANTERBOX
			{
				public static LocString NAME = UI.FormatAsLink("Planter Box", "PLANTERBOX");

				public static LocString DESC = "Domestically grown seeds mature more quickly than wild plants.";

				public static LocString EFFECT = "Grows one " + UI.FormatAsLink("Plant", "PLANTS") + " from a " + UI.FormatAsLink("Seed", "PLANTS") + ".";
			}

			public class PRESSUREDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Mechanized Airlock", "PRESSUREDOOR");

				public static LocString DESC = "Mechanized airlocks open and close more quickly than other types of door.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nFunctions as a " + UI.FormatAsLink("Manual Airlock", "MANUALPRESSUREDOOR") + " when no " + UI.FormatAsLink("Power", "POWER") + " is available.\n\nWild " + UI.FormatAsLink("Critters", "CRITTERS") + " cannot pass through doors.";
			}

			public class BUNKERDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Bunker Door", "BUNKERDOOR");

				public static LocString DESC = "A massive, slow-moving door which is nearly indestructible.";

				public static LocString EFFECT = "Blocks " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " flow, maintaining pressure between areas.\n\nCan withstand extremely high pressures and impacts.";
			}

			public class RATIONBOX
			{
				public static LocString NAME = UI.FormatAsLink("Ration Box", "RATIONBOX");

				public static LocString DESC = "Ration boxes keep food safe from hungry critters, but don't slow food spoilage.";

				public static LocString EFFECT = "Stores a small amount of " + UI.FormatAsLink("Food", "FOOD") + ".\n\nFood must be delivered to boxes by Duplicants.";
			}

			public class PARKSIGN
			{
				public static LocString NAME = UI.FormatAsLink("Park Sign", "PARKSIGN");

				public static LocString DESC = "Passing through parks will increase Duplicant Morale.";

				public static LocString EFFECT = "Classifies an area as a Park or Nature Reserve.";
			}

			public class RADIATIONLIGHT
			{
				public static LocString NAME = UI.FormatAsLink("Radiation Lamp", "RADIATIONLIGHT");

				public static LocString DESC = "Duplicants can become sick if exposed to radiation without protection.";

				public static LocString EFFECT = "Emits " + UI.FormatAsLink("Radiation", "RADIATION") + " when " + UI.FormatAsLink("Powered", "POWER") + " that can be collected by a " + UI.FormatAsLink("Radbolt Generator", "HIGHENERGYPARTICLESPAWNER") + ".";
			}

			public class REFRIGERATOR
			{
				public static LocString NAME = UI.FormatAsLink("Refrigerator", "REFRIGERATOR");

				public static LocString DESC = "Food spoilage can be slowed by ambient conditions as well as by refrigerators.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Food", "FOOD") + " at an ideal " + UI.FormatAsLink("Temperature", "HEAT") + " to prevent spoilage.";

				public static LocString LOGIC_PORT = "Full/Not Full";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when full";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class ROLESTATION
			{
				public static LocString NAME = UI.FormatAsLink("Skills Board", "ROLESTATION");

				public static LocString DESC = "A skills board can teach special skills to Duplicants they can't learn on their own.";

				public static LocString EFFECT = "Allows Duplicants to spend Skill Points to learn new " + UI.FormatAsLink("Skills", "JOBS") + ".";
			}

			public class RESETSKILLSSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Skill Scrubber", "RESETSKILLSSTATION");

				public static LocString DESC = "Erase skills from a Duplicant's mind, returning them to their default abilities.";

				public static LocString EFFECT = "Refunds a Duplicant's Skill Points for reassignment.\n\nDuplicants will lose all assigned skills in the process.";
			}

			public class RESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Research Station", "RESEARCHCENTER");

				public static LocString DESC = "Research stations are necessary for unlocking all research tiers.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Novice Research", "RESEARCH") + " to unlock new technologies.\n\nConsumes " + UI.FormatAsLink("Dirt", "DIRT") + ".";
			}

			public class ADVANCEDRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Super Computer", "ADVANCEDRESEARCHCENTER");

				public static LocString DESC = "Super computers unlock higher technology tiers than research stations alone.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Advanced Research", "RESEARCH") + " to unlock new technologies.\n\nConsumes " + UI.FormatAsLink("Water", "WATER") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Advanced Research", "JUNIOR_RESEARCHER") + " skill.";
			}

			public class NUCLEARRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Materials Study Terminal", "NUCLEARLABORATORY");

				public static LocString DESC = "Comes with a few ions thrown in, free of charge.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Materials Science Research", "RESEARCH") + " to unlock new technologies.\n\nConsumes Radbolts.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Materials Science Research", "ATOMIC_RESEARCHER") + " skill.";
			}

			public class ORBITALRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Orbital Microlab", "ORBITALRESEARCHCENTER");

				public static LocString DESC = "Orbital Microlabs perform research from the orbit around a Planetoid.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Interstellar Research", "RESEARCH") + " to unlock new technologies.\n\nConsumes " + UI.FormatAsLink("Plastic", "POLYPROPYLENE") + " and " + UI.FormatAsLink("Power", "POWER") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Orbital Researcher", "COSMIC_RESEARCHER") + " skill.";
			}

			public class COSMICRESEARCHCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Virtual Planetarium", "COSMICRESEARCHCENTER");

				public static LocString DESC = "Planetariums allow the simulated exploration of locations discovered with a telescope.";

				public static LocString EFFECT = "Conducts " + UI.FormatAsLink("Interstellar Research", "RESEARCH") + " to unlock new technologies.\n\nConsumes data from " + UI.FormatAsLink("Telescopes", "TELESCOPE") + " and " + UI.FormatAsLink("Research Modules", "RESEARCHMODULE") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Astronomy", "SENIOR_RESEARCHER") + " skill.";
			}

			public class TELESCOPE
			{
				public static LocString NAME = UI.FormatAsLink("Telescope", "TELESCOPE");

				public static LocString DESC = "Telescopes are necessary for learning starmaps and conducting rocket missions.";

				public static LocString EFFECT = "Maps Starmap destinations.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Field Research", "RESEARCHER") + " skill.\n\nBuilding must be exposed to space to function.";

				public static LocString REQUIREMENT_TOOLTIP = "A steady {0} supply is required to sustain working Duplicants.";
			}

			public class CLUSTERTELESCOPE
			{
				public static LocString NAME = UI.FormatAsLink("Telescope", "TELESCOPE");

				public static LocString DESC = "Telescopes are necessary for studying space, allowing rocket travel to other worlds.";

				public static LocString EFFECT = "Reveals visitable Planetoids in space.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Astronomy", "SENIOR_RESEARCHER") + " skill.\n\nBuilding must be exposed to space to function.";

				public static LocString REQUIREMENT_TOOLTIP = "A steady {0} supply is required to sustain working Duplicants.";
			}

			public class SCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Large Sculpting Block", "SCULPTURE");

				public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";

				public static LocString EFFECT = "Moderately increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Sculpture";
			}

			public class ICESCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Ice Block", "ICESCULPTURE");

				public static LocString DESC = "Prone to melting.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Ice Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Ice Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Ice Sculpture";
			}

			public class MARBLESCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Marble Block", "MARBLESCULPTURE");

				public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Marble Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Marble Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Marble Sculpture";
			}

			public class METALSCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Metal Block", "METALSCULPTURE");

				public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";

				public static LocString EFFECT = "Majorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Metal Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Metal Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Metal Sculpture";
			}

			public class SMALLSCULPTURE
			{
				public static LocString NAME = UI.FormatAsLink("Sculpting Block", "SMALLSCULPTURE");

				public static LocString DESC = "Duplicants who have learned art skills can produce more decorative sculptures.";

				public static LocString EFFECT = "Minorly increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nMust be sculpted by a Duplicant.";

				public static LocString POORQUALITYNAME = "\"Abstract\" Sculpture";

				public static LocString AVERAGEQUALITYNAME = "Mediocre Sculpture";

				public static LocString EXCELLENTQUALITYNAME = "Genius Sculpture";
			}

			public class SHEARINGSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Shearing Station", "SHEARINGSTATION");

				public static LocString DESC = "Shearing stations allow " + UI.FormatAsLink("Dreckos", "DRECKO") + " to be safely sheared for useful raw materials.";

				public static LocString EFFECT = "Allows the assigned Rancher to shear Dreckos.";
			}

			public class OXYGENMASKSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Mask Station", "OXYGENMASKSTATION");

				public static LocString DESC = "Duplicants can't pass by a station if it lacks enough oxygen to fill a mask.";

				public static LocString EFFECT = "Uses designated " + UI.FormatAsLink("Metal Ores", "METAL") + " from filter settings to create " + UI.FormatAsLink("Oxygen Masks", "OXYGENMASK") + ".\n\nAutomatically draws in ambient " + UI.FormatAsLink("Oxygen", "OXYGEN") + " to fill masks.\n\nMarks a threshold where Duplicants must put on or take off a mask.\n\nCan be rotated before construction.";
			}

			public class SWEEPBOTSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Sweepy's Dock", "SWEEPBOTSTATION");

				public static LocString NAMEDSTATION = "{0}'s Dock";

				public static LocString DESC = "The cute little face comes pre-installed.";

				public static LocString EFFECT = "Deploys an automated " + UI.FormatAsLink("Sweepy Bot", "SWEEPBOT") + " to sweep up " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + " debris and " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " spills.\n\nDock stores " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") + " and " + UI.FormatAsLink("Solids", "ELEMENTS_SOLID") + " gathered by the Sweepy.\n\nUses " + UI.FormatAsLink("Power", "POWER") + " to recharge the Sweepy.\n\nDuplicants will empty Dock storage into available storage bins.";
			}

			public class OXYGENMASKMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Mask Checkpoint", "OXYGENMASKMARKER");

				public static LocString DESC = "A checkpoint must have a correlating dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must put on or take off an " + UI.FormatAsLink("Oxygen Mask", "OXYGEN_MASK") + ".\n\nMust be built next to an " + UI.FormatAsLink("Oxygen Mask Dock", "OXYGENMASKLOCKER") + ".\n\nCan be rotated before construction.";
			}

			public class OXYGENMASKLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Oxygen Mask Dock", "OXYGENMASKLOCKER");

				public static LocString DESC = "An oxygen mask dock will store and refill masks while they're not in use.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Oxygen Masks", "OXYGEN_MASK") + " and refuels them with " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".\n\nBuild next to an " + UI.FormatAsLink("Oxygen Mask Checkpoint", "OXYGENMASKMARKER") + " to make Duplicants put on masks when passing by.";
			}

			public class SUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit Checkpoint", "SUITMARKER");

				public static LocString DESC = "A checkpoint must have a correlating dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + ".\n\nMust be built next to an " + UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER") + ".\n\nCan be rotated before construction.";
			}

			public class SUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Suit Dock", "SUITLOCKER");

				public static LocString DESC = "An atmo suit dock will empty atmo suits of waste, but only one suit can charge at a time.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Atmo Suits", "ATMO_SUIT") + " and refuels them with " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".\n\nEmpties suits of " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".\n\nBuild next to an " + UI.FormatAsLink("Atmo Suit Checkpoint", "SUITMARKER") + " to make Duplicants change into suits when passing by.";
			}

			public class JETSUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit Checkpoint", "JETSUITMARKER");

				public static LocString DESC = "A checkpoint must have a correlating dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + UI.FormatAsLink("Jet Suits", "JET_SUIT") + ".\n\nMust be built next to a " + UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER") + ".\n\nCan be rotated before construction.";
			}

			public class JETSUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Jet Suit Dock", "JETSUITLOCKER");

				public static LocString DESC = "Jet suit docks can refill jet suits with air and fuel, or empty them of waste.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Jet Suits", "JET_SUIT") + " and refuels them with " + UI.FormatAsLink("Oxygen", "OXYGEN") + " and " + UI.FormatAsLink("Petroleum", "PETROLEUM") + ".\n\nEmpties suits of " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".\n\nBuild next to a " + UI.FormatAsLink("Jet Suit Checkpoint", "JETSUITMARKER") + " to make Duplicants change into suits when passing by.";
			}

			public class LEADSUITMARKER
			{
				public static LocString NAME = UI.FormatAsLink("Lead Suit Checkpoint", "LEADSUITMARKER");

				public static LocString DESC = "A checkpoint must have a correlating dock built on the opposite side its arrow faces.";

				public static LocString EFFECT = "Marks a threshold where Duplicants must change into or out of " + UI.FormatAsLink("Lead Suits", "LEAD_SUIT") + ".\n\nMust be built next to a " + UI.FormatAsLink("Lead Suit Dock", "LEADSUITLOCKER") + "\n\nCan be rotated before construction.";
			}

			public class LEADSUITLOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Lead Suit Dock", "LEADSUITLOCKER");

				public static LocString DESC = "Lead suit docks can refill lead suits with air and empty them of waste.";

				public static LocString EFFECT = "Stores " + UI.FormatAsLink("Lead Suits", "LEAD_SUIT") + " and refuels them  with " + UI.FormatAsLink("Oxygen", "OXYGEN") + ".\n\nEmpties suits of " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + ".\n\nBuild next to a " + UI.FormatAsLink("Lead Suit Checkpoint", "LEADSUITMARKER") + " to make Duplicants change into suits when passing by.";
			}

			public class CRAFTINGTABLE
			{
				public static LocString NAME = UI.FormatAsLink("Crafting Station", "CRAFTINGTABLE");

				public static LocString DESC = "Crafting stations allow Duplicants to make oxygen masks to wear in low breathability areas.";

				public static LocString EFFECT = "Produces items and equipment for Duplicant use.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class SUITFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Exosuit Forge", "SUITFABRICATOR");

				public static LocString DESC = "Exosuits can be filled with oxygen to allow Duplicants to safely enter hazardous areas.";

				public static LocString EFFECT = "Forges protective " + UI.FormatAsLink("Exosuits", "EXOSUIT") + " for Duplicants to wear.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class CLOTHINGFABRICATOR
			{
				public static LocString NAME = UI.FormatAsLink("Textile Loom", "CLOTHINGFABRICATOR");

				public static LocString DESC = "A textile loom can be used to spin Reed Fiber into wearable Duplicant clothing.";

				public static LocString EFFECT = "Tailors Duplicant " + UI.FormatAsLink("Clothing", "EQUIPMENT") + " items.\n\nDuplicants will not fabricate items unless recipes are queued.";
			}

			public class SOLIDBOOSTER
			{
				public static LocString NAME = UI.FormatAsLink("Solid Fuel Thruster", "SOLIDBOOSTER");

				public static LocString DESC = "Additional thrusters allow rockets to reach far away space destinations.";

				public static LocString EFFECT = "Burns " + UI.FormatAsLink("Refined Iron", "IRON") + " and " + UI.FormatAsLink("Oxylite", "OXYROCK") + " to increase rocket exploration distance.";
			}

			public class SPACEHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Space Heater", "SPACEHEATER");

				public static LocString DESC = "A space heater will radiate heat for as long as it's powered.";

				public static LocString EFFECT = "Radiates a moderate amount of " + UI.FormatAsLink("Heat", "HEAT") + ".";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = UI.FormatAsLink("Storage Bin", "STORAGELOCKER");

				public static LocString DESC = "Resources left on the floor become \"debris\" and lower decor when not put away.";

				public static LocString EFFECT = "Stores the " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.";
			}

			public class STORAGELOCKERSMART
			{
				public static LocString NAME = UI.FormatAsLink("Smart Storage Bin", "STORAGELOCKERSMART");

				public static LocString DESC = "Smart storage bins can automate resource organization based on type and mass.";

				public static LocString EFFECT = "Stores the " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.\n\nSends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when bin is full.";

				public static LocString LOGIC_PORT = "Full/Not Full";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when full";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class OBJECTDISPENSER
			{
				public static LocString NAME = UI.FormatAsLink("Automatic Dispenser", "OBJECTDISPENSER");

				public static LocString DESC = "Automatic dispensers will store and drop resources in small quantities.";

				public static LocString EFFECT = "Stores any " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " delivered to it by Duplicants.\n\nDumps stored materials back into the world when it receives a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".";

				public static LocString LOGIC_PORT = "Dump Trigger";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Dump all stored materials";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Store materials";
			}

			public class LIQUIDRESERVOIR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Reservoir", "LIQUIDRESERVOIR");

				public static LocString DESC = "Reservoirs cannot receive manually delivered resources.";

				public static LocString EFFECT = "Stores any " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
			}

			public class GASRESERVOIR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Reservoir", "GASRESERVOIR");

				public static LocString DESC = "Reservoirs cannot receive manually delivered resources.";

				public static LocString EFFECT = "Stores any " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
			}

			public class SMARTRESERVOIR
			{
				public static LocString LOGIC_PORT = "Refill Parameters";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when reservoir is less than <b>Low Threshold</b> full, until <b>High Threshold</b> is reached again";

				public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when reservoir is <b>High Threshold</b> full, until <b>Low Threshold</b> is reached again";

				public static LocString ACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when reservoir is less than <b>{0}%</b> full, until it is <b>{1}% (High Threshold)</b> full";

				public static LocString DEACTIVATE_TOOLTIP = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when reservoir is <b>{0}%</b> full, until it is less than <b>{1}% (Low Threshold)</b> full";

				public static LocString SIDESCREEN_TITLE = "Logic Activation Parameters";

				public static LocString SIDESCREEN_ACTIVATE = "Low Threshold:";

				public static LocString SIDESCREEN_DEACTIVATE = "High Threshold:";
			}

			public class LIQUIDHEATER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Tepidizer", "LIQUIDHEATER");

				public static LocString DESC = "Tepidizers heat liquid which can kill waterborne germs.";

				public static LocString EFFECT = "Warms large bodies of " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nMust be fully submerged.";
			}

			public class SWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Switch", "SWITCH");

				public static LocString DESC = "Switches can only affect buildings that come after them on a circuit.";

				public static LocString EFFECT = "Turns " + UI.FormatAsLink("Power", "POWER") + " on or off.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString SIDESCREEN_TITLE = "Switch";

				public static LocString TURN_ON = "Turn On";

				public static LocString TURN_ON_TOOLTIP = "Turn On {Hotkey}";

				public static LocString TURN_OFF = "Turn Off";

				public static LocString TURN_OFF_TOOLTIP = "Turn Off {Hotkey}";
			}

			public class LOGICPOWERRELAY
			{
				public static LocString NAME = UI.FormatAsLink("Power Shutoff", "LOGICPOWERRELAY");

				public static LocString DESC = "Automated systems save power and time by removing the need for Duplicant input.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Power", "POWER") + " on or off.\n\nDoes not affect circuitry preceding the switch.";

				public static LocString LOGIC_PORT = "Kill Power";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " through connected circuits";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Prevent " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " from flowing through connected circuits";
			}

			public class TEMPERATURECONTROLLEDSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Switch", "TEMPERATURECONTROLLEDSWITCH");

				public static LocString DESC = "Automated switches can be used to manage circuits in areas where Duplicants cannot enter.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Temperature", "HEAT") + ".\n\nDoes not affect circuitry preceding the switch.";
			}

			public class PRESSURESWITCHLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Switch", "PRESSURESWITCHLIQUID");

				public static LocString DESC = "A hydro switch shuts off power when the liquid pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Liquid Pressure", "PRESSURE") + ".\n\nDoes not affect circuitry preceding the switch.\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";
			}

			public class PRESSURESWITCHGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Switch", "PRESSURESWITCHGAS");

				public static LocString DESC = "An atmo switch shuts off power when the air pressure surrounding it surpasses the set threshold.";

				public static LocString EFFECT = "Automatically turns " + UI.FormatAsLink("Power", "POWER") + " on or off using ambient " + UI.FormatAsLink("Gas Pressure", "PRESSURE") + " .\n\nDoes not affect circuitry preceding the switch.";
			}

			public class TILE
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILE");

				public static LocString DESC = "Tile can be used to bridge gaps and get to unreachable areas.";

				public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nIncreases Duplicant runspeed.";
			}

			public class WATERPURIFIER
			{
				public static LocString NAME = UI.FormatAsLink("Water Sieve", "WATERPURIFIER");

				public static LocString DESC = "Sieves cannot kill germs and pass any they receive into their waste and water output.";

				public static LocString EFFECT = "Produces clean " + UI.FormatAsLink("Water", "WATER") + " from " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " using " + UI.FormatAsLink("Sand", "SAND") + ".\n\nProduces " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ".";
			}

			public class DISTILLATIONCOLUMN
			{
				public static LocString NAME = UI.FormatAsLink("Distillation Column", "DISTILLATIONCOLUMN");

				public static LocString DESC = "Gets hot and steamy.";

				public static LocString EFFECT = "Separates any " + UI.FormatAsLink("Contaminated Water", "DIRTYWATER") + " piped through it into " + UI.FormatAsLink("Steam", "STEAM") + " and " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND") + ".";
			}

			public class WIRE
			{
				public static LocString NAME = UI.FormatAsLink("Wire", "WIRE");

				public static LocString DESC = "Electrical wire is used to connect generators, batteries, and buildings.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through wall and floor tile.";
			}

			public class WIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Wire Bridge", "WIREBRIDGE");

				public static LocString DESC = "Splitting generators onto separate grids can prevent overloads and wasted electricity.";

				public static LocString EFFECT = "Runs one wire section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class HIGHWATTAGEWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular " + UI.FormatAsLink("Wire", "WIRE") + " without overloading.\n\nCannot be run through wall and floor tile.";
			}

			public class WIREBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Joint Plate", "WIREBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi-Watt wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE") + " to be run through wall and floor tile.\n\nFunctions as regular tile.";
			}

			public class WIREREFINED
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire", "WIREREFINED");

				public static LocString DESC = "My Duplicants prefer the look of conductive wire to the regular raggedy stuff.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through wall and floor tile.";
			}

			public class WIREREFINEDBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conductive Wire Bridge", "WIREREFINEDBRIDGE");

				public static LocString DESC = "Splitting generators onto separate systems can prevent overloads and wasted electricity.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular " + UI.FormatAsLink("Wire Bridge", "WIREBRIDGE") + " without overloading.\n\nRuns one wire section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class WIREREFINEDHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Conductive Wire", "WIREREFINEDHIGHWATTAGE");

				public static LocString DESC = "Higher wattage wire is used to avoid power overloads, particularly for strong generators.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than regular " + UI.FormatAsLink("Wire", "WIRE") + " without overloading.\n\nCannot be run through wall and floor tile.";
			}

			public class WIREREFINEDBRIDGEHIGHWATTAGE
			{
				public static LocString NAME = UI.FormatAsLink("Heavi-Watt Conductive Joint Plate", "WIREREFINEDBRIDGEHIGHWATTAGE");

				public static LocString DESC = "Joint plates can run Heavi-Watt wires through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Carries more " + UI.FormatAsLink("Wattage", "POWER") + " than a regular " + UI.FormatAsLink("Heavi-Watt Joint Plate", "WIREBRIDGEHIGHWATTAGE") + " without overloading.\n\nAllows " + UI.FormatAsLink("Heavi-Watt Wire", "HIGHWATTAGEWIRE") + " to be run through wall and floor tile.";
			}

			public class HANDSANITIZER
			{
				public static LocString NAME = UI.FormatAsLink("Hand Sanitizer", "HANDSANITIZER");

				public static LocString DESC = "Hand sanitizers kill germs more effectively than wash basins.";

				public static LocString EFFECT = "Removes most " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Hand Sanitizers when passing by in the selected direction.";
			}

			public class WASHBASIN
			{
				public static LocString NAME = UI.FormatAsLink("Wash Basin", "WASHBASIN");

				public static LocString DESC = "Germ spread can be reduced by building these where Duplicants often get dirty.";

				public static LocString EFFECT = "Removes some " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Wash Basins when passing by in the selected direction.";
			}

			public class WASHSINK
			{
				public static LocString NAME = UI.FormatAsLink("Sink", "WASHSINK");

				public static LocString DESC = "Sinks are plumbed and do not need to be manually emptied or refilled.";

				public static LocString EFFECT = "Removes " + UI.FormatAsLink("Germs", "DISEASE") + " from Duplicants.\n\nGerm-covered Duplicants use Sinks when passing by in the selected direction.";
			}

			public class DECONTAMINATIONSHOWER
			{
				public static LocString NAME = UI.FormatAsLink("Decontamination Shower", "DECONTAMINATIONSHOWER");

				public static LocString DESC = "Don't forget to decontaminate behind your ears.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Water", "WATER") + " to remove " + UI.FormatAsLink("Germs", "DISEASE") + " and " + UI.FormatAsLink("Radiation", "RADIATION") + ".\n\nDecontaminates both Duplicants and their " + UI.FormatAsLink("Clothing", "EQUIPMENT") + ".";
			}

			public class TILEPOI
			{
				public static LocString NAME = UI.FormatAsLink("Tile", "TILEPOI");

				public static LocString DESC = "";

				public static LocString EFFECT = "Used to build the walls and floor of rooms.";
			}

			public class POLYMERIZER
			{
				public static LocString NAME = UI.FormatAsLink("Polymer Press", "POLYMERIZER");

				public static LocString DESC = "Plastic can be used to craft unique buildings and goods.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " into raw " + UI.FormatAsLink("Plastic", "POLYPROPYLENE") + ".";
			}

			public class DIRECTIONALWORLDPUMPLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Channel", "DIRECTIONALWORLDPUMPLIQUID");

				public static LocString DESC = "Channels move more volume than pumps and require no power, but need sufficient pressure to function.";

				public static LocString EFFECT = "Directionally moves large volumes of " + UI.FormatAsLink("LIQUID", "ELEMENTS_LIQUID") + " through a channel.\n\nCan be used as floor tile and rotated before construction.";
			}

			public class STEAMTURBINE
			{
				public static LocString NAME = UI.FormatAsLink("[DEPRECATED] Steam Turbine", "STEAMTURBINE");

				public static LocString DESC = "Useful for converting the geothermal energy of magma into usable power.";

				public static LocString EFFECT = "THIS BUILDING HAS BEEN DEPRECATED AND CANNOT BE BUILT.\n\nGenerates exceptional electrical " + UI.FormatAsLink("Power", "POWER") + " using pressurized, " + UI.FormatAsLink("Scalding", "HEAT") + " " + UI.FormatAsLink("Steam", "STEAM") + ".\n\nOutputs significantly cooler " + UI.FormatAsLink("Steam", "STEAM") + " than it receives.\n\nAir pressure beneath this building must be higher than pressure above for air to flow.";
			}

			public class STEAMTURBINE2
			{
				public static LocString NAME = UI.FormatAsLink("Steam Turbine", "STEAMTURBINE2");

				public static LocString DESC = "Useful for converting the geothermal energy into usable power.";

				public static LocString EFFECT = "Draws in " + UI.FormatAsLink("Steam", "STEAM") + " from the tiles directly below the machine's foundation and uses it to generate electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nOutputs " + UI.FormatAsLink("Water", "WATER") + ".";

				public static LocString HEAT_SOURCE = "Power Generation Waste";
			}

			public class STEAMENGINE
			{
				public static LocString NAME = UI.FormatAsLink("Steam Engine", "STEAMENGINE");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Utilizes " + UI.FormatAsLink("Steam", "STEAM") + " to propel rockets for space exploration.\n\nThe engine of a rocket must be built first before more rocket modules may be added.";
			}

			public class STEAMENGINECLUSTER
			{
				public static LocString NAME = UI.FormatAsLink("Steam Engine", "STEAMENGINECLUSTER");

				public static LocString DESC = "Rockets can be used to send Duplicants into space and retrieve rare resources.";

				public static LocString EFFECT = "Utilizes " + UI.FormatAsLink("Steam", "STEAM") + " to propel rockets for space exploration.\n\nThe engine of a rocket must be built first before more rocket modules may be added.";
			}

			public class SOLARPANEL
			{
				public static LocString NAME = UI.FormatAsLink("Solar Panel", "SOLARPANEL");

				public static LocString DESC = "Solar panels convert high intensity sunlight into power and produce zero waste.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Sunlight", "LIGHT") + " into electrical " + UI.FormatAsLink("Power", "POWER") + ".\n\nMust be exposed to space.";
			}

			public class COMETDETECTOR
			{
				public static LocString NAME = UI.FormatAsLink("Space Scanner", "COMETDETECTOR");

				public static LocString DESC = "Networks of many scanners will scan more efficiently than one on its own.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to its automation circuit when it detects incoming objects from space.\n\nCan be configured to detect incoming meteor showers or returning space rockets.";
			}

			public class OILREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Oil Refinery", "OILREFINERY");

				public static LocString DESC = "Petroleum can only be produced from the refinement of crude oil.";

				public static LocString EFFECT = "Converts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " into " + UI.FormatAsLink("Petroleum", "PETROLEUM") + " and " + UI.FormatAsLink("Natural Gas", "METHANE") + ".";
			}

			public class OILWELLCAP
			{
				public static LocString NAME = UI.FormatAsLink("Oil Well", "OILWELLCAP");

				public static LocString DESC = "Water pumped into an oil reservoir cannot be recovered.";

				public static LocString EFFECT = "Extracts " + UI.FormatAsLink("Crude Oil", "CRUDEOIL") + " using clean " + UI.FormatAsLink("Water", "WATER") + ".\n\nMust be built atop an " + UI.FormatAsLink("Oil Reservoir", "OIL_WELL") + ".";
			}

			public class METALREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Metal Refinery", "METALREFINERY");

				public static LocString DESC = "Refined metals are necessary to build advanced electronics and technologies.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Refined Metals", "REFINEDMETAL") + " from raw " + UI.FormatAsLink("Metal Ore", "RAWMETAL") + ".\n\nSignificantly " + UI.FormatAsLink("Heats", "HEAT") + " and outputs the " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " piped into it.\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class GLASSFORGE
			{
				public static LocString NAME = UI.FormatAsLink("Glass Forge", "GLASSFORGE");

				public static LocString DESC = "Glass can be used to construct window tile.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Molten Glass", "MOLTENGLASS") + " from raw " + UI.FormatAsLink("Sand", "SAND") + ".\n\nOutputs " + UI.FormatAsLink("High Temperature", "HEAT") + " " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Extracts pure {0} from {1}.";
			}

			public class ROCKCRUSHER
			{
				public static LocString NAME = UI.FormatAsLink("Rock Crusher", "ROCKCRUSHER");

				public static LocString DESC = "Rock Crushers loosen nuggets from raw ore and can process many different resources.";

				public static LocString EFFECT = "Inefficiently produces refined materials from raw resources.\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Crushes {0} into {1}.";

				public static LocString METAL_RECIPE_DESCRIPTION = "Crushes {1} into " + UI.FormatAsLink("Sand", "SAND") + " and pure {0}.";

				public static LocString LIME_RECIPE_DESCRIPTION = "Crushes {1} into {0}";

				public static LocString LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION = "Crushes {0} into {1} and a small amount of pure {2}";
			}

			public class SLUDGEPRESS
			{
				public static LocString NAME = UI.FormatAsLink("Sludge Press", "SLUDGEPRESS");

				public static LocString DESC = "What Duplicant doesn't love playing with mud?";

				public static LocString EFFECT = "Separates " + UI.FormatAsLink("Mud", "MUD") + " and other sludges into their base elements.\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString RECIPE_DESCRIPTION = "Separates {0} into its base elements.";
			}

			public class SUPERMATERIALREFINERY
			{
				public static LocString NAME = UI.FormatAsLink("Molecular Forge", "SUPERMATERIALREFINERY");

				public static LocString DESC = "Rare materials can be procured through rocket missions into space.";

				public static LocString EFFECT = "Processes " + UI.FormatAsLink("Rare Materials", "RAREMATERIALS") + " into advanced industrial goods.\n\nRare materials can be retrieved from space missions.\n\nDuplicants will not fabricate items unless recipes are queued.";

				public static LocString SUPERCOOLANT_RECIPE_DESCRIPTION = "Super Coolant is an industrial grade " + UI.FormatAsLink("Fullerene", "FULLERENE") + " coolant.";

				public static LocString SUPERINSULATOR_RECIPE_DESCRIPTION = "Insulation reduces " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and is composed of recrystallized " + UI.FormatAsLink("Abyssalite", "KATAIRITE") + ".";

				public static LocString TEMPCONDUCTORSOLID_RECIPE_DESCRIPTION = "Thermium is an industrial metal alloy formulated to maximize " + UI.FormatAsLink("Heat Transfer", "HEAT") + " and thermal dispersion.";

				public static LocString VISCOGEL_RECIPE_DESCRIPTION = "Visco-Gel is a " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " polymer with high surface tension.";

				public static LocString YELLOWCAKE_RECIPE_DESCRIPTION = "Yellowcake is a " + UI.FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " used in uranium enrichment.";

				public static LocString FULLERENE_RECIPE_DESCRIPTION = "Fullerene is a " + UI.FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " used in the production of " + UI.FormatAsLink("Super Coolant", "SUPERCOOLANT") + ".";
			}

			public class THERMALBLOCK
			{
				public static LocString NAME = UI.FormatAsLink("Tempshift Plate", "THERMALBLOCK");

				public static LocString DESC = "The thermal properties of construction materials determine their heat retention.";

				public static LocString EFFECT = "Accelerates or buffers " + UI.FormatAsLink("Heat", "HEAT") + " dispersal based on the construction material used.\n\nHas a small area of effect.";
			}

			public class POWERCONTROLSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Power Control Station", "POWERCONTROLSTATION");

				public static LocString DESC = "Only one Duplicant may be assigned to a station at a time.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Microchip", "POWER_STATION_TOOLS") + " to increase the " + UI.FormatAsLink("Power", "POWER") + " output of generators.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Tune Up", "POWER_TECHNICIAN") + " trait.\n\nThis building is a necessary component of the Power Plant room.";
			}

			public class FARMSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Farm Station", "FARMSTATION");

				public static LocString DESC = "This station only has an effect on crops grown within the same room.";

				public static LocString EFFECT = "Produces " + UI.FormatAsLink("Micronutrient Fertilizer", "FARM_STATION_TOOLS") + " to increase " + UI.FormatAsLink("Plant", "PLANTS") + " growth rates.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Crop Tending", "FARMER") + " trait.\n\nThis building is a necessary component of the Greenhouse room.";
			}

			public class FISHDELIVERYPOINT
			{
				public static LocString NAME = UI.FormatAsLink("Fish Release", "FISHDELIVERYPOINT");

				public static LocString DESC = "A fish release must be built above liquid to prevent released fish from suffocating.";

				public static LocString EFFECT = "Releases trapped " + UI.FormatAsLink("Pacu", "PACU") + " back into the world.\n\nCan be used multiple times.";
			}

			public class FISHFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Fish Feeder", "FISHFEEDER");

				public static LocString DESC = "Build this feeder above a body of water to feed the fish within.";

				public static LocString EFFECT = "Automatically dispenses stored " + UI.FormatAsLink("Critter", "CRITTERS") + " food into the area below.\n\nDispenses once per day.";
			}

			public class FISHTRAP
			{
				public static LocString NAME = UI.FormatAsLink("Fish Trap", "FISHTRAP");

				public static LocString DESC = "Trapped fish will automatically be bagged for transport.";

				public static LocString EFFECT = "Attracts and traps swimming " + UI.FormatAsLink("Pacu", "PACU") + ".\n\nSingle use.";
			}

			public class RANCHSTATION
			{
				public static LocString NAME = UI.FormatAsLink("Grooming Station", "RANCHSTATION");

				public static LocString DESC = "Grooming critters make them look nice, smell pretty, feel happy, and produce more.";

				public static LocString EFFECT = "Allows the assigned " + UI.FormatAsLink("Rancher", "RANCHER") + " to care for " + UI.FormatAsLink("Critters", "CRITTERS") + ".\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", "RANCHER") + " trait.\n\nThis building is a necessary component of the Stable room.";
			}

			public class MACHINESHOP
			{
				public static LocString NAME = UI.FormatAsLink("Mechanics Station", "MACHINESHOP");

				public static LocString DESC = "Duplicants will only improve the efficiency of buildings in the same room as this station.";

				public static LocString EFFECT = "Allows the assigned " + UI.FormatAsLink("Engineer", "MACHINE_TECHNICIAN") + " to improve building production efficiency.\n\nThis building is a necessary component of the Machine Shop room.";
			}

			public class LOGICWIRE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire", "LOGICWIRE");

				public static LocString DESC = "Automation wire is used to connect building ports to automation gates.";

				public static LocString EFFECT = "Connects buildings to " + UI.FormatAsLink("Sensors", "LOGIC") + ".\n\nCan be run through wall and floor tile.";
			}

			public class LOGICRIBBON
			{
				public static LocString NAME = UI.FormatAsLink("Automation Ribbon", "LOGICRIBBON");

				public static LocString DESC = "Logic ribbons use significantly less space to carry multiple automation signals.";

				public static LocString EFFECT = string.Concat("A 4-Bit ", LOGICWIRE.NAME, " which can carry up to four automation signals.\n\nUse a ", UI.FormatAsLink("Ribbon Writer", "LOGICRIBBONWRITER"), " to output to multiple Bits, and a ", UI.FormatAsLink("Ribbon Reader", "LOGICRIBBONREADER"), " to input from multiple Bits.");
			}

			public class LOGICWIREBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Wire Bridge", "LOGICWIREBRIDGE");

				public static LocString DESC = "Wire bridges allow multiple automation grids to exist in a small area without connecting.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Automation Wire", "LOGICWIRE") + " section over another without joining them.\n\nCan be run through wall and floor tile.";

				public static LocString LOGIC_PORT = "Transmit Signal";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Pass through the " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Pass through the " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICRIBBONBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Automation Ribbon Bridge", "LOGICRIBBONBRIDGE");

				public static LocString DESC = "Wire bridges allow multiple automation grids to exist in a small area without connecting.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Automation Ribbon", "LOGICRIBBON") + " section over another without joining them.\n\nCan be run through wall and floor tile.";

				public static LocString LOGIC_PORT = "Transmit Signal";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Pass through the " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Pass through the " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICGATEAND
			{
				public static LocString NAME = UI.FormatAsLink("AND Gate", "LOGICGATEAND");

				public static LocString DESC = "This gate outputs a Green Signal when both its inputs are receiving Green Signals at the same time.";

				public static LocString EFFECT = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when both Input A <b>AND</b> Input B are receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ".\n\nOutputs a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when even one Input is receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ".";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if both Inputs are receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

				public static LocString OUTPUT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if any Input is receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);
			}

			public class LOGICGATEOR
			{
				public static LocString NAME = UI.FormatAsLink("OR Gate", "LOGICGATEOR");

				public static LocString DESC = "This gate outputs a Green Signal if receiving one or more Green Signals.";

				public static LocString EFFECT = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if at least one of Input A <b>OR</b> Input B is receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ".\n\nOutputs a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when neither Input A or Input B are receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ".";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if any Input is receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

				public static LocString OUTPUT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if both Inputs are receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);
			}

			public class LOGICGATENOT
			{
				public static LocString NAME = UI.FormatAsLink("NOT Gate", "LOGICGATENOT");

				public static LocString DESC = "This gate reverses automation signals, turning a Green Signal into a Red Signal and vice versa.";

				public static LocString EFFECT = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the Input is receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".\n\nOutputs a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when its Input is receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);

				public static LocString OUTPUT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);
			}

			public class LOGICGATEXOR
			{
				public static LocString NAME = UI.FormatAsLink("XOR Gate", "LOGICGATEXOR");

				public static LocString DESC = "This gate outputs a Green Signal if exactly one of its Inputs is receiving a Green Signal.";

				public static LocString EFFECT = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if exactly one of its Inputs is receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ".\n\nOutputs a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if both or neither Inputs are receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if exactly one of its Inputs is receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

				public static LocString OUTPUT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if both Input signals match (any color)";
			}

			public class LOGICGATEBUFFER
			{
				public static LocString NAME = UI.FormatAsLink("BUFFER Gate", "LOGICGATEBUFFER");

				public static LocString DESC = "This gate continues outputting a Green Signal for a short time after the gate stops receiving a Green Signal.";

				public static LocString EFFECT = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the Input is receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".\n\nContinues sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for an amount of buffer time after the Input receives a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". After receiving " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ", will continue sending " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " until the timer has expired";

				public static LocString OUTPUT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".";
			}

			public class LOGICGATEFILTER
			{
				public static LocString NAME = UI.FormatAsLink("FILTER Gate", "LOGICGATEFILTER");

				public static LocString DESC = "This gate only lets a Green Signal through if its Input has received a Green Signal that lasted longer than the selected filter time.";

				public static LocString EFFECT = "Only lets a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " through if the Input has received a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for longer than the selected filter time.\n\nWill continue outputting a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if the " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " did not last long enough.";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " after receiving " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " for longer than the selected filter timer";

				public static LocString OUTPUT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".";
			}

			public class LOGICMEMORY
			{
				public static LocString NAME = UI.FormatAsLink("Memory Toggle", "LOGICMEMORY");

				public static LocString DESC = "A Memory stores a Green Signal received in the Set Port (S) until the Reset Port (R) receives a Green Signal.";

				public static LocString EFFECT = "Contains an internal Memory, and will output whatever signal is stored in that Memory. Signals sent to the Inputs <i>only</i> affect the Memory, and do not pass through to the Output. \n\nSending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to the Set Port (S) will set the memory to " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + ". \n\nSending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to the Reset Port (R) will reset the memory back to " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + ".";

				public static LocString STATUS_ITEM_VALUE = "Current Value: {0}";

				public static LocString READ_PORT = "MEMORY OUTPUT";

				public static LocString READ_PORT_ACTIVE = "Outputs a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the internal Memory is set to " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

				public static LocString READ_PORT_INACTIVE = "Outputs a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if the internal Memory is set to " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);

				public static LocString SET_PORT = "SET PORT (S)";

				public static LocString SET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Set the internal Memory to " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active);

				public static LocString SET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": No effect";

				public static LocString RESET_PORT = "RESET PORT (R)";

				public static LocString RESET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reset the internal Memory to " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby);

				public static LocString RESET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": No effect";
			}

			public class LOGICGATEMULTIPLEXER
			{
				public static LocString NAME = UI.FormatAsLink("Signal Selector", "LOGICGATEMULTIPLEXER");

				public static LocString DESC = "Signal Selectors can be used to select which automation signal is relevant to pass through to a given circuit";

				public static LocString EFFECT = "Select which one of four Input signals should be sent out the Output, using Control Inputs.\n\nSend a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to the two Control Inputs to determine which Input is selected.";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Receives a " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " or " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + " signal from the selected input";

				public static LocString OUTPUT_INACTIVE = "Nothing";
			}

			public class LOGICGATEDEMULTIPLEXER
			{
				public static LocString NAME = UI.FormatAsLink("Signal Distributor", "LOGICGATEDEMULTIPLEXER");

				public static LocString DESC = "Signal Distributors can be used to choose which circuit should receive a given automation signal.";

				public static LocString EFFECT = "Route a single Input signal out one of four possible Outputs, based on the selection made by the Control Inputs.\n\nSend a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to the two Control Inputs to determine which Output is selected.";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " or " + UI.FormatAsAutomationState("Red", UI.AutomationState.Standby) + " signal to the selected output";

				public static LocString OUTPUT_INACTIVE = "Nothing";
			}

			public class LOGICSWITCH
			{
				public static LocString NAME = UI.FormatAsLink("Signal Switch", "LOGICSWITCH");

				public static LocString DESC = "Signal switches don't turn grids on and off like power switches, but add an extra signal.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " on an " + UI.FormatAsLink("Automation", "LOGIC") + " grid.";

				public static LocString SIDESCREEN_TITLE = "Signal Switch";

				public static LocString LOGIC_PORT = "Signal Toggle";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if toggled ON";

				public static LocString LOGIC_PORT_INACTIVE = "Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " if toggled OFF";
			}

			public class LOGICPRESSURESENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Atmo Sensor", "LOGICPRESSURESENSORGAS");

				public static LocString DESC = "Atmo sensors can be used to prevent excess oxygen production and overpressurization.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " pressure enters the chosen range.";

				public static LocString LOGIC_PORT = UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Pressure";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if Gas pressure is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICPRESSURESENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Hydro Sensor", "LOGICPRESSURESENSORLIQUID");

				public static LocString DESC = "A hydro sensor can tell a pump to refill its basin as soon as it contains too little liquid.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " pressure enters the chosen range.\n\nMust be submerged in " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ".";

				public static LocString LOGIC_PORT = UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Pressure";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if Liquid pressure is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Thermo Sensor", "LOGICTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors can disable buildings when they approach dangerous temperatures.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when ambient " + UI.FormatAsLink("Temperature", "HEAT") + " enters the chosen range.";

				public static LocString LOGIC_PORT = "Ambient " + UI.FormatAsLink("Temperature", "HEAT");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if ambient " + UI.FormatAsLink("Temperature", "HEAT") + " is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICWATTAGESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Wattage Sensor", "LOGICWATTSENSOR");

				public static LocString DESC = "Wattage sensors can send a signal when a building has switched on or off.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when " + UI.FormatAsLink("Wattage", "POWER") + " consumed enters the chosen range.";

				public static LocString LOGIC_PORT = "Consumed " + UI.FormatAsLink("Wattage", "POWER");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if current " + UI.FormatAsLink("Wattage", "POWER") + " is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICTIMEOFDAYSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Cycle Sensor", "LOGICTIMEOFDAYSENSOR");

				public static LocString DESC = "Cycle sensors ensure systems always turn on at the same time, day or night, every cycle.";

				public static LocString EFFECT = "Sets an automatic " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " and " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " schedule within one day-night cycle.";

				public static LocString LOGIC_PORT = "Cycle Time";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if current time is within the selected " + UI.FormatAsAutomationState("Green", UI.AutomationState.Active) + " range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICTIMERSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Timer Sensor", "LOGICTIMERSENSOR");

				public static LocString DESC = "Timer sensors create automation schedules for very short or very long periods of time.";

				public static LocString EFFECT = "Creates a timer to send " + UI.FormatAsAutomationState("Green Signals", UI.AutomationState.Active) + " and " + UI.FormatAsAutomationState("Red Signals", UI.AutomationState.Standby) + " for specific amounts of time.";

				public static LocString LOGIC_PORT = "Timer Schedule";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " for the selected amount of Green time";

				public static LocString LOGIC_PORT_INACTIVE = "Then, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " for the selected amount of Red time";
			}

			public class LOGICCRITTERCOUNTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Critter Sensor", "LOGICCRITTERCOUNTSENSOR");

				public static LocString DESC = "Detecting critter populations can help adjust their automated feeding and care regiments.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the number of eggs and critters in a room.";

				public static LocString LOGIC_PORT = "Critter Count";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the number of Critters and Eggs in the Room is greater than the selected threshold.";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString SIDESCREEN_TITLE = "Critter Sensor";

				public static LocString COUNT_CRITTER_LABEL = "Count Critters";

				public static LocString COUNT_EGG_LABEL = "Count Eggs";
			}

			public class LOGICDUPLICANTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Duplicant Motion Sensor", "DUPLICANTSENSOR");

				public static LocString DESC = "Motion sensors save power by only enabling buildings when Duplicants are nearby.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on whether a Duplicant is in the sensor's range.";

				public static LocString LOGIC_PORT = "Duplicant Motion Sensor";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " while a Duplicant is in the sensor's tile range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Germ Sensor", "LOGICDISEASESENSOR");

				public static LocString DESC = "Detecting germ populations can help block off or clean up dangerous areas.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on quantity of surrounding " + UI.FormatAsLink("Germs", "DISEASE") + ".";

				public static LocString LOGIC_PORT = UI.FormatAsLink("Germ", "DISEASE") + " Count";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the number of Germs is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICELEMENTSENSORGAS
			{
				public static LocString NAME = UI.FormatAsLink("Gas Element Sensor", "LOGICELEMENTSENSORGAS");

				public static LocString DESC = "These sensors can detect the presence of a specific gas and alter systems accordingly.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the selected " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is detected on this sensor's tile.\n\nSends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the selected " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is not present.";

				public static LocString LOGIC_PORT = "Specific " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Presence";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the selected Gas is detected";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICELEMENTSENSORLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Element Sensor", "LOGICELEMENTSENSORLIQUID");

				public static LocString DESC = "These sensors can detect the presence of a specific liquid and alter systems accordingly.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the selected " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is detected on this sensor's tile.\n\nSends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when the selected " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is not present.";

				public static LocString LOGIC_PORT = "Specific " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Presence";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the selected Liquid is detected";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICRADIATIONSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Radiation Sensor", "LOGICRADIATIONSENSOR");

				public static LocString DESC = "Radiation sensors can disable buildings when they detect dangerous levels of radiation.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when ambient " + UI.FormatAsLink("Radiation", "RADIATION") + " enters the chosen range.";

				public static LocString LOGIC_PORT = "Ambient " + UI.FormatAsLink("Radiation", "RADIATION");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if ambient " + UI.FormatAsLink("Radiation", "RADIATION") + " is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class GASCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Germ Sensor", "GASCONDUITDISEASESENSOR");

				public static LocString DESC = "Germ sensors can help control automation behavior in the presence of germs.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the internal " + UI.FormatAsLink("Germ", "DISEASE") + " count of the pipe.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Germ", "DISEASE") + " Count";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the number of Germs in the pipe is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LIQUIDCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Germ Sensor", "LIQUIDCONDUITDISEASESENSOR");

				public static LocString DESC = "Germ sensors can help control automation behavior in the presence of germs.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the internal " + UI.FormatAsLink("Germ", "DISEASE") + " count of the pipe.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Germ", "DISEASE") + " Count";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the number of Germs in the pipe is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class SOLIDCONDUITDISEASESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail Germ Sensor", "SOLIDCONDUITDISEASESENSOR");

				public static LocString DESC = "Germ sensors can help control automation behavior in the presence of germs.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " based on the internal " + UI.FormatAsLink("Germ", "DISEASE") + " count of the object on the rail.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Germ", "DISEASE") + " Count";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the number of Germs on the object on the rail is within the selected range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class GASCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Element Sensor", "GASCONDUITELEMENTSENSOR");

				public static LocString DESC = "Element sensors can be used to detect the presence of a specific gas in a pipe.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the selected " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " is detected within a pipe.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " Presence";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the configured Gas is detected";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LIQUIDCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Element Sensor", "LIQUIDCONDUITELEMENTSENSOR");

				public static LocString DESC = "Element sensors can be used to detect the presence of a specific liquid in a pipe.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the selected " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " is detected within a pipe.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " Presence";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the configured Liquid is detected within the pipe";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class SOLIDCONDUITELEMENTSENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail Element Sensor", "SOLIDCONDUITELEMENTSENSOR");

				public static LocString DESC = "Element sensors can be used to detect the presence of a specific item on a rail.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the selected item is detected on a rail.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Item", "ELEMENTS_LIQUID") + " Presence";

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the configured item is detected on the rail";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class GASCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Gas Pipe Thermo Sensor", "GASCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors disable buildings when their pipe contents reach a certain temperature.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when pipe contents enter the chosen " + UI.FormatAsLink("Temperature", "HEAT") + " range.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " " + UI.FormatAsLink("Temperature", "HEAT");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the contained Gas is within the selected Temperature range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LIQUIDCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Pipe Thermo Sensor", "LIQUIDCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors disable buildings when their pipe contents reach a certain temperature.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when pipe contents enter the chosen " + UI.FormatAsLink("Temperature", "HEAT") + " range.";

				public static LocString LOGIC_PORT = "Internal " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " " + UI.FormatAsLink("Temperature", "HEAT");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the contained Liquid is within the selected Temperature range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class SOLIDCONDUITTEMPERATURESENSOR
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail Thermo Sensor", "SOLIDCONDUITTEMPERATURESENSOR");

				public static LocString DESC = "Thermo sensors disable buildings when their rail contents reach a certain temperature.";

				public static LocString EFFECT = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " or a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " when rail contents enter the chosen " + UI.FormatAsLink("Temperature", "HEAT") + " range.";

				public static LocString LOGIC_PORT = "Internal item " + UI.FormatAsLink("Temperature", "HEAT");

				public static LocString LOGIC_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if the contained item is within the selected Temperature range";

				public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICCOUNTER
			{
				public static LocString NAME = UI.FormatAsLink("Signal Counter", "LOGICCOUNTER");

				public static LocString DESC = "For numbers higher than ten connect multiple counters together.";

				public static LocString EFFECT = "Counts how many times a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " has been received up to a chosen number.\n\nWhen the chosen number is reached it sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " until it receives another " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ", when it resets automatically and begins counting again.";

				public static LocString LOGIC_PORT = "Internal Counter Value";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Increase counter by one";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";

				public static LocString LOGIC_PORT_RESET = "Reset Counter";

				public static LocString RESET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reset counter";

				public static LocString RESET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";

				public static LocString LOGIC_PORT_OUTPUT = "Number Reached";

				public static LocString OUTPUT_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " when the counter matches the selected value";

				public static LocString OUTPUT_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICALARM
			{
				public static LocString NAME = UI.FormatAsLink("Automated Notifier", "LOGICALARM");

				public static LocString DESC = "Sends a notification when it receives a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".";

				public static LocString EFFECT = "Attach to sensors to send a notification when certain conditions are met.\n\nNotifications can be customized.";

				public static LocString LOGIC_PORT = "Notification";

				public static LocString INPUT_NAME = "INPUT";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Push notification";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";
			}

			public class PIXELPACK
			{
				public static LocString NAME = UI.FormatAsLink("Pixel Pack", "PIXELPACK");

				public static LocString DESC = "Four pixels which can be individually designated different colors.";

				public static LocString EFFECT = "Pixels can be designated a color when it receives a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " and a different color when it receives a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ".\n\nInput from an " + UI.FormatAsLink("Automation Wire", "LOGICWIRE") + " controls the whole strip. Input from an " + UI.FormatAsLink("Automation Ribbon", "LOGICRIBBON") + " can control individual pixels on the strip.";

				public static LocString LOGIC_PORT = "Color Selection";

				public static LocString INPUT_NAME = "RIBBON INPUT";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Display the configured " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " pixels";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Display the configured " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " pixels";

				public static LocString SIDESCREEN_TITLE = "Pixel Pack";
			}

			public class LOGICHAMMER
			{
				public static LocString NAME = UI.FormatAsLink("Hammer", "LOGICHAMMER");

				public static LocString DESC = "The hammer makes neat sounds when it strikes buildings.";

				public static LocString EFFECT = "In its default orientation, the hammer strikes the building to the left when it receives a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ".\n\nEach building has a unique sound when struck by the hammer.\n\nThe hammer does no damage when it strikes.";

				public static LocString LOGIC_PORT = "Resonating Buildings";

				public static LocString INPUT_NAME = "INPUT";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Hammer strikes once";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";
			}

			public class LOGICRIBBONWRITER
			{
				public static LocString NAME = UI.FormatAsLink("Ribbon Writer", "LOGICRIBBONWRITER");

				public static LocString DESC = "Translates the signal from an " + UI.FormatAsLink("Automation Wire", "LOGICWIRE") + " to a single Bit in an " + UI.FormatAsLink("Automation Ribbon", "LOGICRIBBON");

				public static LocString EFFECT = string.Concat("Writes a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " or a ", UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby), " to the specified Bit of an ", LOGICRIBBON.NAME, "\n\n", LOGICRIBBON.NAME, " must be used as the output wire to avoid overloading.");

				public static LocString LOGIC_PORT = "1-Bit Input";

				public static LocString INPUT_NAME = "INPUT";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Receives " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to be written to selected Bit";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Receives " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to to be written selected Bit";

				public static LocString LOGIC_PORT_OUTPUT = "Bit Writing";

				public static LocString OUTPUT_NAME = "RIBBON OUTPUT";

				public static LocString OUTPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Writes a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to selected Bit of an " + LOGICRIBBON.NAME;

				public static LocString OUTPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Writes a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to selected Bit of an " + LOGICRIBBON.NAME;
			}

			public class LOGICRIBBONREADER
			{
				public static LocString NAME = UI.FormatAsLink("Ribbon Reader", "LOGICRIBBONREADER");

				public static LocString DESC = "Inputs the signal from a single Bit in an " + UI.FormatAsLink("Automation Ribbon", "LOGICRIBBON") + " into an " + UI.FormatAsLink("Automation Wire", "LOGICWIRE") + ".";

				public static LocString EFFECT = string.Concat("Reads a ", UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active), " or a ", UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby), " from the specified Bit of an ", LOGICRIBBON.NAME, " onto an ", LOGICWIRE.NAME, ".");

				public static LocString LOGIC_PORT = "4-Bit Input";

				public static LocString INPUT_NAME = "RIBBON INPUT";

				public static LocString INPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reads a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " from selected Bit";

				public static LocString INPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Reads a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " from selected Bit";

				public static LocString LOGIC_PORT_OUTPUT = "Bit Reading";

				public static LocString OUTPUT_NAME = "OUTPUT";

				public static LocString OUTPUT_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " to attached " + UI.FormatAsLink("Automation Wire", "LOGICWIRE");

				public static LocString OUTPUT_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + " to attached " + UI.FormatAsLink("Automation Wire", "LOGICWIRE");
			}

			public class TRAVELTUBEENTRANCE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE");

				public static LocString DESC = "Duplicants require access points to enter tubes, but not to exit them.";

				public static LocString EFFECT = "Allows Duplicants to enter the connected " + UI.FormatAsLink("Transit Tube", "TRAVELTUBE") + " system.\n\nStops drawing " + UI.FormatAsLink("Power", "POWER") + " once fully charged.";
			}

			public class TRAVELTUBE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube", "TRAVELTUBE");

				public static LocString DESC = "Duplicants will only exit a transit tube when a safe landing area is available beneath it.";

				public static LocString EFFECT = "Quickly transports Duplicants from a " + UI.FormatAsLink("Transit Tube Access", "TRAVELTUBEENTRANCE") + " to the tube's end.\n\nOnly transports Duplicants.";
			}

			public class TRAVELTUBEWALLBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Transit Tube Crossing", "TRAVELTUBEWALLBRIDGE");

				public static LocString DESC = "Tube crossings can run transit tubes through walls without leaking gas or liquid.";

				public static LocString EFFECT = "Allows " + UI.FormatAsLink("Transit Tubes", "TRAVELTUBE") + " to be run through wall and floor tile.\n\nFunctions as regular tile.";
			}

			public class SOLIDCONDUIT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT");

				public static LocString DESC = "Rails move materials where they'll be needed most, saving Duplicants the walk.";

				public static LocString EFFECT = "Transports " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " on a track between " + UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX") + " and " + UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX") + ".\n\nCan be run through wall and floor tile.";
			}

			public class SOLIDCONDUITINBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX");

				public static LocString DESC = "Material filters can be used to determine what resources are sent down the rail.";

				public static LocString EFFECT = "Loads " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " onto " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " for transport.\n\nOnly loads the resources of your choosing.";
			}

			public class SOLIDCONDUITOUTBOX
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX");

				public static LocString DESC = "When materials reach the end of a rail they enter a receptacle to be used by Duplicants.";

				public static LocString EFFECT = "Unloads " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " from a " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " into storage.";
			}

			public class SOLIDTRANSFERARM
			{
				public static LocString NAME = UI.FormatAsLink("Auto-Sweeper", "SOLIDTRANSFERARM");

				public static LocString DESC = "An auto-sweeper's range can be viewed at any time by clicking on the building.";

				public static LocString EFFECT = "Automates " + UI.FormatAsLink("Sweeping", "CHORES") + " and " + UI.FormatAsLink("Supplying", "CHORES") + " errands by sucking up all nearby " + UI.FormatAsLink("Debris", "DECOR") + ".\n\nMaterials are automatically delivered to any " + UI.FormatAsLink("Conveyor Loader", "SOLIDCONDUITINBOX") + ", " + UI.FormatAsLink("Conveyor Receptacle", "SOLIDCONDUITOUTBOX") + ", storage, or buildings within range.";
			}

			public class SOLIDCONDUITBRIDGE
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Bridge", "SOLIDCONDUITBRIDGE");

				public static LocString DESC = "Separating rail systems helps ensure materials go to the intended destinations.";

				public static LocString EFFECT = "Runs one " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " section over another without joining them.\n\nCan be run through wall and floor tile.";
			}

			public class SOLIDVENT
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Chute", "SOLIDVENT");

				public static LocString DESC = "When materials reach the end of a rail they are dropped back into the world.";

				public static LocString EFFECT = "Unloads " + UI.FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " from a " + UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " onto the floor.";
			}

			public class SOLIDLOGICVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Shutoff", "SOLIDLOGICVALVE");

				public static LocString DESC = "Automated conveyors save power and time by removing the need for Duplicant input.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn " + UI.FormatAsLink("Solid Material", "ELEMENTS_SOLID") + " transport on or off.";

				public static LocString LOGIC_PORT = "Open/Close";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow material transport";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Prevent material transport";
			}

			public class SOLIDLIMITVALVE
			{
				public static LocString NAME = UI.FormatAsLink("Conveyor Meter", "SOLIDLIMITVALVE");

				public static LocString DESC = "Conveyor Meters let an exact amount of materials pass through before shutting off.";

				public static LocString EFFECT = "Connects to an " + UI.FormatAsLink("Automation", "LOGIC") + " grid to automatically turn material transfer off when the specified amount has passed through it.";

				public static LocString LOGIC_PORT_OUTPUT = "Limit Reached";

				public static LocString OUTPUT_PORT_ACTIVE = "Sends a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + " if limit has been reached";

				public static LocString OUTPUT_PORT_INACTIVE = "Otherwise, sends a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString LOGIC_PORT_RESET = "Reset Meter";

				public static LocString RESET_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Reset the amount";

				public static LocString RESET_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Nothing";
			}

			public class AUTOMINER
			{
				public static LocString NAME = UI.FormatAsLink("Robo-Miner", "AUTOMINER");

				public static LocString DESC = "A robo-miner's range can be viewed at any time by selecting the building.";

				public static LocString EFFECT = "Automatically digs out all materials in a set range.";
			}

			public class CREATUREFEEDER
			{
				public static LocString NAME = UI.FormatAsLink("Critter Feeder", "CREATUREFEEDER");

				public static LocString DESC = "Critters tend to stay close to their food source and wander less when given a feeder.";

				public static LocString EFFECT = "Automatically dispenses food for hungry " + UI.FormatAsLink("Critters", "CRITTERS") + ".";
			}

			public class GRAVITASPEDESTAL
			{
				public static LocString NAME = UI.FormatAsLink("Pedestal", "ITEMPEDESTAL");

				public static LocString DESC = "Perception can be drastically changed by a bit of thoughtful presentation.";

				public static LocString EFFECT = "Displays a single object, doubling its " + UI.FormatAsLink("Decor", "DECOR") + " value.\n\nObjects with negative Decor will gain some positive Decor when displayed.";

				public static LocString DISPLAYED_ITEM_FMT = "Displayed {0}";
			}

			public class ITEMPEDESTAL
			{
				public static LocString NAME = UI.FormatAsLink("Pedestal", "ITEMPEDESTAL");

				public static LocString DESC = "Perception can be drastically changed by a bit of thoughtful presentation.";

				public static LocString EFFECT = "Displays a single object, doubling its " + UI.FormatAsLink("Decor", "DECOR") + " value.\n\nObjects with negative Decor will gain some positive Decor when displayed.";

				public static LocString DISPLAYED_ITEM_FMT = "Displayed {0}";
			}

			public class CROWNMOULDING
			{
				public static LocString NAME = UI.FormatAsLink("Crown Moulding", "CROWNMOULDING");

				public static LocString DESC = "Crown moulding is used as purely decorative trim for ceilings.";

				public static LocString EFFECT = "Used to decorate the ceilings of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class CORNERMOULDING
			{
				public static LocString NAME = UI.FormatAsLink("Corner Moulding", "CORNERMOULDING");

				public static LocString DESC = "Corner moulding is used as purely decorative trim for ceiling corners.";

				public static LocString EFFECT = "Used to decorate the ceiling corners of rooms.\n\nIncreases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class EGGINCUBATOR
			{
				public static LocString NAME = UI.FormatAsLink("Incubator", "EGGINCUBATOR");

				public static LocString DESC = "Incubators can maintain the ideal internal conditions for several species of critter egg.";

				public static LocString EFFECT = "Incubates " + UI.FormatAsLink("Critter", "CRITTERS") + " eggs until ready to hatch.\n\nAssigned Duplicants must possess the " + UI.FormatAsLink("Critter Wrangling", "RANCHER") + " trait.";
			}

			public class EGGCRACKER
			{
				public static LocString NAME = UI.FormatAsLink("Egg Cracker", "EGGCRACKER");

				public static LocString DESC = "Raw eggs are an ingredient in certain high quality food recipes.";

				public static LocString EFFECT = "Converts viable " + UI.FormatAsLink("Critter", "CRITTERS") + " eggs into cooking ingredients.\n\nCracked Eggs cannot hatch.\n\nDuplicants will not crack eggs unless tasks are queued.";

				public static LocString RECIPE_DESCRIPTION = "Turns {0} into {1}.";

				public static LocString RESULT_DESCRIPTION = "Cracked {0}";
			}

			public class URANIUMCENTRIFUGE
			{
				public static LocString NAME = UI.FormatAsLink("Uranium Centrifuge", "URANIUMCENTRIFUGE");

				public static LocString DESC = "Enriched uranium is a specialized substance that can be used to fuel powerful research reactors.";

				public static LocString EFFECT = "Extracts " + UI.FormatAsLink("Enriched Uranium", "ENRICHEDURANIUM") + " from " + UI.FormatAsLink("Uranium Ore", "URANIUMORE") + ".\n\nOutputs " + UI.FormatAsLink("Depleted Uranium", "DEPLETEDURANIUM") + " in molten form.";

				public static LocString RECIPE_DESCRIPTION = "Convert Uranium ore to Molten Uranium and Enriched Uranium";
			}

			public class HIGHENERGYPARTICLEREDIRECTOR
			{
				public static LocString NAME = UI.FormatAsLink("Radbolt Reflector", "HIGHENERGYPARTICLEREDIRECTOR");

				public static LocString DESC = "We were all out of mirrors.";

				public static LocString EFFECT = "Receives and redirects Radbolts from " + UI.FormatAsLink("Radbolt Generators", "HIGHENERGYPARTICLESPAWNER") + ".";

				public static LocString LOGIC_PORT = "Ignore incoming Radbolts";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Allow incoming Radbolts";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Ignore incoming Radbolts";
			}

			public class HIGHENERGYPARTICLESPAWNER
			{
				public static LocString NAME = UI.FormatAsLink("Radbolt Generator", "HIGHENERGYPARTICLESPAWNER");

				public static LocString DESC = "Radbolts are necessary for producing Materials Science research.";

				public static LocString EFFECT = "Attracts nearby " + UI.FormatAsLink("Radiation", "RADIATION") + " to generate Radbolts.\n\nEmits generated Radbolts in the direction of your choosing.";

				public static LocString LOGIC_PORT = "Do not emit Radbolts";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Emit Radbolts";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Do not emit Radbolts";
			}

			public class ASTRONAUTTRAININGCENTER
			{
				public static LocString NAME = UI.FormatAsLink("Space Cadet Centrifuge", "ASTRONAUTTRAININGCENTER");

				public static LocString DESC = "Duplicants must complete astronaut training in order to pilot space rockets.";

				public static LocString EFFECT = "Trains Duplicants to become " + UI.FormatAsLink("Astronauts", "ASTRONAUT") + ".\n\nDuplicants must possess the " + UI.FormatAsLink("Astronaut-in-Training", "ASTRONAUTTRAINEE") + " trait to receive training.";
			}

			public class HOTTUB
			{
				public static LocString NAME = UI.FormatAsLink("Hot Tub", "HOTTUB");

				public static LocString DESC = "Relaxes Duplicants with massaging jets of heated liquid.";

				public static LocString EFFECT = "Requires " + UI.FormatAsLink("Pipes", "LIQUIDPIPING") + " to and from tub and " + UI.FormatAsLink("Power", "POWER") + " to run the jets.\n\nWater must be a comfortable temperature and will cool rapidly.\n\nIncreases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";

				public static LocString WATER_REQUIREMENT = "{element}: {amount}";

				public static LocString WATER_REQUIREMENT_TOOLTIP = "This building must be filled with {amount} {element} in order to function.";

				public static LocString TEMPERATURE_REQUIREMENT = "Minimum {element} Temperature: {temperature}";

				public static LocString TEMPERATURE_REQUIREMENT_TOOLTIP = "The Hot Tub will only be usable if supplied with {temperature} {element}. If the {element} gets too cold, the Hot Tub will drain and require refilling with {element}.";
			}

			public class SODAFOUNTAIN
			{
				public static LocString NAME = UI.FormatAsLink("Soda Fountain", "SODAFOUNTAIN");

				public static LocString DESC = "Sparkling water puts a twinkle in a Duplicant's eye.";

				public static LocString EFFECT = "Creates soda from " + UI.FormatAsLink("Water", "WATER") + " and " + UI.FormatAsLink("Carbon Dioxide", "CARBONDIOXIDE") + ".\n\nConsuming soda water increases Duplicant " + UI.FormatAsLink("Morale", "MORALE") + ".";
			}

			public class UNCONSTRUCTEDROCKETMODULE
			{
				public static LocString NAME = "Empty Rocket Module";

				public static LocString DESC = "Something useful could be put here someday";

				public static LocString EFFECT = "Can be changed into a different rocket module";
			}

			public class MODULARLAUNCHPADPORT
			{
				public static LocString NAME = UI.FormatAsLink("Rocket Port", "MODULARLAUNCHPADPORTSOLID");

				public static LocString NAME_PLURAL = UI.FormatAsLink("Rocket Ports", "MODULARLAUNCHPADPORTSOLID");
			}

			public class MODULARLAUNCHPADPORTGAS
			{
				public static LocString NAME = UI.FormatAsLink("Gas Rocket Port Loader", "MODULARLAUNCHPADPORTGAS");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Loads ", UI.FormatAsLink("Gases", "ELEMENTS_GAS"), " to the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the gas filters set on the rocket's cargo bays.");
			}

			public class MODULARLAUNCHPADPORTLIQUID
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Rocket Port Loader", "MODULARLAUNCHPADPORTLIQUID");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Loads ", UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"), " to the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the liquid filters set on the rocket's cargo bays.");
			}

			public class MODULARLAUNCHPADPORTSOLID
			{
				public static LocString NAME = UI.FormatAsLink("Solid Rocket Port Loader", "MODULARLAUNCHPADPORTSOLID");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Loads ", UI.FormatAsLink("Solids", "ELEMENTS_SOLID"), " to the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the solid material filters set on the rocket's cargo bays.");
			}

			public class MODULARLAUNCHPADPORTGASUNLOADER
			{
				public static LocString NAME = UI.FormatAsLink("Gas Rocket Port Unloader", "MODULARLAUNCHPADPORTGASUNLOADER");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Unloads ", UI.FormatAsLink("Gases", "ELEMENTS_GAS"), " from the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the gas filters set on this unloader.");
			}

			public class MODULARLAUNCHPADPORTLIQUIDUNLOADER
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Rocket Port Unloader", "MODULARLAUNCHPADPORTLIQUIDUNLOADER");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Unloads ", UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID"), " from the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the liquid filters set on this unloader.");
			}

			public class MODULARLAUNCHPADPORTSOLIDUNLOADER
			{
				public static LocString NAME = UI.FormatAsLink("Solid Rocket Port Unloader", "MODULARLAUNCHPADPORTSOLIDUNLOADER");

				public static LocString DESC = "Rockets must be landed to load or unload resources.";

				public static LocString EFFECT = string.Concat("Unloads ", UI.FormatAsLink("Solids", "ELEMENTS_SOLID"), " from the storage of a linked rocket.\n\nAutomatically links when built to the side of a ", LAUNCHPAD.NAME, " or another ", MODULARLAUNCHPADPORT.NAME, ".\n\nUses the solid material filters set on this unloader.");
			}

			public class STICKERBOMB
			{
				public static LocString NAME = UI.FormatAsLink("Sticker Bomb", "STICKERBOMB");

				public static LocString DESC = "Surprise decor sneak attacks a Duplicant's gloomy day.";
			}

			public class HEATCOMPRESSOR
			{
				public static LocString NAME = UI.FormatAsLink("Liquid Heatquilizer", "HEATCOMPRESSOR");

				public static LocString DESC = "\"Room temperature\" is relative, really.";

				public static LocString EFFECT = "Heats or cools " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " to match ambient " + UI.FormatAsLink("Air Temperature", "HEAT") + ".";
			}

			public class PARTYCAKE
			{
				public static LocString NAME = UI.FormatAsLink("Triple Decker Cake", "PARTYCAKE");

				public static LocString DESC = "Any way you slice it, that's a good looking cake.";

				public static LocString EFFECT = "Increases " + UI.FormatAsLink("Decor", "DECOR") + ", contributing to " + UI.FormatAsLink("Morale", "MORALE") + ".\n\nAdds a " + UI.FormatAsLink("Morale", "MORALE") + " bonus to Duplicants' parties.";
			}

			public class RAILGUN
			{
				public static LocString NAME = UI.FormatAsLink("Interplanetary Launcher", "RAILGUN");

				public static LocString DESC = "It's tempting to climb inside but trust me... don't.";

				public static LocString EFFECT = "Launches " + UI.FormatAsLink("Interplanetary Payloads", "RAILGUNPAYLOAD") + " between Planetoids.\n\nPayloads can contain " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + ", " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", or " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " materials.\n\nCannot transport Duplicants.";

				public static LocString SIDESCREEN_HEP_REQUIRED = "Launch cost: {current} / {required} radbolts";

				public static LocString LOGIC_PORT = "Launch Toggle";

				public static LocString LOGIC_PORT_ACTIVE = UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ": Enable payload launching.";

				public static LocString LOGIC_PORT_INACTIVE = UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ": Disable payload launching.";
			}

			public class RAILGUNPAYLOADOPENER
			{
				public static LocString NAME = UI.FormatAsLink("Payload Opener", "RAILGUNPAYLOADOPENER");

				public static LocString DESC = "Payload openers can be hooked up to conveyors, plumbing and ventilation for improved sorting.";

				public static LocString EFFECT = "Unpacks " + UI.FormatAsLink("Interplanetary Payloads", "RAILGUNPAYLOAD") + " delivered by Duplicants.\n\nAutomatically separates " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + ", " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " materials and distributes them to the appropriate systems.";
			}

			public class LANDINGBEACON
			{
				public static LocString NAME = UI.FormatAsLink("Targeting Beacon", "LANDINGBEACON");

				public static LocString DESC = "Microtarget where your " + UI.FormatAsLink("Interplanetary Payload", "RAILGUNPAYLOAD") + " lands on a Planetoid surface.";

				public static LocString EFFECT = "Guides " + UI.FormatAsLink("Interplanetary Payloads", "RAILGUNPAYLOAD") + " and " + UI.FormatAsLink("Orbital Cargo Modules", "ORBITALCARGOMODULE") + " to land nearby.\n\n" + UI.FormatAsLink("Interplanetary Payloads", "RAILGUNPAYLOAD") + " must be launched from a " + UI.FormatAsLink("Interplanetary Launcher", "RAILGUN") + ".";
			}

			public class DIAMONDPRESS
			{
				public static LocString NAME = UI.FormatAsLink("Diamond Press", "DIAMONDPRESS");

				public static LocString DESC = "Crushes refined carbon into diamond.";

				public static LocString EFFECT = "Uses " + UI.FormatAsLink("Power", "POWER") + " and " + UI.FormatAsLink("Radbolts", "HEP") + " to crush " + UI.FormatAsLink("Refined Carbon", "REFINEDCARBON") + " into " + UI.FormatAsLink("Diamond", "DIAMOND") + ".\n\nDuplicants will not fabricate items unless recipes are queued and " + UI.FormatAsLink("Refined Carbon", "REFINEDCARBON") + " has been discovered.";

				public static LocString REFINED_CARBON_RECIPE_DESCRIPTION = "Converts {1} to {0}";
			}

			public class MASSIVEHEATSINK
			{
				public static LocString NAME = UI.FormatAsLink("Anti Entropy Thermo-Nullifier", "MASSIVEHEATSINK");

				public static LocString DESC = "";

				public static LocString EFFECT = "A self-sustaining machine powered by what appears to be refined " + UI.FormatAsLink("Neutronium", "UNOBTANIUM") + ".\n\nAbsorbs and neutralizes " + UI.FormatAsLink("Heat", "HEAT") + " energy when provided with piped " + UI.FormatAsLink("Hydrogen", "HYDROGEN") + ".";
			}

			public class FACILITYBACKWALLWINDOW
			{
				public static LocString NAME = UI.FormatAsLink("Window", "FACILITYBACKWALLWINDOW");

				public static LocString DESC = "";

				public static LocString EFFECT = "A tall, thin window.";
			}

			public class POIBUNKEREXTERIORDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIBUNKEREXTERIORDOOR");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = "";
			}

			public class POIDOORINTERNAL
			{
				public static LocString NAME = UI.FormatAsLink("Security Door", "POIDOORINTERNAL");

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";

				public static LocString DESC = "";
			}

			public class POIFACILITYDOOR
			{
				public static LocString NAME = UI.FormatAsLink("Lobby Doors", "FACILITYDOOR");

				public static LocString EFFECT = "Large double doors that were once the main entrance to a large facility.";

				public static LocString DESC = "";
			}

			public class VENDINGMACHINE
			{
				public static LocString NAME = "Vending Machine";

				public static LocString DESC = "A pristine " + UI.FormatAsLink("Field Ration", "FIELDRATION") + " dispenser.";
			}

			public class GENESHUFFLER
			{
				public static LocString NAME = "Neural Vacillator";

				public static LocString DESC = "A massive synthetic brain, suspended in saline solution.\n\nThere is a chair attached to the device with room for one person.";
			}

			public class PROPTALLPLANT
			{
				public static LocString NAME = "Potted Plant";

				public static LocString DESC = "Looking closely, it appears to be fake.";
			}

			public class PROPTABLE
			{
				public static LocString NAME = "Table";

				public static LocString DESC = "A table and some chairs.";
			}

			public class PROPDESK
			{
				public static LocString NAME = "Computer Desk";

				public static LocString DESC = "An intact office desk, decorated with several personal belongings and a barely functioning computer.";
			}

			public class PROPFACILITYCHAIR
			{
				public static LocString NAME = "Lobby Chair";

				public static LocString DESC = "A chair where visitors can comfortably wait before their appointments.";
			}

			public class PROPFACILITYCOUCH
			{
				public static LocString NAME = "Lobby Couch";

				public static LocString DESC = "A couch where visitors can comfortably wait before their appointments.";
			}

			public class PROPFACILITYDESK
			{
				public static LocString NAME = "Director's Desk";

				public static LocString DESC = "A spotless desk filled with impeccably organized office supplies.\n\nA photo peeks out from beneath the desk pad, depicting two beaming young women in caps and gowns.\n\nThe photo is quite old.";
			}

			public class PROPFACILITYTABLE
			{
				public static LocString NAME = "Coffee Table";

				public static LocString DESC = "A low coffee table that may have once held old science magazines.";
			}

			public class PROPFACILITYSTATUE
			{
				public static LocString NAME = "Gravitas Monument";

				public static LocString DESC = "A large, modern sculpture that sits in the center of the lobby.\n\nIt's an artistic cross between an hourglass shape and a double helix.";
			}

			public class PROPFACILITYCHANDELIER
			{
				public static LocString NAME = "Chandelier";

				public static LocString DESC = "A large chandelier that hangs from the ceiling.\n\nIt does not appear to function.";
			}

			public class PROPFACILITYGLOBEDROORS
			{
				public static LocString NAME = "Filing Cabinet";

				public static LocString DESC = "A filing cabinet for storing hard copy employee records.\n\nThe contents have been shredded.";
			}

			public class PROPFACILITYDISPLAY1
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a familiar device.\n\nIt looks like a Printing Pod.";
			}

			public class PROPFACILITYDISPLAY2
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a familiar device.\n\nIt looks like a Mining Gun.";
			}

			public class PROPFACILITYDISPLAY3
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a strange device.\n\nPerhaps these displays were used to entice visitors.";
			}

			public class PROPFACILITYTALLPLANT
			{
				public static LocString NAME = "Office Plant";

				public static LocString DESC = "It's survived the vacuum of space by virtue of being plastic.";
			}

			public class PROPFACILITYLAMP
			{
				public static LocString NAME = "Light Fixture";

				public static LocString DESC = "A long light fixture that hangs from the ceiling.\n\nIt does not appear to function.";
			}

			public class PROPFACILITYWALLDEGREE
			{
				public static LocString NAME = "Doctorate Degree";

				public static LocString DESC = "Certification in Applied Physics, awarded in recognition of one \"Jacquelyn A. Stern\".";
			}

			public class PROPFACILITYPAINTING
			{
				public static LocString NAME = "Landscape Portrait";

				public static LocString DESC = "A painting featuring a copse of fir trees and a magnificent mountain range on the horizon.\n\nThe air in the room prickles with the sensation that I'm not meant to be here.";
			}

			public class PROPRECEPTIONDESK
			{
				public static LocString NAME = "Reception Desk";

				public static LocString DESC = "A full coffee cup and a note abandoned mid sentence sit behind the desk.\n\nIt gives me an eerie feeling, as if the receptionist has stepped out and will return any moment.";
			}

			public class PROPELEVATOR
			{
				public static LocString NAME = "Broken Elevator";

				public static LocString DESC = "Out of service.\n\nThe buttons inside indicate it went down more than a dozen floors at one point in time.";
			}

			public class SETLOCKER
			{
				public static LocString NAME = "Locker";

				public static LocString DESC = "A basic metal locker.\n\nIt contains an assortment of personal effects.";
			}

			public class PROPLIGHT
			{
				public static LocString NAME = "Light Fixture";

				public static LocString DESC = "An elegant ceiling lamp, slightly worse for wear.";
			}

			public class PROPLADDER
			{
				public static LocString NAME = "Ladder";

				public static LocString DESC = "A hard plastic ladder.";
			}

			public class PROPSKELETON
			{
				public static LocString NAME = "Model Skeleton";

				public static LocString DESC = "A detailed anatomical model.\n\nIt appears to be made of resin.";
			}

			public class PROPSURFACESATELLITE1
			{
				public static LocString NAME = "Crashed Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPSURFACESATELLITE2
			{
				public static LocString NAME = "Wrecked Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPSURFACESATELLITE3
			{
				public static LocString NAME = "Crushed Satellite";

				public static LocString DESC = "All that remains of a once peacefully orbiting satellite.";
			}

			public class PROPCLOCK
			{
				public static LocString NAME = "Clock";

				public static LocString DESC = "A simple wall clock.\n\nIt is no longer ticking.";
			}

			public class PROPGRAVITASDECORATIVEWINDOW
			{
				public static LocString NAME = "Window";

				public static LocString DESC = "A tall, thin window which once pointed to a courtyard.";
			}

			public class PROPGRAVITASLABWINDOW
			{
				public static LocString NAME = "Lab Window";

				public static LocString DESC = "A lab window. Formerly a portal to the outside world.";

				public static LocString EFFECT = "";
			}

			public class PROPGRAVITASLABWINDOWHORIZONTAL
			{
				public static LocString NAME = "Lab Window";

				public static LocString DESC = "A lab window.\n\nSomeone once stared out this contemplating the results of an experiment.";

				public static LocString EFFECT = "";
			}

			public class PROPGRAVITASLABWALL
			{
				public static LocString NAME = "Lab Wall";

				public static LocString DESC = "A regular wall that once existed in a working lab.";

				public static LocString EFFECT = "";
			}

			public class GRAVITASDOOR
			{
				public static LocString NAME = "Gravitas Door";

				public static LocString DESC = "An office door to an office that no longer exists.";

				public static LocString EFFECT = "";
			}

			public class PROPGRAVITASWALL
			{
				public static LocString NAME = "Wall";

				public static LocString DESC = "The granite wall of a once great scientific facility.";

				public static LocString EFFECT = "";
			}

			public class PROPGRAVITASDISPLAY4
			{
				public static LocString NAME = "Electronic Display";

				public static LocString DESC = "An electronic display projecting the blueprint of a robotic device.\n\nIt looks like a ceiling robot.";
			}

			public class PROPGRAVITASCEILINGROBOT
			{
				public static LocString NAME = "Ceiling Robot";

				public static LocString DESC = "Non-functioning robotic arms that once assisted lab technicians.";
			}

			public class PROPGRAVITASFLOORROBOT
			{
				public static LocString NAME = "Robotic Arm";

				public static LocString DESC = "The grasping robotic claw designed to assist technicians in a lab.";
			}

			public class PROPGRAVITASJAR1
			{
				public static LocString NAME = "Big Brain Jar";

				public static LocString DESC = "An abnormally large brain floating in embalming liquid to prevent decomposition.";
			}

			public class PROPGRAVITASHANDSCANNER
			{
				public static LocString NAME = "Hand Scanner";

				public static LocString DESC = "A sophisticated security device.\n\nIt appears to use a method other than fingerprints to verify an individual's identity.";
			}

			public class PROPGRAVITASLABTABLE
			{
				public static LocString NAME = "Lab Desk";

				public static LocString DESC = "The quaint research desk of a departed lab technician.\n\nPerhaps the computer stores something of interest.";
			}

			public class PROPGRAVITASROBTICTABLE
			{
				public static LocString NAME = "Robotics Research Desk";

				public static LocString DESC = "The work space of an extinct robotics technician who left behind some unfinished prototypes.";
			}

			public class PROPGRAVITASSHELF
			{
				public static LocString NAME = "Shelf";

				public static LocString DESC = "A shelf holding jars just out of reach for a short person.";
			}

			public class PROPGRAVITASJAR2
			{
				public static LocString NAME = "Sample Jar";

				public static LocString DESC = "The corpse of a proto-hatch creature meticulously preserved in a jar.";
			}

			public class WARPCONDUITRECEIVER
			{
				public static LocString NAME = "Supply Teleporter Output";

				public static LocString DESC = "The tubes at the back disappear into nowhere.";

				public static LocString EFFECT = "A machine capable of teleporting " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + ", and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources to another asteroid.\n\nIt can be activated by a Duplicant with the " + UI.FormatAsLink("Field Research", "RESEARCHER") + " skill.\n\nThis is the receiving side.";
			}

			public class WARPCONDUITSENDER
			{
				public static LocString NAME = "Supply Teleporter Input";

				public static LocString DESC = "The tubes at the back disappear into nowhere.";

				public static LocString EFFECT = "A machine capable of teleporting " + UI.FormatAsLink("Liquid", "ELEMENTS_LIQUID") + ", " + UI.FormatAsLink("Solid", "ELEMENTS_SOLID") + ", and " + UI.FormatAsLink("Gas", "ELEMENTS_GAS") + " resources to another asteroid.\n\nIt can be activated by a Duplicant with the " + UI.FormatAsLink("Field Research", "RESEARCHER") + " skill.\n\nThis is the transmitting side.";
			}

			public class WARPPORTAL
			{
				public static LocString NAME = "Teleporter Transmitter";

				public static LocString DESC = "The functional remnants of an intricate teleportation system.\n\nThis is the outgoing side, and has one pre-programmed destination.";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";
			}

			public class WARPRECEIVER
			{
				public static LocString NAME = "Teleporter Receiver";

				public static LocString DESC = "The functional remnants of an intricate teleportation system.\n\nThis is the incoming side.";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";
			}

			public class TEMPORALTEARANALYZER
			{
				public static LocString NAME = "Temporal Tear Scanner";

				public static LocString DESC = "Hard to say what this is for.\n\nWas it looking for something?";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";
			}

			public class TEMPORALTEAROPENER
			{
				public class SIDESCREEN
				{
					public static LocString TEXT = "Fire!";

					public static LocString TOOLTIP = "The big red button.";
				}

				public static LocString NAME = "Temporal Tear Opener";

				public static LocString DESC = "R.I.P.\n\nLooks dangerous.";

				public static LocString EFFECT = "A strong door with a sophisticated genetic lock.";
			}
		}

		public static class DAMAGESOURCES
		{
			public static LocString NOTIFICATION_TOOLTIP = "A {0} sustained damage from {1}";

			public static LocString CONDUIT_CONTENTS_FROZE = "pipe contents becoming too cold";

			public static LocString CONDUIT_CONTENTS_BOILED = "pipe contents becoming too hot";

			public static LocString BUILDING_OVERHEATED = "overheating";

			public static LocString CORROSIVE_ELEMENT = "corrosive element";

			public static LocString BAD_INPUT_ELEMENT = "receiving an incorrect substance";

			public static LocString MINION_DESTRUCTION = "an angry Duplicant. Rude!";

			public static LocString LIQUID_PRESSURE = "neighboring liquid pressure";

			public static LocString CIRCUIT_OVERLOADED = "an overloaded circuit";

			public static LocString LOGIC_CIRCUIT_OVERLOADED = "an overloaded logic circuit";

			public static LocString MICROMETEORITE = "micrometeorite";

			public static LocString COMET = "falling space rocks";

			public static LocString ROCKET = "rocket engine";
		}

		public static class AUTODISINFECTABLE
		{
			public static class ENABLE_AUTODISINFECT
			{
				public static LocString NAME = "Enable Disinfect";

				public static LocString TOOLTIP = "Automatically disinfect this building when it becomes contaminated";
			}

			public static class DISABLE_AUTODISINFECT
			{
				public static LocString NAME = "Disable Disinfect";

				public static LocString TOOLTIP = "Do not automatically disinfect this building";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is clean";
			}
		}

		public static class DISINFECTABLE
		{
			public static class ENABLE_DISINFECT
			{
				public static LocString NAME = "Disinfect";

				public static LocString TOOLTIP = "Mark this building for disinfection";
			}

			public static class DISABLE_DISINFECT
			{
				public static LocString NAME = "Cancel Disinfect";

				public static LocString TOOLTIP = "Cancel this disinfect order";
			}

			public static class NO_DISEASE
			{
				public static LocString TOOLTIP = "This building is already clean";
			}
		}

		public static class REPAIRABLE
		{
			public static class ENABLE_AUTOREPAIR
			{
				public static LocString NAME = "Enable Autorepair";

				public static LocString TOOLTIP = "Automatically repair this building when damaged";
			}

			public static class DISABLE_AUTOREPAIR
			{
				public static LocString NAME = "Disable Autorepair";

				public static LocString TOOLTIP = "Only repair this building when ordered";
			}
		}

		public static class AUTOMATABLE
		{
			public static class ENABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Disable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by Auto-Sweepers only" + UI.HORIZONTAL_BR_RULE + "Duplicants will not be permitted to add or remove materials from this building";
			}

			public static class DISABLE_AUTOMATIONONLY
			{
				public static LocString NAME = "Enable Manual";

				public static LocString TOOLTIP = "This building's storage may be accessed by both Duplicants and Auto-Sweeper buildings";
			}
		}
	}
}
