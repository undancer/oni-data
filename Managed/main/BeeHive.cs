using System.Collections;
using STRINGS;
using UnityEngine;

public class BeeHive : StateMachineComponent<BeeHive.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, BeeHive, object>.GameInstance
	{
		public Chore emptyChore;

		public StatesInstance(BeeHive smi)
			: base(smi)
		{
		}

		public void CreateEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("dupe");
			}
			HiveWorkableEmpty component = base.master.GetComponent<HiveWorkableEmpty>();
			emptyChore = new WorkChore<HiveWorkableEmpty>(Db.Get().ChoreTypes.EmptyStorage, component, null, run_until_complete: true, OnEmptyComplete, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
		}

		public void CancelEmptyChore()
		{
			if (emptyChore != null)
			{
				emptyChore.Cancel("Cancelled");
				emptyChore = null;
			}
		}

		private void OnEmptyComplete(Chore chore)
		{
			emptyChore = null;
			base.master.storage.Drop(SimHashes.UraniumOre.CreateTag());
		}
	}

	public class States : GameStateMachine<States, StatesInstance, BeeHive>
	{
		public State dayTime;

		public State nightTime;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = dayTime;
			dayTime.EventTransition(GameHashes.Nighttime, (StatesInstance smi) => GameClock.Instance, nightTime).Enter(delegate(StatesInstance smi)
			{
				smi.master.ReleaseBees();
			});
			nightTime.EventTransition(GameHashes.NewDay, (StatesInstance smi) => GameClock.Instance, dayTime).Exit(delegate(StatesInstance smi)
			{
				smi.master.AddNewLarvaToHive();
			});
		}
	}

	public string beePrefabID;

	public string larvaPrefabID;

	public Storage storage;

	private static readonly EventSystem.IntraObjectHandler<BeeHive> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<BeeHive>(delegate(BeeHive component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BeeHive> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BeeHive>(delegate(BeeHive component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BeeHive> OnStartDeconstructDelegate = new EventSystem.IntraObjectHandler<BeeHive>(delegate(BeeHive component, object data)
	{
		component.OnStartDeconstruct(data);
	});

	private Coroutine newGameSpawnRoutine;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(1119167081, OnNewGameSpawnDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.BeeHives.Add(this);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(1830962028, OnStartDeconstructDelegate);
	}

	protected override void OnCleanUp()
	{
		if (newGameSpawnRoutine != null)
		{
			StopCoroutine(newGameSpawnRoutine);
		}
		Components.BeeHives.Remove(this);
		base.OnCleanUp();
	}

	public void OnStartDeconstruct(object data)
	{
		ReleaseBees();
	}

	public void DEBUG_SpawnBees()
	{
		if (storage.IsEmpty())
		{
			int num = 3;
			for (int i = 0; i < num; i++)
			{
				AddNewBeeToHive();
			}
			num = 2;
			for (int j = 0; j < num; j++)
			{
				AddNewLarvaToHive();
			}
		}
		ReleaseBees();
	}

	private void OnNewGameSpawn(object data)
	{
		newGameSpawnRoutine = StartCoroutine(NewGamePopulateHiveRoutine());
	}

	private IEnumerator NewGamePopulateHiveRoutine()
	{
		int numBees2 = 3;
		for (int j = 0; j < numBees2; j++)
		{
			yield return new WaitForEndOfFrame();
			AddNewBeeToHive();
		}
		numBees2 = 2;
		for (int i = 0; i < numBees2; i++)
		{
			yield return new WaitForEndOfFrame();
			AddNewLarvaToHive();
		}
		newGameSpawnRoutine = null;
		yield return 0;
	}

	public void ReleaseBees()
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		storage.Drop(beePrefabID.ToTag(), pooledList);
		foreach (GameObject item in pooledList)
		{
			item.Trigger(-1220248099);
		}
		pooledList.Clear();
		storage.Drop(larvaPrefabID.ToTag(), pooledList);
		foreach (GameObject item2 in pooledList)
		{
			item2.Trigger(-1220248099);
		}
		pooledList.Recycle();
	}

	public void AddNewLarvaToHive()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(larvaPrefabID), base.transform.GetPosition());
		gameObject.SetActive(value: true);
		EnterHive(gameObject);
	}

	public void AddNewBeeToHive()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(beePrefabID), base.transform.GetPosition());
		gameObject.SetActive(value: true);
		EnterHive(gameObject);
	}

	public void EnterHive(GameObject bee)
	{
		storage.Store(bee, hide_popups: true);
		bee.Trigger(-2099923209);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.emptyChore != null)
		{
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("status_item_barren", UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.NAME, delegate
			{
				base.smi.CancelEmptyChore();
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.TOOLTIP));
		}
		else
		{
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("status_item_fabricator_empty", UI.USERMENUACTIONS.EMPTYBEEHIVE.NAME, delegate
			{
				base.smi.CreateEmptyChore();
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.EMPTYBEEHIVE.TOOLTIP));
		}
	}
}
