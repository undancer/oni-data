using System;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
	private ulong numFramesAbove30;

	private ulong numFramesBelow30;

	private LinkedList<float> frameTimes = new LinkedList<float>();

	private float frameTimeTotal;

	private static readonly int frameRateWindowSize = 150;

	private const float GOOD_FRAME_TIME = 71f / (678f * (float)Math.PI);

	public ulong NumFramesAbove30 => numFramesAbove30;

	public ulong NumFramesBelow30 => numFramesBelow30;

	public float FPS
	{
		get
		{
			if (frameTimeTotal != 0f)
			{
				return (float)frameTimes.Count / frameTimeTotal;
			}
			return 0f;
		}
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (unscaledDeltaTime <= 71f / (678f * (float)Math.PI))
			{
				numFramesAbove30++;
			}
			else
			{
				numFramesBelow30++;
			}
			if (frameTimes.Count == frameRateWindowSize)
			{
				LinkedListNode<float> first = frameTimes.First;
				frameTimeTotal -= first.Value;
				frameTimes.RemoveFirst();
				first.Value = unscaledDeltaTime;
				frameTimes.AddLast(first);
			}
			else
			{
				frameTimes.AddLast(unscaledDeltaTime);
			}
			frameTimeTotal += unscaledDeltaTime;
		}
	}

	public void Reset()
	{
		numFramesAbove30 = 0uL;
		numFramesBelow30 = 0uL;
	}
}
