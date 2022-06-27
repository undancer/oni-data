using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BreakdownListRow")]
public class BreakdownListRow : KMonoBehaviour
{
	public enum Status
	{
		Default,
		Red,
		Green,
		Yellow
	}

	private static Color[] statusColour = new Color[4]
	{
		new Color(29f / 85f, 0.36862746f, 39f / 85f, 1f),
		new Color(0.72156864f, 0.38431373f, 0f, 1f),
		new Color(0.38431373f, 0.72156864f, 0f, 1f),
		new Color(0.72156864f, 0.72156864f, 0f, 1f)
	};

	public Image dotOutlineImage;

	public Image dotInsideImage;

	public Image iconImage;

	public Image checkmarkImage;

	public LocText nameLabel;

	public LocText valueLabel;

	private bool isHighlighted;

	private bool isDisabled;

	private bool isImportant;

	private ToolTip tooltip;

	[SerializeField]
	private Sprite statusSuccessIcon;

	[SerializeField]
	private Sprite statusWarningIcon;

	[SerializeField]
	private Sprite statusFailureIcon;

	public void ShowData(string name, string value)
	{
		base.gameObject.transform.localScale = Vector3.one;
		nameLabel.text = name;
		valueLabel.text = value;
		dotOutlineImage.gameObject.SetActive(value: true);
		Vector2 vector = Vector2.one * 0.6f;
		dotOutlineImage.rectTransform.localScale.Set(vector.x, vector.y, 1f);
		dotInsideImage.gameObject.SetActive(value: true);
		dotInsideImage.color = statusColour[0];
		iconImage.gameObject.SetActive(value: false);
		checkmarkImage.gameObject.SetActive(value: false);
		SetHighlighted(highlighted: false);
		SetImportant(important: false);
	}

	public void ShowStatusData(string name, string value, Status dotColor)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(value: true);
		dotInsideImage.gameObject.SetActive(value: true);
		iconImage.gameObject.SetActive(value: false);
		checkmarkImage.gameObject.SetActive(value: false);
		SetStatusColor(dotColor);
	}

	public void SetStatusColor(Status dotColor)
	{
		checkmarkImage.gameObject.SetActive(dotColor != Status.Default);
		checkmarkImage.color = statusColour[(int)dotColor];
		switch (dotColor)
		{
		case Status.Green:
			checkmarkImage.sprite = statusSuccessIcon;
			break;
		case Status.Yellow:
			checkmarkImage.sprite = statusWarningIcon;
			break;
		case Status.Red:
			checkmarkImage.sprite = statusFailureIcon;
			break;
		}
	}

	public void ShowCheckmarkData(string name, string value, Status status)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(value: true);
		dotOutlineImage.rectTransform.localScale = Vector3.one;
		dotInsideImage.gameObject.SetActive(value: true);
		iconImage.gameObject.SetActive(value: false);
		SetStatusColor(status);
	}

	public void ShowIconData(string name, string value, Sprite sprite)
	{
		ShowData(name, value);
		dotOutlineImage.gameObject.SetActive(value: false);
		dotInsideImage.gameObject.SetActive(value: false);
		iconImage.gameObject.SetActive(value: true);
		checkmarkImage.gameObject.SetActive(value: false);
		iconImage.sprite = sprite;
		iconImage.color = Color.white;
	}

	public void ShowIconData(string name, string value, Sprite sprite, Color spriteColor)
	{
		ShowIconData(name, value, sprite);
		iconImage.color = spriteColor;
	}

	public void SetHighlighted(bool highlighted)
	{
		isHighlighted = highlighted;
		Vector2 vector = Vector2.one * 0.8f;
		dotOutlineImage.rectTransform.localScale.Set(vector.x, vector.y, 1f);
		nameLabel.alpha = (isHighlighted ? 0.9f : 0.5f);
		valueLabel.alpha = (isHighlighted ? 0.9f : 0.5f);
	}

	public void SetDisabled(bool disabled)
	{
		isDisabled = disabled;
		nameLabel.alpha = (isDisabled ? 0.4f : 0.5f);
		valueLabel.alpha = (isDisabled ? 0.4f : 0.5f);
	}

	public void SetImportant(bool important)
	{
		isImportant = important;
		dotOutlineImage.rectTransform.localScale = Vector3.one;
		nameLabel.alpha = (isImportant ? 1f : 0.5f);
		valueLabel.alpha = (isImportant ? 1f : 0.5f);
		nameLabel.fontStyle = (isImportant ? FontStyles.Bold : FontStyles.Normal);
		valueLabel.fontStyle = (isImportant ? FontStyles.Bold : FontStyles.Normal);
	}

	public void HideIcon()
	{
		dotOutlineImage.gameObject.SetActive(value: false);
		dotInsideImage.gameObject.SetActive(value: false);
		iconImage.gameObject.SetActive(value: false);
		checkmarkImage.gameObject.SetActive(value: false);
	}

	public void AddTooltip(string tooltipText)
	{
		if (tooltip == null)
		{
			tooltip = base.gameObject.AddComponent<ToolTip>();
		}
		tooltip.SetSimpleTooltip(tooltipText);
	}

	public void ClearTooltip()
	{
		if (tooltip != null)
		{
			tooltip.ClearMultiStringTooltip();
		}
	}

	public void SetValue(string value)
	{
		valueLabel.text = value;
	}
}
