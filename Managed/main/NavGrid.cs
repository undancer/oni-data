using System;
using System.Collections.Generic;
using HUSL;
using UnityEngine;

public class NavGrid
{
	public struct Link
	{
		public int link;

		public NavType startNavType;

		public NavType endNavType;

		private byte _transitionId;

		private byte _cost;

		public byte transitionId
		{
			get
			{
				return _transitionId;
			}
			set
			{
				_transitionId = value;
			}
		}

		public byte cost
		{
			get
			{
				return _cost;
			}
			set
			{
				_cost = value;
			}
		}

		public Link(int link, NavType start_nav_type, NavType end_nav_type, byte transition_id, byte cost)
		{
			_transitionId = 0;
			_cost = 0;
			this.link = link;
			startNavType = start_nav_type;
			endNavType = end_nav_type;
			transitionId = transition_id;
			this.cost = cost;
		}
	}

	public struct NavTypeData
	{
		public NavType navType;

		public Vector2 animControllerOffset;

		public bool flipX;

		public bool flipY;

		public float rotation;

		public HashedString idleAnim;
	}

	public struct Transition
	{
		public NavType start;

		public NavType end;

		public NavAxis startAxis;

		public int x;

		public int y;

		public byte id;

		public byte cost;

		public bool isLooping;

		public bool isEscape;

		public string preAnim;

		public string anim;

		public float animSpeed;

		public CellOffset[] voidOffsets;

		public CellOffset[] solidOffsets;

		public NavOffset[] validNavOffsets;

		public NavOffset[] invalidNavOffsets;

		public bool isCritter;

		public override string ToString()
		{
			return $"{id}: {start}->{end} ({startAxis}); offset {x},{y}";
		}

		public Transition(NavType start, NavType end, int x, int y, NavAxis start_axis, bool is_looping, bool loop_has_pre, bool is_escape, int cost, string anim, CellOffset[] void_offsets, CellOffset[] solid_offsets, NavOffset[] valid_nav_offsets, NavOffset[] invalid_nav_offsets, bool critter = false, float animSpeed = 1f)
		{
			DebugUtil.Assert(cost <= 255 && cost >= 0);
			id = byte.MaxValue;
			this.start = start;
			this.end = end;
			this.x = x;
			this.y = y;
			startAxis = start_axis;
			isLooping = is_looping;
			isEscape = is_escape;
			this.anim = anim;
			preAnim = "";
			this.cost = (byte)cost;
			if (string.IsNullOrEmpty(this.anim))
			{
				this.anim = start.ToString().ToLower() + "_" + end.ToString().ToLower() + "_" + x + "_" + y;
			}
			if (isLooping)
			{
				if (loop_has_pre)
				{
					preAnim = this.anim + "_pre";
				}
				this.anim += "_loop";
			}
			if (startAxis != 0)
			{
				this.anim += ((startAxis == NavAxis.X) ? "_x" : "_y");
			}
			voidOffsets = void_offsets;
			solidOffsets = solid_offsets;
			validNavOffsets = valid_nav_offsets;
			invalidNavOffsets = invalid_nav_offsets;
			isCritter = critter;
			this.animSpeed = animSpeed;
		}

		public int IsValid(int cell, NavTable nav_table)
		{
			if (!Grid.IsCellOffsetValid(cell, x, y))
			{
				return Grid.InvalidCell;
			}
			int num = Grid.OffsetCell(cell, x, y);
			if (!nav_table.IsValid(num, end))
			{
				return Grid.InvalidCell;
			}
			Grid.BuildFlags buildFlags = Grid.BuildFlags.Solid | Grid.BuildFlags.DupeImpassable;
			if (isCritter)
			{
				buildFlags |= Grid.BuildFlags.CritterImpassable;
			}
			CellOffset[] array = voidOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				CellOffset cellOffset = array[i];
				int num2 = Grid.OffsetCell(cell, cellOffset.x, cellOffset.y);
				if (Grid.IsValidCell(num2) && (Grid.BuildMasks[num2] & buildFlags) != 0)
				{
					if (isCritter)
					{
						return Grid.InvalidCell;
					}
					if ((Grid.BuildMasks[num2] & Grid.BuildFlags.DupePassable) == 0)
					{
						return Grid.InvalidCell;
					}
				}
			}
			array = solidOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				CellOffset cellOffset2 = array[i];
				int num3 = Grid.OffsetCell(cell, cellOffset2.x, cellOffset2.y);
				if (Grid.IsValidCell(num3) && !Grid.Solid[num3])
				{
					return Grid.InvalidCell;
				}
			}
			NavOffset[] array2 = validNavOffsets;
			for (int i = 0; i < array2.Length; i++)
			{
				NavOffset navOffset = array2[i];
				int cell2 = Grid.OffsetCell(cell, navOffset.offset.x, navOffset.offset.y);
				if (!nav_table.IsValid(cell2, navOffset.navType))
				{
					return Grid.InvalidCell;
				}
			}
			array2 = invalidNavOffsets;
			for (int i = 0; i < array2.Length; i++)
			{
				NavOffset navOffset2 = array2[i];
				int cell3 = Grid.OffsetCell(cell, navOffset2.offset.x, navOffset2.offset.y);
				if (nav_table.IsValid(cell3, navOffset2.navType))
				{
					return Grid.InvalidCell;
				}
			}
			if (start == NavType.Tube)
			{
				if (end == NavType.Tube)
				{
					GameObject gameObject = Grid.Objects[cell, 9];
					GameObject gameObject2 = Grid.Objects[num, 9];
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = (gameObject ? gameObject.GetComponent<TravelTubeUtilityNetworkLink>() : null);
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink2 = (gameObject2 ? gameObject2.GetComponent<TravelTubeUtilityNetworkLink>() : null);
					if ((bool)travelTubeUtilityNetworkLink)
					{
						travelTubeUtilityNetworkLink.GetCells(out var linked_cell, out var linked_cell2);
						if (num != linked_cell && num != linked_cell2)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, num);
						if (utilityConnections == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						if (Game.Instance.travelTubeSystem.GetConnections(num, is_physical_building: false) != utilityConnections)
						{
							return Grid.InvalidCell;
						}
					}
					else if ((bool)travelTubeUtilityNetworkLink2)
					{
						travelTubeUtilityNetworkLink2.GetCells(out var linked_cell3, out var linked_cell4);
						if (cell != linked_cell3 && cell != linked_cell4)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections2 = UtilityConnectionsExtensions.DirectionFromToCell(num, cell);
						if (utilityConnections2 == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						if (Game.Instance.travelTubeSystem.GetConnections(cell, is_physical_building: false) != utilityConnections2)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						bool flag = startAxis == NavAxis.X;
						int cell4 = cell;
						for (int j = 0; j < 2; j++)
						{
							if ((flag && j == 0) || (!flag && j == 1))
							{
								int num4 = ((x > 0) ? 1 : (-1));
								for (int k = 0; k < Mathf.Abs(x); k++)
								{
									UtilityConnections connections = Game.Instance.travelTubeSystem.GetConnections(cell4, is_physical_building: false);
									if (num4 > 0 && (connections & UtilityConnections.Right) == 0)
									{
										return Grid.InvalidCell;
									}
									if (num4 < 0 && (connections & UtilityConnections.Left) == 0)
									{
										return Grid.InvalidCell;
									}
									cell4 = Grid.OffsetCell(cell4, num4, 0);
								}
								continue;
							}
							int num5 = ((y > 0) ? 1 : (-1));
							for (int l = 0; l < Mathf.Abs(y); l++)
							{
								UtilityConnections connections2 = Game.Instance.travelTubeSystem.GetConnections(cell4, is_physical_building: false);
								if (num5 > 0 && (connections2 & UtilityConnections.Up) == 0)
								{
									return Grid.InvalidCell;
								}
								if (num5 < 0 && (connections2 & UtilityConnections.Down) == 0)
								{
									return Grid.InvalidCell;
								}
								cell4 = Grid.OffsetCell(cell4, 0, num5);
							}
						}
					}
				}
				else
				{
					UtilityConnections connections3 = Game.Instance.travelTubeSystem.GetConnections(cell, is_physical_building: false);
					if (y > 0)
					{
						if (connections3 != UtilityConnections.Down)
						{
							return Grid.InvalidCell;
						}
					}
					else if (x > 0)
					{
						if (connections3 != UtilityConnections.Left)
						{
							return Grid.InvalidCell;
						}
					}
					else if (x < 0)
					{
						if (connections3 != UtilityConnections.Right)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						if (y >= 0)
						{
							return Grid.InvalidCell;
						}
						if (connections3 != UtilityConnections.Up)
						{
							return Grid.InvalidCell;
						}
					}
				}
			}
			else if (start == NavType.Floor && end == NavType.Tube)
			{
				int cell5 = Grid.OffsetCell(cell, x, y);
				if (Game.Instance.travelTubeSystem.GetConnections(cell5, is_physical_building: false) != UtilityConnections.Up)
				{
					return Grid.InvalidCell;
				}
			}
			return num;
		}
	}

	public bool DebugViewAllPaths;

	public bool DebugViewValidCells;

	public bool[] DebugViewValidCellsType;

	public bool DebugViewValidCellsAll;

	public bool DebugViewLinks;

	public bool[] DebugViewLinkType;

	public bool DebugViewLinksAll;

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public Dictionary<int, int> teleportTransitions = new Dictionary<int, int>();

	public Link[] Links;

	private HashSet<int> DirtyCells = new HashSet<int>();

	private HashSet<int> ExpandedDirtyCells = new HashSet<int>();

	private NavTableValidator[] Validators = new NavTableValidator[0];

	private CellOffset[] boundingOffsets;

	public string id;

	public bool updateEveryFrame;

	public PathFinder.PotentialScratchPad potentialScratchPad;

	public Action<HashSet<int>> OnNavGridUpdateComplete;

	public NavType[] ValidNavTypes;

	public NavTypeData[] navTypeData;

	private Color[] debugColorLookup;

	public NavTable NavTable
	{
		get;
		private set;
	}

	public NavGraph NavGraph
	{
		get;
		private set;
	}

	public Transition[] transitions
	{
		get;
		set;
	}

	public Transition[][] transitionsByNavType
	{
		get;
		private set;
	}

	public int updateRangeX
	{
		get;
		private set;
	}

	public int updateRangeY
	{
		get;
		private set;
	}

	public int maxLinksPerCell
	{
		get;
		private set;
	}

	public static NavType MirrorNavType(NavType nav_type)
	{
		return nav_type switch
		{
			NavType.LeftWall => NavType.RightWall, 
			NavType.RightWall => NavType.LeftWall, 
			_ => nav_type, 
		};
	}

	public NavGrid(string id, Transition[] transitions, NavTypeData[] nav_type_data, CellOffset[] bounding_offsets, NavTableValidator[] validators, int update_range_x, int update_range_y, int max_links_per_cell)
	{
		this.id = id;
		Validators = validators;
		navTypeData = nav_type_data;
		this.transitions = transitions;
		boundingOffsets = bounding_offsets;
		List<NavType> list = new List<NavType>();
		updateRangeX = update_range_x;
		updateRangeY = update_range_y;
		maxLinksPerCell = max_links_per_cell + 1;
		for (int i = 0; i < transitions.Length; i++)
		{
			DebugUtil.Assert(i >= 0 && i <= 255);
			transitions[i].id = (byte)i;
			if (!list.Contains(transitions[i].start))
			{
				list.Add(transitions[i].start);
			}
			if (!list.Contains(transitions[i].end))
			{
				list.Add(transitions[i].end);
			}
		}
		ValidNavTypes = list.ToArray();
		DebugViewLinkType = new bool[ValidNavTypes.Length];
		DebugViewValidCellsType = new bool[ValidNavTypes.Length];
		NavType[] validNavTypes = ValidNavTypes;
		foreach (NavType nav_type in validNavTypes)
		{
			GetNavTypeData(nav_type);
		}
		Links = new Link[maxLinksPerCell * Grid.CellCount];
		NavTable = new NavTable(Grid.CellCount);
		this.transitions = transitions;
		transitionsByNavType = new Transition[11][];
		for (int k = 0; k < 11; k++)
		{
			List<Transition> list2 = new List<Transition>();
			NavType navType = (NavType)k;
			for (int j = 0; j < transitions.Length; j++)
			{
				Transition item = transitions[j];
				if (item.start == navType)
				{
					list2.Add(item);
				}
			}
			transitionsByNavType[k] = list2.ToArray();
		}
		foreach (NavTableValidator obj in validators)
		{
			obj.onDirty = (Action<int>)Delegate.Combine(obj.onDirty, new Action<int>(AddDirtyCell));
		}
		potentialScratchPad = new PathFinder.PotentialScratchPad(maxLinksPerCell);
		InitializeGraph();
		NavGraph = new NavGraph(Grid.CellCount, this);
	}

	public NavTypeData GetNavTypeData(NavType nav_type)
	{
		NavTypeData[] array = navTypeData;
		for (int i = 0; i < array.Length; i++)
		{
			NavTypeData result = array[i];
			if (result.navType == nav_type)
			{
				return result;
			}
		}
		throw new Exception("Missing nav type data for nav type:" + nav_type);
	}

	public bool HasNavTypeData(NavType nav_type)
	{
		NavTypeData[] array = navTypeData;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].navType == nav_type)
			{
				return true;
			}
		}
		return false;
	}

	public HashedString GetIdleAnim(NavType nav_type)
	{
		return GetNavTypeData(nav_type).idleAnim;
	}

	public void InitializeGraph()
	{
		NavGridUpdater.InitializeNavGrid(NavTable, ValidNavTypes, Validators, boundingOffsets, maxLinksPerCell, Links, transitionsByNavType);
	}

	public void UpdateGraph()
	{
		foreach (int dirtyCell in DirtyCells)
		{
			for (int i = -updateRangeY; i <= updateRangeY; i++)
			{
				for (int j = -updateRangeX; j <= updateRangeX; j++)
				{
					int num = Grid.OffsetCell(dirtyCell, j, i);
					if (Grid.IsValidCell(num))
					{
						ExpandedDirtyCells.Add(num);
					}
				}
			}
		}
		UpdateGraph(ExpandedDirtyCells);
		DirtyCells = new HashSet<int>();
		ExpandedDirtyCells = new HashSet<int>();
	}

	public void UpdateGraph(HashSet<int> dirty_nav_cells)
	{
		NavGridUpdater.UpdateNavGrid(NavTable, ValidNavTypes, Validators, boundingOffsets, maxLinksPerCell, Links, transitionsByNavType, teleportTransitions, dirty_nav_cells);
		if (OnNavGridUpdateComplete != null)
		{
			OnNavGridUpdateComplete(dirty_nav_cells);
		}
	}

	public static void DebugDrawPath(int start_cell, int end_cell)
	{
		Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
		Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
	}

	public static void DebugDrawPath(PathFinder.Path path)
	{
		if (path.nodes != null)
		{
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				DebugDrawPath(path.nodes[i].cell, path.nodes[i + 1].cell);
			}
		}
	}

	private void DebugDrawValidCells()
	{
		Color color = Color.white;
		int cellCount = Grid.CellCount;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < 11; j++)
			{
				NavType nav_type = (NavType)j;
				if (NavTable.IsValid(i, nav_type) && DrawNavTypeCell(nav_type, ref color))
				{
					DebugExtension.DebugPoint(NavTypeHelper.GetNavPos(i, nav_type), color, 1f, 0f, depthTest: false);
				}
			}
		}
	}

	private void DebugDrawLinks()
	{
		Color color = Color.white;
		for (int i = 0; i < Grid.CellCount; i++)
		{
			int num = i * maxLinksPerCell;
			for (int link = Links[num].link; link != InvalidCell; link = Links[num].link)
			{
				NavTypeHelper.GetNavPos(i, Links[num].startNavType);
				if (DrawNavTypeLink(Links[num].startNavType, ref color) || DrawNavTypeLink(Links[num].endNavType, ref color))
				{
					NavTypeHelper.GetNavPos(link, Links[num].endNavType);
				}
				num++;
			}
		}
	}

	private bool DrawNavTypeLink(NavType nav_type, ref Color color)
	{
		color = NavTypeColor(nav_type);
		if (DebugViewLinksAll)
		{
			return true;
		}
		for (int i = 0; i < ValidNavTypes.Length; i++)
		{
			if (ValidNavTypes[i] == nav_type)
			{
				return DebugViewLinkType[i];
			}
		}
		return false;
	}

	private bool DrawNavTypeCell(NavType nav_type, ref Color color)
	{
		color = NavTypeColor(nav_type);
		if (DebugViewValidCellsAll)
		{
			return true;
		}
		for (int i = 0; i < ValidNavTypes.Length; i++)
		{
			if (ValidNavTypes[i] == nav_type)
			{
				return DebugViewValidCellsType[i];
			}
		}
		return false;
	}

	public void DebugUpdate()
	{
		if (DebugViewValidCells)
		{
			DebugDrawValidCells();
		}
		if (DebugViewLinks)
		{
			DebugDrawLinks();
		}
	}

	public void AddDirtyCell(int cell)
	{
		DirtyCells.Add(cell);
	}

	public void Clear()
	{
		NavTableValidator[] validators = Validators;
		for (int i = 0; i < validators.Length; i++)
		{
			validators[i].Clear();
		}
	}

	private Color NavTypeColor(NavType navType)
	{
		if (debugColorLookup == null)
		{
			debugColorLookup = new Color[11];
			for (int i = 0; i < 11; i++)
			{
				double num = (double)i / 11.0;
				IList<double> list = ColorConverter.HUSLToRGB(new double[3]
				{
					num * 360.0,
					100.0,
					50.0
				});
				debugColorLookup[i] = new Color((float)list[0], (float)list[1], (float)list[2]);
			}
		}
		return debugColorLookup[(uint)navType];
	}
}
