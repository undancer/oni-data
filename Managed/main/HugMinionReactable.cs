using Klei.AI;
using UnityEngine;

public class HugMinionReactable : Reactable
{
	public HugMinionReactable(GameObject gameObject)
		: base(gameObject, "HugMinionReactable", Db.Get().ChoreTypes.Hug, 1, 1, follow_transform: true, 1f)
	{
	}

	public override bool InternalCanBegin(GameObject newReactor, Navigator.ActiveTransition transition)
	{
		if (reactor != null)
		{
			return false;
		}
		Navigator component = newReactor.GetComponent<Navigator>();
		if (component == null)
		{
			return false;
		}
		if (!component.IsMoving())
		{
			return false;
		}
		return true;
	}

	public override void Update(float dt)
	{
		gameObject.GetComponent<Facing>().SetFacing(reactor.GetComponent<Facing>().GetFacing());
	}

	protected override void InternalBegin()
	{
		KAnimControllerBase component = reactor.GetComponent<KAnimControllerBase>();
		component.AddAnimOverrides(Assets.GetAnim("anim_react_pip_kanim"));
		component.Play("hug_dupe_pre");
		component.Queue("hug_dupe_loop");
		component.Queue("hug_dupe_pst");
		component.onAnimComplete += Finish;
		gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnimSequence(new HashedString[3] { "hug_dupe_pre", "hug_dupe_loop", "hug_dupe_pst" });
	}

	private void Finish(HashedString anim)
	{
		if (anim == "hug_dupe_pst")
		{
			if (reactor != null)
			{
				reactor.GetComponent<KAnimControllerBase>().onAnimComplete -= Finish;
				ApplyEffects();
			}
			else
			{
				DebugUtil.DevLogError("HugMinionReactable finishing without adding a Hugged effect.");
			}
			End();
		}
	}

	private void ApplyEffects()
	{
		reactor.GetComponent<Effects>().Add("Hugged", should_save: true);
		gameObject.GetSMI<HugMonitor.Instance>()?.EnterHuggingFrenzy();
	}

	protected override void InternalEnd()
	{
	}

	protected override void InternalCleanup()
	{
	}
}
