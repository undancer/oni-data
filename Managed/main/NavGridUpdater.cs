using System.Collections.Generic;
using UnityEngine;

public class NavGridUpdater
{
	private struct CreateLinkWorkItem : IWorkItem<object>
	{
		private int startCell;

		private NavTable navTable;

		private int maxLinksPerCell;

		private NavGrid.Link[] links;

		private NavGrid.Transition[][] transitionsByNavType;

		private Dictionary<int, int> teleportTransitions;

		public CreateLinkWorkItem(int start_cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions)
		{
			startCell = start_cell;
			navTable = nav_table;
			maxLinksPerCell = max_links_per_cell;
			this.links = links;
			transitionsByNavType = transitions_by_nav_type;
			teleportTransitions = teleport_transitions;
		}

		public void Run(object shared_data)
		{
			for (int i = 0; i < Grid.WidthInCells; i++)
			{
				int cell = startCell + i;
				CreateLinksForCell(cell, navTable, maxLinksPerCell, links, transitionsByNavType, teleportTransitions);
			}
		}
	}

	private struct MarkValidCellWorkItem : IWorkItem<object>
	{
		private NavTable navTable;

		private CellOffset[] boundingOffsets;

		private NavTableValidator[] validators;

		private int startCell;

		public MarkValidCellWorkItem(int start_cell, NavTable nav_table, CellOffset[] bounding_offsets, NavTableValidator[] validators)
		{
			startCell = start_cell;
			navTable = nav_table;
			boundingOffsets = bounding_offsets;
			this.validators = validators;
		}

		public void Run(object shared_data)
		{
			for (int i = 0; i < Grid.WidthInCells; i++)
			{
				int cell = startCell + i;
				NavTableValidator[] array = validators;
				foreach (NavTableValidator navTableValidator in array)
				{
					navTableValidator.UpdateCell(cell, navTable, boundingOffsets);
				}
			}
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static void InitializeNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		MarkValidCells(nav_table, valid_nav_types, validators, bounding_offsets);
		CreateLinks(nav_table, max_links_per_cell, links, transitions_by_nav_type, new Dictionary<int, int>());
	}

	public static void UpdateNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions, HashSet<int> dirty_nav_cells)
	{
		UpdateValidCells(dirty_nav_cells, nav_table, valid_nav_types, validators, bounding_offsets);
		UpdateLinks(dirty_nav_cells, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
	}

	private static void UpdateValidCells(HashSet<int> dirty_solid_cells, NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		foreach (int dirty_solid_cell in dirty_solid_cells)
		{
			foreach (NavTableValidator navTableValidator in validators)
			{
				navTableValidator.UpdateCell(dirty_solid_cell, nav_table, bounding_offsets);
			}
		}
	}

	private static void CreateLinksForCell(int cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions)
	{
		CreateLinks(cell, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
	}

	private static void UpdateLinks(HashSet<int> dirty_nav_cells, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions)
	{
		foreach (int dirty_nav_cell in dirty_nav_cells)
		{
			CreateLinksForCell(dirty_nav_cell, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
		}
	}

	private static void CreateLinks(NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions)
	{
		WorkItemCollection<CreateLinkWorkItem, object> workItemCollection = new WorkItemCollection<CreateLinkWorkItem, object>();
		workItemCollection.Reset(null);
		for (int i = 0; i < Grid.HeightInCells; i++)
		{
			workItemCollection.Add(new CreateLinkWorkItem(Grid.OffsetCell(0, new CellOffset(0, i)), nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions));
		}
		GlobalJobManager.Run(workItemCollection);
	}

	private static void CreateLinks(int cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, Dictionary<int, int> teleport_transitions)
	{
		int num = cell * max_links_per_cell;
		int num2 = 0;
		for (int i = 0; i < 11; i++)
		{
			NavType nav_type = (NavType)i;
			NavGrid.Transition[] array = transitions_by_nav_type[i];
			if (array == null || !nav_table.IsValid(cell, nav_type))
			{
				continue;
			}
			NavGrid.Transition[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				NavGrid.Transition transition = array2[j];
				NavGrid.Transition transition2 = transition;
				if (transition.start == NavType.Teleport && teleport_transitions.ContainsKey(cell))
				{
					Grid.CellToXY(cell, out var x, out var y);
					int num3 = teleport_transitions[cell];
					Grid.CellToXY(teleport_transitions[cell], out var x2, out var y2);
					transition2.x = x2 - x;
					transition2.y = y2 - y;
				}
				int num4 = transition2.IsValid(cell, nav_table);
				if (num4 != Grid.InvalidCell)
				{
					links[num] = new NavGrid.Link(num4, transition2.start, transition2.end, transition2.id, transition2.cost);
					num++;
					num2++;
				}
			}
		}
		if (num2 >= max_links_per_cell)
		{
			Debug.LogError("Out of nav links. Need to increase maxLinksPerCell:" + max_links_per_cell);
		}
		links[num].link = Grid.InvalidCell;
	}

	private static void MarkValidCells(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		WorkItemCollection<MarkValidCellWorkItem, object> workItemCollection = new WorkItemCollection<MarkValidCellWorkItem, object>();
		workItemCollection.Reset(null);
		for (int i = 0; i < Grid.HeightInCells; i++)
		{
			workItemCollection.Add(new MarkValidCellWorkItem(Grid.OffsetCell(0, new CellOffset(0, i)), nav_table, bounding_offsets, validators));
		}
		GlobalJobManager.Run(workItemCollection);
	}

	public static void DebugDrawPath(int start_cell, int end_cell)
	{
		Vector3 vector = Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
		Vector3 vector2 = Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
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
}
