using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : KScreen
{
	private class Entry
	{
		public string message;

		public int clickIdx;

		public GameObject label;

		public List<Notification> notifications = new List<Notification>();

		public Notification NextClickedNotification => notifications[clickIdx++ % notifications.Count];

		public Entry(GameObject label)
		{
			this.label = label;
		}

		public void Add(Notification notification)
		{
			notifications.Add(notification);
			UpdateMessage(notification);
		}

		public void Remove(Notification notification)
		{
			notifications.Remove(notification);
			UpdateMessage(notification, playSound: false);
		}

		public void UpdateMessage(Notification notification, bool playSound = true)
		{
			if (Game.IsQuitting())
			{
				return;
			}
			message = notification.titleText;
			if (notifications.Count > 1)
			{
				if (playSound && (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening))
				{
					Instance.PlayDingSound(notification, notifications.Count);
				}
				message = message + " (" + notifications.Count + ")";
			}
			if (label != null)
			{
				label.GetComponent<HierarchyReferences>().GetReference<LocText>("Text").text = message;
			}
		}
	}

	public float lifetime;

	public bool dirty;

	public GameObject LabelPrefab;

	public GameObject LabelsFolder;

	public GameObject MessagesPrefab;

	public GameObject MessagesFolder;

	private MessageDialogFrame messageDialog;

	private float initTime;

	[MyCmpAdd]
	private Notifier notifier;

	[SerializeField]
	private List<MessageDialog> dialogPrefabs = new List<MessageDialog>();

	[SerializeField]
	private Color badColorBG;

	[SerializeField]
	private Color badColor = Color.red;

	[SerializeField]
	private Color normalColorBG;

	[SerializeField]
	private Color normalColor = Color.white;

	[SerializeField]
	private Color warningColorBG;

	[SerializeField]
	private Color warningColor;

	[SerializeField]
	private Color messageColorBG;

	[SerializeField]
	private Color messageColor;

	[SerializeField]
	private Color eventColorBG;

	[SerializeField]
	private Color eventColor;

	public Sprite icon_normal;

	public Sprite icon_warning;

	public Sprite icon_bad;

	public Sprite icon_threatening;

	public Sprite icon_message;

	public Sprite icon_video;

	public Sprite icon_event;

	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();

	public TextStyleSetting TooltipTextStyle;

	private Dictionary<NotificationType, string> notificationSounds = new Dictionary<NotificationType, string>();

	private Dictionary<string, float> timeOfLastNotification = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private List<Entry> entries = new List<Entry>();

	private Dictionary<string, Entry> entriesByMessage = new Dictionary<string, Entry>();

	public static NotificationScreen Instance { get; private set; }

	public Color32 BadColorBG => badColorBG;

	public static void DestroyInstance()
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
		dirty = true;
		pendingNotifications.Remove(notification);
		Entry value = null;
		entriesByMessage.TryGetValue(notification.titleText, out value);
		if (value != null)
		{
			notifications.Remove(notification);
			value.Remove(notification);
			if (value.notifications.Count == 0)
			{
				UnityEngine.Object.Destroy(value.label);
				entriesByMessage[notification.titleText] = null;
				entries.Remove(value);
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		Components.Notifiers.OnAdd += OnAddNotifier;
		Components.Notifiers.OnRemove += OnRemoveNotifier;
		foreach (Notifier item in Components.Notifiers.Items)
		{
			OnAddNotifier(item);
		}
		MessagesPrefab.gameObject.SetActive(value: false);
		LabelPrefab.gameObject.SetActive(value: false);
		InitNotificationSounds();
	}

	private void OnNewMessage(object data)
	{
		Message m = (Message)data;
		notifier.Add(new MessageNotification(m));
	}

	private void ShowMessage(MessageNotification mn)
	{
		mn.message.OnClick();
		if (mn.message.ShowDialog())
		{
			for (int i = 0; i < dialogPrefabs.Count; i++)
			{
				if (dialogPrefabs[i].CanDisplay(mn.message))
				{
					if (messageDialog != null)
					{
						UnityEngine.Object.Destroy(messageDialog.gameObject);
						messageDialog = null;
					}
					messageDialog = Util.KInstantiateUI<MessageDialogFrame>(ScreenPrefabs.Instance.MessageDialogFrame.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
					MessageDialog dialog = Util.KInstantiateUI<MessageDialog>(dialogPrefabs[i].gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
					messageDialog.SetMessage(dialog, mn.message);
					messageDialog.Show();
					break;
				}
			}
		}
		Messenger.Instance.RemoveMessage(mn.message);
		mn.Clear();
	}

	public void OnClickNextMessage()
	{
		Notification notification2 = notifications.Find((Notification notification) => notification.Type == NotificationType.Messages);
		ShowMessage((MessageNotification)notification2);
	}

	protected override void OnCleanUp()
	{
		Components.Notifiers.OnAdd -= OnAddNotifier;
		Components.Notifiers.OnRemove -= OnRemoveNotifier;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		initTime = KTime.Instance.UnscaledGameTime;
		LocText[] componentsInChildren = LabelPrefab.GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = normalColor;
		}
		componentsInChildren = MessagesPrefab.GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = normalColor;
		}
		Subscribe(Messenger.Instance.gameObject, 1558809273, OnNewMessage);
		foreach (Message message in Messenger.Instance.Messages)
		{
			Notification notification = new MessageNotification(message);
			notification.playSound = false;
			notifier.Add(notification);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		dirty = true;
	}

	private void AddNotification(Notification notification)
	{
		notifications.Add(notification);
		Entry entry = null;
		entriesByMessage.TryGetValue(notification.titleText, out entry);
		if (entry == null)
		{
			HierarchyReferences hierarchyReferences = ((notification.Type != NotificationType.Messages) ? Util.KInstantiateUI<HierarchyReferences>(LabelPrefab, LabelsFolder) : Util.KInstantiateUI<HierarchyReferences>(MessagesPrefab, MessagesFolder));
			hierarchyReferences.GetReference<NotificationAnimator>("Animator").Begin();
			hierarchyReferences.gameObject.SetActive(value: true);
			Button reference = hierarchyReferences.GetReference<Button>("MainButton");
			ColorBlock colors = reference.colors;
			if (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening)
			{
				colors.normalColor = badColorBG;
			}
			else if (notification.Type == NotificationType.Messages)
			{
				colors.normalColor = messageColorBG;
				Debug.Assert(notification.GetType() == typeof(MessageNotification), $"Notification: \"{notification.titleText}\" is not of type MessageNotification");
				hierarchyReferences.GetReference<Button>("DismissButton").onClick.AddListener(delegate
				{
					foreach (MessageNotification item in notifications.FindAll((Notification n) => n.titleText == notification.titleText))
					{
						Messenger.Instance.RemoveMessage(item.message);
						item.Clear();
					}
				});
			}
			else if (notification.Type == NotificationType.Tutorial)
			{
				colors.normalColor = warningColorBG;
			}
			else if (notification.Type == NotificationType.Event)
			{
				colors.normalColor = eventColorBG;
			}
			else
			{
				colors.normalColor = normalColorBG;
			}
			reference.colors = colors;
			reference.onClick.AddListener(delegate
			{
				OnClick(entry);
			});
			if (notification.ToolTip != null)
			{
				ToolTip tooltip = hierarchyReferences.GetReference<ToolTip>("ToolTip");
				tooltip.OnToolTip = delegate
				{
					tooltip.ClearMultiStringTooltip();
					tooltip.AddMultiStringTooltip(notification.ToolTip(entry.notifications, notification.tooltipData), TooltipTextStyle);
					return "";
				};
			}
			entry = new Entry(hierarchyReferences.gameObject);
			entriesByMessage[notification.titleText] = entry;
			entries.Add(entry);
			KImage reference2 = hierarchyReferences.GetReference<KImage>("Icon");
			LocText reference3 = hierarchyReferences.GetReference<LocText>("Text");
			switch (notification.Type)
			{
			case NotificationType.Bad:
				reference3.color = badColor;
				reference2.sprite = icon_bad;
				break;
			case NotificationType.DuplicantThreatening:
				reference3.color = badColor;
				reference2.sprite = icon_threatening;
				break;
			case NotificationType.Tutorial:
				reference3.color = warningColor;
				reference2.sprite = icon_warning;
				break;
			case NotificationType.Messages:
			{
				reference3.color = messageColor;
				reference2.sprite = icon_message;
				MessageNotification messageNotification = notification as MessageNotification;
				if (messageNotification != null)
				{
					TutorialMessage tutorialMessage = messageNotification.message as TutorialMessage;
					if (tutorialMessage != null && !string.IsNullOrEmpty(tutorialMessage.videoClipId))
					{
						reference2.sprite = icon_video;
					}
				}
				break;
			}
			case NotificationType.Event:
				reference3.color = eventColor;
				reference2.sprite = icon_event;
				break;
			default:
				reference3.color = normalColor;
				reference2.sprite = icon_normal;
				break;
			}
			reference2.color = reference3.color;
			string text = "";
			if (KTime.Instance.UnscaledGameTime - initTime > 5f && notification.playSound)
			{
				PlayDingSound(notification, 0);
			}
			else
			{
				text = "too early";
			}
			if (AudioDebug.Get().debugNotificationSounds)
			{
				Debug.Log("Notification(" + notification.titleText + "):" + text);
			}
		}
		entry.Add(notification);
		entry.UpdateMessage(notification);
		dirty = true;
		SortNotifications();
	}

	private void SortNotifications()
	{
		notifications.Sort((Notification n1, Notification n2) => (n1.Type == n2.Type) ? (n1.Idx - n2.Idx) : (n1.Type - n2.Type));
		foreach (Notification notification in notifications)
		{
			Entry value = null;
			entriesByMessage.TryGetValue(notification.titleText, out value);
			value?.label.GetComponent<RectTransform>().SetAsLastSibling();
		}
	}

	private void PlayDingSound(Notification notification, int count)
	{
		if (!notificationSounds.TryGetValue(notification.Type, out var value))
		{
			value = "Notification";
		}
		if (!timeOfLastNotification.TryGetValue(value, out var value2))
		{
			value2 = 0f;
		}
		float value3 = (notification.volume_attenuation ? ((Time.time - value2) / soundDecayTime) : 1f);
		timeOfLastNotification[value] = Time.time;
		string sound;
		if (count > 1)
		{
			sound = GlobalAssets.GetSound(value + "_AddCount", force_no_warning: true);
			if (sound == null)
			{
				sound = GlobalAssets.GetSound(value);
			}
		}
		else
		{
			sound = GlobalAssets.GetSound(value);
		}
		if (notification.playSound)
		{
			EventInstance instance = KFMOD.BeginOneShot(sound, Vector3.zero);
			instance.setParameterByName("timeSinceLast", value3);
			KFMOD.EndOneShot(instance);
		}
	}

	private void Update()
	{
		int num = 0;
		while (num < pendingNotifications.Count)
		{
			if (pendingNotifications[num].IsReady())
			{
				AddNotification(pendingNotifications[num]);
				pendingNotifications.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < notifications.Count; i++)
		{
			Notification notification = notifications[i];
			if (notification.Type == NotificationType.Messages)
			{
				num3++;
			}
			else
			{
				num2++;
			}
			if (notification.expires && KTime.Instance.UnscaledGameTime - notification.Time > lifetime)
			{
				dirty = true;
				if (notification.Notifier == null)
				{
					OnRemoveNotification(notification);
				}
				else
				{
					notification.Clear();
				}
			}
		}
	}

	private void OnClick(Entry entry)
	{
		Notification nextClickedNotification = entry.NextClickedNotification;
		PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open"));
		if (nextClickedNotification.customClickCallback != null)
		{
			nextClickedNotification.customClickCallback(nextClickedNotification.customClickData);
			return;
		}
		if (nextClickedNotification.clickFocus != null)
		{
			Vector3 position = nextClickedNotification.clickFocus.GetPosition();
			position.z = -40f;
			ClusterGridEntity component = nextClickedNotification.clickFocus.GetComponent<ClusterGridEntity>();
			KSelectable component2 = nextClickedNotification.clickFocus.GetComponent<KSelectable>();
			int myWorldId = nextClickedNotification.clickFocus.gameObject.GetMyWorldId();
			if (myWorldId != -1)
			{
				CameraController.Instance.ActiveWorldStarWipe(myWorldId, position);
			}
			else if (DlcManager.FeatureClusterSpaceEnabled() && component != null && component.IsVisible)
			{
				ManagementMenu.Instance.OpenClusterMap();
				ClusterMapScreen.Instance.SetTargetFocusPosition(component.Location);
			}
			if (component2 != null)
			{
				if (DlcManager.FeatureClusterSpaceEnabled() && component != null && component.IsVisible)
				{
					ClusterMapSelectTool.Instance.Select(component2);
				}
				else
				{
					SelectTool.Instance.Select(component2);
				}
			}
		}
		else if (nextClickedNotification.Notifier != null)
		{
			SelectTool.Instance.Select(nextClickedNotification.Notifier.GetComponent<KSelectable>());
		}
		if (nextClickedNotification.Type == NotificationType.Messages)
		{
			ShowMessage((MessageNotification)nextClickedNotification);
		}
	}

	private void PositionLocatorIcon()
	{
	}

	private void InitNotificationSounds()
	{
		notificationSounds[NotificationType.Good] = "Notification";
		notificationSounds[NotificationType.BadMinor] = "Notification";
		notificationSounds[NotificationType.Bad] = "Warning";
		notificationSounds[NotificationType.Neutral] = "Notification";
		notificationSounds[NotificationType.Tutorial] = "Notification";
		notificationSounds[NotificationType.Messages] = "Message";
		notificationSounds[NotificationType.DuplicantThreatening] = "Warning_DupeThreatening";
		notificationSounds[NotificationType.Event] = "Message";
	}

	public Sprite GetNotificationIcon(NotificationType type)
	{
		return type switch
		{
			NotificationType.Bad => icon_bad, 
			NotificationType.DuplicantThreatening => icon_threatening, 
			NotificationType.Tutorial => icon_warning, 
			NotificationType.Messages => icon_message, 
			NotificationType.Event => icon_event, 
			_ => icon_normal, 
		};
	}

	public Color GetNotificationColour(NotificationType type)
	{
		return type switch
		{
			NotificationType.Bad => badColor, 
			NotificationType.DuplicantThreatening => badColor, 
			NotificationType.Tutorial => warningColor, 
			NotificationType.Messages => messageColor, 
			NotificationType.Event => eventColor, 
			_ => normalColor, 
		};
	}

	public Color GetNotificationBGColour(NotificationType type)
	{
		return type switch
		{
			NotificationType.Bad => badColorBG, 
			NotificationType.DuplicantThreatening => badColorBG, 
			NotificationType.Tutorial => warningColorBG, 
			NotificationType.Messages => messageColorBG, 
			NotificationType.Event => eventColorBG, 
			_ => normalColorBG, 
		};
	}

	public string GetNotificationSound(NotificationType type)
	{
		return notificationSounds[type];
	}
}
