using System;
using System.Collections.Generic;

public class NotificationManager : KMonoBehaviour
{
	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();

	public static NotificationManager Instance { get; private set; }

	public event Action<Notification> notificationAdded;

	public event Action<Notification> notificationRemoved;

	protected override void OnPrefabInit()
	{
		Debug.Assert(Instance == null);
		Instance = this;
		Components.Notifiers.OnAdd += OnAddNotifier;
		Components.Notifiers.OnRemove += OnRemoveNotifier;
		foreach (Notifier item in Components.Notifiers.Items)
		{
			OnAddNotifier(item);
		}
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	private void OnAddNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Combine(notifier.OnAdd, new Action<Notification>(OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Combine(notifier.OnRemove, new Action<Notification>(OnRemoveNotification));
	}

	private void OnRemoveNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Remove(notifier.OnAdd, new Action<Notification>(OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Remove(notifier.OnRemove, new Action<Notification>(OnRemoveNotification));
	}

	private void OnAddNotification(Notification notification)
	{
		pendingNotifications.Add(notification);
	}

	private void OnRemoveNotification(Notification notification)
	{
		pendingNotifications.Remove(notification);
		if (notifications.Remove(notification))
		{
			this.notificationRemoved(notification);
		}
	}

	private void Update()
	{
		int num = 0;
		while (num < pendingNotifications.Count)
		{
			if (pendingNotifications[num].IsReady())
			{
				DoAddNotification(pendingNotifications[num]);
				pendingNotifications.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	private void DoAddNotification(Notification notification)
	{
		notifications.Add(notification);
		if (this.notificationAdded != null)
		{
			this.notificationAdded(notification);
		}
	}
}
