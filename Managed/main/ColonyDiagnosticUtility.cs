using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColonyDiagnosticUtility : KMonoBehaviour, ISim4000ms
{
	public enum DisplaySetting
	{
		Always,
		AlertOnly,
		Never,
		LENGTH
	}

	public class IdleDiagnostic : ColonyDiagnostic
	{
		public IdleDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<IdleTracker>(worldID);
			icon = "icon_errand_operate";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.NORMAL;
				if (tracker.GetMinValue(30f) > 0f && tracker.GetCurrentValue() > 0f)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.IDLEDIAGNOSTIC.IDLE;
					MinionIdentity minionIdentity = Components.LiveMinionIdentities.GetWorldItems(base.worldID).Find((MinionIdentity match) => match.HasTag(GameTags.Idle));
					if (minionIdentity != null)
					{
						result.clickThroughTarget = new Tuple<Vector3, GameObject>(minionIdentity.transform.position, minionIdentity.gameObject);
					}
				}
			}
			return result;
		}
	}

	public class PowerUseDiagnostic : ColonyDiagnostic
	{
		public PowerUseDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<PowerUseTracker>(worldID);
			trackerSampleCountSeconds = 30f;
			icon = "overlay_power";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
			}
			return result;
		}
	}

	public class HeatDiagnostic : ColonyDiagnostic
	{
		public HeatDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.HEATDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<BatteryTracker>(worldID);
			trackerSampleCountSeconds = 4f;
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NORMAL;
			}
			return result;
		}
	}

	public class TrappedDuplicantDiagnostic : ColonyDiagnostic
	{
		public TrappedDuplicantDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.ALL_NAME)
		{
		}

		public override DiagnosticResult Evaluate()
		{
			DiagnosticResult result = default(DiagnosticResult);
			bool flag = false;
			foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(base.worldID))
			{
				if (flag)
				{
					break;
				}
				if (ClusterManager.Instance.GetWorld(base.worldID).IsModuleInterior || !CheckMinionBasicallyIdle(worldItem))
				{
					continue;
				}
				Navigator component = worldItem.GetComponent<Navigator>();
				bool flag2 = true;
				foreach (MinionIdentity worldItem2 in Components.LiveMinionIdentities.GetWorldItems(base.worldID))
				{
					if (worldItem == worldItem2 || CheckMinionBasicallyIdle(worldItem2) || !component.CanReach(worldItem2.GetComponent<IApproachable>()))
					{
						continue;
					}
					flag2 = false;
					break;
				}
				List<Telepad> worldItems = Components.Telepads.GetWorldItems(component.GetMyWorld().id);
				if (worldItems != null && worldItems.Count > 0)
				{
					flag2 = flag2 && !component.CanReach(worldItems[0].GetComponent<IApproachable>());
				}
				List<WarpReceiver> worldItems2 = Components.WarpReceivers.GetWorldItems(component.GetMyWorld().id);
				if (worldItems2 != null && worldItems2.Count > 0)
				{
					foreach (WarpReceiver item in worldItems2)
					{
						flag2 = flag2 && !component.CanReach(worldItems2[0].GetComponent<IApproachable>());
					}
				}
				List<Sleepable> worldItems3 = Components.Sleepables.GetWorldItems(component.GetMyWorld().id);
				Assignable assignable = null;
				for (int i = 0; i < worldItems3.Count; i++)
				{
					assignable = worldItems3[i].GetComponent<Assignable>();
					if (assignable != null && assignable.IsAssignedTo(worldItem))
					{
						flag2 = flag2 && !component.CanReach(worldItems3[i].GetComponent<IApproachable>());
					}
				}
				if (flag2)
				{
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(worldItem.transform.position, worldItem.gameObject);
				}
				flag = flag || flag2;
			}
			result.opinion = ((!flag) ? DiagnosticResult.Opinion.Normal : DiagnosticResult.Opinion.Bad);
			result.Message = (flag ? UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.STUCK : UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.NORMAL);
			return result;
		}

		private bool CheckMinionBasicallyIdle(MinionIdentity minion)
		{
			if (minion.HasTag(GameTags.Idle) || minion.HasTag(GameTags.RecoveringBreath) || minion.HasTag(GameTags.MakingMess))
			{
				return true;
			}
			return false;
		}
	}

	public class BatteryDiagnostic : ColonyDiagnostic
	{
		public BatteryDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<BatteryTracker>(worldID);
			trackerSampleCountSeconds = 4f;
			icon = "overlay_power";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			int num = 5;
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NONE;
				IList<UtilityNetwork> networks = Game.Instance.electricalConduitSystem.GetNetworks();
				foreach (ElectricalUtilityNetwork item in networks)
				{
					if (item.allWires == null || item.allWires.Count == 0)
					{
						continue;
					}
					float num2 = 0f;
					int num3 = Grid.PosToCell(item.allWires[0]);
					if (Grid.WorldIdx[num3] != base.worldID)
					{
						continue;
					}
					ushort circuitID = Game.Instance.circuitManager.GetCircuitID(num3);
					List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
					if (batteriesOnCircuit == null || batteriesOnCircuit.Count == 0)
					{
						continue;
					}
					foreach (Battery item2 in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
					{
						result.opinion = DiagnosticResult.Opinion.Normal;
						result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.NORMAL;
						num2 += item2.capacity;
					}
					if (num2 < Game.Instance.circuitManager.GetWattsUsedByCircuit(circuitID) * (float)num)
					{
						result.opinion = DiagnosticResult.Opinion.Warning;
						result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.LIMITED_CAPACITY;
						Battery battery = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID)[0];
						if (battery != null)
						{
							result.clickThroughTarget = new Tuple<Vector3, GameObject>(battery.transform.position, battery.gameObject);
						}
					}
					foreach (Battery item3 in Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID))
					{
						if (!PastNewBuildingGracePeriod(item3.transform) || item3.CircuitID == ushort.MaxValue || item3.JoulesAvailable != 0f)
						{
							continue;
						}
						result.opinion = DiagnosticResult.Opinion.Warning;
						result.Message = UI.COLONY_DIAGNOSTICS.BATTERYDIAGNOSTIC.DEAD_BATTERY;
						result.clickThroughTarget = new Tuple<Vector3, GameObject>(item3.transform.position, item3.gameObject);
						break;
					}
				}
			}
			return result;
		}
	}

	public class AllChoresDiagnostic : ColonyDiagnostic
	{
		public AllChoresDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<AllChoresCountTracker>(worldID);
			colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
			icon = "icon_errand_operate";
		}

		public override DiagnosticResult Evaluate()
		{
			DiagnosticResult result = default(DiagnosticResult);
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
			return result;
		}
	}

	public class ChoreGroupDiagnostic : ColonyDiagnostic
	{
		public ChoreGroup choreGroup;

		public ChoreGroupDiagnostic(int worldID, ChoreGroup choreGroup)
			: base(worldID, UI.COLONY_DIAGNOSTICS.CHOREGROUPDIAGNOSTIC.ALL_NAME)
		{
			this.choreGroup = choreGroup;
			tracker = TrackerTool.Instance.GetChoreGroupTracker(worldID, choreGroup);
			name = choreGroup.Name;
			colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
			id = "ChoreGroupDiagnostic_" + choreGroup.Id;
		}

		public override DiagnosticResult Evaluate()
		{
			DiagnosticResult result = default(DiagnosticResult);
			result.opinion = ((tracker.GetCurrentValue() > 0f) ? DiagnosticResult.Opinion.Good : DiagnosticResult.Opinion.Normal);
			result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLCHORESDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
			return result;
		}
	}

	public class AllWorkTimeDiagnostic : ColonyDiagnostic
	{
		public AllWorkTimeDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<AllWorkTimeTracker>(worldID);
			colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
		}

		public override DiagnosticResult Evaluate()
		{
			DiagnosticResult result = default(DiagnosticResult);
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetCurrentValue()));
			return result;
		}
	}

	public class WorkTimeDiagnostic : ColonyDiagnostic
	{
		public ChoreGroup choreGroup;

		public WorkTimeDiagnostic(int worldID, ChoreGroup choreGroup)
			: base(worldID, UI.COLONY_DIAGNOSTICS.WORKTIMEDIAGNOSTIC.ALL_NAME)
		{
			this.choreGroup = choreGroup;
			tracker = TrackerTool.Instance.GetWorkTimeTracker(worldID, choreGroup);
			trackerSampleCountSeconds = 100f;
			name = choreGroup.Name;
			id = "WorkTimeDiagnostic_" + choreGroup.Id;
			colors[DiagnosticResult.Opinion.Good] = Constants.NEUTRAL_COLOR;
		}

		public override DiagnosticResult Evaluate()
		{
			DiagnosticResult result = default(DiagnosticResult);
			result.opinion = ((tracker.GetAverageValue(trackerSampleCountSeconds) > 0f) ? DiagnosticResult.Opinion.Good : DiagnosticResult.Opinion.Normal);
			result.Message = string.Format(UI.COLONY_DIAGNOSTICS.ALLWORKTIMEDIAGNOSTIC.NORMAL, tracker.FormatValueString(tracker.GetAverageValue(trackerSampleCountSeconds)));
			return result;
		}
	}

	public class FarmDiagnostic : ColonyDiagnostic
	{
		public FarmDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_errand_farm";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			List<PlantablePlot> list = Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed));
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NORMAL;
				if (list.Count == 0)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE;
				}
				else
				{
					bool flag = false;
					foreach (PlantablePlot item in list)
					{
						if (item.plant != null)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						result.opinion = DiagnosticResult.Opinion.Warning;
						result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE_PLANTED;
					}
					else
					{
						foreach (PlantablePlot item2 in list)
						{
							if (item2.plant != null && item2.plant.HasTag(GameTags.Wilting))
							{
								StandardCropPlant component = item2.plant.GetComponent<StandardCropPlant>();
								if (component != null && component.smi.IsInsideState(component.smi.sm.alive.wilting) && component.smi.timeinstate > 15f)
								{
									result.opinion = DiagnosticResult.Opinion.Warning;
									result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.WILTING;
									result.clickThroughTarget = new Tuple<Vector3, GameObject>(item2.transform.position, item2.gameObject);
									break;
								}
							}
						}
					}
					foreach (PlantablePlot item3 in list)
					{
						if (item3.plant != null && !item3.HasTag(GameTags.Operational))
						{
							result.opinion = DiagnosticResult.Opinion.Warning;
							result.Message = UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.INOPERATIONAL;
							result.clickThroughTarget = new Tuple<Vector3, GameObject>(item3.transform.position, item3.gameObject);
							break;
						}
					}
				}
			}
			return result;
		}

		public override string GetAverageValueString()
		{
			return TrackerTool.Instance.GetWorldTracker<CropTracker>(base.worldID).GetCurrentValue() + "/" + Components.PlantablePlots.GetWorldItems(base.worldID).FindAll((PlantablePlot match) => match.HasDepositTag(GameTags.CropSeed)).Count.ToString();
		}
	}

	public class DecorDiagnostic : ColonyDiagnostic
	{
		public DecorDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.DECORDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_category_decor";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			List<PlantablePlot> worldItems2 = Components.PlantablePlots.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			return result;
		}
	}

	public class BreathabilityDiagnostic : ColonyDiagnostic
	{
		public BreathabilityDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<BreathabilityTracker>(worldID);
			trackerSampleCountSeconds = 50f;
			icon = "overlay_oxygen";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
			{
				return result;
			}
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.NORMAL;
				if (tracker.GetAverageValue(trackerSampleCountSeconds) < 60f)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.POOR;
				}
				foreach (MinionIdentity item in worldItems)
				{
					OxygenBreather component = item.GetComponent<OxygenBreather>();
					OxygenBreather.IGasProvider gasProvider = component.GetGasProvider();
					SuffocationMonitor.Instance sMI = item.GetSMI<SuffocationMonitor.Instance>();
					if (sMI != null && sMI.IsInsideState(sMI.sm.nooxygen.suffocating))
					{
						result.opinion = DiagnosticResult.Opinion.Bad;
						result.Message = UI.COLONY_DIAGNOSTICS.BREATHABILITYDIAGNOSTIC.SUFFOCATING;
						result.clickThroughTarget = new Tuple<Vector3, GameObject>(sMI.transform.position, sMI.gameObject);
					}
				}
			}
			return result;
		}
	}

	public class StressDiagnostic : ColonyDiagnostic
	{
		public StressDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(worldID);
			icon = "mod_stress";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.NORMAL;
				if (tracker.GetAverageValue(trackerSampleCountSeconds) >= STRESS.ACTING_OUT_RESET)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.STRESSDIAGNOSTIC.HIGH_STRESS;
					MinionIdentity minionIdentity = worldItems.Find((MinionIdentity match) => match.gameObject.GetAmounts().GetValue(Db.Get().Amounts.Stress.Id) >= STRESS.ACTING_OUT_RESET);
					if (minionIdentity != null)
					{
						result.clickThroughTarget = new Tuple<Vector3, GameObject>(minionIdentity.gameObject.transform.position, minionIdentity.gameObject);
					}
				}
			}
			return result;
		}
	}

	public class EntombedDiagnostic : ColonyDiagnostic
	{
		private int entombedCount = 0;

		public EntombedDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_action_dig";
		}

		public override DiagnosticResult Evaluate()
		{
			List<BuildingComplete> worldItems = Components.BuildingCompletes.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.NORMAL;
			int num = 0;
			foreach (BuildingComplete item in worldItems)
			{
				if (item.HasTag(GameTags.Entombed))
				{
					result.opinion = DiagnosticResult.Opinion.Bad;
					result.Message = UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.BUILDING_ENTOMBED;
					result.clickThroughTarget = new Tuple<Vector3, GameObject>(item.gameObject.transform.position, item.gameObject);
					num++;
				}
			}
			return result;
		}

		public override string GetAverageValueString()
		{
			return entombedCount.ToString();
		}
	}

	public class BedDiagnostic : ColonyDiagnostic
	{
		public BedDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_action_region_bedroom";
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NORMAL;
				int num = 0;
				List<Sleepable> worldItems2 = Components.Sleepables.GetWorldItems(base.worldID);
				for (int i = 0; i < worldItems2.Count; i++)
				{
					if (worldItems2[i].GetComponent<Assignable>() != null && worldItems2[i].GetComponent<Clinic>() == null)
					{
						num++;
					}
				}
				if (num < worldItems.Count)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NOT_ENOUGH_BEDS;
				}
			}
			return result;
		}

		public override string GetAverageValueString()
		{
			return Components.Sleepables.GetWorldItems(base.worldID).FindAll((Sleepable match) => match.GetComponent<Assignable>() != null).Count + "/" + Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count;
		}
	}

	public class FoodDiagnostic : ColonyDiagnostic
	{
		public FoodDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<KCalTracker>(worldID);
			icon = "icon_category_food";
			trackerSampleCountSeconds = 150f;
			presentationSetting = PresentationSetting.CurrentValue;
		}

		public override DiagnosticResult Evaluate()
		{
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
			{
				return result;
			}
			if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.NORMAL;
				if (tracker.GetDataTimeLength() < 10f)
				{
					result.opinion = DiagnosticResult.Opinion.Normal;
					result.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
				}
				else if (tracker.GetAverageValue(trackerSampleCountSeconds) == 0f)
				{
					result.opinion = DiagnosticResult.Opinion.Bad;
					result.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.NO_FOOD;
				}
				else
				{
					int num = 3000;
					if ((float)worldItems.Count * (1000f * (float)num) > tracker.GetAverageValue(trackerSampleCountSeconds))
					{
						result.opinion = DiagnosticResult.Opinion.Warning;
						float currentValue = tracker.GetCurrentValue();
						float f = (float)Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count * -1000000f;
						result.Message = string.Format(MISC.NOTIFICATIONS.FOODLOW.TOOLTIP, GameUtil.GetFormattedCalories(currentValue), GameUtil.GetFormattedCalories(Mathf.Abs(f)));
					}
				}
				foreach (MinionIdentity item in worldItems)
				{
					CalorieMonitor.Instance sMI = item.GetSMI<CalorieMonitor.Instance>();
					if (sMI.IsInsideState(sMI.sm.hungry.starving))
					{
						result.opinion = DiagnosticResult.Opinion.Bad;
						result.Message = UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.HUNGRY;
						result.clickThroughTarget = new Tuple<Vector3, GameObject>(sMI.gameObject.transform.position, sMI.gameObject);
					}
				}
			}
			return result;
		}

		public override string GetCurrentValueString()
		{
			return GameUtil.GetFormattedCalories(tracker.GetCurrentValue());
		}
	}

	public class FloatingRocketDiagnostic : ColonyDiagnostic
	{
		public FloatingRocketDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_action_dig";
		}

		public override DiagnosticResult Evaluate()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.worldID);
			Clustercraft component = world.gameObject.GetComponent<Clustercraft>();
			DiagnosticResult result = default(DiagnosticResult);
			if (IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
			{
				return result;
			}
			result.opinion = DiagnosticResult.Opinion.Normal;
			if (world.ParentWorldId == ClusterManager.INVALID_WORLD_IDX || world.ParentWorldId == world.id)
			{
				result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_FLIGHT;
				if (component.Destination == component.Location)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_DESTINATION;
				}
				else if (component.Speed == 0f)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_SPEED;
				}
			}
			else
			{
				result.Message = UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_LANDED;
			}
			return result;
		}
	}

	public class RocketFuelDiagnostic : ColonyDiagnostic
	{
		public RocketFuelDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<RocketFuelTracker>(worldID);
			icon = "icon_action_dig";
		}

		public override DiagnosticResult Evaluate()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.worldID);
			Clustercraft component = world.gameObject.GetComponent<Clustercraft>();
			DiagnosticResult result = default(DiagnosticResult);
			if (IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
			{
				return result;
			}
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.NORMAL;
			if (component.ModuleInterface.FuelRemaining == 0f)
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.ROCKETFUELDIAGNOSTIC.WARNING;
			}
			return result;
		}
	}

	public class RocketOxidizerDiagnostic : ColonyDiagnostic
	{
		public RocketOxidizerDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.ALL_NAME)
		{
			tracker = TrackerTool.Instance.GetWorldTracker<RocketOxidizerTracker>(worldID);
			icon = "icon_action_dig";
		}

		public override DiagnosticResult Evaluate()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(base.worldID);
			Clustercraft component = world.gameObject.GetComponent<Clustercraft>();
			DiagnosticResult result = default(DiagnosticResult);
			if (IgnoreRocketsWithNoCrewRequested(base.worldID, out result))
			{
				return result;
			}
			result.opinion = DiagnosticResult.Opinion.Normal;
			result.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.NORMAL;
			RocketEngine engine = component.ModuleInterface.GetEngine();
			if (component.ModuleInterface.OxidizerPowerRemaining == 0f && engine != null && engine.requireOxidizer)
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.ROCKETOXIDIZERDIAGNOSTIC.WARNING;
			}
			return result;
		}
	}

	public class ToiletDiagnostic : ColonyDiagnostic
	{
		public ToiletDiagnostic(int worldID)
			: base(worldID, UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.ALL_NAME)
		{
			icon = "icon_action_region_toilet";
			tracker = TrackerTool.Instance.GetWorldTracker<WorkingToiletTracker>(worldID);
		}

		public override DiagnosticResult Evaluate()
		{
			List<IUsable> worldItems = Components.Toilets.GetWorldItems(base.worldID);
			List<MinionIdentity> worldItems2 = Components.LiveMinionIdentities.GetWorldItems(base.worldID);
			DiagnosticResult result = default(DiagnosticResult);
			if (worldItems2.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS;
			}
			else if (worldItems.Count == 0)
			{
				result.opinion = DiagnosticResult.Opinion.Warning;
				result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_TOILETS;
			}
			else
			{
				result.opinion = DiagnosticResult.Opinion.Normal;
				result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NORMAL;
				if (tracker.GetDataTimeLength() > 10f && tracker.GetAverageValue(trackerSampleCountSeconds) <= 0f)
				{
					result.opinion = DiagnosticResult.Opinion.Warning;
					result.Message = UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_WORKING_TOILETS;
				}
			}
			return result;
		}

		public override string GetAverageValueString()
		{
			List<IUsable> worldItems = Components.Toilets.GetWorldItems(base.worldID);
			int num = worldItems.Count;
			for (int i = 0; i < worldItems.Count; i++)
			{
				if (!worldItems[i].IsUsable())
				{
					num--;
				}
			}
			return num + ":" + Components.LiveMinionIdentities.GetWorldItems(base.worldID).Count;
		}
	}

	public abstract class ColonyDiagnostic
	{
		public enum PresentationSetting
		{
			AverageValue,
			CurrentValue
		}

		public struct DiagnosticResult
		{
			public enum Opinion
			{
				Bad,
				Warning,
				Normal,
				Good
			}

			public Opinion opinion;

			public Tuple<Vector3, GameObject> clickThroughTarget;

			private string message;

			public string Message
			{
				get
				{
					string result = "";
					switch (opinion)
					{
					case Opinion.Warning:
						result = "<color=" + Constants.WARNING_COLOR_STR + ">" + message + "</color>";
						break;
					case Opinion.Bad:
						result = "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + message + "</color>";
						break;
					case Opinion.Normal:
						result = message;
						break;
					case Opinion.Good:
						result = string.Concat("<color=", Constants.POSITIVE_COLOR, ">", message, "</color>");
						break;
					}
					return result;
				}
				set
				{
					message = value;
				}
			}

			public DiagnosticResult(string message, Opinion opinion)
			{
				this.message = message;
				this.opinion = opinion;
				clickThroughTarget = null;
			}
		}

		public string name;

		public string id;

		public string icon = "icon_errand_operate";

		public PresentationSetting presentationSetting = PresentationSetting.AverageValue;

		private DiagnosticResult latestResult = new DiagnosticResult(UI.COLONY_DIAGNOSTICS.NO_DATA, DiagnosticResult.Opinion.Normal);

		public Dictionary<DiagnosticResult.Opinion, Color> colors = new Dictionary<DiagnosticResult.Opinion, Color>();

		public Tracker tracker;

		protected float trackerSampleCountSeconds = 4f;

		public int worldID
		{
			get;
			protected set;
		}

		public DiagnosticResult LatestResult
		{
			get
			{
				return latestResult;
			}
			private set
			{
				latestResult = value;
			}
		}

		public ColonyDiagnostic(int worldID, string name)
		{
			this.worldID = worldID;
			this.name = name;
			id = GetType().Name;
			colors = new Dictionary<DiagnosticResult.Opinion, Color>();
			colors.Add(DiagnosticResult.Opinion.Bad, Constants.NEGATIVE_COLOR);
			colors.Add(DiagnosticResult.Opinion.Good, Constants.POSITIVE_COLOR);
			colors.Add(DiagnosticResult.Opinion.Normal, Constants.NEUTRAL_COLOR);
			colors.Add(DiagnosticResult.Opinion.Warning, Constants.WARNING_COLOR);
		}

		public virtual string GetAverageValueString()
		{
			if (tracker != null)
			{
				return tracker.FormatValueString(Mathf.Round(tracker.GetAverageValue(trackerSampleCountSeconds)));
			}
			return "";
		}

		public virtual string GetCurrentValueString()
		{
			return "";
		}

		public abstract DiagnosticResult Evaluate();

		public void SetResult(DiagnosticResult result)
		{
			LatestResult = result;
		}
	}

	public static ColonyDiagnosticUtility Instance;

	private Dictionary<int, List<ColonyDiagnostic>> worldDiagnostics = new Dictionary<int, List<ColonyDiagnostic>>();

	[Serialize]
	public Dictionary<int, Dictionary<string, DisplaySetting>> diagnosticDisplaySettings = new Dictionary<int, Dictionary<string, DisplaySetting>>();

	[Serialize]
	private Dictionary<string, float> diagnosticTutorialStatus = new Dictionary<string, float>
	{
		{
			typeof(ToiletDiagnostic).Name,
			450f
		},
		{
			typeof(BedDiagnostic).Name,
			900f
		},
		{
			typeof(BreathabilityDiagnostic).Name,
			1800f
		},
		{
			typeof(FoodDiagnostic).Name,
			3000f
		},
		{
			typeof(FarmDiagnostic).Name,
			6000f
		},
		{
			typeof(StressDiagnostic).Name,
			9000f
		},
		{
			typeof(PowerUseDiagnostic).Name,
			12000f
		},
		{
			typeof(BatteryDiagnostic).Name,
			12000f
		}
	};

	private bool ignoreFirstUpdate = true;

	public ColonyDiagnostic.DiagnosticResult.Opinion GetWorldDiagnosticResult(int worldID)
	{
		ColonyDiagnostic.DiagnosticResult.Opinion opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Good;
		foreach (ColonyDiagnostic item in worldDiagnostics[worldID])
		{
			DisplaySetting displaySetting = Instance.diagnosticDisplaySettings[worldID][item.id];
			if (displaySetting != DisplaySetting.Never && !Instance.IsDiagnosticTutorialDisabled(item.id))
			{
				switch (diagnosticDisplaySettings[worldID][item.id])
				{
				case DisplaySetting.Always:
				case DisplaySetting.AlertOnly:
				{
					int num = Math.Min((int)opinion, (int)item.LatestResult.opinion);
					opinion = (ColonyDiagnostic.DiagnosticResult.Opinion)num;
					break;
				}
				}
			}
		}
		return opinion;
	}

	public string GetWorldDiagnosticResultTooltip(int worldID)
	{
		string text = "";
		foreach (ColonyDiagnostic item in worldDiagnostics[worldID])
		{
			DisplaySetting displaySetting = Instance.diagnosticDisplaySettings[worldID][item.id];
			if (displaySetting == DisplaySetting.Never || Instance.IsDiagnosticTutorialDisabled(item.id))
			{
				continue;
			}
			switch (diagnosticDisplaySettings[worldID][item.id])
			{
			case DisplaySetting.Always:
			case DisplaySetting.AlertOnly:
				if (item.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
				{
					text = text + "\n" + item.LatestResult.Message;
				}
				break;
			}
		}
		return text;
	}

	public bool IsDiagnosticTutorialDisabled(string id)
	{
		if (Instance.diagnosticTutorialStatus.ContainsKey(id) && GameClock.Instance.GetTime() < Instance.diagnosticTutorialStatus[id])
		{
			return true;
		}
		return false;
	}

	public void ClearDiagnosticTutorialSetting(string id)
	{
		if (Instance.diagnosticTutorialStatus.ContainsKey(id))
		{
			Instance.diagnosticTutorialStatus[id] = -1f;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (int worldID in ClusterManager.Instance.GetWorldIDs())
		{
			AddWorld(worldID);
		}
		ClusterManager.Instance.Subscribe(-1280433810, Refresh);
		ClusterManager.Instance.Subscribe(-1078710002, RemoveWorld);
	}

	private void Refresh(object data)
	{
		int worldID = (int)data;
		AddWorld(worldID);
	}

	private void RemoveWorld(object data)
	{
		int key = (int)data;
		if (diagnosticDisplaySettings.Remove(key))
		{
			worldDiagnostics.Remove(key);
		}
	}

	public ColonyDiagnostic GetDiagnostic(string id, int worldID)
	{
		return worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match.id == id);
	}

	public T GetDiagnostic<T>(int worldID) where T : ColonyDiagnostic
	{
		return (T)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is T);
	}

	public string GetDiagnosticName(string id)
	{
		foreach (KeyValuePair<int, List<ColonyDiagnostic>> worldDiagnostic in worldDiagnostics)
		{
			foreach (ColonyDiagnostic item in worldDiagnostic.Value)
			{
				if (item.id == id)
				{
					return item.name;
				}
			}
		}
		Debug.LogWarning("Cannot locate name of diagnostic " + id + " because no worlds have a diagnostic with that id ");
		return "";
	}

	public ChoreGroupDiagnostic GetChoreGroupDiagnostic(int worldID, ChoreGroup choreGroup)
	{
		return (ChoreGroupDiagnostic)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is ChoreGroupDiagnostic && ((ChoreGroupDiagnostic)match).choreGroup == choreGroup);
	}

	public WorkTimeDiagnostic GetWorkTimeDiagnostic(int worldID, ChoreGroup choreGroup)
	{
		return (WorkTimeDiagnostic)worldDiagnostics[worldID].Find((ColonyDiagnostic match) => match is WorkTimeDiagnostic && ((WorkTimeDiagnostic)match).choreGroup == choreGroup);
	}

	public void AddWorld(int worldID)
	{
		if (!diagnosticDisplaySettings.ContainsKey(worldID))
		{
			diagnosticDisplaySettings.Add(worldID, new Dictionary<string, DisplaySetting>());
		}
		List<ColonyDiagnostic> list = new List<ColonyDiagnostic>();
		list.Add(new BreathabilityDiagnostic(worldID));
		list.Add(new FoodDiagnostic(worldID));
		list.Add(new StressDiagnostic(worldID));
		if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
		{
			list.Add(new FloatingRocketDiagnostic(worldID));
			list.Add(new RocketFuelDiagnostic(worldID));
			list.Add(new RocketOxidizerDiagnostic(worldID));
		}
		else
		{
			list.Add(new BedDiagnostic(worldID));
			list.Add(new ToiletDiagnostic(worldID));
			list.Add(new PowerUseDiagnostic(worldID));
			list.Add(new BatteryDiagnostic(worldID));
			list.Add(new IdleDiagnostic(worldID));
			list.Add(new TrappedDuplicantDiagnostic(worldID));
			list.Add(new FarmDiagnostic(worldID));
			list.Add(new EntombedDiagnostic(worldID));
			for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
			{
				list.Add(new ChoreGroupDiagnostic(worldID, Db.Get().ChoreGroups[i]));
				list.Add(new WorkTimeDiagnostic(worldID, Db.Get().ChoreGroups[i]));
			}
			list.Add(new AllChoresDiagnostic(worldID));
			list.Add(new AllWorkTimeDiagnostic(worldID));
		}
		worldDiagnostics.Add(worldID, list);
		foreach (ColonyDiagnostic item in list)
		{
			if (!diagnosticDisplaySettings[worldID].ContainsKey(item.id))
			{
				diagnosticDisplaySettings[worldID].Add(item.id, DisplaySetting.AlertOnly);
			}
		}
		diagnosticDisplaySettings[worldID][typeof(BreathabilityDiagnostic).Name] = DisplaySetting.Always;
		diagnosticDisplaySettings[worldID][typeof(FoodDiagnostic).Name] = DisplaySetting.Always;
		diagnosticDisplaySettings[worldID][typeof(StressDiagnostic).Name] = DisplaySetting.Always;
		diagnosticDisplaySettings[worldID][typeof(IdleDiagnostic).Name] = DisplaySetting.Never;
		if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
		{
			diagnosticDisplaySettings[worldID][typeof(FloatingRocketDiagnostic).Name] = DisplaySetting.Always;
			diagnosticDisplaySettings[worldID][typeof(RocketFuelDiagnostic).Name] = DisplaySetting.Always;
			diagnosticDisplaySettings[worldID][typeof(RocketOxidizerDiagnostic).Name] = DisplaySetting.Always;
		}
	}

	public void Sim4000ms(float dt)
	{
		ColonyDiagnostic.DiagnosticResult diagnosticResult = default(ColonyDiagnostic.DiagnosticResult);
		if (ignoreFirstUpdate)
		{
			diagnosticResult.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
			diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
			diagnosticResult.clickThroughTarget = null;
		}
		foreach (KeyValuePair<int, List<ColonyDiagnostic>> worldDiagnostic in worldDiagnostics)
		{
			foreach (ColonyDiagnostic item in worldDiagnostic.Value)
			{
				item.SetResult(ignoreFirstUpdate ? diagnosticResult : item.Evaluate());
			}
		}
		if (ignoreFirstUpdate)
		{
			ignoreFirstUpdate = false;
		}
	}

	private static bool PastNewBuildingGracePeriod(Transform building)
	{
		BuildingComplete component = building.GetComponent<BuildingComplete>();
		if (component != null && GameClock.Instance.GetTime() - component.creationTime < 600f)
		{
			return false;
		}
		return true;
	}

	private static bool IgnoreRocketsWithNoCrewRequested(int worldID, out ColonyDiagnostic.DiagnosticResult result)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(worldID);
		result = default(ColonyDiagnostic.DiagnosticResult);
		if (world.IsModuleInterior)
		{
			for (int i = 0; i < Components.Clustercrafts.Count; i++)
			{
				WorldContainer interiorWorld = Components.Clustercrafts[i].ModuleInterface.GetInteriorWorld();
				if (!(interiorWorld == null) && interiorWorld.id == worldID)
				{
					PassengerRocketModule passengerModule = Components.Clustercrafts[i].ModuleInterface.GetPassengerModule();
					if (passengerModule != null && !passengerModule.ShouldCrewGetIn())
					{
						result = default(ColonyDiagnostic.DiagnosticResult);
						result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
						result.Message = UI.COLONY_DIAGNOSTICS.NO_MINIONS_REQUESTED;
						return true;
					}
				}
			}
		}
		return false;
	}
}
