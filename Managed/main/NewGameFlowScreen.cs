using System;

public abstract class NewGameFlowScreen : KModalScreen
{
	public event System.Action OnNavigateForward;

	public event System.Action OnNavigateBackward;

	protected void NavigateBackward()
	{
		this.OnNavigateBackward();
	}

	protected void NavigateForward()
	{
		this.OnNavigateForward();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			if (e.TryConsume(Action.MouseRight))
			{
				NavigateBackward();
			}
			base.OnKeyDown(e);
		}
	}
}
