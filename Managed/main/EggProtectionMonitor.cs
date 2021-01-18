using System;
using System.Collections.Generic;
using UnityEngine;

public class EggProtectionMonitor : GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>
{
	public class Def : BaseDef
	{
		public Tag[] allyTags;
	}

	public class GuardEggStates : State
	{
		public State safe;

		public State threatened;
	}

	public new class Instance : GameInstance
	{
		private struct Egg
		{
			public GameObject game_object;

			public int cell;
		}

		private struct FindEggsTask : IWorkItem<List<KPrefabID>>
		{
			private static readonly Tag EGG_TAG = "CrabEgg".ToTag();

			private ListPool<int, EggProtectionMonitor>.PooledList eggs;

			private int start;

			private int end;

			public FindEggsTask(int start, int end)
			{
				this.start = start;
				this.end = end;
				eggs = ListPool<int, EggProtectionMonitor>.Allocate();
			}

			public void Run(List<KPrefabID> prefab_ids)
			{
				for (int i = start; i != end; i++)
				{
					if (prefab_ids[i].HasTag(EGG_TAG))
					{
						eggs.Add(i);
					}
				}
			}

			public void Finish(List<KPrefabID> prefab_ids, List<Egg> eggs)
			{
				foreach (int egg in this.eggs)
				{
					GameObject gameObject = prefab_ids[egg].gameObject;
					eggs.Add(new Egg
					{
						game_object = gameObject,
						cell = Grid.PosToCell(gameObject)
					});
				}
				this.eggs.Recycle();
			}
		}

		public GameObject eggToProtect;

		public FactionAlignment alignment;

		private Navigator navigator;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 12;

		private Action<object> refreshThreatDelegate;

		private static WorkItemCollection<FindEggsTask, List<KPrefabID>> find_eggs_job = new WorkItemCollection<FindEggsTask, List<KPrefabID>>();

		public GameObject MainThreat => mainThreat;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			alignment = master.GetComponent<FactionAlignment>();
			navigator = master.GetComponent<Navigator>();
			refreshThreatDelegate = RefreshThreat;
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if (eggToProtect == null)
			{
				flag = false;
			}
			if (flag)
			{
				int num = 150;
				int navigationCost = navigator.GetNavigationCost(Grid.PosToCell(eggToProtect));
				if (navigationCost == -1 || navigationCost >= num)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				SetEggToGuard(null);
			}
		}

		public static void FindEggToGuard(List<UpdateBucketWithUpdater<Instance>.Entry> instances, float time_delta)
		{
			ListPool<KPrefabID, EggProtectionMonitor>.PooledList pooledList = ListPool<KPrefabID, EggProtectionMonitor>.Allocate();
			pooledList.Capacity = Mathf.Max(pooledList.Capacity, Components.IncubationMonitors.Count);
			foreach (IncubationMonitor.Instance incubationMonitor in Components.IncubationMonitors)
			{
				pooledList.Add(incubationMonitor.gameObject.GetComponent<KPrefabID>());
			}
			ListPool<Egg, EggProtectionMonitor>.PooledList pooledList2 = ListPool<Egg, EggProtectionMonitor>.Allocate();
			find_eggs_job.Reset(pooledList);
			for (int i = 0; i < pooledList.Count; i += 256)
			{
				find_eggs_job.Add(new FindEggsTask(i, Mathf.Min(i + 256, pooledList.Count)));
			}
			GlobalJobManager.Run(find_eggs_job);
			for (int j = 0; j != find_eggs_job.Count; j++)
			{
				find_eggs_job.GetWorkItem(j).Finish(pooledList, pooledList2);
			}
			pooledList.Recycle();
			foreach (UpdateBucketWithUpdater<Instance>.Entry item in new List<UpdateBucketWithUpdater<Instance>.Entry>(instances))
			{
				GameObject eggToGuard = null;
				int num = 100;
				foreach (Egg item2 in pooledList2)
				{
					int navigationCost = item.data.navigator.GetNavigationCost(item2.cell);
					if (navigationCost != -1 && navigationCost < num)
					{
						eggToGuard = item2.game_object;
						num = navigationCost;
					}
				}
				item.data.SetEggToGuard(eggToGuard);
			}
			pooledList2.Recycle();
		}

		public void SetEggToGuard(GameObject egg)
		{
			eggToProtect = egg;
			base.sm.hasEggToGuard.Set(egg != null, base.smi);
		}

		public void SetMainThreat(GameObject threat)
		{
			if (threat == mainThreat)
			{
				return;
			}
			if (mainThreat != null)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
				if (threat == null)
				{
					Trigger(2144432245);
				}
			}
			if (mainThreat != null)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
			}
			mainThreat = threat;
			if (mainThreat != null)
			{
				mainThreat.Subscribe(1623392196, refreshThreatDelegate);
				mainThreat.Subscribe(1969584890, refreshThreatDelegate);
			}
		}

		public void Cleanup(object data)
		{
			if ((bool)mainThreat)
			{
				mainThreat.Unsubscribe(1623392196, refreshThreatDelegate);
				mainThreat.Unsubscribe(1969584890, refreshThreatDelegate);
			}
		}

		public void GoToThreatened()
		{
			base.smi.GoTo(base.sm.guard.threatened);
		}

		public void RefreshThreat(object data)
		{
			if (IsRunning() && !(eggToProtect == null))
			{
				if (base.smi.CheckForThreats())
				{
					GoToThreatened();
				}
				else if (base.smi.GetCurrentState() != base.sm.guard.safe)
				{
					Trigger(-21431934);
					base.smi.GoTo(base.sm.guard.safe);
				}
			}
		}

		public bool CheckForThreats()
		{
			if (eggToProtect == null)
			{
				return false;
			}
			GameObject x = FindThreat();
			SetMainThreat(x);
			return x != null;
		}

		public GameObject FindThreat()
		{
			threats.Clear();
			ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
			Extents extents = new Extents(Grid.PosToCell(eggToProtect), maxThreatDistance);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				FactionAlignment factionAlignment = pooledList[i].obj as FactionAlignment;
				if (factionAlignment.transform == null || factionAlignment == alignment || !factionAlignment.IsAlignmentActive() || !navigator.CanReach(factionAlignment.attackable))
				{
					continue;
				}
				bool flag = false;
				Tag[] allyTags = base.def.allyTags;
				foreach (Tag tag in allyTags)
				{
					if (factionAlignment.HasTag(tag))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					threats.Add(factionAlignment);
				}
			}
			pooledList.Recycle();
			return PickBestTarget(threats);
		}

		public GameObject PickBestTarget(List<FactionAlignment> threats)
		{
			float num = 1f;
			Vector2 a = base.gameObject.transform.GetPosition();
			GameObject result = null;
			float num2 = float.PositiveInfinity;
			for (int num3 = threats.Count - 1; num3 >= 0; num3--)
			{
				FactionAlignment factionAlignment = threats[num3];
				float num4 = Vector2.Distance(a, factionAlignment.transform.GetPosition()) / num;
				if (num4 < num2)
				{
					num2 = num4;
					result = factionAlignment.gameObject;
				}
			}
			return result;
		}
	}

	public BoolParameter hasEggToGuard;

	public State find_egg;

	public GuardEggStates guard;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = find_egg;
		root.EventHandler(GameHashes.ObjectDestroyed, delegate(Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		find_egg.BatchUpdate(Instance.FindEggToGuard).ParamTransition(hasEggToGuard, guard.safe, GameStateMachine<EggProtectionMonitor, Instance, IStateMachineTarget, Def>.IsTrue);
		guard.Enter(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat");
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData());
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).Update("evaulate_egg", delegate(Instance smi, float dt)
		{
			smi.CanProtectEgg();
		}, UpdateRate.SIM_1000ms, load_balance: true)
			.ParamTransition(hasEggToGuard, find_egg, GameStateMachine<EggProtectionMonitor, Instance, IStateMachineTarget, Def>.IsFalse);
		guard.safe.Enter(delegate(Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("safe", delegate(Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_200ms, load_balance: true);
		guard.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (Instance smi) => smi.MainThreat != null, delegate(Instance smi)
		{
			smi.GoTo(guard.safe);
		}).Update("Threatened", CritterUpdateThreats);
	}

	private static void CritterUpdateThreats(Instance smi, float dt)
	{
		if (!smi.isMasterNull && !smi.CheckForThreats())
		{
			smi.GoTo(smi.sm.guard.safe);
		}
	}
}
