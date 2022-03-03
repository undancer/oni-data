using System;
using Steamworks;

public class SteamGamepadTextInput
{
	private static bool active;

	private static Action<SteamGamepadTextInputData> action;

	private static Callback<GamepadTextInputDismissed_t> GamepadInputDismissed;

	public static bool IsActive()
	{
		if (KInputManager.steamInputInterpreter.Initialized)
		{
			if (!KInputManager.currentControllerIsGamepad)
			{
				return SteamUtils.IsSteamRunningOnSteamDeck();
			}
			return true;
		}
		return false;
	}

	public static void ShowTextInputScreen(string desc, string init, Action<SteamGamepadTextInputData> action)
	{
		DebugUtil.DevAssert(!active, "Gamepad input already active.");
		if (SteamUtils.ShowGamepadTextInput(EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, desc, 512u, init))
		{
			GamepadInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(OnGamepadInputDismissed);
			SteamGamepadTextInput.action = action;
			active = true;
		}
	}

	private static void OnGamepadInputDismissed(GamepadTextInputDismissed_t callback)
	{
		SteamGamepadTextInputData obj = default(SteamGamepadTextInputData);
		obj.submitted = false;
		obj.input = "";
		if (callback.m_bSubmitted)
		{
			obj.submitted = true;
			if (SteamUtils.GetEnteredGamepadTextInput(out var pchText, callback.m_unSubmittedText) && pchText != null)
			{
				obj.input = pchText;
			}
		}
		GamepadInputDismissed.Dispose();
		active = false;
		action(obj);
	}
}
