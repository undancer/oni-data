using System;
using System.Collections.Generic;

public class ManagementMenuNotificationDisplayer : NotificationDisplayer
{
	public List<ManagementMenuNotification> displayedManagementMenuNotifications { get; private set; }

	public event System.Action onNotificationsChanged;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		displayedManagementMenuNotifications = new List<ManagementMenuNotification>();
	}

	public void NotificationWasViewed(ManagementMenuNotification notification)
	{
		this.onNotificationsChanged();
	}

	protected override void OnNotificationAdded(Notification notification)
	{
		displayedManagementMenuNotifications.Add(notification as ManagementMenuNotification);
		this.onNotificationsChanged();
	}

	protected override void OnNotificationRemoved(Notification notification)
	{
		displayedManagementMenuNotifications.Remove(notification as ManagementMenuNotification);
		this.onNotificationsChanged();
	}

	protected override bool ShouldDisplayNotification(Notification notification)
	{
		return notification is ManagementMenuNotification;
	}

	public List<ManagementMenuNotification> GetNotificationsForAction(Action hotKey)
	{
		List<ManagementMenuNotification> list = new List<ManagementMenuNotification>();
		foreach (ManagementMenuNotification displayedManagementMenuNotification in displayedManagementMenuNotifications)
		{
			if (displayedManagementMenuNotification.targetMenu == hotKey)
			{
				list.Add(displayedManagementMenuNotification);
			}
		}
		return list;
	}
}
