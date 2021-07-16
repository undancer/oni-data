using System;
using System.Collections.Generic;

public class ScenePartitioner : ISim1000ms
{
	private struct ScenePartitionerNode
	{
		public HashSet<ScenePartitionerEntry> entries;

		public bool dirty;
	}

	private struct DirtyNode
	{
		public int layer;

		public int x;

		public int y;
	}

	public List<ScenePartitionerLayer> layers = new List<ScenePartitionerLayer>();

	private int nodeSize;

	private List<DirtyNode> dirtyNodes = new List<DirtyNode>();

	private ScenePartitionerNode[,,] nodes;

	private int queryId;

	private static readonly Predicate<ScenePartitionerEntry> removeCallback = (ScenePartitionerEntry entry) => entry == null || entry.obj == null;

	public ScenePartitioner(int node_size, int layer_count, int scene_width, int scene_height)
	{
		nodeSize = node_size;
		int num = scene_width / node_size;
		int num2 = scene_height / node_size;
		nodes = new ScenePartitionerNode[layer_count, num2, num];
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				for (int k = 0; k < nodes.GetLength(2); k++)
				{
					nodes[i, j, k].entries = new HashSet<ScenePartitionerEntry>();
				}
			}
		}
		SimAndRenderScheduler.instance.Add(this);
	}

	public void FreeResources()
	{
		for (int i = 0; i < nodes.GetLength(0); i++)
		{
			for (int j = 0; j < nodes.GetLength(1); j++)
			{
				for (int k = 0; k < nodes.GetLength(2); k++)
				{
					foreach (ScenePartitionerEntry entry in nodes[i, j, k].entries)
					{
						if (entry != null)
						{
							entry.partitioner = null;
							entry.obj = null;
						}
					}
					nodes[i, j, k].entries.Clear();
				}
			}
		}
		nodes = null;
	}

	public ScenePartitionerLayer CreateMask(HashedString name)
	{
		foreach (ScenePartitionerLayer layer in layers)
		{
			if (layer.name == name)
			{
				return layer;
			}
		}
		ScenePartitionerLayer scenePartitionerLayer = new ScenePartitionerLayer(name, layers.Count);
		layers.Add(scenePartitionerLayer);
		DebugUtil.Assert(layers.Count <= nodes.GetLength(0));
		return scenePartitionerLayer;
	}

	private int ClampNodeX(int x)
	{
		return Math.Min(Math.Max(x, 0), nodes.GetLength(2) - 1);
	}

	private int ClampNodeY(int y)
	{
		return Math.Min(Math.Max(y, 0), nodes.GetLength(1) - 1);
	}

	private Extents GetNodeExtents(int x, int y, int width, int height)
	{
		Extents result = default(Extents);
		result.x = ClampNodeX(x / nodeSize);
		result.y = ClampNodeY(y / nodeSize);
		result.width = 1 + ClampNodeX((x + width) / nodeSize) - result.x;
		result.height = 1 + ClampNodeY((y + height) / nodeSize) - result.y;
		return result;
	}

	private Extents GetNodeExtents(ScenePartitionerEntry entry)
	{
		return GetNodeExtents(entry.x, entry.y, entry.width, entry.height);
	}

	private void Insert(ScenePartitionerEntry entry)
	{
		if (entry.obj == null)
		{
			Debug.LogWarning("Trying to put null go into scene partitioner");
			return;
		}
		Extents nodeExtents = GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > nodes.GetLength(2))
		{
			Debug.LogError(entry.obj.ToString() + " x/w " + nodeExtents.x + "/" + nodeExtents.width + " < " + nodes.GetLength(2));
		}
		if (nodeExtents.y + nodeExtents.height > nodes.GetLength(1))
		{
			Debug.LogError(entry.obj.ToString() + " y/h " + nodeExtents.y + "/" + nodeExtents.height + " < " + nodes.GetLength(1));
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				if (!nodes[layer, i, j].dirty)
				{
					nodes[layer, i, j].dirty = true;
					dirtyNodes.Add(new DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
				}
				nodes[layer, i, j].entries.Add(entry);
			}
		}
	}

	private void Widthdraw(ScenePartitionerEntry entry)
	{
		Extents nodeExtents = GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > nodes.GetLength(2))
		{
			Debug.LogError(" x/w " + nodeExtents.x + "/" + nodeExtents.width + " < " + nodes.GetLength(2));
		}
		if (nodeExtents.y + nodeExtents.height > nodes.GetLength(1))
		{
			Debug.LogError(" y/h " + nodeExtents.y + "/" + nodeExtents.height + " < " + nodes.GetLength(1));
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				if (nodes[layer, i, j].entries.Remove(entry) && !nodes[layer, i, j].dirty)
				{
					nodes[layer, i, j].dirty = true;
					dirtyNodes.Add(new DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
				}
			}
		}
	}

	public ScenePartitionerEntry Add(ScenePartitionerEntry entry)
	{
		Insert(entry);
		return entry;
	}

	public void UpdatePosition(int x, int y, ScenePartitionerEntry entry)
	{
		Widthdraw(entry);
		entry.x = x;
		entry.y = y;
		Insert(entry);
	}

	public void Remove(ScenePartitionerEntry entry)
	{
		Extents nodeExtents = GetNodeExtents(entry);
		if (nodeExtents.x + nodeExtents.width > nodes.GetLength(2))
		{
			Debug.LogError(" x/w " + nodeExtents.x + "/" + nodeExtents.width + " < " + nodes.GetLength(2));
		}
		if (nodeExtents.y + nodeExtents.height > nodes.GetLength(1))
		{
			Debug.LogError(" y/h " + nodeExtents.y + "/" + nodeExtents.height + " < " + nodes.GetLength(1));
		}
		int layer = entry.layer;
		for (int i = nodeExtents.y; i < nodeExtents.y + nodeExtents.height; i++)
		{
			for (int j = nodeExtents.x; j < nodeExtents.x + nodeExtents.width; j++)
			{
				if (!nodes[layer, i, j].dirty)
				{
					nodes[layer, i, j].dirty = true;
					dirtyNodes.Add(new DirtyNode
					{
						layer = layer,
						x = j,
						y = i
					});
				}
			}
		}
		entry.obj = null;
	}

	public void Sim1000ms(float dt)
	{
		foreach (DirtyNode dirtyNode in dirtyNodes)
		{
			nodes[dirtyNode.layer, dirtyNode.y, dirtyNode.x].entries.RemoveWhere(removeCallback);
			nodes[dirtyNode.layer, dirtyNode.y, dirtyNode.x].dirty = false;
		}
		dirtyNodes.Clear();
	}

	public void TriggerEvent(List<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		ListPool<ScenePartitionerEntry, ScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, ScenePartitioner>.Allocate();
		queryId++;
		for (int i = 0; i < cells.Count; i++)
		{
			int x = 0;
			int y = 0;
			Grid.CellToXY(cells[i], out x, out y);
			GatherEntries(x, y, 1, 1, layer, event_data, pooledList, queryId);
		}
		RunLayerGlobalEvent(cells, layer, event_data);
		RunEntries(pooledList, event_data);
		pooledList.Recycle();
	}

	public void TriggerEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		ListPool<ScenePartitionerEntry, ScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, ScenePartitioner>.Allocate();
		queryId++;
		foreach (int cell in cells)
		{
			int x = 0;
			int y = 0;
			Grid.CellToXY(cell, out x, out y);
			GatherEntries(x, y, 1, 1, layer, event_data, pooledList, queryId);
		}
		RunLayerGlobalEvent(cells, layer, event_data);
		RunEntries(pooledList, event_data);
		pooledList.Recycle();
	}

	public void TriggerEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		ListPool<ScenePartitionerEntry, ScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, ScenePartitioner>.Allocate();
		GatherEntries(x, y, width, height, layer, event_data, pooledList);
		RunLayerGlobalEvent(x, y, width, height, layer, event_data);
		RunEntries(pooledList, event_data);
		pooledList.Recycle();
	}

	private void RunLayerGlobalEvent(List<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		if (layer.OnEvent != null)
		{
			for (int i = 0; i < cells.Count; i++)
			{
				layer.OnEvent(cells[i], event_data);
			}
		}
	}

	private void RunLayerGlobalEvent(HashSet<int> cells, ScenePartitionerLayer layer, object event_data)
	{
		if (layer.OnEvent == null)
		{
			return;
		}
		foreach (int cell in cells)
		{
			layer.OnEvent(cell, event_data);
		}
	}

	private void RunLayerGlobalEvent(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data)
	{
		if (layer.OnEvent == null)
		{
			return;
		}
		for (int i = y; i < y + height; i++)
		{
			for (int j = x; j < x + width; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num))
				{
					layer.OnEvent(num, event_data);
				}
			}
		}
	}

	private void RunEntries(List<ScenePartitionerEntry> gathered_entries, object event_data)
	{
		for (int i = 0; i < gathered_entries.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = gathered_entries[i];
			if (scenePartitionerEntry.obj != null && scenePartitionerEntry.eventCallback != null)
			{
				scenePartitionerEntry.eventCallback(event_data);
			}
		}
	}

	public void GatherEntries(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data, List<ScenePartitionerEntry> gathered_entries)
	{
		GatherEntries(x, y, width, height, layer, event_data, gathered_entries, ++queryId);
	}

	public void GatherEntries(int x, int y, int width, int height, ScenePartitionerLayer layer, object event_data, List<ScenePartitionerEntry> gathered_entries, int query_id)
	{
		Extents nodeExtents = GetNodeExtents(x, y, width, height);
		int num = Math.Min(nodeExtents.y + nodeExtents.height, nodes.GetLength(1));
		int num2 = Math.Max(nodeExtents.y, 0);
		int num3 = Math.Max(nodeExtents.x, 0);
		int num4 = Math.Min(nodeExtents.x + nodeExtents.width, nodes.GetLength(2));
		int layer2 = layer.layer;
		for (int i = num2; i < num; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				ListPool<ScenePartitionerEntry, ScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, ScenePartitioner>.Allocate();
				foreach (ScenePartitionerEntry entry in nodes[layer2, i, j].entries)
				{
					if (entry != null && entry.queryId != queryId)
					{
						if (entry.obj == null)
						{
							pooledList.Add(entry);
						}
						else if (x + width - 1 >= entry.x && x <= entry.x + entry.width - 1 && y + height - 1 >= entry.y && y <= entry.y + entry.height - 1)
						{
							entry.queryId = queryId;
							gathered_entries.Add(entry);
						}
					}
				}
				nodes[layer2, i, j].entries.ExceptWith(pooledList);
				pooledList.Recycle();
			}
		}
	}

	public void Cleanup()
	{
		SimAndRenderScheduler.instance.Remove(this);
	}
}
