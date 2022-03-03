using System.Collections.Generic;

public class GameInputManager : KInputManager
{
	public List<IInputHandler> usedMenus = new List<IInputHandler>();

	public KInputController AddKeyboardMouseController()
	{
		KInputController kInputController = new KInputController(is_gamepad: false);
		BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
		for (int i = 0; i < bindingEntries.Length; i++)
		{
			BindingEntry bindingEntry = bindingEntries[i];
			kInputController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
		}
		AddController(kInputController);
		return kInputController;
	}

	public KInputController AddGamepadController(int gamepad_index)
	{
		KInputController kInputController = new KInputController(is_gamepad: true);
		BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
		for (int i = 0; i < bindingEntries.Length; i++)
		{
			BindingEntry bindingEntry = bindingEntries[i];
			kInputController.Bind(bindingEntry.mKeyCode, Modifier.None, bindingEntry.mAction);
		}
		AddController(kInputController);
		return kInputController;
	}

	public GameInputManager(BindingEntry[] default_keybindings)
	{
		GameInputMapping.SetDefaultKeyBindings(default_keybindings);
		GameInputMapping.LoadBindings();
		AddKeyboardMouseController();
		KInputManager.steamInputInterpreter.OnEnable();
		if (KInputManager.steamInputInterpreter.NumOfISteamInputs > 0)
		{
			AddGamepadController(GetControllerCount());
		}
	}

	public void RebindControls()
	{
		foreach (KInputController mController in mControllers)
		{
			mController.ClearBindings();
			BindingEntry[] bindingEntries = GameInputMapping.GetBindingEntries();
			for (int i = 0; i < bindingEntries.Length; i++)
			{
				BindingEntry bindingEntry = bindingEntries[i];
				mController.Bind(bindingEntry.mKeyCode, bindingEntry.mModifier, bindingEntry.mAction);
			}
			mController.HandleCancelInput();
		}
		KInputManager.InputChange.Invoke();
	}

	public override void Update()
	{
		if (!KInputManager.isFocused)
		{
			return;
		}
		KInputManager.steamInputInterpreter.Update();
		if (KInputManager.steamInputInterpreter.NumOfISteamInputs > 0 && GetControllerCount() <= 1)
		{
			AddGamepadController(GetControllerCount());
		}
		else if (KInputManager.steamInputInterpreter.NumOfISteamInputs < 1 && KInputManager.currentControllerIsGamepad)
		{
			KInputManager.currentControllerIsGamepad = false;
			KInputManager.InputChange.Invoke();
		}
		for (int i = 0; i < mControllers.Count && i + 1 < mControllers.Count; i++)
		{
			if (mControllers[i].inputHandler.HandleChildCount() != mControllers[i + 1].inputHandler.HandleChildCount())
			{
				mControllers[i].inputHandler.TransferHandles(mControllers[i + 1].inputHandler);
			}
		}
		base.Update();
	}

	public override void OnApplicationFocus(bool focusStatus)
	{
		base.OnApplicationFocus(focusStatus);
	}
}
