using UnityEngine;

public class SweetBotReactMonitor : GameStateMachine<SweetBotReactMonitor, SweetBotReactMonitor.Instance, IStateMachineTarget, SweetBotReactMonitor.Def>
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
			int checkCell = Grid.InvalidCell;
			if (sMI != null)
			{
				if (sMI.sm.headingRight.Get(sMI))
				{
					checkCell = Grid.CellRight(Grid.PosToCell(smi.master.gameObject));
				}
				else
				{
					checkCell = Grid.CellLeft(Grid.PosToCell(smi.master.gameObject));
				}
				Brain brain = Components.Brains.Items.Find((Brain match) => Grid.PosToCell(match) == checkCell);
				if (brain != null && brain.GetComponent<KPrefabID>().PrefabID() == "SweepBot")
				{
					if (Vector3.Distance(smi.master.gameObject.transform.position, brain.gameObject.transform.position) < Grid.CellSizeInMeters)
					{
						smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("bump");
						sMI.sm.headingRight.Set(!sMI.sm.headingRight.Get(sMI), sMI);
					}
				}
				else if (brain != null && smi.timeinstate > 10f && Grid.IsValidCell(checkCell))
				{
					if (Grid.Objects[checkCell, 0] != null && !Grid.Objects[checkCell, 0].HasTag(GameTags.Dead))
					{
						smi.GoTo(reactFriendlyThing);
					}
					else if (sMI.sm.bored.Get(sMI) && Grid.Objects[checkCell, 3] != null)
					{
						smi.GoTo(reactFriendlyThing);
					}
					else
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
