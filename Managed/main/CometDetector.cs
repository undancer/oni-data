using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class CometDetector : GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>
{
	public class Def : BaseDef
	{
	}

	public class OnStates : State
	{
		public State pre;

		public State loop;

		public WorkingStates working;

		public State pst;
	}

	public class WorkingStates : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public new class Instance : GameInstance
	{
		public bool ShowWorkingStatus;

		private const float BEST_WARNING_TIME = 200f;

		private const float WORST_WARNING_TIME = 1f;

		private const float VARIANCE = 50f;

		private const int MAX_DISH_COUNT = 6;

		private const int INTERFERENCE_RADIUS = 15;

		[Serialize]
		private float nextAccuracy;

		[Serialize]
		private Ref<LaunchConditionManager> targetCraft;

		private DetectorNetwork.Def detectorNetworkDef;

		private DetectorNetwork.Instance detectorNetwork;

		private List<GameplayEventInstance> meteorShowers = new List<GameplayEventInstance>();

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			detectorNetworkDef = new DetectorNetwork.Def();
			detectorNetworkDef.interferenceRadius = 15;
			detectorNetworkDef.worstWarningTime = 1f;
			detectorNetworkDef.bestWarningTime = 200f;
			detectorNetworkDef.bestNetworkSize = 6;
			targetCraft = new Ref<LaunchConditionManager>();
			RerollAccuracy();
		}

		public override void StartSM()
		{
			if (detectorNetwork == null)
			{
				detectorNetwork = (DetectorNetwork.Instance)detectorNetworkDef.CreateSMI(base.master);
			}
			detectorNetwork.StartSM();
			base.StartSM();
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			detectorNetwork.StopSM(reason);
		}

		public void UpdateDetectionState(bool currentDetection, bool expectedDetectionForState)
		{
			KPrefabID component = GetComponent<KPrefabID>();
			if (currentDetection)
			{
				component.AddTag(GameTags.Detecting);
			}
			else
			{
				component.RemoveTag(GameTags.Detecting);
			}
			if (currentDetection == expectedDetectionForState)
			{
				SetLogicSignal(currentDetection);
			}
		}

		public void ScanSky(bool expectedDetectionForState)
		{
			float detectTime = GetDetectTime();
			GetComponent<KPrefabID>();
			if (targetCraft.Get() == null)
			{
				SaveGame.Instance.GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(this.GetMyWorldId(), ref meteorShowers);
				float num = float.MaxValue;
				foreach (GameplayEventInstance meteorShower in meteorShowers)
				{
					MeteorShowerEvent.StatesInstance statesInstance = meteorShower.smi as MeteorShowerEvent.StatesInstance;
					if (statesInstance != null)
					{
						num = Mathf.Min(num, statesInstance.TimeUntilNextShower());
					}
				}
				meteorShowers.Clear();
				UpdateDetectionState(num < detectTime, expectedDetectionForState);
			}
			else
			{
				Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(targetCraft.Get());
				if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Destroyed)
				{
					targetCraft.Set(null);
					UpdateDetectionState(currentDetection: false, expectedDetectionForState);
				}
				else if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Launching || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.WaitingToLand || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Landing || (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Underway && spacecraftFromLaunchConditionManager.GetTimeLeft() <= detectTime))
				{
					UpdateDetectionState(currentDetection: true, expectedDetectionForState);
				}
				else
				{
					UpdateDetectionState(currentDetection: false, expectedDetectionForState);
				}
			}
		}

		public void RerollAccuracy()
		{
			nextAccuracy = Random.value;
		}

		public void SetLogicSignal(bool on)
		{
			GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);
		}

		public float GetDetectTime()
		{
			return detectorNetwork.GetDetectTimeRange().Lerp(nextAccuracy);
		}

		public void SetTargetCraft(LaunchConditionManager target)
		{
			targetCraft.Set(target);
		}

		public LaunchConditionManager GetTargetCraft()
		{
			return targetCraft.Get();
		}
	}

	public State off;

	public OnStates on;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => smi.GetComponent<Operational>().IsOperational).Update("Scan Sky", delegate(Instance smi, float dt)
		{
			smi.ScanSky(expectedDetectionForState: false);
		}, UpdateRate.SIM_4000ms);
		on.DefaultState(on.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.DetectorScanning).Enter("ToggleActive", delegate(Instance smi)
		{
			smi.GetComponent<Operational>().SetActive(value: true);
		})
			.Exit("ToggleActive", delegate(Instance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			});
		on.pre.PlayAnim("on_pre").OnAnimQueueComplete(on.loop);
		on.loop.PlayAnim("on", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, on.pst, (Instance smi) => !smi.GetComponent<Operational>().IsOperational).TagTransition(GameTags.Detecting, on.working)
			.Enter("UpdateLogic", delegate(Instance smi)
			{
				smi.UpdateDetectionState(smi.HasTag(GameTags.Detecting), expectedDetectionForState: false);
			})
			.Update("Scan Sky", delegate(Instance smi, float dt)
			{
				smi.ScanSky(expectedDetectionForState: false);
			});
		on.pst.PlayAnim("on_pst").OnAnimQueueComplete(off);
		on.working.DefaultState(on.working.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.IncomingMeteors).Enter("UpdateLogic", delegate(Instance smi)
		{
			smi.SetLogicSignal(on: true);
		})
			.Exit("UpdateLogic", delegate(Instance smi)
			{
				smi.SetLogicSignal(on: false);
			})
			.Update("Scan Sky", delegate(Instance smi, float dt)
			{
				smi.ScanSky(expectedDetectionForState: true);
			});
		on.working.pre.PlayAnim("detect_pre").OnAnimQueueComplete(on.working.loop);
		on.working.loop.PlayAnim("detect_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, on.working.pst, (Instance smi) => !smi.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.ActiveChanged, on.working.pst, (Instance smi) => !smi.GetComponent<Operational>().IsActive)
			.TagTransition(GameTags.Detecting, on.working.pst, on_remove: true);
		on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(on.loop).Enter("Reroll", delegate(Instance smi)
		{
			smi.RerollAccuracy();
		});
	}
}
