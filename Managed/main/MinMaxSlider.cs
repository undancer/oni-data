using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/MinMaxSlider")]
public class MinMaxSlider : KMonoBehaviour
{
	public enum LockingType
	{
		Toggle,
		Drag
	}

	public enum Mode
	{
		Single,
		Double,
		Triple
	}

	public LockingType lockType = LockingType.Drag;

	public bool lockRange = false;

	public bool interactable = true;

	public float minLimit = 0f;

	public float maxLimit = 100f;

	public float range = 50f;

	public float barWidth = 10f;

	public float barHeight = 100f;

	public float currentMinValue = 10f;

	public float currentMaxValue = 90f;

	public float currentExtraValue = 50f;

	public Slider.Direction direction = Slider.Direction.LeftToRight;

	public bool wholeNumbers = true;

	public Action<MinMaxSlider> onMinChange;

	public Action<MinMaxSlider> onMaxChange;

	public Slider minSlider;

	public Slider maxSlider;

	public Slider extraSlider;

	public RectTransform minRect;

	public RectTransform maxRect;

	public RectTransform bgFill;

	public RectTransform mgFill;

	public RectTransform fgFill;

	public Text title;

	[MyCmpGet]
	public ToolTip toolTip;

	public Image icon;

	public Image isOverPowered;

	private Vector3 mousePos;

	public Mode mode
	{
		get;
		private set;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ToolTip component = base.transform.parent.gameObject.GetComponent<ToolTip>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(toolTip);
			toolTip = component;
		}
		minSlider.value = currentMinValue;
		maxSlider.value = currentMaxValue;
		minSlider.interactable = interactable;
		maxSlider.interactable = interactable;
		minSlider.maxValue = maxLimit;
		maxSlider.maxValue = maxLimit;
		minSlider.minValue = minLimit;
		maxSlider.minValue = minLimit;
		Slider.Direction direction3 = (minSlider.direction = (maxSlider.direction = this.direction));
		if (isOverPowered != null)
		{
			isOverPowered.enabled = false;
		}
		minSlider.gameObject.SetActive(value: false);
		if (mode != 0)
		{
			minSlider.gameObject.SetActive(value: true);
		}
		if (extraSlider != null)
		{
			extraSlider.value = currentExtraValue;
			Slider slider = extraSlider;
			Slider slider2 = minSlider;
			bool flag2 = (maxSlider.wholeNumbers = wholeNumbers);
			bool flag4 = (slider2.wholeNumbers = flag2);
			slider.wholeNumbers = flag4;
			extraSlider.direction = this.direction;
			extraSlider.interactable = interactable;
			extraSlider.maxValue = maxLimit;
			extraSlider.minValue = minLimit;
			extraSlider.gameObject.SetActive(value: false);
			if (mode == Mode.Triple)
			{
				extraSlider.gameObject.SetActive(value: true);
			}
		}
	}

	public void SetIcon(Image newIcon)
	{
		icon = newIcon;
		icon.gameObject.transform.SetParent(base.transform);
		icon.gameObject.transform.SetAsFirstSibling();
		icon.rectTransform().anchoredPosition = Vector2.zero;
	}

	public void SetMode(Mode mode)
	{
		this.mode = mode;
		if (mode == Mode.Single && extraSlider != null)
		{
			extraSlider.gameObject.SetActive(value: false);
			extraSlider.handleRect.gameObject.SetActive(value: false);
		}
	}

	private void SetAnchor(RectTransform trans, Vector2 min, Vector2 max)
	{
		trans.anchorMin = min;
		trans.anchorMax = max;
	}

	public void SetMinMaxValue(float currentMin, float currentMax, float min, float max)
	{
		float num2 = (currentMinValue = (minSlider.value = currentMin));
		num2 = (currentMaxValue = (maxSlider.value = currentMax));
		minLimit = min;
		maxLimit = max;
		minSlider.minValue = minLimit;
		maxSlider.minValue = minLimit;
		minSlider.maxValue = maxLimit;
		maxSlider.maxValue = maxLimit;
		if (extraSlider != null)
		{
			extraSlider.minValue = minLimit;
			extraSlider.maxValue = maxLimit;
		}
	}

	public void SetExtraValue(float current)
	{
		extraSlider.value = current;
		toolTip.toolTip = base.transform.parent.name + ": " + current.ToString("F2");
	}

	public void SetMaxValue(float current, float max)
	{
		float num = current / max * 100f;
		if (isOverPowered != null)
		{
			isOverPowered.enabled = num > 100f;
		}
		maxSlider.value = Mathf.Min(100f, num);
		if (toolTip != null)
		{
			toolTip.toolTip = base.transform.parent.name + ": " + current.ToString("F2") + "/" + max.ToString("F2");
		}
	}

	private void Update()
	{
		if (interactable)
		{
			minSlider.value = Mathf.Clamp(currentMinValue, minLimit, currentMinValue);
			maxSlider.value = Mathf.Max(minSlider.value, Mathf.Clamp(currentMaxValue, Mathf.Max(minSlider.value, minLimit), maxLimit));
			if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
			{
				minRect.anchorMax = new Vector2(minSlider.value / maxLimit, minRect.anchorMax.y);
				maxRect.anchorMax = new Vector2(maxSlider.value / maxLimit, maxRect.anchorMax.y);
				maxRect.anchorMin = new Vector2(minSlider.value / maxLimit, maxRect.anchorMin.y);
			}
			else
			{
				minRect.anchorMax = new Vector2(minRect.anchorMin.x, minSlider.value / maxLimit);
				maxRect.anchorMin = new Vector2(maxRect.anchorMin.x, minSlider.value / maxLimit);
			}
		}
	}

	public void OnMinValueChanged(float ignoreThis)
	{
		if (interactable)
		{
			if (lockRange)
			{
				currentMaxValue = Mathf.Min(Mathf.Max(minLimit, minSlider.value) + range, maxLimit);
				currentMinValue = Mathf.Max(minLimit, Mathf.Min(maxSlider.value, currentMaxValue - range));
			}
			else
			{
				currentMinValue = Mathf.Clamp(minSlider.value, minLimit, Mathf.Min(maxSlider.value, currentMaxValue));
			}
			if (onMinChange != null)
			{
				onMinChange(this);
			}
		}
	}

	public void OnMaxValueChanged(float ignoreThis)
	{
		if (interactable)
		{
			if (lockRange)
			{
				currentMinValue = Mathf.Max(maxSlider.value - range, minLimit);
				currentMaxValue = Mathf.Max(minSlider.value, Mathf.Clamp(maxSlider.value, Mathf.Max(currentMinValue + range, minLimit), maxLimit));
			}
			else
			{
				currentMaxValue = Mathf.Max(minSlider.value, Mathf.Clamp(maxSlider.value, Mathf.Max(minSlider.value, minLimit), maxLimit));
			}
			if (onMaxChange != null)
			{
				onMaxChange(this);
			}
		}
	}

	public void Lock(bool shouldLock)
	{
		if (interactable && lockType == LockingType.Drag)
		{
			lockRange = shouldLock;
			range = maxSlider.value - minSlider.value;
			mousePos = KInputManager.GetMousePos();
		}
	}

	public void ToggleLock()
	{
		if (interactable && lockType == LockingType.Toggle)
		{
			lockRange = !lockRange;
			if (lockRange)
			{
				range = maxSlider.value - minSlider.value;
			}
		}
	}

	public void OnDrag()
	{
		if (interactable && lockRange && lockType == LockingType.Drag)
		{
			float num = KInputManager.GetMousePos().x - mousePos.x;
			if (direction == Slider.Direction.TopToBottom || direction == Slider.Direction.BottomToTop)
			{
				num = KInputManager.GetMousePos().y - mousePos.y;
			}
			currentMinValue = Mathf.Max(currentMinValue + num, minLimit);
			mousePos = KInputManager.GetMousePos();
		}
	}
}
