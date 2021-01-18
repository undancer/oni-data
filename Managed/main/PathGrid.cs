using System.Collections.Generic;

public class PathGrid
{
	private struct ProberCell
	{
		public int cost;

		public int queryId;
	}

	private PathFinder.Cell[] Cells;

	private ProberCell[] ProberCells;

	private List<int> freshlyOccupiedCells = new List<int>();

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	private int widthInCells;

	private int heightInCells;

	private bool applyOffset;

	private int rootX;

	private int rootY;

	private int serialNo;

	private int previousSerialNo;

	private bool isUpdating;

	private IGroupProber groupProber;

	public static readonly PathFinder.Cell InvalidCell = new PathFinder.Cell
	{
		cost = -1
	};

	public void SetGroupProber(IGroupProber group_prober)
	{
		groupProber = group_prober;
	}

	public PathGrid(int width_in_cells, int height_in_cells, bool apply_offset, NavType[] valid_nav_types)
	{
		applyOffset = apply_offset;
		widthInCells = width_in_cells;
		heightInCells = height_in_cells;
		ValidNavTypes = valid_nav_types;
		int num = 0;
		NavTypeTable = new int[10];
		for (int i = 0; i < NavTypeTable.Length; i++)
		{
			NavTypeTable[i] = -1;
			for (int j = 0; j < ValidNavTypes.Length; j++)
			{
				if ((uint)ValidNavTypes[j] == (byte)i)
				{
					NavTypeTable[i] = num++;
					break;
				}
			}
		}
		Cells = new PathFinder.Cell[width_in_cells * height_in_cells * ValidNavTypes.Length];
		ProberCells = new ProberCell[width_in_cells * height_in_cells];
		serialNo = 0;
		previousSerialNo = -1;
		isUpdating = false;
	}

	public void OnCleanUp()
	{
		if (groupProber != null)
		{
			groupProber.ReleaseProber(this);
		}
	}

	public void ResetUpdate()
	{
		previousSerialNo = -1;
	}

	public void BeginUpdate(int root_cell, bool isContinuation)
	{
		isUpdating = true;
		freshlyOccupiedCells.Clear();
		if (!isContinuation)
		{
			if (applyOffset)
			{
				Grid.CellToXY(root_cell, out rootX, out rootY);
				rootX -= widthInCells / 2;
				rootY -= heightInCells / 2;
			}
			serialNo++;
			if (groupProber != null)
			{
				groupProber.SetValidSerialNos(this, previousSerialNo, serialNo);
			}
		}
	}

	public void EndUpdate(bool isComplete)
	{
		isUpdating = false;
		if (groupProber != null)
		{
			groupProber.Occupy(this, serialNo, freshlyOccupiedCells);
		}
		if (isComplete)
		{
			if (groupProber != null)
			{
				groupProber.SetValidSerialNos(this, serialNo, serialNo);
			}
			previousSerialNo = serialNo;
		}
	}

	private bool IsValidSerialNo(int serialNo)
	{
		if (serialNo != this.serialNo)
		{
			if (!isUpdating && previousSerialNo != -1)
			{
				return serialNo == previousSerialNo;
			}
			return false;
		}
		return true;
	}

	public PathFinder.Cell GetCell(PathFinder.PotentialPath potential_path, out bool is_cell_in_range)
	{
		return GetCell(potential_path.cell, potential_path.navType, out is_cell_in_range);
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type, out bool is_cell_in_range)
	{
		int num = OffsetCell(cell);
		is_cell_in_range = -1 != num;
		if (!is_cell_in_range)
		{
			return InvalidCell;
		}
		PathFinder.Cell result = Cells[num * ValidNavTypes.Length + NavTypeTable[(uint)nav_type]];
		if (!IsValidSerialNo(result.queryId))
		{
			return InvalidCell;
		}
		return result;
	}

	public void SetCell(PathFinder.PotentialPath potential_path, ref PathFinder.Cell cell_data)
	{
		int num = OffsetCell(potential_path.cell);
		if (-1 == num)
		{
			return;
		}
		cell_data.queryId = serialNo;
		int num2 = NavTypeTable[(uint)potential_path.navType];
		int num3 = num * ValidNavTypes.Length + num2;
		Cells[num3] = cell_data;
		if (potential_path.navType != NavType.Tube)
		{
			ProberCell proberCell = ProberCells[num];
			if (cell_data.queryId != proberCell.queryId || cell_data.cost < proberCell.cost)
			{
				proberCell.queryId = cell_data.queryId;
				proberCell.cost = cell_data.cost;
				ProberCells[num] = proberCell;
				freshlyOccupiedCells.Add(potential_path.cell);
			}
		}
	}

	public int GetCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		int num = -1;
		foreach (CellOffset offset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, offset);
			if (Grid.IsValidCell(num2))
			{
				ProberCell proberCell = ProberCells[num2];
				if (IsValidSerialNo(proberCell.queryId) && (num == -1 || proberCell.cost < num))
				{
					num = proberCell.cost;
				}
			}
		}
		return num;
	}

	public int GetCost(int cell)
	{
		int num = OffsetCell(cell);
		if (-1 == num)
		{
			return -1;
		}
		ProberCell proberCell = ProberCells[num];
		if (!IsValidSerialNo(proberCell.queryId))
		{
			return -1;
		}
		return proberCell.cost;
	}

	private int OffsetCell(int cell)
	{
		if (applyOffset)
		{
			Grid.CellToXY(cell, out var x, out var y);
			if (x < rootX || x >= rootX + widthInCells || y < rootY || y >= rootY + heightInCells)
			{
				return -1;
			}
			int num = x - rootX;
			return (y - rootY) * widthInCells + num;
		}
		return cell;
	}
}
