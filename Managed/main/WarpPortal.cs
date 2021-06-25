using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class WarpPortal : Workable
{
	public class WarpPortalSM : GameStateMachine<WarpPortalSM, WarpPortalSM.Instance, WarpPortal>
	{
		public class OccupiedStates : State
		{
			public State get_on;

			public State waiting;

			public State warping;
		}

		public new class Instance : GameInstance
		{
			public Instance(WarpPortal master)
				: base(master)
			{
			}

			public Notification CreateDupeWaitingNotification()
			{
				if (base.master.worker != null)
				{
					return new Notification(MISC.NOTIFICATIONS.WARP_PORTAL_DUPE_READY.NAME.Replace("{dupe}", base.master.worker.name), NotificationType.Neutral, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.WARP_PORTAL_DUPE_READY.TOOLTIP.Replace("{dupe}", base.master.worker.name), null, expires: false, 0f, null, null, base.master.worker.transform);
				}
				return null;
			}
		}

		public State idle;

		public State become_occupied;

		public OccupiedStates occupied;

		public State do_warp;

		public State recharging;

		public BoolParameter isCharged;

		private TargetParameter worker;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			base.serializable = SerializeType.Both_DEPRECATED;
			idle.PlayAnim("idle", KAnim.PlayMode.Loop).Enter(delegate(Instance smi)
			{
				smi.master.IsConsumed = false;
				smi.sm.isCharged.Set(value: true, smi);
				smi.master.SetAssignable(set_it: true);
			}).Exit(delegate(Instance smi)
			{
				smi.master.SetAssignable(set_it: false);
			})
				.WorkableStartTransition((Instance smi) => smi.master, become_occupied)
				.ParamTransition(isCharged, recharging, GameStateMachine<WarpPortalSM, Instance, WarpPortal, object>.IsFalse);
			become_occupied.Enter(delegate(Instance smi)
			{
				worker.Set(smi.master.worker, smi);
				smi.GoTo(occupied.get_on);
			});
			occupied.OnTargetLost(worker, idle).Target(worker).TagTransition(GameTags.Dying, idle)
				.Target(masterTarget)
				.Exit(delegate(Instance smi)
				{
					worker.Set(null, smi);
				});
			occupied.get_on.PlayAnim("sending_pre").OnAnimQueueComplete(occupied.waiting);
			occupied.waiting.PlayAnim("sending_loop", KAnim.PlayMode.Loop).ToggleNotification((Instance smi) => smi.CreateDupeWaitingNotification()).Enter(delegate(Instance smi)
			{
				smi.master.RefreshSideScreen();
			})
				.Exit(delegate(Instance smi)
				{
					smi.master.RefreshSideScreen();
				});
			occupied.warping.PlayAnim("sending_pst").OnAnimQueueComplete(do_warp);
			do_warp.Enter(delegate(Instance smi)
			{
				smi.master.Warp();
			}).GoTo(recharging);
			recharging.Enter(delegate(Instance smi)
			{
				smi.master.SetAssignable(set_it: false);
				smi.master.IsConsumed = true;
				isCharged.Set(value: false, smi);
			}).PlayAnim("recharge", KAnim.PlayMode.Loop).ToggleStatusItem(Db.Get().BuildingStatusItems.WarpPortalCharging, (Instance smi) => smi.master)
				.Update(delegate(Instance smi, float dt)
				{
					smi.master.rechargeProgress += dt;
					if (smi.master.rechargeProgress > 3000f)
					{
						isCharged.Set(value: true, smi);
						smi.master.rechargeProgress = 0f;
						smi.GoTo(idle);
					}
				});
		}
	}

	[MyCmpReq]
	public Assignable assignable;

	[MyCmpAdd]
	public Notifier notifier;

	private Chore chore;

	private WarpPortalSM.Instance warpPortalSMI;

	private Notification notification;

	public const float RECHARGE_TIME = 3000f;

	[Serialize]
	public bool IsConsumed;

	[Serialize]
	public float rechargeProgress;

	[Serialize]
	private bool discovered;

	private int selectEventHandle = -1;

	private Coroutine delayWarpRoutine = null;

	private static readonly HashedString[] printing_anim = new HashedString[3]
	{
		"printing_pre",
		"printing_loop",
		"printing_pst"
	};

	public bool ReadyToWarp => warpPortalSMI.IsInsideState(warpPortalSMI.sm.occupied.waiting);

	public bool IsWorking => warpPortalSMI.IsInsideState(warpPortalSMI.sm.occupied);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		assignable.OnAssign += Assign;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		warpPortalSMI = new WarpPortalSM.Instance(this);
		warpPortalSMI.sm.isCharged.Set(!IsConsumed, warpPortalSMI);
		warpPortalSMI.StartSM();
		selectEventHandle = Game.Instance.Subscribe(-1503271301, OnObjectSelected);
	}

	private void OnObjectSelected(object data)
	{
		if (data != null && (GameObject)data == base.gameObject && Components.LiveMinionIdentities.Count > 0)
		{
			Discover();
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(selectEventHandle);
		base.OnCleanUp();
	}

	private void Discover()
	{
		if (!discovered)
		{
			ClusterManager.Instance.GetWorld(GetTargetWorldID()).SetDiscovered(reveal_surface: true);
			SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.WarpWorldReveal).smi as SimpleEvent.StatesInstance;
			statesInstance.minions = new GameObject[1]
			{
				Components.LiveMinionIdentities[0].gameObject
			};
			statesInstance.callback = delegate
			{
				ManagementMenu.Instance.OpenClusterMap();
				ClusterMapScreen.Instance.SetTargetFocusPosition(ClusterManager.Instance.GetWorld(GetTargetWorldID()).GetMyWorldLocation());
			};
			statesInstance.ShowEventPopup(GetTargetWorldID());
			discovered = true;
		}
	}

	public void StartWarpSequence()
	{
		warpPortalSMI.GoTo(warpPortalSMI.sm.occupied.warping);
	}

	public void CancelAssignment()
	{
		CancelChore();
		assignable.Unassign();
		warpPortalSMI.GoTo(warpPortalSMI.sm.idle);
	}

	private int GetTargetWorldID()
	{
		SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag(WarpReceiverConfig.ID);
		WarpReceiver[] array = Object.FindObjectsOfType<WarpReceiver>();
		WarpReceiver[] array2 = array;
		foreach (WarpReceiver component in array2)
		{
			if (component.GetMyWorldId() != this.GetMyWorldId())
			{
				return component.GetMyWorldId();
			}
		}
		Debug.LogError("No receiver world found for warp portal sender");
		return -1;
	}

	private void Warp()
	{
		if (base.worker == null || base.worker.HasTag(GameTags.Dying) || base.worker.HasTag(GameTags.Dead))
		{
			return;
		}
		WarpReceiver warpReceiver = null;
		WarpReceiver[] array = Object.FindObjectsOfType<WarpReceiver>();
		WarpReceiver[] array2 = array;
		foreach (WarpReceiver warpReceiver2 in array2)
		{
			if (warpReceiver2.GetMyWorldId() != this.GetMyWorldId())
			{
				warpReceiver = warpReceiver2;
				break;
			}
		}
		if (warpReceiver == null)
		{
			SaveGame.Instance.GetComponent<WorldGenSpawner>().SpawnTag(WarpReceiverConfig.ID);
			warpReceiver = Object.FindObjectOfType<WarpReceiver>();
		}
		if (warpReceiver != null)
		{
			delayWarpRoutine = StartCoroutine(DelayedWarp(warpReceiver));
		}
		else
		{
			Debug.LogWarning("No warp receiver found - maybe POI stomping or failure to spawn?");
		}
		if (SelectTool.Instance.selected == GetComponent<KSelectable>())
		{
			SelectTool.Instance.Select(null, skipSound: true);
		}
	}

	public IEnumerator DelayedWarp(WarpReceiver receiver)
	{
		yield return new WaitForEndOfFrame();
		int targetID = receiver.GetMyWorldId();
		CameraController.Instance.ActiveWorldStarWipe(targetID, Grid.CellToPos(Grid.PosToCell(receiver)));
		Worker targetWorker = base.worker;
		targetWorker.StopWork();
		receiver.ReceiveWarpedDuplicant(targetWorker);
		ClusterManager.Instance.MigrateMinion(targetWorker.GetComponent<MinionIdentity>(), targetID);
		delayWarpRoutine = null;
	}

	public void SetAssignable(bool set_it)
	{
		assignable.SetCanBeAssigned(set_it);
		RefreshSideScreen();
	}

	private void Assign(IAssignableIdentity new_assignee)
	{
		CancelChore();
		if (new_assignee != null)
		{
			ActivateChore();
		}
	}

	private void ActivateChore()
	{
		Debug.Assert(chore == null);
		chore = new WorkChore<Workable>(Db.Get().ChoreTypes.Migrate, this, null, run_until_complete: true, delegate
		{
			CompleteChore();
		}, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim("anim_interacts_warp_portal_sender_kanim"), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
		SetWorkTime(float.PositiveInfinity);
		workLayer = Grid.SceneLayer.Building;
		workAnims = new HashedString[2]
		{
			"sending_pre",
			"sending_loop"
		};
		workingPstComplete = new HashedString[1]
		{
			"sending_pst"
		};
		workingPstFailed = new HashedString[1]
		{
			"idle_loop"
		};
		showProgressBar = false;
	}

	private void CancelChore()
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
			if (delayWarpRoutine != null)
			{
				StopCoroutine(delayWarpRoutine);
				delayWarpRoutine = null;
			}
		}
	}

	private void CompleteChore()
	{
		IsConsumed = true;
		chore.Cleanup();
		chore = null;
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
