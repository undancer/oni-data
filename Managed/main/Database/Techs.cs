using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		private readonly List<List<Tuple<string, float>>> TECH_TIERS;

		public static readonly Dictionary<string, float> TIER_5_ORBITAL_DOMINANT = new Dictionary<string, float>
		{
			{
				"nuclear",
				0f
			}
		};

		public static readonly Dictionary<string, float> TIER_5_NUCLEAR_DOMINANT = new Dictionary<string, float>
		{
			{
				"orbital",
				0f
			}
		};

		public static readonly Dictionary<string, float> TIER_6_ORBITAL_DOMINANT = new Dictionary<string, float>
		{
			{
				"nuclear",
				10f
			}
		};

		public static readonly Dictionary<string, float> TIER_6_NUCLEAR_DOMINANT = new Dictionary<string, float>
		{
			{
				"orbital",
				10f
			}
		};

		public static readonly Dictionary<string, float> TIER_7_ORBITAL_DOMINANT = new Dictionary<string, float>
		{
			{
				"nuclear",
				150f
			}
		};

		public static readonly Dictionary<string, float> TIER_7_NUCLEAR_DOMINANT = new Dictionary<string, float>
		{
			{
				"orbital",
				150f
			}
		};

		public static readonly List<string> RADIATION_IGNORED_TECHS = new List<string>
		{
			"NuclearResearch",
			"RadiationProtection",
			"NuclearRefinement"
		};

		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
			if (!DlcManager.IsExpansion1Active())
			{
				TECH_TIERS = new List<List<Tuple<string, float>>>
				{
					new List<Tuple<string, float>>(),
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 15f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 20f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 30f),
						new Tuple<string, float>("advanced", 20f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 35f),
						new Tuple<string, float>("advanced", 30f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 40f),
						new Tuple<string, float>("advanced", 50f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 50f),
						new Tuple<string, float>("advanced", 70f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("space", 200f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("space", 400f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("space", 800f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("space", 1600f)
					}
				};
			}
			else
			{
				TECH_TIERS = new List<List<Tuple<string, float>>>
				{
					new List<Tuple<string, float>>(),
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 15f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 20f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 30f),
						new Tuple<string, float>("advanced", 20f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 35f),
						new Tuple<string, float>("advanced", 30f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 40f),
						new Tuple<string, float>("advanced", 50f),
						new Tuple<string, float>("orbital", 5f),
						new Tuple<string, float>("nuclear", 5f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 50f),
						new Tuple<string, float>("advanced", 70f),
						new Tuple<string, float>("orbital", 40f),
						new Tuple<string, float>("nuclear", 40f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("orbital", 300f),
						new Tuple<string, float>("nuclear", 300f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 340f),
						new Tuple<string, float>("nuclear", 340f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 400f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 800f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 1600f)
					}
				};
			}
		}

		public void Init()
		{
			new Tech("FarmingTech", new List<string>
			{
				"AlgaeHabitat",
				"PlanterBox",
				"RationBox",
				"Compost"
			}, this);
			new Tech("FineDining", new List<string>
			{
				"DiningTable",
				"FarmTile",
				"CookingStation",
				"EggCracker"
			}, this);
			new Tech("FoodRepurposing", new List<string>
			{
				"Juicer"
			}, this);
			new Tech("FinerDining", new List<string>
			{
				"GourmetCookingStation",
				"GeneticAnalysisStation"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("Agriculture", new List<string>
			{
				"FertilizerMaker",
				"HydroponicFarm",
				"Refrigerator",
				"FarmStation",
				"ParkSign"
			}, this);
			new Tech("Ranching", new List<string>
			{
				"CreatureDeliveryPoint",
				"FishDeliveryPoint",
				"CreatureFeeder",
				"FishFeeder",
				"RanchStation",
				"ShearingStation",
				"FlyingCreatureBait"
			}, this);
			new Tech("AnimalControl", new List<string>
			{
				"CreatureTrap",
				"FishTrap",
				"AirborneCreatureLure",
				"EggIncubator",
				LogicCritterCountSensorConfig.ID
			}, this);
			new Tech("ImprovedOxygen", new List<string>
			{
				"Electrolyzer",
				"RustDeoxidizer"
			}, this);
			new Tech("GasPiping", new List<string>
			{
				"GasConduit",
				"GasPump",
				"GasVent",
				"GasConduitBridge"
			}, this);
			List<string> list = new List<string>
			{
				"InsulatedGasConduit",
				LogicPressureSensorGasConfig.ID,
				"GasVentHighPressure",
				"GasLogicValve",
				"GasConduitPreferentialFlow",
				"GasConduitOverflow",
				"ModularLaunchpadPortGas",
				"ModularLaunchpadPortGasUnloader",
				"GasCargoBaySmall"
			};
			if (!DlcManager.IsExpansion1Active())
			{
				list.Add("GasBottler");
			}
			new Tech("ImprovedGasPiping", list, this);
			new Tech("PressureManagement", new List<string>
			{
				"LiquidValve",
				"GasValve",
				"ManualPressureDoor",
				"GasPermeableMembrane"
			}, this);
			List<string> list2 = new List<string>
			{
				"OxygenMask",
				"OxygenMaskLocker",
				"OxygenMaskMarker",
				"CO2Engine"
			};
			if (DlcManager.IsExpansion1Active())
			{
				list2.Add("GasBottler");
				list2.Add("BottleEmptierGas");
			}
			new Tech("PortableGasses", list2, this);
			new Tech("DirectedAirStreams", new List<string>
			{
				"PressureDoor",
				"AirFilter",
				"CO2Scrubber"
			}, this);
			new Tech("LiquidFiltering", new List<string>
			{
				"OreScrubber",
				"Desalinator"
			}, this);
			new Tech("MedicineI", new List<string>
			{
				"Apothecary"
			}, this);
			new Tech("MedicineII", new List<string>
			{
				"DoctorStation",
				"HandSanitizer"
			}, this);
			new Tech("MedicineIII", new List<string>
			{
				LogicDiseaseSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID
			}, this);
			new Tech("MedicineIV", new List<string>
			{
				"AdvancedDoctorStation",
				"AdvancedApothecary",
				"HotTub",
				LogicRadiationSensorConfig.ID
			}, this);
			new Tech("LiquidPiping", new List<string>
			{
				"LiquidConduit",
				"LiquidPump",
				"LiquidVent",
				"LiquidConduitBridge"
			}, this);
			new Tech("ImprovedLiquidPiping", new List<string>
			{
				"InsulatedLiquidConduit",
				LogicPressureSensorLiquidConfig.ID,
				"LiquidLogicValve",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidReservoir"
			}, this);
			new Tech("PrecisionPlumbing", new List<string>
			{
				"EspressoMachine",
				"LiquidFuelTankCluster"
			}, this, TIER_5_ORBITAL_DOMINANT);
			new Tech("SanitationSciences", new List<string>
			{
				"WashSink",
				"FlushToilet",
				ShowerConfig.ID,
				"MeshTile"
			}, this);
			new Tech("FlowRedirection", new List<string>
			{
				"MechanicalSurfboard",
				"ModularLaunchpadPortLiquid",
				"ModularLaunchpadPortLiquidUnloader",
				"LiquidCargoBaySmall"
			}, this);
			new Tech("LiquidDistribution", new List<string>
			{
				"RocketInteriorLiquidInput",
				"RocketInteriorLiquidOutput"
			}, this);
			new Tech("AdvancedSanitation", new List<string>
			{
				"DecontaminationShower"
			}, this, TIER_5_ORBITAL_DOMINANT);
			new Tech("AdvancedFiltration", new List<string>
			{
				"GasFilter",
				"LiquidFilter",
				"SludgePress"
			}, this);
			List<string> list3 = new List<string>
			{
				"WaterPurifier",
				"AlgaeDistillery",
				"EthanolDistillery"
			};
			if (!DlcManager.IsExpansion1Active())
			{
				list3.Add("BottleEmptierGas");
			}
			new Tech("Distillation", list3, this);
			new Tech("Catalytics", new List<string>
			{
				"OxyliteRefinery",
				"SupermaterialRefinery",
				"SodaFountain",
				"GasCargoBayCluster"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("PowerRegulation", new List<string>
			{
				SwitchConfig.ID,
				"BatteryMedium",
				"WireBridge"
			}, this);
			new Tech("AdvancedPowerRegulation", new List<string>
			{
				"HydrogenGenerator",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"PowerTransformerSmall",
				LogicPowerRelayConfig.ID,
				LogicWattageSensorConfig.ID
			}, this);
			new Tech("PrettyGoodConductors", new List<string>
			{
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"PowerTransformer"
			}, this);
			new Tech("RenewableEnergy", new List<string>
			{
				"SteamTurbine",
				"SteamTurbine2",
				"SolarPanel",
				"Sauna",
				"SteamEngineCluster"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("Combustion", new List<string>
			{
				"Generator",
				"WoodGasGenerator",
				"SugarEngine",
				"SmallOxidizerTank"
			}, this);
			new Tech("ImprovedCombustion", new List<string>
			{
				"MethaneGenerator",
				"OilRefinery",
				"PetroleumGenerator"
			}, this);
			new Tech("InteriorDecor", new List<string>
			{
				"FlowerVase",
				"FloorLamp",
				"CeilingLight"
			}, this);
			new Tech("Artistry", new List<string>
			{
				"CrownMoulding",
				"CornerMoulding",
				"SmallSculpture",
				"IceSculpture",
				"ItemPedestal",
				"FlowerVaseWall",
				"FlowerVaseHanging"
			}, this);
			new Tech("Clothing", new List<string>
			{
				"ClothingFabricator",
				"CarpetTile"
			}, this);
			new Tech("Acoustics", new List<string>
			{
				"Phonobox",
				"BatterySmart",
				"PowerControlStation"
			}, this);
			new Tech("SpacePower", new List<string>
			{
				"RocketInteriorPowerPlug",
				"BatteryModule",
				"SolarPanelModule"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("NuclearRefinement", new List<string>
			{
				"NuclearReactor",
				"UraniumCentrifuge"
			}, this, TIER_6_NUCLEAR_DOMINANT);
			new Tech("FineArt", new List<string>
			{
				"Canvas",
				"Sculpture"
			}, this);
			new Tech("EnvironmentalAppreciation", new List<string>
			{
				"BeachChair"
			}, this, TIER_6_ORBITAL_DOMINANT);
			new Tech("Luxury", new List<string>
			{
				LuxuryBedConfig.ID,
				"LadderFast",
				"PlasticTile"
			}, this);
			new Tech("RefractiveDecor", new List<string>
			{
				"MetalSculpture",
				"CanvasWide"
			}, this);
			new Tech("GlassFurnishings", new List<string>
			{
				"GlassTile",
				"FlowerVaseHangingFancy",
				"SunLamp"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("Screens", new List<string>
			{
				PixelPackConfig.ID
			}, this, TIER_6_NUCLEAR_DOMINANT);
			List<string> list4 = new List<string>
			{
				"MarbleSculpture",
				"CanvasTall"
			};
			if (!DlcManager.IsExpansion1Active())
			{
				list4.AddRange(new List<string>
				{
					"MonumentBottom",
					"MonumentMiddle",
					"MonumentTop"
				});
			}
			new Tech("RenaissanceArt", list4, this, TIER_5_ORBITAL_DOMINANT);
			if (DlcManager.IsExpansion1Active())
			{
				new Tech("Monuments", new List<string>
				{
					"MonumentBottom",
					"MonumentMiddle",
					"MonumentTop"
				}, this);
			}
			new Tech("Plastics", new List<string>
			{
				"Polymerizer",
				"OilWellCap"
			}, this);
			new Tech("ValveMiniaturization", new List<string>
			{
				"LiquidMiniPump",
				"GasMiniPump",
				"KeroseneEngineClusterSmall"
			}, this, TIER_5_ORBITAL_DOMINANT);
			new Tech("HydrocarbonPropulsion", new List<string>
			{
				"KeroseneEngineCluster"
			}, this, TIER_6_ORBITAL_DOMINANT);
			new Tech("CryoFuelPropulsion", new List<string>
			{
				"HydrogenEngineCluster",
				"OxidizerTankLiquidCluster"
			}, this);
			new Tech("Suits", new List<string>
			{
				"ExteriorWall",
				"SuitMarker",
				"SuitLocker",
				"SuitFabricator",
				"SuitsOverlay",
				"AtmoSuit"
			}, this);
			new Tech("Jobs", new List<string>
			{
				"RoleStation",
				"WaterCooler",
				"CraftingTable"
			}, this);
			new Tech("AdvancedResearch", new List<string>
			{
				"AdvancedResearchCenter",
				"BetaResearchPoint",
				"ResetSkillsStation",
				"ClusterTelescope",
				"ExobaseHeadquarters"
			}, this);
			new Tech("SpaceProgram", new List<string>
			{
				"LaunchPad",
				"HabitatModuleSmall",
				"OrbitalCargoModule",
				RocketControlStationConfig.ID
			}, this);
			new Tech("CrashPlan", new List<string>
			{
				"PioneerModule",
				"OrbitalResearchCenter",
				"OrbitalResearchPoint"
			}, this);
			new Tech("DurableLifeSupport", new List<string>
			{
				"NoseconeBasic",
				"HabitatModuleMedium"
			}, this, TIER_5_ORBITAL_DOMINANT);
			new Tech("NuclearResearch", new List<string>
			{
				"DeltaResearchPoint",
				"NuclearResearchCenter",
				"HighEnergyParticleSpawner",
				"HighEnergyParticleRedirector"
			}, this);
			new Tech("NuclearPropulsion", new List<string>
			{
				"HEPEngine"
			}, this);
			new Tech("NotificationSystems", new List<string>
			{
				LogicHammerConfig.ID,
				LogicAlarmConfig.ID
			}, this);
			new Tech("ArtificialFriends", new List<string>
			{
				"SweepBotStation",
				"ScoutModule"
			}, this);
			if (DlcManager.IsExpansion1Active())
			{
				new Tech("RoboticTools", new List<string>
				{
					"AutoMiner",
					"RailGunPayloadOpener"
				}, this);
			}
			new Tech("BasicRefinement", new List<string>
			{
				"RockCrusher",
				"Kiln"
			}, this);
			new Tech("RefinedObjects", new List<string>
			{
				"ThermalBlock",
				"FirePole"
			}, this);
			new Tech("Smelting", new List<string>
			{
				"MetalRefinery",
				"MetalTile"
			}, this);
			new Tech("HighTempForging", new List<string>
			{
				"GlassForge",
				"BunkerTile",
				"BunkerDoor",
				"Gantry"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("RadiationProtection", new List<string>
			{
				"LeadSuit",
				"LeadSuitMarker",
				"LeadSuitLocker"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("TemperatureModulation", new List<string>
			{
				"LiquidCooledFan",
				"IceCooledFan",
				"IceMachine",
				"SpaceHeater",
				"InsulationTile"
			}, this);
			new Tech("HVAC", new List<string>
			{
				"AirConditioner",
				LogicTemperatureSensorConfig.ID,
				"GasConduitRadiant",
				GasConduitTemperatureSensorConfig.ID,
				GasConduitElementSensorConfig.ID,
				"GasReservoir"
			}, this);
			new Tech("GasDistribution", new List<string>
			{
				"RocketInteriorGasInput",
				"RocketInteriorGasOutput",
				"OxidizerTankCluster"
			}, this);
			new Tech("LiquidTemperature", new List<string>
			{
				"LiquidHeater",
				"LiquidConditioner",
				"LiquidConduitRadiant",
				LiquidConduitTemperatureSensorConfig.ID,
				LiquidConduitElementSensorConfig.ID
			}, this);
			new Tech("LogicControl", new List<string>
			{
				"LogicWire",
				"LogicDuplicantSensor",
				LogicSwitchConfig.ID,
				"LogicWireBridge",
				"AutomationOverlay"
			}, this);
			new Tech("GenericSensors", new List<string>
			{
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID,
				"FloorSwitch",
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				"LogicGateNOT"
			}, this);
			new Tech("LogicCircuits", new List<string>
			{
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateBUFFER",
				"LogicGateFILTER"
			}, this);
			new Tech("ParallelAutomation", new List<string>
			{
				"LogicRibbon",
				"LogicRibbonBridge",
				LogicRibbonWriterConfig.ID,
				LogicRibbonReaderConfig.ID
			}, this);
			new Tech("DupeTrafficControl", new List<string>
			{
				"Checkpoint",
				LogicMemoryConfig.ID,
				"ArcadeMachine",
				"CosmicResearchCenter",
				"LogicGateXOR",
				LogicCounterConfig.ID
			}, this, TIER_5_ORBITAL_DOMINANT);
			new Tech("AdvancedScanners", new List<string>
			{
				"ScannerModule"
			}, this, TIER_6_ORBITAL_DOMINANT);
			new Tech("Multiplexing", new List<string>
			{
				"LogicGateMultiplexer",
				"LogicGateDemultiplexer"
			}, this, TIER_6_NUCLEAR_DOMINANT);
			new Tech("SkyDetectors", new List<string>
			{
				CometDetectorConfig.ID,
				"Telescope",
				"AstronautTrainingCenter"
			}, this);
			new Tech("TravelTubes", new List<string>
			{
				"TravelTubeEntrance",
				"TravelTube",
				"TravelTubeWallBridge",
				"VerticalWindTunnel"
			}, this, TIER_5_NUCLEAR_DOMINANT);
			new Tech("SmartStorage", new List<string>
			{
				"StorageLockerSmart",
				"SolidTransferArm",
				"ObjectDispenser",
				"ConveyorOverlay"
			}, this);
			List<string> list5 = new List<string>
			{
				"SolidConduit",
				"SolidConduitBridge",
				"SolidConduitInbox",
				"SolidConduitOutbox"
			};
			if (!DlcManager.IsExpansion1Active())
			{
				list5.AddRange(new List<string>
				{
					"SolidVent",
					"SolidLogicValve",
					"AutoMiner"
				});
			}
			new Tech("SolidTransport", list5, this);
			if (DlcManager.IsExpansion1Active())
			{
				new Tech("SolidSpace", new List<string>
				{
					"SolidVent",
					"SolidLogicValve",
					"ModularLaunchpadPortSolid",
					"ModularLaunchpadPortSolidUnloader",
					"SolidCargoBaySmall",
					"RocketInteriorSolidInput",
					"RocketInteriorSolidOutput"
				}, this, TIER_5_ORBITAL_DOMINANT);
			}
			new Tech("SolidManagement", new List<string>
			{
				"SolidFilter",
				SolidConduitTemperatureSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitDiseaseSensorConfig.ID,
				"CargoBayCluster"
			}, this, TIER_6_ORBITAL_DOMINANT);
			new Tech("HighVelocityTransport", new List<string>
			{
				"RailGun",
				"LandingBeacon"
			}, this, TIER_6_NUCLEAR_DOMINANT);
			new Tech("BasicRocketry", new List<string>
			{
				"CommandModule",
				"SteamEngine",
				"ResearchModule",
				"Gantry"
			}, this);
			new Tech("CargoI", new List<string>
			{
				"CargoBay"
			}, this);
			new Tech("CargoII", new List<string>
			{
				"LiquidCargoBay",
				"GasCargoBay"
			}, this);
			new Tech("CargoIII", new List<string>
			{
				"TouristModule",
				"SpecialCargoBay",
				"ScannerModule"
			}, this);
			new Tech("EnginesI", new List<string>
			{
				"SolidBooster"
			}, this);
			new Tech("EnginesII", new List<string>
			{
				"KeroseneEngine",
				"LiquidFuelTank",
				"OxidizerTank"
			}, this);
			new Tech("EnginesIII", new List<string>
			{
				"OxidizerTankLiquid",
				"OxidizerTankCluster",
				"HydrogenEngine",
				"HEPEngine"
			}, this);
			new Tech("Jetpacks", new List<string>
			{
				"JetSuit",
				"JetSuitMarker",
				"JetSuitLocker",
				"LiquidCargoBayCluster"
			}, this, TIER_6_ORBITAL_DOMINANT);
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			if (!Sim.IsRadiationEnabled())
			{
				foreach (ResourceTreeNode item in new List<ResourceTreeNode>(resourceTreeLoader.resources))
				{
					if (RADIATION_IGNORED_TECHS.Contains(item.Id))
					{
						resourceTreeLoader.resources.Remove(item);
						continue;
					}
					for (int num = item.edges.Count - 1; num >= 0; num--)
					{
						ResourceTreeNode.Edge edge = item.edges[num];
						if (RADIATION_IGNORED_TECHS.Contains(edge.source.Id) || RADIATION_IGNORED_TECHS.Contains(edge.target.Id))
						{
							item.edges.RemoveAt(num);
						}
					}
					for (int num2 = item.references.Count - 1; num2 >= 0; num2--)
					{
						ResourceTreeNode resourceTreeNode = item.references[num2];
						if (RADIATION_IGNORED_TECHS.Contains(resourceTreeNode.Id))
						{
							item.references.RemoveAt(num2);
						}
					}
				}
				foreach (List<Tuple<string, float>> tECH_TIER in TECH_TIERS)
				{
					tECH_TIER.RemoveAll((Tuple<string, float> m) => m.first == "nuclear");
				}
			}
			List<TechTreeTitle> list = new List<TechTreeTitle>();
			for (int i = 0; i < Db.Get().TechTreeTitles.Count; i++)
			{
				list.Add(Db.Get().TechTreeTitles[i]);
			}
			list.Sort((TechTreeTitle a, TechTreeTitle b) => a.center.y.CompareTo(b.center.y));
			foreach (ResourceTreeNode item2 in resourceTreeLoader)
			{
				string a2 = item2.Id.Substring(0, 1);
				if (string.Equals(a2, "_"))
				{
					continue;
				}
				Tech tech = TryGet(item2.Id);
				Debug.Assert(tech != null, "Tech node found in yEd that is not found in DbTechs constructor: " + item2.Id);
				string categoryID = "";
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].center.y >= item2.center.y)
					{
						categoryID = list[j].Id;
						break;
					}
				}
				tech.SetNode(item2, categoryID);
				foreach (ResourceTreeNode reference in item2.references)
				{
					Tech tech2 = TryGet(reference.Id);
					Debug.Assert(tech2 != null, "Tech node found in yEd that is not found in DbTechs constructor: " + reference.Id);
					categoryID = "";
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k].center.y >= item2.center.y)
						{
							categoryID = list[k].Id;
							break;
						}
					}
					tech2.SetNode(reference, categoryID);
					tech2.requiredTech.Add(tech);
					tech.unlockedTech.Add(tech2);
				}
			}
			foreach (Tech resource in resources)
			{
				resource.tier = GetTier(resource);
				List<Tuple<string, float>> list2 = TECH_TIERS[resource.tier];
				foreach (Tuple<string, float> item3 in list2)
				{
					if (!resource.costsByResearchTypeID.ContainsKey(item3.first))
					{
						resource.costsByResearchTypeID.Add(item3.first, item3.second);
					}
				}
			}
			for (int num3 = Count - 1; num3 >= 0; num3--)
			{
				if (!((Tech)GetResource(num3)).FoundNode)
				{
					Remove((Tech)GetResource(num3));
				}
			}
		}

		public static int GetTier(Tech tech)
		{
			if (tech == null)
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

		public Tech TryGetTechForTechItem(string itemId)
		{
			for (int i = 0; i < Count; i++)
			{
				if (((Tech)GetResource(i)).unlockedItemIDs.Find((string match) => match == itemId) != null)
				{
					return (Tech)GetResource(i);
				}
			}
			return null;
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
