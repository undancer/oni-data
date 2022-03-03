using System;
using System.Collections.Generic;
using System.Diagnostics;

public class PathFinder
{
	public struct Cell
	{
		public int queryId;

		public int cost;

		public int parent;

		public byte underwaterCost;

		public NavType navType;

		public NavType parentNavType;

		public int transitionId;
	}

	public struct PotentialPath
	{
		[Flags]
		public enum Flags : byte
		{
			None = 0x0,
			HasAtmoSuit = 0x1,
			HasJetPack = 0x2,
			HasOxygenMask = 0x4,
			PerformSuitChecks = 0x8
		}

		public int cell;

		public NavType navType;

		public Flags flags { get; private set; }

		public PotentialPath(int cell, NavType nav_type, Flags flags)
		{
			this.cell = cell;
			navType = nav_type;
			this.flags = flags;
		}

		public void SetFlags(Flags new_flags)
		{
			flags |= new_flags;
		}

		public void ClearFlags(Flags new_flags)
		{
			flags &= (Flags)(byte)(~(int)new_flags);
		}

		public bool HasFlag(Flags flag)
		{
			return HasAnyFlag(flag);
		}

		public bool HasAnyFlag(Flags mask)
		{
			return (flags & mask) != 0;
		}
	}

	public struct Path
	{
		public struct Node
		{
			public int cell;

			public NavType navType;

			public int transitionId;
		}

		public int cost;

		public List<Node> nodes;

		public void AddNode(Node node)
		{
			if (nodes == null)
			{
				nodes = new List<Node>();
			}
			nodes.Add(node);
		}

		public bool IsValid()
		{
			if (nodes != null)
			{
				return nodes.Count > 1;
			}
			return false;
		}

		public bool HasArrived()
		{
			if (nodes != null)
			{
				return nodes.Count > 0;
			}
			return false;
		}

		public void Clear()
		{
			cost = 0;
			if (nodes != null)
			{
				nodes.Clear();
			}
		}
	}

	public class PotentialList
	{
		public class PriorityQueue<TValue>
		{
			private List<KeyValuePair<int, TValue>> _baseHeap;

			public int Count => _baseHeap.Count;

			public PriorityQueue()
			{
				_baseHeap = new List<KeyValuePair<int, TValue>>();
			}

			public void Enqueue(int priority, TValue value)
			{
				Insert(priority, value);
			}

			public KeyValuePair<int, TValue> Dequeue()
			{
				KeyValuePair<int, TValue> result = _baseHeap[0];
				DeleteRoot();
				return result;
			}

			public KeyValuePair<int, TValue> Peek()
			{
				if (Count > 0)
				{
					return _baseHeap[0];
				}
				throw new InvalidOperationException("Priority queue is empty");
			}

			private void ExchangeElements(int pos1, int pos2)
			{
				KeyValuePair<int, TValue> value = _baseHeap[pos1];
				_baseHeap[pos1] = _baseHeap[pos2];
				_baseHeap[pos2] = value;
			}

			private void Insert(int priority, TValue value)
			{
				KeyValuePair<int, TValue> item = new KeyValuePair<int, TValue>(priority, value);
				_baseHeap.Add(item);
				HeapifyFromEndToBeginning(_baseHeap.Count - 1);
			}

			private int HeapifyFromEndToBeginning(int pos)
			{
				if (pos >= _baseHeap.Count)
				{
					return -1;
				}
				while (pos > 0)
				{
					int num = (pos - 1) / 2;
					if (_baseHeap[num].Key - _baseHeap[pos].Key <= 0)
					{
						break;
					}
					ExchangeElements(num, pos);
					pos = num;
				}
				return pos;
			}

			private void DeleteRoot()
			{
				if (_baseHeap.Count <= 1)
				{
					_baseHeap.Clear();
					return;
				}
				_baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
				_baseHeap.RemoveAt(_baseHeap.Count - 1);
				HeapifyFromBeginningToEnd(0);
			}

			private void HeapifyFromBeginningToEnd(int pos)
			{
				int count = _baseHeap.Count;
				if (pos >= count)
				{
					return;
				}
				while (true)
				{
					int num = pos;
					int num2 = 2 * pos + 1;
					int num3 = 2 * pos + 2;
					if (num2 < count && _baseHeap[num].Key - _baseHeap[num2].Key > 0)
					{
						num = num2;
					}
					if (num3 < count && _baseHeap[num].Key - _baseHeap[num3].Key > 0)
					{
						num = num3;
					}
					if (num != pos)
					{
						ExchangeElements(num, pos);
						pos = num;
						continue;
					}
					break;
				}
			}

			public void Clear()
			{
				_baseHeap.Clear();
			}
		}

		private class HOTQueue<TValue>
		{
			private PriorityQueue<TValue> hotQueue = new PriorityQueue<TValue>();

			private PriorityQueue<TValue> coldQueue = new PriorityQueue<TValue>();

			private int hotThreshold = int.MinValue;

			private int coldThreshold = int.MinValue;

			private int count;

			public int Count => count;

			public KeyValuePair<int, TValue> Dequeue()
			{
				if (hotQueue.Count == 0)
				{
					PriorityQueue<TValue> priorityQueue = hotQueue;
					hotQueue = coldQueue;
					coldQueue = priorityQueue;
					hotThreshold = coldThreshold;
				}
				count--;
				return hotQueue.Dequeue();
			}

			public void Enqueue(int priority, TValue value)
			{
				if (priority <= hotThreshold)
				{
					hotQueue.Enqueue(priority, value);
				}
				else
				{
					coldQueue.Enqueue(priority, value);
					coldThreshold = Math.Max(coldThreshold, priority);
				}
				count++;
			}

			public KeyValuePair<int, TValue> Peek()
			{
				if (hotQueue.Count == 0)
				{
					PriorityQueue<TValue> priorityQueue = hotQueue;
					hotQueue = coldQueue;
					coldQueue = priorityQueue;
					hotThreshold = coldThreshold;
				}
				return hotQueue.Peek();
			}

			public void Clear()
			{
				count = 0;
				hotThreshold = int.MinValue;
				hotQueue.Clear();
				coldThreshold = int.MinValue;
				coldQueue.Clear();
			}
		}

		private HOTQueue<PotentialPath> queue = new HOTQueue<PotentialPath>();

		public int Count => queue.Count;

		public KeyValuePair<int, PotentialPath> Next()
		{
			return queue.Dequeue();
		}

		public void Add(int cost, PotentialPath path)
		{
			queue.Enqueue(cost, path);
		}

		public void Clear()
		{
			queue.Clear();
		}
	}

	private class Temp
	{
		public static PotentialList Potentials = new PotentialList();
	}

	public class PotentialScratchPad
	{
		public struct PathGridCellData
		{
			public Cell pathGridCell;

			public NavGrid.Link link;

			public bool isSubmerged;
		}

		public NavGrid.Link[] linksWithCorrectNavType;

		public PathGridCellData[] linksInCellRange;

		public PotentialScratchPad(int max_links_per_cell)
		{
			linksWithCorrectNavType = new NavGrid.Link[max_links_per_cell];
			linksInCellRange = new PathGridCellData[max_links_per_cell];
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static PathGrid PathGrid;

	private static readonly Func<int, bool> allowPathfindingFloodFillCb = delegate(int cell)
	{
		if (Grid.Solid[cell])
		{
			return false;
		}
		if (Grid.AllowPathfinding[cell])
		{
			return false;
		}
		Grid.AllowPathfinding[cell] = true;
		return true;
	};

	public static void Initialize()
	{
		NavType[] array = new NavType[11];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (NavType)i;
		}
		PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, apply_offset: false, array);
		for (int j = 0; j < Grid.CellCount; j++)
		{
			if (Grid.Visible[j] > 0 || Grid.Spawnable[j] > 0)
			{
				ListPool<int, PathFinder>.PooledList pooledList = ListPool<int, PathFinder>.Allocate();
				GameUtil.FloodFillConditional(j, allowPathfindingFloodFillCb, pooledList);
				Grid.AllowPathfinding[j] = true;
				pooledList.Recycle();
			}
		}
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(OnReveal));
	}

	private static void OnReveal(int cell)
	{
	}

	public static void UpdatePath(NavGrid nav_grid, PathFinderAbilities abilities, PotentialPath potential_path, PathFinderQuery query, ref Path path)
	{
		Run(nav_grid, abilities, potential_path, query, ref path);
	}

	public static bool ValidatePath(NavGrid nav_grid, PathFinderAbilities abilities, ref Path path)
	{
		if (!path.IsValid())
		{
			return false;
		}
		for (int i = 0; i < path.nodes.Count; i++)
		{
			Path.Node node = path.nodes[i];
			if (i >= path.nodes.Count - 1)
			{
				continue;
			}
			Path.Node node2 = path.nodes[i + 1];
			int num = node.cell * nav_grid.maxLinksPerCell;
			bool flag = false;
			NavGrid.Link link = nav_grid.Links[num];
			while (link.link != InvalidHandle)
			{
				if (link.link == node2.cell && node2.navType == link.endNavType && node.navType == link.startNavType)
				{
					PotentialPath path2 = new PotentialPath(node.cell, node.navType, PotentialPath.Flags.None);
					flag = abilities.TraversePath(ref path2, node.cell, node.navType, 0, link.transitionId, 0);
					if (flag)
					{
						break;
					}
				}
				num++;
				link = nav_grid.Links[num];
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PotentialPath potential_path, PathFinderQuery query)
	{
		int result_cell = InvalidCell;
		NavType result_nav_type = NavType.NumNavTypes;
		query.ClearResult();
		if (Grid.IsValidCell(potential_path.cell))
		{
			FindPaths(nav_grid, ref abilities, potential_path, query, Temp.Potentials, ref result_cell, ref result_nav_type);
			if (result_cell != InvalidCell)
			{
				bool is_cell_in_range = false;
				query.SetResult(cost: PathGrid.GetCell(result_cell, result_nav_type, out is_cell_in_range).cost, cell: result_cell, nav_type: result_nav_type);
			}
		}
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PotentialPath potential_path, PathFinderQuery query, ref Path path)
	{
		Run(nav_grid, abilities, potential_path, query);
		if (query.GetResultCell() != InvalidCell)
		{
			BuildResultPath(query.GetResultCell(), query.GetResultNavType(), ref path);
		}
		else
		{
			path.Clear();
		}
	}

	private static void BuildResultPath(int path_cell, NavType path_nav_type, ref Path path)
	{
		if (path_cell == InvalidCell)
		{
			return;
		}
		bool is_cell_in_range = false;
		Cell cell = PathGrid.GetCell(path_cell, path_nav_type, out is_cell_in_range);
		path.Clear();
		path.cost = cell.cost;
		while (path_cell != InvalidCell)
		{
			path.AddNode(new Path.Node
			{
				cell = path_cell,
				navType = cell.navType,
				transitionId = cell.transitionId
			});
			path_cell = cell.parent;
			if (path_cell != InvalidCell)
			{
				cell = PathGrid.GetCell(path_cell, cell.parentNavType, out is_cell_in_range);
			}
		}
		if (path.nodes != null)
		{
			for (int i = 0; i < path.nodes.Count / 2; i++)
			{
				Path.Node value = path.nodes[i];
				path.nodes[i] = path.nodes[path.nodes.Count - i - 1];
				path.nodes[path.nodes.Count - i - 1] = value;
			}
		}
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PotentialPath potential_path, PathFinderQuery query, PotentialList potentials, ref int result_cell, ref NavType result_nav_type)
	{
		potentials.Clear();
		PathGrid.ResetUpdate();
		PathGrid.BeginUpdate(potential_path.cell, isContinuation: false);
		Cell cell_data = PathGrid.GetCell(potential_path, out var is_cell_in_range);
		AddPotential(potential_path, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, potentials, PathGrid, ref cell_data);
		int num = int.MaxValue;
		while (potentials.Count > 0)
		{
			KeyValuePair<int, PotentialPath> keyValuePair = potentials.Next();
			cell_data = PathGrid.GetCell(keyValuePair.Value, out is_cell_in_range);
			if (cell_data.cost == keyValuePair.Key)
			{
				if (cell_data.navType != NavType.Tube && query.IsMatch(keyValuePair.Value.cell, cell_data.parent, cell_data.cost) && cell_data.cost < num)
				{
					result_cell = keyValuePair.Value.cell;
					num = cell_data.cost;
					result_nav_type = cell_data.navType;
					break;
				}
				AddPotentials(nav_grid.potentialScratchPad, keyValuePair.Value, cell_data.cost, cell_data.underwaterCost, ref abilities, query, nav_grid.maxLinksPerCell, nav_grid.Links, potentials, PathGrid, cell_data.parent, cell_data.parentNavType);
			}
		}
		PathGrid.EndUpdate(isComplete: true);
	}

	public static void AddPotential(PotentialPath potential_path, int parent_cell, NavType parent_nav_type, int cost, int underwater_cost, int transition_id, PotentialList potentials, PathGrid path_grid, ref Cell cell_data)
	{
		cell_data.cost = cost;
		cell_data.underwaterCost = (byte)Math.Min(underwater_cost, 255);
		cell_data.parent = parent_cell;
		cell_data.navType = potential_path.navType;
		cell_data.parentNavType = parent_nav_type;
		cell_data.transitionId = transition_id;
		potentials.Add(cost, potential_path);
		path_grid.SetCell(potential_path, ref cell_data);
	}

	[Conditional("ENABLE_PATH_DETAILS")]
	private static void BeginDetailSample(string region_name)
	{
	}

	[Conditional("ENABLE_PATH_DETAILS")]
	private static void EndDetailSample(string region_name)
	{
	}

	public static bool IsSubmerged(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		if (Grid.IsValidCell(num) && Grid.Element[num].IsLiquid)
		{
			return true;
		}
		if (Grid.Element[cell].IsLiquid && Grid.IsValidCell(num) && Grid.Element[num].IsSolid)
		{
			return true;
		}
		return false;
	}

	public static void AddPotentials(PotentialScratchPad potential_scratch_pad, PotentialPath potential, int cost, int underwater_cost, ref PathFinderAbilities abilities, PathFinderQuery query, int max_links_per_cell, NavGrid.Link[] links, PotentialList potentials, PathGrid path_grid, int parent_cell, NavType parent_nav_type)
	{
		if (!Grid.IsValidCell(potential.cell))
		{
			return;
		}
		int num = 0;
		NavGrid.Link[] linksWithCorrectNavType = potential_scratch_pad.linksWithCorrectNavType;
		int num2 = potential.cell * max_links_per_cell;
		NavGrid.Link link = links[num2];
		for (int link2 = link.link; link2 != InvalidHandle; link2 = link.link)
		{
			if (link.startNavType == potential.navType && (parent_cell != link2 || parent_nav_type != link.startNavType))
			{
				linksWithCorrectNavType[num++] = link;
			}
			num2++;
			link = links[num2];
		}
		int num3 = 0;
		PotentialScratchPad.PathGridCellData[] linksInCellRange = potential_scratch_pad.linksInCellRange;
		for (int i = 0; i < num; i++)
		{
			NavGrid.Link link3 = linksWithCorrectNavType[i];
			int link4 = link3.link;
			bool is_cell_in_range = false;
			Cell cell = path_grid.GetCell(link4, link3.endNavType, out is_cell_in_range);
			if (is_cell_in_range)
			{
				int num4 = cost + link3.cost;
				bool num5 = cell.cost == -1;
				bool flag = num4 < cell.cost;
				if (num5 || flag)
				{
					linksInCellRange[num3++] = new PotentialScratchPad.PathGridCellData
					{
						pathGridCell = cell,
						link = link3
					};
				}
			}
		}
		for (int j = 0; j < num3; j++)
		{
			PotentialScratchPad.PathGridCellData pathGridCellData = linksInCellRange[j];
			int link5 = pathGridCellData.link.link;
			pathGridCellData.isSubmerged = IsSubmerged(link5);
			linksInCellRange[j] = pathGridCellData;
		}
		for (int k = 0; k < num3; k++)
		{
			PotentialScratchPad.PathGridCellData pathGridCellData2 = linksInCellRange[k];
			NavGrid.Link link6 = pathGridCellData2.link;
			int link7 = link6.link;
			Cell cell_data = pathGridCellData2.pathGridCell;
			int num6 = cost + link6.cost;
			PotentialPath path = potential;
			path.cell = link7;
			path.navType = link6.endNavType;
			int num7 = underwater_cost;
			if (pathGridCellData2.isSubmerged)
			{
				num7 = underwater_cost + 1;
				int submergedPathCostPenalty = abilities.GetSubmergedPathCostPenalty(path, link6);
				num6 += submergedPathCostPenalty;
			}
			else
			{
				num7 = 0;
			}
			PotentialPath.Flags flags = path.flags;
			bool num8 = abilities.TraversePath(ref path, potential.cell, potential.navType, num6, link6.transitionId, num7);
			_ = path.flags;
			if (num8)
			{
				AddPotential(path, potential.cell, potential.navType, num6, num7, link6.transitionId, potentials, path_grid, ref cell_data);
			}
		}
	}
}
