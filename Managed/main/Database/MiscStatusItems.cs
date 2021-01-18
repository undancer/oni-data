using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class MiscStatusItems : StatusItems
	{
		public StatusItem MarkedForDisinfection;

		public StatusItem MarkedForCompost;

		public StatusItem MarkedForCompostInStorage;

		public StatusItem PendingClear;

		public StatusItem PendingClearNoStorage;

		public StatusItem Edible;

		public StatusItem WaitingForDig;

		public StatusItem WaitingForMop;

		public StatusItem OreMass;

		public StatusItem OreTemp;

		public StatusItem ElementalCategory;

		public StatusItem ElementalState;

		public StatusItem ElementalTemperature;

		public StatusItem ElementalMass;

		public StatusItem ElementalDisease;

		public StatusItem TreeFilterableTags;

		public StatusItem SublimationOverpressure;

		public StatusItem SublimationEmitting;

		public StatusItem SublimationBlocked;

		public StatusItem BuriedItem;

		public StatusItem SpoutOverPressure;

		public StatusItem SpoutEmitting;

		public StatusItem SpoutPressureBuilding;

		public StatusItem SpoutIdle;

		public StatusItem SpoutDormant;

		public StatusItem OrderAttack;

		public StatusItem OrderCapture;

		public StatusItem PendingHarvest;

		public StatusItem NotMarkedForHarvest;

		public StatusItem PendingUproot;

		public StatusItem PickupableUnreachable;

		public StatusItem Prioritized;

		public StatusItem Using;

		public StatusItem Operating;

		public StatusItem Cleaning;

		public StatusItem RegionIsBlocked;

		public StatusItem NoClearLocationsAvailable;

		public StatusItem AwaitingStudy;

		public StatusItem Studied;

		public StatusItem StudiedGeyserTimeRemaining;

		public StatusItem Space;

		public StatusItem HighEnergyParticleCount;

		public MiscStatusItems(ResourceSet parent)
			: base("MiscStatusItems", parent)
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
			Edible = CreateStatusItem("Edible", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Edible.resolveStringCallback = delegate(string str, object data)
			{
				Edible edible = (Edible)data;
				str = string.Format(str, GameUtil.GetFormattedCalories(edible.Calories));
				return str;
			};
			PendingClear = CreateStatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingClearNoStorage = CreateStatusItem("PendingClearNoStorage", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			MarkedForCompost = CreateStatusItem("MarkedForCompost", "MISC", "status_item_pending_compost", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			MarkedForCompostInStorage = CreateStatusItem("MarkedForCompostInStorage", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			MarkedForDisinfection = CreateStatusItem("MarkedForDisinfection", "MISC", "status_item_disinfect", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.Disease.ID);
			NoClearLocationsAvailable = CreateStatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			WaitingForDig = CreateStatusItem("WaitingForDig", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			WaitingForMop = CreateStatusItem("WaitingForMop", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			OreMass = CreateStatusItem("OreMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			OreMass.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject3 = (GameObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject3.GetComponent<PrimaryElement>().Mass));
				return str;
			};
			OreTemp = CreateStatusItem("OreTemp", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			OreTemp.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject2.GetComponent<PrimaryElement>().Temperature));
				return str;
			};
			ElementalState = CreateStatusItem("ElementalState", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ElementalState.resolveStringCallback = delegate(string str, object data)
			{
				Element element2 = ((Func<Element>)data)();
				str = str.Replace("{State}", element2.GetStateString());
				return str;
			};
			ElementalCategory = CreateStatusItem("ElementalCategory", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ElementalCategory.resolveStringCallback = delegate(string str, object data)
			{
				Element element = ((Func<Element>)data)();
				str = str.Replace("{Category}", element.GetMaterialCategoryTag().ProperName());
				return str;
			};
			ElementalTemperature = CreateStatusItem("ElementalTemperature", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ElementalTemperature.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject7 = (CellSelectionObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject7.temperature));
				return str;
			};
			ElementalMass = CreateStatusItem("ElementalMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ElementalMass.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject6 = (CellSelectionObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject6.Mass));
				return str;
			};
			ElementalDisease = CreateStatusItem("ElementalDisease", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			ElementalDisease.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject5 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject5.diseaseIdx, cellSelectionObject5.diseaseCount));
				return str;
			};
			ElementalDisease.resolveTooltipCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject4 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject4.diseaseIdx, cellSelectionObject4.diseaseCount, color: true));
				return str;
			};
			TreeFilterableTags = CreateStatusItem("TreeFilterableTags", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			TreeFilterableTags.resolveStringCallback = delegate(string str, object data)
			{
				TreeFilterable treeFilterable = (TreeFilterable)data;
				str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus());
				return str;
			};
			SublimationEmitting = CreateStatusItem("SublimationEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SublimationEmitting.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject3 = (CellSelectionObject)data;
				if (cellSelectionObject3.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", GameUtil.GetElementNameByElementHash(cellSelectionObject3.element.sublimateId));
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject3.FlowRate, GameUtil.TimeSlice.PerSecond));
				return str;
			};
			SublimationEmitting.resolveTooltipCallback = SublimationEmitting.resolveStringCallback;
			SublimationBlocked = CreateStatusItem("SublimationBlocked", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SublimationBlocked.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject2 = (CellSelectionObject)data;
				if (cellSelectionObject2.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", cellSelectionObject2.element.name);
				str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject2.element.sublimateId));
				return str;
			};
			SublimationBlocked.resolveTooltipCallback = SublimationBlocked.resolveStringCallback;
			SublimationOverpressure = CreateStatusItem("SublimationOverpressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SublimationOverpressure.resolveTooltipCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject = (CellSelectionObject)data;
				if (cellSelectionObject.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", cellSelectionObject.element.name);
				str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject.element.sublimateId));
				return str;
			};
			Space = CreateStatusItem("Space", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Bad, allow_multiples: false, OverlayModes.None.ID);
			BuriedItem = CreateStatusItem("BuriedItem", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SpoutOverPressure = CreateStatusItem("SpoutOverPressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SpoutOverPressure.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance4 = (Geyser.StatesInstance)data;
				Studyable component5 = statesInstance4.GetComponent<Studyable>();
				str = ((statesInstance4 == null || !(component5 != null) || !component5.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTOVERPRESSURE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance4.master.RemainingEruptTime()))));
				return str;
			};
			SpoutEmitting = CreateStatusItem("SpoutEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SpoutEmitting.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance3 = (Geyser.StatesInstance)data;
				Studyable component4 = statesInstance3.GetComponent<Studyable>();
				str = ((statesInstance3 == null || !(component4 != null) || !component4.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTEMITTING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance3.master.RemainingEruptTime()))));
				return str;
			};
			SpoutPressureBuilding = CreateStatusItem("SpoutPressureBuilding", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SpoutPressureBuilding.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance2 = (Geyser.StatesInstance)data;
				Studyable component3 = statesInstance2.GetComponent<Studyable>();
				str = ((statesInstance2 == null || !(component3 != null) || !component3.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTPRESSUREBUILDING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance2.master.RemainingNonEruptTime()))));
				return str;
			};
			SpoutIdle = CreateStatusItem("SpoutIdle", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			SpoutIdle.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance = (Geyser.StatesInstance)data;
				Studyable component2 = statesInstance.GetComponent<Studyable>();
				str = ((statesInstance == null || !(component2 != null) || !component2.Studied) ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTIDLE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingNonEruptTime()))));
				return str;
			};
			SpoutDormant = CreateStatusItem("SpoutDormant", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			OrderAttack = CreateStatusItem("OrderAttack", "MISC", "status_item_attack", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			OrderCapture = CreateStatusItem("OrderCapture", "MISC", "status_item_capture", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PendingHarvest = CreateStatusItem("PendingHarvest", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			NotMarkedForHarvest = CreateStatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			NotMarkedForHarvest.conditionalOverlayCallback = (HashedString viewMode, object o) => (!(viewMode != OverlayModes.None.ID)) ? true : false;
			PendingUproot = CreateStatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			PickupableUnreachable = CreateStatusItem("PickupableUnreachable", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Prioritized = CreateStatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Using = CreateStatusItem("Using", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Using.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null)
				{
					KSelectable component = workable.GetComponent<KSelectable>();
					if (component != null)
					{
						str = str.Replace("{Target}", component.GetName());
					}
				}
				return str;
			};
			Operating = CreateStatusItem("Operating", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Cleaning = CreateStatusItem("Cleaning", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			RegionIsBlocked = CreateStatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			AwaitingStudy = CreateStatusItem("AwaitingStudy", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			Studied = CreateStatusItem("Studied", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			HighEnergyParticleCount = CreateStatusItem("HighEnergyParticleCount", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			HighEnergyParticleCount.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				str = GameUtil.GetFormattedHighEnergyParticles(gameObject.GetComponent<HighEnergyParticle>().payload);
				return str;
			};
		}
	}
}
