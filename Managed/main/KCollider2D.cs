using UnityEngine;

public abstract class KCollider2D : KMonoBehaviour, IRenderEveryTick
{
	[SerializeField]
	public Vector2 _offset;

	private Extents cachedExtents;

	private HandleVector<int>.Handle partitionerEntry;

	public Vector2 offset
	{
		get
		{
			return _offset;
		}
		set
		{
			_offset = value;
			MarkDirty();
		}
	}

	public abstract Bounds bounds
	{
		get;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		autoRegisterSimRender = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(base.transform, OnMovementStateChanged);
		MarkDirty(force: true);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(base.transform, OnMovementStateChanged);
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}

	public void MarkDirty(bool force = false)
	{
		bool flag = force || partitionerEntry.IsValid();
		if (!flag)
		{
			return;
		}
		Extents extents = GetExtents();
		if (force || cachedExtents.x != extents.x || cachedExtents.y != extents.y || cachedExtents.width != extents.width || cachedExtents.height != extents.height)
		{
			cachedExtents = extents;
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
			if (flag)
			{
				partitionerEntry = GameScenePartitioner.Instance.Add(base.name, this, cachedExtents, GameScenePartitioner.Instance.collisionLayer, null);
			}
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving)
		{
			MarkDirty();
			SimAndRenderScheduler.instance.Add(this);
		}
		else
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	private static void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		transform.GetComponent<KCollider2D>().OnMovementStateChanged(is_moving);
	}

	public void RenderEveryTick(float dt)
	{
		MarkDirty();
	}

	public abstract bool Intersects(Vector2 pos);

	public abstract Extents GetExtents();
}
