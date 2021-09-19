using System;
using UnityEngine;

public class ClusterMapBallisticAnimator : GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer>
{
	public class StatesInstance : GameInstance
	{
		public ClusterGridEntity entity;

		private int animCompleteHandle = -1;

		private GameObject animCompleteSubscriber;

		public StatesInstance(ClusterMapVisualizer master, ClusterGridEntity entity)
			: base(master)
		{
			this.entity = entity;
			base.sm.entityTarget.Set(entity, this);
		}

		public void PlayVisAnim(string animName, KAnim.PlayMode playMode)
		{
			GetComponent<ClusterMapVisualizer>().PlayAnim(animName, playMode);
		}

		public void ToggleVisAnim(bool on)
		{
			ClusterMapVisualizer component = GetComponent<ClusterMapVisualizer>();
			if (!on)
			{
				component.GetFirstAnimController().Play("grounded");
			}
		}

		public void SubscribeOnVisAnimComplete(Action<object> action)
		{
			ClusterMapVisualizer component = GetComponent<ClusterMapVisualizer>();
			UnsubscribeOnVisAnimComplete();
			animCompleteSubscriber = component.GetFirstAnimController().gameObject;
			animCompleteHandle = animCompleteSubscriber.Subscribe(-1061186183, action);
		}

		public void UnsubscribeOnVisAnimComplete()
		{
			if (animCompleteHandle != -1)
			{
				DebugUtil.DevAssert(animCompleteSubscriber != null, "ClustermapBallisticAnimator animCompleteSubscriber GameObject is null. Whatever the previous gameObject in this variable was, it may not have unsubscribed from an event properly");
				animCompleteSubscriber.Unsubscribe(animCompleteHandle);
				animCompleteHandle = -1;
			}
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			UnsubscribeOnVisAnimComplete();
		}
	}

	public TargetParameter entityTarget;

	public State launching;

	public State moving;

	public State landing;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = moving;
		root.Target(entityTarget).TagTransition(GameTags.BallisticEntityLaunching, launching).TagTransition(GameTags.BallisticEntityLanding, landing)
			.TagTransition(GameTags.BallisticEntityMoving, moving);
		moving.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
		});
		landing.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("landing", KAnim.PlayMode.Loop);
		});
		launching.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("launching", KAnim.PlayMode.Loop);
		});
	}
}
