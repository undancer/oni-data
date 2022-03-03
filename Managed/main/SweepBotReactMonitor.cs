using UnityEngine;

public class SweepBotReactMonitor : GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	private State idle;

	private State reactScaryThing;

	private State reactFriendlyThing;

	private State reactNewOrnament;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventHandler(GameHashes.OccupantChanged, delegate(Instance smi)
		{
			if (smi.master.gameObject.GetComponent<OrnamentReceptacle>().Occupant != null)
			{
				smi.GoTo(reactNewOrnament);
			}
		}).Update(delegate(Instance smi, float dt)
		{
			SweepStates.Instance sMI = smi.master.gameObject.GetSMI<SweepStates.Instance>();
			int invalidCell = Grid.InvalidCell;
			if (sMI != null)
			{
				invalidCell = ((!sMI.sm.headingRight.Get(sMI)) ? Grid.CellLeft(Grid.PosToCell(smi.master.gameObject)) : Grid.CellRight(Grid.PosToCell(smi.master.gameObject)));
				bool flag = false;
				bool flag2 = false;
				Grid.CellToXY(Grid.PosToCell(smi), out var x, out var y);
				ListPool<ScenePartitionerEntry, SweepBotReactMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, SweepBotReactMonitor>.Allocate();
				GameScenePartitioner.Instance.GatherEntries(x - 1, y - 1, 3, 3, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
				foreach (ScenePartitionerEntry item in pooledList)
				{
					Pickupable pickupable = item.obj as Pickupable;
					if (!(pickupable == null) && !(pickupable.gameObject == smi.gameObject))
					{
						int num = Grid.PosToCell(pickupable);
						if (!(Vector3.Distance(smi.gameObject.transform.position, pickupable.gameObject.transform.position) >= Grid.CellSizeInMeters))
						{
							if (pickupable.PrefabID() == "SweepBot" && num == invalidCell)
							{
								smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("bump");
								sMI.sm.headingRight.Set(!sMI.sm.headingRight.Get(sMI), sMI);
								flag = true;
							}
							else if (pickupable.HasTag(GameTags.Creature))
							{
								flag2 = true;
							}
						}
					}
				}
				pooledList.Recycle();
				if (!flag && smi.timeinstate > 10f && Grid.IsValidCell(invalidCell))
				{
					if (Grid.Objects[invalidCell, 0] != null && !Grid.Objects[invalidCell, 0].HasTag(GameTags.Dead))
					{
						smi.GoTo(reactFriendlyThing);
					}
					else if (sMI.sm.bored.Get(sMI) && Grid.Objects[invalidCell, 3] != null)
					{
						smi.GoTo(reactFriendlyThing);
					}
					else if (flag2)
					{
						smi.GoTo(reactScaryThing);
					}
				}
			}
		}, UpdateRate.SIM_33ms);
		reactScaryThing.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_neg");
		}).ToggleStatusItem(Db.Get().RobotStatusItems.ReactNegative, null, Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(idle);
		reactFriendlyThing.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_pos");
		}).ToggleStatusItem(Db.Get().RobotStatusItems.ReactPositive, null, Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(idle);
		reactNewOrnament.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("react_ornament");
		}).OnAnimQueueComplete(idle).ToggleStatusItem(Db.Get().RobotStatusItems.ReactPositive);
	}
}
