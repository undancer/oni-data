using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		public int tierCount;

		public static Dictionary<string, string[]> TECH_GROUPING = new Dictionary<string, string[]>
		{
			{
				"FarmingTech",
				new string[4]
				{
					"AlgaeHabitat",
					"PlanterBox",
					"RationBox",
					"Compost"
				}
			},
			{
				"FineDining",
				new string[4]
				{
					"DiningTable",
					"FarmTile",
					"CookingStation",
					"EggCracker"
				}
			},
			{
				"FoodRepurposing",
				new string[1]
				{
					"Juicer"
				}
			},
			{
				"FinerDining",
				new string[1]
				{
					"GourmetCookingStation"
				}
			},
			{
				"Agriculture",
				new string[5]
				{
					"FertilizerMaker",
					"HydroponicFarm",
					"Refrigerator",
					"FarmStation",
					"ParkSign"
				}
			},
			{
				"Ranching",
				new string[7]
				{
					"CreatureDeliveryPoint",
					"FishDeliveryPoint",
					"CreatureFeeder",
					"FishFeeder",
					"RanchStation",
					"ShearingStation",
					"FlyingCreatureBait"
				}
			},
			{
				"AnimalControl",
				new string[5]
				{
					"CreatureTrap",
					"FishTrap",
					"AirborneCreatureLure",
					"EggIncubator",
					LogicCritterCountSensorConfig.ID
				}
			},
			{
				"ImprovedOxygen",
				new string[2]
				{
					"Electrolyzer",
					"RustDeoxidizer"
				}
			},
			{
				"GasPiping",
				new string[4]
				{
					"GasConduit",
					"GasPump",
					"GasVent",
					"GasConduitBridge"
				}
			},
			{
				"ImprovedGasPiping",
				new string[7]
				{
					"InsulatedGasConduit",
					LogicPressureSensorGasConfig.ID,
					"GasVentHighPressure",
					"GasLogicValve",
					"GasBottler",
					"GasConduitPreferentialFlow",
					"GasConduitOverflow"
				}
			},
			{
				"PressureManagement",
				new string[4]
				{
					"LiquidValve",
					"GasValve",
					"ManualPressureDoor",
					"GasPermeableMembrane"
				}
			},
			{
				"DirectedAirStreams",
				new string[3]
				{
					"PressureDoor",
					"AirFilter",
					"CO2Scrubber"
				}
			},
			{
				"LiquidFiltering",
				new string[2]
				{
					"OreScrubber",
					"Desalinator"
				}
			},
			{
				"MedicineI",
				new string[1]
				{
					"Apothecary"
				}
			},
			{
				"MedicineII",
				new string[2]
				{
					"DoctorStation",
					"HandSanitizer"
				}
			},
			{
				"MedicineIII",
				new string[3]
				{
					LogicDiseaseSensorConfig.ID,
					GasConduitDiseaseSensorConfig.ID,
					LiquidConduitDiseaseSensorConfig.ID
				}
			},
			{
				"MedicineIV",
				new string[2]
				{
					"AdvancedDoctorStation",
					"HotTub"
				}
			},
			{
				"LiquidPiping",
				new string[4]
				{
					"LiquidConduit",
					"LiquidPump",
					"LiquidVent",
					"LiquidConduitBridge"
				}
			},
			{
				"ImprovedLiquidPiping",
				new string[6]
				{
					"InsulatedLiquidConduit",
					LogicPressureSensorLiquidConfig.ID,
					"LiquidLogicValve",
					"LiquidConduitPreferentialFlow",
					"LiquidConduitOverflow",
					"LiquidReservoir"
				}
			},
			{
				"PrecisionPlumbing",
				new string[1]
				{
					"EspressoMachine"
				}
			},
			{
				"SanitationSciences",
				new string[4]
				{
					"WashSink",
					"FlushToilet",
					ShowerConfig.ID,
					"MeshTile"
				}
			},
			{
				"FlowRedirection",
				new string[1]
				{
					"MechanicalSurfboard"
				}
			},
			{
				"AdvancedFiltration",
				new string[2]
				{
					"GasFilter",
					"LiquidFilter"
				}
			},
			{
				"Distillation",
				new string[4]
				{
					"WaterPurifier",
					"AlgaeDistillery",
					"EthanolDistillery",
					"BottleEmptierGas"
				}
			},
			{
				"Catalytics",
				new string[3]
				{
					"OxyliteRefinery",
					"SupermaterialRefinery",
					"SodaFountain"
				}
			},
			{
				"PowerRegulation",
				new string[3]
				{
					SwitchConfig.ID,
					"BatteryMedium",
					"WireBridge"
				}
			},
			{
				"AdvancedPowerRegulation",
				new string[6]
				{
					"HydrogenGenerator",
					"HighWattageWire",
					"WireBridgeHighWattage",
					"PowerTransformerSmall",
					LogicPowerRelayConfig.ID,
					LogicWattageSensorConfig.ID
				}
			},
			{
				"PrettyGoodConductors",
				new string[5]
				{
					"WireRefined",
					"WireRefinedBridge",
					"WireRefinedHighWattage",
					"WireRefinedBridgeHighWattage",
					"PowerTransformer"
				}
			},
			{
				"RenewableEnergy",
				new string[4]
				{
					"SteamTurbine",
					"SteamTurbine2",
					"SolarPanel",
					"Sauna"
				}
			},
			{
				"Combustion",
				new string[2]
				{
					"Generator",
					"WoodGasGenerator"
				}
			},
			{
				"ImprovedCombustion",
				new string[3]
				{
					"MethaneGenerator",
					"OilRefinery",
					"PetroleumGenerator"
				}
			},
			{
				"InteriorDecor",
				new string[3]
				{
					"FlowerVase",
					"FloorLamp",
					"CeilingLight"
				}
			},
			{
				"Artistry",
				new string[7]
				{
					"CrownMoulding",
					"CornerMoulding",
					"SmallSculpture",
					"IceSculpture",
					"ItemPedestal",
					"FlowerVaseWall",
					"FlowerVaseHanging"
				}
			},
			{
				"Clothing",
				new string[2]
				{
					"ClothingFabricator",
					"CarpetTile"
				}
			},
			{
				"Acoustics",
				new string[3]
				{
					"Phonobox",
					"BatterySmart",
					"PowerControlStation"
				}
			},
			{
				"FineArt",
				new string[2]
				{
					"Canvas",
					"Sculpture"
				}
			},
			{
				"EnvironmentalAppreciation",
				new string[1]
				{
					"BeachChair"
				}
			},
			{
				"Luxury",
				new string[3]
				{
					LuxuryBedConfig.ID,
					"LadderFast",
					"PlasticTile"
				}
			},
			{
				"RefractiveDecor",
				new string[2]
				{
					"MetalSculpture",
					"CanvasWide"
				}
			},
			{
				"GlassFurnishings",
				new string[3]
				{
					"GlassTile",
					"FlowerVaseHangingFancy",
					"SunLamp"
				}
			},
			{
				"Screens",
				new string[1]
				{
					PixelPackConfig.ID
				}
			},
			{
				"RenaissanceArt",
				new string[5]
				{
					"MarbleSculpture",
					"CanvasTall",
					"MonumentBottom",
					"MonumentMiddle",
					"MonumentTop"
				}
			},
			{
				"Plastics",
				new string[2]
				{
					"Polymerizer",
					"OilWellCap"
				}
			},
			{
				"ValveMiniaturization",
				new string[2]
				{
					"LiquidMiniPump",
					"GasMiniPump"
				}
			},
			{
				"Suits",
				new string[5]
				{
					"ExteriorWall",
					"SuitMarker",
					"SuitLocker",
					"SuitFabricator",
					"SuitsOverlay"
				}
			},
			{
				"Jobs",
				new string[2]
				{
					"RoleStation",
					"WaterCooler"
				}
			},
			{
				"AdvancedResearch",
				new string[3]
				{
					"AdvancedResearchCenter",
					"BetaResearchPoint",
					"ResetSkillsStation"
				}
			},
			{
				"NotificationSystems",
				new string[2]
				{
					LogicHammerConfig.ID,
					LogicAlarmConfig.ID
				}
			},
			{
				"ArtificialFriends",
				new string[1]
				{
					"SweepBotStation"
				}
			},
			{
				"BasicRefinement",
				new string[2]
				{
					"RockCrusher",
					"Kiln"
				}
			},
			{
				"RefinedObjects",
				new string[2]
				{
					"ThermalBlock",
					"FirePole"
				}
			},
			{
				"Smelting",
				new string[2]
				{
					"MetalRefinery",
					"MetalTile"
				}
			},
			{
				"HighTempForging",
				new string[3]
				{
					"GlassForge",
					"BunkerTile",
					"BunkerDoor"
				}
			},
			{
				"TemperatureModulation",
				new string[5]
				{
					"LiquidCooledFan",
					"IceCooledFan",
					"IceMachine",
					"SpaceHeater",
					"InsulationTile"
				}
			},
			{
				"HVAC",
				new string[6]
				{
					"AirConditioner",
					LogicTemperatureSensorConfig.ID,
					"GasConduitRadiant",
					GasConduitTemperatureSensorConfig.ID,
					GasConduitElementSensorConfig.ID,
					"GasReservoir"
				}
			},
			{
				"LiquidTemperature",
				new string[5]
				{
					"LiquidHeater",
					"LiquidConditioner",
					"LiquidConduitRadiant",
					LiquidConduitTemperatureSensorConfig.ID,
					LiquidConduitElementSensorConfig.ID
				}
			},
			{
				"LogicControl",
				new string[5]
				{
					"LogicWire",
					"LogicDuplicantSensor",
					LogicSwitchConfig.ID,
					"LogicWireBridge",
					"AutomationOverlay"
				}
			},
			{
				"GenericSensors",
				new string[6]
				{
					LogicTimeOfDaySensorConfig.ID,
					LogicTimerSensorConfig.ID,
					"FloorSwitch",
					LogicElementSensorGasConfig.ID,
					LogicElementSensorLiquidConfig.ID,
					"LogicGateNOT"
				}
			},
			{
				"LogicCircuits",
				new string[4]
				{
					"LogicGateAND",
					"LogicGateOR",
					"LogicGateBUFFER",
					"LogicGateFILTER"
				}
			},
			{
				"ParallelAutomation",
				new string[4]
				{
					"LogicRibbon",
					"LogicRibbonBridge",
					LogicRibbonWriterConfig.ID,
					LogicRibbonReaderConfig.ID
				}
			},
			{
				"DupeTrafficControl",
				new string[6]
				{
					"Checkpoint",
					LogicMemoryConfig.ID,
					"ArcadeMachine",
					"CosmicResearchCenter",
					"LogicGateXOR",
					LogicCounterConfig.ID
				}
			},
			{
				"Multiplexing",
				new string[2]
				{
					"LogicGateMultiplexer",
					"LogicGateDemultiplexer"
				}
			},
			{
				"SkyDetectors",
				new string[3]
				{
					CometDetectorConfig.ID,
					"Telescope",
					"AstronautTrainingCenter"
				}
			},
			{
				"TravelTubes",
				new string[4]
				{
					"TravelTubeEntrance",
					"TravelTube",
					"TravelTubeWallBridge",
					"VerticalWindTunnel"
				}
			},
			{
				"SmartStorage",
				new string[4]
				{
					"StorageLockerSmart",
					"SolidTransferArm",
					"ObjectDispenser",
					"ConveyorOverlay"
				}
			},
			{
				"SolidTransport",
				new string[7]
				{
					"SolidConduit",
					"SolidConduitBridge",
					"SolidConduitInbox",
					"SolidConduitOutbox",
					"SolidVent",
					"SolidLogicValve",
					"AutoMiner"
				}
			},
			{
				"SolidManagement",
				new string[4]
				{
					"SolidFilter",
					SolidConduitTemperatureSensorConfig.ID,
					SolidConduitElementSensorConfig.ID,
					SolidConduitDiseaseSensorConfig.ID
				}
			},
			{
				"BasicRocketry",
				new string[4]
				{
					"CommandModule",
					"SteamEngine",
					"ResearchModule",
					"Gantry"
				}
			},
			{
				"CargoI",
				new string[1]
				{
					"CargoBay"
				}
			},
			{
				"CargoII",
				new string[2]
				{
					"LiquidCargoBay",
					"GasCargoBay"
				}
			},
			{
				"CargoIII",
				new string[2]
				{
					"TouristModule",
					"SpecialCargoBay"
				}
			},
			{
				"EnginesI",
				new string[1]
				{
					"SolidBooster"
				}
			},
			{
				"EnginesII",
				new string[3]
				{
					"KeroseneEngine",
					"LiquidFuelTank",
					"OxidizerTank"
				}
			},
			{
				"EnginesIII",
				new string[2]
				{
					"OxidizerTankLiquid",
					"HydrogenEngine"
				}
			},
			{
				"Jetpacks",
				new string[3]
				{
					"JetSuit",
					"JetSuitMarker",
					"JetSuitLocker"
				}
			}
		};

		private readonly List<List<Tuple<string, float>>> TECH_TIERS = new List<List<Tuple<string, float>>>
		{
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 15f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 30f),
				new Tuple<string, float>("beta", 20f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 35f),
				new Tuple<string, float>("beta", 30f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 40f),
				new Tuple<string, float>("beta", 50f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 50f),
				new Tuple<string, float>("beta", 70f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 200f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 400f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 800f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("gamma", 1600f)
			}
		};

		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
		}

		public void Load(TextAsset tree_file)
		{
			foreach (ResourceTreeNode item in new ResourceTreeLoader<ResourceTreeNode>(tree_file))
			{
				if (string.Equals(item.Id.Substring(0, 1), "_"))
				{
					continue;
				}
				Tech tech = TryGet(item.Id);
				if (tech == null)
				{
					tech = new Tech(item.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + item.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + item.Id.ToUpper() + ".DESC"), item);
				}
				foreach (ResourceTreeNode reference in item.references)
				{
					Tech tech2 = TryGet(reference.Id);
					if (tech2 == null)
					{
						tech2 = new Tech(reference.Id, this, Strings.Get("STRINGS.RESEARCH.TECHS." + reference.Id.ToUpper() + ".NAME"), Strings.Get("STRINGS.RESEARCH.TECHS." + reference.Id.ToUpper() + ".DESC"), reference);
					}
					tech2.requiredTech.Add(tech);
					tech.unlockedTech.Add(tech2);
				}
			}
			tierCount = 0;
			foreach (Tech resource in resources)
			{
				resource.tier = GetTier(resource);
				foreach (Tuple<string, float> item2 in TECH_TIERS[resource.tier])
				{
					resource.costsByResearchTypeID.Add(item2.first, item2.second);
				}
				tierCount = Math.Max(resource.tier + 1, tierCount);
			}
		}

		private int GetTier(Tech tech)
		{
			if (tech.requiredTech.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (Tech item in tech.requiredTech)
			{
				num = Math.Max(num, GetTier(item));
			}
			return num + 1;
		}

		private void AddPrerequisite(Tech tech, string prerequisite_name)
		{
			Tech tech2 = TryGet(prerequisite_name);
			if (tech2 != null)
			{
				tech.requiredTech.Add(tech2);
				tech2.unlockedTech.Add(tech);
			}
		}

		public bool IsTechItemComplete(string id)
		{
			foreach (Tech resource in resources)
			{
				foreach (TechItem unlockedItem in resource.unlockedItems)
				{
					if (unlockedItem.Id == id)
					{
						return resource.IsComplete();
					}
				}
			}
			return true;
		}
	}
}
