using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BrainScheduler")]
public class BrainScheduler : KMonoBehaviour, IRenderEveryTick, ICPULoad
{
	private class Tuning : TuningData<Tuning>
	{
		public bool disableAsyncPathProbes;

		public float frameTime = 5f;
	}

	private abstract class BrainGroup : ICPULoad
	{
		private List<Brain> brains = new List<Brain>();

		private string increaseLoadLabel;

		private string decreaseLoadLabel;

		private WorkItemCollection<Navigator.PathProbeTask, object> pathProbeJob = new WorkItemCollection<Navigator.PathProbeTask, object>();

		private int nextUpdateBrain;

		private int nextPathProbeBrain;

		public Tag tag { get; private set; }

		public int probeSize { get; private set; }

		public int probeCount { get; private set; }

		protected BrainGroup(Tag tag)
		{
			this.tag = tag;
			probeSize = InitialProbeSize();
			probeCount = InitialProbeCount();
			string text = tag.ToString();
			increaseLoadLabel = "IncLoad" + text;
			decreaseLoadLabel = "DecLoad" + text;
		}

		public void AddBrain(Brain brain)
		{
			brains.Add(brain);
		}

		public void RemoveBrain(Brain brain)
		{
			int num = brains.IndexOf(brain);
			if (num != -1)
			{
				brains.RemoveAt(num);
				OnRemoveBrain(num, ref nextUpdateBrain);
				OnRemoveBrain(num, ref nextPathProbeBrain);
			}
		}

		public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
		{
			bool flag = frameTimeDelta > 0f;
			int num = 0;
			int num2 = Math.Max(probeCount, Math.Min(brains.Count, CPUBudget.coreCount));
			num += num2 - probeCount;
			probeCount = num2;
			float num3 = Math.Min(1f, (float)probeCount / (float)CPUBudget.coreCount);
			float num4 = num3 * (float)probeSize;
			float num5 = num3 * (float)probeSize;
			float num6 = currentFrameTime / num5;
			float num7 = frameTimeDelta / num6;
			if (num == 0)
			{
				float num8 = num4 + num7 / (float)CPUBudget.coreCount;
				int num9 = MathUtil.Clamp(MinProbeSize(), IdealProbeSize(), (int)(num8 / num3));
				num += num9 - probeSize;
				probeSize = num9;
			}
			if (num == 0)
			{
				int num10 = Math.Max(1, (int)num3 + (flag ? 1 : (-1)));
				int num11 = MathUtil.Clamp(MinProbeSize(), IdealProbeSize(), (int)((num5 + num7) / (float)num10));
				int num12 = Math.Min(brains.Count, num10 * CPUBudget.coreCount);
				num += num12 - probeCount;
				probeCount = num12;
				probeSize = num11;
			}
			if (num == 0 && flag)
			{
				int num13 = probeSize + ProbeSizeStep();
				num += num13 - probeSize;
				probeSize = num13;
			}
			if (num >= 0 && num <= 0)
			{
				Debug.LogWarning("AdjustLoad() failed");
			}
			return num != 0;
		}

		private void IncrementBrainIndex(ref int brainIndex)
		{
			brainIndex++;
			if (brainIndex == brains.Count)
			{
				brainIndex = 0;
			}
		}

		private void ClampBrainIndex(ref int brainIndex)
		{
			brainIndex = MathUtil.Clamp(0, brains.Count - 1, brainIndex);
		}

		private void OnRemoveBrain(int removedIndex, ref int brainIndex)
		{
			if (removedIndex < brainIndex)
			{
				brainIndex--;
			}
			else if (brainIndex == brains.Count)
			{
				brainIndex = 0;
			}
		}

		private void AsyncPathProbe()
		{
			_ = probeSize;
			pathProbeJob.Reset(null);
			for (int i = 0; i != brains.Count; i++)
			{
				ClampBrainIndex(ref nextPathProbeBrain);
				Brain brain = brains[nextPathProbeBrain];
				if (brain.IsRunning())
				{
					Navigator component = brain.GetComponent<Navigator>();
					if (component != null)
					{
						component.executePathProbeTaskAsync = true;
						component.PathProber.potentialCellsPerUpdate = probeSize;
						component.pathProbeTask.Update();
						pathProbeJob.Add(component.pathProbeTask);
						if (pathProbeJob.Count == probeCount)
						{
							break;
						}
					}
				}
				IncrementBrainIndex(ref nextPathProbeBrain);
			}
			CPUBudget.Start(this);
			GlobalJobManager.Run(pathProbeJob);
			CPUBudget.End(this);
		}

		public void RenderEveryTick(float dt, bool isAsyncPathProbeEnabled)
		{
			if (isAsyncPathProbeEnabled)
			{
				AsyncPathProbe();
			}
			int num = InitialProbeCount();
			for (int i = 0; i != brains.Count; i++)
			{
				if (num == 0)
				{
					break;
				}
				ClampBrainIndex(ref nextUpdateBrain);
				Brain brain = brains[nextUpdateBrain];
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					num--;
				}
				IncrementBrainIndex(ref nextUpdateBrain);
			}
		}

		public void AccumulatePathProbeIterations(Dictionary<string, int> pathProbeIterations)
		{
			foreach (Brain brain in brains)
			{
				Navigator component = brain.GetComponent<Navigator>();
				if (!(component == null) && !pathProbeIterations.ContainsKey(brain.name))
				{
					pathProbeIterations.Add(brain.name, component.PathProber.updateCount);
				}
			}
		}

		protected abstract int InitialProbeCount();

		protected abstract int InitialProbeSize();

		protected abstract int MinProbeSize();

		protected abstract int IdealProbeSize();

		protected abstract int ProbeSizeStep();

		public abstract float GetEstimatedFrameTime();

		public abstract float LoadBalanceThreshold();
	}

	private class DupeBrainGroup : BrainGroup
	{
		public class Tuning : TuningData<Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 1000;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 2f;

			public float loadBalanceThreshold = 0.1f;
		}

		public DupeBrainGroup()
			: base(GameTags.DupeBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<Tuning>.Get().loadBalanceThreshold;
		}
	}

	private class CreatureBrainGroup : BrainGroup
	{
		public class Tuning : TuningData<Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 300;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 1f;

			public float loadBalanceThreshold = 0.1f;
		}

		public CreatureBrainGroup()
			: base(GameTags.CreatureBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<Tuning>.Get().loadBalanceThreshold;
		}
	}

	public const float millisecondsPerFrame = 33.33333f;

	public const float secondsPerFrame = 0.033333328f;

	public const float framesPerSecond = 30.000006f;

	private List<BrainGroup> brainGroups = new List<BrainGroup>();

	private bool isAsyncPathProbeEnabled => !TuningData<Tuning>.Get().disableAsyncPathProbes;

	protected override void OnPrefabInit()
	{
		brainGroups.Add(new DupeBrainGroup());
		brainGroups.Add(new CreatureBrainGroup());
		Components.Brains.Register(OnAddBrain, OnRemoveBrain);
		CPUBudget.AddRoot(this);
		foreach (BrainGroup brainGroup in brainGroups)
		{
			CPUBudget.AddChild(this, brainGroup, brainGroup.LoadBalanceThreshold());
		}
		CPUBudget.FinalizeChildren(this);
	}

	private void OnAddBrain(Brain brain)
	{
		bool test = false;
		foreach (BrainGroup brainGroup in brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				brainGroup.AddBrain(brain);
				test = true;
			}
			Navigator component = brain.GetComponent<Navigator>();
			if (component != null)
			{
				component.executePathProbeTaskAsync = isAsyncPathProbeEnabled;
			}
		}
		DebugUtil.Assert(test);
	}

	private void OnRemoveBrain(Brain brain)
	{
		bool test = false;
		foreach (BrainGroup brainGroup in brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				test = true;
				brainGroup.RemoveBrain(brain);
			}
			Navigator component = brain.GetComponent<Navigator>();
			if (component != null)
			{
				component.executePathProbeTaskAsync = false;
			}
		}
		DebugUtil.Assert(test);
	}

	public float GetEstimatedFrameTime()
	{
		return TuningData<Tuning>.Get().frameTime;
	}

	public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
	{
		return false;
	}

	public void RenderEveryTick(float dt)
	{
		if (Game.IsQuitting() || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		foreach (BrainGroup brainGroup in brainGroups)
		{
			brainGroup.RenderEveryTick(dt, isAsyncPathProbeEnabled);
		}
	}

	protected override void OnForcedCleanUp()
	{
		CPUBudget.Remove(this);
		base.OnForcedCleanUp();
	}
}
