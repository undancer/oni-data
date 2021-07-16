using STRINGS;
using UnityEngine;

public class BeeForageStates : GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>
{
	public class Def : BaseDef
	{
		public Tag oreTag;

		public float amountToMine;

		public Def(Tag tag, float amount_to_mine)
		{
			oreTag = tag;
			amountToMine = amount_to_mine;
		}
	}

	public new class Instance : GameInstance
	{
		public int targetMiningCell = Grid.InvalidCell;

		public int cellToMine = Grid.InvalidCell;

		public Pickupable forageTarget;

		public int forageTarget_cell = Grid.InvalidCell;

		public KPrefabID targetHive;

		public KAnimHashedString oreSymbolHash;

		public KAnimHashedString oreLegSymbolHash;

		public KAnimHashedString noOreLegSymbolHash;

		public CellOffset hiveCellOffset = new CellOffset(1, 1);

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			oreSymbolHash = new KAnimHashedString("snapto_thing");
			oreLegSymbolHash = new KAnimHashedString("legBeeOre");
			noOreLegSymbolHash = new KAnimHashedString("legBeeNoOre");
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.oreSymbolHash, is_visible: false);
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.oreLegSymbolHash, is_visible: false);
			base.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(base.smi.noOreLegSymbolHash, is_visible: true);
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToForage);
		}
	}

	public class ForageBehaviourStates : State
	{
		public State moveToTarget;

		public State pickupTarget;
	}

	public class MiningBehaviourStates : State
	{
		public State moveToTarget;

		public State mineTarget;
	}

	public class CollectionBehaviourStates : State
	{
		public State findTarget;

		public ForageBehaviourStates forage;

		public MiningBehaviourStates mine;
	}

	public class StorageBehaviourStates : State
	{
		public State moveToHive;

		public State storeMaterial;

		public State dropMaterial;
	}

	public class ExitStates : State
	{
		public State pre;

		public State pst;
	}

	private const int MAX_NAVIGATE_DISTANCE = 100;

	private const string oreSymbol = "snapto_thing";

	private const string oreLegSymbol = "legBeeOre";

	private const string noOreLegSymbol = "legBeeNoOre";

	public CollectionBehaviourStates collect;

	public StorageBehaviourStates storage;

	public ExitStates behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = collect.findTarget;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.FORAGINGMATERIAL.NAME, CREATURES.STATUSITEMS.FORAGINGMATERIAL.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main).Exit(UnreserveTarget).Exit(DropAll);
		collect.findTarget.Enter(delegate(Instance smi)
		{
			FindTarget(smi);
			smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
			if (smi.targetHive != null)
			{
				if (smi.forageTarget != null)
				{
					ReserveTarget(smi);
					smi.GoTo(collect.forage.moveToTarget);
					return;
				}
				if (Grid.IsValidCell(smi.targetMiningCell))
				{
					smi.GoTo(collect.mine.moveToTarget);
					return;
				}
			}
			smi.GoTo(behaviourcomplete);
		});
		collect.forage.moveToTarget.MoveTo(GetOreCell, collect.forage.pickupTarget, behaviourcomplete);
		collect.forage.pickupTarget.PlayAnim("pickup_pre").Enter(PickupComplete).OnAnimQueueComplete(storage.moveToHive);
		collect.mine.moveToTarget.MoveTo((Instance smi) => smi.targetMiningCell, collect.mine.mineTarget, behaviourcomplete);
		collect.mine.mineTarget.PlayAnim("mining_pre").QueueAnim("mining_loop").QueueAnim("mining_pst")
			.Enter(MineTarget)
			.OnAnimQueueComplete(storage.moveToHive);
		storage.Enter(HoldOre).Exit(DropOre);
		storage.moveToHive.Enter(delegate(Instance smi)
		{
			if (!smi.targetHive)
			{
				smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
			}
			if (!smi.targetHive)
			{
				smi.GoTo(storage.dropMaterial);
			}
		}).MoveTo((Instance smi) => Grid.OffsetCell(Grid.PosToCell(smi.targetHive.transform.GetPosition()), smi.hiveCellOffset), storage.storeMaterial, behaviourcomplete);
		storage.storeMaterial.PlayAnim("deposit").Exit(StoreOre).OnAnimQueueComplete(behaviourcomplete.pre);
		storage.dropMaterial.Enter(delegate(Instance smi)
		{
			smi.GoTo(behaviourcomplete);
		}).Exit(DropAll);
		behaviourcomplete.DefaultState(behaviourcomplete.pst);
		behaviourcomplete.pre.PlayAnim("spawn").OnAnimQueueComplete(behaviourcomplete.pst);
		behaviourcomplete.pst.BehaviourComplete(GameTags.Creatures.WantsToForage);
	}

	private static void FindTarget(Instance smi)
	{
		if (!FindOre(smi))
		{
			FindMineableCell(smi);
		}
	}

	private void HoldOre(Instance smi)
	{
		GameObject gameObject = smi.GetComponent<Storage>().FindFirst(smi.def.oreTag);
		if ((bool)gameObject)
		{
			KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
			KAnim.Build.Symbol source_symbol = gameObject.GetComponent<KBatchedAnimController>().CurrentAnim.animFile.build.symbols[0];
			component.GetComponent<SymbolOverrideController>().AddSymbolOverride(smi.oreSymbolHash, source_symbol, 5);
			component.SetSymbolVisiblity(smi.oreSymbolHash, is_visible: true);
			component.SetSymbolVisiblity(smi.oreLegSymbolHash, is_visible: true);
			component.SetSymbolVisiblity(smi.noOreLegSymbolHash, is_visible: false);
		}
	}

	private void DropOre(Instance smi)
	{
		KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity(smi.oreSymbolHash, is_visible: false);
		component.SetSymbolVisiblity(smi.oreLegSymbolHash, is_visible: false);
		component.SetSymbolVisiblity(smi.noOreLegSymbolHash, is_visible: true);
	}

	private static void PickupComplete(Instance smi)
	{
		if (!smi.forageTarget)
		{
			Debug.LogWarningFormat("PickupComplete forageTarget {0} is null", smi.forageTarget);
			return;
		}
		UnreserveTarget(smi);
		int num = Grid.PosToCell(smi.forageTarget);
		if (smi.forageTarget_cell != num)
		{
			Debug.LogWarningFormat("PickupComplete forageTarget {0} moved {1} != {2}", smi.forageTarget, num, smi.forageTarget_cell);
			smi.forageTarget = null;
		}
		else if (smi.forageTarget.HasTag(GameTags.Stored))
		{
			Debug.LogWarningFormat("PickupComplete forageTarget {0} was stored by {1}", smi.forageTarget, smi.forageTarget.storage);
			smi.forageTarget = null;
		}
		else
		{
			smi.forageTarget = EntitySplitter.Split(smi.forageTarget, 10f);
			smi.GetComponent<Storage>().Store(smi.forageTarget.gameObject);
		}
	}

	private static void MineTarget(Instance smi)
	{
		Storage storage = smi.master.GetComponent<Storage>();
		SimMessages.ConsumeMass(callbackIdx: Game.Instance.massConsumedCallbackManager.Add(delegate(Sim.MassConsumedCallback mass_cb_info, object data)
		{
			if (mass_cb_info.mass > 0f)
			{
				storage.AddOre(ElementLoader.elements[mass_cb_info.elemIdx].id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
			}
		}, null, "BeetaMine").index, gameCell: smi.cellToMine, element: Grid.Element[smi.cellToMine].id, mass: smi.def.amountToMine, radius: 1);
	}

	private static void StoreOre(Instance smi)
	{
		smi.master.GetComponent<Storage>().Transfer(smi.targetHive.GetComponent<Storage>());
		smi.forageTarget = null;
		smi.forageTarget_cell = Grid.InvalidCell;
		smi.targetHive = null;
	}

	private static void DropAll(Instance smi)
	{
		smi.GetComponent<Storage>().DropAll();
	}

	private static bool FindMineableCell(Instance smi)
	{
		smi.targetMiningCell = Grid.InvalidCell;
		MineableCellQuery mineableCellQuery = PathFinderQueries.mineableCellQuery.Reset(smi.def.oreTag, 20);
		smi.GetComponent<Navigator>().RunQuery(mineableCellQuery);
		if (mineableCellQuery.result_cells.Count > 0)
		{
			smi.targetMiningCell = mineableCellQuery.result_cells[Random.Range(0, mineableCellQuery.result_cells.Count)];
			foreach (Direction dIRECTION_CHECK in MineableCellQuery.DIRECTION_CHECKS)
			{
				int cellInDirection = Grid.GetCellInDirection(smi.targetMiningCell, dIRECTION_CHECK);
				if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && Grid.Element[cellInDirection].tag == smi.def.oreTag)
				{
					smi.cellToMine = cellInDirection;
					return true;
				}
			}
		}
		return false;
	}

	private static bool FindOre(Instance smi)
	{
		Navigator component = smi.GetComponent<Navigator>();
		Vector3 position = smi.transform.GetPosition();
		Pickupable forageTarget = null;
		int num = 100;
		Extents extents = new Extents((int)position.x, (int)position.y, 15);
		ListPool<ScenePartitionerEntry, BeeForageStates>.PooledList pooledList = ListPool<ScenePartitionerEntry, BeeForageStates>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		Element element = ElementLoader.GetElement(smi.def.oreTag);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			Pickupable pickupable = item.obj as Pickupable;
			if ((bool)pickupable && (bool)pickupable.GetComponent<ElementChunk>() && (bool)pickupable.GetComponent<PrimaryElement>() && pickupable.GetComponent<PrimaryElement>().Element == element && !pickupable.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				int navigationCost = component.GetNavigationCost(Grid.PosToCell(pickupable));
				if (navigationCost != -1 && navigationCost < num)
				{
					forageTarget = pickupable.GetComponent<Pickupable>();
					num = navigationCost;
				}
			}
		}
		smi.forageTarget = forageTarget;
		smi.forageTarget_cell = (smi.forageTarget ? Grid.PosToCell(smi.forageTarget) : Grid.InvalidCell);
		return smi.forageTarget != null;
	}

	private static void ReserveTarget(Instance smi)
	{
		GameObject gameObject = (smi.forageTarget ? smi.forageTarget.gameObject : null);
		if (gameObject != null)
		{
			DebugUtil.Assert(!gameObject.HasTag(GameTags.Creatures.ReservedByCreature));
			gameObject.AddTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static void UnreserveTarget(Instance smi)
	{
		GameObject go = (smi.forageTarget ? smi.forageTarget.gameObject : null);
		if (smi.forageTarget != null)
		{
			go.RemoveTag(GameTags.Creatures.ReservedByCreature);
		}
	}

	private static int GetOreCell(Instance smi)
	{
		Debug.Assert(smi.forageTarget);
		Debug.Assert(smi.forageTarget_cell != Grid.InvalidCell);
		return smi.forageTarget_cell;
	}
}
