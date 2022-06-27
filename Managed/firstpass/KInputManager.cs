using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KInputManager
{
	protected List<KInputController> mControllers = new List<KInputController>();

	private static bool hasFocus = false;

	public static bool devToolFocus = false;

	public static SteamInputInterpreter steamInputInterpreter = new SteamInputInterpreter();

	public static Vector3F virtualCursorPos;

	public static bool currentControllerIsGamepad;

	public static KInputController prevController;

	public static KInputController currentController;

	public static UnityEvent InputChange = new UnityEvent();

	public static bool isMousePosLocked;

	public static Vector3 lockedMousePos;

	public static bool isFocused
	{
		get
		{
			if (hasFocus)
			{
				return !devToolFocus;
			}
			return false;
		}
	}

	public static long lastUserActionTicks { get; private set; }

	public static void SetUserActive()
	{
		if (isFocused)
		{
			lastUserActionTicks = DateTime.Now.Ticks;
		}
	}

	public KInputManager()
	{
		lastUserActionTicks = DateTime.Now.Ticks;
		hasFocus = true;
	}

	public void AddController(KInputController controller)
	{
		mControllers.Add(controller);
	}

	public KInputController GetController(int controller_index)
	{
		DebugUtil.Assert(controller_index < mControllers.Count);
		return mControllers[controller_index];
	}

	public int GetControllerCount()
	{
		return mControllers.Count;
	}

	public KInputController GetDefaultController()
	{
		return GetController(0);
	}

	public virtual void Update()
	{
		if (isFocused)
		{
			for (int i = 0; i < mControllers.Count; i++)
			{
				mControllers[i].Update();
			}
			Dispatch();
		}
	}

	public virtual void Dispatch()
	{
		if (isFocused)
		{
			for (int i = 0; i < mControllers.Count; i++)
			{
				mControllers[i].Dispatch();
			}
		}
	}

	public virtual void OnApplicationFocus(bool focus)
	{
		hasFocus = focus;
		SetUserActive();
		if (isFocused)
		{
			return;
		}
		foreach (KInputController mController in mControllers)
		{
			mController.HandleCancelInput();
		}
	}

	public static Vector3 GetMousePos()
	{
		if (isMousePosLocked)
		{
			return lockedMousePos;
		}
		if (currentControllerIsGamepad)
		{
			return virtualCursorPos;
		}
		return Input.mousePosition;
	}
}
