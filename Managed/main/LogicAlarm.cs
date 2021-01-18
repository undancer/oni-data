using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/LogicAlarm")]
public class LogicAlarm : KMonoBehaviour, ISaveLoadable
{
	[Serialize]
	public string notificationName;

	[Serialize]
	public string notificationTooltip;

	[Serialize]
	public NotificationType notificationType;

	[Serialize]
	public bool pauseOnNotify;

	[Serialize]
	public bool zoomOnNotify;

	[Serialize]
	public float cooldown;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private bool wasOn;

	private Notifier notifier;

	private Notification notification;

	private Notification lastNotificationCreated;

	private static readonly EventSystem.IntraObjectHandler<LogicAlarm> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicAlarm>(delegate(LogicAlarm component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicAlarm> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicAlarm>(delegate(LogicAlarm component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicAlarmInput");

	protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
	{
		"on_pre",
		"on_loop"
	};

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
	{
		"on_pst",
		"off"
	};

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicAlarm component = ((GameObject)data).GetComponent<LogicAlarm>();
		if (component != null)
		{
			notificationName = component.notificationName;
			notificationType = component.notificationType;
			pauseOnNotify = component.pauseOnNotify;
			zoomOnNotify = component.zoomOnNotify;
			cooldown = component.cooldown;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		notifier = base.gameObject.AddComponent<Notifier>();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		if (string.IsNullOrEmpty(notificationName))
		{
			notificationName = UI.UISIDESCREENS.LOGICALARMSIDESCREEN.NAME_DEFAULT;
		}
		if (string.IsNullOrEmpty(notificationTooltip))
		{
			notificationTooltip = UI.UISIDESCREENS.LOGICALARMSIDESCREEN.TOOLTIP_DEFAULT;
		}
		UpdateVisualState();
		UpdateNotification(clear: false);
	}

	private void UpdateVisualState()
	{
		GetComponent<KBatchedAnimController>().Play(wasOn ? ON_ANIMS : OFF_ANIMS);
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID != INPUT_PORT_ID)
		{
			return;
		}
		int newValue = logicValueChanged.newValue;
		if (LogicCircuitNetwork.IsBitActive(0, newValue))
		{
			if (!wasOn)
			{
				PushNotification();
				wasOn = true;
				if (pauseOnNotify && !SpeedControlScreen.Instance.IsPaused)
				{
					SpeedControlScreen.Instance.Pause(playSound: false);
				}
				if (zoomOnNotify)
				{
					Vector3 position = base.transform.position;
					CameraController.Instance.SetTargetPos(position, 8f, playSound: true);
				}
				UpdateVisualState();
			}
		}
		else if (wasOn)
		{
			wasOn = false;
			UpdateVisualState();
		}
	}

	private void PushNotification()
	{
		notification.Clear();
		notifier.Add(notification);
	}

	public void UpdateNotification(bool clear)
	{
		if (notification != null && clear)
		{
			notification.Clear();
			lastNotificationCreated = null;
		}
		if (notification != lastNotificationCreated || lastNotificationCreated == null)
		{
			notification = CreateNotification();
		}
	}

	public Notification CreateNotification()
	{
		GetComponent<KSelectable>();
		return lastNotificationCreated = new Notification(notificationName, notificationType, HashedString.Invalid, (List<Notification> n, object d) => notificationTooltip, null, expires: true, 0f, null, null, null, volume_attenuation: false);
	}
}
