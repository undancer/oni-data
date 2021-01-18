using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class VerticalWindTunnel : StateMachineComponent<VerticalWindTunnel.StatesInstance>, IGameObjectEffectDescriptor, ISim200ms
{
	public class States : GameStateMachine<States, StatesInstance, VerticalWindTunnel>
	{
		public class OperationalStates : State
		{
			public State stopped;

			public State pre;

			public State playing;

			public State post;
		}

		public IntParameter playerCount;

		public State unoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: false);
			}).TagTransition(GameTags.Operational, operational).PlayAnim("off");
			operational.TagTransition(GameTags.Operational, unoperational, on_remove: true).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores();
			}).Exit("CancelChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores(update: false);
			})
				.DefaultState(operational.stopped);
			operational.stopped.PlayAnim("off").ParamTransition(playerCount, operational.pre, (StatesInstance smi, int p) => p > 0);
			operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(operational.playing);
			operational.playing.PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.SetActive(active: false);
			})
				.ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0)
				.Enter("GasWalls", delegate(StatesInstance smi)
				{
					smi.master.SetGasWalls(set: true);
				})
				.Exit("GasWalls", delegate(StatesInstance smi)
				{
					smi.master.SetGasWalls(set: false);
				});
			operational.post.PlayAnim("working_pst").QueueAnim("off_pre").OnAnimQueueComplete(operational.stopped);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, VerticalWindTunnel, object>.GameInstance
	{
		private Operational operational;

		public StatesInstance(VerticalWindTunnel smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active);
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public int basePriority;

	public float displacementAmount_DescriptorOnly;

	public static Operational.Flag validIntakeFlag = new Operational.Flag("valid_intake", Operational.Flag.Type.Requirement);

	private bool invalidIntake = false;

	private float avgGasAccumTop = 0f;

	private float avgGasAccumBottom = 0f;

	private int avgGasCounter = 0;

	public CellOffset[] choreOffsets = new CellOffset[3]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private VerticalWindTunnelWorkable[] workables;

	private Chore[] chores;

	private ElementConsumer bottomConsumer;

	private ElementConsumer topConsumer;

	private Operational operational;

	public HashSet<int> players = new HashSet<int>();

	public HashedString[] overrideAnims = new HashedString[3]
	{
		"anim_interacts_windtunnel_center_kanim",
		"anim_interacts_windtunnel_left_kanim",
		"anim_interacts_windtunnel_right_kanim"
	};

	public string[][] workPreAnims = new string[3][]
	{
		new string[2]
		{
			"weak_working_front_pre",
			"weak_working_back_pre"
		},
		new string[2]
		{
			"medium_working_front_pre",
			"medium_working_back_pre"
		},
		new string[2]
		{
			"strong_working_front_pre",
			"strong_working_back_pre"
		}
	};

	public string[] workAnims = new string[3]
	{
		"weak_working_loop",
		"medium_working_loop",
		"strong_working_loop"
	};

	public string[][] workPstAnims = new string[3][]
	{
		new string[2]
		{
			"weak_working_back_pst",
			"weak_working_front_pst"
		},
		new string[2]
		{
			"medium_working_back_pst",
			"medium_working_front_pst"
		},
		new string[2]
		{
			"strong_working_back_pst",
			"strong_working_front_pst"
		}
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ElementConsumer[] components = GetComponents<ElementConsumer>();
		bottomConsumer = components[0];
		bottomConsumer.EnableConsumption(enabled: false);
		bottomConsumer.OnElementConsumed += delegate(Sim.ConsumedMassInfo info)
		{
			OnElementConsumed(isTop: false, info);
		};
		topConsumer = components[1];
		topConsumer.EnableConsumption(enabled: false);
		topConsumer.OnElementConsumed += delegate(Sim.ConsumedMassInfo info)
		{
			OnElementConsumed(isTop: true, info);
		};
		operational = GetComponent<Operational>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		invalidIntake = HasInvalidIntake();
		GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.WindTunnelIntake, invalidIntake, this);
		operational.SetFlag(validIntakeFlag, !invalidIntake);
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
		workables = new VerticalWindTunnelWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("VerticalWindTunnelWorkable", pos);
			KSelectable kSelectable = go.AddOrGet<KSelectable>();
			kSelectable.SetName(this.GetProperName());
			kSelectable.IsSelectable = false;
			VerticalWindTunnelWorkable verticalWindTunnelWorkable = go.AddOrGet<VerticalWindTunnelWorkable>();
			int player_index = i;
			verticalWindTunnelWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(verticalWindTunnelWorkable.OnWorkableEventCB, (Action<Workable.WorkableEvent>)delegate(Workable.WorkableEvent ev)
			{
				OnWorkableEvent(player_index, ev);
			});
			verticalWindTunnelWorkable.overrideAnim = overrideAnims[i];
			verticalWindTunnelWorkable.preAnims = workPreAnims[i];
			verticalWindTunnelWorkable.loopAnim = workAnims[i];
			verticalWindTunnelWorkable.pstAnims = workPstAnims[i];
			workables[i] = verticalWindTunnelWorkable;
			workables[i].windTunnel = this;
		}
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		UpdateChores(update: false);
		for (int i = 0; i < workables.Length; i++)
		{
			if ((bool)workables[i])
			{
				Util.KDestroyGameObject(workables[i]);
				workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = workables[i];
		Chore chore = new WorkChore<VerticalWindTunnelWorkable>(Db.Get().ChoreTypes.Relax, workable, null, run_until_complete: true, null, null, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, on_end: OnSocialChoreEnd, allow_in_red_alert: false, ignore_schedule_block: false, only_when_operational: true, override_anims: null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.gameObject.HasTag(GameTags.Operational))
		{
			UpdateChores();
		}
	}

	public void UpdateChores(bool update = true)
	{
		for (int i = 0; i < choreOffsets.Length; i++)
		{
			Chore chore = chores[i];
			if (update)
			{
				if (chore?.isComplete ?? true)
				{
					chores[i] = CreateChore(i);
				}
			}
			else if (chore != null)
			{
				chore.Cancel("locator invalidated");
				chores[i] = null;
			}
		}
	}

	public void Sim200ms(float dt)
	{
		bool flag = HasInvalidIntake();
		if (flag != invalidIntake)
		{
			invalidIntake = flag;
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.WindTunnelIntake, invalidIntake, this);
			operational.SetFlag(validIntakeFlag, !invalidIntake);
		}
	}

	private float GetIntakeRatio(int fromCell, int radius)
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = -radius; i < radius; i++)
		{
			for (int j = -radius; j < radius; j++)
			{
				int cell = Grid.OffsetCell(fromCell, j, i);
				if (!Grid.IsSolidCell(cell))
				{
					if (Grid.IsGas(cell))
					{
						num2 += 1f;
					}
					num += 1f;
				}
			}
		}
		return num2 / num;
	}

	private bool HasInvalidIntake()
	{
		Vector3 position = base.transform.GetPosition();
		int cell = Grid.XYToCell((int)position.x, (int)position.y);
		int fromCell = Grid.OffsetCell(cell, (int)topConsumer.sampleCellOffset.x, (int)topConsumer.sampleCellOffset.y);
		int fromCell2 = Grid.OffsetCell(cell, (int)bottomConsumer.sampleCellOffset.x, (int)bottomConsumer.sampleCellOffset.y);
		avgGasAccumTop += GetIntakeRatio(fromCell, topConsumer.consumptionRadius);
		avgGasAccumBottom += GetIntakeRatio(fromCell2, bottomConsumer.consumptionRadius);
		int num = 5;
		avgGasCounter = (avgGasCounter + 1) % num;
		if (avgGasCounter == 0)
		{
			float num2 = avgGasAccumTop / (float)num;
			float num3 = avgGasAccumBottom / (float)num;
			avgGasAccumBottom = 0f;
			avgGasAccumTop = 0f;
			return (double)num2 < 0.5 || (double)num3 < 0.5;
		}
		return invalidIntake;
	}

	public void SetGasWalls(bool set)
	{
		Building component = GetComponent<Building>();
		Sim.Cell.Properties properties = (Sim.Cell.Properties)3;
		Vector3 position = base.transform.GetPosition();
		for (int i = 0; i < component.Def.HeightInCells; i++)
		{
			int gameCell = Grid.XYToCell(Mathf.FloorToInt(position.x) - 2, Mathf.FloorToInt(position.y) + i);
			int gameCell2 = Grid.XYToCell(Mathf.FloorToInt(position.x) + 2, Mathf.FloorToInt(position.y) + i);
			if (set)
			{
				SimMessages.SetCellProperties(gameCell, (byte)properties);
				SimMessages.SetCellProperties(gameCell2, (byte)properties);
			}
			else
			{
				SimMessages.ClearCellProperties(gameCell, (byte)properties);
				SimMessages.ClearCellProperties(gameCell2, (byte)properties);
			}
		}
	}

	private void OnElementConsumed(bool isTop, Sim.ConsumedMassInfo info)
	{
		Building component = GetComponent<Building>();
		Vector3 position = base.transform.GetPosition();
		CellOffset offset = (isTop ? new CellOffset(0, component.Def.HeightInCells + 1) : new CellOffset(0, 0));
		int gameCell = Grid.OffsetCell(Grid.XYToCell((int)position.x, (int)position.y), offset);
		SimMessages.AddRemoveSubstance(gameCell, info.removedElemIdx, CellEventLogger.Instance.ElementEmitted, info.mass, info.temperature, info.diseaseIdx, info.diseaseCount);
	}

	public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
	{
		if (ev == Workable.WorkableEvent.WorkStarted)
		{
			players.Add(player);
		}
		else
		{
			players.Remove(player);
		}
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT.Replace("{amount}", GameUtil.GetFormattedMass(displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond)), BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT_TOOLTIP.Replace("{amount}", GameUtil.GetFormattedMass(displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond)), Descriptor.DescriptorType.Requirement));
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		return list;
	}
}
