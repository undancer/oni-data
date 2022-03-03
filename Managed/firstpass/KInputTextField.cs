using System;
using TMPro;

public class KInputTextField : TMP_InputField
{
	private KInputTextField()
	{
		onFocus = (System.Action)Delegate.Combine(onFocus, (System.Action)delegate
		{
			if (SteamGamepadTextInput.IsActive())
			{
				SteamGamepadTextInput.ShowTextInputScreen("", base.text, OnGamepadInputDismissed);
			}
		});
	}

	private void OnGamepadInputDismissed(SteamGamepadTextInputData data)
	{
		if (data.submitted)
		{
			base.text = data.input;
		}
		OnDeselect(null);
	}
}
