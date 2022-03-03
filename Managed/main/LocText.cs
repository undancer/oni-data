using System.Text;
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

	[SerializeField]
	private bool allowLinksInternal;

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
		if (input.Contains("{Hotkey/"))
		{
			string[] array = input.Split('{', '}');
			if (array.Length >= 3)
			{
				string text = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					if (i % 2 == 0)
					{
						text += array[i];
						continue;
					}
					string text2 = array[i].Split('/')[1];
					for (int j = 0; j < 273; j++)
					{
						Action action = (Action)j;
						string text3 = action.ToString();
						if (text2 == text3)
						{
							text += GameUtil.ReplaceHotkeyString("{Hotkey}", (Action)j);
							break;
						}
					}
				}
				input = text;
				return text;
			}
		}
		return input;
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
			Object.Destroy(textLinkHandler);
			textLinkHandler = null;
		}
		if (textLinkHandler != null)
		{
			textLinkHandler.CheckMouseOver();
		}
	}
}
