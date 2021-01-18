using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KScrollRect : ScrollRect
{
	public enum SoundType
	{
		OnMouseScroll
	}

	public static Dictionary<SoundType, string> DefaultSounds = new Dictionary<SoundType, string>();

	private Dictionary<SoundType, string> currentSounds = new Dictionary<SoundType, string>();

	private float scrollVelocity = 0f;

	private bool default_intertia = true;

	private float default_elasticity = 0.2f;

	private float default_decelerationRate = 0.02f;

	private float verticalScrollInertiaScale = 10f;

	private float horizontalScrollInertiaScale = 5f;

	private float scrollDeceleration = 0.25f;

	[SerializeField]
	public bool forceContentMatchWidth = false;

	[SerializeField]
	public bool forceContentMatchHeight = false;

	[SerializeField]
	public bool allowHorizontalScrollWheel = true;

	[SerializeField]
	public bool allowVerticalScrollWheel = true;

	[SerializeField]
	public bool allowRightMouseScroll;

	private bool panUp = false;

	private bool panDown = false;

	private bool panRight = false;

	private bool panLeft = false;

	private Vector3 keyboardScrollDelta = default(Vector3);

	private float keyboardScrollSpeed = 1f;

	private bool startDrag = false;

	private bool stopDrag = false;

	private bool autoScrolling = false;

	private float autoScrollTargetVerticalPos;

	public bool isDragging
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		base.Awake();
		base.elasticity = default_elasticity;
		base.inertia = default_intertia;
		base.decelerationRate = default_decelerationRate;
		base.scrollSensitivity = 1f;
		foreach (KeyValuePair<SoundType, string> defaultSound in DefaultSounds)
		{
			currentSounds[defaultSound.Key] = defaultSound.Value;
		}
	}

	public override void OnScroll(PointerEventData data)
	{
		if (base.vertical && allowVerticalScrollWheel)
		{
			scrollVelocity += data.scrollDelta.y * verticalScrollInertiaScale;
		}
		else if (base.horizontal && allowHorizontalScrollWheel)
		{
			scrollVelocity -= data.scrollDelta.y * horizontalScrollInertiaScale;
		}
		if (Mathf.Abs(data.scrollDelta.y) > 0.2f)
		{
			EventInstance instance = KFMOD.BeginOneShot(currentSounds[SoundType.OnMouseScroll], Vector3.zero);
			float boundsExceedAmount = GetBoundsExceedAmount();
			instance.setParameterByName("scrollbarPosition", boundsExceedAmount);
			KFMOD.EndOneShot(instance);
		}
	}

	private float GetBoundsExceedAmount()
	{
		if (base.vertical && base.verticalScrollbar != null)
		{
			RectTransform rectTransform = ((base.viewport == null) ? base.gameObject.GetComponent<RectTransform>() : base.viewport.rectTransform());
			float num = Mathf.Min(rectTransform.rect.size.y, base.content.sizeDelta.y) / base.content.sizeDelta.y;
			float num2 = Mathf.Abs(base.verticalScrollbar.size - num);
			if (Mathf.Abs(num2) < 0.001f)
			{
				num2 = 0f;
			}
			return num2;
		}
		if (base.horizontal && base.horizontalScrollbar != null)
		{
			RectTransform rectTransform2 = ((base.viewport == null) ? base.gameObject.GetComponent<RectTransform>() : base.viewport.rectTransform());
			float num3 = Mathf.Min(rectTransform2.rect.size.x, base.content.sizeDelta.x) / base.content.sizeDelta.x;
			float num4 = Mathf.Abs(base.horizontalScrollbar.size - num3);
			if (Mathf.Abs(num4) < 0.001f)
			{
				num4 = 0f;
			}
			return num4;
		}
		return 0f;
	}

	public void SetSmoothAutoScrollTarget(float normalizedVerticalPos)
	{
		autoScrollTargetVerticalPos = normalizedVerticalPos;
		autoScrolling = true;
	}

	private void PlaySound(SoundType soundType)
	{
		if (currentSounds.ContainsKey(soundType))
		{
			KFMOD.PlayUISound(currentSounds[soundType]);
		}
	}

	public void SetSound(SoundType soundType, string soundPath)
	{
		currentSounds[soundType] = soundPath;
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		startDrag = true;
		base.OnBeginDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		stopDrag = true;
		base.OnEndDrag(eventData);
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (allowRightMouseScroll && (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Middle))
		{
			base.content.localPosition = base.content.localPosition + new Vector3(eventData.delta.x, eventData.delta.y);
			base.normalizedPosition = new Vector2(Mathf.Clamp(base.normalizedPosition.x, 0f, 1f), Mathf.Clamp(base.normalizedPosition.y, 0f, 1f));
		}
		base.OnDrag(eventData);
		scrollVelocity = 0f;
	}

	protected override void LateUpdate()
	{
		UpdateScrollIntertia();
		if (allowRightMouseScroll)
		{
			if (panUp)
			{
				keyboardScrollDelta.y -= keyboardScrollSpeed;
			}
			if (panDown)
			{
				keyboardScrollDelta.y += keyboardScrollSpeed;
			}
			if (panLeft)
			{
				keyboardScrollDelta.x += keyboardScrollSpeed;
			}
			if (panRight)
			{
				keyboardScrollDelta.x -= keyboardScrollSpeed;
			}
			if (panUp || panDown || panLeft || panRight)
			{
				base.content.localPosition = base.content.localPosition + keyboardScrollDelta;
				base.normalizedPosition = new Vector2(Mathf.Clamp(base.normalizedPosition.x, 0f, 1f), Mathf.Clamp(base.normalizedPosition.y, 0f, 1f));
			}
		}
		if (startDrag)
		{
			startDrag = false;
			isDragging = true;
		}
		else if (stopDrag)
		{
			stopDrag = false;
			isDragging = false;
		}
		if (autoScrolling)
		{
			base.normalizedPosition = new Vector2(base.normalizedPosition.x, Mathf.Lerp(base.normalizedPosition.y, autoScrollTargetVerticalPos, Time.unscaledDeltaTime * 3f));
			if (Mathf.Abs(autoScrollTargetVerticalPos - base.normalizedPosition.y) < 0.01f)
			{
				autoScrolling = false;
			}
		}
		base.LateUpdate();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (forceContentMatchWidth)
		{
			Vector2 sizeDelta = base.content.GetComponent<RectTransform>().sizeDelta;
			sizeDelta.x = base.viewport.rectTransform().sizeDelta.x;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		}
		if (forceContentMatchHeight)
		{
			Vector2 sizeDelta2 = base.content.GetComponent<RectTransform>().sizeDelta;
			sizeDelta2.y = base.viewport.rectTransform().sizeDelta.y;
			base.content.GetComponent<RectTransform>().sizeDelta = sizeDelta2;
		}
	}

	private void UpdateScrollIntertia()
	{
		scrollVelocity *= 1f - Mathf.Clamp(scrollDeceleration, 0f, 1f);
		if (Mathf.Abs(scrollVelocity) < 0.001f)
		{
			scrollVelocity = 0f;
		}
		else
		{
			Vector2 anchoredPosition = base.content.anchoredPosition;
			if (base.vertical && allowVerticalScrollWheel)
			{
				anchoredPosition.y -= scrollVelocity;
			}
			if (base.horizontal && allowHorizontalScrollWheel)
			{
				anchoredPosition.x -= scrollVelocity;
			}
			if (base.content.anchoredPosition != anchoredPosition)
			{
				base.content.anchoredPosition = anchoredPosition;
			}
		}
		if (base.vertical && allowVerticalScrollWheel && (base.verticalNormalizedPosition < -0.05f || base.verticalNormalizedPosition > 1.05f))
		{
			scrollVelocity *= 0.9f;
		}
		if (base.horizontal && allowHorizontalScrollWheel && (base.horizontalNormalizedPosition < -0.05f || base.horizontalNormalizedPosition > 1.05f))
		{
			scrollVelocity *= 0.9f;
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (allowRightMouseScroll)
		{
			if (e.TryConsume(Action.PanLeft))
			{
				panLeft = true;
			}
			else if (e.TryConsume(Action.PanRight))
			{
				panRight = true;
			}
			else if (e.TryConsume(Action.PanUp))
			{
				panUp = true;
			}
			else if (e.TryConsume(Action.PanDown))
			{
				panDown = true;
			}
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (allowRightMouseScroll)
		{
			if (panUp && e.TryConsume(Action.PanUp))
			{
				panUp = false;
				keyboardScrollDelta.y = 0f;
			}
			else if (panDown && e.TryConsume(Action.PanDown))
			{
				panDown = false;
				keyboardScrollDelta.y = 0f;
			}
			else if (panRight && e.TryConsume(Action.PanRight))
			{
				panRight = false;
				keyboardScrollDelta.x = 0f;
			}
			else if (panLeft && e.TryConsume(Action.PanLeft))
			{
				panLeft = false;
				keyboardScrollDelta.x = 0f;
			}
		}
	}
}
