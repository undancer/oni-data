using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleSwitch : MonoBehaviour
{
	public VirtualInputModule virtualInput;

	public StandaloneInputModule standaloneInput;

	private Vector3 lastMousePosition;

	private void Update()
	{
		if (lastMousePosition != Input.mousePosition && KInputManager.currentControllerIsGamepad)
		{
			KInputManager.currentControllerIsGamepad = false;
			KInputManager.InputChange.Invoke();
		}
		if (KInputManager.currentControllerIsGamepad)
		{
			virtualInput.enabled = KInputManager.currentControllerIsGamepad;
			if (standaloneInput.enabled)
			{
				standaloneInput.enabled = false;
				ChangeInputHandler();
			}
			return;
		}
		lastMousePosition = Input.mousePosition;
		standaloneInput.enabled = true;
		if (virtualInput.enabled)
		{
			virtualInput.enabled = false;
			ChangeInputHandler();
		}
	}

	private void ChangeInputHandler()
	{
		for (int i = 0; i < Global.Instance.GetInputManager().usedMenus.Count; i++)
		{
			if (Global.Instance.GetInputManager().usedMenus[i].Equals(null))
			{
				Global.Instance.GetInputManager().usedMenus.RemoveAt(i);
			}
		}
		if (Global.Instance.GetInputManager().GetControllerCount() > 1)
		{
			if (KInputManager.currentControllerIsGamepad)
			{
				Cursor.visible = false;
				Global.Instance.GetInputManager().GetController(1).inputHandler.TransferHandles(Global.Instance.GetInputManager().GetController(0).inputHandler);
			}
			else
			{
				Cursor.visible = true;
				Global.Instance.GetInputManager().GetController(0).inputHandler.TransferHandles(Global.Instance.GetInputManager().GetController(1).inputHandler);
			}
		}
	}
}
