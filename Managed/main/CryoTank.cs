using Klei.AI;
using STRINGS;
using UnityEngine;

public class CryoTank : StateMachineComponent<CryoTank.StatesInstance>, ISidescreenButtonControl
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, CryoTank, object>.GameInstance
	{
		public Chore defrostAnimChore;

		public StatesInstance(CryoTank master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, CryoTank>
	{
		public TargetParameter defrostedDuplicant;

		public State closed;

		public State open;

		public State defrost;

		public State defrostExit;

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
			open.GoTo(defrost).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropContents();
			});
			defrost.PlayAnim("defrost").OnAnimQueueComplete(defrostExit).Update(delegate(StatesInstance smi, float dt)
			{
				smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
			})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.ShowEventPopup();
				});
			defrostExit.PlayAnim("defrost_exit").Update(delegate(StatesInstance smi, float dt)
			{
				if (smi.defrostAnimChore == null || smi.defrostAnimChore.isComplete)
				{
					smi.GoTo(off);
				}
			}).Exit(delegate(StatesInstance smi)
			{
				smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Move);
				smi.master.Cheer();
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

	public CellOffset dropOffset = CellOffset.none;

	private GameObject opener;

	private Chore chore;

	public string SidescreenButtonText => BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTON;

	public string SidescreenButtonTooltip => BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTONTOOLTIP;

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
		Vector3 position = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(base.transform.position), dropOffset), Grid.SceneLayer.Move);
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
		base.smi.sm.defrostedDuplicant.Set(gameObject, base.smi);
		gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
		ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
		if (component2 != null)
		{
			base.smi.defrostAnimChore = new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_cryo_chamber_kanim", new HashedString[2] { "defrost", "defrost_exit" }, KAnim.PlayMode.Once);
			Vector3 position2 = gameObject.transform.GetPosition();
			position2.z = Grid.GetLayerZ(Grid.SceneLayer.Gas);
			gameObject.transform.SetPosition(position2);
			gameObject.GetMyWorld().SetDupeVisited();
		}
		SaveGame.Instance.GetComponent<ColonyAchievementTracker>().defrostedDuplicant = true;
	}

	public void ShowEventPopup()
	{
		GameObject gameObject = base.smi.sm.defrostedDuplicant.Get(base.smi);
		if (opener != null && gameObject != null)
		{
			SimpleEvent.StatesInstance statesInstance = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.CryoFriend).smi as SimpleEvent.StatesInstance;
			statesInstance.minions = new GameObject[2] { gameObject, opener };
			statesInstance.SetTextParameter("dupe", opener.GetProperName());
			statesInstance.SetTextParameter("friend", gameObject.GetProperName());
			statesInstance.ShowEventPopup();
		}
	}

	public void Cheer()
	{
		GameObject gameObject = base.smi.sm.defrostedDuplicant.Get(base.smi);
		if (opener != null && gameObject != null)
		{
			opener.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), should_save: true);
			gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), should_save: true);
			new EmoteChore(opener.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[3] { "cheer_pre", "cheer_loop", "cheer_pst" }, null);
			new EmoteChore(gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[3] { "cheer_pre", "cheer_loop", "cheer_pst" }, null);
		}
	}

	private void OnClickOpen()
	{
		ActivateChore();
	}

	private void OnClickCancel()
	{
		CancelActivateChore();
	}

	public void ActivateChore(object param = null)
	{
		if (chore == null)
		{
			GetComponent<Workable>().SetWorkTime(1.5f);
			chore = new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, this, null, run_until_complete: true, delegate
			{
				CompleteActivateChore();
			}, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, Assets.GetAnim(overrideAnim), is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.high);
		}
	}

	public void CancelActivateChore(object param = null)
	{
		if (chore != null)
		{
			chore.Cancel("User cancelled");
			chore = null;
		}
	}

	private void CompleteActivateChore()
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
