using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Notifier")]
public class Notifier : KMonoBehaviour
{
	[MyCmpGet]
	private KSelectable Selectable;

	public Action<Notification> OnAdd;

	public Action<Notification> OnRemove;

	public bool DisableNotifications;

	public bool AutoClickFocus = true;

	protected override void OnPrefabInit()
	{
		Components.Notifiers.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.Notifiers.Remove(this);
	}

	public void Add(Notification notification, string suffix = "")
	{
		if (KScreenManager.Instance == null || DisableNotifications || DebugHandler.NotificationsDisabled)
		{
			return;
		}
		DebugUtil.DevAssert(notification != null, "Trying to add null notification. It's safe to continue playing, the notification won't be displayed.");
		if (notification == null)
		{
			return;
		}
		if (notification.Notifier == null)
		{
			if (Selectable != null)
			{
				notification.NotifierName = "• " + Selectable.GetName() + suffix;
			}
			else
			{
				notification.NotifierName = "• " + base.name + suffix;
			}
			notification.Notifier = this;
			if (AutoClickFocus && notification.clickFocus == null)
			{
				notification.clickFocus = base.transform;
			}
			if (OnAdd != null)
			{
				OnAdd(notification);
			}
			notification.GameTime = Time.time;
		}
		else
		{
			DebugUtil.Assert(notification.Notifier == this);
		}
		notification.Time = KTime.Instance.UnscaledGameTime;
	}

	public void Remove(Notification notification)
	{
		DebugUtil.DevAssert(notification != null, "Trying to remove null notification. It's safe to continue playing.");
		if (notification != null && notification.Notifier != null)
		{
			notification.Notifier = null;
			if (OnRemove != null)
			{
				OnRemove(notification);
			}
		}
	}
}
