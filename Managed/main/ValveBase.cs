using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ValveBase")]
public class ValveBase : KMonoBehaviour, ISaveLoadable
{
	[Serializable]
	public struct AnimRangeInfo
	{
		public float minFlow;

		public string animName;

		public AnimRangeInfo(float min_flow, string anim_name)
		{
			minFlow = min_flow;
			animName = anim_name;
		}
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public float maxFlow = 0.5f;

	[Serialize]
	private float currentFlow;

	[MyCmpGet]
	protected KBatchedAnimController controller;

	protected HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;

	private int curFlowIdx = -1;

	private int inputCell;

	private int outputCell;

	[SerializeField]
	public AnimRangeInfo[] animFlowRanges;

	public float CurrentFlow
	{
		get
		{
			return currentFlow;
		}
		set
		{
			currentFlow = value;
		}
	}

	public HandleVector<int>.Handle AccumulatorHandle => flowAccumulator;

	public float MaxFlow => maxFlow;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		flowAccumulator = Game.Instance.accumulators.Add("Flow", this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		Conduit.GetFlowManager(conduitType).AddConduitUpdater(ConduitUpdate);
		UpdateAnim();
		OnCmpEnable();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(flowAccumulator);
		Conduit.GetFlowManager(conduitType).RemoveConduitUpdater(ConduitUpdate);
		base.OnCleanUp();
	}

	private void ConduitUpdate(float dt)
	{
		ConduitFlow flowManager = Conduit.GetFlowManager(conduitType);
		ConduitFlow.Conduit conduit = flowManager.GetConduit(inputCell);
		if (!flowManager.HasConduit(inputCell) || !flowManager.HasConduit(outputCell))
		{
			UpdateAnim();
			return;
		}
		ConduitFlow.ConduitContents contents = conduit.GetContents(flowManager);
		float num = Mathf.Min(contents.mass, currentFlow * dt);
		if (num > 0f)
		{
			float num2 = num / contents.mass;
			int disease_count = (int)(num2 * (float)contents.diseaseCount);
			float num3 = flowManager.AddElement(outputCell, contents.element, num, contents.temperature, contents.diseaseIdx, disease_count);
			Game.Instance.accumulators.Accumulate(flowAccumulator, num3);
			if (num3 > 0f)
			{
				flowManager.RemoveElement(inputCell, num3);
			}
		}
		UpdateAnim();
	}

	public virtual void UpdateAnim()
	{
		float averageRate = Game.Instance.accumulators.GetAverageRate(flowAccumulator);
		if (averageRate > 0f)
		{
			for (int i = 0; i < animFlowRanges.Length; i++)
			{
				if (averageRate <= animFlowRanges[i].minFlow)
				{
					if (curFlowIdx != i)
					{
						curFlowIdx = i;
						controller.Play(animFlowRanges[i].animName, (averageRate <= 0f) ? KAnim.PlayMode.Once : KAnim.PlayMode.Loop);
					}
					break;
				}
			}
		}
		else
		{
			controller.Play("off");
		}
	}
}
