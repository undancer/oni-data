using System.Collections.Generic;

public abstract class NotificationDisplayer : KMonoBehaviour
{
	protected List<Notification> displayedNotifications;

	protected override void OnSpawn()
	{
		displayedNotifications = new List<Notification>();
		NotificationManager.Instance.notificationAdded += NotificationAdded;
		NotificationManager.Instance.notificationRemoved += NotificationRemoved;
	}

	public void NotificationAdded(Notification notification)
	{
		if (ShouldDisplayNotification(notification))
		{
			displayedNotifications.Add(notification);
			OnNotificationAdded(notification);
		}
	}

	protected abstract void OnNotificationAdded(Notification notification);

	public void NotificationRemoved(Notification notification)
	{
		if (displayedNotifications.Contains(notification))
		{
			displayedNotifications.Remove(notification);
			OnNotificationRemoved(notification);
		}
	}

	protected abstract void OnNotificationRemoved(Notification notification);

	protected abstract bool ShouldDisplayNotification(Notification notification);
}
