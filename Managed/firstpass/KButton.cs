using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/Plugins/KButton")]
public class KButton : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField]
	public ButtonSoundPlayer soundPlayer;

	public KImage bgImage;

	public Image fgImage;

	public KImage[] additionalKImages;

	private bool interactable = true;

	private bool mouseOver = false;

	public bool isInteractable
	{
		get
		{
			return interactable;
		}
		set
		{
			interactable = value;
			UpdateColor(interactable, mouseOver, press: false);
		}
	}

	public bool GetMouseOver => mouseOver;

	public event System.Action onClick;

	public event System.Action onDoubleClick;

	public event Action<KKeyCode> onBtnClick;

	public event System.Action onPointerEnter;

	public event System.Action onPointerExit;

	public event System.Action onPointerDown;

	public event System.Action onPointerUp;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UpdateColor(interactable, hover: false, press: false);
	}

	public void ClearOnClick()
	{
		this.onClick = null;
		this.onBtnClick = null;
		this.onDoubleClick = null;
	}

	public void ClearOnPointerEvents()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
		this.onPointerDown = null;
		this.onPointerUp = null;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			UpdateColor(interactable, hover: false, press: false);
			this.onPointerUp.Signal();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (KInputManager.isFocused)
		{
			KInputManager.SetUserActive();
			UpdateColor(interactable, hover: true, press: true);
			PlayPointerDownSound();
			this.onPointerDown.Signal();
		}
	}

	public void SignalClick(KKeyCode btn)
	{
		if (interactable)
		{
			if (this.onClick != null)
			{
				this.onClick();
			}
			if (this.onBtnClick != null)
			{
				this.onBtnClick(btn);
			}
		}
	}

	public void SignalDoubleClick(KKeyCode btn)
	{
		if (interactable && this.onDoubleClick != null)
		{
			this.onDoubleClick();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (interactable)
		{
			KKeyCode btn = KKeyCode.None;
			switch (eventData.button)
			{
			case PointerEventData.InputButton.Left:
				btn = KKeyCode.Mouse0;
				break;
			case PointerEventData.InputButton.Right:
				btn = KKeyCode.Mouse1;
				break;
			case PointerEventData.InputButton.Middle:
				btn = KKeyCode.Mouse2;
				break;
			}
			if ((eventData.clickCount == 1 || this.onDoubleClick == null) && (this.onClick != null || this.onBtnClick != null))
			{
				SignalClick(btn);
			}
			else if (eventData.clickCount == 2 && this.onDoubleClick != null)
			{
				SignalDoubleClick(btn);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
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
			foreach (ImageToggleState imageToggleState in array)
			{
				imageToggleState.OnHoverIn();
			}
		}
		UpdateColor(interactable, hover: true, press: false);
		soundPlayer.Play(1);
		mouseOver = true;
		this.onPointerEnter.Signal();
	}

	public void OnPointerExit(PointerEventData eventData)
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
			foreach (ImageToggleState imageToggleState in array)
			{
				imageToggleState.OnHoverOut();
			}
		}
		UpdateColor(interactable, hover: false, press: false);
		mouseOver = false;
		this.onPointerExit.Signal();
	}

	private void UpdateColor(bool interactable, bool hover, bool press)
	{
		if (bgImage == null)
		{
			bgImage = GetComponent<KImage>();
			string arg = "";
			Transform transform = base.transform;
			for (int i = 0; i < 5; i++)
			{
				if (!(transform.parent != null))
				{
					break;
				}
				transform = transform.parent;
				string name = transform.name;
				arg = $"{name}/{arg}";
			}
			if (bgImage == null)
			{
				return;
			}
		}
		UpdateKImageColor(bgImage, interactable, hover, press);
		for (int j = 0; j < additionalKImages.Length; j++)
		{
			UpdateKImageColor(additionalKImages[j], interactable, hover, press);
		}
	}

	private void UpdateKImageColor(KImage image, bool interactable, bool hover, bool press)
	{
		if (!(image != null))
		{
			return;
		}
		if (interactable)
		{
			if (press)
			{
				image.ColorState = KImage.ColorSelector.Active;
			}
			else
			{
				image.ColorState = ((!hover) ? KImage.ColorSelector.Inactive : KImage.ColorSelector.Hover);
			}
		}
		else
		{
			image.ColorState = (hover ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Disabled);
		}
	}

	public void PlayPointerDownSound()
	{
		if (!interactable || (soundPlayer.AcceptClickCondition != null && !soundPlayer.AcceptClickCondition()))
		{
			soundPlayer.Play(2);
		}
		else
		{
			soundPlayer.Play(0);
		}
	}
}
