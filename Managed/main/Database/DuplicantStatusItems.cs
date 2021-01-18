using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Database
{
	public class DuplicantStatusItems : StatusItems
	{
		public StatusItem Idle;

		public StatusItem Pacified;

		public StatusItem PendingPacification;

		public StatusItem Dead;

		public StatusItem MoveToSuitNotRequired;

		public StatusItem DroppingUnusedInventory;

		public StatusItem MovingToSafeArea;

		public StatusItem BedUnreachable;

		public StatusItem Hungry;

		public StatusItem Starving;

		public StatusItem Rotten;

		public StatusItem Quarantined;

		public StatusItem NoRationsAvailable;

		public StatusItem RationsUnreachable;

		public StatusItem RationsNotPermitted;

		public StatusItem DailyRationLimitReached;

		public StatusItem Scalding;

		public StatusItem Hot;

		public StatusItem Cold;

		public StatusItem QuarantineAreaUnassigned;

		public StatusItem QuarantineAreaUnreachable;

		public StatusItem Tired;

		public StatusItem NervousBreakdown;

		public StatusItem Unhappy;

		public StatusItem Suffocating;

		public StatusItem HoldingBreath;

		public StatusItem ToiletUnreachable;

		public StatusItem NoUsableToilets;

		public StatusItem NoToilets;

		public StatusItem Vomiting;

		public StatusItem Coughing;

		public StatusItem BreathingO2;

		public StatusItem EmittingCO2;

		public StatusItem LowOxygen;

		public StatusItem RedAlert;

		public StatusItem Digging;

		public StatusItem Eating;

		public StatusItem Sleeping;

		public StatusItem SleepingInterruptedByLight;

		public StatusItem SleepingInterruptedByNoise;

		public StatusItem SleepingPeacefully;

		public StatusItem SleepingBadly;

		public StatusItem SleepingTerribly;

		public StatusItem Cleaning;

		public StatusItem PickingUp;

		public StatusItem Mopping;

		public StatusItem Cooking;

		public StatusItem Arting;

		public StatusItem Mushing;

		public StatusItem Researching;

		public StatusItem Tinkering;

		public StatusItem Storing;

		public StatusItem Building;

		public StatusItem Equipping;

		public StatusItem WarmingUp;

		public StatusItem GeneratingPower;

		public StatusItem Harvesting;

		public StatusItem Uprooting;

		public StatusItem Emptying;

		public StatusItem Toggling;

		public StatusItem Deconstructing;

		public StatusItem Disinfecting;

		public StatusItem Relocating;

		public StatusItem Upgrading;

		public StatusItem Fabricating;

		public StatusItem Processing;

		public StatusItem Clearing;

		public StatusItem BodyRegulatingHeating;

		public StatusItem BodyRegulatingCooling;

		public StatusItem EntombedChore;

		public StatusItem EarlyMorning;

		public StatusItem NightTime;

		public StatusItem PoorDecor;

		public StatusItem PoorQualityOfLife;

		public StatusItem PoorFoodQuality;

		public StatusItem GoodFoodQuality;

		public StatusItem SevereWounds;

		public StatusItem Incapacitated;

		public StatusItem Fighting;

		public StatusItem Fleeing;

		public StatusItem Stressed;

		public StatusItem LashingOut;

		public StatusItem LowImmunity;

		public StatusItem Studying;

		public StatusItem Socializing;

		public StatusItem Mingling;

		public StatusItem ContactWithGerms;

		public StatusItem ExposedToGerms;

		public StatusItem LightWorkEfficiencyBonus;

		public StatusItem BeingProductive;

		public StatusItem BalloonArtistPlanning;

		public StatusItem BalloonArtistHandingOut;

		private const int NONE_OVERLAY = 0;

		public DuplicantStatusItems(ResourceSet parent)
			: base("DuplicantStatusItems", parent)
		{
			CreateStatusItems();
		}

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 2)
		{
			return Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 2)
		{
			return Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
		}

		private void CreateStatusItems()
		{
			Func<string, object, string> resolveStringCallback = delegate(string str, object data)
			{
				Workable workable3 = (Workable)data;
				if (workable3 != null && workable3.GetComponent<KSelectable>() != null)
				{
					str = str.Replace("{Target}", workable3.GetComponent<KSelectable>().GetName());
				}
				return str;
			};
			Func<string, object, string> resolveStringCallback2 = delegate(string str, object data)
			{
				Workable workable2 = (Workable)data;
				if (workable2 != null)
				{
					str = str.Replace("{Target}", workable2.GetComponent<KSelectable>().GetName());
					ComplexFabricatorWorkable complexFabricatorWorkable = workable2 as ComplexFabricatorWorkable;
					if (complexFabricatorWorkable != null)
					{
						ComplexRecipe currentWorkingOrder = complexFabricatorWorkable.CurrentWorkingOrder;
						if (currentWorkingOrder != null)
						{
							str = str.Replace("{Item}", currentWorkingOrder.FirstResult.ProperName());
						}
					}
				}
				return str;
			};
			BedUnreachable = CreateStatusItem("BedUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			BedUnreachable.AddNotification();
			DailyRationLimitReached = CreateStatusItem("DailyRationLimitReached", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			DailyRationLimitReached.AddNotification();
			HoldingBreath = CreateStatusItem("HoldingBreath", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Hungry = CreateStatusItem("Hungry", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Unhappy = CreateStatusItem("Unhappy", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Unhappy.AddNotification();
			NervousBreakdown = CreateStatusItem("NervousBreakdown", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			NervousBreakdown.AddNotification();
			NoRationsAvailable = CreateStatusItem("NoRationsAvailable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			PendingPacification = CreateStatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			QuarantineAreaUnassigned = CreateStatusItem("QuarantineAreaUnassigned", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			QuarantineAreaUnassigned.AddNotification();
			QuarantineAreaUnreachable = CreateStatusItem("QuarantineAreaUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			QuarantineAreaUnreachable.AddNotification();
			Quarantined = CreateStatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RationsUnreachable = CreateStatusItem("RationsUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			RationsUnreachable.AddNotification();
			RationsNotPermitted = CreateStatusItem("RationsNotPermitted", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			RationsNotPermitted.AddNotification();
			Rotten = CreateStatusItem("Rotten", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Starving = CreateStatusItem("Starving", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Starving.AddNotification();
			Suffocating = CreateStatusItem("Suffocating", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, allow_multiples: false, OverlayModes.None.ID);
			Suffocating.AddNotification();
			Tired = CreateStatusItem("Tired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Idle = CreateStatusItem("Idle", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Idle.AddNotification();
			Pacified = CreateStatusItem("Pacified", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Dead = CreateStatusItem("Dead", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Dead.resolveStringCallback = delegate(string str, object data)
			{
				Death death = (Death)data;
				return str.Replace("{Death}", death.Name);
			};
			MoveToSuitNotRequired = CreateStatusItem("MoveToSuitNotRequired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			DroppingUnusedInventory = CreateStatusItem("DroppingUnusedInventory", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			MovingToSafeArea = CreateStatusItem("MovingToSafeArea", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ToiletUnreachable = CreateStatusItem("ToiletUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			ToiletUnreachable.AddNotification();
			NoUsableToilets = CreateStatusItem("NoUsableToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoUsableToilets.AddNotification();
			NoToilets = CreateStatusItem("NoToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NoToilets.AddNotification();
			BreathingO2 = CreateStatusItem("BreathingO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 130);
			BreathingO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather2 = (OxygenBreather)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(oxygenBreather2.O2Accumulator);
				return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(0f - averageRate, GameUtil.TimeSlice.PerSecond));
			};
			EmittingCO2 = CreateStatusItem("EmittingCO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 130);
			EmittingCO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;
				return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather.CO2EmitRate, GameUtil.TimeSlice.PerSecond));
			};
			Vomiting = CreateStatusItem("Vomiting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Coughing = CreateStatusItem("Coughing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			LowOxygen = CreateStatusItem("LowOxygen", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			LowOxygen.AddNotification();
			RedAlert = CreateStatusItem("RedAlert", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Sleeping = CreateStatusItem("Sleeping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Sleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				if (data is SleepChore.StatesInstance)
				{
					string stateChangeNoiseSource = ((SleepChore.StatesInstance)data).stateChangeNoiseSource;
					if (!string.IsNullOrEmpty(stateChangeNoiseSource))
					{
						string text3 = DUPLICANTS.STATUSITEMS.SLEEPING.TOOLTIP;
						text3 = text3.Replace("{Disturber}", stateChangeNoiseSource);
						str += text3;
					}
				}
				return str;
			};
			SleepingInterruptedByNoise = CreateStatusItem("SleepingInterruptedByNoise", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SleepingInterruptedByLight = CreateStatusItem("SleepingInterruptedByLight", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Eating = CreateStatusItem("Eating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Eating.resolveStringCallback = resolveStringCallback;
			Digging = CreateStatusItem("Digging", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Cleaning = CreateStatusItem("Cleaning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Cleaning.resolveStringCallback = resolveStringCallback;
			PickingUp = CreateStatusItem("PickingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PickingUp.resolveStringCallback = resolveStringCallback;
			Mopping = CreateStatusItem("Mopping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Cooking = CreateStatusItem("Cooking", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Cooking.resolveStringCallback = resolveStringCallback2;
			Mushing = CreateStatusItem("Mushing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Mushing.resolveStringCallback = resolveStringCallback2;
			Researching = CreateStatusItem("Researching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Researching.resolveStringCallback = delegate(string str, object data)
			{
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				return (activeResearch != null) ? str.Replace("{Tech}", activeResearch.tech.Name) : str;
			};
			Tinkering = CreateStatusItem("Tinkering", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Tinkering.resolveStringCallback = delegate(string str, object data)
			{
				Tinkerable tinkerable = (Tinkerable)data;
				return (tinkerable != null) ? string.Format(str, tinkerable.tinkerMaterialTag.ProperName()) : str;
			};
			Storing = CreateStatusItem("Storing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Storing.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null && workable.worker != null)
				{
					KSelectable component = workable.GetComponent<KSelectable>();
					if ((bool)component)
					{
						str = str.Replace("{Target}", component.GetName());
					}
					Pickupable pickupable = workable.worker.workCompleteData as Pickupable;
					if (workable.worker != null && (bool)pickupable)
					{
						KSelectable component2 = pickupable.GetComponent<KSelectable>();
						if ((bool)component2)
						{
							str = str.Replace("{Item}", component2.GetName());
						}
					}
				}
				return str;
			};
			Building = CreateStatusItem("Building", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Building.resolveStringCallback = resolveStringCallback;
			Equipping = CreateStatusItem("Equipping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Equipping.resolveStringCallback = resolveStringCallback;
			WarmingUp = CreateStatusItem("WarmingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			WarmingUp.resolveStringCallback = resolveStringCallback;
			GeneratingPower = CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			GeneratingPower.resolveStringCallback = resolveStringCallback;
			Harvesting = CreateStatusItem("Harvesting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Harvesting.resolveStringCallback = resolveStringCallback;
			Uprooting = CreateStatusItem("Uprooting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Uprooting.resolveStringCallback = resolveStringCallback;
			Emptying = CreateStatusItem("Emptying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Emptying.resolveStringCallback = resolveStringCallback;
			Toggling = CreateStatusItem("Toggling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Toggling.resolveStringCallback = resolveStringCallback;
			Deconstructing = CreateStatusItem("Deconstructing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Deconstructing.resolveStringCallback = resolveStringCallback;
			Disinfecting = CreateStatusItem("Disinfecting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Disinfecting.resolveStringCallback = resolveStringCallback;
			Upgrading = CreateStatusItem("Upgrading", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Upgrading.resolveStringCallback = resolveStringCallback;
			Fabricating = CreateStatusItem("Fabricating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Fabricating.resolveStringCallback = resolveStringCallback2;
			Processing = CreateStatusItem("Processing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Processing.resolveStringCallback = resolveStringCallback2;
			Clearing = CreateStatusItem("Clearing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Clearing.resolveStringCallback = resolveStringCallback;
			GeneratingPower = CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			GeneratingPower.resolveStringCallback = resolveStringCallback;
			Cold = CreateStatusItem("Cold", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Cold.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float dtu_s2 = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s2));
				AttributeInstance attributeInstance3 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text2 = "<b>" + attributeInstance3.GetFormattedValue() + "</b>";
				for (int j = 0; j != attributeInstance3.Modifiers.Count; j++)
				{
					AttributeModifier attributeModifier2 = attributeInstance3.Modifiers[j];
					text2 += "\n";
					text2 = text2 + "    • " + attributeModifier2.GetDescription() + " <b>" + attributeModifier2.GetFormattedString(attributeInstance3.gameObject) + "</b>";
				}
				str = str.Replace("{conductivityBarrier}", text2);
				return str;
			};
			Hot = CreateStatusItem("Hot", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Hot.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float dtu_s = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s));
				AttributeInstance attributeInstance2 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text = "<b>" + attributeInstance2.GetFormattedValue() + "</b>";
				for (int i = 0; i != attributeInstance2.Modifiers.Count; i++)
				{
					AttributeModifier attributeModifier = attributeInstance2.Modifiers[i];
					text += "\n";
					text = text + "    • " + attributeModifier.GetDescription() + " <b>" + attributeModifier.GetFormattedString(attributeInstance2.gameObject) + "</b>";
				}
				str = str.Replace("{conductivityBarrier}", text);
				return str;
			};
			BodyRegulatingHeating = CreateStatusItem("BodyRegulatingHeating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			BodyRegulatingHeating.resolveStringCallback = delegate(string str, object data)
			{
				WarmBlooded.StatesInstance statesInstance = (WarmBlooded.StatesInstance)data;
				return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative));
			};
			BodyRegulatingCooling = CreateStatusItem("BodyRegulatingCooling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			BodyRegulatingCooling.resolveStringCallback = BodyRegulatingHeating.resolveStringCallback;
			EntombedChore = CreateStatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, allow_multiples: false, OverlayModes.None.ID);
			EntombedChore.AddNotification();
			EarlyMorning = CreateStatusItem("EarlyMorning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NightTime = CreateStatusItem("NightTime", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PoorDecor = CreateStatusItem("PoorDecor", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PoorQualityOfLife = CreateStatusItem("PoorQualityOfLife", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PoorFoodQuality = CreateStatusItem("PoorFoodQuality", DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			GoodFoodQuality = CreateStatusItem("GoodFoodQuality", DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Arting = CreateStatusItem("Arting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Arting.resolveStringCallback = resolveStringCallback;
			SevereWounds = CreateStatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			SevereWounds.AddNotification();
			Incapacitated = CreateStatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, allow_multiples: false, OverlayModes.None.ID);
			Incapacitated.AddNotification();
			Incapacitated.resolveStringCallback = delegate(string str, object data)
			{
				IncapacitationMonitor.Instance instance = (IncapacitationMonitor.Instance)data;
				float bleedLifeTime = instance.GetBleedLifeTime(instance);
				str = str.Replace("{CauseOfIncapacitation}", instance.GetCauseOfIncapacitation().Name);
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime));
			};
			Relocating = CreateStatusItem("Relocating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Relocating.resolveStringCallback = resolveStringCallback;
			Fighting = CreateStatusItem("Fighting", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Fighting.AddNotification();
			Fleeing = CreateStatusItem("Fleeing", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Fleeing.AddNotification();
			Stressed = CreateStatusItem("Stressed", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Stressed.AddNotification();
			LashingOut = CreateStatusItem("LashingOut", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			LashingOut.AddNotification();
			LowImmunity = CreateStatusItem("LowImmunity", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			LowImmunity.AddNotification();
			Studying = CreateStatusItem("Studying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Socializing = CreateStatusItem("Socializing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, allow_multiples: false, OverlayModes.None.ID);
			Mingling = CreateStatusItem("Mingling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, allow_multiples: false, OverlayModes.None.ID);
			ContactWithGerms = CreateStatusItem("ContactWithGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.Disease.ID);
			ContactWithGerms.resolveStringCallback = delegate(string str, object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData4 = (GermExposureMonitor.ExposureStatusData)data;
				string name2 = Db.Get().Sicknesses.Get(exposureStatusData4.exposure_type.sickness_id).Name;
				str = str.Replace("{Sickness}", name2);
				return str;
			};
			ContactWithGerms.statusItemClickCallback = delegate(object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData3 = (GermExposureMonitor.ExposureStatusData)data;
				Vector3 lastExposurePosition2 = exposureStatusData3.owner.GetLastExposurePosition(exposureStatusData3.exposure_type.germ_id);
				CameraController.Instance.CameraGoTo(lastExposurePosition2);
				if (OverlayScreen.Instance.mode == OverlayModes.None.ID)
				{
					OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID);
				}
			};
			ExposedToGerms = CreateStatusItem("ExposedToGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.Disease.ID);
			ExposedToGerms.resolveStringCallback = delegate(string str, object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData2 = (GermExposureMonitor.ExposureStatusData)data;
				string name = Db.Get().Sicknesses.Get(exposureStatusData2.exposure_type.sickness_id).Name;
				AttributeInstance attributeInstance = Db.Get().Attributes.GermResistance.Lookup(exposureStatusData2.owner.gameObject);
				string lastDiseaseSource = exposureStatusData2.owner.GetLastDiseaseSource(exposureStatusData2.exposure_type.germ_id);
				GermExposureMonitor.Instance sMI = exposureStatusData2.owner.GetSMI<GermExposureMonitor.Instance>();
				float num = (float)exposureStatusData2.exposure_type.base_resistance + GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
				float totalValue = attributeInstance.GetTotalValue();
				float resistanceToExposureType = sMI.GetResistanceToExposureType(exposureStatusData2.exposure_type);
				float contractionChance = GermExposureMonitor.GetContractionChance(resistanceToExposureType);
				float exposureTier = sMI.GetExposureTier(exposureStatusData2.exposure_type.germ_id);
				float num2 = GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[(int)exposureTier - 1] - GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
				str = str.Replace("{Severity}", DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.EXPOSURE_TIERS[(int)exposureTier - 1]);
				str = str.Replace("{Sickness}", name);
				str = str.Replace("{Source}", lastDiseaseSource);
				str = str.Replace("{Base}", GameUtil.GetFormattedSimple(num));
				str = str.Replace("{Dupe}", GameUtil.GetFormattedSimple(totalValue));
				str = str.Replace("{Total}", GameUtil.GetFormattedSimple(resistanceToExposureType));
				str = str.Replace("{ExposureLevelBonus}", GameUtil.GetFormattedSimple(num2));
				str = str.Replace("{Chance}", GameUtil.GetFormattedPercent(contractionChance * 100f));
				return str;
			};
			ExposedToGerms.statusItemClickCallback = delegate(object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData)data;
				Vector3 lastExposurePosition = exposureStatusData.owner.GetLastExposurePosition(exposureStatusData.exposure_type.germ_id);
				CameraController.Instance.CameraGoTo(lastExposurePosition);
				if (OverlayScreen.Instance.mode == OverlayModes.None.ID)
				{
					OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID);
				}
			};
			LightWorkEfficiencyBonus = CreateStatusItem("LightWorkEfficiencyBonus", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, allow_multiples: false, OverlayModes.None.ID);
			LightWorkEfficiencyBonus.resolveStringCallback = delegate(string str, object data)
			{
				string arg = string.Format(DUPLICANTS.STATUSITEMS.LIGHTWORKEFFICIENCYBONUS.NO_BUILDING_WORK_ATTRIBUTE, GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS * 100f), positive: true));
				return string.Format(str, arg);
			};
			BeingProductive = CreateStatusItem("BeingProductive", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			BalloonArtistPlanning = CreateStatusItem("BalloonArtistPlanning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			BalloonArtistHandingOut = CreateStatusItem("BalloonArtistHandingOut", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
		}
	}
}
