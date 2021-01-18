public class FullPuftTransitionLayer : TransitionDriver.OverrideLayer
{
	public FullPuftTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (navigator.GetSMI<CreatureCalorieMonitor.Instance>()?.stomach.IsReadyToPoop() ?? false)
		{
			KBatchedAnimController component = navigator.GetComponent<KBatchedAnimController>();
			string s = HashCache.Get().Get(transition.anim.HashValue) + "_full";
			if (component.HasAnimation(s))
			{
				transition.anim = s;
			}
		}
	}
}
