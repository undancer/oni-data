using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/MultiToggle")]
public class MultiToggle : KMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[Header("Settings")]
	[SerializeField]
	public ToggleState[] states;

	public bool play_sound_on_click = true;

	public bool play_sound_on_release;

	public Image toggle_image;

	protected int state;

	public System.Action onClick;

	public Func<bool> onDoubleClick;

	public System.Action onEnter;

	public System.Action onExit;

	public System.Action onHold;

	public System.Action onStopHold;

	public bool allowRightClick = true;

	protected bool clickHeldDown;

	protected float totalHeldTime;

	protected float heldTimeThreshold = 0.4f;

	private bool pointerOver;

	public int CurrentState => state;

	public void NextState()
	{
		ChangeState((state + 1) % states.Length);
	}

	protected virtual void Update()
	{
		if (clickHeldDown)
		{
			totalHeldTime += Time.unscaledDeltaTime;
			if (totalHeldTime > heldTimeThreshold && onHold != null)
			{
				onHold();
			}
		}
	}

	private void OnDisable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			RefreshHoverColor();
			pointerOver = false;
		}
	}

	public void ChangeState(int new_state_index)
	{
		state = new_state_index;
		try
		{
			toggle_image.sprite = states[new_state_index].sprite;
			toggle_image.color = states[new_state_index].color;
			if (states[new_state_index].use_rect_margins)
			{
				toggle_image.rectTransform().sizeDelta = states[new_state_index].rect_margins;
			}
		}
		catch
		{
			string text = base.gameObject.name;
			Transform transform = base.transform;
			while (transform.parent != null)
			{
				text = text.Insert(0, transform.name + ">");
				transform = transform.parent;
			}
			Debug.LogError("Multi Toggle state index out of range: " + text + " idx:" + new_state_index, base.gameObject);
		}
		StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
		for (int i = 0; i < additional_display_settings.Length; i++)
		{
			StatePresentationSetting statePresentationSetting = additional_display_settings[i];
			if (!(statePresentationSetting.image_target == null))
			{
				statePresentationSetting.image_target.sprite = statePresentationSetting.sprite;
				statePresentationSetting.image_target.color = statePresentationSetting.color;
			}
		}
		RefreshHoverColor();
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (allowRightClick || eventData.button != PointerEventData.InputButton.Right)
		{
			if (states.Length - 1 < state)
			{
				Debug.LogWarning("Multi toggle has too few / no states");
			}
			bool flag = false;
			if (onDoubleClick != null && eventData.clickCount == 2)
			{
				flag = onDoubleClick();
			}
			if (onClick != null && !flag)
			{
				onClick();
			}
			RefreshHoverColor();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerOver = true;
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (states.Length == 0)
		{
			return;
		}
		if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
		{
			toggle_image.color = states[state].color_on_hover;
		}
		if (states[state].use_rect_margins)
		{
			toggle_image.rectTransform().sizeDelta = states[state].rect_margins;
		}
		StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
		for (int i = 0; i < additional_display_settings.Length; i++)
		{
			StatePresentationSetting statePresentationSetting = additional_display_settings[i];
			if (!(statePresentationSetting.image_target == null) && statePresentationSetting.use_color_on_hover)
			{
				statePresentationSetting.image_target.color = statePresentationSetting.color_on_hover;
			}
		}
		if (onEnter != null)
		{
			onEnter();
		}
	}

	protected void RefreshHoverColor()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			if (states.Length == 0)
			{
				return;
			}
			if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
			{
				toggle_image.color = states[state].color;
			}
			StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
			for (int i = 0; i < additional_display_settings.Length; i++)
			{
				StatePresentationSetting statePresentationSetting = additional_display_settings[i];
				if (!(statePresentationSetting.image_target == null) && statePresentationSetting.use_color_on_hover)
				{
					statePresentationSetting.image_target.color = statePresentationSetting.color;
				}
			}
		}
		else
		{
			if (!pointerOver)
			{
				return;
			}
			if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
			{
				toggle_image.color = states[state].color_on_hover;
			}
			StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
			for (int i = 0; i < additional_display_settings.Length; i++)
			{
				StatePresentationSetting statePresentationSetting2 = additional_display_settings[i];
				if (!(statePresentationSetting2.image_target == null) && !(statePresentationSetting2.image_target == null) && statePresentationSetting2.use_color_on_hover)
				{
					statePresentationSetting2.image_target.color = statePresentationSetting2.color_on_hover;
				}
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerOver = false;
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.SetUserActive();
		if (states.Length == 0)
		{
			return;
		}
		if (states[state].use_color_on_hover && states[state].color_on_hover != states[state].color)
		{
			toggle_image.color = states[state].color;
		}
		if (states[state].use_rect_margins)
		{
			toggle_image.rectTransform().sizeDelta = states[state].rect_margins;
		}
		StatePresentationSetting[] additional_display_settings = states[state].additional_display_settings;
		for (int i = 0; i < additional_display_settings.Length; i++)
		{
			StatePresentationSetting statePresentationSetting = additional_display_settings[i];
			if (!(statePresentationSetting.image_target == null) && statePresentationSetting.use_color_on_hover)
			{
				statePresentationSetting.image_target.color = statePresentationSetting.color;
			}
		}
		if (onExit != null)
		{
			onExit();
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (!allowRightClick && eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		clickHeldDown = true;
		if (play_sound_on_click)
		{
			if (states[state].on_click_override_sound_path == "")
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
			}
			else
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound(states[state].on_click_override_sound_path));
			}
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (!allowRightClick && eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		if (clickHeldDown)
		{
			if (play_sound_on_release && states[state].on_release_override_sound_path != "")
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound(states[state].on_release_override_sound_path));
			}
			clickHeldDown = false;
			if (onStopHold != null)
			{
				onStopHold();
			}
		}
		totalHeldTime = 0f;
	}
}
