using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocketCluster : StateMachineComponent<LaunchableRocketCluster.StatesInstance>, ILaunchableRocket
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LaunchableRocketCluster, object>.GameInstance
	{
		private const float TAKEOFF_ACCEL_POWER = 4f;

		private const float MAX_ACCELERATION_DISTANCE = 25f;

		private const float TAKEOFF_ACCEL_POWER_INV = 0.25f;

		private const float WARMUP_TIME = 5f;

		public const float EXTRA_CEILING_HEIGHT = 1f;

		private float landingDistancePartsTimeConstant;

		private float partsLaunchSpeedRatio => base.master.parts.Count / 2;

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
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			Debug.Assert(!DlcManager.IsExpansion1Active());
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			spacecraftFromLaunchConditionManager.SetState(state);
		}

		public Spacecraft.MissionState GetMissionState()
		{
			Debug.Assert(!DlcManager.IsExpansion1Active());
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			return spacecraftFromLaunchConditionManager.state;
		}

		public bool IsGrounded()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Grounded;
		}

		public bool IsNotSpaceBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Grounded || component.Status == Clustercraft.CraftStatus.Landing;
		}

		public bool IsNotGroundBound()
		{
			Clustercraft component = base.smi.master.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Launching || component.Status == Clustercraft.CraftStatus.InFlight;
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
			foreach (GameObject engine in base.master.GetEngines())
			{
				engine.Trigger(-1358394196);
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
			}
		}

		public void LaunchLoop(float dt)
		{
			base.master.isLanding = false;
			if (base.sm.warmupTimeRemaining.Get(this) > 0f)
			{
				base.sm.warmupTimeRemaining.Delta(0f - dt, this);
			}
			else
			{
				float num = base.smi.timeinstate - 5f;
				float distanceAboveGround = DistanceAboveGround;
				if (DistanceAboveGround < 25f)
				{
					DistanceAboveGround = Mathf.Pow(num / partsLaunchSpeedRatio, 4f);
					base.master.rocketSpeed = (DistanceAboveGround - distanceAboveGround) / dt;
				}
				else
				{
					if (base.master.rocketSpeed == 0f)
					{
						base.master.rocketSpeed = 25f - Mathf.Pow((base.smi.timeinstate - 1f) / partsLaunchSpeedRatio, 4f);
					}
					DistanceAboveGround += base.master.rocketSpeed * dt;
				}
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
			DistanceAboveGround = base.sm.distanceToSpace.Get(base.smi) + 1f;
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
			float f = (DistanceAboveGround = base.master.InitialFlightAnimOffsetForLanding());
			landingDistancePartsTimeConstant = Mathf.Pow(f, 0.25f) * partsLaunchSpeedRatio;
			base.master.isLanding = true;
			base.master.rocketSpeed = 0f;
			List<GameObject> engines = base.smi.master.GetEngines();
			if (engines.Count > 0)
			{
				engines[engines.Count - 1].Trigger(-1358394196);
			}
		}

		public void LandingLoop(float dt)
		{
			base.master.isLanding = true;
			if (base.smi.timeinstate > landingDistancePartsTimeConstant)
			{
				DistanceAboveGround = 0f;
			}
			else
			{
				DistanceAboveGround = Mathf.Pow((0f - base.smi.timeinstate + landingDistancePartsTimeConstant) / partsLaunchSpeedRatio, 4f);
			}
			base.master.rocketSpeed = DistanceAboveGround - Mathf.Pow((0f - (base.smi.timeinstate - 1f) + landingDistancePartsTimeConstant) / partsLaunchSpeedRatio, 4f);
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

		private int UpdatePartsAnimPositionsAndDamage()
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
					bool flag = Grid.IsValidCell(Grid.PosToCell(positionIncludingOffset));
					bool flag2 = ClusterManager.Instance.activeWorldId == myWorldId;
					if (component.enabled != flag2)
					{
						component.enabled = flag2;
					}
					if (flag)
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
				distanceToSpace.Set(ConditionFlightPathIsClear.PadPositionDistanceToCeiling(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.gameObject), smi);
				smi.GoTo(not_grounded.launch_loop);
			});
			not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Update(delegate(StatesInstance smi, float dt)
			{
				smi.LaunchLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK).ParamTransition(distanceAboveGround, not_grounded.space, (StatesInstance smi, float p) => p >= distanceToSpace.Get(smi));
			not_grounded.space.EnterTransition(not_grounded.landing_setup, (StatesInstance smi) => smi.IsNotSpaceBound()).EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Enter(delegate(StatesInstance smi)
			{
				smi.FinalizeLaunch();
			});
			not_grounded.landing_setup.Enter(delegate(StatesInstance smi)
			{
				smi.SetupLanding();
				smi.GoTo(not_grounded.landing_loop);
			});
			not_grounded.landing_loop.Update(delegate(StatesInstance smi, float dt)
			{
				smi.LandingLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK).ParamTransition(distanceAboveGround, not_grounded.land, (StatesInstance smi, float p) => p < 0.0025f);
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
				}
				smi.GoTo(grounded);
			});
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
					GameObject gameObject = Grid.Objects[num, 1];
					if (gameObject == null || !gameObject.HasTag(GameTags.DontBlockRockets))
					{
						WorldDamage.Instance.ApplyDamage(num, 10000f, num, BUILDINGS.DAMAGESOURCES.ROCKET, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
					}
				}
				else
				{
					if (!Grid.FakeFloor[num])
					{
						continue;
					}
					GameObject gameObject2 = Grid.Objects[num, 39];
					if (gameObject2 != null)
					{
						BuildingHP component2 = gameObject2.GetComponent<BuildingHP>();
						if (component2 != null)
						{
							gameObject2.Trigger(-794517298, new BuildingHP.DamageSourceInfo
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

	public bool isLanding
	{
		get;
		private set;
	}

	public float rocketSpeed
	{
		get;
		private set;
	}

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

	private float InitialFlightAnimOffsetForLanding()
	{
		int num = Grid.PosToCell(base.gameObject);
		WorldContainer world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[num]);
		float num2 = world.maximumBounds.y - base.gameObject.transform.GetPosition().y;
		float num3 = 0f;
		foreach (Ref<RocketModuleCluster> part in parts)
		{
			num3 += (float)part.Get().GetComponent<Building>().Def.HeightInCells;
		}
		return num2 + num3 + 100f;
	}
}
