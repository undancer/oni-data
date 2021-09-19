using System;
using UnityEngine;

public class ClusterMapRocketAnimator : GameStateMachine<ClusterMapRocketAnimator, ClusterMapRocketAnimator.StatesInstance, ClusterMapVisualizer>
{
	public class MovingStates : State
	{
		public State takeoff;

		public State traveling;

		public State landing;
	}

	public class UtilityStates : State
	{
		public class MiningStates : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public MiningStates mining;
	}

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
				DebugUtil.DevAssert(animCompleteSubscriber != null, "ClusterMapRocketAnimator animCompleteSubscriber GameObject is null. Whatever the previous gameObject in this variable was, it may not have unsubscribed from an event properly");
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

	public State idle;

	public State grounded;

	public MovingStates moving;

	public UtilityStates utility;

	public State exploding;

	public State exploding_pst;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = idle;
		root.OnTargetLost(entityTarget, null).Target(entityTarget).EventHandlerTransition(GameHashes.RocketSelfDestructRequested, exploding, (StatesInstance smi, object data) => true)
			.EventHandlerTransition(GameHashes.StartMining, utility.mining, (StatesInstance smi, object data) => true)
			.EventHandlerTransition(GameHashes.RocketLaunched, moving.takeoff, (StatesInstance smi, object data) => true);
		idle.Target(masterTarget).Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		}).Target(entityTarget)
			.Transition(moving.traveling, IsTraveling)
			.Transition(grounded, IsGrounded)
			.Transition(moving.landing, IsLanding)
			.Transition(utility.mining, IsMining);
		grounded.Enter(delegate(StatesInstance smi)
		{
			ToggleSelectable(isSelectable: false, smi);
			smi.ToggleVisAnim(on: false);
		}).Exit(delegate(StatesInstance smi)
		{
			ToggleSelectable(isSelectable: true, smi);
			smi.ToggleVisAnim(on: true);
		}).Target(entityTarget)
			.EventTransition(GameHashes.RocketLaunched, moving.takeoff);
		moving.takeoff.Transition(idle, GameStateMachine<ClusterMapRocketAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(IsSurfaceTransitioning)).Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("launching", KAnim.PlayMode.Loop);
			ToggleSelectable(isSelectable: false, smi);
		}).Exit(delegate(StatesInstance smi)
		{
			ToggleSelectable(isSelectable: true, smi);
		});
		moving.landing.Transition(idle, GameStateMachine<ClusterMapRocketAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(IsSurfaceTransitioning)).Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("landing", KAnim.PlayMode.Loop);
			ToggleSelectable(isSelectable: false, smi);
		}).Exit(delegate(StatesInstance smi)
		{
			ToggleSelectable(isSelectable: true, smi);
		});
		moving.traveling.Target(entityTarget).EventTransition(GameHashes.ClusterLocationChanged, idle, GameStateMachine<ClusterMapRocketAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(IsTraveling)).EventTransition(GameHashes.ClusterDestinationChanged, idle, GameStateMachine<ClusterMapRocketAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(IsTraveling))
			.Enter(delegate(StatesInstance smi)
			{
				smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
			});
		utility.Target(masterTarget).EventTransition(GameHashes.ClusterDestinationChanged, idle, IsTraveling);
		utility.mining.DefaultState(utility.mining.pre).Target(entityTarget).EventTransition(GameHashes.StopMining, utility.mining.pst);
		utility.mining.pre.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("mining_pre", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate
			{
				smi.GoTo(utility.mining.loop);
			});
		});
		utility.mining.loop.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("mining_loop", KAnim.PlayMode.Loop);
		});
		utility.mining.pst.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("mining_pst", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate
			{
				smi.GoTo(idle);
			});
		});
		exploding.Enter(delegate(StatesInstance smi)
		{
			smi.GetComponent<ClusterMapVisualizer>().GetFirstAnimController().SwapAnims(new KAnimFile[1] { Assets.GetAnim("rocket_self_destruct_kanim") });
			smi.PlayVisAnim("explode", KAnim.PlayMode.Once);
			smi.SubscribeOnVisAnimComplete(delegate
			{
				smi.GoTo(exploding_pst);
			});
		});
		exploding_pst.Enter(delegate(StatesInstance smi)
		{
			smi.GetComponent<ClusterMapVisualizer>().GetFirstAnimController().Stop();
			smi.entity.gameObject.Trigger(-1311384361);
		});
	}

	private bool ClusterChangedAtMyLocation(StatesInstance smi, object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		if (!(clusterLocationChangedEvent.oldLocation == smi.entity.Location))
		{
			return clusterLocationChangedEvent.newLocation == smi.entity.Location;
		}
		return true;
	}

	private bool IsTraveling(StatesInstance smi)
	{
		if (smi.entity.GetComponent<ClusterTraveler>().IsTraveling())
		{
			return ((Clustercraft)smi.entity).HasResourcesToMove();
		}
		return false;
	}

	private bool IsGrounded(StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).Status == Clustercraft.CraftStatus.Grounded;
	}

	private bool IsLanding(StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).Status == Clustercraft.CraftStatus.Landing;
	}

	private bool IsMining(StatesInstance smi)
	{
		return ((Clustercraft)smi.entity).HasTag(GameTags.POIHarvesting);
	}

	private bool IsSurfaceTransitioning(StatesInstance smi)
	{
		Clustercraft clustercraft = smi.entity as Clustercraft;
		if (clustercraft != null)
		{
			if (clustercraft.Status != Clustercraft.CraftStatus.Landing)
			{
				return clustercraft.Status == Clustercraft.CraftStatus.Launching;
			}
			return true;
		}
		return false;
	}

	private void ToggleSelectable(bool isSelectable, StatesInstance smi)
	{
		KSelectable component = smi.entity.GetComponent<KSelectable>();
		component.IsSelectable = isSelectable;
		if (!isSelectable && component.IsSelected && ClusterMapScreen.Instance.GetMode() != ClusterMapScreen.Mode.SelectDestination)
		{
			ClusterMapSelectTool.Instance.Select(null, skipSound: true);
			SelectTool.Instance.Select(null, skipSound: true);
		}
	}
}
