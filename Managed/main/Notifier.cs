using System;
using System.Collections.Generic;
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

	private Dictionary<HashedString, Notification> NotificationGroups;

	protected override void OnPrefabInit()
	{
		Components.Notifiers.Add(this);
	}

	protected override void OnCleanUp()
	{
		ClearNotifications();
		Components.Notifiers.Remove(this);
	}

	public void Add(Notification notification, string suffix = "")
	{
		if (KScreenManager.Instance == null || DisableNotifications)
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
			if (notification.Group.IsValid && notification.Group != "")
			{
				if (NotificationGroups == null)
				{
					NotificationGroups = new Dictionary<HashedString, Notification>();
				}
				NotificationGroups.TryGetValue(notification.Group, out var value);
				if (value != null)
				{
					Remove(value);
				}
				NotificationGroups[notification.Group] = notification;
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
		if (notification.Notifier != null)
		{
			notification.Notifier = null;
			if (NotificationGroups != null && notification.Group.IsValid && notification.Group != "")
			{
				NotificationGroups.Remove(notification.Group);
			}
			if (OnRemove != null)
			{
				OnRemove(notification);
			}
		}
	}

	public void ClearNotifications()
	{
		if (NotificationGroups == null)
		{
			return;
		}
		foreach (HashedString item in new List<HashedString>(NotificationGroups.Keys))
		{
			Remove(NotificationGroups[item]);
		}
	}
}
