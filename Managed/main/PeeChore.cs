using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PeeChore, object>.GameInstance
	{
		public Notification stressfullyEmptyingBladder = new Notification(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)));

		public AmountInstance bladder;

		private AmountInstance bodyTemperature;

		public StatesInstance(PeeChore master, GameObject worker)
			: base(master)
		{
			bladder = Db.Get().Amounts.Bladder.Lookup(worker);
			bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
			base.sm.worker.Set(worker, base.smi);
		}

		public bool IsDonePeeing()
		{
			return bladder.value <= 0f;
		}

		public void SpawnDirtyWater(float dt)
		{
			int gameCell = Grid.PosToCell(base.sm.worker.Get<KMonoBehaviour>(base.smi));
			byte index = Db.Get().Diseases.GetIndex("FoodPoisoning");
			float num = dt * (0f - bladder.GetDelta()) / bladder.GetMax();
			if (num > 0f)
			{
				float mass = 2f * num;
				Equippable equippable = GetComponent<SuitEquipper>().IsWearingAirtightSuit();
				if (equippable != null)
				{
					equippable.GetComponent<Storage>().AddLiquid(SimHashes.DirtyWater, mass, bodyTemperature.value, index, Mathf.CeilToInt(100000f * num));
				}
				else
				{
					SimMessages.AddRemoveSubstance(gameCell, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, mass, bodyTemperature.value, index, Mathf.CeilToInt(100000f * num));
				}
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PeeChore>
	{
		public TargetParameter worker;

		public State running;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = running;
			Target(worker);
			running.ToggleAnims("anim_expel_kanim").ToggleEffect("StressfulyEmptyingBladder").DoNotification((StatesInstance smi) => smi.stressfullyEmptyingBladder)
				.DoReport(ReportManager.ReportType.ToiletIncident, (StatesInstance smi) => 1f, (StatesInstance smi) => masterTarget.Get(smi).GetProperName())
				.DoTutorial(Tutorial.TutorialMessages.TM_Mopping)
				.Transition(null, (StatesInstance smi) => smi.IsDonePeeing())
				.Update("SpawnDirtyWater", delegate(StatesInstance smi, float dt)
				{
					smi.SpawnDirtyWater(dt);
				})
				.PlayAnim("working_loop", KAnim.PlayMode.Loop)
				.ToggleTag(GameTags.MakingMess)
				.Enter(delegate(StatesInstance smi)
				{
					if (Sim.IsRadiationEnabled() && smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0f)
					{
						smi.master.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
					}
				})
				.Exit(delegate(StatesInstance smi)
				{
					if (Sim.IsRadiationEnabled())
					{
						smi.master.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
						AmountInstance amountInstance = smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id);
						RadiationMonitor.Instance sMI = smi.master.gameObject.GetSMI<RadiationMonitor.Instance>();
						if (sMI != null)
						{
							float num = Math.Min(amountInstance.value, 100f * sMI.difficultySettingMod);
							smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id).ApplyDelta(0f - num);
							if (num >= 1f)
							{
								PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Mathf.FloorToInt(num).ToString() + UI.UNITSUFFIXES.RADIATION.RADS, smi.master.transform);
							}
						}
					}
				});
		}
	}

	public PeeChore(IStateMachineTarget target)
		: base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), run_until_complete: false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.compulsory, 5, is_preemptable: false, allow_in_context_menu: true, 0, add_to_daily_report: false, ReportManager.ReportType.WorkTime)
	{
		base.smi = new StatesInstance(this, target.gameObject);
	}
}
