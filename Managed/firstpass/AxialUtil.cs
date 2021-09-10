using System;
using System.Collections.Generic;
using UnityEngine;

public static class AxialUtil
{
	public static List<AxialI> GetRing(AxialI center, int radius)
	{
		if (radius < 0)
		{
			Debug.LogError($"Negative radius specified: {radius}");
		}
		if (radius == 0)
		{
			return new List<AxialI> { center };
		}
		List<AxialI> list = new List<AxialI>();
		for (int i = 0; i < AxialI.DIRECTIONS.Count; i++)
		{
			AxialI axialI = center + AxialI.DIRECTIONS[i] * radius;
			AxialI axialI2 = AxialI.CLOCKWISE[i];
			for (int j = 0; j < radius; j++)
			{
				list.Add(axialI + axialI2 * j);
			}
		}
		return list;
	}

	public static List<AxialI> GetRings(AxialI center, int minRadius, int maxRadius)
	{
		List<AxialI> list = new List<AxialI>();
		for (int i = minRadius; i <= maxRadius; i++)
		{
			list.AddRange(GetRing(center, i));
		}
		return list;
	}

	public static List<AxialI> GetAllPointsWithinRadius(AxialI center, int radius)
	{
		return GetRings(center, 0, radius);
	}

	public static int GetDistance(AxialI a, AxialI b)
	{
		Vector3I vector3I = a.ToCube();
		Vector3I vector3I2 = b.ToCube();
		return (Math.Abs(vector3I.x - vector3I2.x) + Math.Abs(vector3I.y - vector3I2.y) + Math.Abs(vector3I.z - vector3I2.z)) / 2;
	}

	public static bool IsAdjacent(this AxialI a, AxialI b)
	{
		return GetDistance(a, b) == 1;
	}

	public static bool IsWithinRadius(this AxialI a, AxialI center, int radius)
	{
		return GetDistance(a, center) <= radius;
	}

	public static Vector3 AxialToWorld(float r, float q)
	{
		return new Vector3(Mathf.Sqrt(3f) * r + Mathf.Sqrt(3f) / 2f * q, -1.5f * q, 0f);
	}

	public static IEnumerable<AxialI> SpiralOut(AxialI startLocation, int maximumRings)
	{
		for (int ring = 0; ring < maximumRings; ring++)
		{
			for (int i = 0; i < AxialI.DIRECTIONS.Count; i++)
			{
				AxialI vertex = startLocation + AxialI.DIRECTIONS[i] * ring;
				AxialI increment = AxialI.CLOCKWISE[i];
				for (int j = 0; j < ring; j++)
				{
					yield return vertex + increment * j;
				}
			}
		}
	}
}
