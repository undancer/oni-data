using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Phonobox : StateMachineComponent<Phonobox.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, Phonobox>
	{
		public class OperationalStates : State
		{
			public State stopped;

			public State pre;

			public State bridge;

			public State playing;

			public State song_end;

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
			}).ParamTransition(playerCount, operational.pre, (StatesInstance smi, int p) => p > 0).PlayAnim("on");
			operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(operational.playing);
			operational.playing.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: true);
			}).ScheduleGoTo(25f, operational.song_end).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0)
				.PlayAnim(GetPlayAnim, KAnim.PlayMode.Loop);
			operational.song_end.ParamTransition(playerCount, operational.bridge, (StatesInstance smi, int p) => p > 0).ParamTransition(playerCount, operational.post, (StatesInstance smi, int p) => p == 0);
			operational.bridge.PlayAnim("working_trans").OnAnimQueueComplete(operational.playing);
			operational.post.PlayAnim("working_pst").OnAnimQueueComplete(operational.stopped);
		}

		public static string GetPlayAnim(StatesInstance smi)
		{
			int num = Random.Range(0, building_anims.Length);
			return building_anims[num];
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Phonobox, object>.GameInstance
	{
		private FetchChore chore;

		private Operational operational;

		public StatesInstance(Phonobox smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active);
		}
	}

	public const string SPECIFIC_EFFECT = "Danced";

	public const string TRACKING_EFFECT = "RecentlyDanced";

	public CellOffset[] choreOffsets = new CellOffset[5]
	{
		new CellOffset(0, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 0),
		new CellOffset(-2, 0),
		new CellOffset(2, 0)
	};

	private PhonoboxWorkable[] workables;

	private Chore[] chores;

	private HashSet<Worker> players = new HashSet<Worker>();

	private static string[] building_anims = new string[3]
	{
		"working_loop",
		"working_loop2",
		"working_loop3"
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
		workables = new PhonoboxWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("PhonoboxWorkable", pos);
			KSelectable kSelectable = go.AddOrGet<KSelectable>();
			kSelectable.SetName(this.GetProperName());
			kSelectable.IsSelectable = false;
			PhonoboxWorkable phonoboxWorkable = go.AddOrGet<PhonoboxWorkable>();
			phonoboxWorkable.owner = this;
			workables[i] = phonoboxWorkable;
		}
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
		Chore chore = new WorkChore<PhonoboxWorkable>(Db.Get().ChoreTypes.Relax, workable, null, run_until_complete: true, null, null, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, on_end: OnSocialChoreEnd, allow_in_red_alert: false, ignore_schedule_block: false, only_when_operational: true, override_anims: null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
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

	public void AddWorker(Worker player)
	{
		players.Add(player);
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	public void RemoveWorker(Worker player)
	{
		players.Remove(player);
		base.smi.sm.playerCount.Set(players.Count, base.smi);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "Danced", increase_indent: true);
		return list;
	}
}
