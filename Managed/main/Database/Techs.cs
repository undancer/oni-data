using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
	public class Techs : ResourceSet<Tech>
	{
		private readonly List<List<Tuple<string, float>>> TECH_TIERS = new List<List<Tuple<string, float>>>
		{
			new List<Tuple<string, float>>(),
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
				new Tuple<string, float>("beta", 50f),
				new Tuple<string, float>("delta", 10f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 50f),
				new Tuple<string, float>("beta", 70f),
				new Tuple<string, float>("delta", 40f)
			},
			new List<Tuple<string, float>>
			{
				new Tuple<string, float>("alpha", 70f),
				new Tuple<string, float>("beta", 100f),
				new Tuple<string, float>("delta", 100f)
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

		public static readonly List<string> RADIATION_IGNORED_TECHS = new List<string>
		{
			"NuclearResearch",
			"RadiationProtection",
			"NuclearRefinement"
		};

		public Techs(ResourceSet parent)
			: base("Techs", parent)
		{
		}

		public void Init()
		{
			new Tech("FarmingTech", new string[4]
			{
				"AlgaeHabitat",
				"PlanterBox",
				"RationBox",
				"Compost"
			}, this);
			new Tech("FineDining", new string[4]
			{
				"DiningTable",
				"FarmTile",
				"CookingStation",
				"EggCracker"
			}, this);
			new Tech("FoodRepurposing", new string[1]
			{
				"Juicer"
			}, this);
			new Tech("FinerDining", new string[1]
			{
				"GourmetCookingStation"
			}, this);
			new Tech("Agriculture", new string[5]
			{
				"FertilizerMaker",
				"HydroponicFarm",
				"Refrigerator",
				"FarmStation",
				"ParkSign"
			}, this);
			new Tech("Ranching", new string[7]
			{
				"CreatureDeliveryPoint",
				"FishDeliveryPoint",
				"CreatureFeeder",
				"FishFeeder",
				"RanchStation",
				"ShearingStation",
				"FlyingCreatureBait"
			}, this);
			new Tech("AnimalControl", new string[5]
			{
				"CreatureTrap",
				"FishTrap",
				"AirborneCreatureLure",
				"EggIncubator",
				LogicCritterCountSensorConfig.ID
			}, this);
			new Tech("ImprovedOxygen", new string[2]
			{
				"Electrolyzer",
				"RustDeoxidizer"
			}, this);
			new Tech("GasPiping", new string[4]
			{
				"GasConduit",
				"GasPump",
				"GasVent",
				"GasConduitBridge"
			}, this);
			new Tech("ImprovedGasPiping", new string[9]
			{
				"InsulatedGasConduit",
				LogicPressureSensorGasConfig.ID,
				"GasVentHighPressure",
				"GasLogicValve",
				"GasConduitPreferentialFlow",
				"GasConduitOverflow",
				"ModularLaunchpadPortGas",
				"GasCargoBaySmall",
				"SmallOxidizerTank"
			}, this);
			new Tech("PressureManagement", new string[4]
			{
				"LiquidValve",
				"GasValve",
				"ManualPressureDoor",
				"GasPermeableMembrane"
			}, this);
			new Tech("PortableGasses", new string[4]
			{
				"OxygenMaskStation",
				"GasBottler",
				"BottleEmptierGas",
				"CO2Engine"
			}, this);
			new Tech("DirectedAirStreams", new string[3]
			{
				"PressureDoor",
				"AirFilter",
				"CO2Scrubber"
			}, this);
			new Tech("LiquidFiltering", new string[2]
			{
				"OreScrubber",
				"Desalinator"
			}, this);
			new Tech("MedicineI", new string[1]
			{
				"Apothecary"
			}, this);
			new Tech("MedicineII", new string[2]
			{
				"DoctorStation",
				"HandSanitizer"
			}, this);
			new Tech("MedicineIII", new string[3]
			{
				LogicDiseaseSensorConfig.ID,
				GasConduitDiseaseSensorConfig.ID,
				LiquidConduitDiseaseSensorConfig.ID
			}, this);
			new Tech("MedicineIV", new string[2]
			{
				"AdvancedDoctorStation",
				"HotTub"
			}, this);
			new Tech("LiquidPiping", new string[4]
			{
				"LiquidConduit",
				"LiquidPump",
				"LiquidVent",
				"LiquidConduitBridge"
			}, this);
			new Tech("ImprovedLiquidPiping", new string[6]
			{
				"InsulatedLiquidConduit",
				LogicPressureSensorLiquidConfig.ID,
				"LiquidLogicValve",
				"LiquidConduitPreferentialFlow",
				"LiquidConduitOverflow",
				"LiquidReservoir"
			}, this);
			new Tech("PrecisionPlumbing", new string[1]
			{
				"EspressoMachine"
			}, this);
			new Tech("SanitationSciences", new string[4]
			{
				"WashSink",
				"FlushToilet",
				ShowerConfig.ID,
				"MeshTile"
			}, this);
			new Tech("FlowRedirection", new string[3]
			{
				"MechanicalSurfboard",
				"ModularLaunchpadPortLiquid",
				"LiquidCargoBaySmall"
			}, this);
			new Tech("AdvancedFiltration", new string[3]
			{
				"GasFilter",
				"LiquidFilter",
				"SludgePress"
			}, this);
			new Tech("Distillation", new string[3]
			{
				"WaterPurifier",
				"AlgaeDistillery",
				"EthanolDistillery"
			}, this);
			new Tech("Catalytics", new string[3]
			{
				"OxyliteRefinery",
				"SupermaterialRefinery",
				"SodaFountain"
			}, this);
			new Tech("PowerRegulation", new string[3]
			{
				SwitchConfig.ID,
				"BatteryMedium",
				"WireBridge"
			}, this);
			new Tech("AdvancedPowerRegulation", new string[6]
			{
				"HydrogenGenerator",
				"HighWattageWire",
				"WireBridgeHighWattage",
				"PowerTransformerSmall",
				LogicPowerRelayConfig.ID,
				LogicWattageSensorConfig.ID
			}, this);
			new Tech("PrettyGoodConductors", new string[5]
			{
				"WireRefined",
				"WireRefinedBridge",
				"WireRefinedHighWattage",
				"WireRefinedBridgeHighWattage",
				"PowerTransformer"
			}, this);
			new Tech("RenewableEnergy", new string[4]
			{
				"SteamTurbine",
				"SteamTurbine2",
				"SolarPanel",
				"Sauna"
			}, this);
			new Tech("Combustion", new string[3]
			{
				"Generator",
				"WoodGasGenerator",
				"SugarEngine"
			}, this);
			new Tech("ImprovedCombustion", new string[3]
			{
				"MethaneGenerator",
				"OilRefinery",
				"PetroleumGenerator"
			}, this);
			new Tech("InteriorDecor", new string[3]
			{
				"FlowerVase",
				"FloorLamp",
				"CeilingLight"
			}, this);
			new Tech("Artistry", new string[7]
			{
				"CrownMoulding",
				"CornerMoulding",
				"SmallSculpture",
				"IceSculpture",
				"ItemPedestal",
				"FlowerVaseWall",
				"FlowerVaseHanging"
			}, this);
			new Tech("Clothing", new string[2]
			{
				"ClothingFabricator",
				"CarpetTile"
			}, this);
			new Tech("Acoustics", new string[3]
			{
				"Phonobox",
				"BatterySmart",
				"PowerControlStation"
			}, this);
			new Tech("NuclearRefinement", new string[2]
			{
				"NuclearReactor",
				"UraniumCentrifuge"
			}, this);
			new Tech("FineArt", new string[2]
			{
				"Canvas",
				"Sculpture"
			}, this);
			new Tech("EnvironmentalAppreciation", new string[1]
			{
				"BeachChair"
			}, this);
			new Tech("Luxury", new string[3]
			{
				LuxuryBedConfig.ID,
				"LadderFast",
				"PlasticTile"
			}, this);
			new Tech("RefractiveDecor", new string[2]
			{
				"MetalSculpture",
				"CanvasWide"
			}, this);
			new Tech("GlassFurnishings", new string[3]
			{
				"GlassTile",
				"FlowerVaseHangingFancy",
				"SunLamp"
			}, this);
			new Tech("Screens", new string[1]
			{
				PixelPackConfig.ID
			}, this);
			new Tech("RenaissanceArt", new string[5]
			{
				"MarbleSculpture",
				"CanvasTall",
				"MonumentBottom",
				"MonumentMiddle",
				"MonumentTop"
			}, this);
			new Tech("Plastics", new string[2]
			{
				"Polymerizer",
				"OilWellCap"
			}, this);
			new Tech("ValveMiniaturization", new string[2]
			{
				"LiquidMiniPump",
				"GasMiniPump"
			}, this);
			new Tech("Suits", new string[6]
			{
				"ExteriorWall",
				"SuitMarker",
				"SuitLocker",
				"SuitFabricator",
				"SuitsOverlay",
				"AtmoSuit"
			}, this);
			new Tech("Jobs", new string[2]
			{
				"RoleStation",
				"WaterCooler"
			}, this);
			new Tech("AdvancedResearch", new string[5]
			{
				"AdvancedResearchCenter",
				"BetaResearchPoint",
				"ResetSkillsStation",
				"ClusterTelescope",
				"ExobaseHeadquarters"
			}, this);
			new Tech("SpaceProgram", new string[4]
			{
				"LaunchPad",
				"HabitatModuleSmall",
				"OrbitalCargoModule",
				RocketControlStationConfig.ID
			}, this);
			new Tech("CrashPlan", new string[1]
			{
				"PioneerModule"
			}, this);
			new Tech("DurableLifeSupport", new string[2]
			{
				"NoseconeBasic",
				"HabitatModuleMedium"
			}, this);
			new Tech("NuclearResearch", new string[4]
			{
				"DeltaResearchPoint",
				"NuclearResearchCenter",
				"HighEnergyParticleSpawner",
				"HighEnergyParticleRedirector"
			}, this);
			new Tech("NotificationSystems", new string[2]
			{
				LogicHammerConfig.ID,
				LogicAlarmConfig.ID
			}, this);
			new Tech("ArtificialFriends", new string[2]
			{
				"SweepBotStation",
				"ScoutModule"
			}, this);
			new Tech("RoboticTools", new string[2]
			{
				"AutoMiner",
				"RailGunPayloadOpener"
			}, this);
			new Tech("BasicRefinement", new string[2]
			{
				"RockCrusher",
				"Kiln"
			}, this);
			new Tech("RefinedObjects", new string[2]
			{
				"ThermalBlock",
				"FirePole"
			}, this);
			new Tech("Smelting", new string[2]
			{
				"MetalRefinery",
				"MetalTile"
			}, this);
			new Tech("HighTempForging", new string[3]
			{
				"GlassForge",
				"BunkerTile",
				"BunkerDoor"
			}, this);
			new Tech("RadiationProtection", new string[3]
			{
				"LeadSuit",
				"LeadSuitMarker",
				"LeadSuitLocker"
			}, this);
			new Tech("TemperatureModulation", new string[5]
			{
				"LiquidCooledFan",
				"IceCooledFan",
				"IceMachine",
				"SpaceHeater",
				"InsulationTile"
			}, this);
			new Tech("HVAC", new string[6]
			{
				"AirConditioner",
				LogicTemperatureSensorConfig.ID,
				"GasConduitRadiant",
				GasConduitTemperatureSensorConfig.ID,
				GasConduitElementSensorConfig.ID,
				"GasReservoir"
			}, this);
			new Tech("LiquidTemperature", new string[5]
			{
				"LiquidHeater",
				"LiquidConditioner",
				"LiquidConduitRadiant",
				LiquidConduitTemperatureSensorConfig.ID,
				LiquidConduitElementSensorConfig.ID
			}, this);
			new Tech("LogicControl", new string[5]
			{
				"LogicWire",
				"LogicDuplicantSensor",
				LogicSwitchConfig.ID,
				"LogicWireBridge",
				"AutomationOverlay"
			}, this);
			new Tech("GenericSensors", new string[6]
			{
				LogicTimeOfDaySensorConfig.ID,
				LogicTimerSensorConfig.ID,
				"FloorSwitch",
				LogicElementSensorGasConfig.ID,
				LogicElementSensorLiquidConfig.ID,
				"LogicGateNOT"
			}, this);
			new Tech("LogicCircuits", new string[4]
			{
				"LogicGateAND",
				"LogicGateOR",
				"LogicGateBUFFER",
				"LogicGateFILTER"
			}, this);
			new Tech("ParallelAutomation", new string[4]
			{
				"LogicRibbon",
				"LogicRibbonBridge",
				LogicRibbonWriterConfig.ID,
				LogicRibbonReaderConfig.ID
			}, this);
			new Tech("DupeTrafficControl", new string[6]
			{
				"Checkpoint",
				LogicMemoryConfig.ID,
				"ArcadeMachine",
				"CosmicResearchCenter",
				"LogicGateXOR",
				LogicCounterConfig.ID
			}, this);
			new Tech("Multiplexing", new string[2]
			{
				"LogicGateMultiplexer",
				"LogicGateDemultiplexer"
			}, this);
			new Tech("SkyDetectors", new string[3]
			{
				CometDetectorConfig.ID,
				"Telescope",
				"AstronautTrainingCenter"
			}, this);
			new Tech("TravelTubes", new string[4]
			{
				"TravelTubeEntrance",
				"TravelTube",
				"TravelTubeWallBridge",
				"VerticalWindTunnel"
			}, this);
			new Tech("SmartStorage", new string[4]
			{
				"StorageLockerSmart",
				"SolidTransferArm",
				"ObjectDispenser",
				"ConveyorOverlay"
			}, this);
			new Tech("SolidTransport", new string[8]
			{
				"SolidConduit",
				"SolidConduitBridge",
				"SolidConduitInbox",
				"SolidConduitOutbox",
				"SolidVent",
				"SolidLogicValve",
				"ModularLaunchpadPortSolid",
				"SolidCargoBaySmall"
			}, this);
			new Tech("SolidManagement", new string[4]
			{
				"SolidFilter",
				SolidConduitTemperatureSensorConfig.ID,
				SolidConduitElementSensorConfig.ID,
				SolidConduitDiseaseSensorConfig.ID
			}, this);
			new Tech("BasicRocketry", new string[4]
			{
				"CommandModule",
				"SteamEngine",
				"ResearchModule",
				"Gantry"
			}, this);
			new Tech("CargoI", new string[1]
			{
				"CargoBay"
			}, this);
			new Tech("CargoII", new string[2]
			{
				"LiquidCargoBay",
				"GasCargoBay"
			}, this);
			new Tech("CargoIII", new string[3]
			{
				"TouristModule",
				"SpecialCargoBay",
				"ScannerModule"
			}, this);
			new Tech("EnginesI", new string[1]
			{
				"SolidBooster"
			}, this);
			new Tech("EnginesII", new string[3]
			{
				"KeroseneEngine",
				"LiquidFuelTank",
				"OxidizerTank"
			}, this);
			new Tech("EnginesIII", new string[2]
			{
				"OxidizerTankLiquid",
				"HydrogenEngine"
			}, this);
			new Tech("Jetpacks", new string[3]
			{
				"JetSuit",
				"JetSuitMarker",
				"JetSuitLocker"
			}, this);
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
					tECH_TIER.RemoveAll((Tuple<string, float> m) => m.first == "delta");
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
					resource.costsByResearchTypeID.Add(item3.first, item3.second);
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
