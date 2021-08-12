using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ChainedBuilding : GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>
{
	public class Def : BaseDef
	{
		public Tag headBuildingTag;

		public Tag linkBuildingTag;

		public ObjectLayer objectLayer;
	}

	public class StatesInstance : GameInstance
	{
		private int widthInCells;

		private List<int> neighbourCheckCells;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			BuildingDef buildingDef = master.GetComponent<Building>().Def;
			widthInCells = buildingDef.WidthInCells;
			int cell = Grid.PosToCell(this);
			neighbourCheckCells = new List<int>
			{
				Grid.OffsetCell(cell, -(widthInCells - 1) / 2 - 1, 0),
				Grid.OffsetCell(cell, widthInCells / 2 + 1, 0)
			};
		}

		public override void StartSM()
		{
			base.StartSM();
			bool foundHead = false;
			HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain = HashSetPool<StatesInstance, StatesInstance>.Allocate();
			CollectToChain(ref chain, ref foundHead);
			PropogateFoundHead(foundHead, chain);
			PropagateChangedEvent(this, chain);
			chain.Recycle();
		}

		public void DEBUG_Relink()
		{
			bool foundHead = false;
			HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain = HashSetPool<StatesInstance, StatesInstance>.Allocate();
			CollectToChain(ref chain, ref foundHead);
			PropogateFoundHead(foundHead, chain);
			chain.Recycle();
		}

		protected override void OnCleanUp()
		{
			HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain = HashSetPool<StatesInstance, StatesInstance>.Allocate();
			foreach (int neighbourCheckCell in neighbourCheckCells)
			{
				bool foundHead = false;
				CollectNeighbourToChain(neighbourCheckCell, ref chain, ref foundHead, this);
				PropogateFoundHead(foundHead, chain);
				PropagateChangedEvent(this, chain);
				chain.Clear();
			}
			chain.Recycle();
			base.OnCleanUp();
		}

		public HashSet<StatesInstance> GetLinkedBuildings(ref HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain)
		{
			bool foundHead = false;
			CollectToChain(ref chain, ref foundHead);
			return chain;
		}

		private void PropogateFoundHead(bool foundHead, HashSet<StatesInstance> chain)
		{
			foreach (StatesInstance item in chain)
			{
				item.sm.isConnectedToHead.Set(foundHead, item);
			}
		}

		private void PropagateChangedEvent(StatesInstance changedLink, HashSet<StatesInstance> chain)
		{
			foreach (StatesInstance item in chain)
			{
				item.Trigger(-1009905786, changedLink);
			}
		}

		private void CollectToChain(ref HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain, ref bool foundHead, StatesInstance ignoredLink = null)
		{
			if ((ignoredLink != null && ignoredLink == this) || chain.Contains(this))
			{
				return;
			}
			chain.Add(this);
			if (HasTag(base.def.headBuildingTag))
			{
				foundHead = true;
			}
			foreach (int neighbourCheckCell in neighbourCheckCells)
			{
				CollectNeighbourToChain(neighbourCheckCell, ref chain, ref foundHead);
			}
		}

		private void CollectNeighbourToChain(int cell, ref HashSetPool<StatesInstance, StatesInstance>.PooledHashSet chain, ref bool foundHead, StatesInstance ignoredLink = null)
		{
			GameObject gameObject = Grid.Objects[cell, (int)base.def.objectLayer];
			if (!(gameObject == null) && (gameObject.HasTag(base.def.linkBuildingTag) || gameObject.HasTag(base.def.headBuildingTag)))
			{
				gameObject.GetSMI<StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
			}
		}
	}

	private State unlinked;

	private State linked;

	private State DEBUG_relink;

	private BoolParameter isConnectedToHead = new BoolParameter();

	private Signal doRelink;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = unlinked;
		StatusItem statusItem = new StatusItem("NotLinkedToHeadStatusItem", BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.NAME, BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.TOOLTIP, "status_item_not_linked", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		statusItem.resolveTooltipCallback = delegate(string tooltip, object obj)
		{
			StatesInstance statesInstance = (StatesInstance)obj;
			return tooltip.Replace("{headBuilding}", Strings.Get("STRINGS.BUILDINGS.PREFABS." + statesInstance.def.headBuildingTag.Name.ToUpper() + ".NAME")).Replace("{linkBuilding}", Strings.Get("STRINGS.BUILDINGS.PREFABS." + statesInstance.def.linkBuildingTag.Name.ToUpper() + ".NAME"));
		};
		root.OnSignal(doRelink, DEBUG_relink);
		unlinked.ParamTransition(isConnectedToHead, linked, GameStateMachine<ChainedBuilding, StatesInstance, IStateMachineTarget, Def>.IsTrue).ToggleStatusItem(statusItem, (StatesInstance smi) => smi);
		linked.ParamTransition(isConnectedToHead, unlinked, GameStateMachine<ChainedBuilding, StatesInstance, IStateMachineTarget, Def>.IsFalse);
		DEBUG_relink.Enter(delegate(StatesInstance smi)
		{
			smi.DEBUG_Relink();
		});
	}
}
