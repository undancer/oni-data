using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SocialGatheringPoint : StateMachineComponent<SocialGatheringPoint.StatesInstance>
{
	public class States : GameStateMachine<States, StatesInstance, SocialGatheringPoint>
	{
		public State off;

		public State on;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = off;
			root.DoNothing();
			off.TagTransition(GameTags.Operational, on);
			on.TagTransition(GameTags.Operational, off, on_remove: true).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.master.tracker.Update();
			}).Exit("CancelChore", delegate(StatesInstance smi)
			{
				smi.master.tracker.Update(update: false);
			});
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SocialGatheringPoint, object>.GameInstance
	{
		public StatesInstance(SocialGatheringPoint smi)
			: base(smi)
		{
		}
	}

	public CellOffset[] choreOffsets = new CellOffset[2]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public int basePriority;

	public string socialEffect;

	public float workTime = 15f;

	public System.Action OnSocializeBeginCB;

	public System.Action OnSocializeEndCB;

	private SocialChoreTracker tracker;

	private SocialGatheringPointWorkable[] workables;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		workables = new SocialGatheringPointWorkable[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("SocialGatheringPointWorkable", pos);
			SocialGatheringPointWorkable socialGatheringPointWorkable = go.AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.basePriority = basePriority;
			socialGatheringPointWorkable.specificEffect = socialEffect;
			socialGatheringPointWorkable.OnWorkableEventCB = OnWorkableEvent;
			socialGatheringPointWorkable.SetWorkTime(workTime);
			workables[i] = socialGatheringPointWorkable;
		}
		tracker = new SocialChoreTracker(base.gameObject, choreOffsets);
		tracker.choreCount = choreCount;
		tracker.CreateChoreCB = CreateChore;
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		if (tracker != null)
		{
			tracker.Clear();
			tracker = null;
		}
		if (workables != null)
		{
			for (int i = 0; i < workables.Length; i++)
			{
				if ((bool)workables[i])
				{
					Util.KDestroyGameObject(workables[i]);
					workables[i] = null;
				}
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = workables[i];
		Chore chore = new WorkChore<SocialGatheringPointWorkable>(Db.Get().ChoreTypes.Relax, workable, null, run_until_complete: true, null, null, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, on_end: OnSocialChoreEnd, allow_in_red_alert: false, ignore_schedule_block: false, only_when_operational: true, override_anims: null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, priority_class_value: 5, ignore_building_assignment: false, add_to_daily_report: false);
		chore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		chore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.smi.IsInsideState(base.smi.sm.on))
		{
			tracker.Update();
		}
	}

	private void OnWorkableEvent(Workable.WorkableEvent workable_event)
	{
		switch (workable_event)
		{
		case Workable.WorkableEvent.WorkStarted:
			if (OnSocializeBeginCB != null)
			{
				OnSocializeBeginCB();
			}
			break;
		case Workable.WorkableEvent.WorkStopped:
			if (OnSocializeEndCB != null)
			{
				OnSocializeEndCB();
			}
			break;
		}
	}
}
