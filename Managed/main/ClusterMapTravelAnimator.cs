using UnityEngine;

public class ClusterMapTravelAnimator : GameStateMachine<ClusterMapTravelAnimator, ClusterMapTravelAnimator.StatesInstance, ClusterMapVisualizer>
{
	private class Tuning : TuningData<Tuning>
	{
		public float visualizerTransitionSpeed = 1f;

		public float visualizerRotationSpeed = 1f;
	}

	public class TravelingStates : State
	{
		public State travelIdle;

		public State orientToPath;

		public State move;

		public State orientToIdle;
	}

	public class StatesInstance : GameInstance
	{
		public ClusterGridEntity entity;

		private float simpleAngle;

		public StatesInstance(ClusterMapVisualizer master, ClusterGridEntity entity)
			: base(master)
		{
			this.entity = entity;
			base.sm.entityTarget.Set(entity, this);
		}

		public void PlayVisAnim(string animName, KAnim.PlayMode playMode)
		{
			ClusterMapVisualizer component = GetComponent<ClusterMapVisualizer>();
			component.PlayAnim(animName, playMode);
		}

		public bool MoveTowards(Vector3 targetPosition, float dt)
		{
			RectTransform component = GetComponent<RectTransform>();
			ClusterMapVisualizer component2 = GetComponent<ClusterMapVisualizer>();
			Vector3 localPosition = component.GetLocalPosition();
			Vector3 vector = targetPosition - localPosition;
			Vector3 normalized = vector.normalized;
			float magnitude = vector.magnitude;
			float num = TuningData<Tuning>.Get().visualizerTransitionSpeed * dt;
			if (num < magnitude)
			{
				Vector3 b = normalized * num;
				component.SetLocalPosition(localPosition + b);
				component2.RefreshPathDrawing();
				return false;
			}
			component.SetLocalPosition(targetPosition);
			component2.RefreshPathDrawing();
			return true;
		}

		public bool RotateTowards(float targetAngle, float dt)
		{
			ClusterMapVisualizer component = GetComponent<ClusterMapVisualizer>();
			float num = targetAngle - simpleAngle;
			if (num > 180f)
			{
				num -= 360f;
			}
			else if (num < -180f)
			{
				num += 360f;
			}
			float num2 = TuningData<Tuning>.Get().visualizerRotationSpeed * dt;
			if (num > 0f && num2 < num)
			{
				simpleAngle += num2;
				component.SetAnimRotation(simpleAngle);
				return false;
			}
			if (num < 0f && 0f - num2 > num)
			{
				simpleAngle -= num2;
				component.SetAnimRotation(simpleAngle);
				return false;
			}
			simpleAngle = targetAngle;
			component.SetAnimRotation(simpleAngle);
			return true;
		}
	}

	public State idle;

	public State repositioning;

	public State takeoff;

	public State landing;

	public TravelingStates traveling;

	public TargetParameter entityTarget;

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = idle;
		root.OnTargetLost(entityTarget, null);
		idle.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		}).Target(entityTarget).EventHandlerTransition(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, repositioning, ClusterChangedAtMyLocation)
			.EventTransition(GameHashes.ClusterDestinationChanged, traveling, IsTraveling)
			.Target(masterTarget);
		takeoff.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("takeoff", KAnim.PlayMode.Once);
		});
		landing.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("landing", KAnim.PlayMode.Once);
		});
		repositioning.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		}).Transition(idle, DoReposition, UpdateRate.RENDER_EVERY_TICK);
		traveling.DefaultState(traveling.orientToPath).Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("inflight_loop", KAnim.PlayMode.Loop);
		});
		traveling.travelIdle.Target(entityTarget).EventHandlerTransition(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, repositioning, ClusterChangedAtMyLocation).EventTransition(GameHashes.ClusterDestinationChanged, traveling.orientToIdle, GameStateMachine<ClusterMapTravelAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(IsTraveling))
			.EventTransition(GameHashes.ClusterDestinationChanged, traveling.orientToPath, GameStateMachine<ClusterMapTravelAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(DoOrientToPath))
			.EventTransition(GameHashes.ClusterLocationChanged, traveling.move, GameStateMachine<ClusterMapTravelAnimator, StatesInstance, ClusterMapVisualizer, object>.Not(DoMove))
			.Target(masterTarget);
		traveling.orientToPath.Transition(traveling.travelIdle, DoOrientToPath, UpdateRate.RENDER_EVERY_TICK).Target(entityTarget).EventHandlerTransition(GameHashes.ClusterLocationChanged, (StatesInstance smi) => Game.Instance, repositioning, ClusterChangedAtMyLocation)
			.Target(masterTarget);
		traveling.move.Transition(traveling.travelIdle, DoMove, UpdateRate.RENDER_EVERY_TICK);
		traveling.orientToIdle.Enter(delegate(StatesInstance smi)
		{
			smi.PlayVisAnim("idle_loop", KAnim.PlayMode.Loop);
		}).Transition(idle, DoOrientToIdle, UpdateRate.RENDER_EVERY_TICK);
	}

	private bool IsTraveling(StatesInstance smi)
	{
		return smi.entity.GetComponent<ClusterTraveler>().IsTraveling();
	}

	private bool DoReposition(StatesInstance smi)
	{
		Vector3 position = ClusterGrid.Instance.GetPosition(smi.entity);
		return smi.MoveTowards(position, Time.unscaledDeltaTime);
	}

	private bool DoMove(StatesInstance smi)
	{
		Vector3 position = ClusterGrid.Instance.GetPosition(smi.entity);
		return smi.MoveTowards(position, Time.unscaledDeltaTime);
	}

	private bool DoOrientToPath(StatesInstance smi)
	{
		float pathAngle = smi.GetComponent<ClusterMapVisualizer>().GetPathAngle();
		return smi.RotateTowards(pathAngle, Time.unscaledDeltaTime);
	}

	private bool DoOrientToIdle(StatesInstance smi)
	{
		return smi.RotateTowards(0f, Time.unscaledDeltaTime);
	}

	private bool ClusterChangedAtMyLocation(StatesInstance smi, object data)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = (ClusterLocationChangedEvent)data;
		return clusterLocationChangedEvent.oldLocation == smi.entity.Location || clusterLocationChangedEvent.newLocation == smi.entity.Location;
	}
}
