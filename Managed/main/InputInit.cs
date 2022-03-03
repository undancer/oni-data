using UnityEngine;

internal class InputInit : MonoBehaviour
{
	private void Awake()
	{
		GameInputManager inputManager = Global.Instance.GetInputManager();
		for (int i = 0; i < inputManager.GetControllerCount(); i++)
		{
			KInputController controller = inputManager.GetController(i);
			if (!controller.IsGamepad)
			{
				continue;
			}
			Component[] components = base.gameObject.GetComponents<Component>();
			for (int j = 0; j < components.Length; j++)
			{
				IInputHandler inputHandler = components[j] as IInputHandler;
				if (inputHandler != null)
				{
					KInputHandler.Add(controller, inputHandler);
					Global.Instance.GetInputManager().usedMenus.Add(inputHandler);
				}
			}
		}
		if (KInputManager.currentController != null)
		{
			KInputHandler.Add(KInputManager.currentController, KScreenManager.Instance, 10);
		}
		else
		{
			KInputHandler.Add(inputManager.GetDefaultController(), KScreenManager.Instance, 10);
		}
		Global.Instance.GetInputManager().usedMenus.Add(KScreenManager.Instance);
		DebugHandler debugHandler = new DebugHandler();
		if (KInputManager.currentController != null)
		{
			KInputHandler.Add(KInputManager.currentController, debugHandler, -1);
		}
		else
		{
			KInputHandler.Add(inputManager.GetDefaultController(), debugHandler, -1);
		}
		Global.Instance.GetInputManager().usedMenus.Add(debugHandler);
	}
}
