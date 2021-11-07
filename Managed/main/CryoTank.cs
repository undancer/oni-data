using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CryoTank : StateMachineComponent<CryoTank.StatesInstance>, ISidescreenButtonControl
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CryoTank, object>.GameInstance
	{
		public StatesInstance(CryoTank master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CryoTank>
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

	private GameObject opener;

	private Chore chore;

	public string SidescreenButtonText => BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTON;

	public string SidescreenButtonTooltip => BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTONTOOLTIP;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		OnClickOpen();
	}

	public bool SidescreenButtonInteractable()
	{
		return HasDefrostedFriend();
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Demolishable component = GetComponent<Demolishable>();
		if (component != null)
		{
			component.allowDemolition = !HasDefrostedFriend();
		}
	}

	public bool HasDefrostedFriend()
	{
		if (base.smi.IsInsideState(base.smi.sm.closed))
		{
			return chore == null;
		}
		return false;
	}

	public void DropContents()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(base.gameObject), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(position);
		gameObject.SetActive(value: true);
		new MinionStartingStats(is_starter_minion: false, null, "AncientKnowledge").Apply(gameObject);
		gameObject.GetComponent<MinionIdentity>().arrivalTime = Random.Range(-2000, -1000);
		MinionResume component = gameObject.GetComponent<MinionResume>();
		int num = 3;
		for (int i = 0; i < num; i++)
		{
			component.ForceAddSkillPoint();
		}
		if (opener != null)
		{
			opener.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), should_save: true);
			gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), should_save: true);
			SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.CryoFriend).smi as SimpleEvent.StatesInstance;
			statesInstance.minions = new GameObject[2] { gameObject, opener };
			statesInstance.SetTextParameter("dupe", opener.GetProperName());
			statesInstance.SetTextParameter("friend", gameObject.GetProperName());
			statesInstance.ShowEventPopup();
		}
		SaveGame.Instance.GetComponent<ColonyAchievementTracker>().defrostedDuplicant = true;
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
		opener = chore.driver.gameObject;
		base.smi.GoTo(base.smi.sm.open);
		chore = null;
		Demolishable component = base.smi.GetComponent<Demolishable>();
		if (component != null)
		{
			component.allowDemolition = true;
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}
}
