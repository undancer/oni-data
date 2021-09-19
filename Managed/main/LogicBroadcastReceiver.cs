using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class LogicBroadcastReceiver : KMonoBehaviour, ISimEveryTick
{
	[Serialize]
	private Ref<LogicBroadcaster> channel = new Ref<LogicBroadcaster>();

	public string PORT_ID = "";

	private List<int> channelEventListeners = new List<int>();

	private bool syncToChannelComplete;

	public static readonly Operational.Flag spaceVisible = new Operational.Flag("spaceVisible", Operational.Flag.Type.Requirement);

	public static readonly Operational.Flag validChannelInRange = new Operational.Flag("validChannelInRange", Operational.Flag.Type.Requirement);

	[MyCmpGet]
	private Operational operational;

	private bool wasOperational;

	[MyCmpGet]
	private KBatchedAnimController animController;

	private Guid rangeStatusItem = Guid.Empty;

	private Guid spaceNotVisibleStatusItem = Guid.Empty;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-592767678, OnOperationalChanged);
		SetChannel(channel.Get());
		operational.SetFlag(spaceVisible, IsSpaceVisible());
		operational.SetFlag(validChannelInRange, CheckChannelValid() && CheckRange(channel.Get().gameObject, base.gameObject));
		wasOperational = !operational.IsOperational;
		OnOperationalChanged(null);
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
		bool flag2 = CheckChannelValid() && CheckRange(channel.Get().gameObject, base.gameObject);
		operational.SetFlag(validChannelInRange, flag2);
		if (flag2 && !syncToChannelComplete)
		{
			SyncWithBroadcast();
		}
	}

	public bool IsSpaceVisible()
	{
		if (!base.gameObject.GetMyWorld().IsModuleInterior)
		{
			return Grid.ExposedToSunlight[Grid.PosToCell(base.gameObject)] > 0;
		}
		return true;
	}

	private bool CheckChannelValid()
	{
		if (channel.Get() != null)
		{
			return channel.Get().GetComponent<LogicPorts>().inputPorts != null;
		}
		return false;
	}

	public LogicBroadcaster GetChannel()
	{
		return channel.Get();
	}

	public void SetChannel(LogicBroadcaster broadcaster)
	{
		ClearChannel();
		if (!(broadcaster == null))
		{
			channel.Set(broadcaster);
			syncToChannelComplete = false;
			channelEventListeners.Add(channel.Get().gameObject.Subscribe(-801688580, OnChannelLogicEvent));
			channelEventListeners.Add(channel.Get().gameObject.Subscribe(-592767678, OnChannelLogicEvent));
			SyncWithBroadcast();
		}
	}

	private void ClearChannel()
	{
		if (CheckChannelValid())
		{
			for (int i = 0; i < channelEventListeners.Count; i++)
			{
				channel.Get().gameObject.Unsubscribe(channelEventListeners[i]);
			}
		}
		channelEventListeners.Clear();
	}

	private void OnChannelLogicEvent(object data)
	{
		if (channel.Get().GetComponent<Operational>().IsOperational)
		{
			SyncWithBroadcast();
		}
	}

	private void SyncWithBroadcast()
	{
		if (CheckChannelValid())
		{
			bool flag = CheckRange(channel.Get().gameObject, base.gameObject);
			UpdateRangeStatus(flag);
			if (flag)
			{
				GetComponent<LogicPorts>().SendSignal(PORT_ID, channel.Get().GetCurrentValue());
				syncToChannelComplete = true;
			}
		}
	}

	public static bool CheckRange(GameObject broadcaster, GameObject receiver)
	{
		return AxialUtil.GetDistance(broadcaster.GetMyWorldLocation(), receiver.GetMyWorldLocation()) <= LogicBroadcaster.RANGE;
	}

	private void UpdateRangeStatus(bool inRange)
	{
		if (!inRange && rangeStatusItem == Guid.Empty)
		{
			KSelectable component = GetComponent<KSelectable>();
			rangeStatusItem = component.AddStatusItem(Db.Get().BuildingStatusItems.BroadcasterOutOfRange);
		}
		else if (rangeStatusItem != Guid.Empty)
		{
			GetComponent<KSelectable>().RemoveStatusItem(rangeStatusItem);
			rangeStatusItem = Guid.Empty;
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

	protected override void OnCleanUp()
	{
		ClearChannel();
		base.OnCleanUp();
	}
}
