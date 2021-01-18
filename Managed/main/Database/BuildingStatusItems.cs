using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class BuildingStatusItems : StatusItems
	{
		public StatusItem MissingRequirements;

		public StatusItem GettingReady;

		public StatusItem Working;

		public MaterialsStatusItem MaterialsUnavailable;

		public MaterialsStatusItem MaterialsUnavailableForRefill;

		public StatusItem AngerDamage;

		public StatusItem ClinicOutsideHospital;

		public StatusItem DigUnreachable;

		public StatusItem MopUnreachable;

		public StatusItem StorageUnreachable;

		public StatusItem PassengerModuleUnreachable;

		public StatusItem ConstructableDigUnreachable;

		public StatusItem ConstructionUnreachable;

		public StatusItem NewDuplicantsAvailable;

		public StatusItem NeedPlant;

		public StatusItem NeedPower;

		public StatusItem NotEnoughPower;

		public StatusItem PowerLoopDetected;

		public StatusItem NeedLiquidIn;

		public StatusItem NeedGasIn;

		public StatusItem NeedResourceMass;

		public StatusItem NeedSolidIn;

		public StatusItem NeedLiquidOut;

		public StatusItem NeedGasOut;

		public StatusItem NeedSolidOut;

		public StatusItem InvalidBuildingLocation;

		public StatusItem PendingDeconstruction;

		public StatusItem PendingSwitchToggle;

		public StatusItem GasVentObstructed;

		public StatusItem LiquidVentObstructed;

		public StatusItem LiquidPipeEmpty;

		public StatusItem LiquidPipeObstructed;

		public StatusItem GasPipeEmpty;

		public StatusItem GasPipeObstructed;

		public StatusItem SolidPipeObstructed;

		public StatusItem PartiallyDamaged;

		public StatusItem Broken;

		public StatusItem PendingRepair;

		public StatusItem PendingUpgrade;

		public StatusItem RequiresSkillPerk;

		public StatusItem DigRequiresSkillPerk;

		public StatusItem ColonyLacksRequiredSkillPerk;

		public StatusItem WorkRequiresMinion;

		public StatusItem PendingWork;

		public StatusItem Flooded;

		public StatusItem PowerButtonOff;

		public StatusItem SwitchStatusActive;

		public StatusItem SwitchStatusInactive;

		public StatusItem LogicSwitchStatusActive;

		public StatusItem LogicSwitchStatusInactive;

		public StatusItem LogicSensorStatusActive;

		public StatusItem LogicSensorStatusInactive;

		public StatusItem ChangeDoorControlState;

		public StatusItem CurrentDoorControlState;

		public StatusItem Entombed;

		public MaterialsStatusItem WaitingForMaterials;

		public StatusItem WaitingForHighEnergyParticles;

		public StatusItem WaitingForRepairMaterials;

		public StatusItem MissingFoundation;

		public StatusItem NeutroniumUnminable;

		public StatusItem NoStorageFilterSet;

		public StatusItem PendingFish;

		public StatusItem NoFishableWaterBelow;

		public StatusItem GasVentOverPressure;

		public StatusItem LiquidVentOverPressure;

		public StatusItem NoWireConnected;

		public StatusItem NoLogicWireConnected;

		public StatusItem NoTubeConnected;

		public StatusItem NoTubeExits;

		public StatusItem StoredCharge;

		public StatusItem NoPowerConsumers;

		public StatusItem PressureOk;

		public StatusItem UnderPressure;

		public StatusItem AssignedTo;

		public StatusItem Unassigned;

		public StatusItem AssignedPublic;

		public StatusItem AssignedToRoom;

		public StatusItem RationBoxContents;

		public StatusItem ConduitBlocked;

		public StatusItem OutputPipeFull;

		public StatusItem ConduitBlockedMultiples;

		public StatusItem MeltingDown;

		public StatusItem UnderConstruction;

		public StatusItem UnderConstructionNoWorker;

		public StatusItem Normal;

		public StatusItem ManualGeneratorChargingUp;

		public StatusItem ManualGeneratorReleasingEnergy;

		public StatusItem GeneratorOffline;

		public StatusItem Pipe;

		public StatusItem Conveyor;

		public StatusItem FabricatorIdle;

		public StatusItem FabricatorEmpty;

		public StatusItem FlushToilet;

		public StatusItem FlushToiletInUse;

		public StatusItem Toilet;

		public StatusItem ToiletNeedsEmptying;

		public StatusItem DesalinatorNeedsEmptying;

		public StatusItem Unusable;

		public StatusItem NoResearchSelected;

		public StatusItem NoApplicableResearchSelected;

		public StatusItem NoApplicableAnalysisSelected;

		public StatusItem NoResearchOrDestinationSelected;

		public StatusItem Researching;

		public StatusItem ValveRequest;

		public StatusItem EmittingLight;

		public StatusItem EmittingElement;

		public StatusItem EmittingOxygenAvg;

		public StatusItem EmittingGasAvg;

		public StatusItem PumpingLiquidOrGas;

		public StatusItem NoLiquidElementToPump;

		public StatusItem NoGasElementToPump;

		public StatusItem PipeFull;

		public StatusItem PipeMayMelt;

		public StatusItem ElementConsumer;

		public StatusItem ElementEmitterOutput;

		public StatusItem AwaitingWaste;

		public StatusItem AwaitingCompostFlip;

		public StatusItem JoulesAvailable;

		public StatusItem Wattage;

		public StatusItem SolarPanelWattage;

		public StatusItem SteamTurbineWattage;

		public StatusItem Wattson;

		public StatusItem WireConnected;

		public StatusItem WireNominal;

		public StatusItem WireDisconnected;

		public StatusItem Cooling;

		public StatusItem CoolingStalledHotEnv;

		public StatusItem CoolingStalledColdGas;

		public StatusItem CoolingStalledHotLiquid;

		public StatusItem CoolingStalledColdLiquid;

		public StatusItem CannotCoolFurther;

		public StatusItem NeedsValidRegion;

		public StatusItem NeedSeed;

		public StatusItem AwaitingSeedDelivery;

		public StatusItem AwaitingBaitDelivery;

		public StatusItem NoAvailableSeed;

		public StatusItem NeedEgg;

		public StatusItem AwaitingEggDelivery;

		public StatusItem NoAvailableEgg;

		public StatusItem Grave;

		public StatusItem GraveEmpty;

		public StatusItem NoFilterElementSelected;

		public StatusItem NoLureElementSelected;

		public StatusItem BuildingDisabled;

		public StatusItem Overheated;

		public StatusItem Overloaded;

		public StatusItem LogicOverloaded;

		public StatusItem Expired;

		public StatusItem PumpingStation;

		public StatusItem EmptyPumpingStation;

		public StatusItem GeneShuffleCompleted;

		public StatusItem GeneticAnalysisCompleted;

		public StatusItem DirectionControl;

		public StatusItem WellPressurizing;

		public StatusItem WellOverpressure;

		public StatusItem ReleasingPressure;

		public StatusItem NoSuitMarker;

		public StatusItem SuitMarkerWrongSide;

		public StatusItem SuitMarkerTraversalAnytime;

		public StatusItem SuitMarkerTraversalOnlyWhenRoomAvailable;

		public StatusItem TooCold;

		public StatusItem NotInAnyRoom;

		public StatusItem NotInRequiredRoom;

		public StatusItem NotInRecommendedRoom;

		public StatusItem IncubatorProgress;

		public StatusItem HabitatNeedsEmptying;

		public StatusItem DetectorScanning;

		public StatusItem IncomingMeteors;

		public StatusItem HasGantry;

		public StatusItem MissingGantry;

		public StatusItem DisembarkingDuplicant;

		public StatusItem RocketName;

		public StatusItem PathNotClear;

		public StatusItem InvalidPortOverlap;

		public StatusItem EmergencyPriority;

		public StatusItem SkillPointsAvailable;

		public StatusItem Baited;

		public StatusItem NoCoolant;

		public StatusItem TanningLightSufficient;

		public StatusItem TanningLightInsufficient;

		public StatusItem HotTubWaterTooCold;

		public StatusItem HotTubTooHot;

		public StatusItem HotTubFilling;

		public StatusItem WindTunnelIntake;

		public StatusItem WarpPortalCharging;

		public StatusItem WarpConduitPartnerDisabled;

		public StatusItem InOrbit;

		public StatusItem InFlight;

		public StatusItem DestinationOutOfRange;

		public StatusItem RocketStranded;

		public StatusItem RailgunpayloadNeedsEmptying;

		public StatusItem AwaitingEmptyBuilding;

		public StatusItem DuplicantActivationRequired;

		public StatusItem RocketChecklistIncomplete;

		public StatusItem RocketCargoEmptying;

		public StatusItem RocketCargoFilling;

		public StatusItem RocketCargoFull;

		public StatusItem PilotNeeded;

		public StatusItem AutoPilotActive;

		public StatusItem InvalidMaskStationConsumptionState;

		public StatusItem ClusterTelescopeAllWorkComplete;

		public BuildingStatusItems(ResourceSet parent)
			: base("BuildingStatusItems", parent)
		{
			CreateStatusItems();
		}

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 129022)
		{
			return Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 129022)
		{
			return Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
		}

		private void CreateStatusItems()
		{
			AngerDamage = CreateStatusItem("AngerDamage", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			AssignedTo = CreateStatusItem("AssignedTo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AssignedTo.resolveStringCallback = delegate(string str, object data)
			{
				Assignable assignable2 = (Assignable)data;
				IAssignableIdentity assignee2 = assignable2.assignee;
				if (assignee2 != null)
				{
					string properName2 = assignee2.GetProperName();
					str = str.Replace("{Assignee}", properName2);
				}
				return str;
			};
			AssignedToRoom = CreateStatusItem("AssignedToRoom", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AssignedToRoom.resolveStringCallback = delegate(string str, object data)
			{
				Assignable assignable = (Assignable)data;
				IAssignableIdentity assignee = assignable.assignee;
				if (assignee != null)
				{
					string properName = assignee.GetProperName();
					str = str.Replace("{Assignee}", properName);
				}
				return str;
			};
			Broken = CreateStatusItem("Broken", "BUILDING", "status_item_broken", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Broken.resolveStringCallback = delegate(string str, object data)
			{
				BuildingHP.SMInstance sMInstance3 = (BuildingHP.SMInstance)data;
				return str.Replace("{DamageInfo}", sMInstance3.master.GetDamageSourceInfo().ToString());
			};
			Broken.conditionalOverlayCallback = ShowInUtilityOverlay;
			ChangeDoorControlState = CreateStatusItem("ChangeDoorControlState", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ChangeDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door2 = (Door)data;
				return str.Replace("{ControlState}", door2.RequestedState.ToString());
			};
			CurrentDoorControlState = CreateStatusItem("CurrentDoorControlState", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			CurrentDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door = (Door)data;
				string newValue13 = Strings.Get("STRINGS.BUILDING.STATUSITEMS.CURRENTDOORCONTROLSTATE." + door.CurrentState.ToString().ToUpper());
				return str.Replace("{ControlState}", newValue13);
			};
			ClinicOutsideHospital = CreateStatusItem("ClinicOutsideHospital", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ConduitBlocked = CreateStatusItem("ConduitBlocked", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			OutputPipeFull = CreateStatusItem("OutputPipeFull", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ConstructionUnreachable = CreateStatusItem("ConstructionUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ConduitBlockedMultiples = CreateStatusItem("ConduitBlocked", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			DigUnreachable = CreateStatusItem("DigUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			MopUnreachable = CreateStatusItem("MopUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			StorageUnreachable = CreateStatusItem("StorageUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			PassengerModuleUnreachable = CreateStatusItem("PassengerModuleUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DirectionControl = CreateStatusItem("DirectionControl", BUILDING.STATUSITEMS.DIRECTION_CONTROL.NAME, BUILDING.STATUSITEMS.DIRECTION_CONTROL.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			DirectionControl.resolveStringCallback = delegate(string str, object data)
			{
				DirectionControl directionControl = (DirectionControl)data;
				string newValue12 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.BOTH;
				switch (directionControl.allowedDirection)
				{
				case WorkableReactable.AllowedDirection.Left:
					newValue12 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.LEFT;
					break;
				case WorkableReactable.AllowedDirection.Right:
					newValue12 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.RIGHT;
					break;
				}
				str = str.Replace("{Direction}", newValue12);
				return str;
			};
			ConstructableDigUnreachable = CreateStatusItem("ConstructableDigUnreachable", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Entombed = CreateStatusItem("Entombed", "BUILDING", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Entombed.AddNotification();
			Flooded = CreateStatusItem("Flooded", "BUILDING", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Flooded.AddNotification();
			GasVentObstructed = CreateStatusItem("GasVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID);
			GasVentOverPressure = CreateStatusItem("GasVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID);
			GeneShuffleCompleted = CreateStatusItem("GeneShuffleCompleted", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			GeneticAnalysisCompleted = CreateStatusItem("GeneticAnalysisCompleted", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			InvalidBuildingLocation = CreateStatusItem("InvalidBuildingLocation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			LiquidVentObstructed = CreateStatusItem("LiquidVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			LiquidVentOverPressure = CreateStatusItem("LiquidVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			MaterialsUnavailable = new MaterialsStatusItem("MaterialsUnavailable", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			MaterialsUnavailable.AddNotification();
			MaterialsUnavailable.resolveStringCallback = delegate(string str, object data)
			{
				string text7 = "";
				Dictionary<Tag, float> dictionary = null;
				if (data is IFetchList)
				{
					IFetchList fetchList3 = (IFetchList)data;
					dictionary = fetchList3.GetRemainingMinimum();
				}
				else if (data is Dictionary<Tag, float>)
				{
					dictionary = data as Dictionary<Tag, float>;
				}
				if (dictionary.Count > 0)
				{
					bool flag4 = true;
					foreach (KeyValuePair<Tag, float> item in dictionary)
					{
						if (item.Value != 0f)
						{
							if (!flag4)
							{
								text7 += "\n";
							}
							text7 = ((!Assets.IsTagCountable(item.Key)) ? (text7 + string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_MASS, item.Key.ProperName(), GameUtil.GetFormattedMass(item.Value))) : (text7 + string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(item.Key.ProperName(), item.Value))));
							flag4 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text7);
				return str;
			};
			MaterialsUnavailableForRefill = new MaterialsStatusItem("MaterialsUnavailableForRefill", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: true, OverlayModes.None.ID);
			MaterialsUnavailableForRefill.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList2 = (IFetchList)data;
				string text6 = "";
				Dictionary<Tag, float> remaining2 = fetchList2.GetRemaining();
				if (remaining2.Count > 0)
				{
					bool flag3 = true;
					foreach (KeyValuePair<Tag, float> item2 in remaining2)
					{
						if (item2.Value != 0f)
						{
							if (!flag3)
							{
								text6 += "\n";
							}
							text6 += string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLEFORREFILL.LINE_ITEM, item2.Key.ProperName());
							flag3 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text6);
				return str;
			};
			Func<string, object, string> resolveStringCallback = delegate(string str, object data)
			{
				RoomType roomType = Db.Get().RoomTypes.Get((string)data);
				return (roomType != null) ? string.Format(str, roomType.Name) : str;
			};
			NoCoolant = CreateStatusItem("NoCoolant", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NotInAnyRoom = CreateStatusItem("NotInAnyRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NotInRequiredRoom = CreateStatusItem("NotInRequiredRoom", "BUILDING", "status_item_room_required", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NotInRequiredRoom.resolveStringCallback = resolveStringCallback;
			NotInRecommendedRoom = CreateStatusItem("NotInRecommendedRoom", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NotInRecommendedRoom.resolveStringCallback = resolveStringCallback;
			WaitingForRepairMaterials = CreateStatusItem("WaitingForRepairMaterials", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID, showWorldIcon: false);
			WaitingForRepairMaterials.resolveStringCallback = delegate(string str, object data)
			{
				KeyValuePair<Tag, float> keyValuePair = (KeyValuePair<Tag, float>)data;
				if (keyValuePair.Value != 0f)
				{
					string newValue11 = string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, keyValuePair.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair.Value));
					str = str.Replace("{ItemsRemaining}", newValue11);
				}
				return str;
			};
			WaitingForMaterials = new MaterialsStatusItem("WaitingForMaterials", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			WaitingForMaterials.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList = (IFetchList)data;
				string text5 = "";
				Dictionary<Tag, float> remaining = fetchList.GetRemaining();
				if (remaining.Count > 0)
				{
					bool flag2 = true;
					foreach (KeyValuePair<Tag, float> item3 in remaining)
					{
						if (item3.Value != 0f)
						{
							if (!flag2)
							{
								text5 += "\n";
							}
							text5 = ((!Assets.IsTagCountable(item3.Key)) ? (text5 + string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, item3.Key.ProperName(), GameUtil.GetFormattedMass(item3.Value))) : (text5 + string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(item3.Key.ProperName(), item3.Value))));
							flag2 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text5);
				return str;
			};
			WaitingForHighEnergyParticles = new StatusItem("WaitingForRadiation", "BUILDING", "status_item_need_high_energy_particles", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			MeltingDown = CreateStatusItem("MeltingDown", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			MissingFoundation = CreateStatusItem("MissingFoundation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeutroniumUnminable = CreateStatusItem("NeutroniumUnminable", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeedGasIn = CreateStatusItem("NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID);
			NeedGasIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple2 = (Tuple<ConduitType, Tag>)data;
				string newValue10 = string.Format(BUILDING.STATUSITEMS.NEEDGASIN.LINE_ITEM, tuple2.second.ProperName());
				str = str.Replace("{GasRequired}", newValue10);
				return str;
			};
			NeedGasOut = CreateStatusItem("NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.GasConduits.ID);
			NeedLiquidIn = CreateStatusItem("NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			NeedLiquidIn.resolveStringCallback = delegate(string str, object data)
			{
				Tuple<ConduitType, Tag> tuple = (Tuple<ConduitType, Tag>)data;
				string newValue9 = string.Format(BUILDING.STATUSITEMS.NEEDLIQUIDIN.LINE_ITEM, tuple.second.ProperName());
				str = str.Replace("{LiquidRequired}", newValue9);
				return str;
			};
			NeedLiquidOut = CreateStatusItem("NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.LiquidConduits.ID);
			NeedSolidIn = CreateStatusItem("NeedSolidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.SolidConveyor.ID);
			NeedSolidOut = CreateStatusItem("NeedSolidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.SolidConveyor.ID);
			NeedResourceMass = CreateStatusItem("NeedResourceMass", "BUILDING", "status_item_need_resource", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeedResourceMass.resolveStringCallback = delegate(string str, object data)
			{
				string text4 = "";
				EnergyGenerator.Formula formula = (EnergyGenerator.Formula)data;
				if (formula.inputs.Length != 0)
				{
					bool flag = true;
					EnergyGenerator.InputItem[] inputs = formula.inputs;
					for (int j = 0; j < inputs.Length; j++)
					{
						EnergyGenerator.InputItem inputItem = inputs[j];
						if (!flag)
						{
							text4 += "\n";
							flag = false;
						}
						text4 += string.Format(BUILDING.STATUSITEMS.NEEDRESOURCEMASS.LINE_ITEM, inputItem.tag.ProperName());
					}
				}
				str = str.Replace("{ResourcesRequired}", text4);
				return str;
			};
			LiquidPipeEmpty = CreateStatusItem("LiquidPipeEmpty", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			LiquidPipeObstructed = CreateStatusItem("LiquidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.LiquidConduits.ID);
			GasPipeEmpty = CreateStatusItem("GasPipeEmpty", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID);
			GasPipeObstructed = CreateStatusItem("GasPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.GasConduits.ID);
			SolidPipeObstructed = CreateStatusItem("SolidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.SolidConveyor.ID);
			NeedPlant = CreateStatusItem("NeedPlant", "BUILDING", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeedPower = CreateStatusItem("NeedPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			NotEnoughPower = CreateStatusItem("NotEnoughPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			PowerLoopDetected = CreateStatusItem("PowerLoopDetected", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			NewDuplicantsAvailable = CreateStatusItem("NewDuplicantsAvailable", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NewDuplicantsAvailable.AddNotification();
			NewDuplicantsAvailable.notificationClickCallback = delegate
			{
				int idx = 0;
				for (int i = 0; i < Components.Telepads.Items.Count; i++)
				{
					KSelectable component6 = Components.Telepads[i].GetComponent<KSelectable>();
					if (component6.IsSelected)
					{
						idx = (i + 1) % Components.Telepads.Items.Count;
						break;
					}
				}
				Telepad targetTelepad = Components.Telepads[idx];
				int myWorldId = targetTelepad.GetMyWorldId();
				CameraController.Instance.ActiveWorldStarWipe(myWorldId, targetTelepad.transform.GetPosition(), delegate
				{
					SelectTool.Instance.Select(targetTelepad.GetComponent<KSelectable>());
				});
			};
			NoStorageFilterSet = CreateStatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoSuitMarker = CreateStatusItem("NoSuitMarker", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			SuitMarkerWrongSide = CreateStatusItem("suitMarkerWrongSide", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			SuitMarkerTraversalAnytime = CreateStatusItem("suitMarkerTraversalAnytime", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SuitMarkerTraversalOnlyWhenRoomAvailable = CreateStatusItem("suitMarkerTraversalOnlyWhenRoomAvailable", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NoFishableWaterBelow = CreateStatusItem("NoFishableWaterBelow", "BUILDING", "status_item_no_fishable_water_below", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoPowerConsumers = CreateStatusItem("NoPowerConsumers", "BUILDING", "status_item_no_power_consumers", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			NoWireConnected = CreateStatusItem("NoWireConnected", "BUILDING", "status_item_no_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: true, OverlayModes.Power.ID);
			NoLogicWireConnected = CreateStatusItem("NoLogicWireConnected", "BUILDING", "status_item_no_logic_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Logic.ID);
			NoTubeConnected = CreateStatusItem("NoTubeConnected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoTubeExits = CreateStatusItem("NoTubeExits", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			StoredCharge = CreateStatusItem("StoredCharge", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			StoredCharge.resolveStringCallback = delegate(string str, object data)
			{
				TravelTubeEntrance.SMInstance sMInstance2 = (TravelTubeEntrance.SMInstance)data;
				if (sMInstance2 != null)
				{
					str = string.Format(str, GameUtil.GetFormattedRoundedJoules(sMInstance2.master.AvailableJoules), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.TotalCapacity), GameUtil.GetFormattedRoundedJoules(sMInstance2.master.UsageJoules));
				}
				return str;
			};
			PendingDeconstruction = CreateStatusItem("PendingDeconstruction", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingDeconstruction.conditionalOverlayCallback = ShowInUtilityOverlay;
			PendingRepair = CreateStatusItem("PendingRepair", "BUILDING", "status_item_pending_repair", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			PendingRepair.resolveStringCallback = delegate(string str, object data)
			{
				Repairable.SMInstance sMInstance = (Repairable.SMInstance)data;
				BuildingHP component5 = sMInstance.master.GetComponent<BuildingHP>();
				return str.Replace("{DamageInfo}", component5.GetDamageSourceInfo().ToString());
			};
			PendingRepair.conditionalOverlayCallback = (HashedString mode, object data) => true;
			RequiresSkillPerk = CreateStatusItem("RequiresSkillPerk", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RequiresSkillPerk.resolveStringCallback = delegate(string str, object data)
			{
				string id = (string)data;
				SkillPerk perk = Db.Get().SkillPerks.Get(id);
				List<Skill> skillsWithPerk = Db.Get().Skills.GetSkillsWithPerk(perk);
				List<string> list = new List<string>();
				foreach (Skill item4 in skillsWithPerk)
				{
					list.Add(item4.Name);
				}
				str = str.Replace("{Skills}", string.Join(", ", list.ToArray()));
				return str;
			};
			DigRequiresSkillPerk = CreateStatusItem("DigRequiresSkillPerk", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			DigRequiresSkillPerk.resolveStringCallback = RequiresSkillPerk.resolveStringCallback;
			ColonyLacksRequiredSkillPerk = CreateStatusItem("ColonyLacksRequiredSkillPerk", "BUILDING", "status_item_role_required", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ColonyLacksRequiredSkillPerk.resolveStringCallback = RequiresSkillPerk.resolveStringCallback;
			WorkRequiresMinion = CreateStatusItem("WorkRequiresMinion", "BUILDING", "status_item_role_required", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			SwitchStatusActive = CreateStatusItem("SwitchStatusActive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SwitchStatusInactive = CreateStatusItem("SwitchStatusInactive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			LogicSwitchStatusActive = CreateStatusItem("LogicSwitchStatusActive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			LogicSwitchStatusInactive = CreateStatusItem("LogicSwitchStatusInactive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			LogicSensorStatusActive = CreateStatusItem("LogicSensorStatusActive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			LogicSensorStatusInactive = CreateStatusItem("LogicSensorStatusInactive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingFish = CreateStatusItem("PendingFish", "BUILDING", "status_item_pending_fish", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingSwitchToggle = CreateStatusItem("PendingSwitchToggle", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingUpgrade = CreateStatusItem("PendingUpgrade", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingWork = CreateStatusItem("PendingWork", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			PowerButtonOff = CreateStatusItem("PowerButtonOff", "BUILDING", "status_item_power_button_off", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			PressureOk = CreateStatusItem("PressureOk", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Oxygen.ID);
			UnderPressure = CreateStatusItem("UnderPressure", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Oxygen.ID);
			Unassigned = CreateStatusItem("Unassigned", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Rooms.ID);
			AssignedPublic = CreateStatusItem("AssignedPublic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Rooms.ID);
			UnderConstruction = CreateStatusItem("UnderConstruction", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			UnderConstructionNoWorker = CreateStatusItem("UnderConstructionNoWorker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Normal = CreateStatusItem("Normal", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ManualGeneratorChargingUp = CreateStatusItem("ManualGeneratorChargingUp", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			ManualGeneratorReleasingEnergy = CreateStatusItem("ManualGeneratorReleasingEnergy", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			GeneratorOffline = CreateStatusItem("GeneratorOffline", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			Pipe = CreateStatusItem("Pipe", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			Pipe.resolveStringCallback = delegate(string str, object data)
			{
				Conduit conduit = (Conduit)data;
				int cell2 = Grid.PosToCell(conduit);
				ConduitFlow flowManager = conduit.GetFlowManager();
				ConduitFlow.ConduitContents contents2 = flowManager.GetContents(cell2);
				string text3 = BUILDING.STATUSITEMS.PIPECONTENTS.EMPTY;
				if (contents2.mass > 0f)
				{
					Element element3 = ElementLoader.FindElementByHash(contents2.element);
					text3 = string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS, GameUtil.GetFormattedMass(contents2.mass), element3.name, GameUtil.GetFormattedTemperature(contents2.temperature));
					if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode == OverlayModes.Disease.ID && contents2.diseaseIdx != byte.MaxValue)
					{
						text3 += string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(contents2.diseaseIdx, contents2.diseaseCount, color: true));
					}
				}
				str = str.Replace("{Contents}", text3);
				return str;
			};
			Conveyor = CreateStatusItem("Conveyor", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.SolidConveyor.ID);
			Conveyor.resolveStringCallback = delegate(string str, object data)
			{
				SolidConduit cmp = (SolidConduit)data;
				int cell = Grid.PosToCell(cmp);
				SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
				SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
				string text2 = BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.EMPTY;
				if (contents.pickupableHandle.IsValid())
				{
					Pickupable pickupable = solidConduitFlow.GetPickupable(contents.pickupableHandle);
					if ((bool)pickupable)
					{
						PrimaryElement component4 = pickupable.GetComponent<PrimaryElement>();
						float mass = component4.Mass;
						if (mass > 0f)
						{
							text2 = string.Format(BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.CONTENTS, GameUtil.GetFormattedMass(mass), pickupable.GetProperName(), GameUtil.GetFormattedTemperature(component4.Temperature));
							if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode == OverlayModes.Disease.ID && component4.DiseaseIdx != byte.MaxValue)
							{
								text2 += string.Format(BUILDING.STATUSITEMS.CONVEYOR_CONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(component4.DiseaseIdx, component4.DiseaseCount, color: true));
							}
						}
					}
				}
				str = str.Replace("{Contents}", text2);
				return str;
			};
			FabricatorIdle = CreateStatusItem("FabricatorIdle", "BUILDING", "status_item_fabricator_select", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			FabricatorEmpty = CreateStatusItem("FabricatorEmpty", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Toilet = CreateStatusItem("Toilet", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Toilet.resolveStringCallback = delegate(string str, object data)
			{
				Toilet.StatesInstance statesInstance5 = (Toilet.StatesInstance)data;
				if (statesInstance5 != null)
				{
					str = str.Replace("{FlushesRemaining}", statesInstance5.GetFlushesRemaining().ToString());
				}
				return str;
			};
			ToiletNeedsEmptying = CreateStatusItem("ToiletNeedsEmptying", "BUILDING", "status_item_toilet_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DesalinatorNeedsEmptying = CreateStatusItem("DesalinatorNeedsEmptying", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Unusable = CreateStatusItem("Unusable", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoResearchSelected = CreateStatusItem("NoResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoResearchSelected.AddNotification();
			StatusItem noResearchSelected = NoResearchSelected;
			noResearchSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noResearchSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				string newValue8 = GameInputMapping.FindEntry(Action.ManageResearch).mKeyCode.ToString();
				str = str.Replace("{RESEARCH_MENU_KEY}", newValue8);
				return str;
			});
			NoResearchSelected.notificationClickCallback = delegate
			{
				ManagementMenu.Instance.OpenResearch();
			};
			NoApplicableResearchSelected = CreateStatusItem("NoApplicableResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoApplicableResearchSelected.AddNotification();
			NoApplicableAnalysisSelected = CreateStatusItem("NoApplicableAnalysisSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoApplicableAnalysisSelected.AddNotification();
			StatusItem noApplicableAnalysisSelected = NoApplicableAnalysisSelected;
			noApplicableAnalysisSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noApplicableAnalysisSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				string newValue7 = GameInputMapping.FindEntry(Action.ManageStarmap).mKeyCode.ToString();
				str = str.Replace("{STARMAP_MENU_KEY}", newValue7);
				return str;
			});
			NoApplicableAnalysisSelected.notificationClickCallback = delegate
			{
				ManagementMenu.Instance.OpenStarmap();
			};
			NoResearchOrDestinationSelected = CreateStatusItem("NoResearchOrDestinationSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			StatusItem noResearchOrDestinationSelected = NoResearchOrDestinationSelected;
			noResearchOrDestinationSelected.resolveTooltipCallback = (Func<string, object, string>)Delegate.Combine(noResearchOrDestinationSelected.resolveTooltipCallback, (Func<string, object, string>)delegate(string str, object data)
			{
				string newValue5 = GameInputMapping.FindEntry(Action.ManageStarmap).mKeyCode.ToString();
				str = str.Replace("{STARMAP_MENU_KEY}", newValue5);
				string newValue6 = GameInputMapping.FindEntry(Action.ManageResearch).mKeyCode.ToString();
				str = str.Replace("{RESEARCH_MENU_KEY}", newValue6);
				return str;
			});
			NoResearchOrDestinationSelected.AddNotification();
			ValveRequest = CreateStatusItem("ValveRequest", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ValveRequest.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve = (Valve)data;
				str = str.Replace("{QueuedMaxFlow}", GameUtil.GetFormattedMass(valve.QueuedMaxFlow, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			EmittingLight = CreateStatusItem("EmittingLight", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			EmittingLight.resolveStringCallback = delegate(string str, object data)
			{
				string newValue4 = GameInputMapping.FindEntry(Action.Overlay5).mKeyCode.ToString();
				str = str.Replace("{LightGridOverlay}", newValue4);
				return str;
			};
			RationBoxContents = CreateStatusItem("RationBoxContents", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RationBoxContents.resolveStringCallback = delegate(string str, object data)
			{
				RationBox rationBox = (RationBox)data;
				if (rationBox == null)
				{
					return str;
				}
				Storage component2 = rationBox.GetComponent<Storage>();
				if (component2 == null)
				{
					return str;
				}
				float num2 = 0f;
				foreach (GameObject item5 in component2.items)
				{
					Edible component3 = item5.GetComponent<Edible>();
					if ((bool)component3)
					{
						num2 += component3.Calories;
					}
				}
				str = str.Replace("{Stored}", GameUtil.GetFormattedCalories(num2));
				return str;
			};
			EmittingElement = CreateStatusItem("EmittingElement", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			EmittingElement.resolveStringCallback = delegate(string str, object data)
			{
				IElementEmitter elementEmitter2 = (IElementEmitter)data;
				string newValue3 = ElementLoader.FindElementByHash(elementEmitter2.Element).tag.ProperName();
				str = str.Replace("{ElementType}", newValue3);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter2.AverageEmitRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			EmittingOxygenAvg = CreateStatusItem("EmittingOxygenAvg", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			EmittingOxygenAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates2 = (Sublimates)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates2.AvgFlowRate(), GameUtil.TimeSlice.PerSecond));
				return str;
			};
			EmittingGasAvg = CreateStatusItem("EmittingGasAvg", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			EmittingGasAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates = (Sublimates)data;
				str = str.Replace("{Element}", ElementLoader.FindElementByHash(sublimates.info.sublimatedElement).name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates.AvgFlowRate(), GameUtil.TimeSlice.PerSecond));
				return str;
			};
			PumpingLiquidOrGas = CreateStatusItem("PumpingLiquidOrGas", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			PumpingLiquidOrGas.resolveStringCallback = delegate(string str, object data)
			{
				HandleVector<int>.Handle handle = (HandleVector<int>.Handle)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(handle);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(averageRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			PipeMayMelt = CreateStatusItem("PipeMayMelt", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoLiquidElementToPump = CreateStatusItem("NoLiquidElementToPump", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.LiquidConduits.ID);
			NoGasElementToPump = CreateStatusItem("NoGasElementToPump", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.GasConduits.ID);
			NoFilterElementSelected = CreateStatusItem("NoFilterElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoLureElementSelected = CreateStatusItem("NoLureElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ElementConsumer = CreateStatusItem("ElementConsumer", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			ElementConsumer.resolveStringCallback = delegate(string str, object data)
			{
				ElementConsumer elementConsumer = (ElementConsumer)data;
				string newValue2 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag.ProperName();
				str = str.Replace("{ElementTypes}", newValue2);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementConsumer.AverageConsumeRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			ElementEmitterOutput = CreateStatusItem("ElementEmitterOutput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			ElementEmitterOutput.resolveStringCallback = delegate(string str, object data)
			{
				ElementEmitter elementEmitter = (ElementEmitter)data;
				if (elementEmitter != null)
				{
					str = str.Replace("{ElementTypes}", elementEmitter.outputElement.Name);
					str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter.outputElement.massGenerationRate / elementEmitter.emissionFrequency, GameUtil.TimeSlice.PerSecond));
				}
				return str;
			};
			AwaitingWaste = CreateStatusItem("AwaitingWaste", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			AwaitingCompostFlip = CreateStatusItem("AwaitingCompostFlip", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID);
			JoulesAvailable = CreateStatusItem("JoulesAvailable", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			JoulesAvailable.resolveStringCallback = delegate(string str, object data)
			{
				Battery battery = (Battery)data;
				str = str.Replace("{JoulesAvailable}", GameUtil.GetFormattedJoules(battery.JoulesAvailable));
				str = str.Replace("{JoulesCapacity}", GameUtil.GetFormattedJoules(battery.Capacity));
				return str;
			};
			Wattage = CreateStatusItem("Wattage", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			Wattage.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator = (Generator)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator.WattageRating));
				return str;
			};
			SolarPanelWattage = CreateStatusItem("SolarPanelWattage", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			SolarPanelWattage.resolveStringCallback = delegate(string str, object data)
			{
				SolarPanel solarPanel = (SolarPanel)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(solarPanel.CurrentWattage));
				return str;
			};
			SteamTurbineWattage = CreateStatusItem("SteamTurbineWattage", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			SteamTurbineWattage.resolveStringCallback = delegate(string str, object data)
			{
				SteamTurbine steamTurbine = (SteamTurbine)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(steamTurbine.CurrentWattage));
				return str;
			};
			Wattson = CreateStatusItem("Wattson", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Wattson.resolveStringCallback = delegate(string str, object data)
			{
				Telepad telepad = (Telepad)data;
				str = ((GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver()) ? ((string)BUILDING.STATUSITEMS.WATTSONGAMEOVER.NAME) : ((!telepad.GetComponent<Operational>().IsOperational) ? str.Replace("{TimeRemaining}", BUILDING.STATUSITEMS.WATTSON.UNAVAILABLE) : str.Replace("{TimeRemaining}", GameUtil.GetFormattedCycles(telepad.GetTimeRemaining()))));
				return str;
			};
			FlushToilet = CreateStatusItem("FlushToilet", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			FlushToiletInUse = CreateStatusItem("FlushToiletInUse", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			WireNominal = CreateStatusItem("WireNominal", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			WireConnected = CreateStatusItem("WireConnected", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.Power.ID);
			WireDisconnected = CreateStatusItem("WireDisconnected", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.Power.ID);
			Overheated = CreateStatusItem("Overheated", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Overloaded = CreateStatusItem("Overloaded", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			LogicOverloaded = CreateStatusItem("LogicOverloaded", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Cooling = CreateStatusItem("Cooling", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Func<string, object, string> resolveStringCallback2 = delegate(string str, object data)
			{
				AirConditioner airConditioner2 = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner2.lastGasTemp));
			};
			CoolingStalledColdGas = CreateStatusItem("CoolingStalledColdGas", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			CoolingStalledColdGas.resolveStringCallback = resolveStringCallback2;
			CoolingStalledColdLiquid = CreateStatusItem("CoolingStalledColdLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			CoolingStalledColdLiquid.resolveStringCallback = resolveStringCallback2;
			Func<string, object, string> resolveStringCallback3 = delegate(string str, object data)
			{
				AirConditioner airConditioner = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner.lastEnvTemp), GameUtil.GetFormattedTemperature(airConditioner.lastGasTemp), GameUtil.GetFormattedTemperature(airConditioner.maxEnvironmentDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative));
			};
			CoolingStalledHotEnv = CreateStatusItem("CoolingStalledHotEnv", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			CoolingStalledHotEnv.resolveStringCallback = resolveStringCallback3;
			CoolingStalledHotLiquid = CreateStatusItem("CoolingStalledHotLiquid", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			CoolingStalledHotLiquid.resolveStringCallback = resolveStringCallback3;
			MissingRequirements = CreateStatusItem("MissingRequirements", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			GettingReady = CreateStatusItem("GettingReady", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Working = CreateStatusItem("Working", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NeedsValidRegion = CreateStatusItem("NeedsValidRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NeedSeed = CreateStatusItem("NeedSeed", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			AwaitingSeedDelivery = CreateStatusItem("AwaitingSeedDelivery", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AwaitingBaitDelivery = CreateStatusItem("AwaitingBaitDelivery", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NoAvailableSeed = CreateStatusItem("NoAvailableSeed", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeedEgg = CreateStatusItem("NeedEgg", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			AwaitingEggDelivery = CreateStatusItem("AwaitingEggDelivery", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NoAvailableEgg = CreateStatusItem("NoAvailableEgg", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Grave = CreateStatusItem("Grave", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Grave.resolveStringCallback = delegate(string str, object data)
			{
				Grave.StatesInstance statesInstance4 = (Grave.StatesInstance)data;
				string text = str.Replace("{DeadDupe}", statesInstance4.master.graveName);
				string[] strings = LocString.GetStrings(typeof(NAMEGEN.GRAVE.EPITAPHS));
				int num = statesInstance4.master.epitaphIdx % strings.Length;
				return text.Replace("{Epitaph}", strings[num]);
			};
			GraveEmpty = CreateStatusItem("GraveEmpty", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			CannotCoolFurther = CreateStatusItem("CannotCoolFurther", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			BuildingDisabled = CreateStatusItem("BuildingDisabled", "BUILDING", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Expired = CreateStatusItem("Expired", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PumpingStation = CreateStatusItem("PumpingStation", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PumpingStation.resolveStringCallback = delegate(string str, object data)
			{
				LiquidPumpingStation liquidPumpingStation = (LiquidPumpingStation)data;
				return (liquidPumpingStation != null) ? liquidPumpingStation.ResolveString(str) : str;
			};
			EmptyPumpingStation = CreateStatusItem("EmptyPumpingStation", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			WellPressurizing = CreateStatusItem("WellPressurizing", BUILDING.STATUSITEMS.WELL_PRESSURIZING.NAME, BUILDING.STATUSITEMS.WELL_PRESSURIZING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			WellPressurizing.resolveStringCallback = delegate(string str, object data)
			{
				OilWellCap.StatesInstance statesInstance3 = (OilWellCap.StatesInstance)data;
				return (statesInstance3 != null) ? string.Format(str, GameUtil.GetFormattedPercent(100f * statesInstance3.GetPressurePercent())) : str;
			};
			WellOverpressure = CreateStatusItem("WellOverpressure", BUILDING.STATUSITEMS.WELL_OVERPRESSURE.NAME, BUILDING.STATUSITEMS.WELL_OVERPRESSURE.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ReleasingPressure = CreateStatusItem("ReleasingPressure", BUILDING.STATUSITEMS.RELEASING_PRESSURE.NAME, BUILDING.STATUSITEMS.RELEASING_PRESSURE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			TooCold = CreateStatusItem("TooCold", BUILDING.STATUSITEMS.TOO_COLD.NAME, BUILDING.STATUSITEMS.TOO_COLD.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			IncubatorProgress = CreateStatusItem("IncubatorProgress", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			IncubatorProgress.resolveStringCallback = delegate(string str, object data)
			{
				EggIncubator eggIncubator = (EggIncubator)data;
				str = str.Replace("{Percent}", GameUtil.GetFormattedPercent(eggIncubator.GetProgress() * 100f));
				return str;
			};
			HabitatNeedsEmptying = CreateStatusItem("HabitatNeedsEmptying", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DetectorScanning = CreateStatusItem("DetectorScanning", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			IncomingMeteors = CreateStatusItem("IncomingMeteors", "BUILDING", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			HasGantry = CreateStatusItem("HasGantry", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			MissingGantry = CreateStatusItem("MissingGantry", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DisembarkingDuplicant = CreateStatusItem("DisembarkingDuplicant", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			RocketName = CreateStatusItem("RocketName", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RocketName.resolveStringCallback = delegate(string str, object data)
			{
				RocketModule rocketModule2 = (RocketModule)data;
				return (rocketModule2 != null) ? str.Replace("{0}", rocketModule2.GetParentRocketName()) : str;
			};
			RocketName.resolveTooltipCallback = delegate(string str, object data)
			{
				RocketModule rocketModule = (RocketModule)data;
				return (rocketModule != null) ? str.Replace("{0}", rocketModule.GetParentRocketName()) : str;
			};
			PathNotClear = new StatusItem("PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			PathNotClear.resolveTooltipCallback = delegate(string str, object data)
			{
				ConditionFlightPathIsClear conditionFlightPathIsClear = (ConditionFlightPathIsClear)data;
				if (conditionFlightPathIsClear != null)
				{
					str = string.Format(str, conditionFlightPathIsClear.GetObstruction());
				}
				return str;
			};
			InvalidPortOverlap = CreateStatusItem("InvalidPortOverlap", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			InvalidPortOverlap.AddNotification();
			EmergencyPriority = CreateStatusItem("EmergencyPriority", BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NAME, BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.TOOLTIP, "status_item_doubleexclamation", StatusItem.IconType.Custom, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			EmergencyPriority.AddNotification(null, BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NOTIFICATION_NAME, BUILDING.STATUSITEMS.TOP_PRIORITY_CHORE.NOTIFICATION_TOOLTIP);
			SkillPointsAvailable = CreateStatusItem("SkillPointsAvailable", BUILDING.STATUSITEMS.SKILL_POINTS_AVAILABLE.NAME, BUILDING.STATUSITEMS.SKILL_POINTS_AVAILABLE.TOOLTIP, "status_item_jobs", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Baited = CreateStatusItem("Baited", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Baited.resolveStringCallback = delegate(string str, object data)
			{
				CreatureBait.StatesInstance statesInstance2 = (CreatureBait.StatesInstance)data;
				Element element2 = ElementLoader.FindElementByName(statesInstance2.master.baitElement.ToString());
				str = str.Replace("{0}", element2.name);
				return str;
			};
			Baited.resolveTooltipCallback = delegate(string str, object data)
			{
				CreatureBait.StatesInstance statesInstance = (CreatureBait.StatesInstance)data;
				Element element = ElementLoader.FindElementByName(statesInstance.master.baitElement.ToString());
				str = str.Replace("{0}", element.name);
				return str;
			};
			TanningLightSufficient = CreateStatusItem("TanningLightSufficient", BUILDING.STATUSITEMS.TANNINGLIGHTSUFFICIENT.NAME, BUILDING.STATUSITEMS.TANNINGLIGHTSUFFICIENT.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			TanningLightInsufficient = CreateStatusItem("TanningLightInsufficient", BUILDING.STATUSITEMS.TANNINGLIGHTINSUFFICIENT.NAME, BUILDING.STATUSITEMS.TANNINGLIGHTINSUFFICIENT.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			HotTubWaterTooCold = CreateStatusItem("HotTubWaterTooCold", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			HotTubWaterTooCold.resolveStringCallback = delegate(string str, object data)
			{
				HotTub hotTub3 = (HotTub)data;
				str = str.Replace("{temperature}", GameUtil.GetFormattedTemperature(hotTub3.minimumWaterTemperature));
				return str;
			};
			HotTubTooHot = CreateStatusItem("HotTubTooHot", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			HotTubTooHot.resolveStringCallback = delegate(string str, object data)
			{
				HotTub hotTub2 = (HotTub)data;
				str = str.Replace("{temperature}", GameUtil.GetFormattedTemperature(hotTub2.maxOperatingTemperature));
				return str;
			};
			HotTubFilling = CreateStatusItem("HotTubFilling", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			HotTubFilling.resolveStringCallback = delegate(string str, object data)
			{
				HotTub hotTub = (HotTub)data;
				str = str.Replace("{fullness}", GameUtil.GetFormattedPercent(hotTub.PercentFull));
				return str;
			};
			WindTunnelIntake = CreateStatusItem("WindTunnelIntake", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			WarpPortalCharging = CreateStatusItem("WarpPortalCharging", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			WarpPortalCharging.resolveStringCallback = delegate(string str, object data)
			{
				WarpPortal warpPortal2 = (WarpPortal)data;
				str = str.Replace("{charge}", GameUtil.GetFormattedPercent(100f * (((WarpPortal)data).rechargeProgress / 3000f)));
				return str;
			};
			WarpPortalCharging.resolveTooltipCallback = delegate(string str, object data)
			{
				WarpPortal warpPortal = (WarpPortal)data;
				str = str.Replace("{cycles}", $"{(3000f - ((WarpPortal)data).rechargeProgress) / 600f:0.0}");
				return str;
			};
			WarpConduitPartnerDisabled = CreateStatusItem("WarpConduitPartnerDisabled", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			WarpConduitPartnerDisabled.resolveStringCallback = (string str, object data) => str.Replace("{x}", data.ToString());
			InOrbit = CreateStatusItem("InOrbit", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			InOrbit.resolveStringCallback = delegate(string str, object data)
			{
				ClusterGridEntity clusterGridEntity = (ClusterGridEntity)data;
				return str.Replace("{Destination}", clusterGridEntity.Name);
			};
			InFlight = CreateStatusItem("InFlight", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			InFlight.resolveStringCallback = delegate(string str, object data)
			{
				Clustercraft clustercraft2 = (Clustercraft)data;
				RocketClusterDestinationSelector component = clustercraft2.GetComponent<RocketClusterDestinationSelector>();
				ClusterGrid.Instance.GetLocationDescription(component.GetDestination(), out var _, out var label, out var _);
				LaunchPad destinationPad = component.GetDestinationPad();
				string newValue = ((destinationPad != null) ? destinationPad.GetProperName() : UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE.ToString());
				return str.Replace("{Destination_Asteroid}", label).Replace("{Destination_Pad}", newValue).Replace("{ETA}", GameUtil.GetFormattedCycles(clustercraft2.TravelETA()));
			};
			DestinationOutOfRange = CreateStatusItem("DestinationOutOfRange", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DestinationOutOfRange.resolveStringCallback = delegate(string str, object data)
			{
				Clustercraft clustercraft = (Clustercraft)data;
				str = str.Replace("{Range}", GameUtil.GetFormattedRocketRange(clustercraft.ModuleInterface.Range, GameUtil.TimeSlice.None, displaySuffix: false));
				return str.Replace("{Distance}", clustercraft.RemainingTravelNodes() + " " + (string)UI.CLUSTERMAP.TILES);
			};
			RocketStranded = CreateStatusItem("RocketStranded", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			RailgunpayloadNeedsEmptying = CreateStatusItem("RailgunpayloadNeedsEmptying", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AwaitingEmptyBuilding = CreateStatusItem("AwaitingEmptyBuilding", "BUILDING", "action_empty_contents", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			DuplicantActivationRequired = CreateStatusItem("DuplicantActivationRequired", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RocketChecklistIncomplete = CreateStatusItem("RocketChecklistIncomplete", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			RocketCargoEmptying = CreateStatusItem("RocketCargoEmptying", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RocketCargoFilling = CreateStatusItem("RocketCargoFilling", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RocketCargoFull = CreateStatusItem("RocketCargoFull", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PilotNeeded = CreateStatusItem("PilotNeeded", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PilotNeeded.resolveStringCallback = delegate(string str, object data)
			{
				RocketControlStation master = ((RocketControlStation.StatesInstance)data).master;
				return str.Replace("{timeRemaining}", GameUtil.GetFormattedTime(master.TimeRemaining));
			};
			AutoPilotActive = CreateStatusItem("AutoPilotActive", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			InvalidMaskStationConsumptionState = CreateStatusItem("InvalidMaskStationConsumptionState", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ClusterTelescopeAllWorkComplete = CreateStatusItem("ClusterTelescopeAllWorkComplete", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
		}

		private static bool ShowInUtilityOverlay(HashedString mode, object data)
		{
			Transform transform = (Transform)data;
			bool result = false;
			if (mode == OverlayModes.GasConduits.ID)
			{
				Tag prefabTag = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.GasVentIDs.Contains(prefabTag);
			}
			else if (mode == OverlayModes.LiquidConduits.ID)
			{
				Tag prefabTag2 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.LiquidVentIDs.Contains(prefabTag2);
			}
			else if (mode == OverlayModes.Power.ID)
			{
				Tag prefabTag3 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.WireIDs.Contains(prefabTag3);
			}
			else if (mode == OverlayModes.Logic.ID)
			{
				Tag prefabTag4 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag4);
			}
			else if (mode == OverlayModes.SolidConveyor.ID)
			{
				Tag prefabTag5 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.SolidConveyorIDs.Contains(prefabTag5);
			}
			else if (mode == OverlayModes.Radiation.ID)
			{
				Tag prefabTag6 = transform.GetComponent<KPrefabID>().PrefabTag;
				result = OverlayScreen.RadiationIDs.Contains(prefabTag6);
			}
			return result;
		}
	}
}
