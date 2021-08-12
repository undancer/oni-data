using UnityEngine;

public class FallerComponents : KGameObjectComponentManager<FallerComponent>
{
	private const float EPSILON = 0.07f;

	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity)
	{
		return Add(go, new FallerComponent(go.transform, initial_velocity));
	}

	public override void Remove(GameObject go)
	{
		HandleVector<int>.Handle handle = GetHandle(go);
		OnCleanUpImmediate(handle);
		CleanupInfo cleanupInfo = new CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			cleanupList.Add(cleanupInfo);
		}
		else
		{
			InternalRemoveComponent(cleanupInfo);
		}
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle h)
	{
		FallerComponent new_data = GetData(h);
		Vector3 position = new_data.transform.GetPosition();
		int num = Grid.PosToCell(position);
		new_data.cellChangedCB = delegate
		{
			OnSolidChanged(h);
		};
		float groundOffset = GravityComponent.GetGroundOffset(new_data.transform.GetComponent<KCollider2D>());
		int num2 = Grid.PosToCell(new Vector3(position.x, position.y - groundOffset - 0.07f, position.z));
		bool flag = Grid.IsValidCell(num2) && Grid.Solid[num2] && new_data.initialVelocity.sqrMagnitude == 0f;
		if ((Grid.IsValidCell(num) && Grid.Solid[num]) || flag)
		{
			new_data.solidChangedCB = delegate
			{
				OnSolidChanged(h);
			};
			int height = 2;
			Vector2I vector2I = Grid.CellToXY(num);
			vector2I.y--;
			if (vector2I.y < 0)
			{
				vector2I.y = 0;
				height = 1;
			}
			else if (vector2I.y == Grid.HeightInCells - 1)
			{
				height = 1;
			}
			new_data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", new_data.transform.gameObject, vector2I.x, vector2I.y, 1, height, GameScenePartitioner.Instance.solidChangedLayer, new_data.solidChangedCB);
			GameComps.Fallers.SetData(h, new_data);
		}
		else
		{
			GameComps.Fallers.SetData(h, new_data);
			AddGravity(new_data.transform, new_data.initialVelocity);
		}
	}

	protected override void OnSpawn(HandleVector<int>.Handle h)
	{
		base.OnSpawn(h);
		FallerComponent fallerComponent = GetData(h);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(fallerComponent.transform, fallerComponent.cellChangedCB, "FallerComponent.OnSpawn");
	}

	private void OnCleanUpImmediate(HandleVector<int>.Handle h)
	{
		FallerComponent new_data = GetData(h);
		GameScenePartitioner.Instance.Free(ref new_data.partitionerEntry);
		if (new_data.cellChangedCB != null)
		{
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(new_data.transformInstanceId, new_data.cellChangedCB);
			new_data.cellChangedCB = null;
		}
		if (GameComps.Gravities.Has(new_data.transform.gameObject))
		{
			GameComps.Gravities.Remove(new_data.transform.gameObject);
		}
		SetData(h, new_data);
	}

	private static void AddGravity(Transform transform, Vector2 initial_velocity)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Add(transform.gameObject, initial_velocity, delegate
			{
				OnLanded(transform);
			});
			HandleVector<int>.Handle handle = GameComps.Fallers.GetHandle(transform.gameObject);
			FallerComponent new_data = GameComps.Fallers.GetData(handle);
			if (new_data.partitionerEntry.IsValid())
			{
				GameScenePartitioner.Instance.Free(ref new_data.partitionerEntry);
				GameComps.Fallers.SetData(handle, new_data);
			}
		}
	}

	private static void RemoveGravity(Transform transform)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			return;
		}
		GameComps.Gravities.Remove(transform.gameObject);
		HandleVector<int>.Handle h = GameComps.Fallers.GetHandle(transform.gameObject);
		FallerComponent new_data = GameComps.Fallers.GetData(h);
		int cell = Grid.CellBelow(Grid.PosToCell(transform.GetPosition()));
		GameScenePartitioner.Instance.Free(ref new_data.partitionerEntry);
		if (Grid.IsValidCell(cell))
		{
			new_data.solidChangedCB = delegate
			{
				OnSolidChanged(h);
			};
			new_data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", transform.gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new_data.solidChangedCB);
		}
		GameComps.Fallers.SetData(h, new_data);
	}

	private static void OnLanded(Transform transform)
	{
		RemoveGravity(transform);
	}

	private static void OnSolidChanged(HandleVector<int>.Handle handle)
	{
		FallerComponent fallerComponent = GameComps.Fallers.GetData(handle);
		if (fallerComponent.transform == null)
		{
			return;
		}
		Vector3 position = fallerComponent.transform.GetPosition();
		position.y = position.y - fallerComponent.offset - 0.1f;
		int num = Grid.PosToCell(position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		bool flag = !Grid.Solid[num];
		if (flag != fallerComponent.isFalling)
		{
			fallerComponent.isFalling = flag;
			if (flag)
			{
				AddGravity(fallerComponent.transform, Vector2.zero);
			}
			else
			{
				RemoveGravity(fallerComponent.transform);
			}
		}
	}
}
