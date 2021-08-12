using System;
using STRINGS;
using UnityEngine;

public class FoodFightChore : Chore<FoodFightChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, FoodFightChore, object>.GameInstance
	{
		private int locatorCell;

		public StatesInstance(FoodFightChore master, GameObject locator)
			: base(master)
		{
			base.sm.locator.Set(locator, base.smi);
		}

		public void UpdateAttackTarget()
		{
			int num = 0;
			MinionIdentity minionIdentity;
			do
			{
				num++;
				minionIdentity = Components.LiveMinionIdentities[UnityEngine.Random.Range(0, Components.LiveMinionIdentities.Count)];
				if (num > 30)
				{
					minionIdentity = null;
					break;
				}
			}
			while (Components.LiveMinionIdentities.Count > 1 && ((base.sm.attackableTarget.Get(base.smi) != null && minionIdentity.gameObject == base.sm.attackableTarget.Get(base.smi).gameObject) || Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject) == null || (Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject).roomType != Db.Get().RoomTypes.MessHall && Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject).roomType != Db.Get().RoomTypes.GreatHall) || minionIdentity.gameObject == base.smi.master.gameObject || Game.Instance.roomProber.GetRoomOfGameObject(minionIdentity.gameObject) != Game.Instance.roomProber.GetRoomOfGameObject(base.smi.master.gameObject)));
			if (minionIdentity == null)
			{
				base.smi.GoTo(base.sm.end);
			}
			else
			{
				base.smi.sm.attackableTarget.Set(minionIdentity.GetComponent<AttackableBase>(), base.smi);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, FoodFightChore>
	{
		public class AttackStates : State
		{
			public ApproachSubState<AttackableBase> moveto;

			public State throwFood;
		}

		public TargetParameter eater;

		public TargetParameter ediblesource;

		public TargetParameter ediblechunk;

		public TargetParameter attackableTarget;

		public FloatParameter requestedfoodunits;

		public FloatParameter actualfoodunits;

		public TargetParameter locator;

		public State waitForParticipants;

		public State emoteRoar;

		public ApproachSubState<IApproachable> moveToArena;

		public FetchSubState fetch;

		public AttackStates fight;

		public State end;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = fetch;
			Target(eater);
			root.ToggleAnims("anim_loco_run_angry_kanim");
			fetch.InitializeStates(eater, ediblesource, ediblechunk, requestedfoodunits, actualfoodunits, moveToArena).ToggleAnims("anim_loco_run_angry_kanim");
			moveToArena.InitializeStates(eater, locator, waitForParticipants);
			waitForParticipants.Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<Facing>().SetFacing(Game.Instance.roomProber.GetRoomOfGameObject(smi.master.gameObject).cavity.GetCenter().x <= smi.master.transform.position.x);
			}).ToggleAnims("anim_rage_kanim").PlayAnim("idle_pre")
				.QueueAnim("idle_default", loop: true)
				.ScheduleGoTo(30f, emoteRoar)
				.EventTransition(GameHashes.GameplayEventCommence, emoteRoar);
			emoteRoar.Enter("ChooseTarget", delegate(StatesInstance smi)
			{
				smi.UpdateAttackTarget();
			}).ToggleAnims("anim_rage_kanim").PlayAnim("rage_pre")
				.QueueAnim("rage_loop")
				.QueueAnim("rage_pst")
				.OnAnimQueueComplete(fight);
			fight.DefaultState(fight.moveto);
			fight.moveto.InitializeStates(eater, attackableTarget, fight.throwFood, null, null, NavigationTactics.Range_3_ProhibitOverlap);
			fight.throwFood.Enter(delegate(StatesInstance smi)
			{
				smi.master.GetComponent<Facing>().Face(attackableTarget.Get(smi).transform.position.x);
				GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(FoodCometConfig.ID), smi.master.transform.position + Vector3.up);
				gameObject.SetActive(value: true);
				Comet comet = gameObject.GetComponent<Comet>();
				Vector3 vector = attackableTarget.Get(smi).transform.position - smi.master.transform.position;
				vector.Normalize();
				comet.Velocity = vector * 5f;
				Comet comet2 = comet;
				comet2.OnImpact = (System.Action)Delegate.Combine(comet2.OnImpact, (System.Action)delegate
				{
					GameObject gameObject3 = Grid.Objects[Grid.PosToCell(comet), 0];
					if (gameObject3 != null)
					{
						if (UnityEngine.Random.Range(0, 100) > 75)
						{
							new FleeChore(gameObject3.GetComponent<IStateMachineTarget>(), smi.master.gameObject);
						}
						else
						{
							gameObject3.Trigger(508119890);
						}
					}
				});
				GameObject gameObject2 = smi.master.GetComponent<Storage>().FindFirst(GameTags.Edible);
				if (gameObject2 != null)
				{
					Edible component = gameObject2.GetComponent<Edible>();
					float num = Math.Min(200000f, component.Calories);
					component.Calories -= num;
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, 0f - num, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.FOODFIGHT_CONTEXT, "{0}", component.GetProperName()));
					if (component.Calories <= 0f)
					{
						Util.KDestroyGameObject(gameObject2);
						smi.GoTo(end);
					}
					else
					{
						smi.GoTo(emoteRoar);
					}
				}
				else
				{
					smi.GoTo(end);
				}
			});
			end.Enter(delegate(StatesInstance smi)
			{
				Util.KDestroyGameObject(ediblechunk.Get(smi));
				smi.StopSM("complete");
			});
		}
	}

	public static readonly Precondition EdibleIsNotNull = new Precondition
	{
		id = "EdibleIsNotNull",
		description = DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
		fn = delegate(ref Precondition.Context context, object data)
		{
			return null != context.consumerState.consumer.GetSMI<RationMonitor.Instance>().GetEdible();
		}
	};

	public FoodFightChore(IStateMachineTarget master, GameObject locator)
		: base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.high, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.PersonalTime)
	{
		base.smi = new StatesInstance(this, locator);
		showAvailabilityInHoverText = false;
		AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
		AddPrecondition(EdibleIsNotNull);
	}

	public override void Begin(Precondition.Context context)
	{
		if (context.consumerState.consumer == null)
		{
			Debug.LogError("FOODFIGHTCHORE null context.consumer");
			return;
		}
		RationMonitor.Instance sMI = context.consumerState.consumer.GetSMI<RationMonitor.Instance>();
		if (sMI == null)
		{
			Debug.LogError("FOODFIGHTCHORE null RationMonitor.Instance");
			return;
		}
		Edible edible = sMI.GetEdible();
		if (edible.gameObject == null)
		{
			Debug.LogError("FOODFIGHTCHORE null edible.gameObject");
			return;
		}
		if (base.smi == null)
		{
			Debug.LogError("FOODFIGHTCHORE null smi");
			return;
		}
		if (base.smi.sm == null)
		{
			Debug.LogError("FOODFIGHTCHORE null smi.sm");
			return;
		}
		if (base.smi.sm.ediblesource == null)
		{
			Debug.LogError("FOODFIGHTCHORE null smi.sm.ediblesource");
			return;
		}
		base.smi.sm.ediblesource.Set(edible.gameObject, base.smi);
		KCrashReporter.Assert(edible.FoodInfo.CaloriesPerUnit > 0f, edible.GetProperName() + " has invalid calories per unit. Will result in NaNs");
		float num = 0.5f;
		KCrashReporter.Assert(num > 0f, "FoodFightChore is requesting an invalid amount of food");
		base.smi.sm.requestedfoodunits.Set(num, base.smi);
		base.smi.sm.eater.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
