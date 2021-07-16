using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ArcadeMachine : StateMachineComponent<ArcadeMachine.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, ArcadeMachine>
	{
		public class OperationalStates : State
		{
			public State stopped;

			public State pre;

			public State playing;

			public State playing_coop;

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
			operational.stopped.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: false);
			}).PlayAnim("on").ParamTransition(playerCount, operational.pre, (StatesInstance smi, int p) => p > 0);
			operational.pre.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: true);
			}).PlayAnim("working_pre").OnAnimQueueComplete(operational.playing);
			operational.playing.PlayAnim(GetPlayingAnim, KAnim.PlayMode.Loop).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0).ParamTransition(playerCount, operational.playing_coop, (StatesInstance smi, int p) => p > 1);
			operational.playing_coop.PlayAnim(GetPlayingAnim, KAnim.PlayMode.Loop).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0).ParamTransition(playerCount, operational.playing, (StatesInstance smi, int p) => p == 1);
			operational.post.PlayAnim("working_pst").OnAnimQueueComplete(operational.stopped);
		}

		private string GetPlayingAnim(StatesInstance smi)
		{
			bool flag = smi.master.players.Contains(0);
			bool flag2 = smi.master.players.Contains(1);
			if (flag && !flag2)
			{
				return "working_loop_one_p";
			}
			if (flag2 && !flag)
			{
				return "working_loop_two_p";
			}
			return "working_loop_coop_p";
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, ArcadeMachine, object>.GameInstance
	{
		private Operational operational;

		public StatesInstance(ArcadeMachine smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active);
		}
	}

	public const string SPECIFIC_EFFECT = "PlayedArcade";

	public const string TRACKING_EFFECT = "RecentlyPlayedArcade";

	public CellOffset[] choreOffsets = new CellOffset[2]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0)
	};

	private ArcadeMachineWorkable[] workables;

	private Chore[] chores;

	public HashSet<int> players = new HashSet<int>();

	public KAnimFile[][] overrideAnims = new KAnimFile[2][]
	{
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_arcade_cabinet_playerone_kanim")
		},
		new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_arcade_cabinet_playertwo_kanim")
		}
	};

	public HashedString[][] workAnims = new HashedString[2][]
	{
		new HashedString[2]
		{
			"working_pre",
			"working_loop_one_p"
		},
		new HashedString[2]
		{
			"working_pre",
			"working_loop_two_p"
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
		workables = new ArcadeMachineWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			Vector3 pos = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]), Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("ArcadeMachineWorkable", pos);
			ArcadeMachineWorkable arcadeMachineWorkable = go.AddOrGet<ArcadeMachineWorkable>();
			KSelectable kSelectable = go.AddOrGet<KSelectable>();
			kSelectable.SetName(this.GetProperName());
			kSelectable.IsSelectable = false;
			int player_index = i;
			arcadeMachineWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(arcadeMachineWorkable.OnWorkableEventCB, (Action<Workable.WorkableEvent>)delegate(Workable.WorkableEvent ev)
			{
				OnWorkableEvent(player_index, ev);
			});
			arcadeMachineWorkable.overrideAnims = overrideAnims[i];
			arcadeMachineWorkable.workAnims = workAnims[i];
			workables[i] = arcadeMachineWorkable;
			workables[i].owner = this;
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
		WorkChore<ArcadeMachineWorkable> obj = new WorkChore<ArcadeMachineWorkable>(Db.Get().ChoreTypes.Relax, workable, null, run_until_complete: true, null, null, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, on_end: OnSocialChoreEnd, allow_in_red_alert: false, ignore_schedule_block: false, only_when_operational: true, override_anims: null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
		obj.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return obj;
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
				if (chore == null || chore.isComplete)
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
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "PlayedArcade", increase_indent: true);
		return list;
	}
}
