using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionGroupProber")]
public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	private static MinionGroupProber Instance;

	private Dictionary<object, short>[] cells;

	private Dictionary<object, KeyValuePair<short, short>> valid_serial_nos = new Dictionary<object, KeyValuePair<short, short>>();

	private List<object> pending_removals = new List<object>();

	private readonly object access = new object();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public static MinionGroupProber Get()
	{
		return Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		cells = new Dictionary<object, short>[Grid.CellCount];
	}

	private bool IsReachable_AssumeLock(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		Dictionary<object, short> dictionary = cells[cell];
		if (dictionary == null)
		{
			return false;
		}
		bool result = false;
		foreach (KeyValuePair<object, short> item in dictionary)
		{
			object key = item.Key;
			short value = item.Value;
			if (valid_serial_nos.TryGetValue(key, out var value2) && (value == value2.Key || value == value2.Value))
			{
				result = true;
				break;
			}
			pending_removals.Add(key);
		}
		foreach (object pending_removal in pending_removals)
		{
			dictionary.Remove(pending_removal);
			if (dictionary.Count == 0)
			{
				cells[cell] = null;
			}
		}
		pending_removals.Clear();
		return result;
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool flag = false;
		lock (access)
		{
			return IsReachable_AssumeLock(cell);
		}
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool result = false;
		lock (access)
		{
			foreach (CellOffset offset in offsets)
			{
				if (IsReachable_AssumeLock(Grid.OffsetCell(cell, offset)))
				{
					return true;
				}
			}
			return result;
		}
	}

	public bool IsAllReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		bool result = false;
		lock (access)
		{
			if (IsReachable_AssumeLock(cell))
			{
				return true;
			}
			foreach (CellOffset offset in offsets)
			{
				if (IsReachable_AssumeLock(Grid.OffsetCell(cell, offset)))
				{
					return true;
				}
			}
			return result;
		}
	}

	public bool IsReachable(Workable workable)
	{
		return IsReachable(Grid.PosToCell(workable), workable.GetOffsets());
	}

	public void Occupy(object prober, short serial_no, IEnumerable<int> cells)
	{
		lock (access)
		{
			foreach (int cell in cells)
			{
				if (this.cells[cell] == null)
				{
					this.cells[cell] = new Dictionary<object, short>();
				}
				this.cells[cell][prober] = serial_no;
			}
		}
	}

	public void SetValidSerialNos(object prober, short previous_serial_no, short serial_no)
	{
		lock (access)
		{
			valid_serial_nos[prober] = new KeyValuePair<short, short>(previous_serial_no, serial_no);
		}
	}

	public bool ReleaseProber(object prober)
	{
		lock (access)
		{
			return valid_serial_nos.Remove(prober);
		}
	}
}
