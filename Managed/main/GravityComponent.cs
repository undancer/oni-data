using System;
using UnityEngine;

public struct GravityComponent
{
	public Transform transform;

	public Vector2 velocity;

	public float radius;

	public float elapsedTime;

	public System.Action onLanded;

	public bool landOnFakeFloors;

	public GravityComponent(Transform transform, System.Action on_landed, Vector2 initial_velocity, bool land_on_fake_floors)
	{
		this.transform = transform;
		elapsedTime = 0f;
		velocity = initial_velocity;
		onLanded = on_landed;
		radius = GetRadius(transform);
		landOnFakeFloors = land_on_fake_floors;
	}

	public static float GetRadius(Transform transform)
	{
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if (component != null)
		{
			return component.radius;
		}
		KCollider2D component2 = transform.GetComponent<KCollider2D>();
		if (component2 != null)
		{
			return transform.GetPosition().y - component2.bounds.min.y;
		}
		return 0f;
	}
}
