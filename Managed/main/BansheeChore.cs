using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class BansheeChore : Chore<BansheeChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BansheeChore, object>.GameInstance
	{
		public Notification notification;

		public StatesInstance(BansheeChore master, GameObject wailer, Notification notification)
			: base(master)
		{
			base.sm.wailer.Set(wailer, base.smi);
			this.notification = notification;
		}

		public void FindAudience()
		{
			Navigator component = GetComponent<Navigator>();
			int worldId = Grid.WorldIdx[Grid.PosToCell(base.gameObject)];
			int num = int.MaxValue;
			int num2 = Grid.InvalidCell;
			List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(worldId);
			for (int i = 0; i < worldItems.Count; i++)
			{
				if (worldItems[i].IsNullOrDestroyed() || worldItems[i].gameObject == base.gameObject)
				{
					continue;
				}
				int num3 = Grid.PosToCell(worldItems[i]);
				if (component.CanReach(num3) && !worldItems[i].GetComponent<Effects>().HasEffect("WailedAt"))
				{
					int navigationCost = component.GetNavigationCost(num3);
					if (navigationCost < num)
					{
						num = navigationCost;
						num2 = num3;
					}
				}
			}
			if (num2 == Grid.InvalidCell)
			{
				num2 = FindIdleCell();
			}
			base.sm.targetWailLocation.Set(num2, base.smi);
			GoTo(base.sm.moveToAudience);
		}

		public int FindIdleCell()
		{
			Navigator component = base.smi.master.GetComponent<Navigator>();
			MinionPathFinderAbilities minionPathFinderAbilities = (MinionPathFinderAbilities)component.GetCurrentAbilities();
			minionPathFinderAbilities.SetIdleNavMaskEnabled(enabled: true);
			IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(GetComponent<MinionBrain>(), UnityEngine.Random.Range(30, 90));
			component.RunQuery(idleCellQuery);
			minionPathFinderAbilities.SetIdleNavMaskEnabled(enabled: false);
			return idleCellQuery.GetResultCell();
		}

		public void BotherAudience(float dt)
		{
			if (dt <= 0f)
			{
				return;
			}
			int num = Grid.PosToCell(base.smi.master.gameObject);
			int worldId = Grid.WorldIdx[num];
			foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(worldId))
			{
				if (worldItem.IsNullOrDestroyed() || worldItem.gameObject == base.smi.master.gameObject)
				{
					continue;
				}
				int endCell = Grid.PosToCell(worldItem);
				if (Grid.GetCellDistance(num, Grid.PosToCell(worldItem)) > STRESS.BANSHEE_WAIL_RADIUS)
				{
					continue;
				}
				HashSet<int> hashSet = new HashSet<int>();
				Grid.CollectCellsInLine(num, endCell, hashSet);
				bool flag = false;
				foreach (int item in hashSet)
				{
					if (Grid.Solid[item])
					{
						flag = true;
						break;
					}
				}
				if (!flag && !worldItem.GetComponent<Effects>().HasEffect("WailedAt"))
				{
					worldItem.GetComponent<Effects>().Add("WailedAt", should_save: true);
					worldItem.GetSMI<ThreatMonitor.Instance>().ClearMainThreat();
					new FleeChore(worldItem.GetComponent<IStateMachineTarget>(), base.smi.master.gameObject);
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BansheeChore>
	{
		public class Wail : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public TargetParameter wailer;

		public IntParameter targetWailLocation;

		public State findAudience;

		public State moveToAudience;

		public Wail wail;

		public State recover;

		public State delay;

		public State wander;

		public State complete;

		private Effect wailPreEffect;

		private Effect wailRecoverEffect;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = findAudience;
			Target(wailer);
			wailPreEffect = new Effect("BansheeWailing", DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NAME, DUPLICANTS.MODIFIERS.BANSHEE_WAILING.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
			wailPreEffect.Add(new AttributeModifier("AirConsumptionRate", 7.5f));
			Db.Get().effects.Add(wailPreEffect);
			wailRecoverEffect = new Effect("BansheeWailingRecovery", DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.NAME, DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: true);
			wailRecoverEffect.Add(new AttributeModifier("AirConsumptionRate", 1f));
			Db.Get().effects.Add(wailRecoverEffect);
			findAudience.Enter("FindAudience", delegate(StatesInstance smi)
			{
				smi.FindAudience();
			}).ToggleAnims("anim_loco_banshee_kanim");
			moveToAudience.MoveTo((StatesInstance smi) => smi.sm.targetWailLocation.Get(smi), wail, delay).ToggleAnims("anim_loco_banshee_kanim");
			wail.defaultState = wail.pre.DoNotification((StatesInstance smi) => smi.notification);
			wail.pre.ToggleAnims("anim_banshee_kanim").PlayAnim("working_pre").ToggleEffect((StatesInstance smi) => wailPreEffect)
				.OnAnimQueueComplete(wail.loop);
			wail.loop.ToggleAnims("anim_banshee_kanim").Enter(delegate(StatesInstance smi)
			{
				smi.Play("working_loop", KAnim.PlayMode.Loop);
				AcousticDisturbance.Emit(smi.master.gameObject, STRESS.BANSHEE_WAIL_RADIUS);
			}).ScheduleGoTo(5f, wail.pst)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.BotherAudience(dt);
				});
			wail.pst.ToggleAnims("anim_banshee_kanim").QueueAnim("working_pst").EventHandlerTransition(GameHashes.AnimQueueComplete, recover, (StatesInstance smi, object data) => true)
				.ScheduleGoTo(3f, recover);
			recover.ToggleEffect((StatesInstance smi) => wailRecoverEffect).ToggleAnims("anim_emotes_default_kanim").QueueAnim("breathe_pre")
				.QueueAnim("breathe_loop")
				.QueueAnim("breathe_loop")
				.QueueAnim("breathe_loop")
				.QueueAnim("breathe_pst")
				.OnAnimQueueComplete(complete);
			delay.ScheduleGoTo(1f, wander);
			wander.MoveTo((StatesInstance smi) => smi.FindIdleCell(), findAudience, findAudience).ToggleAnims("anim_loco_banshee_kanim");
			complete.Enter(delegate(StatesInstance smi)
			{
				smi.StopSM("complete");
			});
		}
	}

	private const string audienceEffectName = "WailedAt";

	public BansheeChore(ChoreType chore_type, IStateMachineTarget target, Notification notification, Action<Chore> on_complete = null)
		: base(Db.Get().ChoreTypes.BansheeWail, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, on_complete, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject, notification);
	}
}
