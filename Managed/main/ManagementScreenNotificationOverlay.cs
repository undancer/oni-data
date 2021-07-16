using System.Collections.Generic;
using UnityEngine;

public class ManagementScreenNotificationOverlay : KMonoBehaviour
{
	public Action currentMenu;

	public NotificationAlertBar alertBarPrefab;

	public RectTransform alertContainer;

	private List<NotificationAlertBar> alertBars = new List<NotificationAlertBar>();

	protected void OnEnable()
	{
	}

	protected void OnDisable()
	{
	}

	private NotificationAlertBar CreateAlertBar(ManagementMenuNotification notification)
	{
		NotificationAlertBar notificationAlertBar = Util.KInstantiateUI<NotificationAlertBar>(alertBarPrefab.gameObject, alertContainer.gameObject);
		notificationAlertBar.Init(notification);
		notificationAlertBar.gameObject.SetActive(value: true);
		return notificationAlertBar;
	}

	private void NotificationsChanged()
	{
	}
}
