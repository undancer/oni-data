using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AlarmSideScreen : SideScreenContent
{
	public LogicAlarm targetAlarm;

	[SerializeField]
	private KInputField nameInputField;

	[SerializeField]
	private KInputField tooltipInputField;

	[SerializeField]
	private KToggle pauseToggle;

	[SerializeField]
	private Image pauseCheckmark;

	[SerializeField]
	private KToggle zoomToggle;

	[SerializeField]
	private Image zoomCheckmark;

	[SerializeField]
	private GameObject typeButtonPrefab;

	private List<NotificationType> validTypes = new List<NotificationType>
	{
		NotificationType.Bad,
		NotificationType.Neutral,
		NotificationType.DuplicantThreatening
	};

	private Dictionary<NotificationType, MultiToggle> toggles_by_type = new Dictionary<NotificationType, MultiToggle>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		nameInputField.onEndEdit += OnEndEditName;
		nameInputField.field.characterLimit = 30;
		tooltipInputField.onEndEdit += OnEndEditTooltip;
		tooltipInputField.field.characterLimit = 90;
		pauseToggle.onClick += TogglePause;
		zoomToggle.onClick += ToggleZoom;
		InitializeToggles();
	}

	private void OnEndEditName()
	{
		targetAlarm.notificationName = nameInputField.field.text;
		UpdateNotification(clear: true);
	}

	private void OnEndEditTooltip()
	{
		targetAlarm.notificationTooltip = tooltipInputField.field.text;
		UpdateNotification(clear: true);
	}

	private void TogglePause()
	{
		targetAlarm.pauseOnNotify = !targetAlarm.pauseOnNotify;
		pauseCheckmark.enabled = targetAlarm.pauseOnNotify;
		UpdateNotification(clear: true);
	}

	private void ToggleZoom()
	{
		targetAlarm.zoomOnNotify = !targetAlarm.zoomOnNotify;
		zoomCheckmark.enabled = targetAlarm.zoomOnNotify;
		UpdateNotification(clear: true);
	}

	private void SelectType(NotificationType type)
	{
		targetAlarm.notificationType = type;
		UpdateNotification(clear: true);
		RefreshToggles();
	}

	private void InitializeToggles()
	{
		if (toggles_by_type.Count != 0)
		{
			return;
		}
		foreach (NotificationType type in validTypes)
		{
			GameObject gameObject = Util.KInstantiateUI(typeButtonPrefab, typeButtonPrefab.transform.parent.gameObject, force_active: true);
			gameObject.name = "TypeButton: " + type;
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			Color notificationBGColour = NotificationScreen.Instance.GetNotificationBGColour(type);
			Color notificationColour = NotificationScreen.Instance.GetNotificationColour(type);
			notificationBGColour.a = 1f;
			notificationColour.a = 1f;
			component.GetReference<KImage>("bg").color = notificationBGColour;
			component.GetReference<KImage>("icon").color = notificationColour;
			component.GetReference<KImage>("icon").sprite = NotificationScreen.Instance.GetNotificationIcon(type);
			ToolTip component2 = gameObject.GetComponent<ToolTip>();
			switch (type)
			{
			case NotificationType.Bad:
				component2.SetSimpleTooltip(UI.UISIDESCREENS.LOGICALARMSIDESCREEN.TOOLTIPS.BAD);
				break;
			case NotificationType.Neutral:
				component2.SetSimpleTooltip(UI.UISIDESCREENS.LOGICALARMSIDESCREEN.TOOLTIPS.NEUTRAL);
				break;
			case NotificationType.DuplicantThreatening:
				component2.SetSimpleTooltip(UI.UISIDESCREENS.LOGICALARMSIDESCREEN.TOOLTIPS.DUPLICANT_THREATENING);
				break;
			}
			if (!toggles_by_type.ContainsKey(type))
			{
				toggles_by_type.Add(type, gameObject.GetComponent<MultiToggle>());
			}
			toggles_by_type[type].onClick = delegate
			{
				SelectType(type);
			};
			for (int i = 0; i < toggles_by_type[type].states.Length; i++)
			{
				toggles_by_type[type].states[i].on_click_override_sound_path = NotificationScreen.Instance.GetNotificationSound(type);
			}
		}
	}

	private void RefreshToggles()
	{
		InitializeToggles();
		foreach (KeyValuePair<NotificationType, MultiToggle> item in toggles_by_type)
		{
			if (targetAlarm.notificationType == item.Key)
			{
				item.Value.ChangeState(0);
			}
			else
			{
				item.Value.ChangeState(1);
			}
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LogicAlarm>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		targetAlarm = target.GetComponent<LogicAlarm>();
		RefreshToggles();
		UpdateVisuals();
	}

	private void UpdateNotification(bool clear)
	{
		targetAlarm.UpdateNotification(clear);
	}

	private void UpdateVisuals()
	{
		nameInputField.SetDisplayValue(targetAlarm.notificationName);
		tooltipInputField.SetDisplayValue(targetAlarm.notificationTooltip);
		pauseCheckmark.enabled = targetAlarm.pauseOnNotify;
		zoomCheckmark.enabled = targetAlarm.zoomOnNotify;
	}
}
