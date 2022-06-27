public class KButtonEvent : KInputEvent
{
	private bool[] mIsAction;

	private Action mAction;

	public KButtonEvent(KInputController controller, InputEventType event_type, bool[] is_action)
		: base(controller, event_type)
	{
		mIsAction = is_action;
	}

	public KButtonEvent(KInputController controller, InputEventType event_type, Action action)
		: base(controller, event_type)
	{
		mIsAction = null;
		mAction = action;
	}

	public bool TryConsume(Action action)
	{
		if (base.Consumed)
		{
			return base.Consumed;
		}
		if (action != Action.NumActions)
		{
			if (mIsAction != null)
			{
				if (mIsAction[(int)action])
				{
					base.Consumed = true;
				}
			}
			else if (mAction == action)
			{
				base.Consumed = true;
			}
		}
		return base.Consumed;
	}

	public bool IsAction(Action action)
	{
		if (mIsAction != null)
		{
			return mIsAction[(int)action];
		}
		return mAction == action;
	}

	public Action GetAction()
	{
		if (mIsAction != null)
		{
			for (int i = 0; i < mIsAction.Length; i++)
			{
				if (mIsAction[i])
				{
					return (Action)i;
				}
			}
			return Action.NumActions;
		}
		return mAction;
	}
}
