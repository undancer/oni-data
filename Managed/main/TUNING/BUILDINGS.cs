using System;
using System.Collections.Generic;

namespace TUNING
{
	public class BUILDINGS
	{
		public class PHARMACY
		{
			public class FABRICATIONTIME
			{
				public const float TIER0 = 50f;

				public const float TIER1 = 100f;

				public const float TIER2 = 200f;
			}
		}

		public class NUCLEAR_REACTOR
		{
			public class REACTOR_MASSES
			{
				public const float MIN = 1f;

				public const float MAX = 10f;
			}
		}

		public class OVERPRESSURE
		{
			public const float TIER0 = 1.8f;
		}

		public class OVERHEAT_TEMPERATURES
		{
			public const float LOW_3 = 10f;

			public const float LOW_2 = 328.15f;

			public const float LOW_1 = 338.15f;

			public const float NORMAL = 348.15f;

			public const float HIGH_1 = 363.15f;

			public const float HIGH_2 = 398.15f;

			public const float HIGH_3 = 1273.15f;

			public const float HIGH_4 = 2273.15f;
		}

		public class OVERHEAT_MATERIAL_MOD
		{
			public const float LOW_3 = -200f;

			public const float LOW_2 = -20f;

			public const float LOW_1 = -10f;

			public const float NORMAL = 0f;

			public const float HIGH_1 = 15f;

			public const float HIGH_2 = 50f;

			public const float HIGH_3 = 200f;

			public const float HIGH_4 = 500f;

			public const float HIGH_5 = 900f;
		}

		public class DECOR_MATERIAL_MOD
		{
			public const float NORMAL = 0f;

			public const float HIGH_1 = 0.1f;

			public const float HIGH_2 = 0.2f;

			public const float HIGH_3 = 0.5f;

			public const float HIGH_4 = 1f;
		}

		public class CONSTRUCTION_MASS_KG
		{
			public static readonly float[] TIER_TINY = new float[1] { 5f };

			public static readonly float[] TIER0 = new float[1] { 25f };

			public static readonly float[] TIER1 = new float[1] { 50f };

			public static readonly float[] TIER2 = new float[1] { 100f };

			public static readonly float[] TIER3 = new float[1] { 200f };

			public static readonly float[] TIER4 = new float[1] { 400f };

			public static readonly float[] TIER5 = new float[1] { 800f };

			public static readonly float[] TIER6 = new float[1] { 1200f };

			public static readonly float[] TIER7 = new float[1] { 2000f };
		}

		public class ROCKETRY_MASS_KG
		{
			public static float[] COMMAND_MODULE_MASS = new float[1] { 200f };

			public static float[] CARGO_MASS = new float[2] { 1000f, 1000f };

			public static float[] CARGO_MASS_SMALL = new float[2] { 400f, 400f };

			public static float[] FUEL_TANK_DRY_MASS = new float[1] { 100f };

			public static float[] FUEL_TANK_WET_MASS = new float[1] { 900f };

			public static float[] FUEL_TANK_WET_MASS_SMALL = new float[1] { 300f };

			public static float[] FUEL_TANK_WET_MASS_GAS = new float[1] { 100f };

			public static float[] FUEL_TANK_WET_MASS_GAS_LARGE = new float[1] { 150f };

			public static float[] OXIDIZER_TANK_OXIDIZER_MASS = new float[1] { 900f };

			public static float[] ENGINE_MASS_SMALL = new float[1] { 200f };

			public static float[] ENGINE_MASS_LARGE = new float[1] { 500f };

			public static float[] HOLLOW_TIER1 = new float[2] { 200f, 100f };

			public static float[] HOLLOW_TIER2 = new float[2] { 400f, 200f };

			public static float[] HOLLOW_TIER3 = new float[2] { 800f, 400f };

			public static float[] DENSE_TIER0 = new float[1] { 200f };

			public static float[] DENSE_TIER1 = new float[1] { 500f };

			public static float[] DENSE_TIER2 = new float[1] { 1000f };

			public static float[] DENSE_TIER3 = new float[1] { 2000f };
		}

		public class ENERGY_CONSUMPTION_WHEN_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 5f;

			public const float TIER2 = 60f;

			public const float TIER3 = 120f;

			public const float TIER4 = 240f;

			public const float TIER5 = 480f;

			public const float TIER6 = 960f;

			public const float TIER7 = 1200f;

			public const float TIER8 = 1600f;
		}

		public class EXHAUST_ENERGY_ACTIVE
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.125f;

			public const float TIER2 = 0.25f;

			public const float TIER3 = 0.5f;

			public const float TIER4 = 1f;

			public const float TIER5 = 2f;

			public const float TIER6 = 4f;

			public const float TIER7 = 8f;

			public const float TIER8 = 16f;
		}

		public class JOULES_LEAK_PER_CYCLE
		{
			public const float TIER0 = 400f;

			public const float TIER1 = 1000f;

			public const float TIER2 = 2000f;
		}

		public class SELF_HEAT_KILOWATTS
		{
			public const float TIER0 = 0f;

			public const float TIER1 = 0.5f;

			public const float TIER2 = 1f;

			public const float TIER3 = 2f;

			public const float TIER4 = 4f;

			public const float TIER5 = 8f;

			public const float TIER6 = 16f;

			public const float TIER7 = 32f;

			public const float TIER8 = 64f;

			public const float TIER_NUCLEAR = 16384f;
		}

		public class MELTING_POINT_KELVIN
		{
			public const float TIER0 = 800f;

			public const float TIER1 = 1600f;

			public const float TIER2 = 2400f;

			public const float TIER3 = 3200f;

			public const float TIER4 = 9999f;
		}

		public class CONSTRUCTION_TIME_SECONDS
		{
			public const float TIER0 = 3f;

			public const float TIER1 = 10f;

			public const float TIER2 = 30f;

			public const float TIER3 = 60f;

			public const float TIER4 = 120f;

			public const float TIER5 = 240f;

			public const float TIER6 = 480f;
		}

		public class HITPOINTS
		{
			public const int TIER0 = 10;

			public const int TIER1 = 30;

			public const int TIER2 = 100;

			public const int TIER3 = 250;

			public const int TIER4 = 1000;
		}

		public class DAMAGE_SOURCES
		{
			public const int CONDUIT_CONTENTS_BOILED = 1;

			public const int CONDUIT_CONTENTS_FROZE = 1;

			public const int BAD_INPUT_ELEMENT = 1;

			public const int BUILDING_OVERHEATED = 1;

			public const int HIGH_LIQUID_PRESSURE = 10;

			public const int MICROMETEORITE = 1;

			public const int CORROSIVE_ELEMENT = 1;
		}

		public class RELOCATION_TIME_SECONDS
		{
			public const float DECONSTRUCT = 4f;

			public const float CONSTRUCT = 4f;
		}

		public class WORK_TIME_SECONDS
		{
			public const float VERYSHORT_WORK_TIME = 5f;

			public const float SHORT_WORK_TIME = 15f;

			public const float MEDIUM_WORK_TIME = 30f;

			public const float LONG_WORK_TIME = 90f;

			public const float VERY_LONG_WORK_TIME = 150f;

			public const float EXTENSIVE_WORK_TIME = 180f;
		}

		public class FABRICATION_TIME_SECONDS
		{
			public const float VERY_SHORT = 20f;

			public const float SHORT = 40f;

			public const float MODERATE = 80f;

			public const float LONG = 250f;
		}

		public class DECOR
		{
			public class BONUS
			{
				public class MONUMENT
				{
					public static readonly EffectorValues COMPLETE;

					public static readonly EffectorValues INCOMPLETE;

					static MONUMENT()
					{
						EffectorValues effectorValues = new EffectorValues
						{
							amount = 40,
							radius = 10
						};
						COMPLETE = effectorValues;
						effectorValues = new EffectorValues
						{
							amount = 10,
							radius = 5
						};
						INCOMPLETE = effectorValues;
					}
				}

				public static readonly EffectorValues TIER0;

				public static readonly EffectorValues TIER1;

				public static readonly EffectorValues TIER2;

				public static readonly EffectorValues TIER3;

				public static readonly EffectorValues TIER4;

				public static readonly EffectorValues TIER5;

				static BONUS()
				{
					EffectorValues effectorValues = new EffectorValues
					{
						amount = 5,
						radius = 1
					};
					TIER0 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = 10,
						radius = 2
					};
					TIER1 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = 15,
						radius = 3
					};
					TIER2 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = 20,
						radius = 4
					};
					TIER3 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = 25,
						radius = 5
					};
					TIER4 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = 30,
						radius = 6
					};
					TIER5 = effectorValues;
				}
			}

			public class PENALTY
			{
				public static readonly EffectorValues TIER0;

				public static readonly EffectorValues TIER1;

				public static readonly EffectorValues TIER2;

				public static readonly EffectorValues TIER3;

				public static readonly EffectorValues TIER4;

				public static readonly EffectorValues TIER5;

				static PENALTY()
				{
					EffectorValues effectorValues = new EffectorValues
					{
						amount = -5,
						radius = 1
					};
					TIER0 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = -10,
						radius = 2
					};
					TIER1 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = -15,
						radius = 3
					};
					TIER2 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = -20,
						radius = 4
					};
					TIER3 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = -20,
						radius = 5
					};
					TIER4 = effectorValues;
					effectorValues = new EffectorValues
					{
						amount = -25,
						radius = 6
					};
					TIER5 = effectorValues;
				}
			}

			public static readonly EffectorValues NONE = new EffectorValues
			{
				amount = 0,
				radius = 1
			};
		}

		public class MASS_KG
		{
			public const float TIER0 = 25f;

			public const float TIER1 = 50f;

			public const float TIER2 = 100f;

			public const float TIER3 = 200f;

			public const float TIER4 = 400f;

			public const float TIER5 = 800f;

			public const float TIER6 = 1200f;

			public const float TIER7 = 2000f;
		}

		public class UPGRADES
		{
			public class MATERIALTAGS
			{
				public const string METAL = "Metal";

				public const string REFINEDMETAL = "RefinedMetal";

				public const string CARBON = "Carbon";
			}

			public class MATERIALMASS
			{
				public const int TIER0 = 100;

				public const int TIER1 = 200;

				public const int TIER2 = 400;

				public const int TIER3 = 500;
			}

			public class MODIFIERAMOUNTS
			{
				public const float MANUALGENERATOR_ENERGYGENERATION = 1.2f;

				public const float MANUALGENERATOR_CAPACITY = 2f;

				public const float PROPANEGENERATOR_ENERGYGENERATION = 1.6f;

				public const float PROPANEGENERATOR_HEATGENERATION = 1.6f;

				public const float GENERATOR_HEATGENERATION = 0.8f;

				public const float GENERATOR_ENERGYGENERATION = 1.3f;

				public const float TURBINE_ENERGYGENERATION = 1.2f;

				public const float TURBINE_CAPACITY = 1.2f;

				public const float SUITRECHARGER_EXECUTIONTIME = 1.2f;

				public const float SUITRECHARGER_HEATGENERATION = 1.2f;

				public const float STORAGELOCKER_CAPACITY = 2f;

				public const float SOLARPANEL_ENERGYGENERATION = 1.2f;

				public const float SMELTER_HEATGENERATION = 0.7f;
			}

			public const float BUILDTIME_TIER0 = 120f;
		}

		public const float DEFAULT_STORAGE_CAPACITY = 2000f;

		public const float STANDARD_MANUAL_REFILL_LEVEL = 0.2f;

		public const float MASS_TEMPERATURE_SCALE = 0.2f;

		public const float AIRCONDITIONER_TEMPDELTA = -14f;

		public const float MAX_ENVIRONMENT_DELTA = -50f;

		public const float COMPOST_FLIP_TIME = 20f;

		public const int TUBE_LAUNCHER_MAX_CHARGES = 3;

		public const float TUBE_LAUNCHER_RECHARGE_TIME = 10f;

		public const float TUBE_LAUNCHER_WORK_TIME = 1f;

		public const float SMELTER_INGOT_INPUTKG = 500f;

		public const float SMELTER_INGOT_OUTPUTKG = 100f;

		public const float SMELTER_FABRICATIONTIME = 120f;

		public const float GEOREFINERY_SLAB_INPUTKG = 1000f;

		public const float GEOREFINERY_SLAB_OUTPUTKG = 200f;

		public const float GEOREFINERY_FABRICATIONTIME = 120f;

		public const float MASS_BURN_RATE_HYDROGENGENERATOR = 0.1f;

		public const float COOKER_FOOD_TEMPERATURE = 368.15f;

		public const float OVERHEAT_DAMAGE_INTERVAL = 7.5f;

		public const float MIN_BUILD_TEMPERATURE = 288.15f;

		public const float MAX_BUILD_TEMPERATURE = 318.15f;

		public const float MELTDOWN_TEMPERATURE = 533.15f;

		public const float REPAIR_FORCE_TEMPERATURE = 293.15f;

		public const int REPAIR_EFFECTIVENESS_BASE = 10;

		public static List<PlanScreen.PlanInfo> PLANORDER = new List<PlanScreen.PlanInfo>
		{
			new PlanScreen.PlanInfo(new HashedString("Base"), hideIfNotResearched: false, new List<string>
			{
				"Ladder", "FirePole", "LadderFast", "Tile", "GasPermeableMembrane", "MeshTile", "InsulationTile", "PlasticTile", "MetalTile", "GlassTile",
				"BunkerTile", "CarpetTile", "ExobaseHeadquarters", "Door", "ManualPressureDoor", "PressureDoor", "BunkerDoor", "StorageLocker", "StorageLockerSmart", "LiquidReservoir",
				"GasReservoir", "ObjectDispenser", "TravelTube", "TravelTubeEntrance", "TravelTubeWallBridge"
			}),
			new PlanScreen.PlanInfo(new HashedString("Oxygen"), hideIfNotResearched: false, new List<string> { "MineralDeoxidizer", "SublimationStation", "AlgaeHabitat", "AirFilter", "CO2Scrubber", "Electrolyzer", "RustDeoxidizer" }),
			new PlanScreen.PlanInfo(new HashedString("Power"), hideIfNotResearched: false, new List<string>
			{
				"DevGenerator",
				"ManualGenerator",
				"Generator",
				"WoodGasGenerator",
				"HydrogenGenerator",
				"MethaneGenerator",
				"PetroleumGenerator",
				"SteamTurbine",
				"SteamTurbine2",
				"SolarPanel",
				"Wire",
				"WireBridge",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"Battery",
				"BatteryMedium",
				"BatterySmart",
				"PowerTransformerSmall",
				"PowerTransformer",
				SwitchConfig.ID,
				LogicPowerRelayConfig.ID,
				TemperatureControlledSwitchConfig.ID,
				PressureSwitchLiquidConfig.ID,
				PressureSwitchGasConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("Food"), hideIfNotResearched: false, new List<string>
			{
				"MicrobeMusher", "CookingStation", "GourmetCookingStation", "PlanterBox", "FarmTile", "HydroponicFarm", "RationBox", "Refrigerator", "CreatureDeliveryPoint", "FishDeliveryPoint",
				"CreatureFeeder", "FishFeeder", "EggIncubator", "EggCracker", "CreatureTrap", "FishTrap", "AirborneCreatureLure", "FlyingCreatureBait"
			}),
			new PlanScreen.PlanInfo(new HashedString("Plumbing"), hideIfNotResearched: false, new List<string>
			{
				"Outhouse",
				"FlushToilet",
				"WallToilet",
				ShowerConfig.ID,
				"LiquidPumpingStation",
				"BottleEmptier",
				"LiquidConduit",
				"InsulatedLiquidConduit",
				"LiquidConduitRadiant",
				"LiquidConduitBridge",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidPump",
				"LiquidMiniPump",
				"LiquidVent",
				"LiquidFilter",
				"LiquidValve",
				"LiquidLogicValve",
				"LiquidLimitValve",
				LiquidConduitElementSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LiquidConduitTemperatureSensorConfig.ID,
				"ModularLaunchpadPortLiquid",
				"ModularLaunchpadPortLiquidUnloader"
			}),
			new PlanScreen.PlanInfo(new HashedString("HVAC"), hideIfNotResearched: false, new List<string>
			{
				"GasConduit",
				"InsulatedGasConduit",
				"GasConduitRadiant",
				"GasConduitBridge",
				"GasConduitPreferentialFlow",
				"GasConduitOverflow",
				"GasPump",
				"GasMiniPump",
				"GasVent",
				"GasVentHighPressure",
				"GasFilter",
				"GasValve",
				"GasLogicValve",
				"GasLimitValve",
				"GasBottler",
				"BottleEmptierGas",
				"ModularLaunchpadPortGas",
				"ModularLaunchpadPortGasUnloader",
				GasConduitElementSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID
			}),
			new PlanScreen.PlanInfo(new HashedString("Refining"), hideIfNotResearched: false, new List<string>
			{
				"Compost", "WaterPurifier", "Desalinator", "FertilizerMaker", "AlgaeDistillery", "EthanolDistillery", "RockCrusher", "Kiln", "SludgePress", "MetalRefinery",
				"GlassForge", "OilRefinery", "Polymerizer", "OxyliteRefinery", "SupermaterialRefinery", "DiamondPress"
			}),
			new PlanScreen.PlanInfo(new HashedString("Medical"), hideIfNotResearched: false, new List<string> { "WashBasin", "WashSink", "HandSanitizer", "DecontaminationShower", "Apothecary", "DoctorStation", "AdvancedDoctorStation", "MedicalCot", "MassageTable", "Grave" }),
			new PlanScreen.PlanInfo(new HashedString("Furniture"), hideIfNotResearched: false, new List<string>
			{
				BedConfig.ID,
				LuxuryBedConfig.ID,
				LadderBedConfig.ID,
				"FloorLamp",
				"CeilingLight",
				"SunLamp",
				"DiningTable",
				"WaterCooler",
				"Phonobox",
				"ArcadeMachine",
				"EspressoMachine",
				"HotTub",
				"MechanicalSurfboard",
				"Sauna",
				"Juicer",
				"SodaFountain",
				"BeachChair",
				"VerticalWindTunnel",
				PixelPackConfig.ID,
				"Telephone",
				"FlowerVase",
				"FlowerVaseWall",
				"FlowerVaseHanging",
				"FlowerVaseHangingFancy",
				"SmallSculpture",
				"Sculpture",
				"IceSculpture",
				"MarbleSculpture",
				"MetalSculpture",
				"CrownMoulding",
				"CornerMoulding",
				"Canvas",
				"CanvasWide",
				"CanvasTall",
				"ItemPedestal",
				"MonumentBottom",
				"MonumentMiddle",
				"MonumentTop",
				"ParkSign"
			}),
			new PlanScreen.PlanInfo(new HashedString("Equipment"), hideIfNotResearched: false, new List<string>
			{
				"ResearchCenter", "AdvancedResearchCenter", "NuclearResearchCenter", "OrbitalResearchCenter", "CosmicResearchCenter", "Telescope", "PowerControlStation", "FarmStation", "GeneticAnalysisStation", "RanchStation",
				"ShearingStation", "RoleStation", "ResetSkillsStation", "ArtifactAnalysisStation", "CraftingTable", "ClothingFabricator", "SuitFabricator", "OxygenMaskMarker", "OxygenMaskLocker", "SuitMarker",
				"SuitLocker", "JetSuitMarker", "JetSuitLocker", "LeadSuitMarker", "LeadSuitLocker", "AstronautTrainingCenter"
			}),
			new PlanScreen.PlanInfo(new HashedString("Utilities"), hideIfNotResearched: true, new List<string>
			{
				"SpaceHeater", "LiquidHeater", "LiquidCooledFan", "IceCooledFan", "IceMachine", "AirConditioner", "LiquidConditioner", "OreScrubber", "OilWellCap", "ThermalBlock",
				"ExteriorWall", "SweepBotStation"
			}),
			new PlanScreen.PlanInfo(new HashedString("Automation"), hideIfNotResearched: true, new List<string>
			{
				"LogicWire",
				"LogicWireBridge",
				"LogicRibbon",
				"LogicRibbonBridge",
				LogicSwitchConfig.ID,
				"LogicDuplicantSensor",
				LogicPressureSensorGasConfig.ID,
				LogicPressureSensorLiquidConfig.ID,
				LogicTemperatureSensorConfig.ID,
				LogicWattageSensorConfig.ID,
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID,
				LogicDiseaseSensorConfig.ID,
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				LogicCritterCountSensorConfig.ID,
				LogicRadiationSensorConfig.ID,
				LogicHEPSensorConfig.ID,
				LogicCounterConfig.ID,
				LogicAlarmConfig.ID,
				LogicHammerConfig.ID,
				"LogicInterasteroidSender",
				"LogicInterasteroidReceiver",
				LogicRibbonReaderConfig.ID,
				LogicRibbonWriterConfig.ID,
				"FloorSwitch",
				"Checkpoint",
				CometDetectorConfig.ID,
				"LogicGateNOT",
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateBUFFER",
				"LogicGateFILTER",
				"LogicGateXOR",
				LogicMemoryConfig.ID,
				"LogicGateMultiplexer",
				"LogicGateDemultiplexer"
			}),
			new PlanScreen.PlanInfo(new HashedString("Conveyance"), hideIfNotResearched: true, new List<string>
			{
				"SolidTransferArm",
				"SolidConduit",
				"SolidConduitBridge",
				"SolidConduitInbox",
				"SolidConduitOutbox",
				"SolidFilter",
				"SolidVent",
				"SolidLogicValve",
				"SolidLimitValve",
				SolidConduitDiseaseSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitTemperatureSensorConfig.ID,
				"AutoMiner",
				"ModularLaunchpadPortSolid",
				"ModularLaunchpadPortSolidUnloader"
			}),
			new PlanScreen.PlanInfo(new HashedString("Rocketry"), hideIfNotResearched: true, new List<string>
			{
				"ClusterTelescope",
				"LaunchPad",
				"Gantry",
				"SteamEngine",
				"KeroseneEngine",
				"SolidBooster",
				"LiquidFuelTank",
				"OxidizerTank",
				"OxidizerTankLiquid",
				"CargoBay",
				"GasCargoBay",
				"LiquidCargoBay",
				"CommandModule",
				"TouristModule",
				"ResearchModule",
				"SpecialCargoBay",
				"HydrogenEngine",
				RocketControlStationConfig.ID,
				"RocketInteriorPowerPlug",
				"RocketInteriorLiquidInput",
				"RocketInteriorLiquidOutput",
				"RocketInteriorGasInput",
				"RocketInteriorGasOutput",
				"RocketInteriorSolidInput",
				"RocketInteriorSolidOutput",
				LogicClusterLocationSensorConfig.ID,
				"RailGun",
				"RailGunPayloadOpener",
				"LandingBeacon"
			}),
			new PlanScreen.PlanInfo(new HashedString("HEP"), hideIfNotResearched: true, new List<string> { "NuclearReactor", "UraniumCentrifuge", "HighEnergyParticleSpawner", "HighEnergyParticleRedirector", "HEPBridgeTile", "DevRadiationGenerator" }, "EXPANSION1_ID")
		};

		public static List<Type> COMPONENT_DESCRIPTION_ORDER = new List<Type>
		{
			typeof(BottleEmptier),
			typeof(CookingStation),
			typeof(GourmetCookingStation),
			typeof(RoleStation),
			typeof(ResearchCenter),
			typeof(NuclearResearchCenter),
			typeof(LiquidCooledFan),
			typeof(HandSanitizer),
			typeof(HandSanitizer.Work),
			typeof(PlantAirConditioner),
			typeof(Clinic),
			typeof(BuildingElementEmitter),
			typeof(ElementConverter),
			typeof(ElementConsumer),
			typeof(PassiveElementConsumer),
			typeof(TinkerStation),
			typeof(EnergyConsumer),
			typeof(AirConditioner),
			typeof(Storage),
			typeof(Battery),
			typeof(AirFilter),
			typeof(FlushToilet),
			typeof(Toilet),
			typeof(EnergyGenerator),
			typeof(MassageTable),
			typeof(Shower),
			typeof(Ownable),
			typeof(PlantablePlot),
			typeof(RelaxationPoint),
			typeof(BuildingComplete),
			typeof(Building),
			typeof(BuildingPreview),
			typeof(BuildingUnderConstruction),
			typeof(Crop),
			typeof(Growing),
			typeof(Equippable),
			typeof(ColdBreather),
			typeof(ResearchPointObject),
			typeof(SuitTank),
			typeof(IlluminationVulnerable),
			typeof(TemperatureVulnerable),
			typeof(PressureVulnerable),
			typeof(SubmersionMonitor),
			typeof(BatterySmart),
			typeof(Compost),
			typeof(Refrigerator),
			typeof(Bed),
			typeof(OreScrubber),
			typeof(OreScrubber.Work),
			typeof(MinimumOperatingTemperature),
			typeof(RoomTracker),
			typeof(EnergyConsumerSelfSustaining),
			typeof(ArcadeMachine),
			typeof(Telescope),
			typeof(EspressoMachine),
			typeof(JetSuitTank),
			typeof(Phonobox),
			typeof(ArcadeMachine),
			typeof(BeachChair),
			typeof(Sauna),
			typeof(VerticalWindTunnel),
			typeof(HotTub),
			typeof(Juicer),
			typeof(SodaFountain),
			typeof(MechanicalSurfboard),
			typeof(BottleEmptier),
			typeof(AccessControl),
			typeof(GammaRayOven),
			typeof(Reactor),
			typeof(HighEnergyParticlePort),
			typeof(LeadSuitTank),
			typeof(ActiveParticleConsumer.Def),
			typeof(WaterCooler),
			typeof(Edible),
			typeof(PlantableSeed),
			typeof(SicknessTrigger),
			typeof(MedicinalPill),
			typeof(SeedProducer),
			typeof(Geyser),
			typeof(SpaceHeater),
			typeof(Overheatable),
			typeof(CreatureCalorieMonitor.Def),
			typeof(LureableMonitor.Def),
			typeof(CropSleepingMonitor.Def),
			typeof(FertilizationMonitor.Def),
			typeof(IrrigationMonitor.Def),
			typeof(ScaleGrowthMonitor.Def),
			typeof(TravelTubeEntrance.Work),
			typeof(ToiletWorkableUse),
			typeof(ReceptacleMonitor),
			typeof(Light2D),
			typeof(Ladder),
			typeof(SimCellOccupier),
			typeof(Vent),
			typeof(LogicPorts),
			typeof(Capturable),
			typeof(Trappable),
			typeof(SpaceArtifact),
			typeof(MessStation),
			typeof(PlantElementEmitter),
			typeof(Radiator),
			typeof(DecorProvider)
		};
	}
}
