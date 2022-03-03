using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class ClusterCometDetector : GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>
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
		public enum ClusterCometDetectorState
		{
			MeteorShower,
			BallisticObject,
			Rocket
		}

		public bool ShowWorkingStatus;

		private const float BEST_WARNING_TIME = 200f;

		private const float WORST_WARNING_TIME = 1f;

		private const float VARIANCE = 50f;

		private const int MAX_DISH_COUNT = 6;

		private const int INTERFERENCE_RADIUS = 15;

		[Serialize]
		private ClusterCometDetectorState detectorState;

		[Serialize]
		private float nextAccuracy;

		[Serialize]
		private Ref<Clustercraft> targetCraft;

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
			int myWorldId = this.GetMyWorldId();
			if (GetDetectorState() == ClusterCometDetectorState.MeteorShower)
			{
				SaveGame.Instance.GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(myWorldId, ref meteorShowers);
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
			if (GetDetectorState() == ClusterCometDetectorState.BallisticObject)
			{
				float num2 = float.MaxValue;
				foreach (ClusterTraveler clusterTraveler in Components.ClusterTravelers)
				{
					bool num3 = clusterTraveler.IsTraveling();
					bool flag = clusterTraveler.GetComponent<Clustercraft>() != null;
					if (num3 && !flag && clusterTraveler.GetDestinationWorldID() == myWorldId)
					{
						num2 = Mathf.Min(num2, clusterTraveler.TravelETA());
					}
				}
				UpdateDetectionState(num2 < detectTime, expectedDetectionForState);
			}
			if (GetDetectorState() != ClusterCometDetectorState.Rocket || targetCraft == null)
			{
				return;
			}
			Clustercraft clustercraft = targetCraft.Get();
			if (clustercraft.IsNullOrDestroyed())
			{
				return;
			}
			ClusterTraveler component = clustercraft.GetComponent<ClusterTraveler>();
			bool flag2 = false;
			if (clustercraft.Status != 0)
			{
				bool flag3 = component.GetDestinationWorldID() == myWorldId;
				bool flag4 = component.IsTraveling();
				bool flag5 = clustercraft.HasResourcesToMove();
				float num4 = component.TravelETA();
				flag2 = (flag3 && flag4 && flag5 && num4 < detectTime) || (!flag4 && flag3 && clustercraft.Status == Clustercraft.CraftStatus.Landing);
				if (!flag2)
				{
					ClusterGridEntity adjacentAsteroid = clustercraft.GetAdjacentAsteroid();
					flag2 = ((adjacentAsteroid != null) ? ClusterUtil.GetAsteroidWorldIdAtLocation(adjacentAsteroid.Location) : ClusterManager.INVALID_WORLD_IDX) == myWorldId && clustercraft.Status == Clustercraft.CraftStatus.Launching;
				}
			}
			UpdateDetectionState(flag2, expectedDetectionForState);
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

		public void SetDetectorState(ClusterCometDetectorState newState)
		{
			detectorState = newState;
		}

		public ClusterCometDetectorState GetDetectorState()
		{
			return detectorState;
		}

		public void SetClustercraftTarget(Clustercraft target)
		{
			if ((bool)target)
			{
				targetCraft = new Ref<Clustercraft>(target);
			}
			else
			{
				targetCraft = null;
			}
		}

		public Clustercraft GetClustercraftTarget()
		{
			return targetCraft?.Get();
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
