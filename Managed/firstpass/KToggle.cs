using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KToggle : Toggle
{
	public delegate void PointerEvent();

	[SerializeField]
	public ToggleSoundPlayer soundPlayer;

	public Image bgImage;

	public Image fgImage;

	public KToggleArtExtensions artExtension;

	protected bool mouseOver;

	public bool GetMouseOver => mouseOver;

	public new bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			base.isOn = value;
			OnValueChanged(base.isOn);
		}
	}

	public event System.Action onClick;

	public event System.Action onDoubleClick;

	public new event Action<bool> onValueChanged;

	public event PointerEvent onPointerEnter;

	public event PointerEvent onPointerExit;

	public void ClearOnClick()
	{
		this.onClick = null;
	}

	public void ClearPointerCallbacks()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
	}

	public void ClearAllCallbacks()
	{
		ClearOnClick();
		ClearPointerCallbacks();
		this.onDoubleClick = null;
	}

	public void Click()
	{
		if (KInputManager.isFocused && IsInteractable() && !(UnityEngine.EventSystems.EventSystem.current == null) && UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			if (isOn)
			{
				Deselect();
				isOn = false;
			}
			else
			{
				Select();
				isOn = true;
			}
			if (soundPlayer.AcceptClickCondition != null && !soundPlayer.AcceptClickCondition())
			{
				soundPlayer.Play(3);
			}
			else
			{
				soundPlayer.Play((!isOn) ? 1 : 0);
			}
			base.gameObject.Trigger(2098165161);
			this.onClick.Signal();
		}
	}

	private void OnValueChanged(bool value)
	{
		if (!IsInteractable())
		{
			return;
		}
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActiveState(value);
			}
		}
		ActivateFlourish(value);
		this.onValueChanged.Signal(value);
	}

	public void ForceUpdateVisualState()
	{
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetColor();
			}
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (KInputManager.isFocused && eventData.button != PointerEventData.InputButton.Right && IsInteractable())
		{
			if (eventData.clickCount == 1 || this.onDoubleClick == null)
			{
				Click();
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				this.onDoubleClick();
			}
		}
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		if (GetParentToggleGroup(eventData) == base.group)
		{
			base.OnDeselect(eventData);
		}
	}

	public void Deselect()
	{
		base.OnDeselect(null);
	}

	public void ClearAnimState()
	{
		if (artExtension.animator != null && artExtension.animator.isInitialized)
		{
			Animator obj = artExtension.animator;
			obj.SetBool("Toggled", value: false);
			obj.Play("idle", 0);
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if (base.group != null)
		{
			foreach (KToggle item in base.group.ActiveToggles())
			{
				item.Deselect();
			}
			base.group.SetAllTogglesOff();
		}
		base.OnSelect(eventData);
	}

	public void ActivateFlourish(bool state)
	{
		if (artExtension.animator != null && artExtension.animator.isInitialized)
		{
			artExtension.animator.SetBool("Toggled", state);
		}
		if (artExtension.SelectedFlourish != null)
		{
			artExtension.SelectedFlourish.enabled = state;
		}
	}

	public void ActivateFlourish(bool state, ImageToggleState.State ImageState)
	{
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetState(ImageState);
			}
		}
		ActivateFlourish(state);
	}

	private ToggleGroup GetParentToggleGroup(BaseEventData eventData)
	{
		if (!(eventData is PointerEventData pointerEventData))
		{
			return null;
		}
		GameObject gameObject = pointerEventData.pointerPressRaycast.gameObject;
		if (gameObject == null)
		{
			return null;
		}
		Toggle componentInParent = gameObject.GetComponentInParent<Toggle>();
		if (componentInParent == null || componentInParent.group == null)
		{
			return null;
		}
		return componentInParent.group;
	}

	public void OnPointerEnter()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnHoverIn();
			}
		}
		soundPlayer.Play(2);
		mouseOver = true;
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
	}

	public void OnPointerExit()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		ImageToggleState[] components = GetComponents<ImageToggleState>();
		if (components != null && components.Length != 0)
		{
			ImageToggleState[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnHoverOut();
			}
		}
		mouseOver = false;
		if (this.onPointerExit != null)
		{
			this.onPointerExit();
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			OnPointerEnter();
			base.OnPointerEnter(eventData);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			OnPointerExit();
			base.OnPointerExit(eventData);
		}
	}
}
