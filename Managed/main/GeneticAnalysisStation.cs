using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class GeneticAnalysisStation : Workable
{
	public class GeneticAnalysisStationSM : GameStateMachine<GeneticAnalysisStationSM, GeneticAnalysisStationSM.Instance, GeneticAnalysisStation>
	{
		public class WorkingStates : State
		{
			public State pre;

			public State loop;

			public State complete;

			public State pst;
		}

		public new class Instance : GameInstance
		{
			public Instance(GeneticAnalysisStation master)
				: base(master)
			{
			}
		}

		public State idle;

		public WorkingStates working;

		public State consumed;

		public State recharging;

		public BoolParameter isCharged;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.PlayAnim("on").Enter(delegate(Instance smi)
			{
				if (!isCharged.Get(smi) && !smi.master.RechargeRequested)
				{
					smi.master.RequestRecharge(request: true);
				}
			}).WorkableStartTransition((Instance smi) => smi.master, working.pre)
				.ParamTransition(isCharged, consumed, GameStateMachine<GeneticAnalysisStationSM, Instance, GeneticAnalysisStation, object>.IsFalse);
			working.pre.PlayAnim("working_pre").OnAnimQueueComplete(working.loop);
			working.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Update(delegate(Instance smi, float dt)
			{
				if (smi.master.chore == null || smi.master.chore.isComplete)
				{
					smi.GoTo(smi.sm.working.complete);
				}
			}).WorkableCompleteTransition((Instance smi) => smi.master, working.complete);
			working.complete.ToggleStatusItem(Db.Get().BuildingStatusItems.GeneticAnalysisCompleted, null).Enter(delegate(Instance smi)
			{
				smi.master.RefreshSideScreen();
			});
			working.pst.EventTransition(GameHashes.AnimQueueComplete, consumed);
			consumed.PlayAnim("off", KAnim.PlayMode.Once).ParamTransition(isCharged, recharging, GameStateMachine<GeneticAnalysisStationSM, Instance, GeneticAnalysisStation, object>.IsTrue).Enter(delegate(Instance smi)
			{
				if (!isCharged.Get(smi) && !smi.master.RechargeRequested)
				{
					smi.master.RequestRecharge(request: true);
				}
			});
			recharging.PlayAnim("recharging", KAnim.PlayMode.Once).OnAnimQueueComplete(idle);
		}
	}

	[MyCmpAdd]
	public Notifier notifier;

	[MyCmpReq]
	public ManualDeliveryKG delivery;

	[MyCmpReq]
	public Storage storage;

	[Serialize]
	public bool IsConsumed;

	[Serialize]
	public bool RechargeRequested;

	private Chore chore;

	private GeneticAnalysisStationSM.Instance geneticAnalysisStationSMI;

	private Notification notification;

	private static Tag RechargeTag = GameTags.UnidentifiedSeed;

	private static readonly EventSystem.IntraObjectHandler<GeneticAnalysisStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<GeneticAnalysisStation>(delegate(GeneticAnalysisStation component, object data)
	{
		component.OnStorageChange(data);
	});

	private bool storage_recursion_guard = false;

	public bool WorkComplete => geneticAnalysisStationSMI.IsInsideState(geneticAnalysisStationSMI.sm.working.complete);

	public bool IsWorking => geneticAnalysisStationSMI.IsInsideState(geneticAnalysisStationSMI.sm.working);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		lightEfficiencyBonus = false;
		IsConsumed = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		showProgressBar = false;
		geneticAnalysisStationSMI = new GeneticAnalysisStationSM.Instance(this);
		RefreshRechargeChore();
		RefreshConsumedState();
		Subscribe(-1697596308, OnStorageChangeDelegate);
		geneticAnalysisStationSMI.StartSM();
	}

	private void Recharge()
	{
		SetConsumed(consumed: false);
		RequestRecharge(request: false);
		RefreshRechargeChore();
		RefreshSideScreen();
		ActivateChore();
	}

	private void SetConsumed(bool consumed)
	{
		IsConsumed = consumed;
		RefreshConsumedState();
	}

	private void RefreshConsumedState()
	{
		geneticAnalysisStationSMI.sm.isCharged.Set(!IsConsumed, geneticAnalysisStationSMI);
	}

	private void OnStorageChange(object data)
	{
		if (storage_recursion_guard)
		{
			return;
		}
		storage_recursion_guard = true;
		if (IsConsumed)
		{
			for (int num = storage.items.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = storage.items[num];
				if (!(gameObject == null) && gameObject.HasTag(RechargeTag))
				{
					Recharge();
					break;
				}
			}
		}
		else if (chore != null && storage.items.Count == 0)
		{
			CancelChore();
		}
		storage_recursion_guard = false;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		notification = new Notification(MISC.NOTIFICATIONS.GENESHUFFLER.NAME, NotificationType.Good, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.GENESHUFFLER.TOOLTIP, notificationList.ReduceMessages(countNames: false)), null, expires: false);
		notifier.Add(notification);
		DeSelectBuilding();
	}

	private void DeSelectBuilding()
	{
		if (GetComponent<KSelectable>().IsSelected)
		{
			SelectTool.Instance.Select(null, skipSound: true);
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		if (chore != null)
		{
			chore.Cancel("aborted");
		}
		notifier.Remove(notification);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		if (chore != null)
		{
			chore.Cancel("stopped");
		}
		notifier.Remove(notification);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
	}

	public void IdentifyMutant()
	{
		CameraController.Instance.CameraGoTo(base.transform.GetPosition(), 1f, playSound: false);
		SubSpeciesInfoScreen subSpeciesInfoScreen = (SubSpeciesInfoScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.SubSpeciesInfoScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
		MutantPlant component = storage.items[0].GetComponent<MutantPlant>();
		string speciesID = PlantSubSpeciesCatalog.instance.EntityToSpeciesID(component.gameObject);
		PlantSubSpeciesCatalog.instance.IdentifySubSpecies(speciesID, component.subspeciesID);
		subSpeciesInfoScreen.DisplayDiscovery(speciesID, component.subspeciesID, this);
		SetConsumed(consumed: true);
		DeSelectBuilding();
		notifier.Remove(notification);
		geneticAnalysisStationSMI.GoTo(geneticAnalysisStationSMI.sm.consumed);
	}

	private void ActivateChore()
	{
		Debug.Assert(chore == null);
		SetWorkTime(30f);
		chore = new WorkChore<GeneticAnalysisStation>(Db.Get().ChoreTypes.GeneShuffle, this, null, run_until_complete: true, delegate
		{
			CompleteChore();
		}, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim("anim_interacts_genetic_analysisstation_kanim"), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
	}

	public void DiscardSeed()
	{
		Util.KDestroyGameObject(storage.items[0]);
		CompleteChore();
		RequestRecharge(request: true);
	}

	public void EjectSeed()
	{
		CompleteChore();
		storage.DropAll();
		RequestRecharge(request: true);
	}

	private void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void CompleteChore()
	{
		chore.Cleanup();
		chore = null;
	}

	public void RequestRecharge(bool request)
	{
		RechargeRequested = request;
		RefreshRechargeChore();
	}

	private void RefreshRechargeChore()
	{
		delivery.Pause(!RechargeRequested, "No recharge requested");
	}

	public void RefreshSideScreen()
	{
		KSelectable component = GetComponent<KSelectable>();
		if (component.IsSelected)
		{
			DetailsScreen.Instance.Refresh(base.gameObject);
		}
	}
}
