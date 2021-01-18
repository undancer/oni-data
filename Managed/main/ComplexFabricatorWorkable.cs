using System;
using Klei.AI;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ComplexFabricatorWorkable")]
public class ComplexFabricatorWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private ComplexFabricator fabricator;

	public Action<Worker, float> OnWorkTickActions;

	public MeterController meter;

	protected GameObject visualizer;

	protected KAnimLink visualizerLink;

	public StatusItem WorkerStatusItem
	{
		get
		{
			return workerStatusItem;
		}
		set
		{
			workerStatusItem = value;
		}
	}

	public AttributeConverter AttributeConverter
	{
		get
		{
			return attributeConverter;
		}
		set
		{
			attributeConverter = value;
		}
	}

	public float AttributeExperienceMultiplier
	{
		get
		{
			return attributeExperienceMultiplier;
		}
		set
		{
			attributeExperienceMultiplier = value;
		}
	}

	public string SkillExperienceSkillGroup
	{
		set
		{
			skillExperienceSkillGroup = value;
		}
	}

	public float SkillExperienceMultiplier
	{
		set
		{
			skillExperienceMultiplier = value;
		}
	}

	public ComplexRecipe CurrentWorkingOrder => (fabricator != null) ? fabricator.CurrentWorkingOrder : null;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
	}

	public override string GetConversationTopic()
	{
		string conversationTopic = fabricator.GetConversationTopic();
		return (conversationTopic != null) ? conversationTopic : base.GetConversationTopic();
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (operational.IsOperational)
		{
			if (fabricator.CurrentWorkingOrder != null)
			{
				InstantiateVisualizer(fabricator.CurrentWorkingOrder);
				return;
			}
			DebugUtil.DevAssertArgs(false, "ComplexFabricatorWorkable.OnStartWork called but CurrentMachineOrder is null", base.gameObject);
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (OnWorkTickActions != null)
		{
			OnWorkTickActions(worker, dt);
		}
		UpdateOrderProgress(worker, dt);
		return base.OnWorkTick(worker, dt);
	}

	public override float GetWorkTime()
	{
		ComplexRecipe currentWorkingOrder = fabricator.CurrentWorkingOrder;
		if (currentWorkingOrder != null)
		{
			workTime = currentWorkingOrder.time;
			return workTime;
		}
		return -1f;
	}

	public Chore CreateWorkChore(ChoreType choreType, float order_progress)
	{
		WorkChore<ComplexFabricatorWorkable> result = new WorkChore<ComplexFabricatorWorkable>(choreType, this);
		workTimeRemaining = GetWorkTime() * (1f - order_progress);
		return result;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		fabricator.CompleteWorkingOrder();
		DestroyVisualizer();
	}

	private void InstantiateVisualizer(ComplexRecipe recipe)
	{
		if (visualizer != null)
		{
			DestroyVisualizer();
		}
		if (visualizerLink != null)
		{
			visualizerLink.Unregister();
			visualizerLink = null;
		}
		if (!(recipe.FabricationVisualizer == null))
		{
			visualizer = Util.KInstantiate(recipe.FabricationVisualizer);
			visualizer.transform.parent = meter.meterController.transform;
			visualizer.transform.SetLocalPosition(new Vector3(0f, 0f, 1f));
			visualizer.SetActive(value: true);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			KBatchedAnimController component2 = visualizer.GetComponent<KBatchedAnimController>();
			visualizerLink = new KAnimLink(component, component2);
		}
	}

	private void UpdateOrderProgress(Worker worker, float dt)
	{
		float workTime = GetWorkTime();
		float num = Mathf.Clamp01((workTime - base.WorkTimeRemaining) / workTime);
		if ((bool)fabricator)
		{
			fabricator.OrderProgress = num;
		}
		if (meter != null)
		{
			meter.SetPositionPercent(num);
		}
	}

	private void DestroyVisualizer()
	{
		if (visualizer != null)
		{
			if (visualizerLink != null)
			{
				visualizerLink.Unregister();
				visualizerLink = null;
			}
			Util.KDestroyGameObject(visualizer);
			visualizer = null;
		}
	}
}
