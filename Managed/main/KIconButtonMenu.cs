using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KIconButtonMenu : KScreen
{
	public class ButtonInfo
	{
		public delegate void Callback();

		public string iconName;

		public string text;

		public string tooltipText;

		public string[] multiText;

		public Action shortcutKey;

		public bool isInteractable;

		public Action<ButtonInfo> onCreate;

		public System.Action onClick;

		public Func<string> onToolTip;

		public GameObject buttonGo;

		public object userData;

		public Texture texture;

		public ButtonInfo(string iconName = "", string text = "", System.Action on_click = null, Action shortcutKey = Action.NumActions, Action<GameObject> on_refresh = null, Action<ButtonInfo> on_create = null, Texture texture = null, string tooltipText = "", bool is_interactable = true)
		{
			this.iconName = iconName;
			this.text = text;
			this.shortcutKey = shortcutKey;
			onClick = on_click;
			onCreate = on_create;
			this.texture = texture;
			this.tooltipText = tooltipText;
			isInteractable = is_interactable;
		}

		public string GetTooltipText()
		{
			string text = ((tooltipText == "") ? this.text : tooltipText);
			if (shortcutKey != Action.NumActions)
			{
				text = GameUtil.ReplaceHotkeyString(text, shortcutKey);
			}
			return text;
		}
	}

	[SerializeField]
	protected bool followGameObject;

	[SerializeField]
	protected bool keepMenuOpen;

	[SerializeField]
	protected bool automaticNavigation = true;

	[SerializeField]
	protected Transform buttonParent;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	protected Sprite[] icons;

	[SerializeField]
	private ToggleGroup externalToggleGroup;

	protected KToggle currentlySelectedToggle;

	[NonSerialized]
	public GameObject[] buttonObjects;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	protected GameObject go;

	protected IList<ButtonInfo> buttons;

	private static readonly EventSystem.IntraObjectHandler<KIconButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KIconButtonMenu>(delegate(KIconButtonMenu component, object data)
	{
		component.OnSetActivator(data);
	});

	protected override void OnActivate()
	{
		base.OnActivate();
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
		if (buttons == null || buttons.Count == 0)
		{
			return;
		}
		buttonObjects = new GameObject[buttons.Count];
		for (int j = 0; j < buttons.Count; j++)
		{
			ButtonInfo buttonInfo = buttons[j];
			if (buttonInfo == null)
			{
				continue;
			}
			GameObject binstance = UnityEngine.Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
			buttonInfo.buttonGo = binstance;
			buttonObjects[j] = binstance;
			Transform transform = null;
			transform = ((buttonParent != null) ? buttonParent : base.transform);
			binstance.transform.SetParent(transform, worldPositionStays: false);
			binstance.SetActive(value: true);
			binstance.name = buttonInfo.text + "Button";
			KButton component = binstance.GetComponent<KButton>();
			if (component != null && buttonInfo.onClick != null)
			{
				component.onClick += buttonInfo.onClick;
			}
			Image image = null;
			if ((bool)component)
			{
				image = component.fgImage;
			}
			if (image != null)
			{
				image.gameObject.SetActive(value: false);
				Sprite[] array = icons;
				foreach (Sprite sprite in array)
				{
					if (sprite != null && sprite.name == buttonInfo.iconName)
					{
						image.sprite = sprite;
						image.gameObject.SetActive(value: true);
						break;
					}
				}
			}
			if (buttonInfo.texture != null)
			{
				RawImage componentInChildren = binstance.GetComponentInChildren<RawImage>();
				if (componentInChildren != null)
				{
					componentInChildren.gameObject.SetActive(value: true);
					componentInChildren.texture = buttonInfo.texture;
				}
			}
			ToolTip componentInChildren2 = binstance.GetComponentInChildren<ToolTip>();
			if (buttonInfo.text != null && buttonInfo.text != "" && componentInChildren2 != null)
			{
				componentInChildren2.toolTip = buttonInfo.GetTooltipText();
				LocText componentInChildren3 = binstance.GetComponentInChildren<LocText>();
				if (componentInChildren3 != null)
				{
					componentInChildren3.text = buttonInfo.text;
				}
			}
			if (buttonInfo.onToolTip != null)
			{
				componentInChildren2.OnToolTip = buttonInfo.onToolTip;
			}
			KIconButtonMenu screen = this;
			System.Action onClick = buttonInfo.onClick;
			System.Action value = delegate
			{
				onClick.Signal();
				if (!keepMenuOpen && screen != null)
				{
					screen.Deactivate();
				}
				if (binstance != null)
				{
					KToggle component3 = binstance.GetComponent<KToggle>();
					if (component3 != null)
					{
						SelectToggle(component3);
					}
				}
			};
			KToggle componentInChildren4 = binstance.GetComponentInChildren<KToggle>();
			if (componentInChildren4 != null)
			{
				ToggleGroup component2 = GetComponent<ToggleGroup>();
				if (component2 == null)
				{
					component2 = externalToggleGroup;
				}
				componentInChildren4.group = component2;
				componentInChildren4.onClick += value;
				Navigation navigation = componentInChildren4.navigation;
				navigation.mode = (automaticNavigation ? Navigation.Mode.Automatic : Navigation.Mode.None);
				componentInChildren4.navigation = navigation;
			}
			else
			{
				KBasicToggle componentInChildren5 = binstance.GetComponentInChildren<KBasicToggle>();
				if (componentInChildren5 != null)
				{
					componentInChildren5.onClick += value;
				}
			}
			if (component != null)
			{
				component.isInteractable = buttonInfo.isInteractable;
			}
			buttonInfo.onCreate.Signal(buttonInfo);
		}
		Update();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (buttons == null || !base.gameObject.activeSelf || !base.enabled)
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

	protected void SelectToggle(KToggle selectedToggle)
	{
		if (UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			return;
		}
		if (currentlySelectedToggle == selectedToggle)
		{
			currentlySelectedToggle = null;
		}
		else
		{
			currentlySelectedToggle = selectedToggle;
		}
		GameObject[] array = buttonObjects;
		foreach (GameObject gameObject in array)
		{
			KToggle component = gameObject.GetComponent<KToggle>();
			if (component != null)
			{
				if (component == currentlySelectedToggle)
				{
					component.Select();
					component.isOn = true;
				}
				else
				{
					component.Deselect();
					component.isOn = false;
				}
			}
		}
	}

	public void ClearSelection()
	{
		GameObject[] array = buttonObjects;
		foreach (GameObject gameObject in array)
		{
			KToggle component = gameObject.GetComponent<KToggle>();
			if (component != null)
			{
				component.Deselect();
				component.isOn = false;
			}
			else
			{
				KBasicToggle component2 = gameObject.GetComponent<KBasicToggle>();
				if (component2 != null)
				{
					component2.isOn = false;
				}
			}
			ImageToggleState component3 = gameObject.GetComponent<ImageToggleState>();
			if (component3.GetIsActive())
			{
				component3.SetInactive();
			}
		}
		ToggleGroup component4 = GetComponent<ToggleGroup>();
		if (component4 != null)
		{
			component4.SetAllTogglesOff();
		}
		SelectToggle(null);
	}
}
