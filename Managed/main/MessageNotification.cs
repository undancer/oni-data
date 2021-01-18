using System.Collections.Generic;

public class MessageNotification : Notification
{
	public Message message;

	private string OnToolTip(List<Notification> notifications, string tooltipText)
	{
		return tooltipText;
	}

	public MessageNotification(Message m)
		: base(m.GetTitle(), NotificationType.Messages, HashedString.Invalid, null, null, expires: false)
	{
		MessageNotification messageNotification = this;
		message = m;
		if (!message.PlayNotificationSound())
		{
			playSound = false;
		}
		base.ToolTip = (List<Notification> notifications, object data) => messageNotification.OnToolTip(notifications, m.GetTooltip());
		base.clickFocus = null;
	}
}
