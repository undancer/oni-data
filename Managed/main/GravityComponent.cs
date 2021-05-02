using System;
using UnityEngine;

public struct GravityComponent
{
	public Transform transform;

	public Vector2 velocity;

	public float elapsedTime;

	public System.Action onLanded;

	public bool landOnFakeFloors;

	public Vector2 extents;

	public float yOffset;

	public GravityComponent(Transform transform, System.Action on_landed, Vector2 initial_velocity, bool land_on_fake_floors)
	{
		this.transform = transform;
		elapsedTime = 0f;
		velocity = initial_velocity;
		onLanded = on_landed;
		landOnFakeFloors = land_on_fake_floors;
		KCollider2D component = transform.GetComponent<KCollider2D>();
		extents = GetExtents(component);
		yOffset = GetOffset(component).y;
	}

	public static float GetGroundOffset(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.bounds.extents.y - collider.offset.y;
		}
		return 0f;
	}

	public static Vector2 GetExtents(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.bounds.extents;
		}
		return Vector2.zero;
	}

	public static Vector2 GetOffset(KCollider2D collider)
	{
		if (collider != null)
		{
			return collider.offset;
		}
		return Vector2.zero;
	}
}
