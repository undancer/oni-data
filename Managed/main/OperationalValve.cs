using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalValve : ValveBase
{
	[MyCmpReq]
	private Operational operational;

	private bool isDispensing;

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

	protected override void OnMassTransfer(float amount)
	{
		isDispensing = amount > 0f;
	}

	public override void UpdateAnim()
	{
		if (operational.IsOperational)
		{
			if (isDispensing)
			{
				controller.Queue("on_flow", KAnim.PlayMode.Loop);
			}
			else
			{
				controller.Queue("on");
			}
		}
		else
		{
			controller.Queue("off");
		}
	}
}
