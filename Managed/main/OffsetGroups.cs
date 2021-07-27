using System;
using System.Collections.Generic;

public static class OffsetGroups
{
	private class CellOffsetComparer : IComparer<CellOffset>
	{
		public int Compare(CellOffset a, CellOffset b)
		{
			int num = Math.Abs(a.x) + Math.Abs(a.y);
			int value = Math.Abs(b.x) + Math.Abs(b.y);
			return num.CompareTo(value);
		}
	}

	public static CellOffset[] Use = new CellOffset[1];

	public static CellOffset[] Chat = new CellOffset[6]
	{
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1)
	};

	public static CellOffset[] LeftOnly = new CellOffset[1]
	{
		new CellOffset(-1, 0)
	};

	public static CellOffset[] RightOnly = new CellOffset[1]
	{
		new CellOffset(1, 0)
	};

	public static CellOffset[] Standard = InitGrid(-2, 2, -3, 3);

	public static CellOffset[] LiquidSource = new CellOffset[11]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(0, 1),
		new CellOffset(0, -1),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1),
		new CellOffset(2, 0),
		new CellOffset(-2, 0)
	};

	public static CellOffset[][] InvertedStandardTable = OffsetTable.Mirror(new CellOffset[28][]
	{
		new CellOffset[1]
		{
			new CellOffset(0, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(0, -3),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 0)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(0, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[5]
		{
			new CellOffset(2, 3),
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -1),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -2),
			new CellOffset(2, -1),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		}
	});

	public static CellOffset[][] InvertedStandardTableWithCorners = OffsetTable.Mirror(new CellOffset[24][]
	{
		new CellOffset[1]
		{
			new CellOffset(0, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(0, -3),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 3),
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -1),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -2),
			new CellOffset(2, -1),
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1)
		}
	});

	public static CellOffset[][] InvertedWideTable = OffsetTable.Mirror(new CellOffset[33][]
	{
		new CellOffset[1]
		{
			new CellOffset(0, 0)
		},
		new CellOffset[1]
		{
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(0, -2)
		},
		new CellOffset[3]
		{
			new CellOffset(0, -3),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, 0)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[2]
		{
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, 2),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, 3),
			new CellOffset(0, 3),
			new CellOffset(0, 2),
			new CellOffset(0, 1)
		},
		new CellOffset[1]
		{
			new CellOffset(1, -1)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(0, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(1, -3),
			new CellOffset(1, -2),
			new CellOffset(0, -2),
			new CellOffset(0, -1)
		},
		new CellOffset[2]
		{
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(2, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[5]
		{
			new CellOffset(2, 3),
			new CellOffset(1, 3),
			new CellOffset(1, 2),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[3]
		{
			new CellOffset(2, -1),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -2),
			new CellOffset(2, -1),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(2, -3),
			new CellOffset(1, -2),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		},
		new CellOffset[3]
		{
			new CellOffset(3, 0),
			new CellOffset(2, 0),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(3, 1),
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(0, 1)
		},
		new CellOffset[4]
		{
			new CellOffset(3, 1),
			new CellOffset(2, 1),
			new CellOffset(1, 1),
			new CellOffset(1, 0)
		},
		new CellOffset[4]
		{
			new CellOffset(3, -1),
			new CellOffset(2, -1),
			new CellOffset(1, -1),
			new CellOffset(0, -1)
		},
		new CellOffset[4]
		{
			new CellOffset(3, -1),
			new CellOffset(2, -1),
			new CellOffset(1, -1),
			new CellOffset(1, 0)
		}
	});

	private static Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>> reachabilityTableCache = new Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>>();

	private static readonly CellOffset[] nullFilter = new CellOffset[0];

	public static CellOffset[] InitGrid(int x0, int x1, int y0, int y1)
	{
		List<CellOffset> list = new List<CellOffset>();
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				list.Add(new CellOffset(j, i));
			}
		}
		CellOffset[] array = list.ToArray();
		Array.Sort(array, 0, array.Length, new CellOffsetComparer());
		return array;
	}

	public static CellOffset[][] BuildReachabilityTable(CellOffset[] area_offsets, CellOffset[][] table, CellOffset[] filter)
	{
		Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>> value = null;
		Dictionary<CellOffset[], CellOffset[][]> value2 = null;
		CellOffset[][] value3 = null;
		if (reachabilityTableCache.TryGetValue(area_offsets, out value) && value.TryGetValue(table, out value2) && value2.TryGetValue((filter == null) ? nullFilter : filter, out value3))
		{
			return value3;
		}
		HashSet<CellOffset> hashSet = new HashSet<CellOffset>();
		CellOffset[] array = area_offsets;
		foreach (CellOffset cellOffset in array)
		{
			CellOffset[][] array2 = table;
			foreach (CellOffset[] array3 in array2)
			{
				if (filter == null || Array.IndexOf(filter, array3[0]) == -1)
				{
					CellOffset item = cellOffset + array3[0];
					hashSet.Add(item);
				}
			}
		}
		List<CellOffset[]> list = new List<CellOffset[]>();
		foreach (CellOffset item2 in hashSet)
		{
			CellOffset cellOffset2 = area_offsets[0];
			array = area_offsets;
			foreach (CellOffset cellOffset3 in array)
			{
				if ((item2 - cellOffset2).GetOffsetDistance() > (item2 - cellOffset3).GetOffsetDistance())
				{
					cellOffset2 = cellOffset3;
				}
			}
			CellOffset[][] array2 = table;
			foreach (CellOffset[] array4 in array2)
			{
				if ((filter == null || Array.IndexOf(filter, array4[0]) == -1) && array4[0] + cellOffset2 == item2)
				{
					CellOffset[] array5 = new CellOffset[array4.Length];
					for (int k = 0; k < array4.Length; k++)
					{
						array5[k] = array4[k] + cellOffset2;
					}
					list.Add(array5);
				}
			}
		}
		value3 = list.ToArray();
		Array.Sort(value3, (CellOffset[] x, CellOffset[] y) => x[0].GetOffsetDistance().CompareTo(y[0].GetOffsetDistance()));
		if (value == null)
		{
			value = new Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>();
			reachabilityTableCache.Add(area_offsets, value);
		}
		if (value2 == null)
		{
			value2 = new Dictionary<CellOffset[], CellOffset[][]>();
			value.Add(table, value2);
		}
		value2.Add((filter == null) ? nullFilter : filter, value3);
		return value3;
	}
}
