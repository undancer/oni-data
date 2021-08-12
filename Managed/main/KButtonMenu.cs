using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KButtonMenu : KScreen
{
	public class ButtonInfo
	{
		public delegate void HoverCallback(GameObject hoverTarget);

		public delegate void Callback();

		public string text;

		public Action shortcutKey;

		public GameObject visualizer;

		public UnityAction onClick;

		public HoverCallback onHover;

		public FMODAsset clickSound;

		public KButton uibutton;

		public string toolTip;

		public bool isEnabled = true;

		public string[] popupOptions;

		public Action<string> onPopupClick;

		public Func<string[]> onPopulatePopup;

		public object userData;

		public ButtonInfo(string text = null, UnityAction on_click = null, Action shortcut_key = Action.NumActions, HoverCallback on_hover = null, string tool_tip = null, GameObject visualizer = null, bool is_enabled = true, string[] popup_options = null, Action<string> on_popup_click = null, Func<string[]> on_populate_popup = null)
		{
			this.text = text;
			shortcutKey = shortcut_key;
			onClick = on_click;
			onHover = on_hover;
			this.visualizer = visualizer;
			toolTip = tool_tip;
			isEnabled = is_enabled;
			uibutton = null;
			popupOptions = popup_options;
			onPopupClick = on_popup_click;
			onPopulatePopup = on_populate_popup;
		}

		public ButtonInfo(string text, Action shortcutKey, UnityAction onClick, HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.userData = userData;
			visualizer = null;
			uibutton = null;
		}

		public ButtonInfo(string text, GameObject visualizer, Action shortcutKey, UnityAction onClick, HoverCallback onHover = null, object userData = null)
		{
			this.text = text;
			this.shortcutKey = shortcutKey;
			this.onClick = onClick;
			this.onHover = onHover;
			this.visualizer = visualizer;
			this.userData = userData;
			uibutton = null;
		}
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	protected Transform buttonParent;

	public GameObject buttonPrefab;

	public bool ShouldConsumeMouseScroll;

	[NonSerialized]
	public GameObject[] buttonObjects;

	protected GameObject go;

	protected IList<ButtonInfo> buttons;

	private static readonly EventSystem.IntraObjectHandler<KButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KButtonMenu>(delegate(KButtonMenu component, object data)
	{
		component.OnSetActivator(data);
	});

	protected override void OnActivate()
	{
		base.ConsumeMouseScroll = ShouldConsumeMouseScroll;
		RefreshButtons();
	}

	public void SetButtons(IList<ButtonInfo> buttons)
	{
		this.buttons = buttons;
		if (activateOnSpawn)
		{
			RefreshButtons();
		}
	}

	public virtual void RefreshButtons()
	{
		if (buttonObjects != null)
		{
			for (int i = 0; i < buttonObjects.Length; i++)
			{
				UnityEngine.Object.Destroy(buttonObjects[i]);
			}
			buttonObjects = null;
		}
		if (buttons == null)
		{
			return;
		}
		buttonObjects = new GameObject[buttons.Count];
		for (int j = 0; j < buttons.Count; j++)
		{
			ButtonInfo binfo = buttons[j];
			GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
			buttonObjects[j] = gameObject;
			Transform parent = ((buttonParent != null) ? buttonParent : base.transform);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.SetActive(value: true);
			gameObject.name = binfo.text + "Button";
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(includeInactive: true);
			if (componentsInChildren != null)
			{
				LocText[] array = componentsInChildren;
				foreach (LocText obj in array)
				{
					obj.text = ((obj.name == "Hotkey") ? GameUtil.GetActionString(binfo.shortcutKey) : binfo.text);
					obj.color = (binfo.isEnabled ? new Color(1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f));
				}
			}
			ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
			if (binfo.toolTip != null && binfo.toolTip != "" && componentInChildren != null)
			{
				componentInChildren.toolTip = binfo.toolTip;
			}
			KButtonMenu screen = this;
			KButton button = gameObject.GetComponent<KButton>();
			button.isInteractable = binfo.isEnabled;
			if (binfo.popupOptions == null && binfo.onPopulatePopup == null)
			{
				UnityAction onClick = binfo.onClick;
				System.Action value = delegate
				{
					onClick();
					if (!keepMenuOpen && screen != null)
					{
						screen.Deactivate();
					}
				};
				button.onClick += value;
			}
			else
			{
				button.onClick += delegate
				{
					SetupPopupMenu(binfo, button);
				};
			}
			binfo.uibutton = button;
			_ = binfo.onHover;
		}
		Update();
	}

	protected Button.ButtonClickedEvent SetupPopupMenu(ButtonInfo binfo, KButton button)
	{
		Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
		UnityAction unityAction = delegate
		{
			List<ButtonInfo> list = new List<ButtonInfo>();
			if (binfo.onPopulatePopup != null)
			{
				binfo.popupOptions = binfo.onPopulatePopup();
			}
			string[] popupOptions = binfo.popupOptions;
			foreach (string text in popupOptions)
			{
				string delegate_str = text;
				list.Add(new ButtonInfo(delegate_str, delegate
				{
					binfo.onPopupClick(delegate_str);
					if (!keepMenuOpen)
					{
						Deactivate();
					}
				}));
			}
			KButtonMenu component = Util.KInstantiate(ScreenPrefabs.Instance.ButtonGrid.gameObject).GetComponent<KButtonMenu>();
			component.SetButtons(list.ToArray());
			RootMenu.Instance.AddSubMenu(component);
			Game.Instance.LocalPlayer.ScreenManager.ActivateScreen(component.gameObject);
			Vector3 vector = default(Vector3);
			if (Util.IsOnLeftSideOfScreen(button.transform.GetPosition()))
			{
				vector.x = button.GetComponent<RectTransform>().rect.width * 0.25f;
			}
			else
			{
				vector.x = (0f - button.GetComponent<RectTransform>().rect.width) * 0.25f;
			}
			component.transform.SetPosition(button.transform.GetPosition() + vector);
		};
		binfo.onClick = unityAction;
		buttonClickedEvent.AddListener(unityAction);
		return buttonClickedEvent;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (buttons == null)
		{
			return;
		}
		for (int i = 0; i < buttons.Count; i++)
		{
			ButtonInfo buttonInfo = buttons[i];
			if (e.TryConsume(buttonInfo.shortcutKey))
			{
				buttonObjects[i].GetComponent<KButton>().PlayPointerDownSound();
				buttonObjects[i].GetComponent<KButton>().SignalClick(KKeyCode.Mouse0);
				break;
			}
		}
		base.OnKeyDown(e);
	}

	protected override void OnPrefabInit()
	{
		Subscribe(315865555, OnSetActivatorDelegate);
	}

	private void OnSetActivator(object data)
	{
		go = (GameObject)data;
		Update();
	}

	protected override void OnDeactivate()
	{
	}

	private void Update()
	{
		if (followGameObject && !(go == null) && !(base.canvas == null))
		{
			Vector3 vector = Camera.main.WorldToViewportPoint(go.transform.GetPosition());
			RectTransform component = GetComponent<RectTransform>();
			RectTransform component2 = base.canvas.GetComponent<RectTransform>();
			if (component != null)
			{
				component.anchoredPosition = new Vector2(vector.x * component2.sizeDelta.x - component2.sizeDelta.x * 0.5f, vector.y * component2.sizeDelta.y - component2.sizeDelta.y * 0.5f);
			}
		}
	}
}
