using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityThreatMonitor : GameStateMachine<EntityThreatMonitor, EntityThreatMonitor.Instance, IStateMachineTarget, EntityThreatMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public GameObject entityToProtect;

		public FactionAlignment alignment;

		private Navigator navigator;

		public ChoreDriver choreDriver;

		public Tag allyTag;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 6;

		private Action<object> refreshThreatDelegate;

		public GameObject MainThreat => mainThreat;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			alignment = master.GetComponent<FactionAlignment>();
			navigator = master.GetComponent<Navigator>();
			choreDriver = master.GetComponent<ChoreDriver>();
			refreshThreatDelegate = RefreshThreat;
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
			base.smi.GoTo(base.sm.threatened);
		}

		public void RefreshThreat(object data)
		{
			if (IsRunning() && !(entityToProtect == null))
			{
				if (base.smi.CheckForThreats())
				{
					GoToThreatened();
				}
				else if (base.smi.GetCurrentState() != base.sm.safe)
				{
					Trigger(-21431934);
					base.smi.GoTo(base.sm.safe);
				}
			}
		}

		public bool CheckForThreats()
		{
			if (entityToProtect == null)
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
			Extents extents = new Extents(Grid.PosToCell(entityToProtect), maxThreatDistance);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				FactionAlignment factionAlignment = pooledList[i].obj as FactionAlignment;
				if (!(factionAlignment.transform == null) && !(factionAlignment == alignment) && factionAlignment.IsAlignmentActive() && navigator.CanReach(factionAlignment.attackable) && (!(allyTag != null) || !factionAlignment.HasTag(allyTag)))
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

	public State safe;

	public State threatened;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = safe;
		root.EventHandler(GameHashes.ObjectDestroyed, delegate(Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		safe.Enter(delegate(Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("safe", delegate(Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_1000ms, load_balance: true);
		threatened.ToggleBehaviour(GameTags.Creatures.Defend, (Instance smi) => smi.MainThreat != null, delegate(Instance smi)
		{
			smi.GoTo(safe);
		}).Update("Threatened", CritterUpdateThreats);
	}

	private static void CritterUpdateThreats(Instance smi, float dt)
	{
		if (!smi.isMasterNull && !smi.CheckForThreats())
		{
			smi.GoTo(smi.sm.safe);
		}
	}
}
