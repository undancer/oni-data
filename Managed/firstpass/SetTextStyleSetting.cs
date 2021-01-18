using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("KMonoBehaviour/Plugins/SetTextStyleSetting")]
public class SetTextStyleSetting : KMonoBehaviour
{
	public enum TextStyle
	{
		Standard,
		Header
	}

	[MyCmpGet]
	private Text text;

	[MyCmpGet]
	private TextMeshProUGUI sdfText;

	[SerializeField]
	private TextStyleSetting style;

	private TextStyleSetting currentStyle;

	public static void ApplyStyle(TextMeshProUGUI sdfText, TextStyleSetting style)
	{
		if ((bool)sdfText && (bool)style)
		{
			sdfText.enableWordWrapping = style.enableWordWrapping;
			sdfText.enableKerning = true;
			sdfText.extraPadding = true;
			sdfText.fontSize = style.fontSize;
			sdfText.color = style.textColor;
			sdfText.font = style.sdfFont;
			sdfText.fontStyle = style.style;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void SetStyle(TextStyleSetting newstyle)
	{
		if (sdfText == null)
		{
			sdfText = GetComponent<TextMeshProUGUI>();
		}
		if (currentStyle != newstyle)
		{
			currentStyle = newstyle;
			style = currentStyle;
			ApplyStyle(sdfText, style);
		}
	}
}
