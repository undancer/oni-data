using System;
using KSerialization;

public class LogicBroadcaster : KMonoBehaviour, ISimEveryTick
{
	public static int RANGE = 5;

	private static int INVALID_CHANNEL_ID = -1;

	public string PORT_ID = "";

	private bool wasOperational;

	[Serialize]
	private int broadcastChannelID = INVALID_CHANNEL_ID;

	private static readonly EventSystem.IntraObjectHandler<LogicBroadcaster> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicBroadcaster>(delegate(LogicBroadcaster component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static readonly Operational.Flag spaceVisible = new Operational.Flag("spaceVisible", Operational.Flag.Type.Requirement);

	private Guid spaceNotVisibleStatusItem = Guid.Empty;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private KBatchedAnimController animController;

	public int BroadCastChannelID
	{
		get
		{
			return broadcastChannelID;
		}
		private set
		{
			broadcastChannelID = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.LogicBroadcasters.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.LogicBroadcasters.Remove(this);
		base.OnCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, OnOperationalChanged);
		operational.SetFlag(spaceVisible, IsSpaceVisible());
		wasOperational = !operational.IsOperational;
		OnOperationalChanged(null);
	}

	public bool IsSpaceVisible()
	{
		if (!base.gameObject.GetMyWorld().IsModuleInterior)
		{
			return Grid.ExposedToSunlight[Grid.PosToCell(base.gameObject)] > 0;
		}
		return true;
	}

	public int GetCurrentValue()
	{
		return GetComponent<LogicPorts>().GetInputValue(PORT_ID);
	}

	private void OnLogicValueChanged(object data)
	{
	}

	public void SimEveryTick(float dt)
	{
		bool flag = IsSpaceVisible();
		operational.SetFlag(spaceVisible, flag);
		if (!flag)
		{
			if (spaceNotVisibleStatusItem == Guid.Empty)
			{
				spaceNotVisibleStatusItem = GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight);
			}
		}
		else if (spaceNotVisibleStatusItem != Guid.Empty)
		{
			GetComponent<KSelectable>().RemoveStatusItem(spaceNotVisibleStatusItem);
			spaceNotVisibleStatusItem = Guid.Empty;
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (operational.IsOperational)
		{
			if (!wasOperational)
			{
				wasOperational = true;
				animController.Queue("on_pre");
				animController.Queue("on", KAnim.PlayMode.Loop);
			}
		}
		else if (wasOperational)
		{
			wasOperational = false;
			animController.Queue("on_pst");
			animController.Queue("off", KAnim.PlayMode.Loop);
		}
	}
}
