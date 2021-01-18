using UnityEngine;

public class SelfEmoteReactable : EmoteReactable
{
	private EmoteChore emote;

	public SelfEmoteReactable(GameObject gameObject, HashedString id, ChoreType chore_type, HashedString animset, float min_reactable_time = 0f, float min_reactor_time = 20f, float max_trigger_time = float.PositiveInfinity)
		: base(gameObject, id, chore_type, animset, 3, 3, min_reactable_time, min_reactor_time, max_trigger_time)
	{
	}

	public override bool InternalCanBegin(GameObject reactor, Navigator.ActiveTransition transition)
	{
		if (reactor == null)
		{
			return false;
		}
		Navigator component = reactor.GetComponent<Navigator>();
		if (component == null)
		{
			return false;
		}
		if (!component.IsMoving())
		{
			return false;
		}
		return gameObject == reactor;
	}

	public void PairEmote(EmoteChore emote)
	{
		this.emote = emote;
	}

	protected override void InternalEnd()
	{
		if (emote != null && emote.driver != null)
		{
			emote.PairReactable(null);
			emote.Cancel("Reactable ended");
			emote = null;
		}
		base.InternalEnd();
	}
}
