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
			return SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).state == state;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).SetState(state);
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
			grounded.ToggleTag(GameTags.RocketOnGround).Enter(delegate(StatesInstance smi)
			{
				foreach (GameObject part in smi.master.parts)
				{
					if (!(part == null))
					{
						part.AddTag(GameTags.RocketOnGround);
					}
				}
			}).Exit(delegate(StatesInstance smi)
			{
				foreach (GameObject part2 in smi.master.parts)
				{
					if (!(part2 == null))
					{
						part2.RemoveTag(GameTags.RocketOnGround);
					}
				}
			})
				.EventTransition(GameHashes.DoLaunchRocket, not_grounded.launch_pre)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.rocketSpeed = 0f;
					foreach (GameObject part3 in smi.master.parts)
					{
						if (!(part3 == null))
						{
							part3.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
						}
					}
					smi.SetMissionState(Spacecraft.MissionState.Grounded);
				});
			not_grounded.ToggleTag(GameTags.RocketNotOnGround).Enter(delegate(StatesInstance smi)
			{
				foreach (GameObject part4 in smi.master.parts)
				{
					if (!(part4 == null))
					{
						part4.AddTag(GameTags.RocketNotOnGround);
					}
				}
			}).Exit(delegate(StatesInstance smi)
			{
				foreach (GameObject part5 in smi.master.parts)
				{
					if (!(part5 == null))
					{
						part5.RemoveTag(GameTags.RocketNotOnGround);
					}
				}
			});
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
				Game.Instance.Trigger(-1277991738, this);
				foreach (GameObject part6 in smi.master.parts)
				{
					if (!(part6 == null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						part6.Trigger(-1277991738);
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Launching);
			}).ScheduleGoTo(5f, not_grounded.launch_loop);
			not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, not_grounded.returning).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag2 = true;
				float num5 = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num5;
				smi.master.flightAnimOffset += dt * num5;
				foreach (GameObject part7 in smi.master.parts)
				{
					if (!(part7 == null))
					{
						KBatchedAnimController component4 = part7.GetComponent<KBatchedAnimController>();
						component4.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset3 = component4.PositionIncludingOffset;
						if (smi.master.soundSpeakerObject == null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						if (Grid.PosToXY(positionIncludingOffset3).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y)
						{
							part7.GetComponent<KBatchedAnimController>().enabled = false;
						}
						else
						{
							flag2 = false;
							DoWorldDamage(part7, positionIncludingOffset3);
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
				foreach (GameObject part8 in smi.master.parts)
				{
					if (!(part8 == null))
					{
						part8.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
						part8.GetComponent<KBatchedAnimController>().enabled = false;
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
				KBatchedAnimController component2 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component2.Offset = Vector3.up * smi.master.flightAnimOffset;
				float num3 = Mathf.Abs(smi.master.gameObject.transform.position.y + component2.Offset.y - (Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f)).y);
				float num4 = 0f;
				num4 = Mathf.Clamp(0.5f * num3, 0f, 10f) * dt;
				smi.master.rocketSpeed = num4;
				smi.master.flightAnimOffset -= num4;
				bool flag = true;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				foreach (GameObject part9 in smi.master.parts)
				{
					if (!(part9 == null))
					{
						KBatchedAnimController component3 = part9.GetComponent<KBatchedAnimController>();
						component3.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset2 = component3.PositionIncludingOffset;
						if (Grid.IsValidCell(Grid.PosToCell(part9)))
						{
							part9.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag = false;
						}
						DoWorldDamage(part9, positionIncludingOffset2);
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
				int num2 = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject = smi.master.parts[i];
					if (!(gameObject == null) && gameObject != smi.master.gameObject && gameObject.GetComponent<RocketEngine>() != null)
					{
						num2 = i;
					}
				}
				if (num2 != -1)
				{
					smi.master.parts[num2].Trigger(-1358394196);
				}
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.master.gameObject.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num = 0f;
				num = Mathf.Clamp(0.5f * flightAnimOffset, 0f, 10f);
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
					foreach (GameObject part10 in smi.master.parts)
					{
						if (!(part10 == null))
						{
							part10.Trigger(-887025858);
						}
					}
					smi.GoTo(grounded);
				}
				else
				{
					foreach (GameObject part11 in smi.master.parts)
					{
						if (!(part11 == null))
						{
							KBatchedAnimController component = part11.GetComponent<KBatchedAnimController>();
							component.Offset = Vector3.up * smi.master.flightAnimOffset;
							Vector3 positionIncludingOffset = component.PositionIncludingOffset;
							DoWorldDamage(part11, positionIncludingOffset);
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
	private float flightAnimOffset;

	private GameObject soundSpeakerObject;

	public LaunchableRocketRegisterType registerType => LaunchableRocketRegisterType.Spacecraft;

	public GameObject LaunchableGameObject => base.gameObject;

	public float rocketSpeed { get; private set; }

	public bool isLanding { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.parts = AttachableBuilding.GetAttachedNetwork(base.smi.master.GetComponent<AttachableBuilding>());
		if (SpacecraftManager.instance.GetSpacecraftID(this) == -1)
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
