using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolidConduitFlow : IConduitFlow
{
	private struct StoredInfo
	{
		public KBatchedAnimController kbac;

		public Pickupable pickupable;
	}

	public class SOAInfo
	{
		private List<Conduit> conduits = new List<Conduit>();

		private List<ConduitConnections> conduitConnections = new List<ConduitConnections>();

		private List<ConduitFlowInfo> lastFlowInfo = new List<ConduitFlowInfo>();

		private List<ConduitContents> initialContents = new List<ConduitContents>();

		private List<GameObject> conduitGOs = new List<GameObject>();

		private List<bool> diseaseContentsVisible = new List<bool>();

		private List<bool> updated = new List<bool>();

		private List<int> cells = new List<int>();

		private List<int> permittedFlowDirections = new List<int>();

		private List<int> srcFlowIdx = new List<int>();

		private List<FlowDirection> srcFlowDirections = new List<FlowDirection>();

		private List<FlowDirection> targetFlowDirections = new List<FlowDirection>();

		public int NumEntries => conduits.Count;

		public List<int> Cells => cells;

		public int AddConduit(SolidConduitFlow manager, GameObject conduit_go, int cell)
		{
			int count = conduitConnections.Count;
			Conduit item = new Conduit(count);
			conduits.Add(item);
			conduitConnections.Add(new ConduitConnections
			{
				left = -1,
				right = -1,
				up = -1,
				down = -1
			});
			ConduitContents contents = manager.grid[cell].contents;
			initialContents.Add(contents);
			lastFlowInfo.Add(new ConduitFlowInfo
			{
				direction = FlowDirection.None
			});
			cells.Add(cell);
			updated.Add(item: false);
			diseaseContentsVisible.Add(item: false);
			conduitGOs.Add(conduit_go);
			srcFlowIdx.Add(-1);
			permittedFlowDirections.Add(0);
			srcFlowDirections.Add(FlowDirection.None);
			targetFlowDirections.Add(FlowDirection.None);
			return count;
		}

		public void Clear(SolidConduitFlow manager)
		{
			for (int i = 0; i < conduits.Count; i++)
			{
				ForcePermanentDiseaseContainer(i, force_on: false);
				int num = cells[i];
				ConduitContents contents = manager.grid[num].contents;
				manager.grid[num].contents = contents;
				manager.grid[num].conduitIdx = -1;
			}
			cells.Clear();
			updated.Clear();
			diseaseContentsVisible.Clear();
			srcFlowIdx.Clear();
			permittedFlowDirections.Clear();
			srcFlowDirections.Clear();
			targetFlowDirections.Clear();
			conduitGOs.Clear();
			initialContents.Clear();
			lastFlowInfo.Clear();
			conduitConnections.Clear();
			conduits.Clear();
		}

		public Conduit GetConduit(int idx)
		{
			return conduits[idx];
		}

		public GameObject GetConduitGO(int idx)
		{
			return conduitGOs[idx];
		}

		public ConduitConnections GetConduitConnections(int idx)
		{
			return conduitConnections[idx];
		}

		public void SetConduitConnections(int idx, ConduitConnections data)
		{
			conduitConnections[idx] = data;
		}

		public void ForcePermanentDiseaseContainer(int idx, bool force_on)
		{
			if (diseaseContentsVisible[idx] != force_on)
			{
				diseaseContentsVisible[idx] = force_on;
				GameObject gameObject = conduitGOs[idx];
				if (!(gameObject == null))
				{
					gameObject.GetComponent<PrimaryElement>().ForcePermanentDiseaseContainer(force_on);
				}
			}
		}

		public Conduit GetConduitFromDirection(int idx, FlowDirection direction)
		{
			Conduit result = Conduit.Invalid();
			ConduitConnections conduitConnections = this.conduitConnections[idx];
			switch (direction)
			{
			case FlowDirection.Left:
				result = ((conduitConnections.left != -1) ? conduits[conduitConnections.left] : Conduit.Invalid());
				break;
			case FlowDirection.Right:
				result = ((conduitConnections.right != -1) ? conduits[conduitConnections.right] : Conduit.Invalid());
				break;
			case FlowDirection.Up:
				result = ((conduitConnections.up != -1) ? conduits[conduitConnections.up] : Conduit.Invalid());
				break;
			case FlowDirection.Down:
				result = ((conduitConnections.down != -1) ? conduits[conduitConnections.down] : Conduit.Invalid());
				break;
			}
			return result;
		}

		public void BeginFrame(SolidConduitFlow manager)
		{
			for (int i = 0; i < conduits.Count; i++)
			{
				updated[i] = false;
				ConduitContents contents = conduits[i].GetContents(manager);
				initialContents[i] = contents;
				lastFlowInfo[i] = new ConduitFlowInfo
				{
					direction = FlowDirection.None
				};
				int num = cells[i];
				manager.grid[num].contents = contents;
			}
			for (int j = 0; j < manager.freedHandles.Count; j++)
			{
				HandleVector<int>.Handle handle = manager.freedHandles[j];
				manager.conveyorPickupables.Free(handle);
			}
			manager.freedHandles.Clear();
		}

		public void EndFrame(SolidConduitFlow manager)
		{
		}

		public void UpdateFlowDirection(SolidConduitFlow manager)
		{
			for (int i = 0; i < conduits.Count; i++)
			{
				Conduit conduit = conduits[i];
				if (!updated[i])
				{
					int cell = conduit.GetCell(manager);
					ConduitContents contents = manager.grid[cell].contents;
					if (!contents.pickupableHandle.IsValid())
					{
						srcFlowDirections[conduit.idx] = conduit.GetNextFlowSource(manager);
					}
				}
			}
		}

		public void MarkConduitEmpty(int idx, SolidConduitFlow manager)
		{
			if (lastFlowInfo[idx].direction != 0)
			{
				lastFlowInfo[idx] = new ConduitFlowInfo
				{
					direction = FlowDirection.None
				};
				Conduit conduit = conduits[idx];
				targetFlowDirections[idx] = conduit.GetNextFlowTarget(manager);
				int num = cells[idx];
				manager.grid[num].contents = ConduitContents.EmptyContents();
			}
		}

		public void SetLastFlowInfo(int idx, FlowDirection direction)
		{
			lastFlowInfo[idx] = new ConduitFlowInfo
			{
				direction = direction
			};
		}

		public ConduitContents GetInitialContents(int idx)
		{
			return initialContents[idx];
		}

		public ConduitFlowInfo GetLastFlowInfo(int idx)
		{
			return lastFlowInfo[idx];
		}

		public int GetPermittedFlowDirections(int idx)
		{
			return permittedFlowDirections[idx];
		}

		public void SetPermittedFlowDirections(int idx, int permitted)
		{
			permittedFlowDirections[idx] = permitted;
		}

		public FlowDirection GetTargetFlowDirection(int idx)
		{
			return targetFlowDirections[idx];
		}

		public void SetTargetFlowDirection(int idx, FlowDirection directions)
		{
			targetFlowDirections[idx] = directions;
		}

		public int GetSrcFlowIdx(int idx)
		{
			return srcFlowIdx[idx];
		}

		public void SetSrcFlowIdx(int idx, int new_src_idx)
		{
			srcFlowIdx[idx] = new_src_idx;
		}

		public FlowDirection GetSrcFlowDirection(int idx)
		{
			return srcFlowDirections[idx];
		}

		public void SetSrcFlowDirection(int idx, FlowDirection directions)
		{
			srcFlowDirections[idx] = directions;
		}

		public int GetCell(int idx)
		{
			return cells[idx];
		}

		public void SetCell(int idx, int cell)
		{
			cells[idx] = cell;
		}

		public bool GetUpdated(int idx)
		{
			return updated[idx];
		}

		public void SetUpdated(int idx, bool is_updated)
		{
			updated[idx] = is_updated;
		}
	}

	[DebuggerDisplay("{priority} {callback.Target.name} {callback.Target} {callback.Method}")]
	public struct ConduitUpdater
	{
		public ConduitFlowPriority priority;

		public Action<float> callback;
	}

	public struct GridNode
	{
		public int conduitIdx;

		public ConduitContents contents;
	}

	public enum FlowDirection
	{
		Blocked = -1,
		None,
		Left,
		Right,
		Up,
		Down,
		Num
	}

	public struct ConduitConnections
	{
		public int left;

		public int right;

		public int up;

		public int down;
	}

	public struct ConduitFlowInfo
	{
		public FlowDirection direction;
	}

	[Serializable]
	public struct Conduit : IEquatable<Conduit>
	{
		public int idx;

		public static Conduit Invalid()
		{
			return new Conduit(-1);
		}

		public Conduit(int idx)
		{
			this.idx = idx;
		}

		public int GetPermittedFlowDirections(SolidConduitFlow manager)
		{
			return manager.soaInfo.GetPermittedFlowDirections(idx);
		}

		public void SetPermittedFlowDirections(int permitted, SolidConduitFlow manager)
		{
			manager.soaInfo.SetPermittedFlowDirections(idx, permitted);
		}

		public FlowDirection GetTargetFlowDirection(SolidConduitFlow manager)
		{
			return manager.soaInfo.GetTargetFlowDirection(idx);
		}

		public void SetTargetFlowDirection(FlowDirection directions, SolidConduitFlow manager)
		{
			manager.soaInfo.SetTargetFlowDirection(idx, directions);
		}

		public ConduitContents GetContents(SolidConduitFlow manager)
		{
			int cell = manager.soaInfo.GetCell(idx);
			return manager.grid[cell].contents;
		}

		public void SetContents(SolidConduitFlow manager, ConduitContents contents)
		{
			int cell = manager.soaInfo.GetCell(idx);
			manager.grid[cell].contents = contents;
			if (contents.pickupableHandle.IsValid())
			{
				Pickupable pickupable = manager.GetPickupable(contents.pickupableHandle);
				if (pickupable != null)
				{
					pickupable.transform.parent = null;
					Vector3 position = Grid.CellToPosCCC(cell, Grid.SceneLayer.SolidConduitContents);
					pickupable.transform.SetPosition(position);
					pickupable.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.SolidConduitContents);
				}
			}
		}

		public FlowDirection GetNextFlowSource(SolidConduitFlow manager)
		{
			if (manager.soaInfo.GetPermittedFlowDirections(idx) == -1)
			{
				return FlowDirection.Blocked;
			}
			FlowDirection flowDirection = manager.soaInfo.GetSrcFlowDirection(idx);
			if (flowDirection == FlowDirection.None)
			{
				flowDirection = FlowDirection.Down;
			}
			for (int i = 0; i < 5; i++)
			{
				FlowDirection flowDirection2 = (FlowDirection)((int)(flowDirection + i - 1 + 1) % 5 + 1);
				Conduit conduitFromDirection = manager.soaInfo.GetConduitFromDirection(idx, flowDirection2);
				if (conduitFromDirection.idx == -1)
				{
					continue;
				}
				ConduitContents contents = manager.grid[conduitFromDirection.GetCell(manager)].contents;
				if (!contents.pickupableHandle.IsValid())
				{
					continue;
				}
				int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection.idx);
				if (permittedFlowDirections != -1)
				{
					FlowDirection direction = InverseFlow(flowDirection2);
					if (manager.soaInfo.GetConduitFromDirection(conduitFromDirection.idx, direction).idx != -1 && (permittedFlowDirections & FlowBit(direction)) != 0)
					{
						return flowDirection2;
					}
				}
			}
			for (int j = 0; j < 5; j++)
			{
				FlowDirection flowDirection3 = (FlowDirection)((int)(manager.soaInfo.GetTargetFlowDirection(idx) + j - 1 + 1) % 5 + 1);
				FlowDirection direction2 = InverseFlow(flowDirection3);
				Conduit conduitFromDirection2 = manager.soaInfo.GetConduitFromDirection(idx, flowDirection3);
				if (conduitFromDirection2.idx != -1)
				{
					int permittedFlowDirections2 = manager.soaInfo.GetPermittedFlowDirections(conduitFromDirection2.idx);
					if (permittedFlowDirections2 != -1 && (permittedFlowDirections2 & FlowBit(direction2)) != 0)
					{
						return flowDirection3;
					}
				}
			}
			return FlowDirection.None;
		}

		public FlowDirection GetNextFlowTarget(SolidConduitFlow manager)
		{
			int permittedFlowDirections = manager.soaInfo.GetPermittedFlowDirections(idx);
			if (permittedFlowDirections == -1)
			{
				return FlowDirection.Blocked;
			}
			for (int i = 0; i < 5; i++)
			{
				int num = (int)(manager.soaInfo.GetTargetFlowDirection(idx) + i - 1 + 1) % 5 + 1;
				if (manager.soaInfo.GetConduitFromDirection(idx, (FlowDirection)num).idx != -1 && (permittedFlowDirections & FlowBit((FlowDirection)num)) != 0)
				{
					return (FlowDirection)num;
				}
			}
			return FlowDirection.Blocked;
		}

		public ConduitFlowInfo GetLastFlowInfo(SolidConduitFlow manager)
		{
			return manager.soaInfo.GetLastFlowInfo(idx);
		}

		public ConduitContents GetInitialContents(SolidConduitFlow manager)
		{
			return manager.soaInfo.GetInitialContents(idx);
		}

		public int GetCell(SolidConduitFlow manager)
		{
			return manager.soaInfo.GetCell(idx);
		}

		public bool Equals(Conduit other)
		{
			return idx == other.idx;
		}
	}

	[DebuggerDisplay("{pickupable}")]
	public struct ConduitContents
	{
		public HandleVector<int>.Handle pickupableHandle;

		public ConduitContents(HandleVector<int>.Handle pickupable_handle)
		{
			pickupableHandle = pickupable_handle;
		}

		public static ConduitContents EmptyContents()
		{
			ConduitContents result = default(ConduitContents);
			result.pickupableHandle = HandleVector<int>.InvalidHandle;
			return result;
		}
	}

	public const float MAX_SOLID_MASS = 20f;

	public const float TickRate = 1f;

	public const float WaitTime = 1f;

	private float elapsedTime;

	private float lastUpdateTime = float.NegativeInfinity;

	private KCompactedVector<StoredInfo> conveyorPickupables = new KCompactedVector<StoredInfo>();

	private List<HandleVector<int>.Handle> freedHandles = new List<HandleVector<int>.Handle>();

	private SOAInfo soaInfo = new SOAInfo();

	private bool dirtyConduitUpdaters;

	private List<ConduitUpdater> conduitUpdaters = new List<ConduitUpdater>();

	private GridNode[] grid;

	public IUtilityNetworkMgr networkMgr;

	private HashSet<int> visited = new HashSet<int>();

	private HashSet<int> replacements = new HashSet<int>();

	private List<Conduit> path = new List<Conduit>();

	private List<List<Conduit>> pathList = new List<List<Conduit>>();

	public static readonly ConduitContents emptyContents = new ConduitContents
	{
		pickupableHandle = HandleVector<int>.InvalidHandle
	};

	private int maskedOverlayLayer;

	private bool viewingConduits;

	private static readonly Color32 NormalColour = Color.white;

	private static readonly Color32 OverlayColour = new Color(0.25f, 0.25f, 0.25f, 0f);

	public float ContinuousLerpPercent => Mathf.Clamp01((Time.time - lastUpdateTime) / 1f);

	public float DiscreteLerpPercent => Mathf.Clamp01(elapsedTime / 1f);

	public event System.Action onConduitsRebuilt;

	public SOAInfo GetSOAInfo()
	{
		return soaInfo;
	}

	public void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default)
	{
		conduitUpdaters.Add(new ConduitUpdater
		{
			priority = priority,
			callback = callback
		});
		dirtyConduitUpdaters = true;
	}

	public void RemoveConduitUpdater(Action<float> callback)
	{
		for (int i = 0; i < conduitUpdaters.Count; i++)
		{
			if (conduitUpdaters[i].callback == callback)
			{
				conduitUpdaters.RemoveAt(i);
				dirtyConduitUpdaters = true;
				break;
			}
		}
	}

	public static int FlowBit(FlowDirection direction)
	{
		return 1 << (int)(direction - 1);
	}

	public SolidConduitFlow(int num_cells, IUtilityNetworkMgr network_mgr, float initial_elapsed_time)
	{
		elapsedTime = initial_elapsed_time;
		networkMgr = network_mgr;
		maskedOverlayLayer = LayerMask.NameToLayer("MaskedOverlay");
		Initialize(num_cells);
		network_mgr.AddNetworksRebuiltListener(OnUtilityNetworksRebuilt);
	}

	public void Initialize(int num_cells)
	{
		grid = new GridNode[num_cells];
		for (int i = 0; i < num_cells; i++)
		{
			grid[i].conduitIdx = -1;
			grid[i].contents.pickupableHandle = HandleVector<int>.InvalidHandle;
		}
	}

	private void OnUtilityNetworksRebuilt(IList<UtilityNetwork> networks, ICollection<int> root_nodes)
	{
		RebuildConnections(root_nodes);
		foreach (FlowUtilityNetwork network in networks)
		{
			ScanNetworkSources(network);
		}
		RefreshPaths();
	}

	private void RebuildConnections(IEnumerable<int> root_nodes)
	{
		soaInfo.Clear(this);
		pathList.Clear();
		ObjectLayer layer = ObjectLayer.SolidConduit;
		foreach (int root_node in root_nodes)
		{
			if (replacements.Contains(root_node))
			{
				replacements.Remove(root_node);
			}
			GameObject gameObject = Grid.Objects[root_node, (int)layer];
			if (!(gameObject == null))
			{
				int conduitIdx = soaInfo.AddConduit(this, gameObject, root_node);
				grid[root_node].conduitIdx = conduitIdx;
			}
		}
		Game.Instance.conduitTemperatureManager.Sim200ms(0f);
		foreach (int root_node2 in root_nodes)
		{
			UtilityConnections connections = networkMgr.GetConnections(root_node2, is_physical_building: true);
			if (connections != 0 && grid[root_node2].conduitIdx != -1)
			{
				int conduitIdx2 = grid[root_node2].conduitIdx;
				ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduitIdx2);
				int num = root_node2 - 1;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Left) != 0)
				{
					conduitConnections.left = grid[num].conduitIdx;
				}
				num = root_node2 + 1;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Right) != 0)
				{
					conduitConnections.right = grid[num].conduitIdx;
				}
				num = root_node2 - Grid.WidthInCells;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Down) != 0)
				{
					conduitConnections.down = grid[num].conduitIdx;
				}
				num = root_node2 + Grid.WidthInCells;
				if (Grid.IsValidCell(num) && (connections & UtilityConnections.Up) != 0)
				{
					conduitConnections.up = grid[num].conduitIdx;
				}
				soaInfo.SetConduitConnections(conduitIdx2, conduitConnections);
			}
		}
		if (this.onConduitsRebuilt != null)
		{
			this.onConduitsRebuilt();
		}
	}

	public void ScanNetworkSources(FlowUtilityNetwork network)
	{
		if (network != null)
		{
			for (int i = 0; i < network.sources.Count; i++)
			{
				FlowUtilityNetwork.IItem item = network.sources[i];
				path.Clear();
				visited.Clear();
				FindSinks(i, item.Cell);
			}
		}
	}

	public void RefreshPaths()
	{
		foreach (List<Conduit> path2 in pathList)
		{
			for (int i = 0; i < path2.Count - 1; i++)
			{
				Conduit conduit = path2[i];
				Conduit target_conduit = path2[i + 1];
				if (conduit.GetTargetFlowDirection(this) == FlowDirection.None)
				{
					FlowDirection direction = GetDirection(conduit, target_conduit);
					conduit.SetTargetFlowDirection(direction, this);
				}
			}
		}
	}

	private void FindSinks(int source_idx, int cell)
	{
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			FindSinksInternal(source_idx, gridNode.conduitIdx);
		}
	}

	private void FindSinksInternal(int source_idx, int conduit_idx)
	{
		if (visited.Contains(conduit_idx))
		{
			return;
		}
		visited.Add(conduit_idx);
		Conduit conduit = soaInfo.GetConduit(conduit_idx);
		if (conduit.GetPermittedFlowDirections(this) != -1)
		{
			path.Add(conduit);
			FlowUtilityNetwork.IItem item = (FlowUtilityNetwork.IItem)networkMgr.GetEndpoint(soaInfo.GetCell(conduit_idx));
			if (item != null && item.EndpointType == Endpoint.Sink)
			{
				FoundSink(source_idx);
			}
			ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit_idx);
			if (conduitConnections.down != -1)
			{
				FindSinksInternal(source_idx, conduitConnections.down);
			}
			if (conduitConnections.left != -1)
			{
				FindSinksInternal(source_idx, conduitConnections.left);
			}
			if (conduitConnections.right != -1)
			{
				FindSinksInternal(source_idx, conduitConnections.right);
			}
			if (conduitConnections.up != -1)
			{
				FindSinksInternal(source_idx, conduitConnections.up);
			}
			if (path.Count > 0)
			{
				path.RemoveAt(path.Count - 1);
			}
		}
	}

	private FlowDirection GetDirection(Conduit conduit, Conduit target_conduit)
	{
		ConduitConnections conduitConnections = soaInfo.GetConduitConnections(conduit.idx);
		if (conduitConnections.up == target_conduit.idx)
		{
			return FlowDirection.Up;
		}
		if (conduitConnections.down == target_conduit.idx)
		{
			return FlowDirection.Down;
		}
		if (conduitConnections.left == target_conduit.idx)
		{
			return FlowDirection.Left;
		}
		if (conduitConnections.right == target_conduit.idx)
		{
			return FlowDirection.Right;
		}
		return FlowDirection.None;
	}

	private void FoundSink(int source_idx)
	{
		for (int i = 0; i < path.Count - 1; i++)
		{
			FlowDirection direction = GetDirection(path[i], path[i + 1]);
			FlowDirection direction2 = InverseFlow(direction);
			int cellFromDirection = GetCellFromDirection(soaInfo.GetCell(path[i].idx), direction2);
			Conduit conduitFromDirection = soaInfo.GetConduitFromDirection(path[i].idx, direction2);
			if (i == 0 || (path[i].GetPermittedFlowDirections(this) & FlowBit(direction2)) == 0 || (cellFromDirection != soaInfo.GetCell(path[i - 1].idx) && (soaInfo.GetSrcFlowIdx(path[i].idx) == source_idx || (conduitFromDirection.GetPermittedFlowDirections(this) & FlowBit(direction2)) == 0)))
			{
				int permittedFlowDirections = path[i].GetPermittedFlowDirections(this);
				soaInfo.SetSrcFlowIdx(path[i].idx, source_idx);
				path[i].SetPermittedFlowDirections(permittedFlowDirections | FlowBit(direction), this);
				path[i].SetTargetFlowDirection(direction, this);
			}
		}
		for (int j = 1; j < path.Count; j++)
		{
			FlowDirection direction3 = GetDirection(path[j], path[j - 1]);
			soaInfo.SetSrcFlowDirection(path[j].idx, direction3);
		}
		List<Conduit> list = new List<Conduit>(path);
		list.Reverse();
		TryAdd(list);
	}

	private void TryAdd(List<Conduit> new_path)
	{
		foreach (List<Conduit> path2 in pathList)
		{
			if (path2.Count < new_path.Count)
			{
				continue;
			}
			bool flag = false;
			int num = path2.FindIndex((Conduit t) => t.idx == new_path[0].idx);
			int num2 = path2.FindIndex((Conduit t) => t.idx == new_path[new_path.Count - 1].idx);
			if (num != -1 && num2 != -1)
			{
				flag = true;
				int num3 = num;
				int num4 = 0;
				while (num3 < num2)
				{
					if (path2[num3].idx != new_path[num4].idx)
					{
						flag = false;
						break;
					}
					num3++;
					num4++;
				}
			}
			if (flag)
			{
				return;
			}
		}
		for (int num5 = pathList.Count - 1; num5 >= 0; num5--)
		{
			if (pathList[num5].Count <= 0)
			{
				pathList.RemoveAt(num5);
			}
		}
		for (int num6 = pathList.Count - 1; num6 >= 0; num6--)
		{
			List<Conduit> old_path = pathList[num6];
			if (new_path.Count >= old_path.Count)
			{
				bool flag2 = false;
				int num7 = new_path.FindIndex((Conduit t) => t.idx == old_path[0].idx);
				int num8 = new_path.FindIndex((Conduit t) => t.idx == old_path[old_path.Count - 1].idx);
				if (num7 != -1 && num8 != -1)
				{
					flag2 = true;
					int num9 = num7;
					int num10 = 0;
					while (num9 < num8)
					{
						if (new_path[num9].idx != old_path[num10].idx)
						{
							flag2 = false;
							break;
						}
						num9++;
						num10++;
					}
				}
				if (flag2)
				{
					pathList.RemoveAt(num6);
				}
			}
		}
		foreach (List<Conduit> path3 in pathList)
		{
			for (int num11 = new_path.Count - 1; num11 >= 0; num11--)
			{
				Conduit new_conduit = new_path[num11];
				if (path3.FindIndex((Conduit t) => t.idx == new_conduit.idx) != -1 && Mathf.IsPowerOfTwo(soaInfo.GetPermittedFlowDirections(new_conduit.idx)))
				{
					new_path.RemoveAt(num11);
				}
			}
		}
		pathList.Add(new_path);
	}

	public ConduitContents GetContents(int cell)
	{
		ConduitContents contents = grid[cell].contents;
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			contents = soaInfo.GetConduit(gridNode.conduitIdx).GetContents(this);
		}
		return contents;
	}

	private void SetContents(int cell, ConduitContents contents)
	{
		GridNode gridNode = grid[cell];
		if (gridNode.conduitIdx != -1)
		{
			soaInfo.GetConduit(gridNode.conduitIdx).SetContents(this, contents);
		}
		else
		{
			grid[cell].contents = contents;
		}
	}

	public void SetContents(int cell, Pickupable pickupable)
	{
		ConduitContents conduitContents = default(ConduitContents);
		conduitContents.pickupableHandle = HandleVector<int>.InvalidHandle;
		ConduitContents contents = conduitContents;
		if (pickupable != null)
		{
			KBatchedAnimController component = pickupable.GetComponent<KBatchedAnimController>();
			StoredInfo storedInfo = default(StoredInfo);
			storedInfo.kbac = component;
			storedInfo.pickupable = pickupable;
			StoredInfo initial_data = storedInfo;
			contents.pickupableHandle = conveyorPickupables.Allocate(initial_data);
			KBatchedAnimController component2 = pickupable.GetComponent<KBatchedAnimController>();
			component2.enabled = false;
			component2.enabled = true;
			pickupable.Trigger(856640610, true);
		}
		SetContents(cell, contents);
	}

	public static int GetCellFromDirection(int cell, FlowDirection direction)
	{
		return direction switch
		{
			FlowDirection.Left => Grid.CellLeft(cell), 
			FlowDirection.Right => Grid.CellRight(cell), 
			FlowDirection.Up => Grid.CellAbove(cell), 
			FlowDirection.Down => Grid.CellBelow(cell), 
			_ => -1, 
		};
	}

	public static FlowDirection InverseFlow(FlowDirection direction)
	{
		return direction switch
		{
			FlowDirection.Left => FlowDirection.Right, 
			FlowDirection.Right => FlowDirection.Left, 
			FlowDirection.Up => FlowDirection.Down, 
			FlowDirection.Down => FlowDirection.Up, 
			_ => FlowDirection.None, 
		};
	}

	public void Sim200ms(float dt)
	{
		if (dt <= 0f)
		{
			return;
		}
		elapsedTime += dt;
		if (elapsedTime < 1f)
		{
			return;
		}
		float obj = 1f;
		elapsedTime -= 1f;
		lastUpdateTime = Time.time;
		soaInfo.BeginFrame(this);
		foreach (List<Conduit> path2 in pathList)
		{
			foreach (Conduit item in path2)
			{
				UpdateConduit(item);
			}
		}
		soaInfo.UpdateFlowDirection(this);
		if (dirtyConduitUpdaters)
		{
			conduitUpdaters.Sort((ConduitUpdater a, ConduitUpdater b) => a.priority - b.priority);
		}
		soaInfo.EndFrame(this);
		for (int i = 0; i < conduitUpdaters.Count; i++)
		{
			conduitUpdaters[i].callback(obj);
		}
	}

	public void RenderEveryTick(float dt)
	{
		for (int i = 0; i < GetSOAInfo().NumEntries; i++)
		{
			Conduit conduit = GetSOAInfo().GetConduit(i);
			ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(this);
			if (lastFlowInfo.direction == FlowDirection.None)
			{
				continue;
			}
			int cell = conduit.GetCell(this);
			int cellFromDirection = GetCellFromDirection(cell, lastFlowInfo.direction);
			ConduitContents contents = GetContents(cellFromDirection);
			if (contents.pickupableHandle.IsValid())
			{
				Vector3 a = Grid.CellToPosCCC(cell, Grid.SceneLayer.SolidConduitContents);
				Vector3 b = Grid.CellToPosCCC(cellFromDirection, Grid.SceneLayer.SolidConduitContents);
				Vector3 position = Vector3.Lerp(a, b, ContinuousLerpPercent);
				Pickupable pickupable = GetPickupable(contents.pickupableHandle);
				if (pickupable != null)
				{
					pickupable.transform.SetPosition(position);
				}
			}
		}
	}

	private void UpdateConduit(Conduit conduit)
	{
		if (soaInfo.GetUpdated(conduit.idx))
		{
			return;
		}
		if (soaInfo.GetSrcFlowDirection(conduit.idx) == FlowDirection.None)
		{
			soaInfo.SetSrcFlowDirection(conduit.idx, conduit.GetNextFlowSource(this));
		}
		int cell = soaInfo.GetCell(conduit.idx);
		ConduitContents contents = grid[cell].contents;
		if (!contents.pickupableHandle.IsValid())
		{
			return;
		}
		FlowDirection targetFlowDirection = soaInfo.GetTargetFlowDirection(conduit.idx);
		Conduit conduitFromDirection = soaInfo.GetConduitFromDirection(conduit.idx, targetFlowDirection);
		if (conduitFromDirection.idx == -1)
		{
			soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
			return;
		}
		int cell2 = soaInfo.GetCell(conduitFromDirection.idx);
		ConduitContents contents2 = grid[cell2].contents;
		if (contents2.pickupableHandle.IsValid())
		{
			soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
			return;
		}
		if ((soaInfo.GetPermittedFlowDirections(conduit.idx) & FlowBit(targetFlowDirection)) != 0)
		{
			bool flag = false;
			for (int i = 0; i < 5; i++)
			{
				Conduit conduitFromDirection2 = soaInfo.GetConduitFromDirection(conduitFromDirection.idx, soaInfo.GetSrcFlowDirection(conduitFromDirection.idx));
				if (conduitFromDirection2.idx == conduit.idx)
				{
					flag = true;
					break;
				}
				if (conduitFromDirection2.idx != -1)
				{
					int cell3 = soaInfo.GetCell(conduitFromDirection2.idx);
					ConduitContents contents3 = grid[cell3].contents;
					if (contents3.pickupableHandle.IsValid())
					{
						break;
					}
				}
				soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
			}
			if (flag && !contents2.pickupableHandle.IsValid())
			{
				ConduitContents contents4 = RemoveFromGrid(conduit);
				AddToGrid(cell2, contents4);
				soaInfo.SetLastFlowInfo(conduit.idx, soaInfo.GetTargetFlowDirection(conduit.idx));
				soaInfo.SetUpdated(conduitFromDirection.idx, is_updated: true);
				soaInfo.SetSrcFlowDirection(conduitFromDirection.idx, conduitFromDirection.GetNextFlowSource(this));
			}
		}
		soaInfo.SetTargetFlowDirection(conduit.idx, conduit.GetNextFlowTarget(this));
	}

	private void AddToGrid(int cell_idx, ConduitContents contents)
	{
		grid[cell_idx].contents = contents;
	}

	private ConduitContents RemoveFromGrid(Conduit conduit)
	{
		int cell = soaInfo.GetCell(conduit.idx);
		ConduitContents contents = grid[cell].contents;
		ConduitContents contents2 = ConduitContents.EmptyContents();
		grid[cell].contents = contents2;
		return contents;
	}

	public void AddPickupable(int cell_idx, Pickupable pickupable)
	{
		if (grid[cell_idx].conduitIdx == -1)
		{
			Debug.LogWarning("No conduit in cell: " + cell_idx);
			DumpPickupable(pickupable);
			return;
		}
		ConduitContents contents = GetConduit(cell_idx).GetContents(this);
		if (contents.pickupableHandle.IsValid())
		{
			Debug.LogWarning("Conduit already full: " + cell_idx);
			DumpPickupable(pickupable);
			return;
		}
		KBatchedAnimController component = pickupable.GetComponent<KBatchedAnimController>();
		StoredInfo storedInfo = default(StoredInfo);
		storedInfo.kbac = component;
		storedInfo.pickupable = pickupable;
		StoredInfo initial_data = storedInfo;
		contents.pickupableHandle = conveyorPickupables.Allocate(initial_data);
		if (viewingConduits)
		{
			ApplyOverlayVisualization(component);
		}
		if ((bool)pickupable.storage)
		{
			pickupable.storage.Remove(pickupable.gameObject);
		}
		pickupable.Trigger(856640610, true);
		SetContents(cell_idx, contents);
	}

	public Pickupable RemovePickupable(int cell_idx)
	{
		Pickupable pickupable = null;
		Conduit conduit = GetConduit(cell_idx);
		if (conduit.idx != -1)
		{
			ConduitContents conduitContents = RemoveFromGrid(conduit);
			if (conduitContents.pickupableHandle.IsValid())
			{
				StoredInfo data = conveyorPickupables.GetData(conduitContents.pickupableHandle);
				ClearOverlayVisualization(data.kbac);
				pickupable = data.pickupable;
				if ((bool)pickupable)
				{
					pickupable.Trigger(856640610, false);
				}
				freedHandles.Add(conduitContents.pickupableHandle);
			}
		}
		return pickupable;
	}

	public int GetPermittedFlow(int cell)
	{
		Conduit conduit = GetConduit(cell);
		if (conduit.idx == -1)
		{
			return 0;
		}
		return soaInfo.GetPermittedFlowDirections(conduit.idx);
	}

	public bool HasConduit(int cell)
	{
		return grid[cell].conduitIdx != -1;
	}

	public Conduit GetConduit(int cell)
	{
		int conduitIdx = grid[cell].conduitIdx;
		if (conduitIdx == -1)
		{
			return Conduit.Invalid();
		}
		return soaInfo.GetConduit(conduitIdx);
	}

	private void DumpPipeContents(int cell)
	{
		Pickupable pickupable = RemovePickupable(cell);
		if ((bool)pickupable)
		{
			pickupable.transform.parent = null;
		}
	}

	private void DumpPickupable(Pickupable pickupable)
	{
		if ((bool)pickupable)
		{
			pickupable.transform.parent = null;
		}
	}

	public void EmptyConduit(int cell)
	{
		if (!replacements.Contains(cell))
		{
			DumpPipeContents(cell);
		}
	}

	public void MarkForReplacement(int cell)
	{
		replacements.Add(cell);
	}

	public void DeactivateCell(int cell)
	{
		grid[cell].conduitIdx = -1;
		ConduitContents contents = ConduitContents.EmptyContents();
		SetContents(cell, contents);
	}

	public UtilityNetwork GetNetwork(Conduit conduit)
	{
		int cell = soaInfo.GetCell(conduit.idx);
		return networkMgr.GetNetworkForCell(cell);
	}

	public void ForceRebuildNetworks()
	{
		networkMgr.ForceRebuildNetworks();
	}

	public bool IsConduitFull(int cell_idx)
	{
		ConduitContents contents = grid[cell_idx].contents;
		return contents.pickupableHandle.IsValid();
	}

	public bool IsConduitEmpty(int cell_idx)
	{
		ConduitContents contents = grid[cell_idx].contents;
		return !contents.pickupableHandle.IsValid();
	}

	public void Initialize()
	{
		if (OverlayScreen.Instance != null)
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
			OverlayScreen instance2 = OverlayScreen.Instance;
			instance2.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance2.OnOverlayChanged, new Action<HashedString>(OnOverlayChanged));
		}
	}

	private void OnOverlayChanged(HashedString mode)
	{
		bool flag = mode == OverlayModes.SolidConveyor.ID;
		if (flag == viewingConduits)
		{
			return;
		}
		viewingConduits = flag;
		int layer = (viewingConduits ? maskedOverlayLayer : Game.PickupableLayer);
		Color32 tintColour = (viewingConduits ? OverlayColour : NormalColour);
		List<StoredInfo> dataList = conveyorPickupables.GetDataList();
		for (int i = 0; i < dataList.Count; i++)
		{
			StoredInfo storedInfo = dataList[i];
			if (storedInfo.kbac != null)
			{
				storedInfo.kbac.SetLayer(layer);
				storedInfo.kbac.TintColour = tintColour;
			}
		}
	}

	private void ApplyOverlayVisualization(KBatchedAnimController kbac)
	{
		if (!(kbac == null))
		{
			kbac.SetLayer(maskedOverlayLayer);
			kbac.TintColour = OverlayColour;
		}
	}

	private void ClearOverlayVisualization(KBatchedAnimController kbac)
	{
		if (!(kbac == null))
		{
			kbac.SetLayer(Game.PickupableLayer);
			kbac.TintColour = NormalColour;
		}
	}

	public Pickupable GetPickupable(HandleVector<int>.Handle h)
	{
		Pickupable result = null;
		if (h.IsValid())
		{
			result = conveyorPickupables.GetData(h).pickupable;
		}
		return result;
	}
}
