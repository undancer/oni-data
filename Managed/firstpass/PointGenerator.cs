using System;
using System.Collections.Generic;
using Delaunay.Geo;
using UnityEngine;

public static class PointGenerator
{
	[SerializeField]
	public enum SampleBehaviour
	{
		UniformSquare,
		UniformHex,
		UniformScaledHex,
		UniformSpiral,
		UniformCircle,
		PoissonDisk,
		StdRand
	}

	public static List<Vector2> GetRandomPoints(Polygon boundingArea, float density, float avoidRadius, List<Vector2> avoidPoints, SampleBehaviour behaviour, bool testInsideBounds, SeededRandom rnd, bool doShuffle = true, bool testAvoidPoints = true)
	{
		float num = boundingArea.bounds.width;
		float num2 = boundingArea.bounds.height;
		float num3 = num / 2f;
		float num4 = num2 / 2f;
		int num5 = (int)Mathf.Floor(num * num2 / density);
		uint num6 = (uint)Mathf.Sqrt(num5);
		int pointsPerIteration = 10;
		uint num7 = (uint)((float)num5 * 0.98f);
		Vector2 min = boundingArea.bounds.min;
		Vector2 max = boundingArea.bounds.max;
		List<Vector2> list = new List<Vector2>();
		switch (behaviour)
		{
		case SampleBehaviour.PoissonDisk:
			list = new UniformPoissonDiskSampler(rnd).SampleRectangle(min, max, density, pointsPerIteration);
			break;
		case SampleBehaviour.UniformSquare:
		{
			for (float num24 = 0f - num4 + density; num24 < num4 - density; num24 += density)
			{
				for (float num25 = 0f - num3 + density; num25 < num3 - density; num25 += density)
				{
					list.Add(boundingArea.Centroid() + new Vector2(num25, num24));
				}
			}
			break;
		}
		case SampleBehaviour.UniformHex:
		{
			for (uint num10 = 0u; num10 < num6; num10++)
			{
				for (uint num11 = 0u; num11 < num6; num11++)
				{
					list.Add(boundingArea.Centroid() + new Vector2(0f - num3 + (0.5f + (float)num10) / (float)num6 * num, 0f - num4 + (0.25f + 0.5f * (float)(num10 % 2u) + (float)num11) / (float)num6 * num2));
				}
			}
			break;
		}
		case SampleBehaviour.UniformSpiral:
		{
			for (uint num18 = 0u; num18 < num7; num18++)
			{
				double num19 = (double)num18 / (32.0 * (double)density * 8.0);
				double num20 = Math.Sqrt(num19 * 512.0 * (double)density);
				double num21 = Math.Sqrt(num19);
				double num22 = Math.Sin(num20) * num21;
				double num23 = Math.Cos(num20) * num21;
				list.Add(boundingArea.bounds.center + new Vector2((float)num22 * boundingArea.bounds.width, (float)num23 * boundingArea.bounds.height));
			}
			break;
		}
		case SampleBehaviour.UniformCircle:
		{
			float num12 = (float)Math.PI * 2f * avoidRadius / density;
			float num13 = rnd.RandomValue();
			for (uint num14 = 1u; (float)num14 < num12; num14++)
			{
				float num15 = num13 + (float)num14 / num12 * (float)Math.PI * 2f;
				double num16 = Math.Cos(num15) * (double)avoidRadius;
				double num17 = Math.Sin(num15) * (double)avoidRadius;
				list.Add(boundingArea.bounds.center + new Vector2((float)num16, (float)num17));
			}
			break;
		}
		default:
		{
			for (float num8 = 0f - num4 + avoidRadius * 0.3f + rnd.RandomValue() * 2f; num8 < num4 - (avoidRadius * 0.3f + rnd.RandomValue() * 2f); num8 += density + rnd.RandomValue())
			{
				for (float num9 = 0f - num3 + avoidRadius * 0.3f + rnd.RandomValue() * 2f + rnd.RandomValue() * 2f; num9 < num3 - (avoidRadius * 0.3f + rnd.RandomValue() * 2f); num9 += density + rnd.RandomValue())
				{
					list.Add(boundingArea.Centroid() + new Vector2(num9, num8 + rnd.RandomValue() - 0.5f));
				}
			}
			break;
		}
		}
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < list.Count; i++)
		{
			if (testInsideBounds && !boundingArea.Contains(list[i]))
			{
				continue;
			}
			bool flag = false;
			if (testAvoidPoints && avoidPoints != null)
			{
				for (int j = 0; j < avoidPoints.Count; j++)
				{
					if (Mathf.Abs((avoidPoints[j] - list[i]).magnitude) < avoidRadius)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list2.Add(list[i]);
			}
		}
		if (doShuffle)
		{
			list2.ShuffleSeeded(rnd.RandomSource());
		}
		return list2;
	}

	public static List<Vector2> GetArchimedesSpiralPoints(int pointCount, Vector2 startPoint, double tetha, double alpha)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < pointCount; i++)
		{
			double num = tetha / (double)pointCount * (double)i;
			double num2 = alpha / (double)pointCount * (double)i;
			list.Add(new Vector2((float)((double)startPoint.x + num2 * Math.Cos(num)), (float)((double)startPoint.y + num2 * Math.Sin(num))));
		}
		return list;
	}

	public static List<Vector2> GetFilliedRectangle(Rect boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		for (float num = boundingArea.xMin; num < boundingArea.xMax; num += density)
		{
			for (float num2 = boundingArea.yMin; num2 < boundingArea.yMax; num2 += density)
			{
				list.Add(new Vector2(num, num2));
			}
		}
		return list;
	}

	public static List<Vector2> GetSpaceFillingRandom(Rect boundingArea, float density, SeededRandom rnd)
	{
		List<Vector2> filliedRectangle = GetFilliedRectangle(boundingArea, density);
		filliedRectangle.ShuffleSeeded(rnd.RandomSource());
		return filliedRectangle;
	}

	private static Vector2I PointOnRightHandSpiralOut(int index)
	{
		int num = (int)Mathf.Ceil(Mathf.Sqrt(4 * index + 1) * 0.5f - 1f + 0.5f);
		int num2 = (((num & 1) == 0) ? 1 : 0);
		int num3 = num * (num + 1);
		bool flag = num3 - index < num;
		int num4 = 2 * (num2 ^ (flag ? 1 : 0)) - 1;
		Vector2I v = new Vector2I(-num4, 2 * num2 - 1);
		Vector2I u = new Vector2I(-((num2 == 0 && flag) ? 1 : 0), 0) + v * (num / 2);
		Vector2I v2 = new Vector2I((!flag) ? 1 : 0, flag ? 1 : 0) * num4;
		int s = index - num3 + 2 * num - (flag ? 1 : 0) * num;
		return u + v2 * s;
	}

	public static List<Vector2> GetSpaceFillingSpiral(Rect boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		float num = boundingArea.width / density;
		float num2 = boundingArea.height / density;
		for (int i = 0; (float)i < num2 * num; i++)
		{
			Vector2I vector2I = PointOnRightHandSpiralOut(i);
			list.Add(new Vector2(boundingArea.center.x + (float)vector2I.x, boundingArea.center.y + (float)vector2I.y - density));
		}
		return list;
	}

	public static List<Vector2> GetSpaceFillingSpiral(Polygon boundingArea, float density)
	{
		List<Vector2> list = new List<Vector2>();
		float num = boundingArea.bounds.width / density;
		float num2 = boundingArea.bounds.height / density;
		for (int i = 0; (float)i < num2 * num; i++)
		{
			Vector2I vector2I = PointOnRightHandSpiralOut(i);
			Vector2 vector = new Vector2(boundingArea.bounds.center.x + (float)vector2I.x, boundingArea.bounds.center.y + (float)vector2I.y - density);
			if (boundingArea.Contains(vector))
			{
				list.Add(vector);
			}
		}
		return list;
	}
}
