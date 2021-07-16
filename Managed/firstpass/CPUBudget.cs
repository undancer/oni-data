using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class CPUBudget
{
	private class Tuning : TuningData<Tuning>
	{
		public int overrideCoreCount = -1;

		public float defaultLoadBalanceThreshold = 0.1f;
	}

	private struct Node
	{
		public ICPULoad load;

		public List<Node> children;

		public long start;

		public float frameTime;

		public float loadBalanceThreshold;
	}

	public static Stopwatch stopwatch = Stopwatch.StartNew();

	private static Dictionary<ICPULoad, Node> nodes = new Dictionary<ICPULoad, Node>();

	public static int coreCount
	{
		get
		{
			int overrideCoreCount = TuningData<Tuning>.Get().overrideCoreCount;
			if (0 >= overrideCoreCount || overrideCoreCount >= SystemInfo.processorCount)
			{
				return SystemInfo.processorCount;
			}
			return overrideCoreCount;
		}
	}

	public static float ComputeDuration(long start)
	{
		return (float)((stopwatch.ElapsedTicks - start) * 1000000 / Stopwatch.Frequency) / 1000f;
	}

	public static void AddRoot(ICPULoad root)
	{
		nodes.Add(root, new Node
		{
			load = root,
			children = new List<Node>(),
			frameTime = root.GetEstimatedFrameTime(),
			loadBalanceThreshold = TuningData<Tuning>.Get().defaultLoadBalanceThreshold
		});
	}

	public static void AddChild(ICPULoad parent, ICPULoad child, float loadBalanceThreshold)
	{
		Node node = default(Node);
		node.load = child;
		node.children = new List<Node>();
		node.frameTime = child.GetEstimatedFrameTime();
		node.loadBalanceThreshold = loadBalanceThreshold;
		Node node2 = node;
		nodes.Add(child, node2);
		nodes[parent].children.Add(node2);
	}

	public static void AddChild(ICPULoad parent, ICPULoad child)
	{
		AddChild(parent, child, TuningData<Tuning>.Get().defaultLoadBalanceThreshold);
	}

	public static void FinalizeChildren(ICPULoad parent)
	{
		Node node = nodes[parent];
		List<Node> children = nodes[parent].children;
		float num = 0f;
		foreach (Node item in children)
		{
			FinalizeChildren(item.load);
			num += item.frameTime;
		}
		for (int i = 0; i != children.Count; i++)
		{
			Node value = children[i];
			value.frameTime = node.frameTime * (value.frameTime / num);
			children[i] = value;
		}
	}

	public static void Start(ICPULoad cpuLoad)
	{
		Node value = nodes[cpuLoad];
		value.start = stopwatch.ElapsedTicks;
		nodes[cpuLoad] = value;
	}

	public static void End(ICPULoad cpuLoad)
	{
		Node node = nodes[cpuLoad];
		float num = node.frameTime - ComputeDuration(node.start);
		if (node.loadBalanceThreshold < Math.Abs(num))
		{
			Balance(cpuLoad, num);
		}
	}

	public static void Balance(ICPULoad cpuLoad, float frameTimeDelta)
	{
		Node node = nodes[cpuLoad];
		List<Node> children = node.children;
		if (children.Count == 0)
		{
			if (node.load.AdjustLoad(node.frameTime, frameTimeDelta))
			{
				node.frameTime += frameTimeDelta;
			}
			return;
		}
		for (int i = 0; i != children.Count; i++)
		{
			Node value = children[i];
			float num = value.frameTime / node.frameTime;
			float frameTimeDelta2 = frameTimeDelta * num;
			Balance(value.load, frameTimeDelta2);
			children[i] = value;
		}
	}
}
