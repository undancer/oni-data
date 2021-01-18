using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class CreatureStatusItems : StatusItems
	{
		public StatusItem Dead;

		public StatusItem HealthStatus;

		public StatusItem Hot;

		public StatusItem Hot_Crop;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem Cold_Crop;

		public StatusItem Crop_Too_Dark;

		public StatusItem Crop_Too_Bright;

		public StatusItem Crop_Blighted;

		public StatusItem Hypothermia;

		public StatusItem Hyperthermia;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem Drowning;

		public StatusItem Saturated;

		public StatusItem DryingOut;

		public StatusItem Growing;

		public StatusItem CropSleeping;

		public StatusItem ReadyForHarvest;

		public StatusItem EnvironmentTooWarm;

		public StatusItem EnvironmentTooCold;

		public StatusItem Entombed;

		public StatusItem Wilting;

		public StatusItem WiltingDomestic;

		public StatusItem WiltingNonGrowing;

		public StatusItem WiltingNonGrowingDomestic;

		public StatusItem WrongAtmosphere;

		public StatusItem AtmosphericPressureTooLow;

		public StatusItem AtmosphericPressureTooHigh;

		public StatusItem Barren;

		public StatusItem NeedsFertilizer;

		public StatusItem NeedsIrrigation;

		public StatusItem WrongTemperature;

		public StatusItem WrongFertilizer;

		public StatusItem WrongIrrigation;

		public StatusItem WrongFertilizerMajor;

		public StatusItem WrongIrrigationMajor;

		public StatusItem CantAcceptFertilizer;

		public StatusItem CantAcceptIrrigation;

		public StatusItem Rotting;

		public StatusItem Fresh;

		public StatusItem Stale;

		public StatusItem Spoiled;

		public StatusItem Refrigerated;

		public StatusItem Unrefrigerated;

		public StatusItem SterilizingAtmosphere;

		public StatusItem ContaminatedAtmosphere;

		public StatusItem Old;

		public StatusItem ExchangingElementOutput;

		public StatusItem ExchangingElementConsume;

		public StatusItem Hungry;

		public StatusItem NoSleepSpot;

		public CreatureStatusItems(ResourceSet parent)
			: base("CreatureStatusItems", parent)
		{
			CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			Dead = new StatusItem("Dead", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Hot = new StatusItem("Hot", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable4 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_Low), GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_High));
			};
			Hot_Crop = new StatusItem("Hot_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Hot_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				str = str.Replace("{low_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_Low));
				str = str.Replace("{high_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_High));
				return str;
			};
			Scalding = new StatusItem("Scalding", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, allow_multiples: true, OverlayModes.None.ID);
			Scalding.resolveTooltipCallback = delegate(string str, object data)
			{
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).GetScaldingThreshold();
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold));
				return str;
			};
			Scalding.AddNotification();
			Cold = new StatusItem("Cold", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_Low), GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_High));
			};
			Cold_Crop = new StatusItem("Cold_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Cold_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				str = str.Replace("low_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_Low));
				str = str.Replace("high_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_High));
				return str;
			};
			Crop_Too_Dark = new StatusItem("Crop_Too_Dark", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Crop_Too_Bright = new StatusItem("Crop_Too_Bright", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Crop_Blighted = new StatusItem("Crop_Blighted", "CREATURES", "status_item_plant_blighted", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Hyperthermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold));
				return str;
			};
			Hypothermia = new StatusItem("Hypothermia", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			Hypothermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold));
				return str;
			};
			Suffocating = new StatusItem("Suffocating", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Hatching = new StatusItem("Hatching", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Incubating = new StatusItem("Incubating", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Drowning = new StatusItem("Drowning", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Drowning.resolveStringCallback = (string str, object data) => str;
			Saturated = new StatusItem("Saturated", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Saturated.resolveStringCallback = (string str, object data) => str;
			DryingOut = new StatusItem("DryingOut", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			DryingOut.resolveStringCallback = (string str, object data) => str;
			ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			Growing = new StatusItem("Growing", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			Growing.resolveStringCallback = delegate(string str, object data)
			{
				Crop component = ((Growing)data).GetComponent<Crop>();
				if (component != null)
				{
					float seconds = ((Growing)data).TimeUntilNextHarvest();
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(seconds));
				}
				float val = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor(Math.Max(val, 0f)).ToString("F0"));
				return str;
			};
			CropSleeping = new StatusItem("Crop_Sleeping", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			CropSleeping.resolveStringCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance8 = (CropSleepingMonitor.Instance)data;
				return str.Replace("{REASON}", instance8.def.prefersDarkness ? CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_BRIGHT : CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_DARK);
			};
			CropSleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance7 = (CropSleepingMonitor.Instance)data;
				string newValue4 = string.Format(CREATURES.STATUSITEMS.CROP_SLEEPING.REQUIREMENT_LUMINANCE, instance7.def.lightIntensityThreshold);
				return str.Replace("{REQUIREMENTS}", newValue4);
			};
			EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float temp3 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float temp4 = ((TemperatureVulnerable)data).internalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(temp3));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(temp4));
				return str;
			};
			EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float temp = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float temp2 = ((TemperatureVulnerable)data).internalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(temp));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(temp2));
				return str;
			};
			Entombed = new StatusItem("Entombed", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Entombed.resolveStringCallback = (string str, object go) => str;
			Entombed.resolveTooltipCallback = delegate(string str, object go)
			{
				GameObject go2 = go as GameObject;
				return string.Format(str, GameUtil.GetIdentityDescriptor(go2));
			};
			Wilting = new StatusItem("Wilting", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false, 1026);
			Wilting.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest())));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			WiltingDomestic = new StatusItem("WiltingDomestic", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			WiltingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest())));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			WiltingNonGrowing = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false, 1026);
			WiltingNonGrowing.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			WiltingNonGrowingDomestic = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: true, 1026);
			WiltingNonGrowingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			WrongAtmosphere = new StatusItem("WrongAtmosphere", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			WrongAtmosphere.resolveStringCallback = delegate(string str, object data)
			{
				string text = "";
				foreach (Element safe_atmosphere in (data as PressureVulnerable).safe_atmospheres)
				{
					text = text + "\n    •  " + safe_atmosphere.name;
				}
				str = str.Replace("{elements}", text);
				return str;
			};
			AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_Low));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_High));
				return str;
			};
			AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_Low));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_High));
				return str;
			};
			HealthStatus = new StatusItem("HealthStatus", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string newValue3 = "";
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					break;
				case Health.HealthState.Scuffed:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					break;
				case Health.HealthState.Injured:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					break;
				case Health.HealthState.Critical:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					break;
				case Health.HealthState.Incapacitated:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					break;
				case Health.HealthState.Dead:
					newValue3 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					break;
				}
				str = str.Replace("{healthState}", newValue3);
				return str;
			};
			HealthStatus.resolveTooltipCallback = delegate(string str, object data)
			{
				string newValue2 = "";
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case Health.HealthState.Scuffed:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case Health.HealthState.Injured:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case Health.HealthState.Critical:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case Health.HealthState.Incapacitated:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case Health.HealthState.Dead:
					newValue2 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", newValue2);
				return str;
			};
			Barren = new StatusItem("Barren", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_plant_solid", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Func<string, object, string> resolveStringCallback = (string str, object data) => str;
			NeedsFertilizer.resolveStringCallback = resolveStringCallback;
			NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Func<string, object, string> resolveStringCallback2 = (string str, object data) => str;
			NeedsIrrigation.resolveStringCallback = resolveStringCallback2;
			WrongFertilizer = new StatusItem("WrongFertilizer", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Func<string, object, string> resolveStringCallback3 = (string str, object data) => str;
			WrongFertilizer.resolveStringCallback = resolveStringCallback3;
			WrongFertilizerMajor = new StatusItem("WrongFertilizerMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			WrongFertilizerMajor.resolveStringCallback = resolveStringCallback3;
			WrongIrrigation = new StatusItem("WrongIrrigation", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Func<string, object, string> resolveStringCallback4 = (string str, object data) => str;
			WrongIrrigation.resolveStringCallback = resolveStringCallback4;
			WrongIrrigationMajor = new StatusItem("WrongIrrigationMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			WrongIrrigationMajor.resolveStringCallback = resolveStringCallback4;
			CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Rotting = new StatusItem("Rotting", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Rotting.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f));
			Fresh = new StatusItem("Fresh", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Fresh.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance6 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance6.RotConstitutionPercentage * 100f) + "%)");
			};
			Fresh.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance5 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance5.GetToolTip());
			};
			Stale = new StatusItem("Stale", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Stale.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance4 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance4.RotConstitutionPercentage * 100f) + "%)");
			};
			Stale.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance3 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance3.GetToolTip());
			};
			Spoiled = new StatusItem("Spoiled", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Refrigerated = new StatusItem("Refrigerated", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Unrefrigerated.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f));
			SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Old = new StatusItem("Old", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			Old.resolveTooltipCallback = delegate(string str, object data)
			{
				AgeMonitor.Instance instance2 = (AgeMonitor.Instance)data;
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(instance2.CyclesUntilDeath * 600f));
			};
			ExchangingElementConsume = new StatusItem("ExchangingElementConsume", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ExchangingElementConsume.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance4 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance4.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance4.master.consumeRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			ExchangingElementConsume.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance3 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance3.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance3.master.consumeRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			ExchangingElementOutput = new StatusItem("ExchangingElementOutput", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ExchangingElementOutput.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance2 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance2.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance2.master.consumeRate * statesInstance2.master.exchangeRatio, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			ExchangingElementOutput.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance.master.consumeRate * statesInstance.master.exchangeRatio, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			Hungry = new StatusItem("Hungry", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Hungry.resolveTooltipCallback = delegate(string str, object data)
			{
				CreatureCalorieMonitor.Instance instance = (CreatureCalorieMonitor.Instance)data;
				CreatureCalorieMonitor.Def def = instance.master.gameObject.GetDef<CreatureCalorieMonitor.Def>();
				Diet diet = def.diet;
				if (diet.consumedTags.Count > 0)
				{
					string[] array = diet.consumedTags.Select((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray();
					if (array.Length > 3)
					{
						string[] array2 = new string[4]
						{
							array[0],
							array[1],
							array[2],
							"..."
						};
						array = array2;
					}
					string newValue = string.Join(", ", array);
					return str + "\n" + UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue);
				}
				return str;
			};
			NoSleepSpot = new StatusItem("NoSleepSpot", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		}
	}
}
