using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocketCluster : StateMachineComponent<LaunchableRocketCluster.StatesInstance>, ILaunchableRocket
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LaunchableRocketCluster, object>.GameInstance
	{
		public class Tuning : TuningData<Tuning>
		{
			public float takeoffAccelPower = 4f;

			public float maxAccelerationDistance = 25f;

			public float warmupTime = 5f;

			public float heightSpeedPower = 0.5f;

			public float heightSpeedFactor = 4f;

			public int maxAccelHeight = 40;
		}

		private float takeoffAccelPowerInv;

		private float constantVelocityPhase_maxSpeed;

		private float heightLaunchSpeedRatio => Mathf.Pow(base.master.GetRocketHeight(), TuningData<Tuning>.Get().heightSpeedPower) * TuningData<Tuning>.Get().heightSpeedFactor;

		public float DistanceAboveGround
		{
			get
			{
				return base.sm.distanceAboveGround.Get(this);
			}
			set
			{
				base.sm.distanceAboveGround.Set(value, this);
			}
		}

		public StatesInstance(LaunchableRocketCluster master)
			: base(master)
		{
			takeoffAccelPowerInv = 1f / TuningData<Tuning>.Get().takeoffAccelPower;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).SetState(state);
		}

		public Spacecraft.MissionState GetMissionState()
		{
			Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
			return SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).state;
		}

		public bool IsGrounded()
		{
			return base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Grounded;
		}

		public bool IsNotSpaceBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component.Status != 0)
			{
				return component.Status == Clustercraft.CraftStatus.Landing;
			}
			return true;
		}

		public bool IsNotGroundBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component.Status != Clustercraft.CraftStatus.Launching)
			{
				return component.Status == Clustercraft.CraftStatus.InFlight;
			}
			return true;
		}

		public void SetupLaunch()
		{
			base.master.isLanding = false;
			base.master.rocketSpeed = 0f;
			base.sm.warmupTimeRemaining.Set(5f, this);
			base.sm.distanceAboveGround.Set(0f, this);
			if (base.master.soundSpeakerObject == null)
			{
				base.master.soundSpeakerObject = new GameObject("rocketSpeaker");
				base.master.soundSpeakerObject.transform.SetParent(base.master.gameObject.transform);
			}
			foreach (Ref<RocketModuleCluster> part in base.master.parts)
			{
				if (part != null)
				{
					base.master.takeOffLocation = Grid.PosToCell(base.master.gameObject);
					part.Get().Trigger(-1277991738, base.master.gameObject);
				}
			}
			CraftModuleInterface craftInterface = base.master.GetComponent<RocketModuleCluster>().CraftInterface;
			if (craftInterface != null)
			{
				craftInterface.Trigger(-1277991738, base.master.gameObject);
				WorldContainer component = craftInterface.GetComponent<WorldContainer>();
				foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(component.id))
				{
					Game.Instance.Trigger(586301400, worldItem);
				}
			}
			constantVelocityPhase_maxSpeed = 0f;
		}

		public void LaunchLoop(float dt)
		{
			base.master.isLanding = false;
			if (constantVelocityPhase_maxSpeed == 0f)
			{
				float num = Mathf.Pow((Mathf.Pow(TuningData<Tuning>.Get().maxAccelerationDistance, takeoffAccelPowerInv) * heightLaunchSpeedRatio - 0.033f) / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				constantVelocityPhase_maxSpeed = (TuningData<Tuning>.Get().maxAccelerationDistance - num) / 0.033f;
			}
			if (base.sm.warmupTimeRemaining.Get(this) > 0f)
			{
				base.sm.warmupTimeRemaining.Delta(0f - dt, this);
			}
			else if (DistanceAboveGround < TuningData<Tuning>.Get().maxAccelerationDistance)
			{
				float num2 = Mathf.Pow(DistanceAboveGround, takeoffAccelPowerInv) * heightLaunchSpeedRatio;
				num2 += dt;
				DistanceAboveGround = Mathf.Pow(num2 / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				float num3 = Mathf.Pow((num2 - 0.033f) / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				base.master.rocketSpeed = (DistanceAboveGround - num3) / 0.033f;
			}
			else
			{
				base.master.rocketSpeed = constantVelocityPhase_maxSpeed;
				DistanceAboveGround += base.master.rocketSpeed * dt;
			}
			UpdateSoundSpeakerObject();
			if (UpdatePartsAnimPositionsAndDamage() == 0)
			{
				base.smi.GoTo(base.sm.not_grounded.space);
			}
		}

		public void FinalizeLaunch()
		{
			base.master.rocketSpeed = 0f;
			DistanceAboveGround = base.sm.distanceToSpace.Get(base.smi);
			foreach (Ref<RocketModuleCluster> part in base.master.parts)
			{
				if (part != null && !(part.Get() == null))
				{
					RocketModuleCluster rocketModuleCluster = part.Get();
					rocketModuleCluster.GetComponent<KBatchedAnimController>().Offset = Vector3.up * DistanceAboveGround;
					rocketModuleCluster.GetComponent<KBatchedAnimController>().enabled = false;
					rocketModuleCluster.GetComponent<RocketModule>().MoveToSpace();
				}
			}
			base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.InFlight);
		}

		public void SetupLanding()
		{
			float num2 = (DistanceAboveGround = base.master.InitialFlightAnimOffsetForLanding());
			base.sm.warmupTimeRemaining.Set(2f, this);
			base.master.isLanding = true;
			base.master.rocketSpeed = 0f;
			constantVelocityPhase_maxSpeed = 0f;
		}

		public void LandingLoop(float dt)
		{
			base.master.isLanding = true;
			if (constantVelocityPhase_maxSpeed == 0f)
			{
				float num = Mathf.Pow((Mathf.Pow(TuningData<Tuning>.Get().maxAccelerationDistance, takeoffAccelPowerInv) * heightLaunchSpeedRatio - 0.033f) / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				constantVelocityPhase_maxSpeed = (num - TuningData<Tuning>.Get().maxAccelerationDistance) / 0.033f;
			}
			if (DistanceAboveGround > TuningData<Tuning>.Get().maxAccelerationDistance)
			{
				base.master.rocketSpeed = constantVelocityPhase_maxSpeed;
				DistanceAboveGround += base.master.rocketSpeed * dt;
			}
			else if (DistanceAboveGround > 0.0025f)
			{
				float num2 = Mathf.Pow(DistanceAboveGround, takeoffAccelPowerInv) * heightLaunchSpeedRatio;
				num2 -= dt;
				DistanceAboveGround = Mathf.Pow(num2 / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				float num3 = Mathf.Pow((num2 - 0.033f) / heightLaunchSpeedRatio, TuningData<Tuning>.Get().takeoffAccelPower);
				base.master.rocketSpeed = (DistanceAboveGround - num3) / 0.033f;
			}
			else if (base.sm.warmupTimeRemaining.Get(this) > 0f)
			{
				base.sm.warmupTimeRemaining.Delta(0f - dt, this);
				DistanceAboveGround = 0f;
			}
			UpdateSoundSpeakerObject();
			UpdatePartsAnimPositionsAndDamage();
		}

		public void FinalizeLanding()
		{
			GetComponent<KSelectable>().IsSelectable = true;
			base.master.rocketSpeed = 0f;
			DistanceAboveGround = 0f;
			foreach (Ref<RocketModuleCluster> part in base.smi.master.parts)
			{
				if (part != null && !(part.Get() == null))
				{
					part.Get().GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
				}
			}
			base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.Grounded);
		}

		private void UpdateSoundSpeakerObject()
		{
			if (base.master.soundSpeakerObject == null)
			{
				base.master.soundSpeakerObject = new GameObject("rocketSpeaker");
				base.master.soundSpeakerObject.transform.SetParent(base.gameObject.transform);
			}
			base.master.soundSpeakerObject.transform.SetLocalPosition(DistanceAboveGround * Vector3.up);
		}

		public int UpdatePartsAnimPositionsAndDamage(bool doDamage = true)
		{
			int myWorldId = base.gameObject.GetMyWorldId();
			if (myWorldId == -1)
			{
				return 0;
			}
			LaunchPad currentPad = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
			if (currentPad != null)
			{
				myWorldId = currentPad.GetMyWorldId();
			}
			int num = 0;
			foreach (Ref<RocketModuleCluster> part in base.master.parts)
			{
				if (part != null)
				{
					RocketModuleCluster rocketModuleCluster = part.Get();
					KBatchedAnimController component = rocketModuleCluster.GetComponent<KBatchedAnimController>();
					component.Offset = Vector3.up * DistanceAboveGround;
					Vector3 positionIncludingOffset = component.PositionIncludingOffset;
					int num2 = Grid.PosToCell(positionIncludingOffset);
					bool flag = Grid.IsValidCell(num2);
					bool flag2 = flag && Grid.WorldIdx[num2] == myWorldId;
					if (component.enabled != flag2)
					{
						component.enabled = flag2;
					}
					if (doDamage && flag)
					{
						num++;
						States.DoWorldDamage(rocketModuleCluster.gameObject, positionIncludingOffset, myWorldId);
					}
				}
			}
			return num;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LaunchableRocketCluster>
	{
		public class NotGroundedStates : State
		{
			public State launch_setup;

			public State launch_loop;

			public State space;

			public State landing_setup;

			public State landing_loop;

			public State land;
		}

		public FloatParameter warmupTimeRemaining;

		public FloatParameter distanceAboveGround;

		public FloatParameter distanceToSpace;

		public State grounded;

		public NotGroundedStates not_grounded;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			base.serializable = SerializeType.Both_DEPRECATED;
			grounded.EventTransition(GameHashes.DoLaunchRocket, not_grounded.launch_setup).EnterTransition(not_grounded.launch_loop, (StatesInstance smi) => smi.IsNotGroundBound()).Enter(delegate(StatesInstance smi)
			{
				smi.FinalizeLanding();
			});
			not_grounded.launch_setup.Enter(delegate(StatesInstance smi)
			{
				smi.SetupLaunch();
				distanceToSpace.Set(ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.gameObject), smi);
				smi.GoTo(not_grounded.launch_loop);
			});
			not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Enter(delegate(StatesInstance smi)
			{
				smi.UpdatePartsAnimPositionsAndDamage(doDamage: false);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.LaunchLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK)
				.ParamTransition(distanceAboveGround, not_grounded.space, (StatesInstance smi, float p) => p >= distanceToSpace.Get(smi));
			not_grounded.space.EnterTransition(not_grounded.landing_setup, (StatesInstance smi) => smi.IsNotSpaceBound()).EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Enter(delegate(StatesInstance smi)
			{
				smi.FinalizeLaunch();
			});
			not_grounded.landing_setup.Enter(delegate(StatesInstance smi)
			{
				smi.SetupLanding();
				smi.GoTo(not_grounded.landing_loop);
			});
			not_grounded.landing_loop.Enter(delegate(StatesInstance smi)
			{
				smi.UpdatePartsAnimPositionsAndDamage(doDamage: false);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.LandingLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK).ParamTransition(distanceAboveGround, not_grounded.land, IsFullyLanded)
				.ParamTransition(warmupTimeRemaining, not_grounded.land, IsFullyLanded);
			not_grounded.land.Enter(delegate(StatesInstance smi)
			{
				foreach (Ref<RocketModuleCluster> part in smi.master.parts)
				{
					if (part != null && !(part.Get() == null))
					{
						part.Get().Trigger(-887025858, smi.gameObject);
					}
				}
				CraftModuleInterface craftInterface = smi.master.GetComponent<RocketModuleCluster>().CraftInterface;
				if (craftInterface != null)
				{
					craftInterface.Trigger(-887025858, smi.gameObject);
					WorldContainer component = craftInterface.GetComponent<WorldContainer>();
					foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(component.id))
					{
						Game.Instance.Trigger(586301400, worldItem);
					}
				}
				smi.GoTo(grounded);
			});
		}

		public bool IsFullyLanded<T>(StatesInstance smi, T p)
		{
			if (distanceAboveGround.Get(smi) <= 0.0025f)
			{
				return warmupTimeRemaining.Get(smi) <= 0f;
			}
			return false;
		}

		public static void DoWorldDamage(GameObject part, Vector3 apparentPosition, int actualWorld)
		{
			OccupyArea component = part.GetComponent<OccupyArea>();
			component.UpdateOccupiedArea();
			CellOffset[] occupiedCellsOffsets = component.OccupiedCellsOffsets;
			foreach (CellOffset offset in occupiedCellsOffsets)
			{
				int num = Grid.OffsetCell(Grid.PosToCell(apparentPosition), offset);
				if (!Grid.IsValidCell(num) || Grid.WorldIdx[num] != Grid.WorldIdx[actualWorld])
				{
					continue;
				}
				if (Grid.Solid[num])
				{
					WorldDamage.Instance.ApplyDamage(num, 10000f, num, BUILDINGS.DAMAGESOURCES.ROCKET, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
				}
				else
				{
					if (!Grid.FakeFloor[num])
					{
						continue;
					}
					GameObject gameObject = Grid.Objects[num, 39];
					if (gameObject != null)
					{
						BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
						if (component2 != null)
						{
							gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
							{
								damage = component2.MaxHitPoints,
								source = BUILDINGS.DAMAGESOURCES.ROCKET,
								popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
							});
						}
					}
				}
			}
		}
	}

	[Serialize]
	private int takeOffLocation;

	private GameObject soundSpeakerObject;

	public IList<Ref<RocketModuleCluster>> parts => GetComponent<RocketModuleCluster>().CraftInterface.ClusterModules;

	public bool isLanding { get; private set; }

	public float rocketSpeed { get; private set; }

	public LaunchableRocketRegisterType registerType => LaunchableRocketRegisterType.Clustercraft;

	public GameObject LaunchableGameObject => base.gameObject;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Ref<RocketModuleCluster> part in parts)
		{
			if ((bool)part.Get().GetComponent<RocketEngineCluster>())
			{
				list.Add(part.Get().gameObject);
			}
		}
		return list;
	}

	private int GetRocketHeight()
	{
		int num = 0;
		foreach (Ref<RocketModuleCluster> part in parts)
		{
			num += part.Get().GetComponent<Building>().Def.HeightInCells;
		}
		return num;
	}

	private float InitialFlightAnimOffsetForLanding()
	{
		int num = Grid.PosToCell(base.gameObject);
		return ClusterManager.Instance.GetWorld(Grid.WorldIdx[num]).maximumBounds.y - base.gameObject.transform.GetPosition().y + (float)GetRocketHeight() + 100f;
	}
}
