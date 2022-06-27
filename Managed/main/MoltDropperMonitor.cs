using KSerialization;
using UnityEngine;

public class MoltDropperMonitor : GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>
{
	public class Def : BaseDef
	{
		public string onGrowDropID;

		public float massToDrop;

		public SimHashes blockedElement;
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public bool spawnedThisCycle;

		[Serialize]
		public float timeOfLastDrop;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "ElementDropperMonitor.Instance");
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		}

		private void OnCellChange()
		{
			base.sm.cellChangedSignal.Trigger(this);
		}

		public bool ShouldDropElement()
		{
			if (IsValidTimeToDrop() && !base.smi.HasTag(GameTags.Creatures.Hungry) && !base.smi.HasTag(GameTags.Creatures.Unhappy))
			{
				return IsValidDropCell();
			}
			return false;
		}

		public void Drop()
		{
			GameObject obj = Scenario.SpawnPrefab(GetDropSpawnLocation(), 0, 0, base.def.onGrowDropID);
			obj.SetActive(value: true);
			obj.GetComponent<PrimaryElement>().Mass = base.def.massToDrop;
			spawnedThisCycle = true;
			timeOfLastDrop = GameClock.Instance.GetTime();
		}

		private int GetDropSpawnLocation()
		{
			int num = Grid.PosToCell(base.gameObject);
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				return num2;
			}
			return num;
		}

		public bool IsValidTimeToDrop()
		{
			if (!spawnedThisCycle)
			{
				if (!(timeOfLastDrop <= 0f))
				{
					return GameClock.Instance.GetTime() - timeOfLastDrop > 600f;
				}
				return true;
			}
			return false;
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.Element[num].id == base.def.blockedElement)
			{
				return false;
			}
			return true;
		}
	}

	public BoolParameter droppedThisCycle = new BoolParameter(default_value: false);

	public State satisfied;

	public State drop;

	public Signal cellChangedSignal;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.EventHandler(GameHashes.NewDay, (Instance smi) => GameClock.Instance, delegate(Instance smi)
		{
			smi.spawnedThisCycle = false;
		});
		satisfied.OnSignal(cellChangedSignal, drop, (Instance smi) => smi.ShouldDropElement());
		drop.Enter(delegate(Instance smi)
		{
			smi.Drop();
		}).EventTransition(GameHashes.NewDay, (Instance smi) => GameClock.Instance, satisfied);
	}
}
