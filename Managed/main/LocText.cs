using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using STRINGS;
using TMPro;
using UnityEngine;

public class LocText : TextMeshProUGUI
{
	public string key;

	public TextStyleSetting textStyleSetting;

	public bool allowOverride;

	public bool staticLayout;

	private TextLinkHandler textLinkHandler;

	private string originalString = string.Empty;

	private static UI.ClickType clickCache = UI.ClickType.click;

	[SerializeField]
	private bool allowLinksInternal;

	private static readonly Dictionary<string, Action> ActionLookup = Enum.GetNames(typeof(Action)).ToDictionary((string x) => x, (string x) => (Action)Enum.Parse(typeof(Action), x), StringComparer.OrdinalIgnoreCase);

	private const string linkPrefix_open = "<link=\"";

	private const string linkSuffix = "</link>";

	private const string linkColorPrefix = "<b><style=\"KLink\">";

	private const string linkColorSuffix = "</style></b>";

	private static readonly string combinedPrefix = "<b><style=\"KLink\"><link=\"";

	private static readonly string combinedSuffix = "</style></b></link>";

	public bool AllowLinks
	{
		get
		{
			return allowLinksInternal;
		}
		set
		{
			allowLinksInternal = value;
			RefreshLinkHandler();
			raycastTarget = raycastTarget || allowLinksInternal;
		}
	}

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = FilterInput(value);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	[ContextMenu("Apply Settings")]
	public void ApplySettings()
	{
		if (this.key != "" && Application.isPlaying)
		{
			StringKey key = new StringKey(this.key);
			text = Strings.Get(key);
		}
		if (textStyleSetting != null)
		{
			SetTextStyleSetting.ApplyStyle(this, textStyleSetting);
		}
	}

	private new void Awake()
	{
		base.Awake();
		if (Application.isPlaying)
		{
			if (key != "")
			{
				StringEntry stringEntry = Strings.Get(new StringKey(key));
				text = stringEntry.String;
			}
			text = Localization.Fixup(text);
			base.isRightToLeftText = Localization.IsRightToLeft;
			KInputManager.InputChange.AddListener(RefreshText);
			SetTextStyleSetting setTextStyleSetting = base.gameObject.GetComponent<SetTextStyleSetting>();
			if (setTextStyleSetting == null)
			{
				setTextStyleSetting = base.gameObject.AddComponent<SetTextStyleSetting>();
			}
			if (!allowOverride)
			{
				setTextStyleSetting.SetStyle(textStyleSetting);
			}
			textLinkHandler = GetComponent<TextLinkHandler>();
		}
	}

	private new void Start()
	{
		base.Start();
		RefreshLinkHandler();
	}

	private new void OnDestroy()
	{
		KInputManager.InputChange.RemoveListener(RefreshText);
		base.OnDestroy();
	}

	public override void SetLayoutDirty()
	{
		if (!staticLayout)
		{
			base.SetLayoutDirty();
		}
	}

	public override void SetText(string text)
	{
		text = FilterInput(text);
		base.SetText(text);
	}

	private string FilterInput(string input)
	{
		if (input != null)
		{
			string obj = ParseText(input);
			if (obj != input)
			{
				originalString = input;
			}
			else
			{
				originalString = string.Empty;
			}
			input = obj;
		}
		if (AllowLinks)
		{
			return ModifyLinkStrings(input);
		}
		return input;
	}

	public static string ParseText(string input)
	{
		string pattern = "\\{Hotkey/(\\w+)\\}";
		string text = Regex.Replace(input, pattern, delegate(Match m)
		{
			string value = m.Groups[1].Value;
			Action value2;
			return ActionLookup.TryGetValue(value, out value2) ? GameUtil.GetHotkeyString(value2) : m.Value;
		});
		if (text.Contains('\\'))
		{
			string[] array = text.Split('\\');
			string text2 = string.Empty;
			if (array.Length >= 3)
			{
				for (int i = 0; i < array.Length; i++)
				{
					text2 = ((i % 2 != 0) ? ((!KInputManager.currentControllerIsGamepad) ? ((!Enum.TryParse<UI.ClickType>(array[i], out clickCache)) ? (text2 + array[i]) : (clickCache switch
					{
						UI.ClickType.Click => text2 + UI.CONTROLS.CLICK, 
						UI.ClickType.Clickable => text2 + UI.CONTROLS.CLICKABLE, 
						UI.ClickType.Clicked => text2 + UI.CONTROLS.CLICKED, 
						UI.ClickType.Clicking => text2 + UI.CONTROLS.CLICKING, 
						UI.ClickType.Clicks => text2 + UI.CONTROLS.CLICKS, 
						UI.ClickType.click => text2 + UI.CONTROLS.CLICKLOWER, 
						UI.ClickType.clickable => text2 + UI.CONTROLS.CLICKABLELOWER, 
						UI.ClickType.clicked => text2 + UI.CONTROLS.CLICKEDLOWER, 
						UI.ClickType.clicking => text2 + UI.CONTROLS.CLICKINGLOWER, 
						UI.ClickType.clicks => text2 + UI.CONTROLS.CLICKSLOWER, 
						UI.ClickType.CLICK => text2 + UI.CONTROLS.CLICKUPPER, 
						UI.ClickType.CLICKABLE => text2 + UI.CONTROLS.CLICKABLEUPPER, 
						UI.ClickType.CLICKED => text2 + UI.CONTROLS.CLICKEDUPPER, 
						UI.ClickType.CLICKING => text2 + UI.CONTROLS.CLICKINGUPPER, 
						UI.ClickType.CLICKS => text2 + UI.CONTROLS.CLICKSUPPER, 
						_ => text2 + array[i], 
					})) : ((!Enum.TryParse<UI.ClickType>(array[i], out clickCache)) ? (text2 + array[i]) : (clickCache switch
					{
						UI.ClickType.Click => text2 + UI.CONTROLS.PRESS, 
						UI.ClickType.Clickable => text2 + UI.CONTROLS.PRESSABLE, 
						UI.ClickType.Clicked => text2 + UI.CONTROLS.PRESSED, 
						UI.ClickType.Clicking => text2 + UI.CONTROLS.PRESSING, 
						UI.ClickType.Clicks => text2 + UI.CONTROLS.PRESSES, 
						UI.ClickType.click => text2 + UI.CONTROLS.PRESSLOWER, 
						UI.ClickType.clickable => text2 + UI.CONTROLS.PRESSABLELOWER, 
						UI.ClickType.clicked => text2 + UI.CONTROLS.PRESSEDLOWER, 
						UI.ClickType.clicking => text2 + UI.CONTROLS.PRESSINGLOWER, 
						UI.ClickType.clicks => text2 + UI.CONTROLS.PRESSESLOWER, 
						UI.ClickType.CLICK => text2 + UI.CONTROLS.PRESSUPPER, 
						UI.ClickType.CLICKABLE => text2 + UI.CONTROLS.PRESSABLEUPPER, 
						UI.ClickType.CLICKED => text2 + UI.CONTROLS.PRESSEDUPPER, 
						UI.ClickType.CLICKING => text2 + UI.CONTROLS.PRESSINGUPPER, 
						UI.ClickType.CLICKS => text2 + UI.CONTROLS.PRESSESUPPER, 
						_ => text2 + array[i], 
					}))) : (text2 + array[i]));
				}
				text = text2;
			}
		}
		return text;
	}

	private void RefreshText()
	{
		if (originalString != string.Empty)
		{
			SetText(originalString);
		}
	}

	protected override void GenerateTextMesh()
	{
		base.GenerateTextMesh();
	}

	internal void SwapFont(TMP_FontAsset font, bool isRightToLeft)
	{
		base.font = font;
		if (key != "")
		{
			StringEntry stringEntry = Strings.Get(new StringKey(key));
			text = stringEntry.String;
		}
		text = Localization.Fixup(text);
		base.isRightToLeftText = isRightToLeft;
	}

	private static string ModifyLinkStrings(string input)
	{
		if (input == null || input.IndexOf("<b><style=\"KLink\">") != -1)
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input);
		stringBuilder.Replace("<link=\"", combinedPrefix);
		stringBuilder.Replace("</link>", combinedSuffix);
		return stringBuilder.ToString();
	}

	private void RefreshLinkHandler()
	{
		if (textLinkHandler == null && allowLinksInternal)
		{
			textLinkHandler = GetComponent<TextLinkHandler>();
			if (textLinkHandler == null)
			{
				textLinkHandler = base.gameObject.AddComponent<TextLinkHandler>();
			}
		}
		else if (!allowLinksInternal && textLinkHandler != null)
		{
			UnityEngine.Object.Destroy(textLinkHandler);
			textLinkHandler = null;
		}
		if (textLinkHandler != null)
		{
			textLinkHandler.CheckMouseOver();
		}
	}
}
