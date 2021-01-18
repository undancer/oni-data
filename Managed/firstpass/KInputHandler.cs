using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class KInputHandler
{
	public delegate void KButtonEventHandler(KButtonEvent e);

	public delegate void KCancelInputHandler();

	private struct HandlerInfo
	{
		public int priority;

		public KInputHandler handler;
	}

	private List<Action<KButtonEvent>> mOnKeyDownDelegates = new List<Action<KButtonEvent>>();

	private List<Action<KButtonEvent>> mOnKeyUpDelegates = new List<Action<KButtonEvent>>();

	private List<HandlerInfo> mChildren;

	private KInputController mController;

	private string name;

	private KButtonEvent lastConsumedEventDown;

	private KButtonEvent lastConsumedEventUp;

	public KInputHandler(IInputHandler obj, KInputController controller)
		: this(obj)
	{
		mController = controller;
	}

	public KInputHandler(IInputHandler obj)
	{
		name = obj.handlerName;
		MethodInfo method = obj.GetType().GetMethod("OnKeyDown");
		if (method != null)
		{
			Action<KButtonEvent> item = (Action<KButtonEvent>)Delegate.CreateDelegate(typeof(Action<KButtonEvent>), obj, method);
			mOnKeyDownDelegates.Add(item);
		}
		MethodInfo method2 = obj.GetType().GetMethod("OnKeyUp");
		if (method2 != null)
		{
			Action<KButtonEvent> item2 = (Action<KButtonEvent>)Delegate.CreateDelegate(typeof(Action<KButtonEvent>), obj, method2);
			mOnKeyUpDelegates.Add(item2);
		}
	}

	private void SetController(KInputController controller)
	{
		mController = controller;
		if (mChildren == null)
		{
			return;
		}
		foreach (HandlerInfo mChild in mChildren)
		{
			mChild.handler.SetController(controller);
		}
	}

	public void AddInputHandler(KInputHandler handler, int priority)
	{
		if (mChildren == null)
		{
			mChildren = new List<HandlerInfo>();
		}
		handler.SetController(mController);
		mChildren.Add(new HandlerInfo
		{
			priority = priority,
			handler = handler
		});
		mChildren.Sort((HandlerInfo a, HandlerInfo b) => b.priority.CompareTo(a.priority));
	}

	public void RemoveInputHandler(KInputHandler handler)
	{
		if (mChildren == null)
		{
			return;
		}
		for (int i = 0; i < mChildren.Count; i++)
		{
			if (mChildren[i].handler == handler)
			{
				mChildren.RemoveAt(i);
				break;
			}
		}
	}

	public void PushInputHandler(KInputHandler handler)
	{
		if (mChildren == null)
		{
			mChildren = new List<HandlerInfo>();
		}
		handler.SetController(mController);
		mChildren.Insert(0, new HandlerInfo
		{
			priority = int.MaxValue,
			handler = handler
		});
	}

	public void PopInputHandler()
	{
		if (mChildren != null)
		{
			mChildren.RemoveAt(0);
		}
	}

	public void HandleEvent(KInputEvent e)
	{
		if (e.Type == InputEventType.KeyDown)
		{
			HandleKeyDown((KButtonEvent)e);
		}
		else if (InputEventType.KeyUp == e.Type)
		{
			HandleKeyUp((KButtonEvent)e);
		}
	}

	public void HandleKeyDown(KButtonEvent e)
	{
		lastConsumedEventDown = null;
		foreach (Action<KButtonEvent> mOnKeyDownDelegate in mOnKeyDownDelegates)
		{
			mOnKeyDownDelegate(e);
			if (e.Consumed)
			{
				lastConsumedEventDown = e;
			}
		}
		if (e.Consumed || mChildren == null)
		{
			return;
		}
		foreach (HandlerInfo mChild in mChildren)
		{
			mChild.handler.HandleKeyDown(e);
			if (e.Consumed)
			{
				break;
			}
		}
	}

	public void HandleKeyUp(KButtonEvent e)
	{
		lastConsumedEventUp = null;
		foreach (Action<KButtonEvent> mOnKeyUpDelegate in mOnKeyUpDelegates)
		{
			mOnKeyUpDelegate(e);
			if (e.Consumed)
			{
				lastConsumedEventUp = e;
			}
		}
		if (e.Consumed || mChildren == null)
		{
			return;
		}
		foreach (HandlerInfo mChild in mChildren)
		{
			mChild.handler.HandleKeyUp(e);
			if (e.Consumed)
			{
				break;
			}
		}
	}

	public static KInputHandler GetInputHandler(IInputHandler handler)
	{
		if (handler.inputHandler == null)
		{
			handler.inputHandler = new KInputHandler(handler);
		}
		return handler.inputHandler;
	}

	public static void Add(IInputHandler parent, GameObject child)
	{
		Component[] components = child.GetComponents<Component>();
		foreach (Component component in components)
		{
			IInputHandler inputHandler = component as IInputHandler;
			if (inputHandler != null)
			{
				Add(parent, inputHandler);
			}
		}
	}

	public static void Add(IInputHandler parent, IInputHandler child, int priority = 0)
	{
		KInputHandler inputHandler = GetInputHandler(parent);
		KInputHandler inputHandler2 = GetInputHandler(child);
		inputHandler.AddInputHandler(inputHandler2, priority);
	}

	public static void Push(IInputHandler parent, IInputHandler child)
	{
		KInputHandler inputHandler = GetInputHandler(parent);
		KInputHandler inputHandler2 = GetInputHandler(child);
		inputHandler.PushInputHandler(inputHandler2);
	}

	public static void Remove(IInputHandler parent, IInputHandler child)
	{
		KInputHandler inputHandler = GetInputHandler(parent);
		KInputHandler inputHandler2 = GetInputHandler(child);
		inputHandler.RemoveInputHandler(inputHandler2);
	}

	public bool IsActive(Action action)
	{
		if (mController != null)
		{
			return mController.IsActive(action);
		}
		return false;
	}

	public float GetAxis(Axis axis)
	{
		if (mController != null)
		{
			return mController.GetAxis(axis);
		}
		return 0f;
	}

	public bool IsGamepad()
	{
		if (mController != null)
		{
			return mController.IsGamepad;
		}
		return false;
	}
}
