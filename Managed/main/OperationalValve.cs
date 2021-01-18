using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalValve : ValveBase
{
	[MyCmpReq]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<OperationalValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalValve>(delegate(OperationalValve component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-592767678, OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		OnOperationalChanged(operational.IsOperational);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Unsubscribe(-592767678, OnOperationalChangedDelegate);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			base.CurrentFlow = base.MaxFlow;
		}
		else
		{
			base.CurrentFlow = 0f;
		}
		operational.SetActive(flag);
	}

	public override void UpdateAnim()
	{
		float averageRate = Game.Instance.accumulators.GetAverageRate(flowAccumulator);
		if (operational.IsOperational)
		{
			if (averageRate > 0f)
			{
				controller.Queue("on_flow", KAnim.PlayMode.Loop);
			}
			else
			{
				controller.Queue("on");
			}
		}
		else if (averageRate > 0f)
		{
			controller.Queue("off_flow", KAnim.PlayMode.Loop);
		}
		else
		{
			controller.Queue("off");
		}
	}
}
