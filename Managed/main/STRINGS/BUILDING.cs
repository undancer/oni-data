namespace STRINGS
{
	public class BUILDING
	{
		public class STATUSITEMS
		{
			public class NOTLINKEDTOHEAD
			{
				public static LocString NAME = "Not Linked";

				public static LocString TOOLTIP = "This building must be built adjacent to a {headBuilding} or another {linkBuilding} in order to function";
			}

			public class BAITED
			{
				public static LocString NAME = "{0} Bait";

				public static LocString TOOLTIP = "This lure is baited with {0}\n\nBait material is set during the construction of the building";
			}

			public class NOCOOLANT
			{
				public static LocString NAME = "No Coolant";

				public static LocString TOOLTIP = "This building needs coolant";
			}

			public class ANGERDAMAGE
			{
				public static LocString NAME = "Damage: Duplicant Tantrum";

				public static LocString TOOLTIP = "A stressed Duplicant is damaging this building";

				public static LocString NOTIFICATION = "Building Damage: Duplicant Tantrum";

				public static LocString NOTIFICATION_TOOLTIP = "Stressed Duplicants are damaging these buildings:\n\n{0}";
			}

			public class PIPECONTENTS
			{
				public static LocString EMPTY = "Empty";

				public static LocString CONTENTS = "{0} of {1} at {2}";

				public static LocString CONTENTS_WITH_DISEASE = "\n  {0}";
			}

			public class CONVEYOR_CONTENTS
			{
				public static LocString EMPTY = "Empty";

				public static LocString CONTENTS = "{0} of {1} at {2}";

				public static LocString CONTENTS_WITH_DISEASE = "\n  {0}";
			}

			public class ASSIGNEDTO
			{
				public static LocString NAME = "Assigned to: {Assignee}";

				public static LocString TOOLTIP = "Only {Assignee} can use this amenity";
			}

			public class ASSIGNEDPUBLIC
			{
				public static LocString NAME = "Assigned to: Public";

				public static LocString TOOLTIP = "Any Duplicant can use this amenity";
			}

			public class ASSIGNEDTOROOM
			{
				public static LocString NAME = "Assigned to: {0}";

				public static LocString TOOLTIP = "Any Duplicant assigned to this " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " can use this amenity";
			}

			public class AWAITINGSEEDDELIVERY
			{
				public static LocString NAME = "Awaiting Delivery";

				public static LocString TOOLTIP = "Awaiting delivery of selected " + UI.PRE_KEYWORD + "Seed" + UI.PST_KEYWORD;
			}

			public class AWAITINGBAITDELIVERY
			{
				public static LocString NAME = "Awaiting Bait";

				public static LocString TOOLTIP = "Awaiting delivery of selected " + UI.PRE_KEYWORD + "Bait" + UI.PST_KEYWORD;
			}

			public class CLINICOUTSIDEHOSPITAL
			{
				public static LocString NAME = "Medical building outside Hospital";

				public static LocString TOOLTIP = "Rebuild this medical equipment in a " + UI.PRE_KEYWORD + "Hospital" + UI.PST_KEYWORD + " to more effectively quarantine sick Duplicants";
			}

			public class BOTTLE_EMPTIER
			{
				public static class ALLOWED
				{
					public static LocString NAME = "Auto-Bottle: On";

					public static LocString TOOLTIP = string.Concat("Duplicants may specifically fetch ", UI.PRE_KEYWORD, "Liquid", UI.PST_KEYWORD, " from a ", BUILDINGS.PREFABS.LIQUIDPUMPINGSTATION.NAME, " to bring to this location");
				}

				public static class DENIED
				{
					public static LocString NAME = "Auto-Bottle: Off";

					public static LocString TOOLTIP = string.Concat("Duplicants may not specifically fetch ", UI.PRE_KEYWORD, "Liquid", UI.PST_KEYWORD, " from a ", BUILDINGS.PREFABS.LIQUIDPUMPINGSTATION.NAME, " to bring to this location");
				}
			}

			public class CANISTER_EMPTIER
			{
				public static class ALLOWED
				{
					public static LocString NAME = "Auto-Bottle: On";

					public static LocString TOOLTIP = string.Concat("Duplicants may specifically fetch ", UI.PRE_KEYWORD, "Gas", UI.PST_KEYWORD, " from a ", BUILDINGS.PREFABS.GASBOTTLER.NAME, " to bring to this location");
				}

				public static class DENIED
				{
					public static LocString NAME = "Auto-Bottle: Off";

					public static LocString TOOLTIP = string.Concat("Duplicants may not specifically fetch ", UI.PRE_KEYWORD, "Gas", UI.PST_KEYWORD, " from a ", BUILDINGS.PREFABS.GASBOTTLER.NAME, " to bring to this location");
				}
			}

			public class BROKEN
			{
				public static LocString NAME = "Broken";

				public static LocString TOOLTIP = "This building received damage from <b>{DamageInfo}</b>\n\nIt will not function until it receives repairs";
			}

			public class CHANGEDOORCONTROLSTATE
			{
				public static LocString NAME = "Pending Door State Change: {ControlState}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to change control state";
			}

			public class SUIT_LOCKER
			{
				public class NEED_CONFIGURATION
				{
					public static LocString NAME = "Current Status: Needs Configuration";

					public static LocString TOOLTIP = "Set this dock to store a suit or leave it empty";
				}

				public class READY
				{
					public static LocString NAME = "Current Status: Empty";

					public static LocString TOOLTIP = "This dock is ready to receive a " + UI.PRE_KEYWORD + "Suit" + UI.PST_KEYWORD + ", either by manual delivery or from a Duplicant returning the suit they're wearing";
				}

				public class SUIT_REQUESTED
				{
					public static LocString NAME = "Current Status: Awaiting Delivery";

					public static LocString TOOLTIP = "Waiting for a Duplicant to deliver a " + UI.PRE_KEYWORD + "Suit" + UI.PST_KEYWORD;
				}

				public class CHARGING
				{
					public static LocString NAME = "Current Status: Charging Suit";

					public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Suit" + UI.PST_KEYWORD + " is docked and refueling";
				}

				public class NO_OXYGEN
				{
					public static LocString NAME = "Current Status: No Oxygen";

					public static LocString TOOLTIP = string.Concat("This dock does not contain enough ", ELEMENTS.OXYGEN.NAME, " to refill a ", UI.PRE_KEYWORD, "Suit", UI.PST_KEYWORD);
				}

				public class NO_FUEL
				{
					public static LocString NAME = "Current Status: No Fuel";

					public static LocString TOOLTIP = string.Concat("This dock does not contain enough ", ELEMENTS.PETROLEUM.NAME, " to refill a ", UI.PRE_KEYWORD, "Suit", UI.PST_KEYWORD);
				}

				public class NO_COOLANT
				{
					public static LocString NAME = "Current Status: No Coolant";

					public static LocString TOOLTIP = string.Concat("This dock does not contain enough ", ELEMENTS.WATER.NAME, " to refill a ", UI.PRE_KEYWORD, "Suit", UI.PST_KEYWORD);
				}

				public class NOT_OPERATIONAL
				{
					public static LocString NAME = "Current Status: Offline";

					public static LocString TOOLTIP = "This dock requires " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD;
				}

				public class FULLY_CHARGED
				{
					public static LocString NAME = "Current Status: Full Fueled";

					public static LocString TOOLTIP = "This suit is fully refueled and ready for use";
				}
			}

			public class SUITMARKERTRAVERSALONLYWHENROOMAVAILABLE
			{
				public static LocString NAME = "Clearance: Vacancy Only";

				public static LocString TOOLTIP = "Suited Duplicants may pass only if there is room in a " + UI.PRE_KEYWORD + "Dock" + UI.PST_KEYWORD + " to store their " + UI.PRE_KEYWORD + "Suit" + UI.PST_KEYWORD;
			}

			public class SUITMARKERTRAVERSALANYTIME
			{
				public static LocString NAME = "Clearance: Always Permitted";

				public static LocString TOOLTIP = "Suited Duplicants may pass even if there is no room to store their " + UI.PRE_KEYWORD + "Suits" + UI.PST_KEYWORD + UI.HORIZONTAL_BR_RULE + "When all available docks are full, Duplicants will unequip their " + UI.PRE_KEYWORD + "Suits" + UI.PST_KEYWORD + " and drop them on the floor";
			}

			public class SUIT_LOCKER_NEEDS_CONFIGURATION
			{
				public static LocString NAME = "Not Configured";

				public static LocString TOOLTIP = "Dock settings not configured";
			}

			public class CURRENTDOORCONTROLSTATE
			{
				public static LocString NAME = "Current State: {ControlState}";

				public static LocString TOOLTIP = "Current State: {ControlState}" + UI.HORIZONTAL_BR_RULE + "Auto: Duplicants open and close this door as needed\nLocked: Nothing may pass through\nOpen: This door will remain open";

				public static LocString OPENED = "Opened";

				public static LocString AUTO = "Auto";

				public static LocString LOCKED = "Locked";
			}

			public class CONDUITBLOCKED
			{
				public static LocString NAME = "Pipe Blocked";

				public static LocString TOOLTIP = "Output " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " is blocked";
			}

			public class CONDUITBLOCKEDMULTIPLES
			{
				public static LocString NAME = "Pipe Blocked";

				public static LocString TOOLTIP = "Output " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " is blocked";
			}

			public class SOLIDCONDUITBLOCKEDMULTIPLES
			{
				public static LocString NAME = "Conveyor Rail Blocked";

				public static LocString TOOLTIP = "Output " + UI.PRE_KEYWORD + "Conveyor Rail" + UI.PST_KEYWORD + " is blocked";
			}

			public class OUTPUTPIPEFULL
			{
				public static LocString NAME = "Output Pipe Full";

				public static LocString TOOLTIP = "Unable to flush contents, output " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " is blocked";
			}

			public class CONSTRUCTIONUNREACHABLE
			{
				public static LocString NAME = "Unreachable Build";

				public static LocString TOOLTIP = "Duplicants cannot reach this construction site";
			}

			public class MOPUNREACHABLE
			{
				public static LocString NAME = "Unreachable Mop";

				public static LocString TOOLTIP = "Duplicants cannot reach this area";
			}

			public class DEADREACTORCOOLINGOFF
			{
				public static LocString NAME = "Cooling ({CyclesRemaining} cycles remaining)";

				public static LocString TOOLTIP = "The radiation coming from this reactor is diminishing";
			}

			public class DIGUNREACHABLE
			{
				public static LocString NAME = "Unreachable Dig";

				public static LocString TOOLTIP = "Duplicants cannot reach this area";
			}

			public class STORAGEUNREACHABLE
			{
				public static LocString NAME = "Unreachable Storage";

				public static LocString TOOLTIP = "Duplicants cannot reach this storage unit";
			}

			public class PASSENGERMODULEUNREACHABLE
			{
				public static LocString NAME = "Unreachable Module";

				public static LocString TOOLTIP = "Duplicants cannot reach this rocket module";
			}

			public class CONSTRUCTABLEDIGUNREACHABLE
			{
				public static LocString NAME = "Unreachable Dig";

				public static LocString TOOLTIP = "This construction site contains cells that cannot be dug out";
			}

			public class EMPTYPUMPINGSTATION
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "This pumping station cannot access any " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD;
			}

			public class ENTOMBED
			{
				public static LocString NAME = "Entombed";

				public static LocString TOOLTIP = "Must be dug out by a Duplicant";

				public static LocString NOTIFICATION_NAME = "Building entombment";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are entombed and need to be dug out:";
			}

			public class INVALIDPORTOVERLAP
			{
				public static LocString NAME = "Invalid Port Overlap";

				public static LocString TOOLTIP = "Ports on this building overlap those on another building" + UI.HORIZONTAL_BR_RULE + "This building must be rebuilt in a valid location";

				public static LocString NOTIFICATION_NAME = "Building has overlapping ports";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings must be rebuilt with non-overlapping ports:";
			}

			public class GENESHUFFLECOMPLETED
			{
				public static LocString NAME = "Vacillation Complete";

				public static LocString TOOLTIP = "The Duplicant has completed the neural vacillation process and is ready to be released";
			}

			public class OVERHEATED
			{
				public static LocString NAME = "Damage: Overheating";

				public static LocString TOOLTIP = "This building is taking damage and will break down if not cooled";
			}

			public class OVERLOADED
			{
				public static LocString NAME = "Damage: Overloading";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Wire" + UI.PST_KEYWORD + " is taking damage because there are too many buildings pulling " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " from this circuit\n\nSplit this " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " circuit into multiple circuits, or use higher quality " + UI.PRE_KEYWORD + "Wires" + UI.PST_KEYWORD + " to prevent overloading";
			}

			public class LOGICOVERLOADED
			{
				public static LocString NAME = "Damage: Overloading";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Logic Wire" + UI.PST_KEYWORD + " is taking damage\n\nLimit the output to one Bit, or replace it with " + UI.PRE_KEYWORD + "Logic Ribbon" + UI.PST_KEYWORD + " to prevent further damage";
			}

			public class OPERATINGENERGY
			{
				public static LocString NAME = "Heat Production: {0}/s";

				public static LocString TOOLTIP = "This building is producing <b>{0}</b> per second\n\nSources:\n{1}";

				public static LocString LINEITEM = "    • {0}: {1}\n";

				public static LocString OPERATING = "Normal operation";

				public static LocString EXHAUSTING = "Excess produced";

				public static LocString PIPECONTENTS_TRANSFER = "Transferred from pipes";

				public static LocString FOOD_TRANSFER = "Internal Cooling";
			}

			public class FLOODED
			{
				public static LocString NAME = "Building Flooded";

				public static LocString TOOLTIP = "Building cannot function at current saturation";

				public static LocString NOTIFICATION_NAME = "Flooding";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are flooded:";
			}

			public class GASVENTOBSTRUCTED
			{
				public static LocString NAME = "Gas Vent Obstructed";

				public static LocString TOOLTIP = "A " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " has been obstructed and is preventing " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " flow to this vent";
			}

			public class GASVENTOVERPRESSURE
			{
				public static LocString NAME = "Gas Vent Overpressure";

				public static LocString TOOLTIP = "High " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " or " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " pressure in this area is preventing further " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " emission\nReduce pressure by pumping " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " away or clearing more space";
			}

			public class DIRECTION_CONTROL
			{
				public static class DIRECTIONS
				{
					public static LocString LEFT = "Left";

					public static LocString RIGHT = "Right";

					public static LocString BOTH = "Both";
				}

				public static LocString NAME = "Use Direction: {Direction}";

				public static LocString TOOLTIP = "Duplicants will only use this building when walking by it\n\nCurrently allowed direction: <b>{Direction}</b>";
			}

			public class WATTSONGAMEOVER
			{
				public static LocString NAME = "Colony Lost";

				public static LocString TOOLTIP = "All Duplicants are dead or incapacitated";
			}

			public class INVALIDBUILDINGLOCATION
			{
				public static LocString NAME = "Invalid Building Location";

				public static LocString TOOLTIP = "Cannot construct a building in this location";
			}

			public class LIQUIDVENTOBSTRUCTED
			{
				public static LocString NAME = "Liquid Vent Obstructed";

				public static LocString TOOLTIP = "A " + UI.PRE_KEYWORD + "Pipe" + UI.PST_KEYWORD + " has been obstructed and is preventing " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " flow to this vent";
			}

			public class LIQUIDVENTOVERPRESSURE
			{
				public static LocString NAME = "Liquid Vent Overpressure";

				public static LocString TOOLTIP = "High " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " or " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " pressure in this area is preventing further " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " emission\nReduce pressure by pumping " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " away or clearing more space";
			}

			public class MANUALLYCONTROLLED
			{
				public static LocString NAME = "Manually Controlled";

				public static LocString TOOLTIP = "This Duplicant is under my control";
			}

			public class LIMITVALVELIMITREACHED
			{
				public static LocString NAME = "Limit Reached";

				public static LocString TOOLTIP = "No more Mass can be transferred";
			}

			public class LIMITVALVELIMITNOTREACHED
			{
				public static LocString NAME = "Amount remaining: {0}";

				public static LocString TOOLTIP = "This building will stop transferring Mass when the amount remaining reaches 0";
			}

			public class MATERIALSUNAVAILABLE
			{
				public static LocString NAME = "Insufficient Resources\n{ItemsRemaining}";

				public static LocString TOOLTIP = "Crucial materials for this building are beyond reach or unavailable";

				public static LocString NOTIFICATION_NAME = "Building lacks resources";

				public static LocString NOTIFICATION_TOOLTIP = "Crucial materials are unavailable or beyond reach for these buildings:";

				public static LocString LINE_ITEM_MASS = "• {0}: {1}";

				public static LocString LINE_ITEM_UNITS = "• {0}";
			}

			public class MATERIALSUNAVAILABLEFORREFILL
			{
				public static LocString NAME = "Resources Low\n{ItemsRemaining}";

				public static LocString TOOLTIP = "This building will soon require materials that are unavailable";

				public static LocString LINE_ITEM = "• {0}";
			}

			public class MELTINGDOWN
			{
				public static LocString NAME = "Breaking Down";

				public static LocString TOOLTIP = "This building is collapsing";

				public static LocString NOTIFICATION_NAME = "Building breakdown";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings are collapsing:";
			}

			public class MISSINGFOUNDATION
			{
				public static LocString NAME = "Missing Tile";

				public static LocString TOOLTIP = "Build " + UI.PRE_KEYWORD + "Tile" + UI.PST_KEYWORD + " beneath this building to regain function" + UI.HORIZONTAL_BR_RULE + "Tile can be found in the " + UI.FormatAsBuildMenuTab("Base Tab") + " " + UI.FormatAsHotkey("[1]") + " of the Build Menu";
			}

			public class NEUTRONIUMUNMINABLE
			{
				public static LocString NAME = "Cannot Mine";

				public static LocString TOOLTIP = "This resource cannot be mined by Duplicant tools";
			}

			public class NEEDGASIN
			{
				public static LocString NAME = "No Gas Intake\n{GasRequired}";

				public static LocString TOOLTIP = string.Concat("This building's ", UI.PRE_KEYWORD, "Gas Intake", UI.PST_KEYWORD, " does not have a ", BUILDINGS.PREFABS.GASCONDUIT.NAME, " connected");

				public static LocString LINE_ITEM = "• {0}";
			}

			public class NEEDGASOUT
			{
				public static LocString NAME = "No Gas Output";

				public static LocString TOOLTIP = string.Concat("This building's ", UI.PRE_KEYWORD, "Gas Output", UI.PST_KEYWORD, " does not have a ", BUILDINGS.PREFABS.GASCONDUIT.NAME, " connected");
			}

			public class NEEDLIQUIDIN
			{
				public static LocString NAME = "No Liquid Intake\n{LiquidRequired}";

				public static LocString TOOLTIP = string.Concat("This building's ", UI.PRE_KEYWORD, "Liquid Intake", UI.PST_KEYWORD, " does not have a ", BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME, " connected");

				public static LocString LINE_ITEM = "• {0}";
			}

			public class NEEDLIQUIDOUT
			{
				public static LocString NAME = "No Liquid Output";

				public static LocString TOOLTIP = string.Concat("This building's ", UI.PRE_KEYWORD, "Liquid Output", UI.PST_KEYWORD, " does not have a ", BUILDINGS.PREFABS.LIQUIDCONDUIT.NAME, " connected");
			}

			public class LIQUIDPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " in this pipe";
			}

			public class LIQUIDPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class GASPIPEEMPTY
			{
				public static LocString NAME = "Empty Pipe";

				public static LocString TOOLTIP = "There is no " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " in this pipe";
			}

			public class GASPIPEOBSTRUCTED
			{
				public static LocString NAME = "Not Pumping";

				public static LocString TOOLTIP = "This pump is not active";
			}

			public class NEEDSOLIDIN
			{
				public static LocString NAME = "No Conveyor Loader";

				public static LocString TOOLTIP = "Material cannot be fed onto this Conveyor system for transport" + UI.HORIZONTAL_BR_RULE + "Enter the " + UI.FormatAsBuildMenuTab("Shipping Tab") + " " + UI.FormatAsHotkey("[7]") + " of the Build Menu to build and connect a " + UI.PRE_KEYWORD + "Conveyor Loader" + UI.PST_KEYWORD;
			}

			public class NEEDSOLIDOUT
			{
				public static LocString NAME = "No Conveyor Receptacle";

				public static LocString TOOLTIP = "Material cannot be offloaded from this Conveyor system and will backup the rails" + UI.HORIZONTAL_BR_RULE + "Enter the " + UI.FormatAsBuildMenuTab("Shipping Tab") + " " + UI.FormatAsHotkey("[7]") + " of the Build Menu to build and connect a " + UI.PRE_KEYWORD + "Conveyor Receptacle" + UI.PST_KEYWORD;
			}

			public class SOLIDPIPEOBSTRUCTED
			{
				public static LocString NAME = "Conveyor Rail Backup";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Conveyor Rail" + UI.PST_KEYWORD + " cannot carry anymore material" + UI.HORIZONTAL_BR_RULE + "Remove material from the " + UI.PRE_KEYWORD + "Conveyor Receptacle" + UI.PST_KEYWORD + " to free space for more objects";
			}

			public class NEEDPLANT
			{
				public static LocString NAME = "No Seeds";

				public static LocString TOOLTIP = "Uproot wild plants to obtain seeds";
			}

			public class NEEDSEED
			{
				public static LocString NAME = "No Seed Selected";

				public static LocString TOOLTIP = "Uproot wild plants to obtain seeds";
			}

			public class NEEDPOWER
			{
				public static LocString NAME = "No Power";

				public static LocString TOOLTIP = "All connected " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " sources have lost charge";
			}

			public class NOTENOUGHPOWER
			{
				public static LocString NAME = "Insufficient Power";

				public static LocString TOOLTIP = "This building does not have enough stored " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " to run";
			}

			public class POWERLOOPDETECTED
			{
				public static LocString NAME = "Power Loop Detected";

				public static LocString TOOLTIP = "A " + UI.PRE_KEYWORD + "Transformer's" + UI.PST_KEYWORD + " " + UI.PRE_KEYWORD + "Power Output" + UI.PST_KEYWORD + " has been connected back to its own " + UI.PRE_KEYWORD + "Input" + UI.PST_KEYWORD;
			}

			public class NEEDRESOURCE
			{
				public static LocString NAME = "Resource Required";

				public static LocString TOOLTIP = "This building is missing required materials";
			}

			public class NEWDUPLICANTSAVAILABLE
			{
				public static LocString NAME = "Printables Available";

				public static LocString TOOLTIP = "I am ready to print a new colony member or care package";

				public static LocString NOTIFICATION_NAME = "New Printables are available";

				public static LocString NOTIFICATION_TOOLTIP = "The Printing Pod " + UI.FormatAsHotkey("[H]") + " is ready to print a new Duplicant or care package.\nI'll need to select a blueprint:";
			}

			public class NOAPPLICABLERESEARCHSELECTED
			{
				public static LocString NAME = "Inapplicable Research";

				public static LocString TOOLTIP = "This building cannot produce the correct " + UI.PRE_KEYWORD + "Research Type" + UI.PST_KEYWORD + " for the current " + UI.FormatAsLink("Research Focus", "TECH");

				public static LocString NOTIFICATION_NAME = UI.FormatAsLink("Research Center", "ADVANCEDRESEARCHCENTER") + " idle";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings cannot produce the correct " + UI.PRE_KEYWORD + "Research Type" + UI.PST_KEYWORD + " for the selected " + UI.FormatAsLink("Research Focus", "TECH") + ":";
			}

			public class NOAPPLICABLEANALYSISSELECTED
			{
				public static LocString NAME = "No Analysis Focus Selected";

				public static LocString TOOLTIP = "Select an unknown destination from the " + UI.FormatAsManagementMenu("Starmap", "[Z]") + " to begin analysis";

				public static LocString NOTIFICATION_NAME = UI.FormatAsLink("Telescope", "TELESCOPE") + " idle";

				public static LocString NOTIFICATION_TOOLTIP = "These buildings require an analysis focus:";
			}

			public class NOAVAILABLESEED
			{
				public static LocString NAME = "No Seed Available";

				public static LocString TOOLTIP = "The selected " + UI.PRE_KEYWORD + "Seed" + UI.PST_KEYWORD + " is not available";
			}

			public class NOSTORAGEFILTERSET
			{
				public static LocString NAME = "Filters Not Designated";

				public static LocString TOOLTIP = "No resources types are marked for storage in this building";
			}

			public class NOSUITMARKER
			{
				public static LocString NAME = "No Checkpoint";

				public static LocString TOOLTIP = string.Concat("Docks must be placed beside a ", BUILDINGS.PREFABS.CHECKPOINT.NAME, ", opposite the side the checkpoint faces");
			}

			public class SUITMARKERWRONGSIDE
			{
				public static LocString NAME = "Invalid Checkpoint";

				public static LocString TOOLTIP = string.Concat("This building has been built on the wrong side of a ", BUILDINGS.PREFABS.CHECKPOINT.NAME, "\n\nDocks must be placed beside a ", BUILDINGS.PREFABS.CHECKPOINT.NAME, ", opposite the side the checkpoint faces");
			}

			public class NOFILTERELEMENTSELECTED
			{
				public static LocString NAME = "No Filter Selected";

				public static LocString TOOLTIP = "Select a resource to filter";
			}

			public class NOLUREELEMENTSELECTED
			{
				public static LocString NAME = "No Bait Selected";

				public static LocString TOOLTIP = "Select a resource to use as bait";
			}

			public class NOFISHABLEWATERBELOW
			{
				public static LocString NAME = "No Fishable Water";

				public static LocString TOOLTIP = "There are no edible " + UI.PRE_KEYWORD + "Fish" + UI.PST_KEYWORD + " beneath this structure";
			}

			public class NOPOWERCONSUMERS
			{
				public static LocString NAME = "No Power Consumers";

				public static LocString TOOLTIP = "No buildings are connected to this " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " source";
			}

			public class NOWIRECONNECTED
			{
				public static LocString NAME = "No Power Wire Connected";

				public static LocString TOOLTIP = "This building has not been connected to a " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " grid";
			}

			public class PENDINGDECONSTRUCTION
			{
				public static LocString NAME = "Deconstruction Errand";

				public static LocString TOOLTIP = "Building will be deconstructed once a Duplicant is available";
			}

			public class PENDINGDEMOLITION
			{
				public static LocString NAME = "Demolition Errand";

				public static LocString TOOLTIP = "Object will be permanently demolished once a Duplicant is available";
			}

			public class PENDINGFISH
			{
				public static LocString NAME = "Fishing Errand";

				public static LocString TOOLTIP = "Spot will be fished once a Duplicant is available";
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

			public class PENDINGREPAIR
			{
				public static LocString NAME = "Repair Errand";

				public static LocString TOOLTIP = "Building will be repaired once a Duplicant is available\nReceived damage from {DamageInfo}";
			}

			public class PENDINGSWITCHTOGGLE
			{
				public static LocString NAME = "Settings Errand";

				public static LocString TOOLTIP = "Settings will be changed once a Duplicant is available";
			}

			public class PENDINGWORK
			{
				public static LocString NAME = "Work Errand";

				public static LocString TOOLTIP = "Building will be operated once a Duplicant is available";
			}

			public class POWERBUTTONOFF
			{
				public static LocString NAME = "Function Suspended";

				public static LocString TOOLTIP = "This building has been toggled off\nPress " + UI.PRE_KEYWORD + "Enable Building" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[ENTER]") + " to resume its use";
			}

			public class PUMPINGSTATION
			{
				public static LocString NAME = "Liquid Available: {Liquids}";

				public static LocString TOOLTIP = "This pumping station has access to: {Liquids}";
			}

			public class PRESSUREOK
			{
				public static LocString NAME = "Max Gas Pressure";

				public static LocString TOOLTIP = "High ambient " + UI.PRE_KEYWORD + "Gas Pressure" + UI.PST_KEYWORD + " is preventing this building from emitting gas" + UI.HORIZONTAL_BR_RULE + "Reduce pressure by pumping " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " away or clearing more space";
			}

			public class UNDERPRESSURE
			{
				public static LocString NAME = "Low Air Pressure";

				public static LocString TOOLTIP = "A minimum atmospheric pressure of <b>{TargetPressure}</b> is needed for this building to operate";
			}

			public class STORAGELOCKER
			{
				public static LocString NAME = "Storing: {Stored} / {Capacity} {Units}";

				public static LocString TOOLTIP = "This container is storing <b>{Stored}{Units}</b> of a maximum <b>{Capacity}{Units}</b>";
			}

			public class SKILL_POINTS_AVAILABLE
			{
				public static LocString NAME = "Skill Points Available";

				public static LocString TOOLTIP = "A Duplicant has " + UI.PRE_KEYWORD + "Skill Points" + UI.PST_KEYWORD + " available";
			}

			public class TANNINGLIGHTSUFFICIENT
			{
				public static LocString NAME = "Tanning Light Available";

				public static LocString TOOLTIP = "There is sufficient " + UI.FormatAsLink("Light", "LIGHT") + " here to create pleasing skin crisping";
			}

			public class TANNINGLIGHTINSUFFICIENT
			{
				public static LocString NAME = "Insufficient Tanning Light";

				public static LocString TOOLTIP = "The " + UI.FormatAsLink("Light", "LIGHT") + " here is not bright enough for that Sunny Day feeling";
			}

			public class UNASSIGNED
			{
				public static LocString NAME = "Unassigned";

				public static LocString TOOLTIP = "Assign a Duplicant to use this amenity";
			}

			public class UNDERCONSTRUCTION
			{
				public static LocString NAME = "Under Construction";

				public static LocString TOOLTIP = "This building is currently being built";
			}

			public class UNDERCONSTRUCTIONNOWORKER
			{
				public static LocString NAME = "Construction Errand";

				public static LocString TOOLTIP = "Building will be constructed once a Duplicant is available";
			}

			public class WAITINGFORMATERIALS
			{
				public static LocString NAME = "Awaiting Delivery\n{ItemsRemaining}";

				public static LocString TOOLTIP = "These materials will be delivered once a Duplicant is available";

				public static LocString LINE_ITEM_MASS = "• {0}: {1}";

				public static LocString LINE_ITEM_UNITS = "• {0}";
			}

			public class WAITINGFORRADIATION
			{
				public static LocString NAME = "Awaiting Radbolts";

				public static LocString TOOLTIP = "This building requires Radbolts to function" + UI.HORIZONTAL_BR_RULE + "Open the " + UI.FormatAsOverlay("Radiation Overlay") + " " + UI.FormatAsHotkey("[L-SHIFT + F4]") + " to view this building's radiation port";
			}

			public class WAITINGFORREPAIRMATERIALS
			{
				public static LocString NAME = "Awaiting Repair Delivery\n{ItemsRemaining}\n";

				public static LocString TOOLTIP = "These materials must be delivered before this building can be repaired";

				public static LocString LINE_ITEM = "• " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD + ": <b>{1}</b>";
			}

			public class MISSINGGANTRY
			{
				public static LocString NAME = "Missing Gantry";

				public static LocString TOOLTIP = "A " + UI.FormatAsLink("Gantry", "GANTRY") + " must be built below " + UI.FormatAsLink("Command Capsules", "COMMANDMODULE") + " and " + UI.FormatAsLink("Sight-Seeing Modules", "TOURISTMODULE") + " for Duplicant access";
			}

			public class DISEMBARKINGDUPLICANT
			{
				public static LocString NAME = "Waiting To Disembark";

				public static LocString TOOLTIP = "The Duplicant inside this rocket can't come out until the " + UI.FormatAsLink("Gantry", "GANTRY") + " is extended";
			}

			public class REACTORMELTDOWN
			{
				public static LocString NAME = "Reactor Meltdown";

				public static LocString TOOLTIP = "This reactor is spilling dangerous radioactive waste and cannot be stopped";
			}

			public class ROCKETNAME
			{
				public static LocString NAME = "Parent Rocket: {0}";

				public static LocString TOOLTIP = "This module belongs to the rocket: " + UI.PRE_KEYWORD + "{0}" + UI.PST_KEYWORD;
			}

			public class HASGANTRY
			{
				public static LocString NAME = "Has Gantry";

				public static LocString TOOLTIP = "Duplicants may now enter this section of the rocket";
			}

			public class NORMAL
			{
				public static LocString NAME = "Normal";

				public static LocString TOOLTIP = "Nothing out of the ordinary here";
			}

			public class MANUALGENERATORCHARGINGUP
			{
				public static LocString NAME = "Charging Up";

				public static LocString TOOLTIP = "This power source is being charged";
			}

			public class MANUALGENERATORRELEASINGENERGY
			{
				public static LocString NAME = "Powering";

				public static LocString TOOLTIP = "This generator is supplying energy to " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " consumers";
			}

			public class GENERATOROFFLINE
			{
				public static LocString NAME = "Generator Idle";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " source is idle";
			}

			public class PIPE
			{
				public static LocString NAME = "Contents: {Contents}";

				public static LocString TOOLTIP = "This pipe is delivering {Contents}";
			}

			public class CONVEYOR
			{
				public static LocString NAME = "Contents: {Contents}";

				public static LocString TOOLTIP = "This conveyor is delivering {Contents}";
			}

			public class FABRICATORIDLE
			{
				public static LocString NAME = "No Fabrications Queued";

				public static LocString TOOLTIP = "Select a recipe to begin fabrication";
			}

			public class FABRICATOREMPTY
			{
				public static LocString NAME = "Waiting For Materials";

				public static LocString TOOLTIP = "Fabrication will begin once materials have been delivered";
			}

			public class FABRICATORLACKSHEP
			{
				public static LocString NAME = "Waiting For Radbolts ({CurrentHEP}/{HEPRequired})";

				public static LocString TOOLTIP = "A queued recipe requires more Radbolts than are currently stored.\n\nCurrently stored: {CurrentHEP}\nRequired for recipe: {HEPRequired}";
			}

			public class TOILET
			{
				public static LocString NAME = "{FlushesRemaining} \"Visits\" Remaining";

				public static LocString TOOLTIP = "{FlushesRemaining} more Duplicants can use this amenity before it requires maintenance";
			}

			public class TOILETNEEDSEMPTYING
			{
				public static LocString NAME = "Requires Emptying";

				public static LocString TOOLTIP = "This amenity cannot be used while full" + UI.HORIZONTAL_BR_RULE + "Emptying it will produce " + UI.FormatAsLink("Polluted Dirt", "TOXICSAND");
			}

			public class DESALINATORNEEDSEMPTYING
			{
				public static LocString NAME = "Requires Emptying";

				public static LocString TOOLTIP = "This building needs to be emptied of " + UI.FormatAsLink("Salt", "SALT") + " to resume function";
			}

			public class HABITATNEEDSEMPTYING
			{
				public static LocString NAME = "Requires Emptying";

				public static LocString TOOLTIP = "This " + UI.FormatAsLink("Algae Terrarium", "ALGAEHABITAT") + " needs to be emptied of " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + UI.HORIZONTAL_BR_RULE + UI.FormatAsLink("Bottle Emptiers", "BOTTLEEMPTIER") + " can be used to transport and dispose of " + UI.FormatAsLink("Polluted Water", "DIRTYWATER") + " in designated areas";
			}

			public class UNUSABLE
			{
				public static LocString NAME = "Out of Order";

				public static LocString TOOLTIP = "This amenity requires maintenance";
			}

			public class NORESEARCHSELECTED
			{
				public static LocString NAME = "No Research Focus Selected";

				public static LocString TOOLTIP = "Open the " + UI.FormatAsManagementMenu("Research Tree", "[R]") + " to select a new " + UI.FormatAsLink("Research", "TECH") + " project";

				public static LocString NOTIFICATION_NAME = "No " + UI.FormatAsLink("Research Focus", "TECH") + " selected";

				public static LocString NOTIFICATION_TOOLTIP = "Open the " + UI.FormatAsManagementMenu("Research Tree", "[R]") + " to select a new " + UI.FormatAsLink("Research", "TECH") + " project";
			}

			public class NORESEARCHORDESTINATIONSELECTED
			{
				public static LocString NAME = "No Research Focus or Starmap Destination Selected";

				public static LocString TOOLTIP = "Select a " + UI.FormatAsLink("Research", "TECH") + " project in the " + UI.FormatAsManagementMenu("Research Tree", "{Hotkey}") + " or a Destination in the " + UI.FormatAsManagementMenu("Starmap", "[Z]");

				public static LocString NOTIFICATION_NAME = "No " + UI.FormatAsLink("Research Focus", "TECH") + " or Starmap destination selected";

				public static LocString NOTIFICATION_TOOLTIP = "Select a " + UI.FormatAsLink("Research", "TECH") + " project in the " + UI.FormatAsManagementMenu("Research Tree", "[R]") + " or a Destination in the " + UI.FormatAsManagementMenu("Starmap", "[Z]");
			}

			public class RESEARCHING
			{
				public static LocString NAME = "Current " + UI.FormatAsLink("Research", "TECH") + ": {Tech}";

				public static LocString TOOLTIP = "Research produced at this station will be invested in {Tech}";
			}

			public class TINKERING
			{
				public static LocString NAME = "Tinkering: {0}";

				public static LocString TOOLTIP = "This Duplicant is creating {0} to use somewhere else";
			}

			public class VALVE
			{
				public static LocString NAME = "Max Flow Rate: {MaxFlow}";

				public static LocString TOOLTIP = "This valve is allowing flow at a volume of <b>{MaxFlow}</b>";
			}

			public class VALVEREQUEST
			{
				public static LocString NAME = "Requested Flow Rate: {QueuedMaxFlow}";

				public static LocString TOOLTIP = "Waiting for a Duplicant to adjust flow rate";
			}

			public class EMITTINGLIGHT
			{
				public static LocString NAME = "Emitting Light";

				public static LocString TOOLTIP = "Open the " + UI.FormatAsOverlay("Light Overlay", "[{LightGridOverlay}]") + " to view this light's visibility radius";
			}

			public class RATIONBOXCONTENTS
			{
				public static LocString NAME = "Storing: {Stored}";

				public static LocString TOOLTIP = "This box contains <b>{Stored}</b> of " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD;
			}

			public class EMITTINGELEMENT
			{
				public static LocString NAME = "Emitting {ElementType}: {FlowRate}";

				public static LocString TOOLTIP = "Producing {ElementType} at " + UI.FormatAsPositiveRate("{FlowRate}");
			}

			public class EMITTINGCO2
			{
				public static LocString NAME = "Emitting CO<sub>2</sub>: {FlowRate}";

				public static LocString TOOLTIP = string.Concat("Producing ", ELEMENTS.CARBONDIOXIDE.NAME, " at ", UI.FormatAsPositiveRate("{FlowRate}"));
			}

			public class EMITTINGOXYGENAVG
			{
				public static LocString NAME = "Emitting " + UI.FormatAsLink("Oxygen", "OXYGEN") + ": {FlowRate}";

				public static LocString TOOLTIP = string.Concat("Producing ", ELEMENTS.OXYGEN.NAME, " at a rate of ", UI.FormatAsPositiveRate("{FlowRate}"));
			}

			public class EMITTINGGASAVG
			{
				public static LocString NAME = "Emitting {Element}: {FlowRate}";

				public static LocString TOOLTIP = "Producing {Element} at a rate of " + UI.FormatAsPositiveRate("{FlowRate}");
			}

			public class EMITTINGBLOCKEDHIGHPRESSURE
			{
				public static LocString NAME = "Not Emitting: Overpressure";

				public static LocString TOOLTIP = "Ambient pressure is too high for {Element} to be released";
			}

			public class EMITTINGBLOCKEDLOWTEMPERATURE
			{
				public static LocString NAME = "Not Emitting: Too Cold";

				public static LocString TOOLTIP = "Temperature is too low for {Element} to be released";
			}

			public class PUMPINGLIQUIDORGAS
			{
				public static LocString NAME = "Average Flow Rate: {FlowRate}";

				public static LocString TOOLTIP = "This building is pumping an average volume of " + UI.FormatAsPositiveRate("{FlowRate}");
			}

			public class WIRECIRCUITSTATUS
			{
				public static LocString NAME = "Current Load: {CurrentLoadAndColor} / {MaxLoad}";

				public static LocString TOOLTIP = "The current " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " load on this wire\n\nOverloading a wire will cause damage to the wire over time and cause it to break";
			}

			public class WIREMAXWATTAGESTATUS
			{
				public static LocString NAME = "Potential Load: {TotalPotentialLoadAndColor} / {MaxLoad}";

				public static LocString TOOLTIP = "How much wattage this network will draw if all " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " consumers on the network become active at once";
			}

			public class NOLIQUIDELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Liquid";

				public static LocString TOOLTIP = "This pump must be submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " to work";
			}

			public class NOGASELEMENTTOPUMP
			{
				public static LocString NAME = "Pump Not In Gas";

				public static LocString TOOLTIP = "This pump must be submerged in " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " to work";
			}

			public class INVALIDMASKSTATIONCONSUMPTIONSTATE
			{
				public static LocString NAME = "Station Not In Oxygen";

				public static LocString TOOLTIP = "This station must be submerged in " + UI.PRE_KEYWORD + "Oxygen" + UI.PST_KEYWORD + " to work";
			}

			public class PIPEMAYMELT
			{
				public static LocString NAME = "High Melt Risk";

				public static LocString TOOLTIP = "This pipe is in danger of melting at the current " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD;
			}

			public class ELEMENTEMITTEROUTPUT
			{
				public static LocString NAME = "Emitting {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This object is releasing {ElementTypes} at a rate of " + UI.FormatAsPositiveRate("{FlowRate}");
			}

			public class ELEMENTCONSUMER
			{
				public static LocString NAME = "Consuming {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This object is utilizing ambient {ElementTypes} from the environment";
			}

			public class SPACECRAFTREADYTOLAND
			{
				public static LocString NAME = "Spacecraft ready to land";

				public static LocString TOOLTIP = "A spacecraft is ready to land";

				public static LocString NOTIFICATION = "Space mission complete";

				public static LocString NOTIFICATION_TOOLTIP = "Spacecrafts have completed their missions";
			}

			public class CONSUMINGFROMSTORAGE
			{
				public static LocString NAME = "Consuming {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is consuming {ElementTypes} from storage";
			}

			public class ELEMENTCONVERTEROUTPUT
			{
				public static LocString NAME = "Emitting {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is releasing {ElementTypes} at a rate of " + UI.FormatAsPositiveRate("{FlowRate}");
			}

			public class ELEMENTCONVERTERINPUT
			{
				public static LocString NAME = "Using {ElementTypes}: {FlowRate}";

				public static LocString TOOLTIP = "This building is using {ElementTypes} from storage at a rate of " + UI.FormatAsNegativeRate("{FlowRate}");
			}

			public class AWAITINGCOMPOSTFLIP
			{
				public static LocString NAME = "Requires Flipping";

				public static LocString TOOLTIP = "Compost must be flipped periodically to produce " + UI.FormatAsLink("Dirt", "DIRT");
			}

			public class AWAITINGWASTE
			{
				public static LocString NAME = "Awaiting Compostables";

				public static LocString TOOLTIP = "More waste material is required to begin the composting process";
			}

			public class BATTERIESSUFFICIENTLYFULL
			{
				public static LocString NAME = "Batteries Sufficiently Full";

				public static LocString TOOLTIP = "All batteries are above the refill threshold";
			}

			public class NEEDRESOURCEMASS
			{
				public static LocString NAME = "Insufficient Resources\n{ResourcesRequired}";

				public static LocString TOOLTIP = "The mass of material that was delivered to this building was too low\n\nDeliver more material to run this building";

				public static LocString LINE_ITEM = "• <b>{0}</b>";
			}

			public class JOULESAVAILABLE
			{
				public static LocString NAME = "Power Available: {JoulesAvailable} / {JoulesCapacity}";

				public static LocString TOOLTIP = "<b>{JoulesAvailable}</b> of stored " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " available for use";
			}

			public class WATTAGE
			{
				public static LocString NAME = "Wattage: {Wattage}";

				public static LocString TOOLTIP = "This building is generating " + UI.FormatAsPositiveRate("{Wattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD;
			}

			public class SOLARPANELWATTAGE
			{
				public static LocString NAME = "Current Wattage: {Wattage}";

				public static LocString TOOLTIP = "This panel is generating " + UI.FormatAsPositiveRate("{Wattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD;
			}

			public class MODULESOLARPANELWATTAGE
			{
				public static LocString NAME = "Current Wattage: {Wattage}";

				public static LocString TOOLTIP = "This panel is generating " + UI.FormatAsPositiveRate("{Wattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD;
			}

			public class WATTSON
			{
				public static LocString NAME = "Next Print: {TimeRemaining}";

				public static LocString TOOLTIP = "The Printing Pod can print out new Duplicants and useful resources over time.\nThe next print will be ready in <b>{TimeRemaining}</b>";

				public static LocString UNAVAILABLE = "UNAVAILABLE";
			}

			public class FLUSHTOILET
			{
				public static LocString NAME = "Lavatory Ready";

				public static LocString TOOLTIP = "This bathroom is ready to receive visitors";
			}

			public class FLUSHTOILETINUSE
			{
				public static LocString NAME = "Lavatory In Use";

				public static LocString TOOLTIP = "This bathroom is occupied";
			}

			public class WIRECONNECTED
			{
				public static LocString NAME = "Wire Connected";

				public static LocString TOOLTIP = "This wire is connected to a network";
			}

			public class WIRENOMINAL
			{
				public static LocString NAME = "Wire Nominal";

				public static LocString TOOLTIP = "This wire is able to handle the wattage it is receiving";
			}

			public class WIREDISCONNECTED
			{
				public static LocString NAME = "Wire Disconnected";

				public static LocString TOOLTIP = "This wire is not connecting a " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " consumer to a " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " generator";
			}

			public class COOLING
			{
				public static LocString NAME = "Cooling";

				public static LocString TOOLTIP = "This building is cooling the surrounding area";
			}

			public class COOLINGSTALLEDHOTENV
			{
				public static LocString NAME = "Gas Too Hot";

				public static LocString TOOLTIP = "Incoming pipe contents cannot be cooled more than <b>{2}</b> below the surrounding environment\n\nEnvironment: {0}\nCurrent Pipe Contents: {1}";
			}

			public class COOLINGSTALLEDCOLDGAS
			{
				public static LocString NAME = "Gas Too Cold";

				public static LocString TOOLTIP = "This building cannot cool incoming pipe contents below <b>{0}</b>\n\nCurrent Pipe Contents: {0}";
			}

			public class COOLINGSTALLEDHOTLIQUID
			{
				public static LocString NAME = "Liquid Too Hot";

				public static LocString TOOLTIP = "Incoming pipe contents cannot be cooled more than <b>{2}</b> below the surrounding environment\n\nEnvironment: {0}\nCurrent Pipe Contents: {1}";
			}

			public class COOLINGSTALLEDCOLDLIQUID
			{
				public static LocString NAME = "Liquid Too Cold";

				public static LocString TOOLTIP = "This building cannot cool incoming pipe contents below <b>{0}</b>\n\nCurrent Pipe Contents: {0}";
			}

			public class CANNOTCOOLFURTHER
			{
				public static LocString NAME = "Minimum Temperature Reached";

				public static LocString TOOLTIP = "This building cannot cool the surrounding environment below <b>{0}</b>";
			}

			public class HEATINGSTALLEDHOTENV
			{
				public static LocString NAME = "Target Temperature Reached";

				public static LocString TOOLTIP = "This building cannot heat the surrounding environment beyond <b>{0}</b>";
			}

			public class HEATINGSTALLEDLOWMASS_GAS
			{
				public static LocString NAME = "Low Air Pressure";

				public static LocString TOOLTIP = "A minimum atmospheric pressure of <b>{TargetPressure}</b> is needed for this building to heat up";
			}

			public class HEATINGSTALLEDLOWMASS_LIQUID
			{
				public static LocString NAME = "Not Submerged In Liquid";

				public static LocString TOOLTIP = "This building must be submerged in " + UI.PRE_KEYWORD + "Liquid" + UI.PST_KEYWORD + " to function";
			}

			public class BUILDINGDISABLED
			{
				public static LocString NAME = "Building Disabled";

				public static LocString TOOLTIP = "Press " + UI.PRE_KEYWORD + "Enable Building" + UI.PST_KEYWORD + " " + UI.FormatAsHotkey("[ENTER]") + " to resume use";
			}

			public class MISSINGREQUIREMENTS
			{
				public static LocString NAME = "Missing Requirements";

				public static LocString TOOLTIP = "There are some problems that need to be fixed before this building is operational";
			}

			public class GETTINGREADY
			{
				public static LocString NAME = "Getting Ready";

				public static LocString TOOLTIP = "This building will soon be ready to use";
			}

			public class WORKING
			{
				public static LocString NAME = "Nominal";

				public static LocString TOOLTIP = "This building is working as intended";
			}

			public class GRAVEEMPTY
			{
				public static LocString NAME = "Empty";

				public static LocString TOOLTIP = "This memorial honors no one.";
			}

			public class GRAVE
			{
				public static LocString NAME = "RIP {DeadDupe}";

				public static LocString TOOLTIP = "{Epitaph}";
			}

			public class AWAITINGARTING
			{
				public static LocString NAME = "Incomplete Artwork";

				public static LocString TOOLTIP = "This building requires a Duplicant's artistic touch";
			}

			public class LOOKINGUGLY
			{
				public static LocString NAME = "Crude";

				public static LocString TOOLTIP = "Honestly, Morbs could've done better";
			}

			public class LOOKINGOKAY
			{
				public static LocString NAME = "Quaint";

				public static LocString TOOLTIP = "Duplicants find this art piece quite charming";
			}

			public class LOOKINGGREAT
			{
				public static LocString NAME = "Masterpiece";

				public static LocString TOOLTIP = "This poignant piece stirs something deep within each Duplicant's soul";
			}

			public class EXPIRED
			{
				public static LocString NAME = "Depleted";

				public static LocString TOOLTIP = "This building has no more use";
			}

			public class EXCAVATOR_BOMB
			{
				public class UNARMED
				{
					public static LocString NAME = "Unarmed";

					public static LocString TOOLTIP = "This explosive is currently inactive";
				}

				public class ARMED
				{
					public static LocString NAME = "Armed";

					public static LocString TOOLTIP = "Stand back, this baby's ready to blow!";
				}

				public class COUNTDOWN
				{
					public static LocString NAME = "Countdown: {0}";

					public static LocString TOOLTIP = "<b>{0}</b> seconds until detonation";
				}

				public class DUPE_DANGER
				{
					public static LocString NAME = "Duplicant Preservation Override";

					public static LocString TOOLTIP = "Explosive disabled due to close Duplicant proximity";
				}

				public class EXPLODING
				{
					public static LocString NAME = "Exploding";

					public static LocString TOOLTIP = "Kaboom!";
				}
			}

			public class BURNER
			{
				public class BURNING_FUEL
				{
					public static LocString NAME = "Consuming Fuel: {0}";

					public static LocString TOOLTIP = "<b>{0}</b> fuel remaining";
				}

				public class HAS_FUEL
				{
					public static LocString NAME = "Fueled: {0}";

					public static LocString TOOLTIP = "<b>{0}</b> fuel remaining";
				}
			}

			public class CREATURE_TRAP
			{
				public class NEEDSBAIT
				{
					public static LocString NAME = "Needs Bait";

					public static LocString TOOLTIP = "This trap needs to be baited before it can be set";
				}

				public class READY
				{
					public static LocString NAME = "Set";

					public static LocString TOOLTIP = "This trap has been set and is ready to catch a " + UI.PRE_KEYWORD + "Critter" + UI.PST_KEYWORD;
				}

				public class SPRUNG
				{
					public static LocString NAME = "Sprung";

					public static LocString TOOLTIP = "This trap has caught a {0}!";
				}
			}

			public class ACCESS_CONTROL
			{
				public class ACTIVE
				{
					public static LocString NAME = "Access Restrictions";

					public static LocString TOOLTIP = "Some Duplicants are prohibited from passing through this door by the current " + UI.PRE_KEYWORD + "Access Permissions" + UI.PST_KEYWORD;
				}

				public class OFFLINE
				{
					public static LocString NAME = "Access Control Offline";

					public static LocString TOOLTIP = "This door has granted Emergency " + UI.PRE_KEYWORD + "Access Permissions" + UI.PST_KEYWORD + UI.HORIZONTAL_BR_RULE + "All Duplicants are permitted to pass through it until " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " is restored";
				}
			}

			public class REQUIRESSKILLPERK
			{
				public static LocString NAME = "Skill-Required Operation";

				public static LocString TOOLTIP = "Only Duplicants with one of the following " + UI.PRE_KEYWORD + "Skills" + UI.PST_KEYWORD + " can operate this building:\n{Skills}";
			}

			public class DIGREQUIRESSKILLPERK
			{
				public static LocString NAME = "Skill-Required Dig";

				public static LocString TOOLTIP = "Only Duplicants with one of the following " + UI.PRE_KEYWORD + "Skills" + UI.PST_KEYWORD + " can mine this material:\n{Skills}";
			}

			public class COLONYLACKSREQUIREDSKILLPERK
			{
				public static LocString NAME = "Colony Lacks {Skills}";

				public static LocString TOOLTIP = "{Skills} Skill required to operate" + UI.HORIZONTAL_BR_RULE + "Open the " + UI.FormatAsManagementMenu("Skills Panel", "[L]") + " to teach {Skills} to a Duplicant";
			}

			public class CLUSTERCOLONYLACKSREQUIREDSKILLPERK
			{
				public static LocString NAME = "Local Colony Lacks {Skills}";

				public static LocString TOOLTIP = string.Concat(COLONYLACKSREQUIREDSKILLPERK.TOOLTIP, ", or bring a Duplicant with the skill from another ", UI.CLUSTERMAP.PLANETOID);
			}

			public class WORKREQUIRESMINION
			{
				public static LocString NAME = "Duplicant Operation Required";

				public static LocString TOOLTIP = "A Duplicant must be present to complete this operation";
			}

			public class SWITCHSTATUSACTIVE
			{
				public static LocString NAME = "Active";

				public static LocString TOOLTIP = "This switch is currently toggled <b>On</b>";
			}

			public class SWITCHSTATUSINACTIVE
			{
				public static LocString NAME = "Inactive";

				public static LocString TOOLTIP = "This switch is currently toggled <b>Off</b>";
			}

			public class LOGICSWITCHSTATUSACTIVE
			{
				public static LocString NAME = "Sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);

				public static LocString TOOLTIP = "This switch is currently sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);
			}

			public class LOGICSWITCHSTATUSINACTIVE
			{
				public static LocString NAME = "Sending a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString TOOLTIP = "This switch is currently sending a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class LOGICSENSORSTATUSACTIVE
			{
				public static LocString NAME = "Sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);

				public static LocString TOOLTIP = "This sensor is currently sending a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active);
			}

			public class LOGICSENSORSTATUSINACTIVE
			{
				public static LocString NAME = "Sending a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);

				public static LocString TOOLTIP = "This sensor is currently sending " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby);
			}

			public class PLAYERCONTROLLEDTOGGLESIDESCREEN
			{
				public static LocString NAME = "Pending Toggle on Unpause";

				public static LocString TOOLTIP = "This will be toggled when time is unpaused";
			}

			public class FOOD_CONTAINERS_OUTSIDE_RANGE
			{
				public static LocString NAME = "Unreachable food";

				public static LocString TOOLTIP = "Recuperating Duplicants must have " + UI.PRE_KEYWORD + "Food" + UI.PST_KEYWORD + " available within <b>{0}</b> cells";
			}

			public class TOILETS_OUTSIDE_RANGE
			{
				public static LocString NAME = "Unreachable restroom";

				public static LocString TOOLTIP = "Recuperating Duplicants must have " + UI.PRE_KEYWORD + "Toilets" + UI.PST_KEYWORD + " available within <b>{0}</b> cells";
			}

			public class BUILDING_DEPRECATED
			{
				public static LocString NAME = "Building Deprecated";

				public static LocString TOOLTIP = "This building is from an older version of the game and its use is not intended";
			}

			public class TURBINE_BLOCKED_INPUT
			{
				public static LocString NAME = "All Inputs Blocked";

				public static LocString TOOLTIP = string.Concat("This turbine's ", UI.PRE_KEYWORD, "Input Vents", UI.PST_KEYWORD, " are blocked, so it can't intake any ", ELEMENTS.STEAM.NAME, ".\n\nThe ", UI.PRE_KEYWORD, "Input Vents", UI.PST_KEYWORD, " are located directly below the foundation ", UI.PRE_KEYWORD, "Tile", UI.PST_KEYWORD, " this building is resting on.");
			}

			public class TURBINE_PARTIALLY_BLOCKED_INPUT
			{
				public static LocString NAME = "{Blocked}/{Total} Inputs Blocked";

				public static LocString TOOLTIP = "<b>{Blocked}</b> of this turbine's <b>{Total}</b> inputs have been blocked, resulting in reduced throughput";
			}

			public class TURBINE_TOO_HOT
			{
				public static LocString NAME = "Turbine Too Hot";

				public static LocString TOOLTIP = "This turbine must be below <b>{Overheat_Temperature}</b> to properly process {Src_Element} into {Dest_Element}";
			}

			public class TURBINE_BLOCKED_OUTPUT
			{
				public static LocString NAME = "Output Blocked";

				public static LocString TOOLTIP = "A blocked " + UI.PRE_KEYWORD + "Output" + UI.PST_KEYWORD + " has stopped this turbine from functioning";
			}

			public class TURBINE_INSUFFICIENT_MASS
			{
				public static LocString NAME = "Not Enough {Src_Element}";

				public static LocString TOOLTIP = "The {Src_Element} present below this turbine must be at least <b>{Min_Mass}</b> in order to turn the turbine";
			}

			public class TURBINE_INSUFFICIENT_TEMPERATURE
			{
				public static LocString NAME = "{Src_Element} Temperature Below {Active_Temperature}";

				public static LocString TOOLTIP = "This turbine requires {Src_Element} that is a minimum of <b>{Active_Temperature}</b> in order to produce power";
			}

			public class TURBINE_ACTIVE_WATTAGE
			{
				public static LocString NAME = "Current Wattage: {Wattage}";

				public static LocString TOOLTIP = "This turbine is generating " + UI.FormatAsPositiveRate("{Wattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + "\n\nIt is running at <b>{Efficiency}</b> of full capacity\n\nIncrease {Src_Element} " + UI.PRE_KEYWORD + "Mass" + UI.PST_KEYWORD + " and " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " to improve output";
			}

			public class TURBINE_SPINNING_UP
			{
				public static LocString NAME = "Spinning Up";

				public static LocString TOOLTIP = "This turbine is currently spinning up\n\nSpinning up allows a turbine to continue running for a short period if the pressure it needs to run becomes unavailable";
			}

			public class TURBINE_ACTIVE
			{
				public static LocString NAME = "Active";

				public static LocString TOOLTIP = "This turbine is running at <b>{0}RPM</b>";
			}

			public class WELL_PRESSURIZING
			{
				public static LocString NAME = "Backpressure: {0}";

				public static LocString TOOLTIP = "Well pressure increases with each use and must be periodically relieved to prevent shutdown";
			}

			public class WELL_OVERPRESSURE
			{
				public static LocString NAME = "Overpressure";

				public static LocString TOOLTIP = "This well can no longer function due to excessive backpressure";
			}

			public class NOTINANYROOM
			{
				public static LocString NAME = "Outside of room";

				public static LocString TOOLTIP = "This building must be built inside a " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " for full functionality\n\nOpen the " + UI.FormatAsOverlay("Room Overlay", "[F11]") + " to view full " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " status";
			}

			public class NOTINREQUIREDROOM
			{
				public static LocString NAME = "Outside of {0}";

				public static LocString TOOLTIP = "This building must be built inside a {0} for full functionality\n\nOpen the " + UI.FormatAsOverlay("Room Overlay", "[F11]") + " to view full " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " status";
			}

			public class NOTINRECOMMENDEDROOM
			{
				public static LocString NAME = "Outside of {0}";

				public static LocString TOOLTIP = "It is recommended to build this building inside a {0}\n\nOpen the " + UI.FormatAsOverlay("Room Overlay", "[F11]") + " to view full " + UI.PRE_KEYWORD + "Room" + UI.PST_KEYWORD + " status";
			}

			public class RELEASING_PRESSURE
			{
				public static LocString NAME = "Releasing Pressure";

				public static LocString TOOLTIP = "Pressure buildup is being safely released";
			}

			public class LOGIC_FEEDBACK_LOOP
			{
				public static LocString NAME = "Feedback Loop";

				public static LocString TOOLTIP = "Feedback loops prevent automation grids from functioning\n\nFeedback loops occur when the " + UI.PRE_KEYWORD + "Output" + UI.PST_KEYWORD + " of an automated building connects back to its own " + UI.PRE_KEYWORD + "Input" + UI.PST_KEYWORD + " through the Automation grid";
			}

			public class ENOUGH_COOLANT
			{
				public static LocString NAME = "Awaiting Coolant";

				public static LocString TOOLTIP = "<b>{1}</b> of {0} must be present in storage to begin production";
			}

			public class ENOUGH_FUEL
			{
				public static LocString NAME = "Awaiting Fuel";

				public static LocString TOOLTIP = "<b>{1}</b> of {0} must be present in storage to begin production";
			}

			public class LOGIC
			{
				public static LocString LOGIC_CONTROLLED_ENABLED = "Enabled by Automation Grid";

				public static LocString LOGIC_CONTROLLED_DISABLED = "Disabled by Automation Grid";
			}

			public class GANTRY
			{
				public static LocString AUTOMATION_CONTROL = "Automation Control: {0}";

				public static LocString MANUAL_CONTROL = "Manual Control: {0}";

				public static LocString EXTENDED = "Extended";

				public static LocString RETRACTED = "Retracted";
			}

			public class OBJECTDISPENSER
			{
				public static LocString AUTOMATION_CONTROL = "Automation Control: {0}";

				public static LocString MANUAL_CONTROL = "Manual Control: {0}";

				public static LocString OPENED = "Opened";

				public static LocString CLOSED = "Closed";
			}

			public class TOO_COLD
			{
				public static LocString NAME = "Too Cold";

				public static LocString TOOLTIP = "Either this building or its surrounding environment is too cold to operate";
			}

			public class CHECKPOINT
			{
				public class TOOLTIPS
				{
					public static LocString LOGIC_CONTROLLED_OPEN = "Automated Checkpoint is receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ", preventing Duplicants from passing";

					public static LocString LOGIC_CONTROLLED_CLOSED = "Automated Checkpoint is receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ", allowing Duplicants to pass";

					public static LocString LOGIC_CONTROLLED_DISCONNECTED = "This Checkpoint has not been connected to an " + UI.PRE_KEYWORD + "Automation" + UI.PST_KEYWORD + " grid";
				}

				public static LocString LOGIC_CONTROLLED_OPEN = "Clearance: Permitted";

				public static LocString LOGIC_CONTROLLED_CLOSED = "Clearance: Not Permitted";

				public static LocString LOGIC_CONTROLLED_DISCONNECTED = "No Automation";
			}

			public class HIGHENERGYPARTICLEREDIRECTOR
			{
				public class TOOLTIPS
				{
					public static LocString LOGIC_CONTROLLED_STANDBY = UI.FormatAsKeyWord("Radbolt Reflector") + " is receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ", ignoring incoming " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD;

					public static LocString LOGIC_CONTROLLED_ACTIVE = UI.FormatAsKeyWord("Radbolt Reflector") + " is receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ", accepting incoming " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD;

					public static LocString NORMAL = "Incoming Radbolts will be accepted and redirected";
				}

				public static LocString LOGIC_CONTROLLED_STANDBY = "Incoming Radbolts: Ignore";

				public static LocString LOGIC_CONTROLLED_ACTIVE = "Incoming Radbolts: Redirect";

				public static LocString NORMAL = "Normal";
			}

			public class HIGHENERGYPARTICLESPAWNER
			{
				public class TOOLTIPS
				{
					public static LocString LOGIC_CONTROLLED_STANDBY = UI.FormatAsKeyWord("Radbolt Generator") + " is receiving a " + UI.FormatAsAutomationState("Red Signal", UI.AutomationState.Standby) + ", ignoring incoming " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD;

					public static LocString LOGIC_CONTROLLED_ACTIVE = UI.FormatAsKeyWord("Radbolt Generator") + " is receiving a " + UI.FormatAsAutomationState("Green Signal", UI.AutomationState.Active) + ", accepting incoming " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD;

					public static LocString NORMAL = "Incoming " + UI.PRE_KEYWORD + "Radbolts" + UI.PST_KEYWORD + " will be accepted and redirected";
				}

				public static LocString LOGIC_CONTROLLED_STANDBY = "Launch Radbolt: Off";

				public static LocString LOGIC_CONTROLLED_ACTIVE = "Launch Radbolt: On";

				public static LocString NORMAL = "Normal";
			}

			public class AWAITINGFUEL
			{
				public static LocString NAME = "Awaiting Fuel: {0}";

				public static LocString TOOLTIP = "This building requires <b>{1}</b> of {0} to operate";
			}

			public class NOLOGICWIRECONNECTED
			{
				public static LocString NAME = "No Automation Wire Connected";

				public static LocString TOOLTIP = "This building has not been connected to an " + UI.PRE_KEYWORD + "Automation" + UI.PST_KEYWORD + " grid";
			}

			public class NOTUBECONNECTED
			{
				public static LocString NAME = "No Tube Connected";

				public static LocString TOOLTIP = string.Concat("The first section of tube extending from a ", BUILDINGS.PREFABS.TRAVELTUBEENTRANCE.NAME, " must connect directly upward");
			}

			public class NOTUBEEXITS
			{
				public static LocString NAME = "No Landing Available";

				public static LocString TOOLTIP = "Duplicants can only exit a tube when there is somewhere for them to land within <b>two tiles</b>";
			}

			public class STOREDCHARGE
			{
				public static LocString NAME = "Charge Available: {0}/{1}";

				public static LocString TOOLTIP = "This building has <b>{0}</b> of stored " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + "\n\nIt consumes " + UI.FormatAsNegativeRate("{2}") + " per use";
			}

			public class NEEDEGG
			{
				public static LocString NAME = "No Egg Selected";

				public static LocString TOOLTIP = "Collect " + UI.PRE_KEYWORD + "Eggs" + UI.PST_KEYWORD + " from " + UI.FormatAsLink("Critters", "CREATURES") + " to incubate";
			}

			public class NOAVAILABLEEGG
			{
				public static LocString NAME = "No Egg Available";

				public static LocString TOOLTIP = "The selected " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + " is not currently available";
			}

			public class AWAITINGEGGDELIVERY
			{
				public static LocString NAME = "Awaiting Delivery";

				public static LocString TOOLTIP = "Awaiting delivery of selected " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD;
			}

			public class INCUBATORPROGRESS
			{
				public static LocString NAME = "Incubating: {Percent}";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Egg" + UI.PST_KEYWORD + " incubating cozily" + UI.HORIZONTAL_BR_RULE + "It will hatch when " + UI.PRE_KEYWORD + "Incubation" + UI.PST_KEYWORD + " reaches <b>100%</b>";
			}

			public class DETECTORQUALITY
			{
				public static LocString NAME = "Scan Quality: {Quality}";

				public static LocString TOOLTIP = "This scanner dish is currently scanning at <b>{Quality}</b> effectiveness\n\nDecreased scan quality may be due to:\n    • Interference from nearby heavy machinery\n    • Rock or tile obstructing the dish's line of sight on space";
			}

			public class NETWORKQUALITY
			{
				public static LocString NAME = "Scan Network Quality: {TotalQuality}";

				public static LocString TOOLTIP = string.Concat("This scanner network is scanning at <b>{TotalQuality}</b> effectiveness\n\nIt will detect incoming objects <b>{WorstTime}</b> to <b>{BestTime}</b> before they arrive\n\nBuild multiple ", BUILDINGS.PREFABS.COMETDETECTOR.NAME, "s and ensure they're each scanning effectively for the best detection results");
			}

			public class DETECTORSCANNING
			{
				public static LocString NAME = "Scanning";

				public static LocString TOOLTIP = "This scanner is currently scouring space for anything of interest";
			}

			public class INCOMINGMETEORS
			{
				public static LocString NAME = "Incoming Object Detected";

				public static LocString TOOLTIP = "Warning!\n\nHigh velocity objects on approach!";
			}

			public class SPACE_VISIBILITY_NONE
			{
				public static LocString NAME = "No Line of Sight";

				public static LocString TOOLTIP = "This building has no view of space\n\nEnsure an unblocked view of the sky is available to collect " + UI.FormatAsManagementMenu("Starmap") + " data\n    • Visibility: <b>{VISIBILITY}</b>\n    • Scan Radius: <b>{RADIUS}</b> cells";
			}

			public class SPACE_VISIBILITY_REDUCED
			{
				public static LocString NAME = "Reduced Visibility";

				public static LocString TOOLTIP = "This building has an inadequate or obscured view of space\n\nEnsure an unblocked view of the sky is available to collect " + UI.FormatAsManagementMenu("Starmap") + " data\n    • Visibility: <b>{VISIBILITY}</b>\n    • Scan Radius: <b>{RADIUS}</b> cells";
			}

			public class LANDEDROCKETLACKSPASSENGERMODULE
			{
				public static LocString NAME = "Rocket lacks spacefarer module";

				public static LocString TOOLTIP = "A rocket must have a spacefarer module";
			}

			public class PATH_NOT_CLEAR
			{
				public static LocString NAME = "Launch Path Blocked";

				public static LocString TOOLTIP = "There are obstructions in the launch trajectory of this rocket:\n    • {0}\n\nThis rocket requires a clear flight path for launch";

				public static LocString TILE_FORMAT = "Solid {0}";
			}

			public class RAILGUN_PATH_NOT_CLEAR
			{
				public static LocString NAME = "Launch Path Blocked";

				public static LocString TOOLTIP = "There are obstructions in the launch trajectory of this " + UI.FormatAsLink("Interplanetary Launcher", "RAILGUN") + ":\n    • {0}\n\nThis launcher requires a clear path to launch payloads";
			}

			public class RAILGUN_NO_DESTINATION
			{
				public static LocString NAME = "No Delivery Destination";

				public static LocString TOOLTIP = "A delivery destination has not been set    • {0}";
			}

			public class NOSURFACESIGHT
			{
				public static LocString NAME = "No Line of Sight";

				public static LocString TOOLTIP = "This building has no view of space\n\nTo properly function, this building requires an unblocked view of space";
			}

			public class ROCKETRESTRICTIONACTIVE
			{
				public static LocString NAME = "Access: Restricted";

				public static LocString TOOLTIP = "This building cannot be operated while restricted, though it can be filled\n\nControlled by a " + BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME;
			}

			public class ROCKETRESTRICTIONINACTIVE
			{
				public static LocString NAME = "Access: Not Restricted";

				public static LocString TOOLTIP = "This building's operation is not restricted\n\nControlled by a " + BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME;
			}

			public class NOROCKETRESTRICTION
			{
				public static LocString NAME = "Not Controlled";

				public static LocString TOOLTIP = "This building is not controlled by a " + BUILDINGS.PREFABS.ROCKETCONTROLSTATION.NAME;
			}

			public class TOP_PRIORITY_CHORE
			{
				public static LocString NAME = "Top Priority";

				public static LocString TOOLTIP = "This errand has been set to " + UI.PRE_KEYWORD + "Top Priority" + UI.PST_KEYWORD + UI.HORIZONTAL_BR_RULE + "The colony will be in " + UI.PRE_KEYWORD + "Yellow Alert" + UI.PST_KEYWORD + " until this task is completed";

				public static LocString NOTIFICATION_NAME = "Yellow Alert";

				public static LocString NOTIFICATION_TOOLTIP = "The following errands have been set to " + UI.PRE_KEYWORD + "Top Priority" + UI.PST_KEYWORD + ":";
			}

			public class HOTTUBWATERTOOCOLD
			{
				public static LocString NAME = "Water Too Cold";

				public static LocString TOOLTIP = "This tub's " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + " is below <b>{temperature}</b>\n\nIt is draining so it can be refilled with warmer " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD;
			}

			public class HOTTUBTOOHOT
			{
				public static LocString NAME = "Building Too Hot";

				public static LocString TOOLTIP = "This tub's " + UI.PRE_KEYWORD + "Temperature" + UI.PST_KEYWORD + " is above <b>{temperature}</b>\n\nIt needs to cool before it can safely be used";
			}

			public class HOTTUBFILLING
			{
				public static LocString NAME = "Filling Up: ({fullness})";

				public static LocString TOOLTIP = "This tub is currently filling with " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + "\n\nIt will be available to use when the " + UI.PRE_KEYWORD + "Water" + UI.PST_KEYWORD + " level reaches <b>100%</b>";
			}

			public class WINDTUNNELINTAKE
			{
				public static LocString NAME = "Intake Requires Gas";

				public static LocString TOOLTIP = "A wind tunnel requires " + UI.PRE_KEYWORD + "Gas" + UI.PST_KEYWORD + " at the top and bottom intakes in order to operate\n\nThe intakes for this wind tunnel don't have enough gas to operate";
			}

			public class TEMPORAL_TEAR_OPENER_NO_TARGET
			{
				public static LocString NAME = "Temporal Tear not revealed";

				public static LocString TOOLTIP = "This machine is meant to target something in space, but the target has not yet been revealed";
			}

			public class TEMPORAL_TEAR_OPENER_NO_LOS
			{
				public static LocString NAME = "Line of Sight: Obstructed";

				public static LocString TOOLTIP = "This device needs a clear view of space to operate";
			}

			public class TEMPORAL_TEAR_OPENER_INSUFFICIENT_COLONIES
			{
				public static LocString NAME = "Too few Printing Pods {progress}";

				public static LocString TOOLTIP = "To open the Temporal Tear, this device relies on a network of activated Printing Pods {progress}";
			}

			public class TEMPORAL_TEAR_OPENER_PROGRESS
			{
				public static LocString NAME = "Charging Progress: {progress}";

				public static LocString TOOLTIP = "This device must be charged with a high number of Radbolts\n\nOperation can commence once this device is fully charged";
			}

			public class TEMPORAL_TEAR_OPENER_READY
			{
				public static LocString NOTIFICATION = "Temporal Tear Opener fully charged";

				public static LocString NOTIFICATION_TOOLTIP = "Push the red button to activate";
			}

			public class WARPPORTALCHARGING
			{
				public static LocString NAME = "Recharging: {charge}";

				public static LocString TOOLTIP = "This teleporter will be ready for use in {cycles} cycles";
			}

			public class WARPCONDUITPARTNERDISABLED
			{
				public static LocString NAME = "Teleporter Disabled ({x}/2)";

				public static LocString TOOLTIP = "This teleporter cannot be used until both the transmitting and receiving sides have been activated";
			}

			public class COLLECTINGHEP
			{
				public static LocString NAME = "Collecting Radbolts ({x}/cycle)";

				public static LocString TOOLTIP = "Collecting Radbolts from ambient radiation";
			}

			public class INORBIT
			{
				public static LocString NAME = "In Orbit: {Destination}";

				public static LocString TOOLTIP = "This rocket is currently in orbit around {Destination}";
			}

			public class INFLIGHT
			{
				public static LocString NAME = "In Flight To {Destination_Asteroid}: {ETA}";

				public static LocString TOOLTIP = "This rocket is currently traveling to {Destination_Pad} on {Destination_Asteroid}\n\nIt will arrive in {ETA}";

				public static LocString TOOLTIP_NO_PAD = "This rocket is currently traveling to {Destination_Asteroid}\n\nIt will arrive in {ETA}";
			}

			public class DESTINATIONOUTOFRANGE
			{
				public static LocString NAME = "Destination Out Of Range";

				public static LocString TOOLTIP = "This rocket lacks the range to reach its destination" + UI.HORIZONTAL_BR_RULE + "Rocket Range: {Range}\nDestination Distance: {Distance}";
			}

			public class ROCKETSTRANDED
			{
				public static LocString NAME = "Stranded";

				public static LocString TOOLTIP = "This rocket has run out of fuel and cannot move";
			}

			public class SPACEPOIHARVESTING
			{
				public static LocString NAME = "Extracting Resources: {0}";

				public static LocString TOOLTIP = "Resources are being mined from this space debris";
			}

			public class SPACEPOIWASTING
			{
				public static LocString NAME = "Cannot store resources: {0}";

				public static LocString TOOLTIP = "Some resources being mined from this space debris cannot be stored in this rocket";
			}

			public class RAILGUNPAYLOADNEEDSEMPTYING
			{
				public static LocString NAME = "Ready To Unpack";

				public static LocString TOOLTIP = "This payload has reached its destination and is ready to be unloaded\n\nIt can be marked for unpacking manually, or automatically unpacked on arrival using a " + BUILDINGS.PREFABS.RAILGUNPAYLOADOPENER.NAME;
			}

			public class AWAITINGEMPTYBUILDING
			{
				public static LocString NAME = "Empty Errand";

				public static LocString TOOLTIP = "Building will be emptied once a Duplicant is available";
			}

			public class DUPLICANTACTIVATIONREQUIRED
			{
				public static LocString NAME = "Activation Required";

				public static LocString TOOLTIP = "A Duplicant is required to bring this building online";
			}

			public class PILOTNEEDED
			{
				public static LocString NAME = "Switching to Autopilot";

				public static LocString TOOLTIP = "Autopilot will engage in {timeRemaining} if a Duplicant pilot does not assume control";
			}

			public class AUTOPILOTACTIVE
			{
				public static LocString NAME = "Autopilot Engaged";

				public static LocString TOOLTIP = "This rocket has entered autopilot mode and will fly at reduced speed\n\nIt can resume full speed once a Duplicant pilot takes over";
			}

			public class ROCKETCHECKLISTINCOMPLETE
			{
				public static LocString NAME = "Launch Checklist Incomplete";

				public static LocString TOOLTIP = "Critical launch tasks uncompleted\n\nRefer to the Launch Checklist in the status panel";
			}

			public class ROCKETCARGOEMPTYING
			{
				public static LocString NAME = "Unloading Cargo";

				public static LocString TOOLTIP = "Rocket cargo is being unloaded into the " + UI.PRE_KEYWORD + "Rocket Platform" + UI.PST_KEYWORD + "\n\nLoading of new cargo will begin once unloading is complete";
			}

			public class ROCKETCARGOFILLING
			{
				public static LocString NAME = "Loading Cargo";

				public static LocString TOOLTIP = "Cargo is being loaded onto the rocket from the " + UI.PRE_KEYWORD + "Rocket Platform" + UI.PST_KEYWORD + "\n\nRocket cargo will be ready for launch once loading is complete";
			}

			public class ROCKETCARGOFULL
			{
				public static LocString NAME = "Platform Ready";

				public static LocString TOOLTIP = "All cargo operations are complete";
			}

			public class FLIGHTALLCARGOFULL
			{
				public static LocString NAME = "All cargo bays are full";

				public static LocString TOOLTIP = "Rocket cannot store any more materials";
			}

			public class FLIGHTCARGOREMAINING
			{
				public static LocString NAME = "Cargo capacity remaining: {0}";

				public static LocString TOOLTIP = "Rocket can store up to {0} more materials";
			}

			public class ROCKET_PORT_IDLE
			{
				public static LocString NAME = "Idle";

				public static LocString TOOLTIP = "This port is idle because there is no rocket on the connected " + UI.PRE_KEYWORD + "Rocket Platform" + UI.PST_KEYWORD;
			}

			public class ROCKET_PORT_UNLOADING
			{
				public static LocString NAME = "Unloading Rocket";

				public static LocString TOOLTIP = "Resources are being unloaded from the rocket into the local network";
			}

			public class ROCKET_PORT_LOADING
			{
				public static LocString NAME = "Loading Rocket";

				public static LocString TOOLTIP = "Resources are being loaded from the local network into the rocket's storage";
			}

			public class ROCKET_PORT_LOADED
			{
				public static LocString NAME = "Cargo Transfer Complete";

				public static LocString TOOLTIP = "The connected rocket has either reached max capacity for this resource type, or lacks appropriate storage modules";
			}

			public class CONNECTED_ROCKET_PORT
			{
				public static LocString NAME = "Port Network Attached";

				public static LocString TOOLTIP = string.Concat("This module has been connected to a ", BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME, " and can now load and unload cargo");
			}

			public class CONNECTED_ROCKET_WRONG_PORT
			{
				public static LocString NAME = "Incorrect Port Network";

				public static LocString TOOLTIP = string.Concat("The attached ", BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME, " is not the correct type for this cargo module");
			}

			public class CONNECTED_ROCKET_NO_PORT
			{
				public static LocString NAME = "No Rocket Ports";

				public static LocString TOOLTIP = string.Concat("This ", UI.PRE_KEYWORD, "Rocket Platform", UI.PST_KEYWORD, " has no ", BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME, " attached", UI.HORIZONTAL_BR_RULE, UI.PRE_KEYWORD, "Solid", UI.PST_KEYWORD, ", ", UI.PRE_KEYWORD, "Gas", UI.PST_KEYWORD, ", and ", UI.PRE_KEYWORD, "Liquid", UI.PST_KEYWORD, " ", BUILDINGS.PREFABS.MODULARLAUNCHPADPORT.NAME_PLURAL, " can be attached to load and unload cargo from a landed rocket's modules");
			}

			public class CLUSTERTELESCOPEALLWORKCOMPLETE
			{
				public static LocString NAME = "Area Complete";

				public static LocString TOOLTIP = "This " + UI.PRE_KEYWORD + "Telescope" + UI.PST_KEYWORD + " has analyzed all the space visible from its current location";
			}

			public class ROCKETPLATFORMCLOSETOCEILING
			{
				public static LocString NAME = "Low Clearance: {distance} Tiles";

				public static LocString TOOLTIP = "Tall rockets may not be able to land on this " + UI.PRE_KEYWORD + "Rocket Platform" + UI.PST_KEYWORD;
			}

			public class MODULEGENERATORNOTPOWERED
			{
				public static LocString NAME = "Thrust Generation: {ActiveWattage}/{MaxWattage}";

				public static LocString TOOLTIP = "Engine will generate " + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " once traveling through space\n\nRight now, it's not doing much of anything";
			}

			public class MODULEGENERATORPOWERED
			{
				public static LocString NAME = "Thrust Generation: {ActiveWattage}/{MaxWattage}";

				public static LocString TOOLTIP = "Engine is extracting " + UI.FormatAsPositiveRate("{MaxWattage}") + " of " + UI.PRE_KEYWORD + "Power" + UI.PST_KEYWORD + " from the thruster\n\nIt will continue generating power as long as it travels through space";
			}

			public class INORBITREQUIRED
			{
				public static LocString NAME = "Grounded";

				public static LocString TOOLTIP = string.Concat("This building cannot operate from the surface of a ", UI.CLUSTERMAP.PLANETOID_KEYWORD, " and must be in space to function");
			}

			public class REACTORREFUELDISABLED
			{
				public static LocString NAME = "Refuel Disabled";

				public static LocString TOOLTIP = "This building will not be refueled once its active fuel has been consumed";
			}

			public class RAILGUNCOOLDOWN
			{
				public static LocString NAME = "Cleaning Rails: {timeleft}";

				public static LocString TOOLTIP = "This building automatically performs routine maintenance every {x} launches";
			}

			public class FRIDGECOOLING
			{
				public static LocString NAME = "Cooling Contents: {UsedPower}";

				public static LocString TOOLTIP = "{UsedPower} of {MaxPower} are being used to cool the contents of this food storage";
			}

			public class FRIDGESTEADY
			{
				public static LocString NAME = "Energy Saver: {UsedPower}";

				public static LocString TOOLTIP = "The contents of this food storage are at refrigeration temperatures\n\nEnergy Saver mode has been automatically activated using only {UsedPower} of {MaxPower}";
			}
		}

		public class DETAILS
		{
			public static LocString USE_COUNT = "Uses: {0}";

			public static LocString USE_COUNT_TOOLTIP = "This building has been used {0} times";
		}
	}
}
