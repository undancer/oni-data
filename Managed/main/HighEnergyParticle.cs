using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HighEnergyParticle : StateMachineComponent<HighEnergyParticle.StatesInstance>
{
	public enum CollisionType
	{
		None,
		Solid,
		Creature,
		Minion,
		Captured,
		HighEnergyParticle
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, HighEnergyParticle, object>.GameInstance
	{
		public StatesInstance(HighEnergyParticle smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, HighEnergyParticle>
	{
		public class ReadyStates : State
		{
			public State pre;

			public State moving;
		}

		public class DestructionStates : State
		{
			public State instant;

			public State explode;

			public State captured;

			public State blackhole;
		}

		public ReadyStates ready;

		public DestructionStates destroying;

		public Signal destroySignal;

		public Signal destroySimpleSignal;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = ready.pre;
			base.serializable = SerializeType.Both_DEPRECATED;
			ready.PlayAnim("travel_loop", KAnim.PlayMode.Loop).OnSignal(destroySimpleSignal, destroying.instant).OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Creature)
				.OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Minion)
				.OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Solid)
				.OnSignal(destroySignal, destroying.blackhole, (StatesInstance smi) => smi.master.collision == CollisionType.HighEnergyParticle)
				.OnSignal(destroySignal, destroying.captured, (StatesInstance smi) => smi.master.collision == CollisionType.Captured)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.emitter.SetEmitting(emitting: true);
					smi.master.isCollideable = true;
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.MovingUpdate(dt);
					smi.master.CheckCollision();
				}, UpdateRate.SIM_EVERY_TICK);
			ready.pre.PlayAnim("travel_pre").OnAnimQueueComplete(ready.moving);
			ready.moving.PlayAnim("travel_loop", KAnim.PlayMode.Loop);
			destroying.Enter(delegate(StatesInstance smi)
			{
				smi.master.isCollideable = false;
			});
			destroying.instant.Enter(delegate(StatesInstance smi)
			{
				Object.Destroy(smi.master.gameObject);
			});
			destroying.explode.PlayAnim("explode").Enter(delegate(StatesInstance smi)
			{
				EmitRemainingPayload(smi, smi.master.explodeEmitDurration, smi.master.explodeEmitRadius);
			});
			destroying.blackhole.PlayAnim("collision").Enter(delegate(StatesInstance smi)
			{
				EmitRemainingPayload(smi, smi.master.blackholeEmitDurration, smi.master.blackholeEmitRadius);
			});
			destroying.captured.PlayAnim("travel_pst").OnAnimQueueComplete(destroying.instant).Enter(delegate(StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(emitting: false);
			});
		}

		private void EmitRemainingPayload(StatesInstance smi, float emitDurration, short emitRadius)
		{
			KAnim.Anim currentAnim = smi.master.GetComponent<KBatchedAnimController>().GetCurrentAnim();
			if (emitDurration < currentAnim.totalTime)
			{
				DebugUtil.LogErrorArgs("HEP anim is shorter than emitDurration, change the animation to match.");
			}
			smi.master.emitter.emitRate = emitDurration;
			smi.master.emitter.emitRadiusX = emitRadius;
			smi.master.emitter.emitRadiusY = emitRadius;
			smi.master.emitter.emitRads = smi.master.payload;
			smi.master.emitter.Refresh();
			smi.Schedule(emitDurration, delegate
			{
				Object.Destroy(smi.master.gameObject);
			});
		}
	}

	[Serialize]
	private EightDirection direction;

	[Serialize]
	public float speed;

	[Serialize]
	public float payload;

	[MyCmpReq]
	private RadiationEmitter emitter;

	[Serialize]
	public float perCellFalloff;

	[Serialize]
	public float radioactiveRatio;

	[Serialize]
	public CollisionType collision;

	[Serialize]
	public HighEnergyParticlePort capturedBy;

	public short emitRadius;

	public float emitRate;

	public float emitSpeed;

	public short explodeEmitRadius;

	public float explodeEmitDurration;

	public short blackholeEmitRadius;

	public float blackholeEmitDurration;

	public bool isCollideable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.HighEnergyParticles.Add(this);
		KSelectable component = GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().MiscStatusItems.HighEnergyParticleCount, base.gameObject);
		emitter.SetEmitting(emitting: false);
		emitter.Refresh();
		SetDirection(direction);
		base.gameObject.layer = LayerMask.NameToLayer("PlaceWithDepth");
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.HighEnergyParticles.Remove(this);
		if (capturedBy != null && capturedBy.currentParticle == this)
		{
			capturedBy.currentParticle = null;
		}
	}

	public void SetDirection(EightDirection direction)
	{
		this.direction = direction;
		float angle = EightDirectionUtil.GetAngle(direction);
		base.smi.master.transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}

	public void Collide(CollisionType collisionType)
	{
		collision = collisionType;
		base.smi.sm.destroySignal.Trigger(base.smi);
	}

	public void DestroyNow()
	{
		base.smi.sm.destroySimpleSignal.Trigger(base.smi);
	}

	private void Capture(HighEnergyParticlePort input)
	{
		if (input.currentParticle != null)
		{
			DebugUtil.LogArgs("Particle was backed up and caused an explosion!");
			base.smi.sm.destroySignal.Trigger(base.smi);
			return;
		}
		capturedBy = input;
		input.currentParticle = this;
		input.Capture(this);
		if (input.currentParticle == this)
		{
			input.currentParticle = null;
			capturedBy = null;
			Collide(CollisionType.Captured);
		}
	}

	public void Uncapture()
	{
		if (capturedBy != null)
		{
			capturedBy.currentParticle = null;
		}
		capturedBy = null;
	}

	public void CheckCollision()
	{
		if (collision != 0)
		{
			return;
		}
		int cell = Grid.PosToCell(base.smi.master.transform.GetPosition());
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			Operational component = gameObject.GetComponent<Operational>();
			HighEnergyParticlePort component2 = gameObject.GetComponent<HighEnergyParticlePort>();
			if (component2 != null && component2.InputActive())
			{
				Vector2 pos = Grid.CellToPosCCC(component2.GetHighEnergyParticleInputPortPosition(), Grid.SceneLayer.NoLayer);
				if (GetComponent<KCircleCollider2D>().Intersects(pos) && component2.AllowCapture(this))
				{
					Capture(component2);
					return;
				}
			}
		}
		KCircleCollider2D component3 = GetComponent<KCircleCollider2D>();
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		ListPool<ScenePartitionerEntry, HighEnergyParticle>.PooledList pooledList = ListPool<ScenePartitionerEntry, HighEnergyParticle>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x - 1, y - 1, x + 1, y + 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			KCollider2D kCollider2D = item.obj as KCollider2D;
			HighEnergyParticle component4 = kCollider2D.gameObject.GetComponent<HighEnergyParticle>();
			if (!(component4 != null) || !component3.Intersects(component4.transform.position) || !kCollider2D.Intersects(base.transform.position) || component4 == this || !component4.isCollideable)
			{
				continue;
			}
			payload += component4.payload;
			component4.DestroyNow();
			Collide(CollisionType.HighEnergyParticle);
			return;
		}
		pooledList.Recycle();
		GameObject gameObject2 = Grid.Objects[cell, 3];
		if (gameObject2 != null)
		{
			ObjectLayerListItem objectLayerListItem = gameObject2.GetComponent<Pickupable>().objectLayerListItem;
			while (objectLayerListItem != null)
			{
				GameObject gameObject3 = objectLayerListItem.gameObject;
				objectLayerListItem = objectLayerListItem.nextItem;
				if (!(gameObject3 == null))
				{
					KPrefabID component5 = gameObject3.GetComponent<KPrefabID>();
					Health component6 = gameObject2.GetComponent<Health>();
					if (component6 != null && component5 != null && component5.HasTag(GameTags.Creature) && !component6.IsDefeated())
					{
						component6.Damage(20f);
						Collide(CollisionType.Creature);
						return;
					}
				}
			}
		}
		GameObject gameObject4 = Grid.Objects[cell, 0];
		if (gameObject4 != null)
		{
			Health component7 = gameObject4.GetComponent<Health>();
			if (component7 != null && !component7.IsDefeated())
			{
				if (component7.CanBeIncapacitated)
				{
					component7.Incapacitate(GameTags.HitByHighEnergyParticle);
				}
				Collide(CollisionType.Minion);
				return;
			}
		}
		if (Grid.IsSolidCell(cell))
		{
			Collide(CollisionType.Solid);
		}
	}

	public void MovingUpdate(float dt)
	{
		if (collision != 0)
		{
			return;
		}
		Vector3 position = base.transform.GetPosition();
		int num = Grid.PosToCell(position);
		Vector3 vector = position + EightDirectionUtil.GetNormal(direction) * speed * dt;
		int num2 = Grid.PosToCell(vector);
		if (!Grid.IsValidCell(num2))
		{
			Debug.LogWarning("High energy particle moved into invalid cell and is destroyed with no radiation");
			base.smi.sm.destroySimpleSignal.Trigger(base.smi);
			return;
		}
		if (num != num2)
		{
			payload -= perCellFalloff;
		}
		if (payload > 0f)
		{
			emitter.emitRads = payload * radioactiveRatio;
			emitter.Refresh();
		}
		else
		{
			base.smi.sm.destroySimpleSignal.Trigger(base.smi);
		}
		base.transform.SetPosition(vector);
	}
}
