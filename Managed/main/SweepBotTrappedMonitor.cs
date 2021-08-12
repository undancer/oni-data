using UnityEngine;

public class SweepBotTrappedMonitor : GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>
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

	public static CellOffset[] defaultOffsets = new CellOffset[1]
	{
		new CellOffset(0, 0)
	};

	public State notTrapped;

	public State trapped;

	public State death;

	public State destroySelf;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = notTrapped;
		notTrapped.Update(delegate(Instance smi, float dt)
		{
			StorageUnloadMonitor.Instance sMI2 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
			Storage storage3 = sMI2.sm.sweepLocker.Get(sMI2);
			Navigator component3 = smi.master.GetComponent<Navigator>();
			if (storage3 == null)
			{
				smi.GoTo(death);
			}
			else if ((smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour) || smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.UnloadBehaviour)) && !component3.CanReach(Grid.PosToCell(storage3), defaultOffsets))
			{
				smi.GoTo(trapped);
			}
		}, UpdateRate.SIM_1000ms);
		trapped.ToggleBehaviour(GameTags.Robots.Behaviours.TrappedBehaviour, (Instance data) => true).ToggleStatusItem(Db.Get().RobotStatusItems.CantReachStation, null, Db.Get().StatusItemCategories.Main).Update(delegate(Instance smi, float dt)
		{
			StorageUnloadMonitor.Instance sMI = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
			Storage storage2 = sMI.sm.sweepLocker.Get(sMI);
			Navigator component2 = smi.master.GetComponent<Navigator>();
			if (storage2 == null)
			{
				smi.GoTo(death);
			}
			else if ((!smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour) && !smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.UnloadBehaviour)) || component2.CanReach(Grid.PosToCell(storage2), defaultOffsets))
			{
				smi.GoTo(notTrapped);
			}
			if (storage2 != null && component2.CanReach(Grid.PosToCell(storage2), defaultOffsets))
			{
				smi.GoTo(notTrapped);
			}
			else if (storage2 == null)
			{
				smi.GoTo(death);
			}
		}, UpdateRate.SIM_1000ms);
		death.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim("death");
		}).OnAnimQueueComplete(destroySelf);
		destroySelf.Enter(delegate(Instance smi)
		{
			Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactDust, smi.master.transform.position, 0f);
			Storage[] components = smi.master.gameObject.GetComponents<Storage>();
			foreach (Storage storage in components)
			{
				for (int num = storage.items.Count - 1; num >= 0; num--)
				{
					GameObject gameObject = storage.Drop(storage.items[num]);
					if (gameObject != null)
					{
						if (GameComps.Fallers.Has(gameObject))
						{
							GameComps.Fallers.Remove(gameObject);
						}
						GameComps.Fallers.Add(gameObject, new Vector2(Random.Range(-5, 5), 8f));
					}
				}
			}
			PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
			component.Element.substance.SpawnResource(Grid.CellToPosCCC(Grid.PosToCell(smi.master.gameObject), Grid.SceneLayer.Ore), SweepBotConfig.MASS, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
			Util.KDestroyGameObject(smi.master.gameObject);
		});
	}
}
