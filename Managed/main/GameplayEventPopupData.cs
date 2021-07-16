using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GameplayEventPopupData
{
	public class PopupOptionIcon
	{
		public enum ContainerType
		{
			Neutral,
			Positive,
			Negative,
			Information
		}

		public ContainerType containerType;

		public Sprite sprite;

		public string tooltip;

		public float scale;

		public PopupOptionIcon(Sprite sprite, ContainerType containerType, string tooltip, float scale = 1f)
		{
			this.sprite = sprite;
			this.containerType = containerType;
			this.tooltip = tooltip;
			this.scale = scale;
		}
	}

	public class PopupOption
	{
		public string mainText;

		public string description;

		public string tooltip;

		public System.Action callback;

		public List<PopupOptionIcon> informationIcons = new List<PopupOptionIcon>();

		public List<PopupOptionIcon> consequenceIcons = new List<PopupOptionIcon>();

		public bool allowed = true;

		public void AddInformationIcon(string tooltip, float scale = 1f)
		{
			informationIcons.Add(new PopupOptionIcon(null, PopupOptionIcon.ContainerType.Information, tooltip, scale));
		}

		public void AddPositiveIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			consequenceIcons.Add(new PopupOptionIcon(sprite, PopupOptionIcon.ContainerType.Positive, tooltip, scale));
		}

		public void AddNeutralIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			consequenceIcons.Add(new PopupOptionIcon(sprite, PopupOptionIcon.ContainerType.Neutral, tooltip, scale));
		}

		public void AddNegativeIcon(Sprite sprite, string tooltip, float scale = 1f)
		{
			consequenceIcons.Add(new PopupOptionIcon(sprite, PopupOptionIcon.ContainerType.Negative, tooltip, scale));
		}
	}

	public string title;

	public string description;

	public string location;

	public string whenDescription;

	public Transform focus;

	public GameObject[] minions;

	public GameObject artifact;

	public HashedString animFileName;

	public HashedString backgroundFileName;

	public Color32 backgroundTint;

	public Dictionary<string, string> textParameters = new Dictionary<string, string>();

	public List<PopupOption> options = new List<PopupOption>();

	private bool dirty;

	public GameplayEventPopupData()
	{
	}

	public GameplayEventPopupData(GameplayEvent evt)
	{
		title = evt.popupTitle;
		description = evt.popupDescription;
		animFileName = evt.popupAnimFileName;
		backgroundFileName = evt.popupBackgroundFileName;
		backgroundTint = evt.popupBackgroundTint;
	}

	public List<PopupOption> GetOptions()
	{
		FinalizeText();
		return options;
	}

	public PopupOption AddOption(string mainText, string description = null)
	{
		PopupOption popupOption = new PopupOption();
		popupOption.mainText = mainText;
		popupOption.description = description;
		options.Add(popupOption);
		dirty = true;
		return popupOption;
	}

	public PopupOption SimpleOption(string mainText, System.Action callback)
	{
		PopupOption popupOption = new PopupOption
		{
			mainText = mainText,
			callback = callback
		};
		options.Add(popupOption);
		dirty = true;
		return popupOption;
	}

	public PopupOption AddDefaultOption(System.Action callback = null)
	{
		return SimpleOption(GAMEPLAY_EVENTS.DEFAULT_OPTION_NAME, callback);
	}

	public PopupOption AddDefaultConsiderLaterOption(System.Action callback = null)
	{
		return SimpleOption(GAMEPLAY_EVENTS.DEFAULT_OPTION_CONSIDER_NAME, callback);
	}

	public void SetTextParameter(string key, string value)
	{
		textParameters[key] = value;
		dirty = true;
	}

	public void FinalizeText()
	{
		if (!dirty)
		{
			return;
		}
		dirty = false;
		foreach (KeyValuePair<string, string> textParameter in textParameters)
		{
			string oldValue = "{" + textParameter.Key + "}";
			if (title != null)
			{
				title = title.Replace(oldValue, textParameter.Value);
			}
			if (description != null)
			{
				description = description.Replace(oldValue, textParameter.Value);
			}
			if (location != null)
			{
				location = location.Replace(oldValue, textParameter.Value);
			}
			if (whenDescription != null)
			{
				whenDescription = whenDescription.Replace(oldValue, textParameter.Value);
			}
			foreach (PopupOption option in options)
			{
				if (option.mainText != null)
				{
					option.mainText = option.mainText.Replace(oldValue, textParameter.Value);
				}
				if (option.description != null)
				{
					option.description = option.description.Replace(oldValue, textParameter.Value);
				}
				if (option.tooltip != null)
				{
					option.tooltip = option.tooltip.Replace(oldValue, textParameter.Value);
				}
				foreach (PopupOptionIcon informationIcon in option.informationIcons)
				{
					if (informationIcon.tooltip != null)
					{
						informationIcon.tooltip = informationIcon.tooltip.Replace(oldValue, textParameter.Value);
					}
				}
				foreach (PopupOptionIcon consequenceIcon in option.consequenceIcons)
				{
					if (consequenceIcon.tooltip != null)
					{
						consequenceIcon.tooltip = consequenceIcon.tooltip.Replace(oldValue, textParameter.Value);
					}
				}
			}
		}
	}
}
