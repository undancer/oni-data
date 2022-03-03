using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct BindingEntry : IEquatable<BindingEntry>
{
	[JsonIgnore]
	public string mGroup;

	[JsonIgnore]
	public bool mRebindable;

	[JsonIgnore]
	public bool mIgnoreRootConflics;

	[JsonIgnore]
	public string[] dlcIds;

	[JsonConverter(typeof(StringEnumConverter))]
	public GamepadButton mButton;

	[JsonConverter(typeof(StringEnumConverter))]
	public KKeyCode mKeyCode;

	[JsonConverter(typeof(StringEnumConverter))]
	public Action mAction;

	[JsonConverter(typeof(StringEnumConverter))]
	public Modifier mModifier;

	public static KKeyCode GetGamepadKeyCode(int gamepad_number, GamepadButton button)
	{
		switch (gamepad_number)
		{
		case 0:
			return (KKeyCode)(button + 350);
		case 1:
			return (KKeyCode)(button + 370);
		case 2:
			return (KKeyCode)(button + 390);
		case 3:
			return (KKeyCode)(button + 410);
		default:
			DebugUtil.Assert(test: false);
			return KKeyCode.None;
		}
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, Action action, bool rebindable = true, bool ignore_root_conflicts = false)
		: this(group, button, key_code, modifier, action, rebindable, ignore_root_conflicts, null)
	{
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, Action action, string[] dlcIds)
		: this(group, button, key_code, modifier, action, rebindable: true, ignore_root_conflicts: false, dlcIds)
	{
	}

	public BindingEntry(string group, GamepadButton button, KKeyCode key_code, Modifier modifier, Action action, bool rebindable, bool ignore_root_conflicts, string[] dlcIds)
	{
		mGroup = group;
		mButton = button;
		mKeyCode = key_code;
		mAction = action;
		mModifier = modifier;
		mRebindable = rebindable;
		mIgnoreRootConflics = ignore_root_conflicts;
		this.dlcIds = dlcIds;
		if (this.dlcIds == null)
		{
			this.dlcIds = DlcManager.AVAILABLE_ALL_VERSIONS;
		}
	}

	public bool Equals(BindingEntry other)
	{
		return this == other;
	}

	public static bool operator ==(BindingEntry a, BindingEntry b)
	{
		if (a.mGroup == b.mGroup && a.mButton == b.mButton && a.mKeyCode == b.mKeyCode && a.mAction == b.mAction && a.mModifier == b.mModifier)
		{
			return a.mRebindable == b.mRebindable;
		}
		return false;
	}

	public bool IsBindingEqual(BindingEntry other)
	{
		if (mButton == other.mButton && mKeyCode == other.mKeyCode)
		{
			return mModifier == other.mModifier;
		}
		return false;
	}

	public static bool operator !=(BindingEntry a, BindingEntry b)
	{
		return !(a == b);
	}

	public override bool Equals(object o)
	{
		if (!(o is BindingEntry))
		{
			return false;
		}
		BindingEntry bindingEntry = (BindingEntry)o;
		return this == bindingEntry;
	}

	public override int GetHashCode()
	{
		return (int)mButton ^ (int)mKeyCode ^ (int)mAction;
	}
}
