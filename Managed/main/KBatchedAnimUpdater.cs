using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimUpdater : Singleton<KBatchedAnimUpdater>
{
	public enum RegistrationState
	{
		Registered,
		PendingRemoval,
		Unregistered
	}

	private struct RegistrationInfo
	{
		public bool register;

		public int transformId;

		public int controllerInstanceId;

		public KBatchedAnimController controller;
	}

	private struct ControllerChunkInfo
	{
		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}

	private class MovingControllerInfo
	{
		public int controllerInstanceId;

		public KBatchedAnimController controller;

		public Vector2I chunkXY;
	}

	private const int VISIBLE_BORDER = 4;

	public static readonly Vector2I INVALID_CHUNK_ID = Vector2I.minusone;

	private Dictionary<int, KBatchedAnimController>[,] controllerGrid;

	private LinkedList<KBatchedAnimController> updateList = new LinkedList<KBatchedAnimController>();

	private LinkedList<KBatchedAnimController> alwaysUpdateList = new LinkedList<KBatchedAnimController>();

	private bool[,] visibleChunkGrid;

	private bool[,] previouslyVisibleChunkGrid;

	private List<Vector2I> visibleChunks = new List<Vector2I>();

	private List<Vector2I> previouslyVisibleChunks = new List<Vector2I>();

	private Vector2I vis_chunk_min = Vector2I.zero;

	private Vector2I vis_chunk_max = Vector2I.zero;

	private List<RegistrationInfo> queuedRegistrations = new List<RegistrationInfo>();

	private Dictionary<int, ControllerChunkInfo> controllerChunkInfos = new Dictionary<int, ControllerChunkInfo>();

	private Dictionary<int, MovingControllerInfo> movingControllerInfos = new Dictionary<int, MovingControllerInfo>();

	private const int CHUNKS_TO_CLEAN_PER_TICK = 16;

	private int cleanUpChunkIndex;

	private static readonly Vector2 VISIBLE_RANGE_SCALE = new Vector2(1.5f, 1.5f);

	public void InitializeGrid()
	{
		Clear();
		Vector2I visibleSize = GetVisibleSize();
		int num = (visibleSize.x + 32 - 1) / 32;
		int num2 = (visibleSize.y + 32 - 1) / 32;
		controllerGrid = new Dictionary<int, KBatchedAnimController>[num, num2];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				controllerGrid[j, i] = new Dictionary<int, KBatchedAnimController>();
			}
		}
		visibleChunks.Clear();
		previouslyVisibleChunks.Clear();
		previouslyVisibleChunkGrid = new bool[num, num2];
		visibleChunkGrid = new bool[num, num2];
	}

	public Vector2I GetVisibleSize()
	{
		if (CameraController.Instance != null)
		{
			CameraController.Instance.GetWorldCamera(out var worldOffset, out var worldSize);
			return new Vector2I((int)((float)(worldSize.x + worldOffset.x) * VISIBLE_RANGE_SCALE.x), (int)((float)(worldSize.y + worldOffset.y) * VISIBLE_RANGE_SCALE.y));
		}
		return new Vector2I((int)((float)Grid.WidthInCells * VISIBLE_RANGE_SCALE.x), (int)((float)Grid.HeightInCells * VISIBLE_RANGE_SCALE.y));
	}

	public void Clear()
	{
		foreach (KBatchedAnimController update in updateList)
		{
			if (update != null)
			{
				UnityEngine.Object.DestroyImmediate(update);
			}
		}
		updateList.Clear();
		foreach (KBatchedAnimController alwaysUpdate in alwaysUpdateList)
		{
			if (alwaysUpdate != null)
			{
				UnityEngine.Object.DestroyImmediate(alwaysUpdate);
			}
		}
		alwaysUpdateList.Clear();
		queuedRegistrations.Clear();
		visibleChunks.Clear();
		previouslyVisibleChunks.Clear();
		controllerGrid = null;
		previouslyVisibleChunkGrid = null;
		visibleChunkGrid = null;
	}

	public void UpdateRegister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case RegistrationState.PendingRemoval:
			controller.updateRegistrationState = RegistrationState.Registered;
			break;
		case RegistrationState.Unregistered:
			((controller.visibilityType == KAnimControllerBase.VisibilityType.Always) ? alwaysUpdateList : updateList).AddLast(controller);
			controller.updateRegistrationState = RegistrationState.Registered;
			break;
		case RegistrationState.Registered:
			break;
		}
	}

	public void UpdateUnregister(KBatchedAnimController controller)
	{
		switch (controller.updateRegistrationState)
		{
		case RegistrationState.Registered:
			controller.updateRegistrationState = RegistrationState.PendingRemoval;
			break;
		case RegistrationState.PendingRemoval:
		case RegistrationState.Unregistered:
			break;
		}
	}

	public void VisibilityRegister(KBatchedAnimController controller)
	{
		queuedRegistrations.Add(new RegistrationInfo
		{
			transformId = controller.transform.GetInstanceID(),
			controllerInstanceId = controller.GetInstanceID(),
			controller = controller,
			register = true
		});
	}

	public void VisibilityUnregister(KBatchedAnimController controller)
	{
		if (!App.IsExiting)
		{
			queuedRegistrations.Add(new RegistrationInfo
			{
				transformId = controller.transform.GetInstanceID(),
				controllerInstanceId = controller.GetInstanceID(),
				controller = controller,
				register = false
			});
		}
	}

	private Dictionary<int, KBatchedAnimController> GetControllerMap(Vector2I chunk_xy)
	{
		Dictionary<int, KBatchedAnimController> result = null;
		if (controllerGrid != null && 0 <= chunk_xy.x && chunk_xy.x < controllerGrid.GetLength(0) && 0 <= chunk_xy.y && chunk_xy.y < controllerGrid.GetLength(1))
		{
			result = controllerGrid[chunk_xy.x, chunk_xy.y];
		}
		return result;
	}

	public void LateUpdate()
	{
		ProcessMovingAnims();
		UpdateVisibility();
		ProcessRegistrations();
		CleanUp();
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		_ = alwaysUpdateList.Count;
		UpdateRegisteredAnims(alwaysUpdateList, unscaledDeltaTime);
		if (DoGridProcessing())
		{
			unscaledDeltaTime = Time.deltaTime;
			if (unscaledDeltaTime > 0f)
			{
				_ = updateList.Count;
				UpdateRegisteredAnims(updateList, unscaledDeltaTime);
			}
		}
	}

	private static void UpdateRegisteredAnims(LinkedList<KBatchedAnimController> list, float dt)
	{
		LinkedListNode<KBatchedAnimController> linkedListNode = list.First;
		while (linkedListNode != null)
		{
			LinkedListNode<KBatchedAnimController> next = linkedListNode.Next;
			KBatchedAnimController value = linkedListNode.Value;
			if (value == null)
			{
				list.Remove(linkedListNode);
			}
			else if (value.updateRegistrationState != 0)
			{
				value.updateRegistrationState = RegistrationState.Unregistered;
				list.Remove(linkedListNode);
			}
			else if (value.forceUseGameTime)
			{
				value.UpdateAnim(Time.deltaTime);
			}
			else
			{
				value.UpdateAnim(dt);
			}
			linkedListNode = next;
		}
	}

	public bool IsChunkVisible(Vector2I chunk_xy)
	{
		return visibleChunkGrid[chunk_xy.x, chunk_xy.y];
	}

	public void GetVisibleArea(out Vector2I vis_chunk_min, out Vector2I vis_chunk_max)
	{
		vis_chunk_min = this.vis_chunk_min;
		vis_chunk_max = this.vis_chunk_max;
	}

	public static Vector2I PosToChunkXY(Vector3 pos)
	{
		return KAnimBatchManager.CellXYToChunkXY(Grid.PosToXY(pos));
	}

	private void UpdateVisibility()
	{
		if (!DoGridProcessing())
		{
			return;
		}
		GetVisibleCellRange(out var min, out var max);
		vis_chunk_min = new Vector2I(min.x / 32, min.y / 32);
		vis_chunk_max = new Vector2I(max.x / 32, max.y / 32);
		vis_chunk_max.x = Math.Min(vis_chunk_max.x, controllerGrid.GetLength(0) - 1);
		vis_chunk_max.y = Math.Min(vis_chunk_max.y, controllerGrid.GetLength(1) - 1);
		bool[,] array = previouslyVisibleChunkGrid;
		previouslyVisibleChunkGrid = visibleChunkGrid;
		visibleChunkGrid = array;
		Array.Clear(visibleChunkGrid, 0, visibleChunkGrid.Length);
		List<Vector2I> list = previouslyVisibleChunks;
		previouslyVisibleChunks = visibleChunks;
		visibleChunks = list;
		visibleChunks.Clear();
		for (int i = vis_chunk_min.y; i <= vis_chunk_max.y; i++)
		{
			for (int j = vis_chunk_min.x; j <= vis_chunk_max.x; j++)
			{
				visibleChunkGrid[j, i] = true;
				visibleChunks.Add(new Vector2I(j, i));
				if (previouslyVisibleChunkGrid[j, i])
				{
					continue;
				}
				foreach (KeyValuePair<int, KBatchedAnimController> item in controllerGrid[j, i])
				{
					KBatchedAnimController value = item.Value;
					if (!(value == null))
					{
						value.SetVisiblity(is_visible: true);
					}
				}
			}
		}
		for (int k = 0; k < previouslyVisibleChunks.Count; k++)
		{
			Vector2I vector2I = previouslyVisibleChunks[k];
			if (visibleChunkGrid[vector2I.x, vector2I.y])
			{
				continue;
			}
			foreach (KeyValuePair<int, KBatchedAnimController> item2 in controllerGrid[vector2I.x, vector2I.y])
			{
				KBatchedAnimController value2 = item2.Value;
				if (!(value2 == null))
				{
					value2.SetVisiblity(is_visible: false);
				}
			}
		}
	}

	private void ProcessMovingAnims()
	{
		foreach (MovingControllerInfo value2 in movingControllerInfos.Values)
		{
			if (value2.controller == null)
			{
				continue;
			}
			Vector2I vector2I = PosToChunkXY(value2.controller.PositionIncludingOffset);
			if (value2.chunkXY != vector2I)
			{
				ControllerChunkInfo value = default(ControllerChunkInfo);
				DebugUtil.Assert(controllerChunkInfos.TryGetValue(value2.controllerInstanceId, out value));
				DebugUtil.Assert(value2.controller == value.controller);
				DebugUtil.Assert(value.chunkXY == value2.chunkXY);
				Dictionary<int, KBatchedAnimController> controllerMap = GetControllerMap(value.chunkXY);
				if (controllerMap != null)
				{
					DebugUtil.Assert(controllerMap.ContainsKey(value2.controllerInstanceId));
					controllerMap.Remove(value2.controllerInstanceId);
				}
				controllerMap = GetControllerMap(vector2I);
				if (controllerMap != null)
				{
					DebugUtil.Assert(!controllerMap.ContainsKey(value2.controllerInstanceId));
					controllerMap[value2.controllerInstanceId] = value.controller;
				}
				value2.chunkXY = vector2I;
				value.chunkXY = vector2I;
				controllerChunkInfos[value2.controllerInstanceId] = value;
				if (controllerMap != null)
				{
					value.controller.SetVisiblity(visibleChunkGrid[vector2I.x, vector2I.y]);
				}
				else
				{
					value.controller.SetVisiblity(is_visible: false);
				}
			}
		}
	}

	private void ProcessRegistrations()
	{
		for (int i = 0; i < queuedRegistrations.Count; i++)
		{
			RegistrationInfo registrationInfo = queuedRegistrations[i];
			if (registrationInfo.register)
			{
				if (!(registrationInfo.controller == null))
				{
					int instanceID = registrationInfo.controller.GetInstanceID();
					DebugUtil.Assert(!controllerChunkInfos.ContainsKey(instanceID));
					ControllerChunkInfo controllerChunkInfo = default(ControllerChunkInfo);
					controllerChunkInfo.controller = registrationInfo.controller;
					controllerChunkInfo.chunkXY = PosToChunkXY(registrationInfo.controller.PositionIncludingOffset);
					ControllerChunkInfo value = controllerChunkInfo;
					controllerChunkInfos[instanceID] = value;
					Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(registrationInfo.controller.transform, OnMovementStateChanged);
					Dictionary<int, KBatchedAnimController> controllerMap = GetControllerMap(value.chunkXY);
					if (controllerMap != null)
					{
						DebugUtil.Assert(!controllerMap.ContainsKey(instanceID));
						controllerMap.Add(instanceID, registrationInfo.controller);
					}
					if (Singleton<CellChangeMonitor>.Instance.IsMoving(registrationInfo.controller.transform))
					{
						DebugUtil.DevAssertArgs(!movingControllerInfos.ContainsKey(instanceID), "Readding controller which is already moving", registrationInfo.controller.name, value.chunkXY, movingControllerInfos.ContainsKey(instanceID) ? movingControllerInfos[instanceID].chunkXY.ToString() : null);
						movingControllerInfos[instanceID] = new MovingControllerInfo
						{
							controllerInstanceId = instanceID,
							controller = registrationInfo.controller,
							chunkXY = value.chunkXY
						};
					}
					if (controllerMap != null && visibleChunkGrid[value.chunkXY.x, value.chunkXY.y])
					{
						registrationInfo.controller.SetVisiblity(is_visible: true);
					}
				}
				continue;
			}
			ControllerChunkInfo value2 = default(ControllerChunkInfo);
			if (!controllerChunkInfos.TryGetValue(registrationInfo.controllerInstanceId, out value2))
			{
				continue;
			}
			if (registrationInfo.controller != null)
			{
				Dictionary<int, KBatchedAnimController> controllerMap2 = GetControllerMap(value2.chunkXY);
				if (controllerMap2 != null)
				{
					DebugUtil.Assert(controllerMap2.ContainsKey(registrationInfo.controllerInstanceId));
					controllerMap2.Remove(registrationInfo.controllerInstanceId);
				}
				registrationInfo.controller.SetVisiblity(is_visible: false);
			}
			movingControllerInfos.Remove(registrationInfo.controllerInstanceId);
			Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(registrationInfo.transformId, OnMovementStateChanged);
			controllerChunkInfos.Remove(registrationInfo.controllerInstanceId);
		}
		queuedRegistrations.Clear();
	}

	public void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		if (!(transform == null))
		{
			KBatchedAnimController component = transform.GetComponent<KBatchedAnimController>();
			int instanceID = component.GetInstanceID();
			ControllerChunkInfo value = default(ControllerChunkInfo);
			DebugUtil.Assert(controllerChunkInfos.TryGetValue(instanceID, out value));
			if (is_moving)
			{
				DebugUtil.DevAssertArgs(!movingControllerInfos.ContainsKey(instanceID), "Readding controller which is already moving", component.name, value.chunkXY, movingControllerInfos.ContainsKey(instanceID) ? movingControllerInfos[instanceID].chunkXY.ToString() : null);
				movingControllerInfos[instanceID] = new MovingControllerInfo
				{
					controllerInstanceId = instanceID,
					controller = component,
					chunkXY = value.chunkXY
				};
			}
			else
			{
				movingControllerInfos.Remove(instanceID);
			}
		}
	}

	private void CleanUp()
	{
		if (!DoGridProcessing())
		{
			return;
		}
		int length = controllerGrid.GetLength(0);
		for (int i = 0; i < 16; i++)
		{
			int num = (cleanUpChunkIndex + i) % controllerGrid.Length;
			int num2 = num % length;
			int num3 = num / length;
			Dictionary<int, KBatchedAnimController> dictionary = controllerGrid[num2, num3];
			ListPool<int, KBatchedAnimUpdater>.PooledList pooledList = ListPool<int, KBatchedAnimUpdater>.Allocate();
			foreach (KeyValuePair<int, KBatchedAnimController> item in dictionary)
			{
				if (item.Value == null)
				{
					pooledList.Add(item.Key);
				}
			}
			foreach (int item2 in pooledList)
			{
				dictionary.Remove(item2);
			}
			pooledList.Recycle();
		}
		cleanUpChunkIndex = (cleanUpChunkIndex + 16) % controllerGrid.Length;
	}

	public static void GetVisibleCellRange(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
		min.x -= 4;
		min.y -= 4;
		if (CameraController.Instance != null && DlcManager.IsExpansion1Active())
		{
			CameraController.Instance.GetWorldCamera(out var worldOffset, out var worldSize);
			min.x = Math.Min(worldOffset.x + worldSize.x - 1, Math.Max(worldOffset.x, min.x));
			min.y = Math.Min(worldOffset.y + worldSize.y - 1, Math.Max(worldOffset.y, min.y));
			max.x += 4;
			max.y += 4;
			max.x = Math.Min(worldOffset.x + worldSize.x - 1, Math.Max(worldOffset.x, max.x));
			max.y = Math.Min(worldOffset.y + worldSize.y - 1, Math.Max(worldOffset.y, max.y));
		}
		else
		{
			min.x = Math.Min((int)((float)Grid.WidthInCells * VISIBLE_RANGE_SCALE.x) - 1, Math.Max(0, min.x));
			min.y = Math.Min((int)((float)Grid.HeightInCells * VISIBLE_RANGE_SCALE.y) - 1, Math.Max(0, min.y));
			max.x += 4;
			max.y += 4;
			max.x = Math.Min((int)((float)Grid.WidthInCells * VISIBLE_RANGE_SCALE.x) - 1, Math.Max(0, max.x));
			max.y = Math.Min((int)((float)Grid.HeightInCells * VISIBLE_RANGE_SCALE.y) - 1, Math.Max(0, max.y));
		}
	}

	private bool DoGridProcessing()
	{
		if (controllerGrid != null)
		{
			return Camera.main != null;
		}
		return false;
	}
}
