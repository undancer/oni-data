using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EightDirectionUtil
{
	public static readonly Vector3[] normals = new Vector3[8]
	{
		Vector3.up,
		(Vector3.up + Vector3.left).normalized,
		Vector3.left,
		(Vector3.down + Vector3.left).normalized,
		Vector3.down,
		(Vector3.down + Vector3.right).normalized,
		Vector3.right,
		(Vector3.up + Vector3.right).normalized
	};

	public static int GetDirectionIndex(EightDirection direction)
	{
		return (int)direction;
	}

	public static EightDirection AngleToDirection(int angle)
	{
		return (EightDirection)Mathf.Floor((float)angle / 45f);
	}

	public static Vector3 GetNormal(EightDirection direction)
	{
		return normals[GetDirectionIndex(direction)];
	}

	public static float GetAngle(EightDirection direction)
	{
		return 45 * GetDirectionIndex(direction);
	}
}
