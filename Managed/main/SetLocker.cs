using KSerialization;
using STRINGS;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SetLocker, object>.GameInstance
	{
		public StatesInstance(SetLocker master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SetLocker>
	{
		public State closed;

		public State open;

		public State off;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = closed;
			base.serializable = true;
			closed.PlayAnim("on").Enter(delegate(StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component2 = smi.master.GetComponent<LoopingSounds>();
					if (component2 != null)
					{
						component2.StartSound(GlobalAssets.GetSound(smi.master.machineSound));
					}
				}
			});
			open.PlayAnim("working").OnAnimQueueComplete(off).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropContents();
			});
			off.PlayAnim("off").Enter(delegate(StatesInstance smi)
			{
				if (smi.master.machineSound != null)
				{
					LoopingSounds component = smi.master.GetComponent<LoopingSounds>();
					if (component != null)
					{
						component.StopSound(GlobalAssets.GetSound(smi.master.machineSound));
					}
				}
			});
		}
	}

	public string[] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public Vector2I dropOffset = Vector2I.zero;

	[Serialize]
	private string contents = "";

	private static readonly EventSystem.IntraObjectHandler<SetLocker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SetLocker>(delegate(SetLocker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	[Serialize]
	private bool used;

	private Chore chore;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		contents = possible_contents_ids[Random.Range(0, possible_contents_ids.Length)];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	public void DropContents()
	{
		Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), dropOffset.x, dropOffset.y, contents, Grid.SceneLayer.Front).SetActive(value: true);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(contents.ToTag()).GetProperName(), base.smi.master.transform);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.IsInsideState(base.smi.sm.closed) && !used)
		{
			KIconButtonMenu.ButtonInfo button = ((chore != null) ? new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.OPENPOI.NAME_OFF, OnClickCancel, Action.NumActions, null, null, null, UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_empty_contents", UI.USERMENUACTIONS.OPENPOI.NAME, OnClickOpen, Action.NumActions, null, null, null, UI.USERMENUACTIONS.OPENPOI.TOOLTIP));
			Game.Instance.userMenu.AddButton(base.gameObject, button);
		}
	}

	private void OnClickOpen()
	{
		ActivateChore();
	}

	private void OnClickCancel()
	{
		CancelChore();
	}

	public void ActivateChore(object param = null)
	{
		if (chore == null)
		{
			GetComponent<Workable>().SetWorkTime(1.5f);
			chore = new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, this, null, run_until_complete: true, delegate
			{
				CompleteChore();
			}, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim(overrideAnim), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.high);
			OnRefreshUserMenu(null);
		}
	}

	public void CancelChore(object param = null)
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void CompleteChore()
	{
		used = true;
		base.smi.GoTo(base.smi.sm.open);
		chore = null;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}
}
