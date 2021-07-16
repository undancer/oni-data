using KSerialization;
using STRINGS;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>, ISidescreenButtonControl
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
			base.serializable = SerializeType.Both_DEPRECATED;
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

	public string[][] possible_contents_ids;

	public string machineSound;

	public string overrideAnim;

	public Vector2I dropOffset = Vector2I.zero;

	[Serialize]
	private string[] contents;

	[Serialize]
	private bool used;

	private Chore chore;

	public string SidescreenButtonText => (chore == null) ? UI.USERMENUACTIONS.OPENPOI.NAME : UI.USERMENUACTIONS.OPENPOI.NAME_OFF;

	public string SidescreenButtonTooltip => (chore == null) ? UI.USERMENUACTIONS.OPENPOI.TOOLTIP : UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void ChooseContents()
	{
		contents = possible_contents_ids[Random.Range(0, possible_contents_ids.GetLength(0))];
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void DropContents()
	{
		if (contents != null)
		{
			for (int i = 0; i < contents.Length; i++)
			{
				Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), dropOffset.x, dropOffset.y, contents[i], Grid.SceneLayer.Front).SetActive(value: true);
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(contents[i].ToTag()).GetProperName(), base.smi.master.transform);
			}
			base.gameObject.Trigger(-372600542, this);
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

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		if (chore == null)
		{
			OnClickOpen();
		}
		else
		{
			OnClickCancel();
		}
	}

	public bool SidescreenButtonInteractable()
	{
		return !used;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}
}
