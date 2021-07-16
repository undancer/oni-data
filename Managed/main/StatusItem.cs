using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusItem : Resource
{
	public enum IconType
	{
		Info,
		Exclamation,
		Custom
	}

	[Flags]
	public enum StatusItemOverlays
	{
		None = 0x2,
		PowerMap = 0x4,
		Temperature = 0x8,
		ThermalComfort = 0x10,
		Light = 0x20,
		LiquidPlumbing = 0x40,
		GasPlumbing = 0x80,
		Decor = 0x100,
		Pathogens = 0x200,
		Farming = 0x400,
		Rooms = 0x1000,
		Suits = 0x2000,
		Logic = 0x4000,
		Conveyor = 0x8000,
		Radiation = 0x10000
	}

	public string tooltipText;

	public string notificationText;

	public string notificationTooltipText;

	public string soundPath;

	public string iconName;

	public TintedSprite sprite;

	public bool shouldNotify;

	public IconType iconType;

	public NotificationType notificationType;

	public Notification.ClickCallback notificationClickCallback;

	public Func<string, object, string> resolveStringCallback;

	public Func<string, object, string> resolveTooltipCallback;

	public bool allowMultiples;

	public Func<HashedString, object, bool> conditionalOverlayCallback;

	public HashedString render_overlay;

	public int status_overlays;

	public Action<object> statusItemClickCallback;

	private string composedPrefix;

	private bool showShowWorldIcon = true;

	public const int ALL_OVERLAYS = 129022;

	private static Dictionary<HashedString, StatusItemOverlays> overlayBitfieldMap = new Dictionary<HashedString, StatusItemOverlays>
	{
		{
			OverlayModes.None.ID,
			StatusItemOverlays.None
		},
		{
			OverlayModes.Power.ID,
			StatusItemOverlays.PowerMap
		},
		{
			OverlayModes.Temperature.ID,
			StatusItemOverlays.Temperature
		},
		{
			OverlayModes.ThermalConductivity.ID,
			StatusItemOverlays.ThermalComfort
		},
		{
			OverlayModes.Light.ID,
			StatusItemOverlays.Light
		},
		{
			OverlayModes.LiquidConduits.ID,
			StatusItemOverlays.LiquidPlumbing
		},
		{
			OverlayModes.GasConduits.ID,
			StatusItemOverlays.GasPlumbing
		},
		{
			OverlayModes.SolidConveyor.ID,
			StatusItemOverlays.Conveyor
		},
		{
			OverlayModes.Decor.ID,
			StatusItemOverlays.Decor
		},
		{
			OverlayModes.Disease.ID,
			StatusItemOverlays.Pathogens
		},
		{
			OverlayModes.Crop.ID,
			StatusItemOverlays.Farming
		},
		{
			OverlayModes.Rooms.ID,
			StatusItemOverlays.Rooms
		},
		{
			OverlayModes.Suit.ID,
			StatusItemOverlays.Suits
		},
		{
			OverlayModes.Logic.ID,
			StatusItemOverlays.Logic
		},
		{
			OverlayModes.Oxygen.ID,
			StatusItemOverlays.None
		},
		{
			OverlayModes.TileMode.ID,
			StatusItemOverlays.None
		},
		{
			OverlayModes.Radiation.ID,
			StatusItemOverlays.Radiation
		}
	};

	private StatusItem(string id, string composed_prefix)
		: base(id, Strings.Get(composed_prefix + ".NAME"))
	{
		composedPrefix = composed_prefix;
		tooltipText = Strings.Get(composed_prefix + ".TOOLTIP");
	}

	public StatusItem(string id, string prefix, string icon, IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 129022, Func<string, object, string> resolve_string_callback = null)
		: this(id, "STRINGS." + prefix + ".STATUSITEMS." + id.ToUpper())
	{
		switch (icon_type)
		{
		case IconType.Info:
			icon = "dash";
			break;
		case IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		iconName = icon;
		notificationType = notification_type;
		sprite = Assets.GetTintedSprite(icon);
		iconType = icon_type;
		allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		showShowWorldIcon = showWorldIcon;
		this.status_overlays = status_overlays;
		resolveStringCallback = resolve_string_callback;
		if (sprite == null)
		{
			Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon);
		}
	}

	public StatusItem(string id, string name, string tooltip, string icon, IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 129022, bool showWorldIcon = true, Func<string, object, string> resolve_string_callback = null)
		: base(id, name)
	{
		switch (icon_type)
		{
		case IconType.Info:
			icon = "dash";
			break;
		case IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		iconName = icon;
		notificationType = notification_type;
		sprite = Assets.GetTintedSprite(icon);
		tooltipText = tooltip;
		iconType = icon_type;
		allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		this.status_overlays = status_overlays;
		showShowWorldIcon = showWorldIcon;
		resolveStringCallback = resolve_string_callback;
		if (sprite == null)
		{
			Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon);
		}
	}

	public void AddNotification(string sound_path = null, string notification_text = null, string notification_tooltip = null)
	{
		shouldNotify = true;
		if (sound_path == null)
		{
			if (notificationType == NotificationType.Bad)
			{
				soundPath = "Warning";
			}
			else
			{
				soundPath = "Notification";
			}
		}
		else
		{
			soundPath = sound_path;
		}
		if (notification_text != null)
		{
			notificationText = notification_text;
		}
		else
		{
			DebugUtil.Assert(composedPrefix != null, "When adding a notification, either set the status prefix or specify strings!");
			notificationText = Strings.Get(composedPrefix + ".NOTIFICATION_NAME");
		}
		if (notification_tooltip != null)
		{
			notificationTooltipText = notification_tooltip;
			return;
		}
		DebugUtil.Assert(composedPrefix != null, "When adding a notification, either set the status prefix or specify strings!");
		notificationTooltipText = Strings.Get(composedPrefix + ".NOTIFICATION_TOOLTIP");
	}

	public virtual string GetName(object data)
	{
		return ResolveString(Name, data);
	}

	public virtual string GetTooltip(object data)
	{
		return ResolveTooltip(tooltipText, data);
	}

	private string ResolveString(string str, object data)
	{
		if (resolveStringCallback != null && data != null)
		{
			return resolveStringCallback(str, data);
		}
		return str;
	}

	private string ResolveTooltip(string str, object data)
	{
		if (data != null)
		{
			if (resolveTooltipCallback != null)
			{
				return resolveTooltipCallback(str, data);
			}
			if (resolveStringCallback != null)
			{
				return resolveStringCallback(str, data);
			}
		}
		return str;
	}

	public bool ShouldShowIcon()
	{
		if (iconType == IconType.Custom)
		{
			return showShowWorldIcon;
		}
		return false;
	}

	public virtual void ShowToolTip(ToolTip tooltip_widget, object data, TextStyleSetting property_style)
	{
		tooltip_widget.ClearMultiStringTooltip();
		string tooltip = GetTooltip(data);
		tooltip_widget.AddMultiStringTooltip(tooltip, property_style);
	}

	public void SetIcon(Image image, object data)
	{
		if (sprite != null)
		{
			image.color = sprite.color;
			image.sprite = sprite.sprite;
		}
	}

	public bool UseConditionalCallback(HashedString overlay, Transform transform)
	{
		if (overlay != OverlayModes.None.ID && conditionalOverlayCallback != null)
		{
			return conditionalOverlayCallback(overlay, transform);
		}
		return false;
	}

	public StatusItem SetResolveStringCallback(Func<string, object, string> cb)
	{
		resolveStringCallback = cb;
		return this;
	}

	public void OnClick(object data)
	{
		if (statusItemClickCallback != null)
		{
			statusItemClickCallback(data);
		}
	}

	public static StatusItemOverlays GetStatusItemOverlayBySimViewMode(HashedString mode)
	{
		if (!overlayBitfieldMap.TryGetValue(mode, out var value))
		{
			HashedString hashedString = mode;
			Debug.LogWarning("ViewMode " + hashedString.ToString() + " has no StatusItemOverlay value");
			return StatusItemOverlays.None;
		}
		return value;
	}
}
