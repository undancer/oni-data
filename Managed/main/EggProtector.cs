using UnityEngine;

public class EggProtector : GameStateMachine<EggProtector, EggProtector.Instance, IStateMachineTarget, EggProtector.Def>
{
	public class Def : BaseDef
	{
		public Tag protectorTag;

		public bool shouldProtect;

		public Def(Tag tag, bool shouldProtect)
		{
			protectorTag = tag;
			this.shouldProtect = shouldProtect;
		}
	}

	public new class Instance : GameInstance
	{
		public GameObject eggToGuard;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			base.gameObject.GetSMI<EntityThreatMonitor.Instance>().allyTag = def.protectorTag;
		}

		public void CheckDistanceToEgg()
		{
			int navigationCost = base.smi.GetComponent<Navigator>().GetNavigationCost(Grid.PosToCell(eggToGuard));
			if (navigationCost > 20)
			{
				base.sm.needsToMoveCloser.Set(value: true, base.smi);
			}
			else if (navigationCost < 0)
			{
				base.sm.needsToMoveCloser.Set(value: false, base.smi);
			}
		}

		public void CanProtectEgg()
		{
			bool flag = true;
			if (eggToGuard == null)
			{
				flag = false;
			}
			Navigator component = base.smi.GetComponent<Navigator>();
			if (flag)
			{
				int num = 150;
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(eggToGuard));
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

		public void FindEggToGuard()
		{
			if (!base.def.shouldProtect)
			{
				return;
			}
			GameObject gameObject = null;
			int num = 100;
			Navigator component = base.smi.GetComponent<Navigator>();
			foreach (Pickupable pickupable in Components.Pickupables)
			{
				if (pickupable.HasTag("CrabEgg".ToTag()) && !(Vector2.Distance(base.smi.transform.position, pickupable.transform.position) > 25f))
				{
					int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable));
					if (navigationCost != -1 && navigationCost < num)
					{
						gameObject = pickupable.gameObject;
						num = navigationCost;
					}
				}
			}
			SetEggToGuard(gameObject);
		}

		public void SetEggToGuard(GameObject egg)
		{
			eggToGuard = egg;
			base.gameObject.GetSMI<EntityThreatMonitor.Instance>().entityToProtect = egg;
			base.sm.hasEggToGuard.Set(egg != null, base.smi);
		}

		public int GetEggPos()
		{
			return Grid.PosToCell(eggToGuard);
		}
	}

	public class GuardingStates : State
	{
		public State idle;

		public State return_to_egg;
	}

	public BoolParameter needsToMoveCloser;

	public BoolParameter hasEggToGuard;

	public State idle;

	public GuardingStates guarding;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.ParamTransition(hasEggToGuard, guarding.idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsTrue).EventHandler(GameHashes.LayEgg, delegate(Instance smi)
		{
			smi.FindEggToGuard();
		}).Update(delegate(Instance smi, float dt)
		{
			smi.FindEggToGuard();
		}, UpdateRate.SIM_4000ms);
		guarding.Enter(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim("pincher_kanim"), null, "_heat");
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
		}).Exit(delegate(Instance smi)
		{
			smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim("pincher_kanim").GetData());
			smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
		}).ParamTransition(hasEggToGuard, idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsFalse)
			.Update(delegate(Instance smi, float dt)
			{
				smi.CanProtectEgg();
			}, UpdateRate.SIM_1000ms);
		guarding.idle.ParamTransition(needsToMoveCloser, guarding.return_to_egg, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsTrue);
		guarding.return_to_egg.MoveTo((Instance smi) => smi.GetEggPos(), null, null, update_cell: true).ParamTransition(needsToMoveCloser, guarding.idle, GameStateMachine<EggProtector, Instance, IStateMachineTarget, Def>.IsFalse);
	}
}
