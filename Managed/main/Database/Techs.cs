using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		private readonly List<List<Tuple<string, float>>> TECH_TIERS;

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
						new Tuple<string, float>("orbital", 0f),
						new Tuple<string, float>("nuclear", 20f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 50f),
						new Tuple<string, float>("advanced", 70f),
						new Tuple<string, float>("orbital", 30f),
						new Tuple<string, float>("nuclear", 40f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 70f),
						new Tuple<string, float>("advanced", 100f),
						new Tuple<string, float>("orbital", 250f),
						new Tuple<string, float>("nuclear", 370f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 400f),
						new Tuple<string, float>("nuclear", 435f)
					},
					new List<Tuple<string, float>>
					{
						new Tuple<string, float>("basic", 100f),
						new Tuple<string, float>("advanced", 130f),
						new Tuple<string, float>("orbital", 600f)
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
				"CookingStation",
				"EggCracker",
				"DiningTable",
				"FarmTile"
			}, this);
			new Tech("FoodRepurposing", new List<string>
			{
				"Juicer"
			}, this);
			new Tech("FinerDining", new List<string>
			{
				"GourmetCookingStation"
			}, this);
			new Tech("Agriculture", new List<string>
			{
				"FarmStation",
				"FertilizerMaker",
				"Refrigerator",
				"HydroponicFarm",
				"ParkSign"
			}, this);
			new Tech("Ranching", new List<string>
			{
				"RanchStation",
				"CreatureDeliveryPoint",
				"ShearingStation",
				"CreatureFeeder",
				"FlyingCreatureBait",
				"FishDeliveryPoint",
				"FishFeeder"
			}, this);
			new Tech("AnimalControl", new List<string>
			{
				"CreatureTrap",
				"FishTrap",
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
				"GasConduitBridge",
				"GasPump",
				"GasVent"
			}, this);
			new Tech("ImprovedGasPiping", new List<string>
			{
				"InsulatedGasConduit",
				LogicPressureSensorGasConfig.ID,
				"GasLogicValve",
				"GasVentHighPressure"
			}, this);
			new Tech("SpaceGas", new List<string>
			{
				"CO2Engine",
				"ModularLaunchpadPortGas",
				"ModularLaunchpadPortGasUnloader",
				"GasCargoBaySmall"
			}, this);
			new Tech("PressureManagement", new List<string>
			{
				"LiquidValve",
				"GasValve",
				"GasPermeableMembrane",
				"ManualPressureDoor"
			}, this);
			new Tech("DirectedAirStreams", new List<string>
			{
				"AirFilter",
				"CO2Scrubber",
				"PressureDoor"
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
				GasConduitDiseaseSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID,
				LogicDiseaseSensorConfig.ID
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
				"LiquidConduitBridge",
				"LiquidPump",
				"LiquidVent"
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
			}, this);
			new Tech("SanitationSciences", new List<string>
			{
				"FlushToilet",
				"WashSink",
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
			}, this);
			new Tech("AdvancedFiltration", new List<string>
			{
				"GasFilter",
				"LiquidFilter",
				"SludgePress"
			}, this);
			new Tech("Distillation", new List<string>
			{
				"AlgaeDistillery",
				"EthanolDistillery",
				"WaterPurifier"
			}, this);
			new Tech("Catalytics", new List<string>
			{
				"OxyliteRefinery",
				"SupermaterialRefinery",
				"SodaFountain",
				"GasCargoBayCluster"
			}, this);
			new Tech("AdvancedResourceExtraction", new List<string>
			{
				"NoseconeHarvest"
			}, this);
			new Tech("PowerRegulation", new List<string>
			{
				"BatteryMedium",
				SwitchConfig.ID,
				"WireBridge"
			}, this);
			new Tech("AdvancedPowerRegulation", new List<string>
			{
				"HighWattageWire",
				"WireBridgeHighWattage",
				"HydrogenGenerator",
				LogicPowerRelayConfig.ID,
				"PowerTransformerSmall",
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
				"SteamTurbine2",
				"SolarPanel",
				"Sauna",
				"SteamEngineCluster"
			}, this);
			new Tech("Combustion", new List<string>
			{
				"Generator",
				"WoodGasGenerator"
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
				"FlowerVaseWall",
				"FlowerVaseHanging",
				"CornerMoulding",
				"CrownMoulding",
				"ItemPedestal",
				"SmallSculpture",
				"IceSculpture"
			}, this);
			new Tech("Clothing", new List<string>
			{
				"ClothingFabricator",
				"CarpetTile"
			}, this);
			new Tech("Acoustics", new List<string>
			{
				"BatterySmart",
				"Phonobox",
				"PowerControlStation"
			}, this);
			new Tech("SpacePower", new List<string>
			{
				"BatteryModule",
				"SolarPanelModule",
				"RocketInteriorPowerPlug"
			}, this);
			new Tech("NuclearRefinement", new List<string>
			{
				"NuclearReactor",
				"UraniumCentrifuge"
			}, this);
			new Tech("FineArt", new List<string>
			{
				"Canvas",
				"Sculpture"
			}, this);
			new Tech("EnvironmentalAppreciation", new List<string>
			{
				"BeachChair"
			}, this);
			new Tech("Luxury", new List<string>
			{
				LuxuryBedConfig.ID,
				"LadderFast",
				"PlasticTile"
			}, this);
			new Tech("RefractiveDecor", new List<string>
			{
				"CanvasWide",
				"MetalSculpture"
			}, this);
			new Tech("GlassFurnishings", new List<string>
			{
				"GlassTile",
				"FlowerVaseHangingFancy",
				"SunLamp"
			}, this);
			new Tech("Screens", new List<string>
			{
				PixelPackConfig.ID
			}, this);
			new Tech("RenaissanceArt", new List<string>
			{
				"CanvasTall",
				"MarbleSculpture"
			}, this);
			new Tech("Plastics", new List<string>
			{
				"Polymerizer",
				"OilWellCap"
			}, this);
			new Tech("ValveMiniaturization", new List<string>
			{
				"LiquidMiniPump",
				"GasMiniPump"
			}, this);
			new Tech("HydrocarbonPropulsion", new List<string>
			{
				"KeroseneEngineClusterSmall"
			}, this);
			new Tech("BetterHydroCarbonPropulsion", new List<string>
			{
				"KeroseneEngineCluster"
			}, this);
			new Tech("CryoFuelPropulsion", new List<string>
			{
				"HydrogenEngineCluster",
				"OxidizerTankLiquidCluster"
			}, this);
			new Tech("Suits", new List<string>
			{
				"SuitsOverlay",
				"AtmoSuit",
				"SuitFabricator",
				"ExteriorWall",
				"SuitMarker",
				"SuitLocker"
			}, this);
			new Tech("Jobs", new List<string>
			{
				"WaterCooler",
				"CraftingTable"
			}, this);
			new Tech("AdvancedResearch", new List<string>
			{
				"BetaResearchPoint",
				"AdvancedResearchCenter",
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
				"OrbitalResearchPoint",
				"PioneerModule",
				"OrbitalResearchCenter"
			}, this);
			new Tech("DurableLifeSupport", new List<string>
			{
				"NoseconeBasic",
				"HabitatModuleMedium",
				"ArtifactAnalysisStation",
				"ArtifactCargoBay"
			}, this);
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
			new Tech("BasicRefinement", new List<string>
			{
				"RockCrusher",
				"Kiln"
			}, this);
			new Tech("RefinedObjects", new List<string>
			{
				"FirePole",
				"ThermalBlock"
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
			}, this);
			new Tech("HighPressureForging", new List<string>
			{
				"DiamondPress"
			}, this);
			new Tech("RadiationProtection", new List<string>
			{
				"LeadSuit",
				"LeadSuitMarker",
				"LeadSuitLocker"
			}, this);
			new Tech("TemperatureModulation", new List<string>
			{
				"LiquidCooledFan",
				"IceCooledFan",
				"IceMachine",
				"InsulationTile",
				"SpaceHeater"
			}, this);
			new Tech("HVAC", new List<string>
			{
				"AirConditioner",
				LogicTemperatureSensorConfig.ID,
				GasConduitTemperatureSensorConfig.ID,
				GasConduitElementSensorConfig.ID,
				"GasConduitRadiant",
				"GasReservoir",
				"GasLimitValve"
			}, this);
			new Tech("LiquidTemperature", new List<string>
			{
				"LiquidConduitRadiant",
				"LiquidConditioner",
				LiquidConduitTemperatureSensorConfig.ID,
				LiquidConduitElementSensorConfig.ID,
				"LiquidHeater",
				"LiquidLimitValve"
			}, this);
			new Tech("LogicControl", new List<string>
			{
				"AutomationOverlay",
				LogicSwitchConfig.ID,
				"LogicWire",
				"LogicWireBridge",
				"LogicDuplicantSensor"
			}, this);
			new Tech("GenericSensors", new List<string>
			{
				"FloorSwitch",
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				"LogicGateNOT",
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID
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
				LogicCounterConfig.ID,
				LogicMemoryConfig.ID,
				"LogicGateXOR",
				"ArcadeMachine",
				"Checkpoint",
				"CosmicResearchCenter"
			}, this);
			new Tech("Multiplexing", new List<string>
			{
				"LogicGateMultiplexer",
				"LogicGateDemultiplexer"
			}, this);
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
			}, this);
			new Tech("SmartStorage", new List<string>
			{
				"ConveyorOverlay",
				"SolidTransferArm",
				"StorageLockerSmart",
				"ObjectDispenser"
			}, this);
			new Tech("SolidManagement", new List<string>
			{
				"SolidFilter",
				SolidConduitTemperatureSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitDiseaseSensorConfig.ID,
				"CargoBayCluster"
			}, this);
			new Tech("HighVelocityTransport", new List<string>
			{
				"RailGun",
				"LandingBeacon"
			}, this);
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
				"SpecialCargoBay"
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
				"HydrogenEngine"
			}, this);
			new Tech("Jetpacks", new List<string>
			{
				"JetSuit",
				"JetSuitMarker",
				"JetSuitLocker",
				"LiquidCargoBayCluster"
			}, this);
			new Tech("SolidTransport", new List<string>
			{
				"SolidConduitInbox",
				"SolidConduit",
				"SolidConduitBridge",
				"SolidVent"
			}, this);
			new Tech("Monuments", new List<string>
			{
				"MonumentBottom",
				"MonumentMiddle",
				"MonumentTop"
			}, this);
			new Tech("SolidSpace", new List<string>
			{
				"SolidLogicValve",
				"SolidConduitOutbox",
				"SolidLimitValve",
				"SolidCargoBaySmall",
				"RocketInteriorSolidInput",
				"RocketInteriorSolidOutput",
				"ModularLaunchpadPortSolid",
				"ModularLaunchpadPortSolidUnloader"
			}, this);
			new Tech("RoboticTools", new List<string>
			{
				"AutoMiner",
				"RailGunPayloadOpener"
			}, this);
			new Tech("PortableGasses", new List<string>
			{
				"GasBottler",
				"BottleEmptierGas",
				"OxygenMask",
				"OxygenMaskLocker",
				"OxygenMaskMarker"
			}, this);
			InitExpansion1();
		}

		private void InitExpansion1()
		{
			if (DlcManager.IsExpansion1Active())
			{
				new Tech("Bioengineering", new List<string>
				{
					"GeneticAnalysisStation"
				}, this);
				new Tech("SpaceCombustion", new List<string>
				{
					"SugarEngine",
					"SmallOxidizerTank"
				}, this);
				new Tech("HighVelocityDestruction", new List<string>
				{
					"NoseconeHarvest"
				}, this);
				new Tech("GasDistribution", new List<string>
				{
					"RocketInteriorGasInput",
					"RocketInteriorGasOutput",
					"OxidizerTankCluster"
				}, this);
				new Tech("AdvancedScanners", new List<string>
				{
					"ScannerModule"
				}, this);
			}
		}

		public void PostProcess()
		{
			foreach (Tech resource in resources)
			{
				List<TechItem> list = new List<TechItem>();
				foreach (string unlockedItemID in resource.unlockedItemIDs)
				{
					TechItem techItem = Db.Get().TechItems.TryGet(unlockedItemID);
					if (techItem != null)
					{
						list.Add(techItem);
					}
				}
				resource.unlockedItems = list;
			}
		}

		public void Load(TextAsset tree_file)
		{
			ResourceTreeLoader<ResourceTreeNode> resourceTreeLoader = new ResourceTreeLoader<ResourceTreeNode>(tree_file);
			List<TechTreeTitle> list = new List<TechTreeTitle>();
			for (int i = 0; i < Db.Get().TechTreeTitles.Count; i++)
			{
				list.Add(Db.Get().TechTreeTitles[i]);
			}
			list.Sort((TechTreeTitle a, TechTreeTitle b) => a.center.y.CompareTo(b.center.y));
			foreach (ResourceTreeNode item in resourceTreeLoader)
			{
				if (string.Equals(item.Id.Substring(0, 1), "_"))
				{
					continue;
				}
				Tech tech = TryGet(item.Id);
				Debug.Assert(tech != null, "Tech node found in yEd that is not found in DbTechs constructor: " + item.Id);
				string categoryID = "";
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].center.y >= item.center.y)
					{
						categoryID = list[j].Id;
						break;
					}
				}
				tech.SetNode(item, categoryID);
				foreach (ResourceTreeNode reference in item.references)
				{
					Tech tech2 = TryGet(reference.Id);
					Debug.Assert(tech2 != null, "Tech node found in yEd that is not found in DbTechs constructor: " + reference.Id);
					categoryID = "";
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k].center.y >= item.center.y)
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
				foreach (Tuple<string, float> item2 in TECH_TIERS[resource.tier])
				{
					if (!resource.costsByResearchTypeID.ContainsKey(item2.first))
					{
						resource.costsByResearchTypeID.Add(item2.first, item2.second);
					}
				}
			}
			for (int num = Count - 1; num >= 0; num--)
			{
				if (!((Tech)GetResource(num)).FoundNode)
				{
					Remove((Tech)GetResource(num));
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
				Tech tech = (Tech)GetResource(i);
				if (tech.unlockedItemIDs.Find((string match) => match == itemId) != null)
				{
					return tech;
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
