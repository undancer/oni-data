using System.Collections.Generic;

public class NotificationAlertBar : KMonoBehaviour
{
	public ManagementMenuNotification notification;

	public KButton thisButton;

	public KImage background;

	public LocText text;

	public ToolTip tooltip;

	public KButton muteButton;

	public List<ColorStyleSetting> alertColorStyle;

	public void Init(ManagementMenuNotification notification)
	{
		this.notification = notification;
		thisButton.onClick += OnThisButtonClicked;
		background.colorStyleSetting = alertColorStyle[(int)notification.valence];
		background.ApplyColorStyleSetting();
		text.text = notification.titleText;
		tooltip.SetSimpleTooltip(notification.ToolTip(null, notification.tooltipData));
		muteButton.onClick += OnMuteButtonClicked;
	}

	private void OnThisButtonClicked()
	{
		NotificationHighlightController componentInParent = GetComponentInParent<NotificationHighlightController>();
		if (componentInParent != null)
		{
			componentInParent.SetActiveTarget(notification);
		}
		else
		{
			notification.View();
		}
	}

	private void OnMuteButtonClicked()
	{
	}
}
