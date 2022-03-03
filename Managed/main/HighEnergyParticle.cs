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
		HighEnergyParticle,
		CaptureAndRelease,
		PassThrough
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

		public State catchAndRelease;

		public Signal destroySignal;

		public Signal destroySimpleSignal;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = ready.pre;
			base.serializable = SerializeType.Both_DEPRECATED;
			ready.OnSignal(destroySimpleSignal, destroying.instant).OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Creature).OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Minion)
				.OnSignal(destroySignal, destroying.explode, (StatesInstance smi) => smi.master.collision == CollisionType.Solid)
				.OnSignal(destroySignal, destroying.blackhole, (StatesInstance smi) => smi.master.collision == CollisionType.HighEnergyParticle)
				.OnSignal(destroySignal, destroying.captured, (StatesInstance smi) => smi.master.collision == CollisionType.Captured)
				.OnSignal(destroySignal, catchAndRelease, (StatesInstance smi) => smi.master.collision == CollisionType.CaptureAndRelease)
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
			catchAndRelease.Enter(delegate(StatesInstance smi)
			{
				smi.master.collision = CollisionType.None;
			}).PlayAnim("explode", KAnim.PlayMode.Once).OnAnimQueueComplete(ready.pre);
			destroying.Enter(delegate(StatesInstance smi)
			{
				smi.master.isCollideable = false;
				smi.master.StopLoopingSound();
			});
			destroying.instant.Enter(delegate(StatesInstance smi)
			{
				Object.Destroy(smi.master.gameObject);
			});
			destroying.explode.PlayAnim("explode").Enter(delegate(StatesInstance smi)
			{
				EmitRemainingPayload(smi);
			});
			destroying.blackhole.PlayAnim("collision").Enter(delegate(StatesInstance smi)
			{
				EmitRemainingPayload(smi);
			});
			destroying.captured.PlayAnim("travel_pst").OnAnimQueueComplete(destroying.instant).Enter(delegate(StatesInstance smi)
			{
				smi.master.emitter.SetEmitting(emitting: false);
			});
		}

		private void EmitRemainingPayload(StatesInstance smi)
		{
			smi.master.GetComponent<KBatchedAnimController>().GetCurrentAnim();
			smi.master.emitter.emitRadiusX = 6;
			smi.master.emitter.emitRadiusY = 6;
			smi.master.emitter.emitRads = smi.master.payload * 0.5f * 600f / 9f;
			smi.master.emitter.Refresh();
			SimMessages.AddRemoveSubstance(Grid.PosToCell(smi.master.gameObject), SimHashes.Fallout, CellEventLogger.Instance.ElementEmitted, smi.master.payload * 0.001f, 5000f, Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.FloorToInt(smi.master.payload * 0.5f / 0.01f));
			smi.Schedule(1f, delegate
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
	public CollisionType collision;

	[Serialize]
	public HighEnergyParticlePort capturedBy;

	public short emitRadius;

	public float emitRate;

	public float emitSpeed;

	private LoopingSounds loopingSounds;

	public string flyingSound;

	public bool isCollideable;

	protected override void OnPrefabInit()
	{
		loopingSounds = base.gameObject.GetComponent<LoopingSounds>();
		flyingSound = GlobalAssets.GetSound("Radbolt_travel_LP");
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.HighEnergyParticles.Add(this);
		GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.HighEnergyParticleCount, base.gameObject);
		emitter.SetEmitting(emitting: false);
		emitter.Refresh();
		SetDirection(direction);
		base.gameObject.layer = LayerMask.NameToLayer("PlaceWithDepth");
		StartLoopingSound();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		StopLoopingSound();
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
		GameObject gameObject = new GameObject("HEPcollideFX");
		gameObject.SetActive(value: false);
		gameObject.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(base.smi.master.transform.position), Grid.SceneLayer.FXFront));
		KBatchedAnimController fxAnim = gameObject.AddComponent<KBatchedAnimController>();
		fxAnim.AnimFiles = new KAnimFile[1] { Assets.GetAnim("hep_impact_kanim") };
		fxAnim.initialAnim = "graze";
		gameObject.SetActive(value: true);
		switch (collisionType)
		{
		case CollisionType.CaptureAndRelease:
			fxAnim.Play("partial");
			break;
		case CollisionType.Captured:
			fxAnim.Play("full");
			break;
		case CollisionType.PassThrough:
			fxAnim.Play("graze");
			break;
		}
		fxAnim.onAnimComplete += delegate
		{
			Util.KDestroyGameObject(fxAnim);
		};
		if (collisionType == CollisionType.PassThrough)
		{
			collision = CollisionType.None;
		}
		else
		{
			base.smi.sm.destroySignal.Trigger(base.smi);
		}
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
		else
		{
			capturedBy = null;
			Collide(CollisionType.CaptureAndRelease);
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
			gameObject.GetComponent<Operational>();
			HighEnergyParticlePort component = gameObject.GetComponent<HighEnergyParticlePort>();
			if (component != null)
			{
				Vector2 pos = Grid.CellToPosCCC(component.GetHighEnergyParticleInputPortPosition(), Grid.SceneLayer.NoLayer);
				if (GetComponent<KCircleCollider2D>().Intersects(pos))
				{
					if (component.InputActive() && component.AllowCapture(this))
					{
						Capture(component);
						return;
					}
					Collide(CollisionType.PassThrough);
				}
			}
		}
		KCircleCollider2D component2 = GetComponent<KCircleCollider2D>();
		int x = 0;
		int y = 0;
		Grid.CellToXY(cell, out x, out y);
		ListPool<ScenePartitionerEntry, HighEnergyParticle>.PooledList pooledList = ListPool<ScenePartitionerEntry, HighEnergyParticle>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x - 1, y - 1, 3, 3, GameScenePartitioner.Instance.collisionLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			KCollider2D kCollider2D = item.obj as KCollider2D;
			HighEnergyParticle component3 = kCollider2D.gameObject.GetComponent<HighEnergyParticle>();
			if (!(component3 == null) && !(component3 == this) && component3.isCollideable)
			{
				bool num = component2.Intersects(component3.transform.position);
				bool flag = kCollider2D.Intersects(base.transform.position);
				if (num && flag)
				{
					payload += component3.payload;
					component3.DestroyNow();
					Collide(CollisionType.HighEnergyParticle);
					return;
				}
			}
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
					KPrefabID component4 = gameObject3.GetComponent<KPrefabID>();
					Health component5 = gameObject2.GetComponent<Health>();
					if (component5 != null && component4 != null && component4.HasTag(GameTags.Creature) && !component5.IsDefeated())
					{
						component5.Damage(20f);
						Collide(CollisionType.Creature);
						return;
					}
				}
			}
		}
		GameObject gameObject4 = Grid.Objects[cell, 0];
		if (gameObject4 != null)
		{
			Health component6 = gameObject4.GetComponent<Health>();
			if (component6 != null && !component6.IsDefeated() && !gameObject4.HasTag(GameTags.Dead) && !gameObject4.HasTag(GameTags.Dying))
			{
				component6.Damage(20f);
				WoundMonitor.Instance sMI = gameObject4.GetSMI<WoundMonitor.Instance>();
				if (sMI != null && !component6.IsDefeated())
				{
					sMI.PlayKnockedOverImpactAnimation();
				}
				gameObject4.GetComponent<PrimaryElement>().AddDisease(Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id), Mathf.FloorToInt(payload * 0.5f / 0.01f), "HEPImpact");
				Collide(CollisionType.Minion);
				return;
			}
		}
		if (Grid.IsSolidCell(cell))
		{
			GameObject gameObject5 = Grid.Objects[cell, 9];
			if (gameObject5 == null || !gameObject5.HasTag(GameTags.HEPPassThrough) || capturedBy == null || capturedBy.gameObject != gameObject5)
			{
				Collide(CollisionType.Solid);
			}
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
		SaveGame.Instance.GetComponent<ColonyAchievementTracker>().radBoltTravelDistance += speed * dt;
		loopingSounds.UpdateVelocity(flyingSound, vector - position);
		if (!Grid.IsValidCell(num2))
		{
			Debug.LogWarning("High energy particle moved into invalid cell and is destroyed with no radiation");
			base.smi.sm.destroySimpleSignal.Trigger(base.smi);
			return;
		}
		if (num != num2)
		{
			payload -= 0.1f;
			byte index = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id);
			int disease_delta = Mathf.FloorToInt(5f);
			SimMessages.ModifyDiseaseOnCell(num2, index, disease_delta);
		}
		if (!(payload > 0f))
		{
			base.smi.sm.destroySimpleSignal.Trigger(base.smi);
		}
		base.transform.SetPosition(vector);
	}

	private void StartLoopingSound()
	{
		loopingSounds.StartSound(flyingSound);
	}

	private void StopLoopingSound()
	{
		loopingSounds.StopSound(flyingSound);
	}
}
