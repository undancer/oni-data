using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tracker
{
	protected List<DataPoint> dataPoints = new List<DataPoint>();

	private int maxPoints = 150;

	public Tuple<float, float>[] ChartableData(float periodLength)
	{
		float time = GameClock.Instance.GetTime();
		List<Tuple<float, float>> list = new List<Tuple<float, float>>();
		int num = dataPoints.Count - 1;
		while (num >= 0 && !(dataPoints[num].periodStart < time - periodLength))
		{
			list.Add(new Tuple<float, float>(dataPoints[num].periodStart, dataPoints[num].periodValue));
			num--;
		}
		if (list.Count == 0)
		{
			if (dataPoints.Count > 0)
			{
				list.Add(new Tuple<float, float>(dataPoints[dataPoints.Count - 1].periodStart, dataPoints[dataPoints.Count - 1].periodValue));
			}
			else
			{
				list.Add(new Tuple<float, float>(0f, 0f));
			}
		}
		list.Reverse();
		return list.ToArray();
	}

	public float GetDataTimeLength()
	{
		float num = 0f;
		for (int num2 = dataPoints.Count - 1; num2 >= 0; num2--)
		{
			num += dataPoints[num2].periodEnd - dataPoints[num2].periodStart;
		}
		return num;
	}

	public abstract void UpdateData();

	public abstract string FormatValueString(float value);

	public float GetCurrentValue()
	{
		if (dataPoints.Count == 0)
		{
			return 0f;
		}
		return dataPoints[dataPoints.Count - 1].periodValue;
	}

	public float GetMinValue(float sampleHistoryLengthSeconds)
	{
		float time = GameClock.Instance.GetTime();
		Tuple<float, float>[] array = ChartableData(sampleHistoryLengthSeconds);
		if (array.Length == 0)
		{
			return 0f;
		}
		if (array.Length == 1)
		{
			return array[0].second;
		}
		float num = array[array.Length - 1].second;
		int num2 = array.Length - 1;
		while (num2 >= 0 && !(time - array[num2].first > sampleHistoryLengthSeconds))
		{
			num = Mathf.Min(num, array[num2].second);
			num2--;
		}
		return num;
	}

	public float GetMaxValue(int sampleHistoryLengthSeconds)
	{
		float time = GameClock.Instance.GetTime();
		Tuple<float, float>[] array = ChartableData(sampleHistoryLengthSeconds);
		if (array.Length == 0)
		{
			return 0f;
		}
		if (array.Length == 1)
		{
			return array[0].second;
		}
		float num = array[array.Length - 1].second;
		int num2 = array.Length - 1;
		while (num2 >= 0 && !(time - array[num2].first > (float)sampleHistoryLengthSeconds))
		{
			num = Mathf.Max(num, array[num2].second);
			num2--;
		}
		return num;
	}

	public float GetAverageValue(float sampleHistoryLengthSeconds)
	{
		float time = GameClock.Instance.GetTime();
		Tuple<float, float>[] array = ChartableData(sampleHistoryLengthSeconds);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int num4 = array.Length - 1; num4 >= 0; num4--)
		{
			if (array[num4].first >= time - sampleHistoryLengthSeconds)
			{
				float num5 = ((num4 == array.Length - 1) ? (time - array[num4].first) : (array[num4 + 1].first - array[num4].first));
				num3 += num5;
				if (!float.IsNaN(array[num4].second))
				{
					num2 += num5 * array[num4].second;
				}
			}
		}
		if (num3 == 0f)
		{
			if (array.Length == 0)
			{
				return 0f;
			}
			return array[array.Length - 1].second;
		}
		return num2 / num3;
	}

	public float GetDelta(float secondsAgo)
	{
		float time = GameClock.Instance.GetTime();
		Tuple<float, float>[] array = ChartableData(secondsAgo);
		if (array.Length < 2)
		{
			return 0f;
		}
		float num = -1f;
		float second = array[array.Length - 1].second;
		float num2 = 0f;
		for (int num3 = array.Length - 1; num3 >= 0; num3--)
		{
			num2 = time - array[num3].first;
			if (num2 >= secondsAgo)
			{
				num = array[num3].second;
			}
		}
		return second - num;
	}

	protected void AddPoint(float value)
	{
		if (float.IsNaN(value))
		{
			value = 0f;
		}
		dataPoints.Add(new DataPoint((dataPoints.Count == 0) ? GameClock.Instance.GetTime() : dataPoints[dataPoints.Count - 1].periodEnd, GameClock.Instance.GetTime(), value));
		int count = Math.Max(0, dataPoints.Count - maxPoints);
		dataPoints.RemoveRange(0, count);
	}

	public List<DataPoint> GetCompressedData()
	{
		int num = 10;
		List<DataPoint> list = new List<DataPoint>();
		float num2 = dataPoints[dataPoints.Count - 1].periodEnd - dataPoints[0].periodStart;
		float num3 = num2 / (float)num;
		for (int i = 0; i < num; i++)
		{
			float num4 = num3 * (float)i;
			float num5 = num4 + num3;
			float num6 = 0f;
			for (int j = 0; j < dataPoints.Count; j++)
			{
				DataPoint dataPoint = dataPoints[j];
				num6 += dataPoint.periodValue * Mathf.Max(0f, Mathf.Min(num5, dataPoint.periodEnd) - Mathf.Max(dataPoint.periodStart, num4));
			}
			list.Add(new DataPoint(num4, num5, num6 / (num5 - num4)));
		}
		return list;
	}

	public void OverwriteData(List<DataPoint> newData)
	{
		dataPoints = newData;
	}
}
