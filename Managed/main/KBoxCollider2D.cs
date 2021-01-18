using UnityEngine;

public class KBoxCollider2D : KCollider2D
{
	[SerializeField]
	private Vector2 _size;

	public Vector2 size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			MarkDirty();
		}
	}

	public override Bounds bounds
	{
		get
		{
			Vector3 center = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
			return new Bounds(center, new Vector3(_size.x, _size.y, 0f));
		}
	}

	public override Extents GetExtents()
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = size * 0.9999f;
		Vector2 vector3 = new Vector2(vector.x - vector2.x * 0.5f, vector.y - vector2.y * 0.5f);
		Vector2 vector4 = new Vector2(vector.x + vector2.x * 0.5f, vector.y + vector2.y * 0.5f);
		Vector2I vector2I = new Vector2I((int)vector3.x, (int)vector3.y);
		Vector2I vector2I2 = new Vector2I((int)vector4.x, (int)vector4.y);
		int width = vector2I2.x - vector2I.x + 1;
		int height = vector2I2.y - vector2I.y + 1;
		return new Extents(vector2I.x, vector2I.y, width, height);
	}

	public override bool Intersects(Vector2 intersect_pos)
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = new Vector2(vector.x - size.x * 0.5f, vector.y - size.y * 0.5f);
		Vector2 vector3 = new Vector2(vector.x + size.x * 0.5f, vector.y + size.y * 0.5f);
		return intersect_pos.x >= vector2.x && intersect_pos.x <= vector3.x && intersect_pos.y >= vector2.y && intersect_pos.y <= vector3.y;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(bounds.center, new Vector3(_size.x, _size.y, 0f));
	}
}
