using UnityEngine;

public class KCircleCollider2D : KCollider2D
{
	[SerializeField]
	private float _radius;

	public float radius
	{
		get
		{
			return _radius;
		}
		set
		{
			_radius = value;
			MarkDirty();
		}
	}

	public override Bounds bounds => new Bounds(base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f), new Vector3(_radius * 2f, _radius * 2f, 0f));

	public override Extents GetExtents()
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = new Vector2(vector.x - radius, vector.y - radius);
		Vector2 vector3 = new Vector2(vector.x + radius, vector.y + radius);
		int width = (int)vector3.x - (int)vector2.x + 1;
		int height = (int)vector3.y - (int)vector2.y + 1;
		return new Extents((int)(vector.x - _radius), (int)(vector.y - _radius), width, height);
	}

	public override bool Intersects(Vector2 pos)
	{
		Vector3 position = base.transform.GetPosition();
		Vector2 b = new Vector2(position.x, position.y) + base.offset;
		return (pos - b).sqrMagnitude <= _radius * _radius;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(bounds.center, radius);
	}
}
