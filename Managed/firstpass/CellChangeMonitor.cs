using System;
using System.Collections.Generic;
using UnityEngine;

public class CellChangeMonitor : Singleton<CellChangeMonitor>
{
	private struct CellChangedEntry
	{
		public struct Handler
		{
			public string name;

			public System.Action callback;
		}

		public Transform transform;

		public List<Handler> handlers;
	}

	private struct MovementStateChangedEntry
	{
		public Transform transform;

		public List<Action<Transform, bool>> handlers;
	}

	private Dictionary<int, CellChangedEntry> cellChangedHandlers = new Dictionary<int, CellChangedEntry>();

	private Dictionary<int, MovementStateChangedEntry> movementStateChangedHandlers = new Dictionary<int, MovementStateChangedEntry>();

	private HashSet<int> pendingDirtyTransforms = new HashSet<int>();

	private HashSet<int> dirtyTransforms = new HashSet<int>();

	private HashSet<int> movingTransforms = new HashSet<int>();

	private HashSet<int> previouslyMovingTransforms = new HashSet<int>();

	private Dictionary<int, int> transformLastKnownCell = new Dictionary<int, int>();

	private List<CellChangedEntry.Handler> cellChangedCallbacksToRun = new List<CellChangedEntry.Handler>();

	private List<Action<Transform, bool>> moveChangedCallbacksToRun = new List<Action<Transform, bool>>();

	private int gridWidth;

	public void MarkDirty(Transform transform)
	{
		if (gridWidth != 0)
		{
			pendingDirtyTransforms.Add(transform.GetInstanceID());
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				MarkDirty(transform.GetChild(i));
			}
		}
	}

	public bool IsMoving(Transform transform)
	{
		return movingTransforms.Contains(transform.GetInstanceID());
	}

	public void RegisterMovementStateChanged(Transform transform, Action<Transform, bool> handler)
	{
		int instanceID = transform.GetInstanceID();
		MovementStateChangedEntry value = default(MovementStateChangedEntry);
		if (!movementStateChangedHandlers.TryGetValue(instanceID, out value))
		{
			value = default(MovementStateChangedEntry);
			value.handlers = new List<Action<Transform, bool>>();
			value.transform = transform;
		}
		value.handlers.Add(handler);
		movementStateChangedHandlers[instanceID] = value;
	}

	public void UnregisterMovementStateChanged(int instance_id, Action<Transform, bool> callback)
	{
		MovementStateChangedEntry value = default(MovementStateChangedEntry);
		if (movementStateChangedHandlers.TryGetValue(instance_id, out value))
		{
			value.handlers.Remove(callback);
			if (value.handlers.Count == 0)
			{
				movementStateChangedHandlers.Remove(instance_id);
			}
		}
	}

	public void UnregisterMovementStateChanged(Transform transform, Action<Transform, bool> callback)
	{
		UnregisterMovementStateChanged(transform.GetInstanceID(), callback);
	}

	public int RegisterCellChangedHandler(Transform transform, System.Action callback, string debug_name)
	{
		int instanceID = transform.GetInstanceID();
		CellChangedEntry value = default(CellChangedEntry);
		if (!cellChangedHandlers.TryGetValue(instanceID, out value))
		{
			value = default(CellChangedEntry);
			value.transform = transform;
			value.handlers = new List<CellChangedEntry.Handler>();
		}
		CellChangedEntry.Handler handler = default(CellChangedEntry.Handler);
		handler.name = debug_name;
		handler.callback = callback;
		CellChangedEntry.Handler item = handler;
		value.handlers.Add(item);
		cellChangedHandlers[instanceID] = value;
		return instanceID;
	}

	public void UnregisterCellChangedHandler(int instance_id, System.Action callback)
	{
		CellChangedEntry value = default(CellChangedEntry);
		if (!cellChangedHandlers.TryGetValue(instance_id, out value))
		{
			return;
		}
		for (int i = 0; i < value.handlers.Count; i++)
		{
			if (!(value.handlers[i].callback != callback))
			{
				value.handlers.RemoveAt(i);
				break;
			}
		}
		if (value.handlers.Count == 0)
		{
			cellChangedHandlers.Remove(instance_id);
		}
	}

	public void UnregisterCellChangedHandler(Transform transform, System.Action callback)
	{
		UnregisterCellChangedHandler(transform.GetInstanceID(), callback);
	}

	public int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		int num = (int)(pos.y + 0.05f);
		int num2 = (int)x;
		return num * gridWidth + num2;
	}

	public void SetGridSize(int grid_width, int grid_height)
	{
		gridWidth = grid_width;
	}

	public void RenderEveryTick()
	{
		HashSet<int> hashSet = pendingDirtyTransforms;
		pendingDirtyTransforms = dirtyTransforms;
		dirtyTransforms = hashSet;
		pendingDirtyTransforms.Clear();
		previouslyMovingTransforms.Clear();
		hashSet = previouslyMovingTransforms;
		previouslyMovingTransforms = movingTransforms;
		movingTransforms = hashSet;
		foreach (int dirtyTransform in dirtyTransforms)
		{
			CellChangedEntry value = default(CellChangedEntry);
			if (cellChangedHandlers.TryGetValue(dirtyTransform, out value))
			{
				if (value.transform == null)
				{
					continue;
				}
				int value2 = -1;
				transformLastKnownCell.TryGetValue(dirtyTransform, out value2);
				int num = PosToCell(value.transform.GetPosition());
				if (value2 != num)
				{
					cellChangedCallbacksToRun.Clear();
					cellChangedCallbacksToRun.AddRange(value.handlers);
					foreach (CellChangedEntry.Handler item in cellChangedCallbacksToRun)
					{
						foreach (CellChangedEntry.Handler handler in value.handlers)
						{
							if (handler.callback == item.callback)
							{
								handler.callback();
								break;
							}
						}
					}
					transformLastKnownCell[dirtyTransform] = num;
				}
			}
			movingTransforms.Add(dirtyTransform);
			if (!previouslyMovingTransforms.Contains(dirtyTransform))
			{
				RunMovementStateChangedCallbacks(dirtyTransform, state: true);
			}
		}
		foreach (int previouslyMovingTransform in previouslyMovingTransforms)
		{
			if (!movingTransforms.Contains(previouslyMovingTransform))
			{
				RunMovementStateChangedCallbacks(previouslyMovingTransform, state: false);
			}
		}
		dirtyTransforms.Clear();
	}

	private void RunMovementStateChangedCallbacks(int instance_id, bool state)
	{
		MovementStateChangedEntry value = default(MovementStateChangedEntry);
		if (!movementStateChangedHandlers.TryGetValue(instance_id, out value))
		{
			return;
		}
		moveChangedCallbacksToRun.Clear();
		moveChangedCallbacksToRun.AddRange(value.handlers);
		foreach (Action<Transform, bool> item in moveChangedCallbacksToRun)
		{
			if (value.handlers.Contains(item))
			{
				item(value.transform, state);
			}
		}
	}

	private void Validate()
	{
	}
}
