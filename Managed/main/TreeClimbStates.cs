using STRINGS;
using UnityEngine;

public class TreeClimbStates : GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		private Storage storage;

		private static readonly Vector2 VEL_MIN = new Vector2(-1f, 2f);

		private static readonly Vector2 VEL_MAX = new Vector2(1f, 4f);

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToClimbTree);
			storage = GetComponent<Storage>();
		}

		public void Toss(Pickupable pu)
		{
			Pickupable pickupable = pu.Take(Mathf.Min(1f, pu.UnreservedAmount));
			if (pickupable != null)
			{
				storage.Store(pickupable.gameObject, hide_popups: true);
				storage.Drop(pickupable.gameObject);
				Throw(pickupable.gameObject);
			}
		}

		private void Throw(GameObject ore_go)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			int num = Grid.PosToCell(position);
			int num2 = Grid.CellAbove(num);
			Vector2 initial_velocity;
			if ((Grid.IsValidCell(num) && Grid.Solid[num]) || (Grid.IsValidCell(num2) && Grid.Solid[num2]))
			{
				initial_velocity = Vector2.zero;
			}
			else
			{
				position.y += 0.5f;
				initial_velocity = new Vector2(Random.Range(VEL_MIN.x, VEL_MAX.x), Random.Range(VEL_MIN.y, VEL_MAX.y));
			}
			ore_go.transform.SetPosition(position);
			if (GameComps.Fallers.Has(ore_go))
			{
				GameComps.Fallers.Remove(ore_go);
			}
			GameComps.Fallers.Add(ore_go, initial_velocity);
		}
	}

	public class ClimbState : State
	{
		public State pre;

		public State loop;

		public State pst;
	}

	public ApproachSubState<Uprootable> moving;

	public ClimbState climbing;

	public State behaviourcomplete;

	public TargetParameter target;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moving;
		root.Enter(SetTarget).Enter(delegate(Instance smi)
		{
			if (!ReserveClimbable(smi))
			{
				smi.GoTo(behaviourcomplete);
			}
		}).Exit(UnreserveClimbable)
			.ToggleStatusItem(CREATURES.STATUSITEMS.RUMMAGINGSEED.NAME, CREATURES.STATUSITEMS.RUMMAGINGSEED.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		moving.MoveTo(GetClimbableCell, climbing, behaviourcomplete);
		climbing.DefaultState(climbing.pre);
		climbing.pre.PlayAnim("rummage_pre").OnAnimQueueComplete(climbing.loop);
		climbing.loop.QueueAnim("rummage_loop", loop: true).ScheduleGoTo(3.5f, climbing.pst).Update(Rummage, UpdateRate.SIM_1000ms);
		climbing.pst.QueueAnim("rummage_pst").OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToClimbTree);
	}

	private static void SetTarget(Instance smi)
	{
		smi.sm.target.Set(smi.GetSMI<ClimbableTreeMonitor.Instance>().climbTarget, smi);
	}

	private static bool ReserveClimbable(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null && !gameObject.HasTag(GameTags.Creatures.ReservedByCreature))
		{
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
			return true;
		}
		return false;
	}

	private static void UnreserveClimbable(Instance smi)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (gameObject != null)
		{
			gameObject.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void Rummage(Instance smi, float dt)
	{
		GameObject gameObject = smi.sm.target.Get(smi);
		if (!(gameObject != null))
		{
			return;
		}
		BuddingTrunk component = gameObject.GetComponent<BuddingTrunk>();
		if ((bool)component)
		{
			component.ExtractExtraSeed();
			return;
		}
		Storage component2 = gameObject.GetComponent<Storage>();
		if ((bool)component2 && component2.items.Count > 0)
		{
			int index = Random.Range(0, component2.items.Count - 1);
			GameObject gameObject2 = component2.items[index];
			Pickupable pickupable = (gameObject2 ? gameObject2.GetComponent<Pickupable>() : null);
			if ((bool)pickupable && pickupable.UnreservedAmount > 0.01f)
			{
				smi.Toss(pickupable);
			}
		}
	}

	private static int GetClimbableCell(Instance smi)
	{
		return Grid.PosToCell(smi.sm.target.Get(smi));
	}
}
