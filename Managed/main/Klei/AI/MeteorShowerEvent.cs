using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	public class MeteorShowerEvent : GameplayEvent<MeteorShowerEvent.StatesInstance>
	{
		private struct BombardmentInfo
		{
			public string prefab;

			public float weight;
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, MeteorShowerEvent>
		{
			public class RunningStates : State
			{
				public State bombarding;

				public State snoozing;
			}

			public State planning;

			public RunningStates running;

			public State finished;

			public FloatParameter runTimeRemaining;

			public FloatParameter bombardTimeRemaining;

			public FloatParameter snoozeTimeRemaining;

			public override void InitializeStates(out BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = planning;
				base.serializable = SerializeType.Both_DEPRECATED;
				planning.Enter(delegate(StatesInstance smi)
				{
					runTimeRemaining.Set(smi.gameplayEvent.duration, smi);
					bombardTimeRemaining.Set(smi.gameplayEvent.secondsBombardmentOn.Get(), smi);
					snoozeTimeRemaining.Set(smi.gameplayEvent.secondsBombardmentOff.Get(), smi);
				}).GoTo(running);
				running.DefaultState(running.snoozing).Update(delegate(StatesInstance smi, float dt)
				{
					runTimeRemaining.Delta(0f - dt, smi);
				}).ParamTransition(runTimeRemaining, finished, GameStateMachine<States, StatesInstance, GameplayEventManager, object>.IsLTEZero);
				running.bombarding.Enter(delegate(StatesInstance smi)
				{
					smi.StartBackgroundEffects();
				}).Exit(delegate(StatesInstance smi)
				{
					smi.StopBackgroundEffects();
				}).Exit(delegate(StatesInstance smi)
				{
					bombardTimeRemaining.Set(smi.gameplayEvent.secondsBombardmentOn.Get(), smi);
				})
					.Update(delegate(StatesInstance smi, float dt)
					{
						bombardTimeRemaining.Delta(0f - dt, smi);
					})
					.ParamTransition(bombardTimeRemaining, running.snoozing, GameStateMachine<States, StatesInstance, GameplayEventManager, object>.IsLTEZero)
					.Update(delegate(StatesInstance smi, float dt)
					{
						smi.Bombarding(dt);
					});
				running.snoozing.Exit(delegate(StatesInstance smi)
				{
					snoozeTimeRemaining.Set(smi.gameplayEvent.secondsBombardmentOff.Get(), smi);
				}).Update(delegate(StatesInstance smi, float dt)
				{
					snoozeTimeRemaining.Delta(0f - dt, smi);
				}).ParamTransition(snoozeTimeRemaining, running.bombarding, GameStateMachine<States, StatesInstance, GameplayEventManager, object>.IsLTEZero);
				finished.ReturnSuccess();
			}
		}

		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, MeteorShowerEvent>.GameplayEventStateMachineInstance
		{
			public GameObject activeMeteorBackground;

			[Serialize]
			private float nextMeteorTime;

			[Serialize]
			private float timeRemaining;

			[Serialize]
			private float timeBetweenMeteors;

			[Serialize]
			private int m_worldId;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, MeteorShowerEvent meteorShowerEvent)
				: base(master, eventInstance, meteorShowerEvent)
			{
				timeRemaining = gameplayEvent.duration;
				timeBetweenMeteors = gameplayEvent.secondsPerMeteor;
				m_worldId = eventInstance.worldId;
			}

			public override void StopSM(string reason)
			{
				StopBackgroundEffects();
				base.StopSM(reason);
			}

			public void StartBackgroundEffects()
			{
				if (activeMeteorBackground == null)
				{
					activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground);
					WorldContainer world = ClusterManager.Instance.GetWorld(m_worldId);
					float x = (world.maximumBounds.x + world.minimumBounds.x) / 2f;
					float y = world.maximumBounds.y;
					float z = 25f;
					activeMeteorBackground.transform.SetPosition(new Vector3(x, y, z));
					activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
				}
			}

			public void StopBackgroundEffects()
			{
				if (activeMeteorBackground != null)
				{
					ParticleSystem component = activeMeteorBackground.GetComponent<ParticleSystem>();
					ParticleSystem.MainModule main = component.main;
					main.stopAction = ParticleSystemStopAction.Destroy;
					component.Stop();
					if (!component.IsAlive())
					{
						Object.Destroy(activeMeteorBackground);
					}
					activeMeteorBackground = null;
				}
			}

			public float TimeUntilNextShower()
			{
				if (IsInsideState(base.sm.running.bombarding))
				{
					return 0f;
				}
				return base.sm.snoozeTimeRemaining.Get(this);
			}

			public void Bombarding(float dt)
			{
				nextMeteorTime -= dt;
				while (nextMeteorTime < 0f)
				{
					DoBombardment(gameplayEvent.bombardmentInfo);
					nextMeteorTime += timeBetweenMeteors;
				}
			}

			private void DoBombardment(List<BombardmentInfo> bombardment_info)
			{
				float num = 0f;
				foreach (BombardmentInfo item in bombardment_info)
				{
					num += item.weight;
				}
				num = Random.Range(0f, num);
				BombardmentInfo bombardmentInfo = bombardment_info[0];
				int num2 = 0;
				while (num - bombardmentInfo.weight > 0f)
				{
					num -= bombardmentInfo.weight;
					bombardmentInfo = bombardment_info[++num2];
				}
				Game.Instance.Trigger(-84771526);
				SpawnBombard(bombardmentInfo.prefab);
			}

			private GameObject SpawnBombard(string prefab)
			{
				WorldContainer world = ClusterManager.Instance.GetWorld(m_worldId);
				float x = (float)world.Width * Random.value + (float)world.WorldOffset.x;
				float y = world.Height + world.WorldOffset.y - 1;
				float layerZ = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				GameObject obj = Util.KInstantiate(position: new Vector3(x, y, layerZ), original: Assets.GetPrefab(prefab), rotation: Quaternion.identity);
				obj.SetActive(value: true);
				return obj;
			}
		}

		private List<BombardmentInfo> bombardmentInfo;

		private MathUtil.MinMax secondsBombardmentOff;

		private MathUtil.MinMax secondsBombardmentOn;

		private float secondsPerMeteor = 0.33f;

		private float duration;

		public MeteorShowerEvent(string id, float duration, float secondsPerMeteor, MathUtil.MinMax secondsBombardmentOff = default(MathUtil.MinMax), MathUtil.MinMax secondsBombardmentOn = default(MathUtil.MinMax))
			: base(id, 0, 0)
		{
			this.duration = duration;
			this.secondsPerMeteor = secondsPerMeteor;
			this.secondsBombardmentOff = secondsBombardmentOff;
			this.secondsBombardmentOn = secondsBombardmentOn;
			bombardmentInfo = new List<BombardmentInfo>();
			tags.Add(GameTags.SpaceDanger);
		}

		public MeteorShowerEvent AddMeteor(string prefab, float weight)
		{
			bombardmentInfo.Add(new BombardmentInfo
			{
				prefab = prefab,
				weight = weight
			});
			return this;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
