using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocket : StateMachineComponent<LaunchableRocket.StatesInstance>
{
	public enum RegisterType
	{
		Spacecraft,
		Clustercraft
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, LaunchableRocket, object>.GameInstance
	{
		private const float TAKEOFF_ACCEL_POWER = 4f;

		private const float TAKEOFF_ACCEL_POWER_INV = 0.25f;

		private const float WARMUP_TIME = 5f;

		public const float ORBIT_HEIGHT = 50f;

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

		public StatesInstance(LaunchableRocket master)
			: base(master)
		{
		}

		public void SetTagOnModules(Tag tag, bool set)
		{
			foreach (Ref<RocketModule> part in base.master.parts)
			{
				if (part != null)
				{
					if (set)
					{
						part.Get().AddTag(tag);
					}
					else
					{
						part.Get().RemoveTag(tag);
					}
				}
			}
			CraftModuleInterface craftInterface = GetComponent<RocketModule>().CraftInterface;
			if (craftInterface != null)
			{
				if (set)
				{
					craftInterface.AddTag(tag);
				}
				else
				{
					craftInterface.RemoveTag(tag);
				}
			}
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
			if (base.smi.master.registerType == RegisterType.Spacecraft)
			{
				Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
				return spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.WaitingToLand || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Landing || spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded;
			}
			Clustercraft component = base.smi.master.GetComponent<RocketModule>().CraftInterface.GetComponent<Clustercraft>();
			return component.Status == Clustercraft.CraftStatus.Grounded;
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
			foreach (Ref<RocketModule> part in base.master.parts)
			{
				if (part != null)
				{
					base.master.takeOffLocation = Grid.PosToCell(base.master.gameObject);
					part.Get().Trigger(-1277991738, base.master.gameObject);
				}
			}
			CraftModuleInterface craftInterface = base.master.GetComponent<RocketModule>().CraftInterface;
			if (craftInterface != null)
			{
				craftInterface.Trigger(-1277991738, base.master.gameObject);
			}
			if (base.master.registerType == RegisterType.Spacecraft)
			{
				SetMissionState(Spacecraft.MissionState.Launching);
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
				DistanceAboveGround = Mathf.Pow(num / partsLaunchSpeedRatio, 4f);
				base.master.rocketSpeed = DistanceAboveGround - Mathf.Pow((base.smi.timeinstate - 1f) / partsLaunchSpeedRatio, 4f);
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
			DistanceAboveGround = 50f;
			foreach (Ref<RocketModule> part in base.master.parts)
			{
				if (part != null && !(part.Get() == null))
				{
					RocketModule rocketModule = part.Get();
					rocketModule.GetComponent<KBatchedAnimController>().Offset = Vector3.up * DistanceAboveGround;
					rocketModule.GetComponent<KBatchedAnimController>().enabled = false;
					if (base.master.registerType == RegisterType.Clustercraft)
					{
						rocketModule.GetComponent<RocketModule>().MoveToSpace();
					}
				}
			}
			if (base.master.registerType == RegisterType.Clustercraft)
			{
				base.smi.master.GetComponent<RocketModule>().CraftInterface.GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.InFlight);
			}
			if (base.master.registerType == RegisterType.Spacecraft)
			{
				SetMissionState(Spacecraft.MissionState.Underway);
			}
		}

		public void SetupLanding()
		{
			float f = (DistanceAboveGround = base.master.InitialFlightAnimOffsetForLanding());
			landingDistancePartsTimeConstant = Mathf.Pow(f, 0.25f) * partsLaunchSpeedRatio;
			base.master.isLanding = true;
			base.master.rocketSpeed = 0f;
			if (base.master.registerType == RegisterType.Spacecraft)
			{
				base.smi.SetMissionState(Spacecraft.MissionState.Landing);
			}
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
			foreach (Ref<RocketModule> part in base.smi.master.parts)
			{
				if (part != null && !(part.Get() == null))
				{
					part.Get().GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
				}
			}
			if (base.smi.master.registerType == RegisterType.Spacecraft)
			{
				base.smi.SetMissionState(Spacecraft.MissionState.Grounded);
			}
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
			int num = 0;
			foreach (Ref<RocketModule> part in base.master.parts)
			{
				if (part != null)
				{
					RocketModule rocketModule = part.Get();
					KBatchedAnimController component = rocketModule.GetComponent<KBatchedAnimController>();
					component.Offset = Vector3.up * DistanceAboveGround;
					Vector3 positionIncludingOffset = component.PositionIncludingOffset;
					bool flag = Grid.IsValidCell(Grid.PosToCell(positionIncludingOffset));
					if (component.enabled != flag)
					{
						component.enabled = flag;
					}
					if (flag)
					{
						num++;
						States.DoWorldDamage(rocketModule.gameObject, positionIncludingOffset, myWorldId);
					}
				}
			}
			return num;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LaunchableRocket>
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

		public State grounded;

		public NotGroundedStates not_grounded;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			base.serializable = SerializeType.Both_DEPRECATED;
			grounded.EventTransition(GameHashes.DoLaunchRocket, not_grounded.launch_setup).EnterTransition(not_grounded.launch_loop, (StatesInstance smi) => !smi.IsGrounded()).Enter(delegate(StatesInstance smi)
			{
				smi.SetTagOnModules(GameTags.RocketOnGround, set: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.SetTagOnModules(GameTags.RocketOnGround, set: false);
				})
				.Enter(delegate(StatesInstance smi)
				{
					smi.FinalizeLanding();
				});
			not_grounded.Enter(delegate(StatesInstance smi)
			{
				smi.SetTagOnModules(GameTags.RocketNotOnGround, set: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.SetTagOnModules(GameTags.RocketNotOnGround, set: false);
			});
			not_grounded.launch_setup.Enter(delegate(StatesInstance smi)
			{
				smi.SetupLaunch();
				smi.GoTo(not_grounded.launch_loop);
			});
			not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Update(delegate(StatesInstance smi, float dt)
			{
				smi.LaunchLoop(dt);
			}, UpdateRate.SIM_EVERY_TICK).ParamTransition(distanceAboveGround, not_grounded.space, (StatesInstance smi, float p) => p >= 50f);
			not_grounded.space.EnterTransition(not_grounded.landing_setup, (StatesInstance smi) => smi.IsGrounded()).EventTransition(GameHashes.DoReturnRocket, not_grounded.landing_setup).Enter(delegate(StatesInstance smi)
			{
				smi.SetTagOnModules(GameTags.RocketInSpace, set: true);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.SetTagOnModules(GameTags.RocketInSpace, set: false);
				})
				.Enter(delegate(StatesInstance smi)
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
				foreach (Ref<RocketModule> part in smi.master.parts)
				{
					if (part != null && !(part.Get() == null))
					{
						part.Get().Trigger(-887025858, smi.gameObject);
					}
				}
				CraftModuleInterface craftInterface = smi.master.GetComponent<RocketModule>().CraftInterface;
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

	private class UpdateRocketLandingParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public PARAMETER_ID parameterId;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateRocketLandingParameter()
			: base("rocketLanding")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.rocketModule = sound.transform.GetComponent<RocketModule>();
			entry.ev = sound.ev;
			entry.parameterId = sound.description.GetParameterId(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				if (entry.rocketModule == null)
				{
					continue;
				}
				LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
				if (conditionManager == null)
				{
					continue;
				}
				LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
				if (!(component == null))
				{
					if (component.isLanding)
					{
						EventInstance ev = entry.ev;
						ev.setParameterByID(entry.parameterId, 1f);
					}
					else
					{
						EventInstance ev = entry.ev;
						ev.setParameterByID(entry.parameterId, 0f);
					}
				}
			}
		}

		public override void Remove(Sound sound)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ev.handle == sound.ev.handle)
				{
					entries.RemoveAt(i);
					break;
				}
			}
		}
	}

	private class UpdateRocketSpeedParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public PARAMETER_ID parameterId;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateRocketSpeedParameter()
			: base("rocketSpeed")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.rocketModule = sound.transform.GetComponent<RocketModule>();
			entry.ev = sound.ev;
			entry.parameterId = sound.description.GetParameterId(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				if (entry.rocketModule == null)
				{
					continue;
				}
				LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
				if (!(conditionManager == null))
				{
					LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
					if (!(component == null))
					{
						EventInstance ev = entry.ev;
						ev.setParameterByID(entry.parameterId, component.rocketSpeed);
					}
				}
			}
		}

		public override void Remove(Sound sound)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ev.handle == sound.ev.handle)
				{
					entries.RemoveAt(i);
					break;
				}
			}
		}
	}

	[Serialize]
	private int takeOffLocation;

	private bool isLanding;

	private float rocketSpeed;

	private GameObject soundSpeakerObject;

	public RegisterType registerType;

	public IList<Ref<RocketModule>> parts => GetComponent<RocketModule>().CraftInterface.Modules;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (registerType == RegisterType.Spacecraft)
		{
			int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(this);
			if (spacecraftID == -1)
			{
				Spacecraft spacecraft = new Spacecraft(GetComponent<LaunchConditionManager>());
				spacecraft.GenerateName();
				SpacecraftManager.instance.RegisterSpacecraft(spacecraft);
			}
		}
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Ref<RocketModule> part in parts)
		{
			if ((bool)part.Get().GetComponent<RocketEngine>())
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
		foreach (Ref<RocketModule> part in parts)
		{
			num3 += (float)part.Get().GetComponent<Building>().Def.HeightInCells;
		}
		return num2 + num3 + 100f;
	}

	protected override void OnCleanUp()
	{
		if (registerType == RegisterType.Spacecraft)
		{
			SpacecraftManager.instance.UnregisterSpacecraft(GetComponent<LaunchConditionManager>());
		}
		base.OnCleanUp();
	}
}
