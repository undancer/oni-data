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

	public enum Direction
	{
		Left = -1,
		None,
		Right
	}

	public class StatesInstance : GameInstance
	{
		private int widthInCells;

		private Dictionary<Direction, StatesInstance> neighbours;

		private Direction headDirection = Direction.None;

		public StatesInstance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Building component = master.GetComponent<Building>();
			BuildingDef def2 = component.Def;
			widthInCells = def2.WidthInCells;
			neighbours = new Dictionary<Direction, StatesInstance>();
			neighbours[Direction.Left] = null;
			neighbours[Direction.Right] = null;
		}

		public override void StartSM()
		{
			base.StartSM();
			SetLinks();
		}

		protected override void OnCleanUp()
		{
			ClearLinks();
			base.OnCleanUp();
		}

		public IEnumerable<GameObject> GetLinkedBuildings()
		{
			if (neighbours[Direction.Left] != null)
			{
				foreach (GameObject linkedBuilding in neighbours[Direction.Left].GetLinkedBuildings(Direction.Left))
				{
					yield return linkedBuilding;
				}
			}
			if (neighbours[Direction.Right] != null)
			{
				foreach (GameObject linkedBuilding2 in neighbours[Direction.Right].GetLinkedBuildings(Direction.Right))
				{
					yield return linkedBuilding2;
				}
			}
			yield return base.gameObject;
		}

		public IEnumerable<GameObject> GetLinkedBuildings(Direction direction)
		{
			if (neighbours[direction] != null)
			{
				foreach (GameObject linkedBuilding in neighbours[direction].GetLinkedBuildings(direction))
				{
					yield return linkedBuilding;
				}
			}
			yield return base.gameObject;
		}

		public void SetLinks()
		{
			Direction[] allDirections = ChainedBuilding.allDirections;
			foreach (Direction direction in allDirections)
			{
				neighbours[direction] = FindNeighbour(direction);
				if (neighbours[direction] != null)
				{
					neighbours[direction].neighbours[Opposite(direction)] = this;
				}
			}
			if (base.gameObject.HasTag(base.def.headBuildingTag))
			{
				Direction[] allDirections2 = ChainedBuilding.allDirections;
				foreach (Direction direction2 in allDirections2)
				{
					PropagateSetHead(direction2, this);
				}
				headDirection = Direction.None;
			}
			else
			{
				Direction[] allDirections3 = ChainedBuilding.allDirections;
				foreach (Direction key in allDirections3)
				{
					StatesInstance statesInstance = CheckNeighbourForHead(neighbours[key]);
					if (statesInstance != null)
					{
						PropagateSetHead(key, statesInstance);
						break;
					}
				}
			}
			base.sm.cachedHeadBuilding.Get(this)?.Trigger(-1009905786);
		}

		private void PropagateSetHead(Direction headDirection, StatesInstance newHead)
		{
			if (base.sm.cachedHeadBuilding.Get(this) == null || base.sm.cachedHeadBuilding.Get(this) == this)
			{
				StatesInstance statesInstance = base.sm.cachedHeadBuilding.Get(this);
				this.headDirection = headDirection;
				base.sm.cachedHeadBuilding.Set(newHead, this);
				neighbours[Opposite(headDirection)]?.PropagateSetHead(headDirection, newHead);
			}
		}

		public void ClearLinks()
		{
			StatesInstance statesInstance = base.sm.cachedHeadBuilding.Get(this);
			PropagateClearHead(statesInstance);
			Direction[] allDirections = ChainedBuilding.allDirections;
			foreach (Direction direction in allDirections)
			{
				if (neighbours[direction] != null)
				{
					neighbours[direction].neighbours[Opposite(direction)] = null;
				}
			}
			statesInstance?.Trigger(-1009905786);
		}

		private void PropagateClearHead(StatesInstance expectedHead)
		{
			if (expectedHead == base.sm.cachedHeadBuilding.Get(this))
			{
				neighbours[Opposite(headDirection)]?.PropagateClearHead(expectedHead);
				headDirection = Direction.None;
				base.sm.cachedHeadBuilding.Set(null, this);
			}
		}

		private StatesInstance CheckNeighbourForHead(StatesInstance neighbour)
		{
			return neighbour?.sm.cachedHeadBuilding.Get(neighbour);
		}

		private StatesInstance FindNeighbour(Direction direction)
		{
			int cell = Grid.PosToCell(this);
			CellOffset offset = ((direction == Direction.Left) ? new CellOffset(-(widthInCells - 1) / 2 - 1, 0) : new CellOffset(widthInCells / 2 + 1, 0));
			GameObject gameObject = Grid.Objects[Grid.OffsetCell(cell, offset), (int)base.def.objectLayer];
			if (gameObject == null)
			{
				return null;
			}
			if (!gameObject.HasAnyTags(new Tag[2]
			{
				base.def.linkBuildingTag,
				base.def.headBuildingTag
			}))
			{
				return null;
			}
			return gameObject.GetSMI<StatesInstance>();
		}
	}

	public static Direction[] allDirections = new Direction[2]
	{
		Direction.Left,
		Direction.Right
	};

	private ObjectParameter<StatesInstance> cachedHeadBuilding;

	private State unlinked;

	private State linked;

	public static Direction Opposite(Direction direction)
	{
		return (direction == Direction.Left) ? Direction.Right : Direction.Left;
	}

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = unlinked;
		StatusItem statusItem = new StatusItem("NotLinkedToHeadStatusItem", BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.NAME, BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.TOOLTIP, "status_item_not_linked", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		statusItem.resolveTooltipCallback = delegate(string tooltip, object obj)
		{
			StatesInstance statesInstance = (StatesInstance)obj;
			return tooltip.Replace("{headBuilding}", Strings.Get("STRINGS.BUILDINGS.PREFABS." + statesInstance.def.headBuildingTag.Name.ToUpper() + ".NAME")).Replace("{linkBuilding}", Strings.Get("STRINGS.BUILDINGS.PREFABS." + statesInstance.def.linkBuildingTag.Name.ToUpper() + ".NAME"));
		};
		unlinked.ParamTransition(cachedHeadBuilding, linked, (StatesInstance smi, StatesInstance p) => p != null).ToggleStatusItem(statusItem, (StatesInstance smi) => smi).TriggerOnEnter(GameHashes.ChainedNetworkChanged);
		linked.ParamTransition(cachedHeadBuilding, unlinked, (StatesInstance smi, StatesInstance p) => p == null).TriggerOnEnter(GameHashes.ChainedNetworkChanged);
	}
}
