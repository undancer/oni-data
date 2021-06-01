using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocket : StateMachineComponent<LaunchableRocket.StatesInstance>, ILaunchableRocket
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, LaunchableRocket, object>.GameInstance
	{
		public StatesInstance(LaunchableRocket master)
			: base(master)
		{
		}

		public bool IsMissionState(Spacecraft.MissionState state)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			return spacecraftFromLaunchConditionManager.state == state;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			spacecraftFromLaunchConditionManager.SetState(state);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, LaunchableRocket>
	{
		public class NotGroundedStates : State
		{
			public State launch_pre;

			public State space;

			public State launch_loop;

			public State returning;

			public State landing_loop;
		}

		public State grounded;

		public NotGroundedStates not_grounded;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			base.serializable = SerializeType.Both_DEPRECATED;
			grounded.EventTransition(GameHashes.DoLaunchRocket, not_grounded.launch_pre).Enter(delegate(StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject part in smi.master.parts)
				{
					if (!(part == null))
					{
						part.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Grounded);
			});
			not_grounded.ToggleTag(GameTags.RocketNotOnGround);
			not_grounded.launch_pre.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = false;
				smi.master.rocketSpeed = 0f;
				smi.master.parts = AttachableBuilding.GetAttachedNetwork(smi.master.GetComponent<AttachableBuilding>());
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				foreach (GameObject engine in smi.master.GetEngines())
				{
					engine.Trigger(-1358394196);
				}
				Game.Instance.Trigger(705820818, this);
				foreach (GameObject part2 in smi.master.parts)
				{
					if (!(part2 == null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						part2.Trigger(705820818);
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Launching);
			}).ScheduleGoTo(5f, not_grounded.launch_loop);
			not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, not_grounded.returning).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag2 = true;
				float num7 = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num7;
				smi.master.flightAnimOffset += dt * num7;
				foreach (GameObject part3 in smi.master.parts)
				{
					if (!(part3 == null))
					{
						KBatchedAnimController component5 = part3.GetComponent<KBatchedAnimController>();
						component5.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset3 = component5.PositionIncludingOffset;
						if (smi.master.soundSpeakerObject == null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						if (Grid.PosToXY(positionIncludingOffset3).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y)
						{
							part3.GetComponent<KBatchedAnimController>().enabled = false;
						}
						else
						{
							flag2 = false;
							DoWorldDamage(part3, positionIncludingOffset3);
						}
					}
				}
				if (flag2)
				{
					smi.GoTo(not_grounded.space);
				}
			}, UpdateRate.SIM_33ms);
			not_grounded.space.Enter(delegate(StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject part4 in smi.master.parts)
				{
					if (!(part4 == null))
					{
						part4.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
						part4.GetComponent<KBatchedAnimController>().enabled = false;
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Underway);
			}).EventTransition(GameHashes.DoReturnRocket, not_grounded.returning, (StatesInstance smi) => smi.IsMissionState(Spacecraft.MissionState.WaitingToLand));
			not_grounded.returning.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = true;
				smi.master.rocketSpeed = 0f;
				smi.SetMissionState(Spacecraft.MissionState.Landing);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.isLanding = true;
				KBatchedAnimController component3 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component3.Offset = Vector3.up * smi.master.flightAnimOffset;
				float num4 = Mathf.Abs(smi.master.gameObject.transform.position.y + component3.Offset.y - (Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f)).y);
				float num5 = 0f;
				float num6 = 0.5f;
				num5 = Mathf.Clamp(num6 * num4, 0f, 10f) * dt;
				smi.master.rocketSpeed = num5;
				smi.master.flightAnimOffset -= num5;
				bool flag = true;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				foreach (GameObject part5 in smi.master.parts)
				{
					if (!(part5 == null))
					{
						KBatchedAnimController component4 = part5.GetComponent<KBatchedAnimController>();
						component4.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset2 = component4.PositionIncludingOffset;
						if (Grid.IsValidCell(Grid.PosToCell(part5)))
						{
							part5.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag = false;
						}
						DoWorldDamage(part5, positionIncludingOffset2);
					}
				}
				if (flag)
				{
					smi.GoTo(not_grounded.landing_loop);
				}
			}, UpdateRate.SIM_33ms);
			not_grounded.landing_loop.Enter(delegate(StatesInstance smi)
			{
				smi.master.isLanding = true;
				int num3 = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject = smi.master.parts[i];
					if (!(gameObject == null) && gameObject != smi.master.gameObject && gameObject.GetComponent<RocketEngine>() != null)
					{
						num3 = i;
					}
				}
				if (num3 != -1)
				{
					smi.master.parts[num3].Trigger(-1358394196);
				}
			}).Update(delegate(StatesInstance smi, float dt)
			{
				KBatchedAnimController component = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component.Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num = 0f;
				float num2 = 0.5f;
				num = Mathf.Clamp(num2 * flightAnimOffset, 0f, 10f);
				smi.master.rocketSpeed = num;
				smi.master.flightAnimOffset -= num * dt;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				if (num <= 0.0025f && dt != 0f)
				{
					smi.master.GetComponent<KSelectable>().IsSelectable = true;
					num = 0f;
					foreach (GameObject part6 in smi.master.parts)
					{
						if (!(part6 == null))
						{
							part6.Trigger(-887025858);
						}
					}
					smi.GoTo(grounded);
				}
				else
				{
					foreach (GameObject part7 in smi.master.parts)
					{
						if (!(part7 == null))
						{
							KBatchedAnimController component2 = part7.GetComponent<KBatchedAnimController>();
							component2.Offset = Vector3.up * smi.master.flightAnimOffset;
							Vector3 positionIncludingOffset = component2.PositionIncludingOffset;
							DoWorldDamage(part7, positionIncludingOffset);
						}
					}
				}
			}, UpdateRate.SIM_33ms);
		}

		private static void DoWorldDamage(GameObject part, Vector3 apparentPosition)
		{
			OccupyArea component = part.GetComponent<OccupyArea>();
			component.UpdateOccupiedArea();
			CellOffset[] occupiedCellsOffsets = component.OccupiedCellsOffsets;
			foreach (CellOffset offset in occupiedCellsOffsets)
			{
				int num = Grid.OffsetCell(Grid.PosToCell(apparentPosition), offset);
				if (!Grid.IsValidCell(num))
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

	public List<GameObject> parts = new List<GameObject>();

	[Serialize]
	private int takeOffLocation;

	[Serialize]
	private float flightAnimOffset = 0f;

	private GameObject soundSpeakerObject;

	public LaunchableRocketRegisterType registerType => LaunchableRocketRegisterType.Spacecraft;

	public GameObject LaunchableGameObject => base.gameObject;

	public float rocketSpeed
	{
		get;
		private set;
	}

	public bool isLanding
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.parts = AttachableBuilding.GetAttachedNetwork(base.smi.master.GetComponent<AttachableBuilding>());
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(this);
		if (spacecraftID == -1)
		{
			Spacecraft spacecraft = new Spacecraft(GetComponent<LaunchConditionManager>());
			spacecraft.GenerateName();
			SpacecraftManager.instance.RegisterSpacecraft(spacecraft);
		}
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject part in parts)
		{
			if ((bool)part.GetComponent<RocketEngine>())
			{
				list.Add(part);
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		SpacecraftManager.instance.UnregisterSpacecraft(GetComponent<LaunchConditionManager>());
		base.OnCleanUp();
	}
}